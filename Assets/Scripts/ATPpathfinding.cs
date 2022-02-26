// **************************************************************
// **** Updated on 10/02/15 by Kevin Means
// **** 1.) Added commentary
// **** 2.) Refactored code
// **** 3.) ATP now tracks any object appropriately tagged
// **** 4.) ATP roaming is now smooth and more random
// **** 5.) Added collision contingency for Inner Cell Wall
// **************************************************************
// **** Updated on 10/09/15 by Kevin Means
// **** 1.) ATP tracks directly to the docking object
// **** 2.) trackThis object is now used instead of array index
// ****     (fixed bug)
// **** 3.) If this ATP accidentally services another object, the 
// ****     object that this was tracking is "UnFound" to allow
// ****     other objects to service it.
// **** 4.) Calculates the incident angle for use with docking
// ****     rotation in ATPproperties "dropOff" method.
// **************************************************************
// **** Updated on 10/22/15 by Kevin Means
// **** 1.) Added code for smooth path around the left or right
// ****     hand side of the nucleus (when it's in the way)
// **************************************************************
// **** Updated Fall 2021 by Ryan Wood
// ****  Added code for seeking an active Adenylyl Cyclase
//*************************************************************
// **** Updated on Spring2022 by CByrd
// **** 1) Gutted ATP:
// ****     - all movement methods moved to Roamer and made 
// ****       generic for use with other classes.
// **************************************************************
using UnityEngine;
using System.Collections;
using System;// for math

public class ATPpathfinding : MonoBehaviour 
{  
    //------------------------------------------------------------------------------------------------
    #region Public Fields + Properties + Events + Delegates + Enums
    public bool droppedOff = false;             // is phospate gone?
    public bool found = false;                  // did this ATP find a dock?
    public float maxHeadingChange;              // max possible rotation angle at a time
    public float angleToRotate;                 // stores the angle in degrees between ATP and dock
    public int minSpeed;                        // slowest the ATP will move
    public int maxSpeed;                        // fastest the ATP will move
    public string trackingTag;                  // objects of this tag are searched for and tracked
    public GameObject trackThis;                // the object with which to dock
    #endregion Public Fields + Properties + Events + Delegates + Enums
    //------------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------------
    #region Private Fields + Properties + Events + Delegates + Enums
    private Roamer r;                             //an object that holds the values for the roaming (random movement) methods
    #endregion Private Fields + Properties + Events + Delegates + Enums
    //------------------------------------------------------------------------------------------------
    #region Private Methods

    //------------------------------------------------------------------------------------------------
    private void Start()
    {
       r = new Roamer(minSpeed, maxSpeed, maxHeadingChange);
    }
  
    //------------------------------------------------------------------------------------------------
    // Update is called once per phyciscs update. Gets an array of potential GameObjects to track and tries to 
    // find one that is not "found" yet. If it finds one then it stores a pointer to the GameObject as
    // "trackThis" and calls raycasting so that the ATP can seek it out.  Else, ATP wanders.
    private void FixedUpdate()
    {
        if(droppedOff) 
        { 
            found = false; 
            trackThis.GetComponent<TrackingProperties>().UnFind();
        }
        else
        {
            if(found == false)
            {
                //GameObject[] foundObjs = GameObject.FindGameObjectsWithTag(trackingTag);       
                //trackThis = findNearest(foundObjs);
                trackThis = BioRubeLibrary.FindClosest(this.transform, trackingTag);
                if(trackThis != null && trackThis.GetComponent<TrackingProperties>().Find() == true)
                { 
                    found = true; 
                        if(trackThis.name == "Adenylyl_cyclase-B(Clone)") //because cyclase takes multiple ATPs, turn off isFound after every Find
                        {
                            trackThis.GetComponent<TrackingProperties>().isFound = false;
                        }
                }
                else
                    trackThis = null;
            }
            if (found == true && trackThis.tag == trackingTag)
            {
                angleToRotate  = r.moveToDock(this.gameObject, trackThis);
            }
            else
                found = false;
        }
        if(found == false)
            r.Roaming(this.gameObject);
    }
    #endregion Private Methods
}

