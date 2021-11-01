using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPCR : MonoBehaviour
{
    public GameObject _ActiveGPCR;
    public GameObject parentObject; //Parent object used for unity editor Tree Hierarchy

    #region Private Methods

    private void OnTriggerEnter2D(Collider2D other)
    {
        //test
        Debug.Log("OnTriggerEnter2D -> object name = " + this.gameObject.name);

        //Get reference for parent object in UnityEditor
        parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
        
        //IF signal protein collides with GPCR
        if(other.gameObject.tag == "ECP" && this.gameObject.name.Equals("GPCR-A(Clone)"))
        {
            GPCRProperties objProps = (GPCRProperties)this.GetComponent("GPCRProperties");
            objProps.isActive = false;
            other.GetComponent<ExtraCellularProperties>().changeState(false);
            other.GetComponent<Rigidbody2D>().isKinematic = true;
       
            //StartCoroutine(transformReceptor(other));
            //check if action is a win condition for the scene/level
            //if(GameObject.FindWithTag("Win_FullReceptorActivated")) WinScenario.dropTag("Win_FullReceptorActivated");
        }
    }

    /*//Transforms full receptor after protein signaller collides
    private IEnumerator transformReceptor(Collider2D other)
    {
        yield return new WaitForSeconds(2);
        GameObject NewReceptor = (GameObject)Instantiate(_ActiveGPCR, transform.position, transform.rotation);

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

        GameObject NewReceptor = (GameObject)Instantiate(_ActiveGPCR, transform.position, transform.rotation);

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

        GameObject NewReceptor = (GameObject)Instantiate(_ActiveGPCR, transform.position, transform.rotation);

        //Sets newReceptor to be under the parent object.
        NewReceptor.transform.parent = parentObject.transform;
        GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(NewReceptor);
        this.gameObject.SetActive(false);

        Destroy(this.gameObject);  
    }*/

    #endregion Private Methods
}
