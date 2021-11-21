/*  File:       WinBoxChange
    Purpose:    This file handles the event that one of the CheckBoxes on
                the current level has been retagged Condition_Met. This
                tells it to instantiate a checked checkbox and remove
                the unchecked box
*/
using UnityEngine;
using System.Collections;

public class WinBoxChange : MonoBehaviour
{
    public GameObject WinConBox_2;
    public GameObject WinCondition;

    /*  Function:   FixedUpdate()
        Purpose:    this function continously searches for a GameObject that has
                    the tag Condition_Met. When it finds such an Object it
                    removes it and spawns a checked checkbox in its place
    */
    void FixedUpdate()
    {
        //find Win Box objects on screen/scene that have been met since last update
        if(GameObject.FindWithTag("Condition_Met"))
        {
            WinCondition = GameObject.FindWithTag("Condition_Met");
            GameObject obj = Instantiate(WinConBox_2, WinCondition.transform.position, Quaternion.identity) as GameObject;  //transforms the "unchecked" box to the "checked one"
            GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(obj);
            Destroy(WinCondition);
        }
        
    }
}
