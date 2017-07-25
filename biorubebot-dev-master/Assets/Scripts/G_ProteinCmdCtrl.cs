using UnityEngine;
using System.Collections;

public class G_ProteinCmdCtrl : MonoBehaviour
{
	private static float _speed = 5f;

	public GameObject GDP;				// for use creating a child of this object
	private GameObject childGDP = null;
	private GameObject Kinase;

    private bool docked = false;		// does g-protein position = receptor phosphate position
	private bool roaming = false;		// is g-protein free to roam about with GTP in tow
	private bool haveGTP = false;		// is g-protein bound to a GTP
	private bool targeting = false;		// is g-protein targeting phosphate
	public bool isActive = true;

	private float delay = 0;			// used to delay proceed to target and undock
	private float deltaDistance;		// // measures distance traveled to see if GTP is stuck behind something
	//private float randomX, randomY;		// random number between MIN/MAX_X and MIN/MAX_Y

	private Transform myTarget; 		// = openTarget.transform
	private GameObject openTarget;		// a 'ReceptorPhosphate' (target) object
	
	//private Vector2 randomDirection;	// new direction vector
	
	private Vector3 lastPosition;		// previous position while moving to phosphate
	private Vector3 dockingPosition;	// where to station the g-protein at docking
	
	private void Start()
	{
        //test
        transform.GetComponent<BoxCollider2D>().enabled = true;



        lastPosition = transform.position;
		
		//Instantiate a GDP child to tag along
		childGDP = (GameObject)Instantiate (GDP, transform.position + new Vector3(2.2f, 0.28f, 0), Quaternion.identity);
		childGDP.GetComponent<CircleCollider2D> ().enabled = false;
		childGDP.GetComponent<Rigidbody2D> ().isKinematic = true;
		childGDP.transform.parent = transform;
		transform.GetChild (2).GetComponent<SpriteRenderer> ().color = Color.white;
		transform.GetChild (3).GetComponent<SpriteRenderer> ().color = Color.white;
	}

	private void FixedUpdate()
	{
      
        //IF G-Protein does not have a GTP(red) AND it does have GDP(blue)
		if (!haveGTP && transform.tag == "OccupiedG_Protein")
        {
            haveGTP = true;
        }
			
        //IF G-Protein is not targeting a phosphate AND G-Protein is not docked to receptor AND G-Protein does not have a GTP(red)
        if (!targeting && !docked && !haveGTP)
        {
            //Receptor phosphate = closest one to G-Protein
			openTarget = Roam.FindClosest (transform, "ReceptorPhosphate");


            //IF phosphate is found
			if (openTarget != null)
            {  


                //Stop movement and set to kinematic   (David 03/05)
                //if(transform.GetComponent<Rigidbody2D>().isKinematic == false)
                //{
                //    transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
                //    transform.GetComponent<BoxCollider2D>().enabled = true;             
                //    transform.GetComponent<Rigidbody2D>().isKinematic = true;

                //    //transform.GetComponent<CircleCollider2D>().enabled = false;
                   
                //}
                                                               
                myTarget = openTarget.transform;
				dockingPosition = GetOffset ();
				LockOn ();  //call dibs
			}

		}

        //ELSE IF G-Protein is not docked to receptor AND G-Protein does not have a GTP(red)                *On route to receptor phosphate
        else if (!docked && !haveGTP)
        {
           
			docked = ProceedToTarget ();
			
			if (docked)
            {
				ReleaseGDP ();
			}
		}

        //IF G-Protein has GTP(red) AND G-Protein is not ready to roam with attached GTP(red) AND wait time is over 2 seconds
        if (haveGTP && !roaming && (delay += Time.deltaTime) > 2)
        {
			Undock ();
            //check if action is a win condition for the scene/level
            if (GameObject.FindWithTag("Win_GProteinFreed")) WinScenario.dropTag("Win_GProteinFreed");
        }



        //ELSE IF G-Protein has GTP(red) AND G-Protein is ready to roam with attached GTP(red)
        else if (haveGTP && roaming)
        {
            /*
            if (Kinase == null) 
            {
				Kinase = Roam.FindClosest (transform, "Kinase");
		    }

			if (Kinase != null || myTarget != null) 
            {
				Roam.FindAndWait (Kinase.GetComponent<KinaseCmdCtrl> (), this.gameObject, ref myTarget, ref delay, "Kinase_Prep_A");
				if (myTarget != null && (delay) >= 5) 
                {

				}
			}
             
            else 
            {
				Roam.Roaming (this.gameObject);
			}
            */

            

			GameObject Kinase = Roam.FindClosest (transform, "Kinase");
			if(Kinase != null && !myTarget && isActive)
            {
				delay = 0;
				Kinase.GetComponent <KinaseCmdCtrl> ().GetObject (this.gameObject, "Kinase_Prep_A");
				myTarget = Kinase.transform;
			}

			if(myTarget && (delay += Time.deltaTime) >= 5)
            {
				isActive = false;
			} 

			else if(isActive == true)
            {
				Roam.Roaming (this.gameObject);
			}

		}

        //ELSE have G-Protein roam
        else
        {
			Roam.Roaming (this.gameObject);
		}

	}


/*	GetOffset determines whether a target is to the  left or right of the receptor
	and based on the targets position, relative to the receptor, an offset is 
	is figured into the docking position so the g-protein will mate up with the
	receptor phosphate.If resizing objects these values will have to be changed to ensure 
	GDP snaps to G_Protein properly */
	private Vector3 GetOffset()
	{
		if (myTarget.GetChild(0).tag == "Left")
		{
			//tag left G-Protein for GTP to reference in GTP_CmdCtrl.cs:
			transform.GetChild(0).tag = "Left"; 
			return myTarget.position + new Vector3 (-2.2f, 0.285f, myTarget.position.z);
		}

		else
        {
            return myTarget.position + new Vector3(2.2f, 0.285f, myTarget.position.z);
        }

	}

/*	LockOn retags the target 'ReceptorPhosphate' to 'target' so it
	is overlooked in subsequent searches for 'ReceptorPhosphate's.  This
	and the assigning of a 'dockingPosition' ensures only one g-protein
	will target an individual receptor phosphate.  */
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

