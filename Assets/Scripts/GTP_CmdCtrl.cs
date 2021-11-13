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

    private bool docked = false;        // GTP position = Docked G-protein position
    private bool targeting = false;     // is GTP targeting docked G-protein
    
    private float delay = 0f;
    private float deltaDistance;        // measures distance traveled to see if GTP is stuck behind something
    //private float randomX, randomY;       // random number between MIN/MAX_X and MIN/MAX_Y
    
    private GameObject openTarget;      //  found docked g-protein
    private Transform myTarget;         // = openTarget.transform
    
    //private Vector2 randomDirection;  // new direction vector
    private Vector3 dockingPosition;    // myTarget position +/- offset
    private Vector3 lastPosition;
    // previous position while moving to docked G-protein

    private void Start()
    {
        lastPosition = transform.position;          
    }

    private void Raycasting()
    {
        if(null != trackThis)
        {
            origin = trackThis.transform;
        }

        Debug.Log(trackThis.name);
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

    private void Roam2()
    {
        if(Time.timeScale != 0)                               // if game not paused
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
                    dockingPosition = GetOffset ();
                    LockOn ();//call dibs
                }
            }
            
            else if(!docked)
            {
                if((delay += Time.deltaTime) < 5)//wait 5 seconds before proceeding to target
                    Roam.Roaming (this.gameObject);
                else
                {
                    docked = Roam.ProceedToVector(this.gameObject,dockingPosition);
                }
                if (docked) Cloak ();
            }
            if(tag == "ReleasedGTP")
            {
                tag = "DyingGDP";
                StartCoroutine (ReleasingGTP ());
                StartCoroutine (DestroyGTP ()); //Destroy GDP
            }
        }
    }

/*  GetOffset determines whether a target is to the  left or right of the receptor
    and based on the targets position, relative to the receptor, an offset is 
    is figured into the docking position so the GTP will mate up with the
    G-protein.*/
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
    
/*  LockOn retags the target 'DockedG_Protein' to 'Target' so it
    is overlooked in subsequent searches for 'DockedG_Protein's.  This
    and the assigning of a 'dockingPosition' ensures only one GTP
    will target an individual docked g-protein.  */
    private void LockOn()
    {
        targeting    = true;
        myTarget.tag = "Target";
    }

    /*ProceedToTarget instructs this object to move towards its 'dockingPosition'
      If this object gets stuck behind the nucleus, it will need a push to
      move around the object  */
    private bool ProceedToTarget()
    {
        //Unity manual says if the distance between the two objects is < _speed * Time.deltaTime,
        //protein position will equal docking...doesn't seem to work, so it's hard coded below
        transform.position = Vector2.MoveTowards (transform.position, dockingPosition, _speed *Time.deltaTime);
        
        if(!docked && Vector2.Distance (transform.position, lastPosition) < _speed * Time.deltaTime)
            Roam.Roaming (this.gameObject);//if I didn't move...I'm stuck.  Give me a push

        lastPosition = transform.position;//breadcrumb trail
        //check to see how close to the g-protein and disable collider when close
        deltaDistance = Vector3.Distance (transform.position, dockingPosition);
        //once in range, station object at docking position
        if(deltaDistance < _speed * Time.deltaTime)
        {
            transform.GetComponent<CircleCollider2D> ().enabled = false;
            transform.GetComponent<Rigidbody2D>().isKinematic = true;
            transform.position = dockingPosition;
            transform.parent = myTarget;
        }//end if close enough
        return (transform.position==dockingPosition);
    }

    //Cloak retags objects for future reference
    private void Cloak()
    {
        transform.GetComponent<CircleCollider2D> ().enabled = false;
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