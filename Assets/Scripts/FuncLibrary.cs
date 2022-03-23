// **************************************************************
//  LIBRARY of non-static functions for BioRubeBot game
//  
//
//
// **** Created on 3/23/2022 by AMosely Spring2022
// **** 1.) Moved Explode function from ReceptorLegScript.cs to here
// **** 2.) 
// **** 3.) 
// **************************************************************
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncLibrary : MonoBehaviour
{
    
    /*
        Function:   Explode(GameObject other, GameObject parentObject, 
                    ParticleSystem destructionEffect) IEnumerator
        Purpose:    This function causes the given GameObject to explode in the
                    game and sets it to inactive, making it leave the game. You will
                    need to declare "ParticleSystem destructionEffect" in the designated
                    script for this function to work.
        Parameters: the object to explode, the exploding objects parent, and 
                    ParticleSystem destructionEffect
        Return:     nothing important
    */
    public IEnumerator Explode(GameObject other, GameObject parentObject, ParticleSystem destructionEffect)
    {
        
        Debug.Log("library was successfully called");

        yield return new WaitForSeconds (3f);

        //Instantiate our one-off particle system
        ParticleSystem explosionEffect = Instantiate(destructionEffect) as ParticleSystem;
        Debug.Log("destructionEffect in static library");
        explosionEffect.transform.position = other.transform.position;

        //Sets explosion effect to be under the parent object.
        explosionEffect.transform.parent



 = parentObject.transform;
    
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


