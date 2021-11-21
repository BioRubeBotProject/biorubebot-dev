/*  File:       AdenylylCyclaseProperties
    Purpose:    this file implements the ActivationProperties Interface,
                providing a get/set for the isActive property. The game
                can set it or get it wherever it has a handle on the
                Adenylyl Cyclase A through GameObject.GetComponent<ActivationProperties>()
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdenylylCyclaseProperties : MonoBehaviour, ActivationProperties
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

    /*  Function:   changeState(bool)
        Purpose:    this function is only called by the start method,
                    and it could probably be removed. It's easier to access
                    the isActive variable through the property, and that's
                    how it is done outside of this file.
                    The other option is to add this function to the ActivationProperties
                    Interface and make other classes that implement that interface
                    implement it, and refactore all the calls to isActive to use
                    this function instead
        Parameters: the state
    */
    public void changeState(bool message)
    {
        this.isActive = message;
    }
  
    #endregion Public Methods
    #region Private Methods
  
    /*  Function:   Start()
        Purpose:    initializer, sets the isActive property of this
                    inactive Adenylyl Cyclase to false
    */
    private void Start()
    {
        changeState(false);
    }

    #endregion Private Methods
}
