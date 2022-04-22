/*  File:       Spawner
    Purpose:    This file handles the spawning of all the GameObjects. When
                the user clicks and drags something from the Dropdown Menu,
                this file is used to actually spawn the Prefab into a GameObject
                that will move about and interact with other Game Objects
*/

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
using System.Linq;

public class Spawner : MonoBehaviour, Tutorial.SwitchOnOff
{
    public GameObject  parentObject;   // Parent object used for unity editor Tree Hierarchy
    public static bool panning = true; // unknown
    public float       snapRadius;     // the radius for the spawned object to snap to
    public float       snapDistance;   // to rotate, object must be within this relative distance
    public GameObject  spawnedObject;  // for final object instantiation (after user releases mouse)
    public Vector3     spawnLocation;  // for final object instantiation (after user releases mouse)
    public Vector3     guidePosition;  // cannot change "transform.position.x,y, or z" directly

    private GameObject cellMembrane;   // the one and only cellMembrane object for this world
    private Transform  nucleus;        // the nucleus (child) of the cellMembrane
    private float      mouseX;         // mouse x coordinate
    private float      mouseY;         // mouse y coordinate
    private float      degrees;        // calculated # of degrees for object instantiation
    private Vector3    returnLocation; // original location of the "button"
    private Quaternion returnRotation; // orginal rotaion of the "button"

    /*  File:       Update()
        Purpose:    Called once per frame, this function updates the x and y position of
                    the mouse
    */
    void Update()
    {
        mouseX = Input.mousePosition.x;
        mouseY = Input.mousePosition.y;
    }

    /*  Function:   OnMouseDown()
        Purpose:    This is called each time a user clicks the mouse button while hovering over a screen button.
                    Whenever a user clicks on an object button to create it, the member variable "degrees" (of 
                    rotation) is initialized to zero so that normal objects (other than the receptor) will 
                    effectively have an instantiated identity rotation, while the receptor still has the ability
                    to be instantiated with a custom rotation.  This method also saves the original location and 
                    rotation of the object's button in order to place back in the menu. It is determined if the
                    "Cell Membrane" exists at this point as well.  If so, that object is saved as "cellMembrane"
                    for use in other methods, if not, then "null" is saved.  This ensures that it's not called 
                    repeatedly during the many "OnMouseDrag" method calls.
    */
    void OnMouseDown()
    {
        panning = false;
        degrees = 0.0f;
        returnLocation = transform.position;
        returnRotation.eulerAngles = transform.eulerAngles;
        cellMembrane = GameObject.FindGameObjectWithTag("CellMembrane");

        //Get reference for parent object in UnityEditor
        parentObject = GameObject.FindGameObjectWithTag("MainCamera");
        if(cellMembrane != null)
        {
            nucleus = cellMembrane.transform.GetChild(0).gameObject.transform;
        }
        this.GetComponent<Collider2D>().enabled = false;
    }

    /*  Function:   OnMouseDrag()
        Purpose:    Called repeatedly as the user drags the mouse with the mouse button held down while hovering
                    over an object button. "Rotate and Snap" is only called for the Receptor and NPC and only when
                    Cell Membrane is in the world. "guidePosition" is a temporary position variable used for 
                    keeping track of where the mouse is. The function "Camera.main.ScreenToWorldPoint" gets the 
                    mouse coordinates and converts them to world coordinates. The "spawnedObject" position is there 
                    because each object has a certain "Z" height or depth associated with it. Also, since 
                    the objects will be spawned at "camera height" the "+1" is so the object will be in front of 
                    the camera instead of on the same level.
    */
    
