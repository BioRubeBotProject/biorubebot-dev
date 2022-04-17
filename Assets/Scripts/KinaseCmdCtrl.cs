/*  Function:   KinaseCmdCtrl
    Purpose:    This is the movement script for the Kinase. A lot is unknown
                about this file, and some analysis and comments will need to
                be done.
                Essentially, though the Kinase works off a system of different
                tags, which tell it to do various actions, and I think other
                Game Objects interact with it differently based on these tags
                as well, although the full list of tags and relevant behaviors
                is not currently clear
                TODO: seperate out the kinase states, and create various scripts for each version.  I e. when it seperates and becomes two new objects. -cb 1/22/22
*/
using UnityEngine;
using System.Collections;

public class KinaseCmdCtrl : MonoBehaviour  //, Roamer.CollectObject
{
    public  GameObject parentObject; //Parent object used for unity editor Tree Hierarchy
    public  GameObject Kinase_P2;
    public  float      timeoutMaxInterval;

    private Roamer       r = new Roamer();
    private GameObject active_G_Protein;
    private GameObject T_Reg;
    private Transform  myTarget;
    public Vector3    midpoint;
    private float      delay;
    private float      timeoutForInteraction;
    private bool       midpointSet;
    private bool[]     midpointAchieved = new bool[2];
    private bool       WinConMet        = false; //used to determine if the win condition has already been met

    // Use this for initialization
    void Start()
    {
        myTarget              = null;
        midpointSet           = false;
        midpointAchieved [0]  = false;
        midpointAchieved [1]  = false;
        active_G_Protein      = null;
        delay                 = 0.0f;
        timeoutForInteraction = 0.0f;
        parentObject          = GameObject.FindGameObjectWithTag("MainCamera");
    }
    
