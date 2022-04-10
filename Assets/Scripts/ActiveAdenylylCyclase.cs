/*  File:       ActiveAdenylylCyclase
    Purpose:    The active Adenylyl Cyclase is connected to the Trimeric
                G-Protein's Alpha subunit. It converts ATP into cAMP.
                This file is connected to the Active version of the Prefab,
                which is instantiated when the Alpha subunit collides with the
                inactive Adenylyl Cyclase.
                This file provides the functions that allow the Active Adenylyl
                Cyclase to turn ATP into cAMP
    Author:     Ryan Wood
    Created:    Fall 2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAdenylylCyclase : MonoBehaviour
{
    public  ParticleSystem destructionEffect;
    public  GameObject     parentObject;      //Parent object used for unity editor Tree Hierarchy
    public  GameObject     replaceATPWith = null;    //what spawns when ATP collides and explodes
    public  GameObject     inactiveCyclase;   //what spawns when this deactivates
    public  GameObject     child;

    /*  Function:   OnTriggerEnter2D(Collider2D) IEnumerator
        Purpose:    this function handles the event that the Active Adenylyl Cyclase
                    colldied with an ATP in the game, calling explode on the ATP
        Parameters: the Collider of the ATP with which the Cyclase collided
        Return:     nothing really imporant
    */
    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "ATP" && other.GetComponent<ATPpathfinding>().found == true)
        {
            other.GetComponent<CircleCollider2D>().enabled = false;
            other.GetComponent<ATPproperties>().changeState(false);

            //Get reference for parent object in UnityEditor
	        parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
      
            yield return new WaitForSeconds(3);
            other.GetComponent<ATPproperties>().changeState(true);
            other.GetComponent<CircleCollider2D>().enabled = true;
            other.gameObject.tag                           = "Untagged";
      
      // StartCoroutine(Explode(other.gameObject)); //self-destruct after 3 seconds
       
        FuncLibrary fl = new FuncLibrary();
        StartCoroutine(fl.ExplodeChild(other.gameObject, parentObject.gameObject, replaceATPWith.gameObject, destructionEffect));
        //StartCoroutine(fl.ExplodeChild(other.gameObject, parentObject.gameObject, child.gameObject, destructionEffect));
        Debug.Log("destroy ATP here"); //prints to console to see if func was successfully called */

        }
    }

     /*   Function:   Explode(GameObject) IEnumerator
        Purpose:    this function causes the given GameObject to explode in the
                    game and sets it to inactive, making it leave the game.
                    in place of the given Object, an instance of replaceATPWith
                    is instantiated. In Unity, this variable is set to the
                    cAMP prefab, so that spawns where the ATP explodes
        Parameters: the ATP to explode
        Return:     nothing important 
    
        public IEnumerator Explode(GameObject other)
    {
        GameObject child = null;

        yield return new WaitForSeconds (3f);
        //Instantiate our one-off particle system
        ParticleSystem explosionEffect     = Instantiate(destructionEffect) as ParticleSystem;
        explosionEffect.transform.position = other.transform.position;

        //Sets explosion effect to be under the parent object.
	    explosionEffect.transform.parent = parentObject.transform;
    
        //play it
        explosionEffect.loop = false;
        explosionEffect.Play();
        
        child = (GameObject)Instantiate(replaceATPWith, transform.position, Quaternion.identity);
        child.GetComponent<Rigidbody2D> ().isKinematic  = true;
        child.transform.parent = parentObject.transform;
    
        //destroy the particle system when its duration is up, right
        //it would play a second time.
        Destroy(explosionEffect.gameObject, explosionEffect.duration);
    
        //destroy our game object
        Destroy(other.gameObject);
    }*/
   
}
