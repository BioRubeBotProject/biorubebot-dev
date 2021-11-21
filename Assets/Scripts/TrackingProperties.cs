/*  File:       TrackingProperties
    Purpose:    Allows tracking of generic Objects. Currently only used
                by ATP, but this class could potentially be made more
                useful, and other GameObjects could be refactored to more
                generically track things.
    Author:     Kevin Means
    Created:    10/8/2015
*/

// **************************************************************
// **** Created on 10/08/15 by Kevin Means
// **** 1.) Allows tracking of generic objects
// **************************************************************
// **** Updated on 10/09/15 by Kevin Means
// **** 1.) added UnFind method for reason stated
// **************************************************************
using UnityEngine;
using System.Collections;

public class TrackingProperties : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates + Enums

    public bool isFound = false;

    #endregion Public Fields + Properties + Events + Delegates + Enums

    //------------------------------------------------------------------------------------------------
    public bool Find()
    {
        if(isFound == false)
        {
            isFound = true;
            return true;
        }
        return false;
    }

    //------------------------------------------------------------------------------------------------
    // In case another object is unintentionally serviced by the ATP that was tracking this one, this
    // allows this object to be "found" again.
    public void UnFind()
    {
        isFound = false;
    }
}