/*  File:       GTP_CmdCtrl
    Purpose:    This file handles the movement of a GTP. A GTP will seek out
                a G-Protein in level one and intro levels. It will seek out
                a Trimeric G-Protein Alpha subunit in level 2. After being
                attached to an Alpha subunit for a period of time, it hydrolizes
                and self-destructs.
*/

using UnityEngine;
using System.Collections; 
using System;//for math

public class GTP_CmdCtrl: MonoBehaviour
{
    //------------------------------------------------------------------------------------------------
    #region Public Fields + Properties + Events + Delegates + Enums
    public ParticleSystem destructionEffect; //'poof' special effect for 'expended' GDP
    public GameObject GTP1;                // transform GTP upon docking
    public GameObject trackThis;           // the object with which to dock
    public Quaternion rotation;
    public Transform  origin;              // origin location/rotation is the physical GTP
    public string     trackingTag;         // objects of this tag are searched for and tracked
    public float      maxHeadingChange;    // max possible rotation angle at a time
    public float      angleToRotate;       // stores the angle in degrees between GTP and dock
    public bool       droppedOff = false;  // is phospate gone?
    public bool       found      = false;  // did this GTP find a dock?
    public bool       spin       = false;
    public int        maxRoamChangeTime;   // how long before changing heading/speed
    public int        minSpeed;            // slowest the GTP will move
    public int        maxSpeed;            // fastest the GTP will move
    #endregion Public Fields + Properties + Events + Delegates + Enums
    //------------------------------------------------------------------------------------------------
    
    //------------------------------------------------------------------------------------------------
    #region Private Fields + Properties + Events + Delegates + Enums
    private float      heading;          // roaming direction
    private float      headingOffset;    // used for smooth rotation while roaming
    private int        movementSpeed;    // roaming velocity
    private int        roamInterval = 0; // how long until heading/speed change while roaming
    private int        roamCounter  = 0; // time since last heading speed change while roaming
    private int        curveCounter = 90;// used for smooth transition when tracking
    private Quaternion rotate;           // rotation while tracking
    #endregion Private Fields + Properties + Events + Delegates + Enums

    //------------------------------------------------------------------------------------------------
    static float _speed = 5f;   

    private bool docked    = false;     // GTP position = Docked G-protein position
    private bool targeting = false;     // is GTP targeting docked G-protein
    private int timerforlockon;         // has GTP been targeting for too long?
    private int timerforlockonMAX = 500; //when is the GTP never going to arrive because its stuck or behind a membrain

    private float delay = 0f;
    private float deltaDistance;        // measures distance traveled to see if GTP is stuck behind something
    //private float randomX, randomY;       // random number between MIN/MAX_X and MIN/MAX_Y
    
    private GameObject openTarget;      //  found docked g-protein
    private Transform myTarget;         // = openTarget.transform
    
    //private Vector2 randomDirection;  // new direction vector
    private Vector3 dockingPosition;    // myTarget position +/- offset
    private Vector3 lastPosition;
    

    // previous position while moving to docked G-protein

    /*  Function:   Start()
        Purpose:    This function is called upon instantiation and
                    initializes last position to current
    */
    private void Start()
    {
        lastPosition = transform.position;          
    }

