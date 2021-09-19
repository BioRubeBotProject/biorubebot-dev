using UnityEngine;
using System.Collections;

public class KinaseCmdCtrl : MonoBehaviour, Roam.CollectObject
{
	private GameObject active_G_Protein;
	private GameObject T_Reg;
	public GameObject Kinase_P2;
    public GameObject parentObject; //Parent object used for unity editor Tree Hierarchy
	private Transform myTarget;
	private Vector3 midpoint;
	private bool[] midpointAchieved = new bool[2];
	private bool midpointSet;
	private float delay;
	private float timeoutForInteraction;
	public float timeoutMaxInterval;
    private bool WinConMet = false;           //used to determine if the win condition has already been met

    // Use this for initialization
    void Start () {
		myTarget = null;
		midpointSet = false;
		midpointAchieved [0] = false;
		midpointAchieved [1] = false;
		active_G_Protein = null;
		delay = 0.0f;
		timeoutForInteraction = 0.0f;
        parentObject = GameObject.FindGameObjectWithTag("MainCamera"); //Get reference for parent object in UnityEditor
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (timeoutForInteraction > timeoutMaxInterval) {
			if(tag == "Kinase_Prep_A" || tag == "Kinase_Prep_B") {
				active_G_Protein.GetComponent<G_ProteinCmdCtrl>().resetTarget();
				active_G_Protein.GetComponent<BoxCollider2D>().enabled = true;
				active_G_Protein = null;
				reset ();
				tag = "Kinase";
			}
		}

		if (tag == "Kinase") {
			Roam.Roaming (this.gameObject);
		} else if (tag == "Kinase_Prep_A" || tag == "Kinase_Prep_B") {
			if ((delay += Time.deltaTime) >= 5.0f) {
			//if ((delay += Time.deltaTime) >= 5.0f && active_G_Protein != null) {
				if (!midpointSet && tag == "Kinase_Prep_A") {
					midpoint = Roam.CalcMidPoint (active_G_Protein, this.gameObject);
					midpointSet = true;
				} else if (Roam.ApproachMidpoint(active_G_Protein,this.gameObject,midpointAchieved,midpoint, setupVector(), setupRestraint())) {
					setupNextPhase ();
				}
			} else {
				Roam.Roaming (this.gameObject);
			}
			timeoutForInteraction += Time.deltaTime;
		} 
		else if (tag == "Kinase_Prep_C") 
		{
			if(!midpointAchieved [0] || !midpointAchieved [1])
			{
				midpointAchieved[0] = Roam.ProceedToVector(active_G_Protein,midpoint + new Vector3(0.0f,0.85f,0.0f)); //these values to be changed 
				midpointAchieved[1] = Roam.ProceedToVector(this.gameObject,midpoint + new Vector3(0.0f,-0.85f,0.0f)); //for snapping kinase to gprotein
			}
			if(midpointAchieved[0] && midpointAchieved[1]) 
			{
				if((delay += Time.deltaTime) >= 3) 
				{
					GameObject obj = Instantiate(Kinase_P2,gameObject.transform.position, Quaternion.identity) as GameObject;
                    obj.transform.parent = parentObject.transform; //Sets curent object to be under the parent object.
          			GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add (obj);
					active_G_Protein.GetComponent<G_ProteinCmdCtrl>().resetTarget();
					Destroy (gameObject);
				}
				else 
				{
					if(this.gameObject.transform.parent.parent == null)
					{
						this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
						this.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
						this.gameObject.transform.parent = active_G_Protein.transform;
						active_G_Protein.GetComponent<BoxCollider2D>().enabled = true;
					}
					Roam.Roaming(active_G_Protein);
                    //determine if win condition has been reached
                    if (!WinConMet & (GameObject.FindWithTag("Win_KinaseTransformation")))
                    {
                        WinScenario.dropTag("Win_KinaseTransformation");
                        WinConMet = true;
                    }
                }
			}
			timeoutForInteraction += Time.deltaTime;
		}
		else if ( tag == "Kinase_Phase_2") {
			if(T_Reg == null){
				T_Reg = Roam.FindClosest (transform, "T_Reg");
			}
			
			/*if(T_Reg != null ) {
				Roam.FindAndWait(T_Reg.GetComponent<T_RegCmdCtrl>(),this.gameObject,ref myTarget,ref delay,"T_Reg_Prep_A");
			}
			else {
				Roam.Roaming(this.gameObject);
			}*/
			if( T_Reg != null && !myTarget) {
				delay = 0;
				T_Reg.GetComponent <T_RegCmdCtrl>().GetObject(this.gameObject,"T_Reg_Prep_A");
				myTarget = T_Reg.transform;
			}
			if (myTarget && (delay += Time.deltaTime) >= 5) {

			} 
			else {
				Roam.Roaming (this.gameObject);
			}
		}
	}

	private Vector3 setupVector(){
		if (tag == "Kinase_Prep_A") {
			return new Vector3 (-2.0f, 0.0f, 0.0f);
		} else if (tag == "Kinase_Prep_B") {
			return new Vector3 (0.0f, 1.0f, 0.0f);
		} else {
			return new Vector3 (0.0f, 0.0f, 0.0f);
		}
	}

	private float setupRestraint () {
		if (tag == "Kinase_Prep_A") {
			return 3.25f;
		} else if (tag == "Kinase_Prep_B") {
			return 1.75f;
		} else {
			return 0.0f;
		}
	}

	private void setupNextPhase() {
		if (tag == "Kinase_Prep_A") {
			midpointAchieved [0] = midpointAchieved [1] = false;
			tag = "Kinase_Prep_B";
		} else if (tag == "Kinase_Prep_B") {
			midpointAchieved [0] = midpointAchieved [1] = false;
			this.GetComponent<PolygonCollider2D>().enabled = false;
			active_G_Protein.GetComponent<BoxCollider2D>().enabled = false;
			tag = "Kinase_Prep_C";
			delay = 0.0f;
		} else if (tag == "Kinase_Prep_C") {
			
		}
	}

	/*private bool ApproachMidpoint (Vector3 Offset, float Restraint) {
		if (!midpointAchieved [0]) {
			midpointAchieved [0] = Roam.ApproachVector (active_G_Protein, midpoint, Offset, Restraint);
		}
	
		if (!midpointAchieved [1]) {
			midpointAchieved [1] = Roam.ApproachVector (this.gameObject, midpoint, -1 * Offset, Restraint);
		}
		return (midpointAchieved [0] && midpointAchieved [1]);
	}*/

	public void reset() {
		T_Reg = null;
		myTarget = null;
		this.GetComponent<PolygonCollider2D>().enabled = true;
		midpointSet = false;
		midpointAchieved [0] = midpointAchieved [1] = false;
		delay = 0;
		timeoutForInteraction = 0.0f;
	}

	public void resetTarget() {
		myTarget = null;
		delay = 0;
	}

	public void GetObject (GameObject obj, string newTag) {
		if (obj.tag == "FreeG_Protein") {
			this.gameObject.tag = newTag;
			active_G_Protein = obj;
		}
	}
}

