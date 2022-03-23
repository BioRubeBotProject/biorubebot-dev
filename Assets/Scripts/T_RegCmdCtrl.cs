/*  File:       T_RegCmdCtrl
    Purpose:    This file handles the movement and seeking of the Transcription
                Regulator. The Transcription Regulator seeks out a Kinase with
                which to bind and transforms once it has bound to one. Also
                awaits an ATP to drop off a phosphate so that it can then
                move into the Nuclear Pore Complex
*/

// **************************************************************
// **** Updated on 10/22/15 by Kevin Means
// **** 1.) Changed the "T_Reg_To_NPC" code to allow the t-reg
// ****     to track to position relative to the NPC and enter
// ****     the cell nucleus.
// **** 2.) Added Physics.IgnoreCollision call so that the T-Reg
// ****     will not pass through the Cell Membrane if the user
// ****     mistakenly puts the NPC there.
// **************************************************************
using UnityEngine;
using System.Collections;
using System;

public class T_RegCmdCtrl : MonoBehaviour
{
    private GameObject     active_Kinase_P2;
    public  GameObject     TReg_P2;
    public  ParticleSystem destructionEffect;
    public  bool           isActive;
    public  static bool    gameWon;
    public  float          timeoutMaxInterval;
    public  float          distanceOffset;
    
    private GameObject Nucleus;
    private GameObject parentObject;            //Parent object used for unity editor Tree Hierarchy
    private Vector3    midpoint;
    private Vector3    ingressDistance;
    private float      delay;
    private float      timeoutForInteraction;
    private bool[]     midpointAchieved = new bool[2];
    private bool       midpointSet;
    private bool       WinConMet = false;             //used to determine if the win condition has already been met
    private float      distancetoconnect;
    private Roamer r;                             //an object that holds the values for the roaming (random movement) methods

    // Use this for initialization
    void Start()
    {
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        this.gameObject.GetComponent<CircleCollider2D> ().enabled = false;

        isActive              = true;
        midpointSet           = false;
        midpointAchieved [0]  = false;
        midpointAchieved [1]  = false;
        active_Kinase_P2      = null;
        delay                 = 0.0f;
        timeoutForInteraction = 0.0f;
        distancetoconnect = 2.0f;  //used to be 6.0f
        Nucleus = GameObject.FindGameObjectWithTag("CellMembrane").transform.GetChild(0).gameObject;
        r = new Roamer();

        //Get reference for parent object in UnityEditor
        parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
    }
    
