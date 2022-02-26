/*  File:       G_ProteinCmdCtrl
    Purpose:    this file contains the movement code for the G-Protein that is
                used in Level one and the intro levels
*/
using UnityEngine;
using System.Collections;

public class G_ProteinCmdCtrl : MonoBehaviour
{
    private static float _speed = 5f;

    public  GameObject GDP;              // for use creating a child of this object
    public  bool       isActive = true;

    private Roamer r;                      //an object that holds the values for the roaming (random movement) methods
    private GameObject childGDP  = null;   // attached GDP, spawns with one of these
    private bool       docked    = false;  // does g-protein position = receptor phosphate position
    private bool       roaming   = false;  // is g-protein free to roam about with GTP in tow
    public bool       haveGTP   = false;  // is g-protein bound to a GTP
    private bool       targeting = false;  // is g-protein targeting phosphate
    private float      delay     = 0;      // used to delay proceed to target and undock
    private float      deltaDistance;      // // measures distance traveled to see if GTP is stuck behind something
    public Transform myTarget = null;           // = openTarget.transform
    private GameObject openTarget;         // a 'ReceptorPhosphate' (target) object
    private Vector3    lastPosition;       // previous position while moving to phosphate
    private Vector3    dockingPosition;    // where to station the g-protein at docking
    private bool winconditionactivated = false;

    /*  Function:   Start()
        Purpose:    This function is called upon instantiation. It instantiates
                    a GDP Game Object as a child on the right side of the GProtein
    */
    private void Start()
    {
        //test
        transform.GetComponent<CircleCollider2D>().enabled = true;
        lastPosition = transform.position;

        //Instantiate a GDP child to tag along
        childGDP = (GameObject)Instantiate (GDP, transform.position + new Vector3(2.2f, 0.28f, 0), Quaternion.identity);
        childGDP.tag = "DockedGDP";

        //set its position and parent
        childGDP.GetComponent<CircleCollider2D> ().enabled = false;
        childGDP.GetComponent<Rigidbody2D> ().isKinematic  = true;
        childGDP.transform.parent                          = transform;

        transform.GetChild(2).GetComponent<SpriteRenderer> ().color = Color.red; 
        transform.GetChild(3).GetComponent<SpriteRenderer> ().color = Color.cyan;

        //initialize the roam object
        r = new Roamer();
    }

    /*  Function:   FixedUpdate()
        Purpose:    this function differs from Update in that, rather than being called once per frame,
                    it may be called once, zero, or multiple times per frame depending on the speed
                    of the physics frames per second. Not sure of the reasoning behind decision to use
                    FixedUpdate rather than Update.
                    Anyway, this function has the G-Protein search around for a Phosphate on a receptor
                    dropped off by an ATP. If the G-Protein has a GTP instead of GDP,
                    seeks out a Protein Kinase with which to bind. If there is nothing to seek,
                    G-Protein roams aimlessly
    */
    private void FixedUpdate()
    {
        //IF G-Protein does not have a GTP(red) AND it does have GDP(blue)
        if(!haveGTP && transform.tag == "OccupiedG_Protein")
            haveGTP = true;

        //IF G-Protein is not targeting a phosphate AND G-Protein is not docked to receptor AND G-Protein does not have a GTP(red)
        if(!targeting && !docked && !haveGTP)
        {
            //Receptor phosphate = closest one to G-Protein
            openTarget = BioRubeLibrary.FindClosest (transform, "ReceptorPhosphate");

            //IF phosphate is found
            if (openTarget != null)
            {
                myTarget        = openTarget.transform;
                dockingPosition = GetOffset();
                LockOn();  //call dibs
            }
        }
        //ELSE IF G-Protein is not docked to receptor AND G-Protein does not have a GTP(red)                *On route to receptor phosphate
        else if(!docked && !haveGTP)
        {
            docked = ProceedToTarget();
            if(docked)
            {
                GetComponent<Rigidbody2D>().simulated = false; //turn off collision so the GTP can slide to the right location
                ReleaseGDP();
            }
        }

        //IF G-Protein has GTP(red) AND G-Protein is not ready to roam with attached GTP(red) AND wait time is over 2 seconds
        if(haveGTP && !roaming && (delay += Time.deltaTime) > 2)
        {
            Undock();
            //check if action is a win condition for the scene/level
            if (!winconditionactivated && GameObject.FindWithTag("Win_GProteinFreed"))
            {
                WinScenario.dropTag("Win_GProteinFreed");
                winconditionactivated = true;
            }
        }
        //ELSE IF G-Protein has GTP(red) AND G-Protein is ready to roam with attached GTP(red)
        else if(haveGTP && roaming)
        {
            GameObject Kinase = BioRubeLibrary.FindClosest (transform, "Kinase");
            if(Kinase != null && !myTarget && isActive)
            {
                delay = 0;
                Kinase.GetComponent <KinaseCmdCtrl> ().GetObject (this.gameObject, "Kinase_Prep_A");
                r.moveToDock(this.gameObject, Kinase);
            }
           
        }
        //ELSE have G-Protein roam
        else if (myTarget  == null)
        {
            r.Roaming (this.gameObject);
        }
    }


