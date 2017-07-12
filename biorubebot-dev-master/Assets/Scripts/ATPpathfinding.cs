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
using UnityEngine;
using System.Collections;
using System;                                 // for math

public class ATPpathfinding : MonoBehaviour 
{  
  //------------------------------------------------------------------------------------------------
  #region Public Fields + Properties + Events + Delegates + Enums
  public bool droppedOff = false;             // is phospate gone?
  public bool found = false;                  // did this ATP find a dock?
  public float maxHeadingChange;              // max possible rotation angle at a time
  public float angleToRotate;                 // stores the angle in degrees between ATP and dock
  public int maxRoamChangeTime;               // how long before changing heading/speed
  public int minSpeed;                        // slowest the ATP will move
  public int maxSpeed;                        // fastest the ATP will move
  public string trackingTag;                  // objects of this tag are searched for and tracked
  public GameObject trackThis;                // the object with which to dock
  public Transform origin;                    // origin location/rotation is the physical ATP
  #endregion Public Fields + Properties + Events + Delegates + Enums
  //------------------------------------------------------------------------------------------------
  
  //------------------------------------------------------------------------------------------------
  #region Private Fields + Properties + Events + Delegates + Enums
  private int objIndex = 0;                   // the index containing the above "trackThis" object
  private float heading;                      // roaming direction
  private float headingOffset;                // used for smooth rotation while roaming
  private int movementSpeed;                  // roaming velocity
  private int roamInterval = 0;               // how long until heading/speed change while roaming
  private int roamCounter = 0;                // time since last heading speed change while roaming
  private int curveCounter = 90;              // used for smooth transition when tracking
  private Quaternion rotate;                  // rotation while tracking
  #endregion Private Fields + Properties + Events + Delegates + Enums
  //------------------------------------------------------------------------------------------------
  
  #region Private Methods
  //------------------------------------------------------------------------------------------------
  // Directs the ATP to the proper dock (to rotate and dropoff tail). The ATP seeks after the circle 
  // collider of the "trackThis" object, which should be projected to the side of the object. This 
  // method will detect whether or not the "Inner Cell Wall" is in the ATP's line of sight with the
  // collider. If it is, a path will be plotted around it. The incident angle is also calculated 
  // ("angleToRotate") in order to give the "dropOff" function a baseline angle to use for rotation.
  private void Raycasting()
  {
    Vector3 trackCollider = trackThis.GetComponent<CircleCollider2D>().bounds.center;
    RaycastHit2D collision = Physics2D.Linecast(origin.position, trackCollider);

    if(collision.collider.name == "Inner Cell Wall")
    {
      Vector3 collisionAngle = collision.normal;
      Vector3 direction = trackCollider - origin.position;
      Vector3 angle = Vector3.Cross(direction, collisionAngle);
      if(angle.z < 0)                                   // track to the right of the nucleus
      { 
        rotate = Quaternion.LookRotation(origin.position-trackCollider, trackThis.transform.right);
        curveCounter = 90;
      }
      else                                              // track to the left of the nucleus
      { 
        rotate = Quaternion.LookRotation(origin.position-trackCollider, -trackThis.transform.right);
        curveCounter = -90;
      }
    }
    else                                                // calculate approach vector
    {            
      float diffX = origin.position.x - trackCollider.x;
      float diffY = origin.position.y - trackCollider.y;
      float degrees = ((float)Math.Atan2(diffY, diffX) * (180 / (float)Math.PI) + 90);
      transform.eulerAngles = new Vector3 (0, 0, degrees - curveCounter);
      rotate = transform.localRotation;
      if(curveCounter > 0) { curveCounter -= 1; }       // slowly rotate left until counter empty
      else if(curveCounter < 0) {curveCounter += 1; }   // slowly rotate right until counter empty
    }
    transform.localRotation = new Quaternion(0,0,rotate.z, rotate.w);
    transform.position += transform.up * Time.deltaTime * maxSpeed;
    
    angleToRotate = Vector3.Angle(trackThis.transform.up, transform.up);
    Vector3 crossProduct = Vector3.Cross(trackThis.transform.up, transform.up);
    if (crossProduct.z < 0) angleToRotate = -angleToRotate; // .Angle always returns a positive #
  }
  
  //------------------------------------------------------------------------------------------------
  // ATP wanders when not actively seeking a receptor leg. This method causes the ATP to randomly
  // change direction and speed at random intervals.  The tendency for purely random motion objects
  // to generally gravitate toward the edges of a circular container has been artificially remedied
  // by Raycasting and turning the ATP onto a 180 degree course (directing them toward the center).  
  private void Roam()
  {
    if(Time.timeScale != 0)                               // if game not paused
    {
      roamCounter++;                                      
      if(roamCounter > roamInterval)                         
      {                                                   
        roamCounter = 0;
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);  
        var ceiling = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        roamInterval = UnityEngine.Random.Range(5, maxRoamChangeTime);   
        movementSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        RaycastHit2D collision = Physics2D.Raycast(origin.position, origin.up);
        if(collision.collider != null &&                  // must check for instance first
           collision.collider.name == "Cell Membrane(Clone)" &&
           collision.distance < 2)
        {
          if(heading <= 180) { heading = heading + 180; }
          else { heading = heading - 180; }
          movementSpeed = maxSpeed;
          roamInterval = maxRoamChangeTime;
        }
        else { heading = UnityEngine.Random.Range(floor, ceiling); }
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
  
  //------------------------------------------------------------------------------------------------
  // Update is called once per frame. Gets an array of potential GameObjects to track and tries to 
  // find one that is not "found" yet. If it finds one then it stores a pointer to the GameObject as
  // "trackThis" and calls raycasting so that the ATP can seek it out.  Else, ATP wanders.
  private void Update()
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
        GameObject[] foundObjs = GameObject.FindGameObjectsWithTag(trackingTag);
        objIndex = 0;
        while(objIndex < foundObjs.Length && 
              foundObjs[objIndex].GetComponent<TrackingProperties>().isFound == true)
        {
          ++objIndex;
        }
        if(objIndex < foundObjs.Length) 
        {
          if(foundObjs[objIndex].GetComponent<TrackingProperties>().Find() == true)
          { 
            trackThis = foundObjs[objIndex];
            found = true; 
          }
        }
        else { trackThis = null; }
      }
      if(found == true && trackThis.tag == trackingTag) { Raycasting(); }
      else { found = false; }
    }
    if(found == false) { Roam(); }
  }
  #endregion Private Methods
}