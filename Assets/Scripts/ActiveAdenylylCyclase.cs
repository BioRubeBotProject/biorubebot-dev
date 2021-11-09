using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAdenylylCyclase : MonoBehaviour
{
    public  ParticleSystem destructionEffect;
    public  GameObject     parentObject;      //Parent object used for unity editor Tree Hierarchy
    public  GameObject     replaceATPWith; //what spawns when ATP collides and explodes
    private bool           WinConMet = false; //used to determine if the win condition has already been met

    private IEnumerator OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "ATP" && other.GetComponent<ATPpathfinding>().found == true)
        {
            /*objProps.isActive = false; 
            objProps.gameObject.tag = "Untagged";
            objProps.GetComponent<CircleCollider2D>().enabled = false;*/
            print("Got hit");

            other.GetComponent<CircleCollider2D>().enabled = false;
            other.GetComponent<ATPproperties>().changeState(false);

            //Get reference for parent object in UnityEditor
	        parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
      
            yield return new WaitForSeconds(3);
            other.GetComponent<ATPproperties>().changeState(true);
            other.GetComponent<CircleCollider2D>().enabled = true;
            other.gameObject.tag = "Untagged";
      
            StartCoroutine(Explode (other.gameObject)); //self-destruct after 3 seconds
        }
    }

    private IEnumerator Explode(GameObject other)
    {
        GameObject child = null;

        yield return new WaitForSeconds (3f);
        //Instantiate our one-off particle system
        ParticleSystem explosionEffect = Instantiate(destructionEffect) as ParticleSystem;
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
    }
}