    // Update is called once per physics update
    void FixedUpdate()
    {
        if(timeoutForInteraction > timeoutMaxInterval)
        {
            if(tag == "Kinase_Prep_A" || tag == "Kinase_Prep_B")
            {
                active_G_Protein.GetComponent<G_ProteinCmdCtrl>().resetTarget();
                active_G_Protein = null;
                reset();
                tag = "Kinase";
            }
        }

        if(tag == "Kinase")
        {
            if(active_G_Protein==null)  //in case kinase was reset because it timed out.
            {
                active_G_Protein = BioRubeLibrary.FindClosest(this.gameObject.transform, "FreeG_Protein"); //returns null if no freeGProteins exist.
            } else
            {
                timeoutForInteraction = 0;
                tag = "Kinase_Prep_A";
            } //then roam
            r.Roaming(this.gameObject);
        }
        else if(tag == "Kinase_Prep_A" || tag == "Kinase_Prep_B")
        {
            // if((delay += Time.deltaTime) >= 5.0f)  //wont move for the first 5 seconds.
            // {
            //     if(!midpointSet && tag == "Kinase_Prep_A")
            //    {
            //        midpoint = BioRubeLibrary.CalcMidPoint(active_G_Protein, this.gameObject);
            //         midpointSet = true;
            //    }
            //    else if(r.ApproachMidpoint(active_G_Protein,this.gameObject,midpointAchieved,midpoint, setupVector(), setupRestraint()))
            //    {
            //        setupNextPhase();
            //    }
            //}
            r.moveToDock(this.gameObject, active_G_Protein);
            //else
           // {
            //    r.Roaming(this.gameObject);
           // }
            timeoutForInteraction += Time.deltaTime;
        } 
        else if(tag == "Kinase_Prep_C") 
        {
            if(!midpointAchieved [0] || !midpointAchieved [1])
            {

                midpoint = BioRubeLibrary.CalcMidPoint(active_G_Protein, this.gameObject);
                midpointAchieved[0] = r.ProceedToVector(active_G_Protein,midpoint + new Vector3(0.0f,0.85f,0.0f)); //these values to be changed 
                midpointAchieved[1] = r.ProceedToVector(this.gameObject,midpoint + new Vector3(0.0f,-0.85f,0.0f)); //for snapping kinase to gprotein
            }
            if(midpointAchieved[0] && midpointAchieved[1]) 
            {
                if((delay += Time.deltaTime) >= 3) 
                {
                    GameObject obj       = Instantiate(Kinase_P2,gameObject.transform.position, Quaternion.identity) as GameObject;
                    obj.transform.parent = parentObject.transform; //Sets curent object to be under the parent object.

                    GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(obj);
                    active_G_Protein.GetComponent<G_ProteinCmdCtrl>().resetTarget();
                    Destroy(gameObject);
                }
                else 
                {
                    if(this.gameObject.transform.parent.parent == null)
                    {
                        this.gameObject.GetComponent<Rigidbody2D>().isKinematic   = true;
                        this.gameObject.GetComponent<PolygonCollider2D>().enabled = true;
                        this.gameObject.transform.parent                          = active_G_Protein.transform;
                        this.GetComponent<Rigidbody2D>().simulated = true;
                    }
            //        r.Roaming(active_G_Protein);  //not sure why this exists
                    //determine if win condition has been reached
                    if(!WinConMet &(GameObject.FindWithTag("Win_KinaseTransformation")))
                    {
                        WinScenario.dropTag("Win_KinaseTransformation");
                        WinConMet = true;
                    }
                }
            }
            timeoutForInteraction += Time.deltaTime;
        }
        else if( tag == "Kinase_Phase_2")
        {
            if(T_Reg == null)
                T_Reg = BioRubeLibrary.FindClosest(transform, "T_Reg");
            if (T_Reg != null && myTarget == null)
            {
                delay = 0;
                T_Reg.GetComponent<T_RegCmdCtrl>().GetObject(this.gameObject, "T_Reg_Prep_A");
                myTarget = T_Reg.transform;
           // }
           // else if (myTarget != null && delay >= 1000)
          //  {
          //      myTarget = null;
          //      T_Reg = null;
            } else if(T_Reg != null && myTarget != null)
            {
                r.moveToDock(this.gameObject, T_Reg);
                //Debug.Log("moving to t_reg");
                //delay += 1;
            }
            else if(this.transform.parent.name == "Transcription Regulator2(Clone)")
            {
                //if(this.transform.parent.tag == "T_Reg_with_Phosphate")
                //{
                //    myTarget = null;
                //    T_Reg = null;  //sudo reset, drop this t_reg2 because the r_reg2 has dropped you.  should have been happening from the .reset() that t_reg2 just sent to kinase.
                //}

                //do nothing

            }
            else{
                r.Roaming(this.gameObject);
            }
        }
    }
    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "FreeG_Protein")
        {
            setupNextPhase();
            this.GetComponent<Rigidbody2D>().simulated = false;
        } 

       yield return new WaitForSeconds(1);

    }

  //  private Vector3 setupVector()
 //   {
  //      if(tag == "Kinase_Prep_A")
  //          return new Vector3(-2.0f, 0.0f, 0.0f);
  //      else if(tag == "Kinase_Prep_B")
 //           return new Vector3(0.0f, 1.0f, 0.0f);
 //       else
//            return new Vector3(0.0f, 0.0f, 0.0f);
  //  }

  //  private float setupRestraint()
 //   {
 //       if(tag == "Kinase_Prep_A")
  //          return 3.25f;
 //       else if(tag == "Kinase_Prep_B")
  //          return 1.75f;
  //      else
//            return 0.0f;
  //  }

    private void setupNextPhase()
    {

    //    if(tag == "Kinase_Prep_A")
    //    {
    //        midpointAchieved [0] = midpointAchieved [1] = false;
    //        tag = "Kinase_Prep_B";
    //    }

            midpointAchieved[0] = false;
            midpointAchieved[1] = false;

            this.GetComponent<PolygonCollider2D>().enabled         = true;

            tag   = "Kinase_Prep_C";
            delay = 0.0f;

    }

    public void reset()
    {
        T_Reg                 = null;
        myTarget              = null;
        delay                 = 0;
        midpointSet           = false;
        midpointAchieved[0]   = false;
        midpointAchieved[1]   = false;
        timeoutForInteraction = 0.0f;

        this.GetComponent<PolygonCollider2D>().enabled = true;
    }

    public void resetTarget()
    {
        myTarget = null;
        delay    = 0;
    }

    public void GetObject(GameObject obj, string newTag)
    {
        if(obj.tag == "FreeG_Protein")
        {
            this.gameObject.tag = newTag;
            active_G_Protein = obj;
        }
    }
}