    void OnMouseDrag()
    {
        string[] rotatableNames = {"_ReceptorInactive", "NPC", "Right_Receptor_Inactive", "Left_Receptor_Inactive",
                                   "GPCR-A", "ABG-ALL", "Adenylyl_cyclase-A"};

        guidePosition = Camera.main.ScreenToWorldPoint(new Vector3(mouseX, mouseY, spawnedObject.transform.position.z + 1));
        if(cellMembrane != null || spawnedObject.name == "Cell Membrane")
        {
            if(rotatableNames.Any(spawnedObject.name.Contains))
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

    /*  Function:   OnMouseUp()
        Purpose:    Called when user releases mouse button. The "if" statement disallows object creation until the
                    Cell Membrane is in place or if the user is trying to create the Cell Membrane.
        ToDO:       Probably need to refactor this a bit to ensure objects are not spawned outside the cell membrane
                    if they shouldn't be or that they aren't spawned inside the membrane if they shouldn't be
                    NOTE:  Dr. Cline has said that things should be able to be spawned anywhere, even if they dont work where they are spawned.
    */
    void OnMouseUp()
    {
        if((cellMembrane != null || spawnedObject.name == "Cell Membrane"))
        {
            spawnLocation = transform.position;
            GameObject obj = Instantiate(spawnedObject, spawnLocation, Quaternion.Euler(0f, 0f, degrees)) as GameObject;

            //Sets curent object to be under the parent object.
            obj.transform.parent = parentObject.transform;
            GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(obj);
            obj = GameObject.FindGameObjectWithTag("CellMembraneButton") as GameObject;
            if(obj != null)
            {
                obj.SetActive(false);
            }
        }

        transform.position = returnLocation;
        transform.localRotation = returnRotation;
        this.GetComponent<Collider2D>().enabled = true;
        panning = true;
    }

    /*  Function:   ThisIsARotatableObject()
        Purpose:    This is called everytime the mouse drags while holding a certain object. It's purpose is to
                    artificially rotate and place the object correctly in relation to the orbitable body (in this
                    case, either the Cell Membrane or the Nucleus). It starts with the guidePosition (mouse 
                    position relative to the world). It finds the distance between the mouse and the respective 
                    centers of the orbitable bodies.  If the object is within a certain distance of an orbitable 
                    body, it will call the SnapAndRotate function this object. If they are not within a certain
                    distance, then it is treated as any ordinary object and placed in the world at the mouse
                    position with zero rotation.
    */
    void ThisIsARotatableObject()
    {
        float cellDistance = Vector3.Distance(guidePosition, cellMembrane.transform.position);
        float nucDistance  = Vector3.Distance(guidePosition, nucleus.transform.position);

        if(cellDistance < snapDistance * cellMembrane.transform.localScale.x &&
           cellDistance > snapRadius / 1.2)//are we close to the cell membrane wall?
        {
            float cellMemX = guidePosition.x - cellMembrane.transform.position.x;
            float cellMemY = guidePosition.y - cellMembrane.transform.position.y;
            SnapAndRotate(cellMemY, cellMemX, cellMembrane.transform);
        }
        else if(cellDistance < snapRadius / 1.3 &&
                nucDistance < snapDistance * 1.8 * nucleus.transform.localScale.x)//nucleus wall?
        {
            float nucleusX = guidePosition.x - nucleus.transform.position.x;
            float nucleusY = guidePosition.y - nucleus.transform.position.y;
            SnapAndRotate(nucleusY, nucleusX, nucleus);
        }
        else
        {
            transform.localRotation = returnRotation;
            transform.position = guidePosition;
            degrees = 0f;
        }
    }

    /*  Function:   SnapAndRotat(float, float, Transform)
        Purpose:    Takes the calculations from the RotatableObject function and applies them to the rotatable 
                    object. Finds the arc tangent of the difference between the center points of the mouse and 
                    orbitBody (gets angle). It converts the radians to degrees and subtracts 90 to make it 
                    perpendicular to the orbitBody, then the object is rotated to the number of degrees specified. 
                    The position of the object is then snapped to "snapRadius" units away from the orbitBody.
        Parameters: the Cell Membrane's Y - the guide position vector's Y,
                    the Cell Membrane's X - the guide position vector's X,
                    the Cell Membrane's Transform
    */
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

    /************ No idea what the following functions are for ***********/
    //------------------------------------------------------------------------------------------------
    void Tutorial.SwitchOnOff.enable()
    {
        this.enabled = true;
        this.GetComponent<Collider2D>().enabled = true;
    }

    //------------------------------------------------------------------------------------------------
    void Tutorial.SwitchOnOff.transparent(bool value)
    {
        if(this.GetComponent<Button>() == null)
        {
            if(value == true)
            {
                BioRubeLibrary.setAlpha(this.gameObject, 0.25f);
            }
            else
            {
                BioRubeLibrary.setAlpha(this.gameObject, 1.00f);
            }
        }
        else
        {
            if(value == true)
            {
                this.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
            }
            else this.GetComponent<Button>().transition = Selectable.Transition.None;
        }
    }

    //------------------------------------------------------------------------------------------------
    void Tutorial.SwitchOnOff.disable()
    {
        this.enabled = false;
        this.GetComponent<Collider2D>().enabled = false;
    }
}
