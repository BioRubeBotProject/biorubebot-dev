/*  File:       ActiveAdenylylCyclaseProperties
    Purpose:    this file implements the ActivationProperties Interface,
                providing a get/set for the isActive property.
    Author:     Ryan Wood
    Created:    Fall 2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAdenylylCyclaseProperties : MonoBehaviour, ActivationProperties
{
    #region Public Fields + Properties + Events + Delegates + Enums
  
    public bool m_isActive = false;
  
    #endregion Public Fields + Properties + Events + Delegates + Enums

    public bool isActive
    {
        get => m_isActive;
        set => m_isActive = value;
    }
}
