/*  File:       AlphaProperties
    Purpose:    this file implements the ActivationProperties Interface,
                providing a get/set for the isActive property. The game
                can set it or get it wherever it has a handle on the
                Alpha through GameObject.GetComponent<ActivationProperties>()
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaProperties : MonoBehaviour, ActivationProperties
{
    #region Public Fields + Properties + Events + Delegates + Enums
  
    public bool m_isActive = false;
  
    #endregion Public Fields + Properties + Events + Delegates + Enums
    #region Public Methods

    public bool isActive
    {
        get => m_isActive;
        set => m_isActive = value;
    }

    public void changeState(bool message)
    {
        this.isActive = message;
    }
  
    #endregion Public Methods
    #region Private Methods
  
    private void Start()
    {
        changeState(false);
    }
    #endregion Private Methods
}