    /*  Function:   GetOffset() Vector3
        Purpose:    Determines whether a target is to the  left or right of the receptor
                    and based on the targets position, relative to the receptor, an offset is
                    is figured into the docking position so the g-protein will mate up with the
                    receptor phosphate. If resizing objects these values will have to be changed to ensure
                    GDP snaps to G_Protein properly
        Return:     the position vector for cocking with the receptor's left or right leg
    */
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

    /*  Function:   LockOn()
        Purpose:    LockOn retags the target 'ReceptorPhosphate' to 'target' so it
                    is overlooked in subsequent searches for 'ReceptorPhosphate'.  This
                    and the assigning of a 'dockingPosition' ensures only one g-protein
                    will target an individual receptor phosphate.
    */
    private void LockOn()
    {
        targeting    = true;
        myTarget.tag = "Target";
    }

    /*  Function:   ProceedToTarget() bool
        Purpose:    ProceedToTarget instructs this object to move towards its 'dockingPosition'
                    If this object gets stuck behind the nucleus, it will need a push to
                    move around the object
        Return:     whether we have approached the docking position
    */
    private bool ProceedToTarget()
    {

        //Unity manual says if the distance between the two objects is < _speed * Time.deltaTime,
        //protein position will equal docking...doesn't seem to work, so it's hard coded below
        transform.position = Vector2.MoveTowards(transform.position, dockingPosition, _speed *Time.deltaTime);

        if (Vector2.Distance (transform.position, lastPosition) < _speed * Time.deltaTime)
        {
            //if I didn't move...I'm stuck.  Give me a push
           // r.Roaming(this.gameObject);
        }

        lastPosition = transform.position;//breadcrumb trail

        //check to see how close to the phosphate and disable collider when close
        deltaDistance = Vector3.Distance (transform.position, dockingPosition);

        //once in range, station object at docking position
        if(deltaDistance < _speed * .5f)
        {
            transform.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            transform.GetComponent<Rigidbody2D>().isKinematic = true;
        }

        if(deltaDistance < _speed * Time.deltaTime)
        {
            transform.position = dockingPosition;
    //        if(myTarget.GetChild(0).tag == "Left")
    //        {
    //            childGDP.transform.position = childGDP.transform.position - (new Vector3(2.2f, 0.28f, 0.0f) * 2);
    //        }
        }
        return(transform.position==dockingPosition);
    }

    /*  Function:   ReleaseGDP()
        Purpose:    Once the G-Protein has docked with a receptor phosphate, it
                    is re-tagged "DockedG_Protein" and is then targeted by a
                    roaming GTP (see GTP_CmdCtrl.cs).
                    The GDP is then 'expended' (released and destroyed)
    */
    private void ReleaseGDP()
    {
        delay         = 0;
        targeting     = false;
        transform.tag = "DockedG_Protein";
        childGDP.tag  = "ReleasedGDP";
        transform.eulerAngles = Vector3.zero; //rotate G-Protein so that the GTP will dock in the correct spot.
    }

    /*  Function:   Undock()
        Purpose:    Once a GTP has bound to the g-protein is released from the receptor phosphate
                    and is free to roam about.  The receptor phosphate is retagged to be targeted
                    by another g-protein
    */
    private void Undock()
    {
        transform.GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<Rigidbody2D>().simulated = true;
        transform.GetComponent<CircleCollider2D>().enabled = true;

        docked        = false;
        targeting     = false;
        myTarget.tag  = "ReceptorPhosphate";
        transform.tag = "FreeG_Protein";
        myTarget      = null;
        roaming       = true;
        delay         = 0;
    }

    /*  Function:   resetTarget()
        Purpose:    this function resets isActive to true and sets the target
                    to null, the delay to zero. This function is called by
                    the Kinase movement script when it dismounts from it
    */
    public void resetTarget()
    {
        isActive = true;
        myTarget = null;
        delay    = 0;
    }
}