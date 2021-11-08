using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaMovement : MonoBehaviour
{
    public  float      speed;
    public  GameObject targetObject;//AdenylylCyclase-A
    private GameObject cellMembrane;
    private GameObject closestTarget;
    private bool       isDockedAtCyclase;
    private bool       targetFound;
    private bool       hasGdpAttached;
    private bool       hasGtpAttached;
    private string     rotationDirection;

    private void Start()
    {
        Transform doc = null;

        cellMembrane      = GameObject.FindGameObjectWithTag("CellMembrane");
        closestTarget     = null;
        targetFound       = false;
        rotationDirection = null;
    }

    public void Update()
    {
        Transform doc = null;

        if(Time.timeScale != 0)//if we are not paused
        {
            if(transform.gameObject.GetComponent<ActivationProperties>().isActive)
            {
                if(!isDockedAtCyclase)//if we aren't bound to a GPCR
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
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
	{
        if(other.gameObject.tag == "AdenylylCyclase" && transform.name == "alpha")
        {
            /*//check if action is a win condition for the scene/level
            if(GameObject.FindWithTag("Win_TGP_Bound_to_GPCR"))
                WinScenario.dropTag("Win_TGP_Bound_to_GPCR");*/
            isDockedAtCyclase = true;
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
            Vector3 diff        = GPCR.transform.position - position;
            float   curDistance = diff.sqrMagnitude;
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
