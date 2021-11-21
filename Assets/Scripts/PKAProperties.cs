/*  File:       PKAProperties
    Purpose:    This file holds the properties for the PKA, implementing the
                ActivationProperties Interface so that its activation status
                can be retrieved or set using
                GameObject.GetComponent<ActivationProperties>().isActive
    Author:     Ryan Wood
    Created:    Fall 2021
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PKAProperties : MonoBehaviour, ActivationProperties
{
    private bool m_isActive   = false;
    public  int  coliderIndex = 0;

    public bool isActive
    {
        get => m_isActive;
        set => m_isActive = value;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
