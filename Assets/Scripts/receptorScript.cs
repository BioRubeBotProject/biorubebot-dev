/*  File:       receptorScript
    Purpose:    This function handles the activation of the Receptor,
                listening for the Collision events that will cause the
                Receptor to transform into an active receptor
*/

// **************************************************************
// **** Updated on 10/08/15 by Kevin Means
// **** 1.) removed excessive "phases" of receptor
// **** 2.) rotates opposite direction for left receptor leg
// **************************************************************

using System.Collections;
using UnityEngine;

public class receptorScript : MonoBehaviour
{
    public GameObject _ActiveReceptor;
    public GameObject parentObject;     //Parent object used for unity editor Tree Hierarchy

    #region Private Methods

    /*  Function:   OnTriggerEnter2D(Collider2D)
        Purpose:    This function handles the event that the receptor came into
                    contact with the Protein Signaler. This kicks off the
                    transformReceptor process, which transforms this Object into
                    an Active receptor
                    Also handles the event that the left receptor collided with
                    the right receptor, which transforms them into active receptor
        Parameters: the Collider of the other Object which may be a Protein Signaler
    */
    private void OnTriggerEnter2D(Collider2D other)
	{
        //test
        //Debug.Log("OnTriggerEnter2D -> object name = " + this.gameObject.name);

        //Get reference for parent object in UnityEditor
		parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
        
        //IF signal protein collides with full receptor (level 1)
        if(other.gameObject.tag == "ECP" && this.gameObject.name.Equals("_ReceptorInactive(Clone)"))
        {
			ExternalReceptorProperties objProps = (ExternalReceptorProperties)this.GetComponent("ExternalReceptorProperties");
			objProps.isActive = false;
			other.GetComponent<ExtraCellularProperties>().changeState(false);
			other.GetComponent<Rigidbody2D>().isKinematic = true;
       
			StartCoroutine(transformReceptor());
            //check if action is a win condition for the scene/level
            if (GameObject.FindWithTag("Win_FullReceptorActivated")) WinScenario.dropTag("Win_FullReceptorActivated");
        }
        //IF signal protein collides with left receptor 
        else if (other.gameObject.tag == "ECP" && this.gameObject.name.Equals("Left_Receptor_Inactive(Clone)"))
        {
            
            ExternalReceptorProperties objProps = (ExternalReceptorProperties)this.GetComponent("ExternalReceptorProperties");
            objProps.isActive = false;
            other.GetComponent<ExtraCellularProperties>().changeState(false);
            other.GetComponent<Rigidbody2D>().isKinematic = true;
      
            StartCoroutine(transformLeftReceptor(other));
            //check if action is a win condition for the scene/level
            if (GameObject.FindWithTag("Win_LeftReceptorWithProtein")) WinScenario.dropTag("Win_LeftReceptorWithProtein");
        }
        //IF right receptor collides with left receptor(with protein signaller)                                                      
        else if (other.gameObject.tag == "RightReceptor" && this.gameObject.name.Equals("Left_Receptor_Active(Clone)"))
        {                
            StartCoroutine(transformLeftReceptorWithProtein(other));
            //check if action is a win condition for the scene/level
            if (GameObject.FindWithTag("Win_ReceptorsCollideWithProtein")) WinScenario.dropTag("Win_ReceptorsCollideWithProtein");
        }

    }

    /*  Function:   transformReceptor() IEnumerator
        Purpose:    this function instantiates an Active Receptor and sets this
                    Game Object to inactive, leaving the game
        Return:     2 second delay
    */
	private IEnumerator transformReceptor()
	{
		yield return new WaitForSeconds(2);
		GameObject NewReceptor = (GameObject)Instantiate(_ActiveReceptor, transform.position, transform.rotation);

        //Sets newReceptor to be under the parent object.
		NewReceptor.transform.parent = parentObject.transform;
        GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add (NewReceptor);
		this.gameObject.SetActive(false);
	}

    /*  Function:   transformLeftReceptor(Collider2D) IEnumerator
        Purpose:    this function instantiates an active Receptor and sets
                    this Game Object inactive, leaving the game
        Parameters: the Collider of the Protein Signaler, which is destroyed
    */
    private IEnumerator transformLeftReceptor(Collider2D other)
    {
        yield return new WaitForSeconds(2);

        //delete protein signaller
        Destroy(other.gameObject);

        GameObject NewReceptor = (GameObject)Instantiate(_ActiveReceptor, transform.position, transform.rotation);

        //Sets newReceptor to be under the parent object.
		NewReceptor.transform.parent = parentObject.transform;
        GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(NewReceptor);
        this.gameObject.SetActive(false);      
    }

    /*  Function:   transformLeftReceptorWithProtein(Collider2D) IEnumerator
        Purpose:    Transforms left receptor(with protein) after right receptor collides
                    this instantiates an active receptor and destroys the right receptor
    */
    
    private IEnumerator transformLeftReceptorWithProtein(Collider2D other)
    {
             
        yield return new WaitForSeconds((float) 0.25);
        other.GetComponent<receptorMovement>().destroyReceptor();

        GameObject NewReceptor = (GameObject)Instantiate(_ActiveReceptor, transform.position, transform.rotation);

        //Sets newReceptor to be under the parent object.
		NewReceptor.transform.parent = parentObject.transform;
        GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(NewReceptor);
        this.gameObject.SetActive(false);

        Destroy(this.gameObject);  
    }

    #endregion Private Methods
}