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
        //Get reference for parent object in UnityEditor
        parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
        
        //IF signal protein collides with GPCR
        if(other.gameObject.tag == "ECP" && this.gameObject.name.Equals("GPCR-A(Clone)"))
        {
            GPCRProperties objProps = (GPCRProperties)this.GetComponent("GPCRProperties");
            objProps.isActive = false;
            other.GetComponent<ExtraCellularProperties>().changeState(false);
            other.GetComponent<Rigidbody2D>().isKinematic = true;
       
            StartCoroutine(transformReceptor(other));
            //check if action is a win condition for the scene/level
            if(GameObject.FindWithTag("Win_GPCR_Activated"))
                WinScenario.dropTag("Win_GPCR_Activated");
        }
    }

    //Transforms full receptor after protein signaller collides
    private IEnumerator transformReceptor(Collider2D other)
    {
        yield return new WaitForSeconds(2);
        GameObject NewGPCR = (GameObject)Instantiate(_ActiveGPCR, transform.position, transform.rotation);

        //Sets newReceptor to be under the parent object.
        NewGPCR.transform.parent = parentObject.transform;
        GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(NewGPCR);
        this.gameObject.SetActive(false);
    }
    #endregion Private Methods
}
