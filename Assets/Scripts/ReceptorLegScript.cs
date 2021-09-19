// **************************************************************
// **** Updated on 10/08/15 by Kevin Means
// **** 1.) Added condition to prevent rogue ATP from hijacking
// ****     a receptor leg
// **** 2.) dropOff function now takes the name of this object
// ****     (to know which way to rotate)
// **************************************************************
// **** Updated on 10/09/15 by Kevin Means
// **** 1.) Tag now reverts from "ATP_tracking" to "Untagged"
// **************************************************************

using UnityEngine;
using System.Collections;

//Updated 6/27/2015
//Lines 24-25:  Disable ATP collider while dropping off a phosphate
//Lines 34-35:  Enable the ATP collider once phosphate dropped off
//Lines 38-43:  Change receptor leg tags (referenced in G_ProteinCmdCtrl.cs)

//Updated 6/29/2015
//Line 44:  Added call to IEnumerator co-routine 'Explode'
//Lines 47-65:  Added 'Explode' to destroy ATP after dropping phosphate at the receptor


public class ReceptorLegScript : MonoBehaviour
{
  
  public ParticleSystem destructionEffect;


  public GameObject parentObject;            //Parent object used for unity editor Tree Hierarchy
  private bool WinConMet = false;           //used to determine if the win condition has already been met

    private IEnumerator OnTriggerEnter2D(Collider2D other)
  {
        if (other.gameObject.tag == "ATP" && other.GetComponent<ATPpathfinding>().found == true) 
        {                                    // helps prevent rogue ATP from hijacking leg
            ReceptorLegProperties objProps = (ReceptorLegProperties)this.GetComponent("ReceptorLegProperties");
            objProps.isActive = false; 
            objProps.gameObject.tag = "Untagged";
            objProps.GetComponent<CircleCollider2D>().enabled = false;
            other.GetComponent<CircleCollider2D>().enabled = false;
            other.GetComponent<ATPproperties>().changeState(false);
            other.GetComponent<ATPproperties>().dropOff(transform.name);

            //Get reference for parent object in UnityEditor
	        parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
      
            yield return new WaitForSeconds(3);
            Transform tail = other.transform.Find ("Tail");
            tail.transform.SetParent (transform);
            other.GetComponent<ATPproperties>().changeState(true);
            other.GetComponent<CircleCollider2D>().enabled = true;
            other.gameObject.tag = "Untagged";
      
            //code added to identify a 'left' receptor phosphate for G-protein docking
            //if it is a left phosphate, G-protein must rotate to dock
            //NOTE: EACH PHOSPHATE ATTACHED TO A RECEPTOR IS NOW TAGGED AS "receptorPhosphate"
            tail.transform.tag = "ReceptorPhosphate";
            if (transform.name == "_InnerReceptorFinalLeft")
            {
                tail.transform.GetChild(0).tag = "Left";
            }

            StartCoroutine(Explode (other.gameObject)); //self-destruct after 3 seconds
            //determine if win condition has been reached
            if (!WinConMet & (GameObject.FindWithTag("Win_ReceptorPhosphorylation")))
            {
                WinScenario.dropTag("Win_ReceptorPhosphorylation");
                WinConMet = true;
            }
        }

        
  }

  private IEnumerator Explode(GameObject other)
  {
    yield return new WaitForSeconds (3f);
    //Instantiate our one-off particle system
    ParticleSystem explosionEffect = Instantiate(destructionEffect) as ParticleSystem;
    explosionEffect.transform.position = other.transform.position;

    //Sets explosion effect to be under the parent object.
	explosionEffect.transform.parent = parentObject.transform;
    
    //play it
    explosionEffect.loop = false;
    explosionEffect.Play();
    
    //destroy the particle system when its duration is up, right
    //it would play a second time.
    Destroy(explosionEffect.gameObject, explosionEffect.duration);
    
    //destroy our game object
    Destroy(other.gameObject);
  }

}