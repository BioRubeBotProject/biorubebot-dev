using UnityEngine;
using System.Collections; 
using System;									//for math

public class GTP_CmdCtrl: MonoBehaviour
{
	//------------------------------------------------------------------------------------------------
	#region Public Fields + Properties + Events + Delegates + Enums
	public float maxHeadingChange;              // max possible rotation angle at a time
	public float angleToRotate;                 // stores the angle in degrees between GTP and dock
	public int maxRoamChangeTime;               // how long before changing heading/speed
	public int minSpeed;                        // slowest the GTP will move
	public int maxSpeed;                        // fastest the GTP will move
	public GameObject GTP1;					// transform GTP upon docking
	public bool droppedOff = false;             // is phospate gone?
	public bool found = false;                  // did this GTP find a dock?
	public string trackingTag;                  // objects of this tag are searched for and tracked
	public GameObject trackThis;                // the object with which to dock
	public Transform origin;                    // origin location/rotation is the physical GTP
	public Quaternion rotation;
	public bool spin = false;
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
	static float _speed = 5f;	

	private bool docked = false;		// GTP position = Docked G-protein position
	private bool targeting = false;		// is GTP targeting docked G-protein
	
	private float delay = 0f;
	private float deltaDistance;		// measures distance traveled to see if GTP is stuck behind something
	//private float randomX, randomY;		// random number between MIN/MAX_X and MIN/MAX_Y
	
	private GameObject openTarget;		// 	found docked g-protein
	private Transform myTarget;			// = openTarget.transform
	
	//private Vector2 randomDirection;	// new direction vector
	private Vector3 dockingPosition;	// myTarget position +/- offset
	private Vector3 lastPosition;
	// previous position while moving to docked G-protein

	private void Start()
	{
		lastPosition = transform.position;			
	}

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

	private void Roam2()
	{
		if(Time.timeScale != 0)                               // if game not paused
		{
			roamCounter++; 
			rotate.z = heading -180;
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

	public void FixedUpdate() 
	{
		if (Time.timeScale > 0)
		{ 
			if (spin) 
			{
				transform.rotation = Quaternion.Slerp (transform.rotation, rotation, 2 * Time.deltaTime);
				if (Quaternion.Angle(transform.rotation,rotation)==0 ) { spin = false; }
			}
			if(found == false) { Roam2(); }
			if (!targeting)//Look for a target
			{
				Roam2();
				//rigidbody.AddForce(transform.forward);
				Roam.Roaming (this.gameObject);
				openTarget = Roam.FindClosest (transform, "DockedG_Protein");
				if (openTarget != null)
				{
					myTarget = openTarget.transform;
					dockingPosition = GetOffset ();
					LockOn ();//call dibs
				}
			}
			
			else if (!docked)
			{
				if ((delay += Time.deltaTime) < 5)//wait 5 seconds before proceeding to target
					Roam.Roaming (this.gameObject);
					//Roam2();
				else {
					//docked = ProceedToTarget();
					docked = Roam.ProceedToVector(this.gameObject,dockingPosition);
				}
				if (docked)	Cloak ();
			}
		}
	}

/*	GetOffset determines whether a target is to the  left or right of the receptor
	and based on the targets position, relative to the receptor, an offset is 
	is figured into the docking position so the GTP will mate up with the
	G-protein.  */
	private Vector3 GetOffset()
	{	
		if (myTarget.GetChild(0).tag == "Left")
			return myTarget.position + new Vector3 (-2.2f, 0.28f, 0);
		else
			return myTarget.position + new Vector3 (2.2f, 0.28f, 0);
	}
	
/*	LockOn retags the target 'DockedG_Protein' to 'Target' so it
	is overlooked in subsequent searches for 'DockedG_Protein's.  This
	and the assigning of a 'dockingPosition' ensures only one GTP
	will target an individual docked g-protein.  */
	private void LockOn()
	{
		targeting = true;
		myTarget.tag = "Target";
	}

/*	ProceedToTarget instructs this object to move towards its 'dockingPosition'
 	If this object gets stuck behind the nucleus, it will need a push to
 	move around the object  */
	private bool ProceedToTarget()
	{
		//Unity manual says if the distance between the two objects is < _speed * Time.deltaTime,
		//protein position will equal docking...doesn't seem to work, so it's hard coded below
		transform.position = Vector2.MoveTowards (transform.position, dockingPosition, _speed *Time.deltaTime);
		
		if (!docked && Vector2.Distance (transform.position, lastPosition) < _speed * Time.deltaTime)
			Roam.Roaming (this.gameObject);//if I didn't move...I'm stuck.  Give me a push
		lastPosition = transform.position;//breadcrumb trail
		//check to see how close to the g-protein and disable collider when close
		deltaDistance = Vector3.Distance (transform.position, dockingPosition);
		//once in range, station object at docking position
		if (deltaDistance < _speed * Time.deltaTime) {
			transform.GetComponent<CircleCollider2D> ().enabled = false;
			transform.GetComponent<Rigidbody2D>().isKinematic = true;
			transform.position = dockingPosition;
			transform.parent = myTarget;
		}//end if close enough
		return (transform.position==dockingPosition);
	}

//	Cloak retags objects for future reference
	private void Cloak()
	{
		//GTP_Script2 ();
		//GameObject obj = Instantiate(GTP1,gameObject.transform.position, Quaternion.identity) as GameObject;
		//GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add (obj);
		transform.GetComponent<CircleCollider2D> ().enabled = false;
		transform.GetComponent<Rigidbody2D>().isKinematic = true;
		transform.position = dockingPosition;
		transform.parent = myTarget;
		myTarget.tag = "OccupiedG_Protein";
		transform.tag = "DockedGTP";
		myTarget = null;
	}
}