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
        print("Alpha isActive = " + m_isActive);
    }
  
    #endregion Public Methods
    #region Private Methods
  
    private void Start()
    {
        changeState(false);
    }
    #endregion Private Methods
}
