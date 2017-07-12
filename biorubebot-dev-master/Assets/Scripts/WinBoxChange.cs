using UnityEngine;
using System.Collections;

public class WinBoxChange : MonoBehaviour
{
    public GameObject WinConBox_2;
    public GameObject WinCondition;

    // Use this for initialization
    void Start()
    {
        //parentObject = GameObject.FindGameObjectWithTag("MainCamera"); //Get reference for parent object in UnityEditor
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameObject.FindWithTag("Condition_Met"))
        {
            WinCondition = GameObject.FindWithTag("Condition_Met");
            GameObject obj = Instantiate(WinConBox_2, WinCondition.transform.position, Quaternion.identity) as GameObject;
            //obj.transform.parent = parentObject.transform; //Sets curent object to be under the parent object.
            GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(obj);
            Destroy(WinCondition);
        }
        
    }
}
