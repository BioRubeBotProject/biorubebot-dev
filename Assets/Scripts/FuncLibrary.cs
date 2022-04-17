/*  File:       Func Library
    Purpose:    This file holds all non-static functions that can be used
                in other scripts. Currently houses Explode() and ExplodeChild()
                which is used in ReceptorLegScript.cs, T_RegCmdCtrl.cs, GTP_CmdCtrl.cs
                and ActiveAdenylylCyclase.cs. 
    Author:     Alyson Mosely
    Created:    Fall 2022
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncLibrary : MonoBehaviour
{
    //GameObject child = null;

        public IEnumerator Explode(GameObject other, GameObject parentObject, ParticleSystem destructionEffect)
    {
        Debug.Log("library was called successfully");
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

    /************************************************************************************/
      public IEnumerator ExplodeChild(GameObject other, GameObject parentObject, GameObject replaceATPWith, ParticleSystem destructionEffect)
    {
        GameObject child = null;
         Debug.Log("ExplodeChild was called successfully");

       
        //Instantiate our one-off particle system
        ParticleSystem explosionEffect     = Instantiate(destructionEffect) as ParticleSystem;
        explosionEffect.transform.position = other.transform.position;

        //Sets explosion effect to be under the parent object.
	    explosionEffect.transform.parent = parentObject.transform;
    
        //play it
        explosionEffect.loop = false;
        explosionEffect.Play();
        
        //child = (GameObject)Instantiate(replaceATPWith, transform.position, Quaternion.identity);
         child = (GameObject)Instantiate(replaceATPWith, parentObject.transform);
         Debug.Log("instantiating here");
         yield return new WaitForSeconds (5f);

        //child.GetComponent<Rigidbody2D> ().isKinematic  = true;
        //child.transform.parent = parentObject.transform;
    
        //destroy the particle system when its duration is up, right
        //it would play a second time.
        Destroy(explosionEffect.gameObject, explosionEffect.duration);
    
        //destroy our game object
        Destroy(other.gameObject);
        Debug.Log("Destroy");
    }
}