    void FixedUpdate()
    {
        // Check if the time if the interaction did not complete reset back to before
        // The interaction was setup to occur
        if(timeoutForInteraction > timeoutMaxInterval)
        {
            //If the interaction is in the state of looking for a Kinase
            if(tag == "T_Reg_Prep_A" || tag == "T_Reg_Prep_B")
            {
                tag = "T_Reg"; // Reset the tag to when no interaction was setup to occur
                reset(); // Reset Components of both objects
            }
            else if(tag == "ATP_tracking" )// If ATP is Tracking this Transcription Regulator
            {
                // Reset Components of the Transcription Regulator
                this.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
                this.gameObject.GetComponent<BoxCollider2D>().enabled     = true;
                
                // Reset the Timeout for Interaction
                timeoutForInteraction = 0.0f;
                
                // Set the T_Reg back to is Active
                isActive = true;
                this.tag = "ATP_tracking";
            }
        }

        if(tag == "ATP_tracking")
        {
            this.transform.parent = parentObject.transform; //Sets curent object to be under the parent object.
            active_Kinase_P2      = BioRubeLibrary.FindClosest(transform, "Kinase_Phase_2");

            // Set the kinase's parent to be this T_Reg
            active_Kinase_P2.transform.parent = this.transform;

            // Switch kinase to move with the its parent
            active_Kinase_P2.GetComponent<Rigidbody2D>().isKinematic = true;
            active_Kinase_P2.GetComponent<PolygonCollider2D>().enabled = false;

            // Enable the Box Collider for this T_Reg
            this.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            // Enable the Circle Collider for the ATP to approach and "Dock"
            this.gameObject.GetComponent<CircleCollider2D>().enabled = true;

            // Setup state for an ATP to come and dock with T_Regulator
            timeoutForInteraction = 0;
            delay = 0;
        }
        
        // Default State when nothing is happening, T_Reg will just roam
        if(tag == "T_Reg")
        {
            r.Roaming (this.gameObject);
        } 
        // Else enter the state of approaching a Kinase
        else if(tag == "T_Reg_Prep_A")
        {
          //  if((delay += Time.deltaTime) >= 5.0f) // If Time delay, is less than 5 seconds keep Roaming
            if(active_Kinase_P2 != null)
            {
                if(!midpointSet)// If midpoint not set, setup the midpoint between 
                {
                    // the paired Kinase and this T_Reg
                    midpoint = BioRubeLibrary.CalcMidPoint (active_Kinase_P2, this.gameObject);
                    
                    // Say the has now been set
                    midpointSet = true; 
                } 
                // Else Approach the midpoint, if this point has been achieved setup the next phase T_Reg_Prep_B
                else if(r.ApproachMidpoint(active_Kinase_P2, this.gameObject, midpointAchieved, midpoint, new Vector3(0.0f, 1.75f, 0.0f), 2.5f))
                {
                    delay = 0;
                    
                    // Disable Collider Components
                    active_Kinase_P2.GetComponent<PolygonCollider2D> ().enabled = false;
                    this.GetComponent<BoxCollider2D> ().enabled = false;
                    
                    // Set MidpointAchieved back to false
                    midpointAchieved [0] = midpointAchieved [1] = false;
                    tag                  = "T_Reg_Prep_B";// Enter the next state by changing the tag to T_Reg_Prep_B
                }
            }
            else
            {
                // Continue Roaming for 5 seconds after entering this state
                r.Roaming (this.gameObject);
            }
            // Increment the timeout variable by delta time
            timeoutForInteraction += Time.deltaTime; 
        } 
        // Else if tag is T_Reg_Prep_B, enter the state next phase of approaching the Kinase
        else if(tag == "T_Reg_Prep_B")
        {
            this.GetComponent<BoxCollider2D>().enabled = false;
            active_Kinase_P2.GetComponent<PolygonCollider2D>().enabled = false;

            //If Midpoint has not been achieved, approach the midpoint
            if (!midpointAchieved [0] || !midpointAchieved [1])
            {
                // Proceed to the Kinase
                midpointAchieved [0] = r.ProceedToVector(active_Kinase_P2, midpoint + new Vector3 (0.0f, 0.52f, 0.0f));
                midpointAchieved [1] = r.ProceedToVector(this.gameObject, midpoint + new Vector3 (0.0f, -0.52f, 0.0f));
            }
            // Check if the midpoint has been achieved
            if(midpointAchieved [0] && midpointAchieved [1])
            {
                // Check if the kinase has a parent
                if(active_Kinase_P2.gameObject.transform.parent.parent == null)
                {
                    GameObject obj = Instantiate(TReg_P2, gameObject.transform.position, Quaternion.identity) as GameObject; //(T_Reg phase 2) is now the parent?
                    active_Kinase_P2.transform.rotation = new Quaternion(0, 0, 0, 1); //rotate the (now)child so that it is at 0,0,0 (matching the parent at whatever its rotation is)
                    //determine if win condition has been reached
                    if (!WinConMet & (GameObject.FindWithTag("Win_Kinase_TReg_dock")))
                    {
                        WinScenario.dropTag("Win_Kinase_TReg_dock");
                        WinConMet = true;
                    }

                    Destroy(this.gameObject);  //destroy t_reg normal, because TReg_2 now exists
                }
            }
            //Increment the timeout variable by delta time
            timeoutForInteraction += Time.deltaTime;
        } 
        // Else if tag is ATP_Tracking, wait for an ATP to Collide with the CircleCollider
        else if(tag == "ATP_tracking")
        {
            // Check if the T_Reg is active
            if(isActive == true)
            {
                // Find the Closest ATP
                GameObject ATP = BioRubeLibrary.FindClosest (transform, "ATP");
                
                //Check if the Closest ATP is not null, therefore one exists
                if(ATP != null)
                {
                    // Set the z position for the T_Regulator to be off the 0.0f
                    transform.position = new Vector3(transform.position.x, transform.position.y, 2.0f);
                    
                    // Setup a Vector in 2D because we only care about distance in the x and y
                    Vector2[] pos = new Vector2[2];
                    
                    //Collect the x and y values for this T_Reg and the ATP in separate Vector2 variables
                    pos[0] = new Vector2 (transform.position.x, transform.position.y);
                    pos[1] = new Vector2 (ATP.transform.position.x, ATP.transform.position.y);

                    // Check if the Distance between the the ATP and the T_Reg is less than distancetoconnect
                    if (Vector2.Distance (pos [0], pos [1]) < distancetoconnect)
                    {
                        //Set the T_Reg to be inactive because an ATP is close enough to dock
                        isActive = false;
                        //Disable the box Collider on this T_Reg
                        this.GetComponent<BoxCollider2D> ().enabled = false;
                    } 
                }
                // Roam while the T_Reg is still active
                r.Roaming (this.gameObject);
            }
            
            //Increment the timeout variable by delta time
            timeoutForInteraction += Time.deltaTime;
        } 
        // Else if tag is T_Reg_With_Phosphate, Enter this block
        else if(tag == "T_Reg_With_Phosphate")
        {
            // Check if T_Reg is active
            if(isActive == true)
            {
                // Check if Kinase is still active
                if(active_Kinase_P2 != null)
                {
                    //Enter the state of looking for the nearest NPC
                    this.tag = "T_Reg_To_NPC";
                    //check if action is a win condition for the scene/level
                    if(GameObject.FindWithTag("Win_TranscriptionFactorCompleted"))
                        WinScenario.dropTag("Win_TranscriptionFactorCompleted");
                    
                    //Disable the Circle Collider the ATP was using
                    this.gameObject.GetComponent<CircleCollider2D> ().enabled = false;
                    
                    // Reset the Kinase back to Kinase_Phase_2, when it was looking for a T_Reg
                    active_Kinase_P2.GetComponent<Rigidbody2D> ().isKinematic = false;
                    //Sets curent object to be under the parent object.
                    active_Kinase_P2.transform.parent = parentObject.transform;
                    active_Kinase_P2.GetComponent<KinaseCmdCtrl> ().reset ();
                    active_Kinase_P2.tag = "Kinase_Phase_2";
                    active_Kinase_P2 = null;
                }
                // Roam while T_Reg is Active but not looking for a NPC
                r.Roaming (this.gameObject);
            }
            // Wait 3.5 Seconds after entering the stage where we have a phosphate
            else if((delay += Time.deltaTime) > 3.5f && isActive == false)
            {
                //Time to release the Kinase and start looking for an NPC
                isActive = true;
                this.GetComponent<BoxCollider2D> ().enabled = true;
            }
        } 
        // If tag is T_Reg_To_NPC, then start moving toward the nearest NPC;
        else if (tag == "T_Reg_To_NPC") 
        {
            GameObject NPC = BioRubeLibrary.FindClosest (this.transform, "NPC");
            Transform nucTransform = Nucleus.transform;
            if (NPC != null) 
            { // calculate the distance and the approach vector
                float diffX    = NPC.transform.position.x - nucTransform.position.x;
                float diffY    = NPC.transform.position.y - nucTransform.position.y;
                float distance = (float)Math.Sqrt((diffX * diffX) + (diffY * diffY));
                float rads     = (float)Math.Atan2(diffY, diffX);
                
                Vector3 tempPosition = NPC.transform.position;
                tempPosition.x    = (distance + distanceOffset) * (float)Math.Cos(rads) + nucTransform.position.x;
                tempPosition.y    = (distance + distanceOffset) * (float)Math.Sin(rads) + nucTransform.position.y;
                tempPosition.z    = 2;
                ingressDistance   = tempPosition;
                ingressDistance.x = (distance - distanceOffset) * (float)Math.Cos(rads) + nucTransform.position.x;
                ingressDistance.y = (distance - distanceOffset) * (float)Math.Sin(rads) + nucTransform.position.y;
                // Check and move to the tempPosition, if we have then change state to T_Reg_To_Nucleus
                if (r.ApproachVector (this.gameObject, tempPosition, new Vector3 (0, 0, 2), 0)) 
                {
                    this.tag = "T_Reg_To_Nucleus";
                }
            }
            else
            {
                r.Roaming(this.gameObject);
            }
        } 
        // Check if Tag is T_Reg_To_Nucleus, proceed to the Nucleus
        else if(tag == "T_Reg_To_Nucleus") 
        {
            //Turn off the Collider on the T_Reg so it can pass through the nucleus
            Physics2D.IgnoreCollision(Nucleus.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true);
            
            //Approach the Nucleus's midpoint
            if(r.ProceedToVector(this.gameObject,ingressDistance))// T_Reg is in the Nucleus, Game is won
            {
                Physics2D.IgnoreCollision(Nucleus.GetComponent<Collider2D>(), GetComponent<Collider2D>(), false);
                this.tag = "T_Reg_Complete";
            }
        } 
        else if(tag == "T_Reg_Complete")
        {
            if(GameObject.FindWithTag("Win_TFactorEntersNPC")) WinScenario.dropTag("Win_TFactorEntersNPC");// FOR CONGRATULATIONS SCREEN
                r.Roaming(this.gameObject);
        }
    }
    
