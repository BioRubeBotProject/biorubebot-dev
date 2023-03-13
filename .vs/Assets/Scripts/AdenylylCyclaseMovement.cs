/*  File:       AdenylylCyclaseMovement
    Purpose:    this file essentially listens for the inactive Adenylyl Cyclase
                to become activated by colliding with the Trimeric G-Protien's
                Alpha Subunit. Once that occurs, this fil instantiates an
                Active Adenylyl Cyclase and sets the GameObject inactive
    Author:     Ryan Wood
    Created:    Fall 2021
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdenylylCyclaseMovement : MonoBehaviour
{
    public GameObject activeCyclase;//Adenylyl Cyclase B, set in the Adenylyl Cyclase A Prefab

    void Start()
    {
        
    }

    /*  Function:   Update()
        Purpose:    this function is called once per frame, and it waits for the
                    Adenylyl Cyclase's isActive property, inherited through the
                    ActivationProperties Interface, to be true. Once it is
                    true, deactivates this game Object and spawns an Active
                    Adenylyl Cyclase in its place. The activeCyclase global
                    variable is used for this, and it is set in the prefab
                    to be the Adenylyl Cyclase B prefab
    */
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
