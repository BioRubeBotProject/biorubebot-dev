using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaMovement : MonoBehaviour
{
    public  float      speed;
    public  GameObject targetObject;//AdenylylCyclase-A
    public  float      gtpActiveTimeMax;
    private GameObject cellMembrane;
    private GameObject closestTarget;
    private bool       isDockedAtCyclase;
    private bool       targetFound;
    private bool       hasGdpAttached;
    private bool       hasGtpAttached;
    private bool       doFindParent;
    private string     rotationDirection;
    private float      activeStart = 0.0f;

    private void Start()
    {
        Transform doc = null;

        cellMembrane      = GameObject.FindGameObjectWithTag("CellMembrane");
        closestTarget     = null;
        targetFound       = false;
        rotationDirection = null;
        doFindParent      = false;
    }

    GameObject getDockStation()
    {
        GameObject doc   = null;
        bool       found = false;

        foreach(Transform child in transform)
        {
            if(child.gameObject.name == "docStation")
            {
                doc   = child.gameObject;
                found = true;
                break;
            }
        }
        if(!found)
            doc = null;

        return doc;
    }

    GameObject getGtpChild()
    {
        GameObject objGtp = null;
        GameObject doc    = getDockStation();
        bool       found  = false;

        if(null != doc)
        {
            print("Found doc");
            foreach(Transform child in doc.transform)
            {
                print(child.gameObject.name);
                if(child.gameObject.name == "GTP(Clone)")
                {
                    objGtp = child.gameObject;
                    found  = true;
                    break;
                }
            }
        }
        if(!found)
            objGtp = null;

        return objGtp;
    }

    void dropGtp()
    {
        GameObject childGtp = getGtpChild();

        if(null != childGtp)
        {
            childGtp.tag   = "ReleasedGTP";
            hasGtpAttached = false;
        }
        else
            print("Could not get the Child GTP");
    }

    public void Update()
    {
        Transform doc = null;

        if(Time.timeScale != 0)//if we are not paused
        {
            if(transform.gameObject.GetComponent<ActivationProperties>().isActive)
            {
                if(0.0f == activeStart)//we just became active
                {
                    hasGtpAttached = true;//we have a GTP attached
                    activeStart    = Time.timeSinceLevelLoad;
                }

                if(!isDockedAtCyclase)//if we aren't bound to a GPCR
                {
                    closestTarget = findClosestTarget();
                    if(null != closestTarget)
                    {
                        rotationDirection = getRotationDirection();

                        if(rotationDirection == "right")
                            transform.RotateAround(cellMembrane.transform.position, Vector3.back, speed * Time.deltaTime);
                        else if(rotationDirection == "left")
                            transform.RotateAround(cellMembrane.transform.position, Vector3.forward, speed * Time.deltaTime);
                    }
                }

                if(Time.timeSinceLevelLoad > activeStart + gtpActiveTimeMax)
                    doFindParent = true;
            }
            if(doFindParent)
            {
                if(transform.gameObject.GetComponent<ActivationProperties>().isActive)
                    transform.gameObject.GetComponent<ActivationProperties>().isActive = false;
                if(hasGtpAttached)
                    dropGtp();
            }
        }
    }

    /*  Function:   onTriggerEnter2D(Collider2D)
        Purpose:    handles the event that we collided with an AdenylylCyclase,
                    setting the Cyclase's isActive property true and setting our
                    global isDockedAtCyclase variable true
    */
    private void OnTriggerEnter2D(Collider2D other)
	{
        if(other.gameObject.tag == "AdenylylCyclase" && transform.name == "alpha")
        {
            isDockedAtCyclase = true;
            other.gameObject.GetComponent<ActivationProperties>().isActive = true;
            //check if action is a win condition for the scene/level
            if(GameObject.FindWithTag("Win_Alpha_Binds_to_Cyclase"))
                WinScenario.dropTag("Win_Alpha_Binds_to_Cyclase");
        }
    }

    /*  Function:   getRotationDirection() string
        Purpose:    this function determines the rotation direction around the
                    Cell Membrane wall depending on whether it would be quicker
                    to get to the target Object by going left or right.
        Return:     left or right
    */
    private string getRotationDirection()
    {       
        //Find rotation direction given closest object
        var    currentRotation = transform.eulerAngles;
        var    targetRotation  = closestTarget.transform.eulerAngles;
        string strDir          = null;

        float direction = (((targetRotation.z - currentRotation.z) + 360f) % 360f) > 180.0f ? -1 : 1;

        if(direction == -1)
            strDir = "right";
        else
            strDir = "left";

        return strDir;
    }

    /*  Function:   findClosestTarget()
        Purpose:    this function locates the closest Object with the same tag
                    as the global variable targetObject, which is set to
                    AdenylylCyclase
        Return:     the closest inactive AdenylylCyclase
    */
    private GameObject findClosestTarget()
    {
        GameObject[] targets  = null;
        GameObject   target   = null;
        float        distance = 0;
        Vector3      position;//TG-Protein position

        targets  = GameObject.FindGameObjectsWithTag(targetObject.tag);
        distance = Mathf.Infinity;
        position = transform.position;

        //for each GPCR in our targets list
        foreach(GameObject cyclase in targets)
        {
            if(cyclase.GetComponent<ActivationProperties>().isActive)
                continue;

            Vector3 diff        = cyclase.transform.position - position;
            float   curDistance = diff.sqrMagnitude;
            if(curDistance < distance)
            {
                target      = cyclase;
                distance    = curDistance;
                targetFound = true;
            }
        }
        if(!targetFound)
            target = null;

        return target;
    }
}