    /*  Functin:    Roam2()
        Purpose:    this function has the GTP roam around the cell membrane
                    aimlessly. This is called when there is nothing around
                    for the GTP to seek out and bind with.
                    Must have been named Roam2 in order to be consistent
                    with other functions in this game that do the same thing
                    but not step on the Roam class that is used in this file
    */
    private void Roam2()
    {
        if(Time.timeScale != 0)// if game not paused
        {
            roamCounter++; 
            rotate.z = heading -180;
            if(roamCounter > roamInterval)                         
            {                                                   
                roamCounter   = 0;
                var floor     = Mathf.Clamp(heading - maxHeadingChange, 0, 360);  
                var ceiling   = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
                roamInterval  = UnityEngine.Random.Range(5, maxRoamChangeTime);   
                movementSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);

                if(null != origin)
                {
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
            }
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - headingOffset);
            transform.position += transform.up * Time.deltaTime * movementSpeed;
        }
    }

    /*  Function:   FixedUpdate()
        Purpose:    This function determines whether the GTP should roam around
                    or if there is something for it to seek our and bind with.
                    In level one, looks for a G-Protein that is docked with the
                    Recptor to go bind with. This it finds by looking for a
                    nearby GameObject with the tag "DockedG_Protein". In the
                    second level, looks for a T_GProtein's Alpha Subunit with
                    which to bind. This it finds by searching for a nearby Game
                    Object with the tag "tGProteinDock". Once an appropriate
                    Object is found, the GTP targets the object and tracks
                    to it until it collides with it.
                    This function also continuously checks the GTP's tag for
                    ReleasedGDP, which causes it to explode and be removed from
                    the game
    */
    public void FixedUpdate() 
    {
        GameObject obj       = null;
        GameObject objParent = null;

        if(Time.timeScale > 0)
        { 
            if(spin) 
            {
                transform.rotation = Quaternion.Slerp (transform.rotation, rotation, 2 * Time.deltaTime);
                if(Quaternion.Angle(transform.rotation,rotation) == 0)
                    spin = false;
            }
            if(found == false)
                Roam2();

            if(!targeting)//Look for a target
            {
                Roam2();
                Roam.Roaming (this.gameObject);

                openTarget = Roam.FindClosest(transform, "DockedG_Protein");//level one
                if(null == openTarget)
                {
                    obj = Roam.FindClosest(transform, "tGProteinDock");//level 2
                    if(null != obj)
                    {
                        //get the TGProtien. Doc has parent alpha, which has parent TGProtein
                        objParent = obj.transform.parent.gameObject;
                        if(null != objParent && objParent.name == "alpha")
                            objParent = objParent.transform.parent.gameObject;

                        TGProteinProperties objProps = (TGProteinProperties)objParent.GetComponent("TGProteinProperties");
                        if(objProps.isActive)
                        {
                            openTarget = obj;
                        }
                    }
                }

                if(openTarget != null)
                {
                    myTarget = openTarget.transform;
                    dockingPosition = GetOffset();
                    LockOn();//call dibs
                }
            }
            else if(!docked)  
            {
                if((delay += Time.deltaTime) < 5)//wait 5 seconds before proceeding to target
                    Roam.Roaming(this.gameObject);
                else
                {
                    docked = Roam.ProceedToVector(this.gameObject, dockingPosition);
                    timerforlockon++;
           
                    if(timerforlockon > timerforlockonMAX && !docked) //if timer is high
                    {                                  //reset lockedon,opentarget?,timer, mytarget.tag
                        targeting = false;
                        openTarget = null;
                        myTarget.tag = "tGProteinDock";
                        myTarget = null;
                        obj = null;
                        timerforlockon = 0;
                    }
                    
                }
                if(docked)
                    Cloak();
            }
            if(tag == "ReleasedGTP")
            {
                tag = "DyingGDP";
                StartCoroutine(ReleasingGTP());
                StartCoroutine(DestroyGTP()); //Destroy GDP
            }
        }
    }

    /*  Function:   GetOffset() Vector3
        Purpose:    GetOffset determines whether a target is to the  left or right of the receptor
                    and based on the target's position, relative to the receptor, an offset is 
                    is figured into the docking position so the GTP will mate up with the
                    G-protein.
        Return:     the offset from the target, a smidge to the left or right
    */
    private Vector3 GetOffset()
    {   
        if(myTarget.childCount > 0)//if we have children dealing Level1 G-Protein
        {
            if(myTarget.GetChild(0).tag == "Left")
                return myTarget.position + new Vector3 (-2.2f, 0.28f, 0);
        }
        else
            return myTarget.position;//if no children, Level2 T-G-Protein, just get the target's pos
        return myTarget.position + new Vector3 (2.2f, 0.28f, 0);
    }

    /*  Function:   LockOn()
        Purpose:    retags the target 'DockedG_Protein' to 'Target' so it
                    is overlooked in subsequent searches for 'DockedG_Protein's.  This
                    and the assigning of a 'dockingPosition' ensures only one GTP
                    will target an individual docked g-protein.
    */
    private void LockOn()
    {
        targeting    = true;
        myTarget.tag = "Target";
        timerforlockon = 0;   //sudo fix for issue where GTP is outside the cell wall and or stuck
    }

    //Cloak retags objects for future reference
    private void Cloak()
    {
        transform.GetComponent<CircleCollider2D>().enabled = false;
        transform.GetComponent<Rigidbody2D>().isKinematic = true;

        transform.position = dockingPosition;
        transform.parent   = myTarget;
        myTarget.tag       = "OccupiedG_Protein";
        transform.tag      = "DockedGTP";
        myTarget           = null;

        //determine if win condition has been reached
        if (GameObject.FindWithTag("Win_DockedGTP")) WinScenario.dropTag("Win_DockedGTP");
    }

    public IEnumerator ReleasingGTP ()
    {
        GameObject parentObject = null;
        yield return new WaitForSeconds (3f);

        parentObject     = GameObject.FindGameObjectWithTag ("MainCamera");
        transform.parent = parentObject.transform;

        transform.GetComponent<Rigidbody2D> ().isKinematic  = false;
        transform.GetComponent<CircleCollider2D>().enabled = true;
    } 

    public IEnumerator DestroyGTP()
    {
        GameObject parentObject = GameObject.FindGameObjectWithTag ("MainCamera");

        yield return new WaitForSeconds (2f);
        ParticleSystem explosionEffect     = Instantiate(destructionEffect) as ParticleSystem;
        explosionEffect.transform.parent   = parentObject.transform;
        explosionEffect.transform.position = transform.position;
        explosionEffect.loop = false;
        explosionEffect.Play();
        Destroy(explosionEffect.gameObject, explosionEffect.duration);
        Destroy(gameObject);
    }

}