    // Enumerator for when the Circle Collider has been hit
    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        // Check if Other is not null and its Tag is ATP and if it has the ATPpathfinding component script
        if(other != null && other.tag == "ATP" && other.GetComponent<ATPpathfinding> ().found == true) 
        {
            if(this.tag == "ATP_tracking")
            {
                //Set the T_Reg to inactive and the tag to T_Reg_With_Phosphate
                this.gameObject.tag = "T_Reg_With_Phosphate";
                this.isActive       = false;
                
                //Disable All Colliders and set the state of ATP Properties to false
                this.GetComponent<BoxCollider2D>().enabled      = false;
                other.GetComponent<CircleCollider2D> ().enabled = false; //turn off collider while dropping off phosphate
                other.GetComponent<ATPproperties> ().changeState (false);
                other.GetComponent<ATPproperties> ().dropOff(transform.name);
                
                // Wait 3 seconds before ATP hands over the phosphate to the T_Reg
                yield return new WaitForSeconds (3);
                Transform tail = other.transform.Find ("Tail");
                tail.transform.SetParent (this.transform);
                this.GetComponent<CircleCollider2D> ().enabled = false;         
                other.GetComponent<ATPproperties> ().changeState (true);
                other.GetComponent<CircleCollider2D> ().enabled = true;
                
                //code added to identify a 'left' receptor phosphate for G-protein docking
                //if it is a left phosphate, G-protein must rotate to dock
                //NOTE: EACH PHOSPHATE ATTACHED TO A RECEPTOR IS NOW TAGGED AS "receptorPhosphate"
                tail.transform.tag      = "T_RegPhosphate";
                tail.transform.localPosition = new Vector3 (0.0f,-0.4f,0.0f);
                
              //  StartCoroutine(Explode(other.gameObject)); //self-destruct after 3 seconds
              FuncLibrary fl = new FuncLibrary();
              StartCoroutine(fl.Explode(other.gameObject, parentObject.gameObject, destructionEffect));
              Debug.Log("destroy ATP here"); //prints to console to see if func was successfully called */
            }
        }
    }
    
  /*  //Enumerator for Exploding the ATP 
    private IEnumerator Explode(GameObject other)
    {
        yield return new WaitForSeconds(3f);
        //Instantiate our one-off particle system
        ParticleSystem explosionEffect = Instantiate(destructionEffect) as ParticleSystem;

        //Sets curent object to be under the parent object.
        explosionEffect.transform.parent   = parentObject.transform;
        explosionEffect.transform.position = other.transform.position;
        
        //play it
        explosionEffect.loop = false;
        explosionEffect.Play();
        
        //destroy the particle system when its duration is up, right
        //it would play a second time.
        Destroy(explosionEffect.gameObject, explosionEffect.duration);
        
        //destroy our game object
        Destroy(other.gameObject);
    } */
    
    // Method to reset the basic to before looking for a Kinase
    private void reset()
    {
        active_Kinase_P2.GetComponent<KinaseCmdCtrl>().resetTarget();

        this.GetComponent<BoxCollider2D>().enabled                 = true;
        active_Kinase_P2.GetComponent<PolygonCollider2D>().enabled = true;

        active_Kinase_P2      = null;
        midpointSet           = false;
        midpointAchieved [0]  = false;
        midpointAchieved [1]  = false;
        delay                 = 0;
        timeoutForInteraction = 0.0f;
    }
    
    // Allows a Kinase to pass itself to this T_Reg by calling this method
    public void GetObject(GameObject obj, string newTag)
    {
        if(obj.tag == "Kinase_Phase_2")
        {
            this.gameObject.tag = newTag;
            active_Kinase_P2    = obj;
        }
    }
}
