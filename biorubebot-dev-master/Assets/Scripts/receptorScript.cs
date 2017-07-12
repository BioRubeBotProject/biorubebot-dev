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

    //Parent object used for unity editor Tree Hierarchy
    public GameObject parentObject;
    private bool WinConMet = false; // Fix This later - CG 7/11/17
    
    #region Private Methods

    private void OnTriggerEnter2D(Collider2D other)
	{
        //test
        Debug.Log("OnTriggerEnter2D -> object name = " + this.gameObject.name);

        //Get reference for parent object in UnityEditor
		parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
        
        //IF signal protein collides with full receptor (level 1)
        if(other.gameObject.tag == "ECP" && this.gameObject.name.Equals("_ReceptorInactive(Clone)"))
        {
			ExternalReceptorProperties objProps = (ExternalReceptorProperties)this.GetComponent("ExternalReceptorProperties");
			objProps.isActive = false;
			other.GetComponent<ExtraCellularProperties>().changeState(false);
			other.GetComponent<Rigidbody2D>().isKinematic = true;
       
			StartCoroutine(transformReceptor(other));

            if(!WinConMet && (GameObject.FindWithTag("Win_FullReceptorActivated"))) WinScenario.dropTag("Win_FullReceptorActivated");
        }



        //IF signal protein collides with left receptor 
        else if (other.gameObject.tag == "ECP" && this.gameObject.name.Equals("Left_Receptor_Inactive(Clone)"))
        {
            
            ExternalReceptorProperties objProps = (ExternalReceptorProperties)this.GetComponent("ExternalReceptorProperties");
            objProps.isActive = false;
            other.GetComponent<ExtraCellularProperties>().changeState(false);
            other.GetComponent<Rigidbody2D>().isKinematic = true;
      
            StartCoroutine(transformLeftReceptor(other));   
        }


        //IF right receptor collides with left receptor(with protein signaller)                                                      
        else if (other.gameObject.tag == "RightReceptor" && this.gameObject.name.Equals("Left_Receptor_Active(Clone)"))
        {                
            StartCoroutine(transformLeftReceptorWithProtein(other));         
        }

    }





   	//Transforms full receptor after protein signaller collides
	private IEnumerator transformReceptor(Collider2D other)
	{
		yield return new WaitForSeconds(2);
		GameObject NewReceptor = (GameObject)Instantiate(_ActiveReceptor, transform.position, transform.rotation);

        //Sets newReceptor to be under the parent object.
		NewReceptor.transform.parent = parentObject.transform;
        GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add (NewReceptor);
		this.gameObject.SetActive(false);
	}


    //Transforms left receptor after protein signaller collides
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

    //Transform left receptor(with protein) after right receptor collides
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