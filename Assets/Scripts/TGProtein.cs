using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGProtein : MonoBehaviour
{
    public GameObject parentObject;//Parent object used for unity editor Tree Hierarchy

    /*private void OnTriggerEnter2D(Collider2D other)
	{
        Debug.Log("OnTriggerEnter2D -> object name = " + this.gameObject.name);

        //Get reference for parent object in UnityEditor
		parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
        //IF right receptor collides with left receptor(with protein signaller)                                                      
        if(other.gameObject.tag == "GPCR-B" && this.gameObject.name.Equals("AGB-ALL(Clone)"))
        {                
            //StartCoroutine(transformLeftReceptorWithProtein(other));
            //check if action is a win condition for the scene/level
            if(GameObject.FindWithTag("Win_02_TGP_Bound_to_GPCR"))
                WinScenario.dropTag("Win_02_TGP_Bound_to_GPCR");
        }

    }*/
}
