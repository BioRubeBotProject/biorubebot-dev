using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdenylylCyclaseMovement : MonoBehaviour
{
    public GameObject activeCyclase;//Adenylyl Cyclase B

    //Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if the Cyclase is active, instantiate an activated cyclase
        if(this.gameObject.GetComponent<ActivationProperties>().isActive)
        {
            GameObject parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
            GameObject newCyclase   = (GameObject)Instantiate(activeCyclase, transform.position, transform.rotation);

            newCyclase.transform.parent = parentObject.transform;
            newCyclase.GetComponent<ActivationProperties>().isActive = true;

            GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(newCyclase);
            this.gameObject.SetActive(false);
        }
    }
}