		if (Vector2.Distance (transform.position, lastPosition) < _speed * Time.deltaTime)
        {
            //if I didn't move...I'm stuck.  Give me a push
            Roam.Roaming(this.gameObject);
        }
      
		lastPosition = transform.position;      //breadcrumb trail

		//check to see how close to the phosphate and disable collider when close
		deltaDistance = Vector3.Distance (transform.position, dockingPosition);



		//once in range, station object at docking position
		if (deltaDistance < _speed * .5f)                                   //ORIGINAL      if (deltaDistance < _speed * .5f)
        {
            //TEST
            transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            //transform.GetComponent<CircleCollider2D>().enabled = false;
            //transform.GetComponent<BoxCollider2D>().enabled = false;
            transform.GetComponent<Rigidbody2D>().isKinematic = true;
             
		}



		if (deltaDistance < _speed * Time.deltaTime)
        {
			transform.position = dockingPosition;
			if (myTarget.GetChild(0).tag == "Left")
            {
				//transform.Rotate(180.0f, 0.0f, 0.0f); //orientate protein for docking
				//transform.Rotate(0.0f, 0.0f,180.0f);
				childGDP.transform.position = childGDP.transform.position - (new Vector3(2.2f, 0.28f, 0.0f) * 2);
			}
		}
		return (transform.position==dockingPosition);
	}

/*  Once the G-Protein has docked with a receptor phosphate, it
 	is re-tagged "DockedG_Protein" and is then targeted by a roaming
 	GTP (see GTP_CmdCtrl.cs).  The GDP is then 'expended'
 	(released and destroyed)  */
	private void ReleaseGDP()
	{
		delay = 0;
		targeting = false;
		transform.tag = "DockedG_Protein";
		childGDP.tag = "ReleasedGDP";
	}

//	Once a GTP has bound to the g-protein is released from the receptor phosphate
//	and is free to roam about.  The receptor phosphate is retagged to be targeted
//  by another g-protein
	private void Undock()
	{
		transform.GetComponent<Rigidbody2D>().isKinematic = false;
		transform.GetComponent<BoxCollider2D>().enabled = true;
		docked = false;
		targeting = false;
		myTarget.tag = "ReceptorPhosphate";
		transform.tag = "FreeG_Protein";
		myTarget = null;
		roaming = true;
		delay = 0;
	}

	public void resetTarget()
    {
		isActive = true;
		myTarget = null;
		delay = 0;
	}
}