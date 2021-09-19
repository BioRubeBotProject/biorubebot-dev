using UnityEngine;
using System.Collections;

public class GTP_CmdCtrl3: MonoBehaviour
{

	static float _speed = 5f;	
	public Quaternion rotation;
	public bool spin = false;
	private bool docked = false;		// GTP position = Docked G-protein position
	private bool targeting = false;		// is GTP targeting docked G-protein
	
	private float delay = 0f;
	private float deltaDistance;		// measures distance traveled to see if GTP is stuck behind something
	//private float randomX, randomY;		// random number between MIN/MAX_X and MIN/MAX_Y
	
	private GameObject openTarget;		// 	found docked g-protein
	private Transform myTarget;			// = openTarget.transform
	
	//private Vector2 randomDirection;	// new direction vector
	private Vector3 dockingPosition;	// myTarget position +/- offset
	private Vector3 lastPosition;       // previous position while moving to docked G-protein

    private void Start()
	{
		lastPosition = transform.position;			
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
			if (!targeting)//Look for a target
			{
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
                //wait 5 seconds before proceeding to target
                if ((delay += Time.deltaTime) < 5)
                {
                    Roam.Roaming(this.gameObject);
                }
					
				else
                {
					//docked = ProceedToTarget();
					docked = Roam.ProceedToVector(this.gameObject,dockingPosition);
				}

				if (docked)
                {
                    Cloak();
                }
                    
			}
		}
	}

/*	GetOffset determines whether a target is to the  left or right of the receptor
	and based on the targets position, relative to the receptor, an offset is 
	is figured into the docking position so the GTP will mate up with the
	G-protein.  IF SCALING: Must change (x,y) coords here for interaction between GTP & Gprotein*/
	private Vector3 GetOffset()
	{	
		if (myTarget.GetChild(0).tag == "Left")
			return myTarget.position + new Vector3 (-2.65f, 0.13f, 0);
		else
			return myTarget.position + new Vector3 (2.65f, 0.13f, 0);
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
		if (deltaDistance < _speed * Time.deltaTime)
        {
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
		transform.GetComponent<CircleCollider2D> ().enabled = false;
		transform.GetComponent<Rigidbody2D>().isKinematic = true;
		transform.position = dockingPosition;
		transform.parent = myTarget;
		myTarget.tag = "OccupiedG_Protein";
		transform.tag = "DockedGTP";
		myTarget = null;

        //determine if win condition has been reached
        if (GameObject.FindWithTag("Win_DockedGTP")) WinScenario.dropTag("Win_DockedGTP");
    }
}