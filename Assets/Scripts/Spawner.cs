// **************************************************************
// **** Updated on 9/25/15 by Kevin Means
// **** 1.) Added all commentary
// **** 2.) Refactored some code
// **** 3.) Receptor now points toward CellMembrane's center
// **** 4.) Receptor now snaps to CellMembrane
// **************************************************************
using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Spawner : MonoBehaviour , Tutorial.SwitchOnOff
{
  public static bool panning = true;  // unknown
  public float snapRadius;            // the radius for the spawned object to snap to
  public float snapDistance;          // to rotate, object must be within this relative distance
  public GameObject spawnedObject;    // for final object instantiation (after user releases mouse)
  public Vector3 spawnLocation;       // for final object instantiation (after user releases mouse)
  public Vector3 guidePosition;       // cannot change "transform.position.x,y, or z" directly
  GameObject cellMembrane;            // the one and only cellMembrane object for this world
  bool isSnapped;                     // has this object been snapped to a radius?
  float x;                            // mouse x coordinate
  float y;                            // mouse y coordinate
  float xfrom;                        // mouse x when object is created
  float degrees;                      // calculated # of degrees for object instantiation
  Transform nucleus;                  // the nucleus (child) of the cellMembrane
  Vector3 ReturnLocation;             // original location of the "button"
  Quaternion ReturnRotation;          // orginal rotaion of the "button"
  public GameObject parentObject;     //Parent object used for unity editor Tree Hierarchy
  
  //------------------------------------------------------------------------------------------------
  // This is called once per frame. 
  void Update ()
  {
    x = Input.mousePosition.x;
    y = Input.mousePosition.y;
  }
  
  //------------------------------------------------------------------------------------------------
  // This is called each time a user clicks the mouse button while hovering over a screen button.
  // Whenever a user clicks on an object button to create it, the member variable "degrees" (of 
  // rotation) is initialized to zero so that normal objects (other than the receptor) will 
  // effectively have an instantiated identity rotation, while the receptor still has the ability
  // to be instantiated with a custom rotation.  This method also saves the original location and 
  // rotation of the object's button in order to place back in the menu. It is determined if the
  // "Cell Membrane" exists at this point as well.  If so, that object is saved as "cellMembrane"
  // for use in other methods, if not, then "null" is saved.  This ensures that it's not called 
  // repeatedly during the many "OnMouseDrag" method calls.
  void OnMouseDown()
  {
    panning = false;
    degrees = 0.0f;
    ReturnLocation = transform.position;
    ReturnRotation.eulerAngles = transform.eulerAngles;
    cellMembrane = GameObject.FindGameObjectWithTag ("CellMembrane");

    //Get reference for parent object in UnityEditor
	parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
    if(cellMembrane != null) 
    { 
      nucleus = cellMembrane.transform.GetChild(0).gameObject.transform;
    }
    this.GetComponent<Collider2D>().enabled = false;
    xfrom = Input.mousePosition.x;
    }
  
  //------------------------------------------------------------------------------------------------
  // Called repeatedly as the user drags the mouse with the mouse button held down while hovering
  // over an object button. "Rotate and Snap" is only called for the Receptor and NPC and only when 
  // Cell Membrane is in the world. "guidePosition" is a temporary position variable used for 
  // keeping track of where the mouse is. The function "Camera.main.ScreenToWorldPoint" gets the 
  // mouse coordinates and converts them to world coordinates. The "spawnedObject" position is there 
  // because each object has a certain "Z" height or depth associated with it. Also, since 
  // the objects will be spawned at "camera height" the "+1" is so the object will be in front of 
  // the camera instead of on the same level.
  void OnMouseDrag()
  {
    guidePosition = Camera.main.ScreenToWorldPoint(new Vector3(x, y, spawnedObject.transform.position.z + 1));

    if(cellMembrane != null || spawnedObject.name == "Cell Membrane")
    {
        if (spawnedObject.name == "_ReceptorInactive" || spawnedObject.name == "NPC" || 
            spawnedObject.name == "Right_Receptor_Inactive" || spawnedObject.name == "Left_Receptor_Inactive") 
        { 
            ThisIsARotatableObject(); 
        }
        else 
        {
            // move the object to the mouse position
            transform.position = guidePosition; 
        }    
    }
  }
  
  //------------------------------------------------------------------------------------------------
  // Called when user releases mouse button. The "if" statement disallows object creation until the
  // Cell Membrane is in place or if the user is trying to create the Cell Membrane.
  // Additional restriction on x position of mouse to ensure object won't be dropped behind the Menu drop down
  void OnMouseUp()
  {
    //float MenuPos = Camera.main.WorldToScreenPoint(GameObject.FindWithTag("Drop_Down_Button").transform.position).x;

    if ((cellMembrane != null || spawnedObject.name == "Cell Membrane"))// && x < (MenuPos - (MenuPos/7.77))) //This is ghetto. Why 7.77? *shrugggg* 
    {     
      spawnLocation = transform.position;
	  GameObject obj = Instantiate (spawnedObject, spawnLocation, Quaternion.Euler(0f, 0f, degrees)) as GameObject;

      //Sets curent object to be under the parent object.
	  obj.transform.parent = parentObject.transform;
      GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add (obj);
      obj = GameObject.FindGameObjectWithTag("CellMembraneButton") as GameObject;
      if(obj != null) {
        obj.SetActive (false);
      }
    } 
    
    transform.position = ReturnLocation;
    transform.localRotation = ReturnRotation;
    this.GetComponent<Collider2D>().enabled = true;
    isSnapped = false;
    panning = true;
  }
  
  //------------------------------------------------------------------------------------------------
  // This is called everytime the mouse drags while holding a certain object. It's purpose is to
  // artificially rotate and place the object correctly in relation to the orbitable body (in this
  // case, either the Cell Membrane or the Nucleus). It starts with the guidePosition (mouse 
  // position relative to the world). It finds the distance between the mouse and the respective 
  // centers of the orbitable bodies.  If the object is within a certain distance of an orbitable 
  // body, it will call the SnapAndRotate function this object. If they are not within a certain
  // distance, then it is treated as any ordinary object and placed in the world at the mouse
  // position with zero rotation.
  void ThisIsARotatableObject()
  { 
    float cellDistance = Vector3.Distance(guidePosition, cellMembrane.transform.position);
    float nucDistance = Vector3.Distance(guidePosition, nucleus.transform.position);
    if(cellDistance < snapDistance * cellMembrane.transform.localScale.x &&
       cellDistance > snapRadius / 1.2)
    {
      float cellMemX = guidePosition.x - cellMembrane.transform.position.x;
      float cellMemY = guidePosition.y - cellMembrane.transform.position.y;
      SnapAndRotate(cellMemY, cellMemX, cellMembrane.transform);
    }
    else if(cellDistance < snapRadius / 1.3 &&
            nucDistance  < snapDistance * 1.8 * nucleus.transform.localScale.x)
    {
      float nucleusX = guidePosition.x - nucleus.transform.position.x;
      float nucleusY = guidePosition.y - nucleus.transform.position.y;
      SnapAndRotate(nucleusY, nucleusX, nucleus);
    }
    else
    { 
      transform.localRotation = ReturnRotation;
      transform.position = guidePosition; 
      degrees = 0f;
    }
  }
  //------------------------------------------------------------------------------------------------
  // Takes the calculations from the RotatableObject function and applies them to the rotatable 
  // object. Finds the arc tangent of the difference between the center points of the mouse and 
  // orbitBody (gets angle). It converts the radians to degrees and subtracts 90 to make it 
  // perpendicular to the orbitBody, then the object is rotated to the number of degrees specified. 
  // The position of the object is then snapped to "snapRadius" units away from the orbitBody.
  void SnapAndRotate(float diffY, float diffX, Transform orbitBody)
  {
    // Rotate:
    float rads = (float)Math.Atan2(diffY, diffX);
    degrees = (rads * (180 / (float)Math.PI)) - 90;
    transform.localRotation = Quaternion.Euler(0f, 0f, degrees);
    
    // Snap: 
    float radius = snapRadius * orbitBody.localScale.x;
    Vector3 tempPosition = guidePosition;
    tempPosition.x = radius * (float)Math.Cos(rads) + orbitBody.position.x;
    tempPosition.y = radius * (float)Math.Sin(rads) + orbitBody.position.y;
    transform.position = tempPosition;
  }
  
  //------------------------------------------------------------------------------------------------
  void Tutorial.SwitchOnOff.enable () {
    this.enabled = true;
    this.GetComponent<Collider2D> ().enabled = true;
  }
  
  //------------------------------------------------------------------------------------------------
  void Tutorial.SwitchOnOff.transparent(bool value) {
    if (this.GetComponent<Button> () == null ) {
      if (value == true) {
        Roam.setAlpha (this.gameObject, 0.25f);
      } else {
        Roam.setAlpha (this.gameObject, 1.00f);
      }
    } 
    else {
      if (value == true) {
        this.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
      }
      else this.GetComponent<Button>().transition = Selectable.Transition.None;
    }
  }
  
  //------------------------------------------------------------------------------------------------
  void Tutorial.SwitchOnOff.disable() {
    this.enabled = false;
    this.GetComponent<Collider2D> ().enabled = false;
  }
}
