using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class receptorMovement : MonoBehaviour
{
    public float speed;
    public GameObject targetObject;
    private GameObject cellMembrane;
    private GameObject closestTarget;
    private bool targetFound;
    private string rotationDirection;


    private void Start()
    {
        cellMembrane = GameObject.FindGameObjectWithTag("CellMembrane");
        closestTarget = null;
        Vector3 position = cellMembrane.transform.position;
        targetFound = false;
        rotationDirection = null;
    }


    public void Update()
    {
        if (Time.timeScale != 0)
        {       
            //If target Found
            if (findClosestTarget())
            {          
                rotationDirection = setRotationDirection();

                if (rotationDirection == "right")
                {
                    transform.RotateAround(cellMembrane.transform.position, Vector3.back, speed * Time.deltaTime);
                }

                else if (rotationDirection == "left")
                {
                    transform.RotateAround(cellMembrane.transform.position, Vector3.forward, speed * Time.deltaTime);
                }
            }          
        }
    }


    private string setRotationDirection()
    {       
        //Find rotation direction given closest object
        var currentRotation = transform.eulerAngles;
        var targetRotation = closestTarget.transform.eulerAngles;

        float direction = (((targetRotation.z - currentRotation.z) + 360f) % 360f) > 180.0f ? -1 : 1;       //Clockwise(right) = -1 , CounterClockWise(left) = 1

        if (direction == -1)
        {
            return ("right");
        }

        else
        {
            return ("left");
        }

    }


    private GameObject findClosestTarget()
    {
        GameObject[] targets;
        targets = GameObject.FindGameObjectsWithTag(targetObject.tag);
        closestTarget = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in targets)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closestTarget = go;
                distance = curDistance;
            }
        }

        return closestTarget;
    }


    public void destroyReceptor()
    {
        if (this.gameObject != null)
        {
            Destroy(this.gameObject);
        }
    }

}
