// **************************************************************
//  LIBRARY of STATIC FUNCTIONS for BioRubeBot the game
//  Please place all static functions here for re-use in multiple classes.
//
// **** Created on 2/19/2022 by CByrd Spring2022
// **** 1.) Moved static functions from Roam (now Roamer) to here
// **** 2.) 
// **** 3.) 
// **************************************************************


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BioRubeLibrary : MonoBehaviour
{
    public static float _speed = 5.0f;

    public static Vector3 CalcMidPoint(GameObject obj_1, GameObject obj_2)
    {
        float[] temp = new float[2];
        temp[0] = (obj_1.transform.position.x + obj_2.transform.position.x) / 2.0f;
        temp[1] = (obj_1.transform.position.y + obj_2.transform.position.y) / 2.0f;
        Vector3 meetingPoint = new Vector3(temp[0], temp[1], obj_1.transform.position.z);

        return meetingPoint;
    }


    public static GameObject FindClosest(Transform my, string objTag)
    {
        float distance = Mathf.Infinity; //initialize distance to 'infinity'

        GameObject[] gos; //array of gameObjects to evaluate
        GameObject closestObject = null;
        //populate the array with the objects you are looking for
        gos = GameObject.FindGameObjectsWithTag(objTag);

        //find the nearest object ('objectTag') to me:
        foreach (GameObject go in gos)
        {
            //calculate square magnitude between objects
            float curDistance = Vector3.Distance(my.position, go.transform.position);
            if (curDistance < distance)
            {
                closestObject = go; //update closest object
                distance = curDistance;//update closest distance
            }
        }
        return closestObject;
    }/* end FindClosest */



    public static void setAlpha(GameObject obj, float alpha)
    {
        if (obj.GetComponent<Renderer>() != null)
        {
            Color a = obj.gameObject.GetComponent<Renderer>().material.color;
            a.a = alpha;

            obj.gameObject.GetComponent<Renderer>().material.color = a;
        }
        else if (obj.GetComponentInChildren<Renderer>() != null)
        {
            Color a = obj.gameObject.GetComponentInChildren<Renderer>().material.color;
            a.a = alpha;

            obj.gameObject.GetComponentInChildren<Renderer>().material.color = a;
            for (int i = 0; i < obj.gameObject.transform.childCount; i++)
            {
                setAlpha(obj.gameObject.transform.GetChild(i).gameObject, alpha);
            }
        }
    }
}
