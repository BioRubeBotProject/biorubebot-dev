/*  File:       AlphaMovement
    Purpose:    The Trimeric G-Protein's Alpha subunit breaks off from the
                whole and moves toward an inactive Adenylyl Cyclase on the
                Cell Membrane. Once there, the subunit activates the Cyclase
                so it can turn ATP into cAMP. The Apha subunit has a docking
                station for GDP or GTP depending on whether the subunit
                is Active or not
    Author:     Ryan Wood
    Created:    Fall 2021
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaMovement : MonoBehaviour
{
    //all these public variables are set in the Prefab
    public  bool       doFindBetaGamma;//whether we are actively seeking beta-gamma subunit
    public  GameObject targetObject;   //AdenylylCyclase-A
    public  GameObject targetBetaGamma;//parent GTProtien
    public  GameObject GDP;            //in the Prefab, this is set to the GTP Prefab
    public  float      speed;          //how quickly to move along, set in the Prefab
    public  float      gtpActiveTimeMax;//how long the GTP remains in the doc before hydrolizing

    private GameObject inactiveCyclase;  //the Adenylyl Cyclase to which this Apha is attached
    private GameObject cellMembrane;     //the Cell Membrane on which the Alpha is attached
    private GameObject closestTarget;    //looking for Adenylyl, this is the nearest one
    private bool       isDockedAtCyclase;//docked at an Active Adenylyl Cyclase
    private bool       targetFound;      //located an inactive Adenylyl Cyclase
    private bool       hasGdpAttached;   //whether GDP is attached to the alpha in the doc
    private bool       hasGtpAttached;   //whether GTP is attached to the alpha in the doc
    private string     rotationDirection;//left of right around the Cell Membrane
    private float      activeStart = 0.0f;//the time at which the Alpha was activated by GTP

    /*  Function:   Start()
        Purpose:    initializes some globals
    */
    private void Start()
    {
        Transform doc = null;

        cellMembrane      = GameObject.FindGameObjectWithTag("CellMembrane");
        closestTarget     = null;
        targetFound       = false;
        rotationDirection = null;
        doFindBetaGamma   = false;
    }

    /*  Function:   getDoc() GameObject
        Purpose:    this function retrieves the Doc station from the Alpha,
                    which is a child of the Alpha and returns it. This is a
                    GameObject that is used for GTP to track and bind with
                    the Alpha. It's also the spawn location for GDP
        Return:     the doc for GTP and GDP
    */
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

    /*  Function:   getGtpChild() GameObject
        Purpose:    this function returns the GTP that is currently attached
                    to this Alpha subunit via its doc station
        Return:     the GTP attached if there is one
    */
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

    /*  Function:   spawnGdp() IEnumerator
        Purpose:    this function spawns a GDP in the doc station of this
                    Alpha Subunit. This is the trigger for the Alpha to move
                    back to the Beta-Gamma complex and away from the Adenylyl
                    Cyclase
        Return:     nothing important
    */
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

    /*  Function:   dropGtp()
        Purpose:    this function simply retags the GTP GameObject to
                    ReleaseGTP. This tag change willl automatically cause
                    the GTP to detatch, and explode, leaving the game
    */
    private void dropGtp()
    {
        GameObject childGtp = getGtpChild();

        if(null != childGtp)
        {
            childGtp.transform.tag = "ReleasedGTP";
            hasGtpAttached         = false;
        }
    }

    /*  Function:   seekBetaGamma()
        Purpose:    this function causes the Alpha to move back toward the
                    Beta-Gamma complex and away from the Adenylyl Cyclase.
                    the Alpha does this once a GDP spawns in place of the GTP
                    after the GTP hydrolizes
    */
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

    /*  Function:   setAdenylylCyclaseInactive()
        Purpose:    this function sets the Active Adenylyl Cyclase with which
                    this Alpha Subunit has bound to inactive. This will cause
                    the Active Cyclase to leave the game and an Inactive Adenylyl
                    Cyclase to spawn in its place
    */
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

    /*  Function:   Update()
        Purpose:    this function is called once per frame. It sets variables and
                    calls functions to move the Alpha toward an inactive Adenylyl
                    Cyclase or toward a Beta-Gamma depending on whether it is
                    active or not, and sets/checks a timer for when the GTP
                    should hydrolize and leave the game
    */
    public void Update()
    {
        Transform doc = null;

        if(Time.timeScale != 0)//if we are not paused
        {
            if(transform.gameObject.GetComponent<ActivationProperties>().isActive)
            {
                //we just became active, start timer and set GTP attached true
                if(0.0f == activeStart)
                {
                    hasGtpAttached = true;//we have a GTP attached
                    activeStart    = Time.timeSinceLevelLoad;
                }

                if(!isDockedAtCyclase)//if we aren't bound to a Cyclase
                {
                    closestTarget = findClosestTarget();//find closes Inactive Adenylyl Cyclase
                    if(null != closestTarget)
                    {
                        rotationDirection = getRotationDirection(closestTarget);

                        if(rotationDirection == "right")
                            transform.RotateAround(cellMembrane.transform.position, Vector3.back, speed * Time.deltaTime);
                        else if(rotationDirection == "left")
                            transform.RotateAround(cellMembrane.transform.position, Vector3.forward, speed * Time.deltaTime);
                    }
                }

                //check the timer. Time for GTP to leave?
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

