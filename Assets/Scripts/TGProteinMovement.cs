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
    private bool       alphaSeparated;
    private string     rotationDirection;

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

    Transform getDoc()
    {
        Transform doc   = null;
        bool      found = false;

        foreach(Transform child in transform)
        {
            if(child.gameObject.name == "alpha")
            {
                foreach(Transform subChild in child.transform)
                {
                    //without a GTP, tGProteinDock, with GTP, OccupiedG_Protein
                    if(subChild.tag == "tGProteinDock" || 
                        subChild.tag == "OccupiedG_Protein")
                    {
                        doc   = subChild;
                        found = true;
                        break;
                    }
                }
            }
        }
        if(!found)
            doc = null;

        return doc;
    }

    private void Start()
    {
        Transform doc = null;

        cellMembrane      = GameObject.FindGameObjectWithTag("CellMembrane");
        closestTarget     = null;
        targetFound       = false;
        rotationDirection = null;
        isDockedAtGpcr    = false;
        hasGdpAttached    = true;
        hasGtpAttached    = false;
        alphaSeparated    = false;

        childGDP = (GameObject)Instantiate (GDP, transform.position, Quaternion.identity);
        childGDP.transform.SetParent(this.transform);

        doc = getDoc();
        if(null != doc)
            childGDP.transform.position = doc.position;
        childGDP.GetComponent<CircleCollider2D> ().enabled = false;
        childGDP.GetComponent<Rigidbody2D> ().isKinematic  = true;
    }

    private void separateAlpha()
    {
        Transform alpha = null;

        if(alphaSeparated)
            return;

        alpha = getAlpha();
        if(null != alpha)
        {
            alpha.parent = null;
            alpha.GetComponent<ActivationProperties>().isActive = true;
            alphaSeparated = true;
        }
        else
            print("alpha is null");
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
                    rotationDirection = setRotationDirection();

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
            if(hasGtpAttached && !alphaSeparated)
                separateAlpha();
        }
    }

    private void dropGdp()
    {
        childGDP.tag   = "ReleasedGDP";
        hasGdpAttached = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
	{
        //IF right receptor collides with left receptor(with protein signaller)                                                      
        if(other.gameObject.tag == "GPCR_B" && this.gameObject.name.Equals("ABG-ALL(Clone)"))
        {                
            //check if action is a win condition for the scene/level
            if(GameObject.FindWithTag("Win_TGP_Bound_to_GPCR"))
                WinScenario.dropTag("Win_TGP_Bound_to_GPCR");
            isDockedAtGpcr = true;
            TGProteinProperties objProps = (TGProteinProperties)this.GetComponent("TGProteinProperties");
            objProps.isActive = true;

            dropGdp();
        }
    }

    private string setRotationDirection()
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
