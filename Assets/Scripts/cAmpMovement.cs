/*  File:       cAmpMovement
    Purpose:    this file handles all the movement for the cAMPs, which
                wander around the Cell Membrane unless there is an inactive
                PKA around, in which case it tracks to it to bind with it.
                This code was modelled after the ATP Pathfinding file
    Notes:      needs improvement because it does not stay inside the Cell
                Membrane as it should
    Author:     Ryan Wood
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class cAmpMovement : MonoBehaviour
{
    //------------------------------------------------------------------------------------------------
    //these public variables are all set in the cAMP Prefab
    #region Public Fields + Properties + Events + Delegates + Enums
    public  GameObject trackThis;                // the object with which to dock
    public  Transform  origin;                   // origin location/rotation is the physical cAMP
    public  string     trackingTag;              // objects of this tag are searched for and tracked
    public  float      maxHeadingChange;         // max possible rotation angle at a time
    public  float      angleToRotate;            // stores the angle in degrees between cAMP and dock
    public  bool       dockedWithPKA     = false;//did this cAMP dock with a PKA?
    public  int        maxRoamChangeTime;        // how long before changing heading/speed
    public  int        minSpeed;                 // slowest the cAMP will move
    public  int        maxSpeed;                 // fastest the cAMP will move
    #endregion Public Fields + Properties + Events + Delegates + Enums
    //------------------------------------------------------------------------------------------------
  
    //------------------------------------------------------------------------------------------------
    #region Private Fields + Properties + Events + Delegates + Enums
    private Quaternion       rotate;                  // rotation while tracking
    private float            heading;                 // roaming direction
    private float            headingOffset;           // used for smooth rotation while roaming
    private bool             foundPKA         = false;// did this cAMP find a PKA doc?
    private int              movementSpeed    = 0;    // roaming velocity
    private int              objIndex         = 0;    // the index containing the above "trackThis" object
    private int              roamInterval     = 0;    // how long until heading/speed change while roaming
    private int              roamCounter      = 0;    // time since last heading speed change while roaming
    private int              curveCounter     = 90;   // used for smooth transition when tracking
    private int              pkaColliderIndex = 0;    //the index of the PKA's collider array to track toward
    #endregion Private Fields + Properties + Events + Delegates + Enums
    //------------------------------------------------------------------------------------------------
  
    #region Private Methods
    // Directs the cAMP to the proper dock (to rotate and dropoff tail). The cAMP seeks after the circle 
    // collider of the "trackThis" object, which should be projected to the side of the object. This 
    // method will detect whether or not the "Inner Cell Wall" is in the cAMP's line of sight with the
    // collider. If it is, a path will be plotted around it. The incident angle is also calculated 
    // ("angleToRotate") in order to give the "dropOff" function a baseline angle to use for rotation.
    private void Raycasting()
    {
        CircleCollider2D[] colliders = trackThis.GetComponents<CircleCollider2D>();

        Vector3      vTrackCollider = colliders[pkaColliderIndex].bounds.center;
        RaycastHit2D collision      = Physics2D.Linecast(origin.position, vTrackCollider);

        if(collision.collider.name == "Inner Cell Wall")
        {
            Vector3 collisionAngle = collision.normal;
            Vector3 direction      = vTrackCollider - origin.position;
            Vector3 angle          = Vector3.Cross(direction, collisionAngle);

            if(angle.z < 0)// track to the right of the nucleus
            { 
                rotate       = Quaternion.LookRotation(origin.position-vTrackCollider, trackThis.transform.right);
                curveCounter = 90;
            }
            else//track to the left of the nucleus
            { 
                rotate       = Quaternion.LookRotation(origin.position-vTrackCollider, -trackThis.transform.right);
                curveCounter = -90;
            }
        }
        else// calculate approach vector
        {            
            float diffX   = origin.position.x - vTrackCollider.x;
            float diffY   = origin.position.y - vTrackCollider.y;
            float degrees = ((float)Math.Atan2(diffY, diffX) * (180 / (float)Math.PI) + 90);

            transform.eulerAngles = new Vector3 (0, 0, degrees - curveCounter);
            rotate                = transform.localRotation;

            if(curveCounter > 0)
                curveCounter -= 1;// slowly rotate left until counter empty
            else if(curveCounter < 0)
                curveCounter += 1;// slowly rotate right until counter empty
        }
        transform.localRotation = new Quaternion(0,0,rotate.z, rotate.w);
        transform.position += transform.up * Time.deltaTime * maxSpeed;
    
        angleToRotate = Vector3.Angle(trackThis.transform.up, transform.up);
        Vector3 crossProduct = Vector3.Cross(trackThis.transform.up, transform.up);
        if(crossProduct.z < 0)
            angleToRotate = -angleToRotate; // .Angle always returns a positive #
    }
  
    //------------------------------------------------------------------------------------------------
    // cAMP wanders when not actively seeking a receptor leg. This method causes the cAMP to randomly
    // change direction and speed at random intervals.  The tendency for purely random motion objects
    // to generally gravitate toward the edges of a circular container has been artificially remedied
    // by Raycasting and turning the cAMP onto a 180 degree course (directing them toward the center).  
    private void roam()
    {
        if(Time.timeScale != 0)// if game not paused
        {
            roamCounter++;                                      
            if(roamCounter > roamInterval)                         
            {
                roamCounter   = 0;
                var floor     = Mathf.Clamp(heading - maxHeadingChange, 0, 360);  
                var ceiling   = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
                roamInterval  = UnityEngine.Random.Range(5, maxRoamChangeTime);   
                movementSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);

                RaycastHit2D collision = Physics2D.Raycast(origin.position, origin.up);
                if(collision.collider != null && collision.collider.name == "Cell Membrane(Clone)" &&
                   collision.distance < 2)
                {
                    if(heading <= 180)
                        heading = heading + 180;
                    else
                        heading = heading - 180;

                    movementSpeed = maxSpeed;
                    roamInterval  = maxRoamChangeTime;
                }
                else
                    heading = UnityEngine.Random.Range(floor, ceiling);

                headingOffset = (transform.eulerAngles.z - heading) / (float)roamInterval;
            }
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - headingOffset);
            transform.position += transform.up * Time.deltaTime * movementSpeed;
        }
    }
  
    //------------------------------------------------------------------------------------------------
    private void Start()
    {
    }

    /*  Function:   OnTriggerEnter2D(Collider2D) IEnumerator
        Purpose:    this function handles the collistion between the cAMP and a
                    PKA GamObject. Sets the dockedWithPKA variable true
        Parameters: the Collider of GameObject with which the cAMP collided
    */
    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if(!dockedWithPKA)
        {
            if(other.gameObject == trackThis)
            {
                if(other.gameObject.tag == "PKA" && other.GetComponent<ActivationProperties>().isActive == false)
                {
                    dockedWithPKA = true;
                }
            }
        }

        yield return new WaitForSeconds(1);
    }
  
    /*  Function:   Update()
        Purpose:    called once per frame, this function looks for a PKA that is
                    inactive and has not been "found", which means it has a
                    colliderIndex of <= 1 because PKA has only two docs
                    with which cAMP can doc. Once two cAMP docs with the PKA, it is activated.
                    if there is no inactive PKA in sight that has not been found,
                    the cAMP roams
    */
    private void Update()
    {
        if(Time.timeScale != 0)
        {
            if(dockedWithPKA)
                return;
            if(foundPKA == false)
            {
                GameObject[] foundObjs = GameObject.FindGameObjectsWithTag(trackingTag);
                GameObject   foundObj  = null;
                objIndex = 0;
                
                //loop through the Objects with the tracking tag until we get an index that isn't "found" yet
                while(objIndex < foundObjs.Length && foundObjs[objIndex].GetComponent<TrackingProperties>().isFound == true)
                {
                    ++objIndex;
                }
                if(objIndex < foundObjs.Length) 
                {
                    foundObj = foundObjs[objIndex];
                    if(foundObj.GetComponent<TrackingProperties>().Find() == true &&
                       foundObj.GetComponent<ActivationProperties>().isActive == false)
                    {
                        //only two colliders on PKA on which to track
                        if(foundObj.GetComponent<PKAProperties>().coliderIndex > 1)
                        {
                            foundObj.GetComponent<TrackingProperties>().isFound = true;
                        }
                        else
                        {
                            trackThis = foundObjs[objIndex];
                            foundPKA  = true; 
                            if(trackThis.name == "PKA-A(Clone)")
                            {
                                trackThis.GetComponent<TrackingProperties>().isFound = false;
                                pkaColliderIndex = trackThis.GetComponent<PKAProperties>().coliderIndex;
                                trackThis.GetComponent<PKAProperties>().coliderIndex++;
                            }
                        }
                    }
                }
                else
                    trackThis = null;
            }
            if(foundPKA == true && trackThis.tag == trackingTag)
            {
                Raycasting();
            }
            else
                foundPKA = false;
    
            if(foundPKA == false)
                roam();
        }
    }

    #endregion Private Methods
}
