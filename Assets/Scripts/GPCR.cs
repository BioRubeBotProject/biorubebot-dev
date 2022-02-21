/*  Function:   GPCR
    Purpose:    this function handles the collision of the inactive GPCR
                with the Protien Signaler and the transformation into an
                Active GPCR
    Author:     Ryan Wood
    Created:    Fall 2021
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPCR : MonoBehaviour
{
    //these are set in the Prefab
    public GameObject _ActiveGPCR; //the Active GPCR Prefab
    public GameObject parentObject;//Parent object used for unity editor Tree Hierarchy

    private bool winconditionactivated = false;


    #region Private Methods

    /*  Function:   OnTriggerEnter2D(Collider2D)
        Purpose:    this function handles the event that the Protein Signaler
                    collideed with the GPCR, calling the function that transforms
                    it into an Active GPCR
    */
    private void OnTriggerEnter2D(Collider2D other)
    {
        //IF signal protein collides with GPCR
        if(other.gameObject.tag == "ECP" && this.gameObject.name.Equals("GPCR-A(Clone)"))
        {
            //Get reference for parent object in UnityEditor
            parentObject = GameObject.FindGameObjectWithTag ("MainCamera");

            this.gameObject.GetComponent<ActivationProperties>().isActive = false;
            other.GetComponent<ExtraCellularProperties>().changeState(false);
            other.GetComponent<Rigidbody2D>().isKinematic = true;
       
            StartCoroutine(transformReceptor());
            //check if action is a win condition for the scene/level
            if (!winconditionactivated && GameObject.FindWithTag("Win_GPCR_Activated"))
            {
                WinScenario.dropTag("Win_GPCR_Activated");
                winconditionactivated = true;
            }
        }
    }

    /*  Function:   transformReceptor() IEnumerator
        Purpose:    Transforms the GPCR to be an activated GPCR once protein collides with it
    */
    private IEnumerator transformReceptor()
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
