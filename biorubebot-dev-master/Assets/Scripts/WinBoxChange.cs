using UnityEngine;
using System.Collections;

public class WinBoxChange : MonoBehaviour
{
    public GameObject WinConBox_2;
    public GameObject WinCondition;

    // Update is called once per frame
    void FixedUpdate()
    {
        //find Win Box objects on screen/scene that have been met since last update
        if (GameObject.FindWithTag("Condition_Met"))
        {
            WinCondition = GameObject.FindWithTag("Condition_Met");
            GameObject obj = Instantiate(WinConBox_2, WinCondition.transform.position, Quaternion.identity) as GameObject;  //transforms the "unchecked" box to the "checked one"
            GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(obj);
            Destroy(WinCondition);
        }
        
    }
}
