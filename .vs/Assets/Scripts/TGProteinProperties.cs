/*  File:       TGProteinProperties
    Purpose:    includes the functions and definitions for the T-G-Protein's
                properties, implementing the ActivationProperties Interface
                so that the isActive variable can easily be retrieved and set
                through GameObject.GetActiveComponent<ActivationProperties>().isActive
    Author:     Ryan Wood
    Created:    Fall 2021
*/
using UnityEngine;

public class TGProteinProperties : MonoBehaviour, ActivationProperties
{
    #region Public Fields + Properties + Events + Delegates + Enums
  
    public bool m_isActive = false;//ready for GTP
  
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
