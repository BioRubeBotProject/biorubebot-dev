using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGProteinMovement : MonoBehaviour
{
    public  float      speed;
    public  GameObject targetObject;
    public  GameObject GDP;
    private GameObject cellMembrane;
    private GameObject closestTarget;
    private GameObject childGDP;
    private bool       targetFound;
    private bool       isDockedAtGpcr;
    private bool       hasGdpAttached;
    private bool       hasGtpAttached;
    private bool       isAlphaSeparated = false;
    private float      childPosX        = 0.0f;
    private float      childPosY        = 0.0f;
    private string     rotationDirection;

    /*  Function:   getAlpha() Transform
        Purpose:    this function retrieves the Alpha child of the TGProtein
        Return:     the T-G-Protein Alpha subunit
    */
    Transform getAlpha()
    {
        Transform alpha = null;
        bool      found = false;

        foreach(Transform child in transform)
        {
            if(child.gameObject.name == "alpha")
            {
                alpha = child;
                found = true;
                break;
            }
        }
        if(!found)
            alpha = null;

        return alpha;
    }

    /*  Function:   getDoc() Transform
        Purpose:    this function retrieves the prefab child of the TGProtein that is used
                    as the docking station for the GTP and GDP. This station has tag of
                    tGProteinDock when nothing is docked or GDP is docked and OccupiedG_Protein
                    when GTP is attached
        Return:     the dock, which is a child of Alpha
    */
    public Transform getDoc()
    {
        Transform doc   = null;
        Transform alpha = null;
        bool      found = false;

        alpha = getAlpha();
        if(null != alpha)
        {
            foreach(Transform subChild in alpha.transform)
            {
                //without a GTP, tGProteinDock, with GTP, OccupiedG_Protein
                if(subChild.tag == "tGProteinDock" || subChild.tag == "OccupiedG_Protein")
                {
                    doc   = subChild;
                    found = true;
                    break;
                }
            }
        }
        if(!found)
            doc = null;

        return doc;
    }

    private void Start()
    {
        Transform  doc   = null;
        Transform alpha = null;

        alpha = getAlpha();
        cellMembrane      = GameObject.FindGameObjectWithTag("CellMembrane");
        closestTarget     = null;
        targetFound       = false;
        rotationDirection = null;
        isDockedAtGpcr    = false;
        hasGdpAttached    = true;
        hasGtpAttached    = false;
        isAlphaSeparated  = false;

        childGDP = (GameObject)Instantiate(GDP, transform.position, Quaternion.identity);
        if(null != alpha)
            childGDP.transform.SetParent(alpha.transform);

        doc = getDoc();
        if(null != doc)
            childGDP.transform.position = doc.position;
        childGDP.GetComponent<CircleCollider2D> ().enabled = false;
        childGDP.GetComponent<Rigidbody2D> ().isKinematic  = true;
    }

    /*  Function:   separateAlpha()
        Purpose:    this function makes the alpha child of the T-GProtein
                    no longer a child and sets the alpha subunit's isActive
                    property to true. This will cause the Alpha to seek out
                    an Adenylyl Cyclase if one is on the Cell Membrane wall
    */
    private void separateAlpha()
    {
        Transform alpha = null;

        if(isAlphaSeparated)
            return;

        alpha = getAlpha();
        if(null != alpha)
        {
            childPosX    = alpha.transform.position.x;
            childPosY    = alpha.transform.position.y;
            alpha.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;

            alpha.GetComponent<ActivationProperties>().isActive = true;
            isAlphaSeparated = true;
            //isDockedAtGpcr   = false;
            alpha.GetComponent<AlphaMovement>().targetBetaGamma = this.gameObject;
        }
    }

    public void Update()
    {
        Transform doc = null;

        if(Time.timeScale != 0)//if we are not paused
        {
            if(!isDockedAtGpcr)//if we aren't bound to a GPCR
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
            doc = getDoc();

            if(null != doc)
            {
                if(doc.tag == "OccupiedG_Protein")
                    hasGtpAttached = true;
                else
                    hasGtpAttached = false;
            }
            if(hasGtpAttached && !isAlphaSeparated)
            {
                separateAlpha();
                if(GameObject.FindWithTag("Win_GTP_Binds_to_Alpha"))
                    WinScenario.dropTag("Win_GTP_Binds_to_Alpha");
            }
        }
    }

    public Transform getGdp()
    {
        Transform alpha = getAlpha();
        Transform gdp   = null;
        bool      found = false;

        foreach(Transform child in alpha.transform)
        {
            if(child.gameObject.name == "GDP(Clone)")
            {
                gdp   = child;
                found = true;
                break;
            }
        }
        if(!found)
            gdp = null;

        return gdp;
    }

    /*  Function:   dropGdp()
        Purpose:    this function retags the child GDP to be ReleasedGDP,
                    which will cause it to detach and self-destruct.
                    Also sets our global hasGDPAttached variable false
    */
    private void dropGdp()
    {
        Transform objGdp = getGdp();
        if(null != objGdp)
        {
            objGdp.tag     = "ReleasedGDP";
            hasGdpAttached = false;
        }
    }

    /*  Function:   OnTriggerEnter2D(Collider2D)
        Purpose:    this function handles the event that the TGProtein collided with the
                    active GCBR, setting our global isDockedAtGpcr variable true and
                    setting the properties of the TGProtein
    */
    private void OnTriggerEnter2D(Collider2D other)
	{
        GameObject objCollided = other.gameObject;
        Transform  objDoc      = null;

        //IF right receptor collides with left receptor(with protein signaller)                                                      
        if(objCollided.tag == "GPCR_B" && this.gameObject.name.Equals("ABG-ALL(Clone)"))
        {
            print("collided");
            if(!isAlphaSeparated)
            {
                //check if action is a win condition for the scene/level
                if(GameObject.FindWithTag("Win_TGP_Bound_to_GPCR"))
                    WinScenario.dropTag("Win_TGP_Bound_to_GPCR");

                isDockedAtGpcr = true;
                this.GetComponent<ActivationProperties>().isActive = true;

                dropGdp();
            }
        }
        else if(objCollided.tag == "Alpha" && this.gameObject.name.Equals("ABG-ALL(Clone)"))
        {
            if(objCollided.GetComponent<AlphaMovement>().doFindBetaGamma && //the Alpha is looking for a parent
               objCollided.GetComponent<AlphaMovement>().targetBetaGamma == this.gameObject)//and its looking for me
            {
                Vector3 newPos = new Vector3(childPosX, childPosY, this.transform.position.z);

                objCollided.GetComponent<AlphaMovement>().doFindBetaGamma = false;
                objCollided.GetComponent<AlphaProperties>().isActive      = false;
                objCollided.transform.parent                              = this.transform;

                objCollided.transform.position = newPos;
                objCollided.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

                childGDP         = objCollided.GetComponent<AlphaMovement>().GDP;
                isAlphaSeparated = false;
                isDockedAtGpcr   = false;

                objDoc = getDoc();
                if(null != objDoc)
                {
                    objDoc.tag = "tGProteinDock";
                    print("setting doc tag to tGProteinDock");
                }
                if(GameObject.FindWithTag("Win_Alpha_Rejoins_GProtein"))
                    WinScenario.dropTag("Win_Alpha_Rejoins_GProtein");
            }
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
        Purpose:    this function finds the closest Object that should be targeted
                    for the TGProtein to dock with. This is done by locating
                    the closest object that has the same tag as the global
                    variable targetObject, which is set in the prefab to be
                    the active GPCR.
        Return:     the closest active GPCR
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
        foreach(GameObject GPCR in targets)
        {
            Vector3 diff = GPCR.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if(curDistance < distance)
            {
                target      = GPCR;
                distance    = curDistance;
                targetFound = true;
            }
        }
        if(!targetFound)
            target = null;

        return target;
    }
}
