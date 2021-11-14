using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaMovement : MonoBehaviour
{
    public  bool       doFindBetaGamma;
    public  GameObject targetObject;   //AdenylylCyclase-A
    public  GameObject targetBetaGamma;//parent GTProtien
    public  GameObject GDP;
    public  float      speed;
    public  float      gtpActiveTimeMax;

    private GameObject inactiveCyclase;
    private GameObject cellMembrane;
    private GameObject closestTarget;
    private bool       isDockedAtCyclase;
    private bool       targetFound;
    private bool       hasGdpAttached;
    private bool       hasGtpAttached;
    private string     rotationDirection;
    private float      activeStart = 0.0f;

    private void Start()
    {
        Transform doc = null;

        cellMembrane      = GameObject.FindGameObjectWithTag("CellMembrane");
        closestTarget     = null;
        targetFound       = false;
        rotationDirection = null;
        doFindBetaGamma   = false;
    }

    private GameObject getDoc()
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

    private GameObject getGtpChild()
    {
        GameObject objGtp = null;
        GameObject doc    = getDoc();
        bool       found  = false;

        if(null != doc)
        {
            foreach(Transform child in doc.transform)
            {
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

    private IEnumerator spawnGdp()
    {
        GameObject doc      = null;
        GameObject childGdp = null;

        yield return new WaitForSeconds(2.5f);
        childGdp = (GameObject)Instantiate(GDP, transform.position, Quaternion.identity);
        childGdp.transform.SetParent(this.transform);

        doc = getDoc();
        if(null != doc)
            childGdp.transform.position = doc.transform.position;

        childGdp.GetComponent<CircleCollider2D> ().enabled = false;
        childGdp.GetComponent<Rigidbody2D> ().isKinematic  = true;
        hasGdpAttached = true;
    }

    private void dropGtp()
    {
        GameObject childGtp = getGtpChild();

        if(null != childGtp)
        {
            childGtp.transform.tag = "ReleasedGTP";
            hasGtpAttached         = false;
        }
    }

    private void seekBetaGamma()
    {
        if(null != targetBetaGamma)
        {
            rotationDirection = getRotationDirection(targetBetaGamma);

            if(rotationDirection == "right")
                transform.RotateAround(cellMembrane.transform.position, Vector3.back, speed * Time.deltaTime);
            else if(rotationDirection == "left")
                transform.RotateAround(cellMembrane.transform.position, Vector3.forward, speed * Time.deltaTime);
        }
    }

    private void setAdenylylCyclaseInactive()
    {
        GameObject activeCyclase = Roam.FindClosest(transform, "ATP_tracking");

        if(null != inactiveCyclase)
        {
            inactiveCyclase.SetActive(true);
            inactiveCyclase.GetComponent<ActivationProperties>().isActive = false;
        }
        if(null != activeCyclase)
        {
            activeCyclase.GetComponent<ActivationProperties>().isActive = false;
            activeCyclase.SetActive(false);
        }
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
                        rotationDirection = getRotationDirection(closestTarget);

                        if(rotationDirection == "right")
                            transform.RotateAround(cellMembrane.transform.position, Vector3.back, speed * Time.deltaTime);
                        else if(rotationDirection == "left")
                            transform.RotateAround(cellMembrane.transform.position, Vector3.forward, speed * Time.deltaTime);
                    }
                }

                if(Time.timeSinceLevelLoad > activeStart + gtpActiveTimeMax)
                    doFindBetaGamma = true;
            }

            if(doFindBetaGamma)
            {
                if(transform.gameObject.GetComponent<ActivationProperties>().isActive)
                    transform.gameObject.GetComponent<ActivationProperties>().isActive = false;
                if(hasGtpAttached)
                {
                    dropGtp();
                    setAdenylylCyclaseInactive();
                    StartCoroutine(spawnGdp());
                    activeStart       = 0.0f;
                    isDockedAtCyclase = false;
                }
                else if(hasGdpAttached)
                {
                    seekBetaGamma();
                }
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
            if(!doFindBetaGamma)
            {
                other.gameObject.GetComponent<ActivationProperties>().isActive = true;
                inactiveCyclase   = other.gameObject;
                isDockedAtCyclase = true;
                //check if action is a win condition for the scene/level
                if(GameObject.FindWithTag("Win_Alpha_Binds_to_Cyclase"))
                    WinScenario.dropTag("Win_Alpha_Binds_to_Cyclase");
            }
        }
    }

    /*  Function:   getRotationDirection(GameObject) string
        Purpose:    this function determines the rotation direction around the
                    Cell Membrane wall depending on whether it would be quicker
                    to get to the given target Object by going left or right.
        Parameters: the target GameObject
        Return:     left or right
    */
    private string getRotationDirection(GameObject targetObj)
    {       
        //Find rotation direction given closest object
        var    currentRotation = transform.eulerAngles;
        var    targetRotation  = targetObj.transform.eulerAngles;
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

