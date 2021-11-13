using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAdenylylCyclaseProperties : MonoBehaviour, ActivationProperties
{
    #region Public Fields + Properties + Events + Delegates + Enums
  
    public bool m_isActive = false;//ready for GTP
  
    #endregion Public Fields + Properties + Events + Delegates + Enums

    public bool isActive
    {
        get => m_isActive;
        set => m_isActive = value;
    }
}
