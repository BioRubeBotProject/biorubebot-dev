using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuncLibrary : MonoBehaviour
{
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


      public IEnumerator ExplodeChild(GameObject other, GameObject parentObject, GameObject replaceATPWith, GameObject child, ParticleSystem destructionEffect)
    {
        Debug.Log("ExplodeChild was called successfully");
        //child = null;

        yield return new WaitForSeconds (3f);
        //Instantiate our one-off particle system
        ParticleSystem explosionEffect     = Instantiate(destructionEffect) as ParticleSystem;
        explosionEffect.transform.position = other.transform.position;

        //Sets explosion effect to be under the parent object.
	    explosionEffect.transform.parent = parentObject.transform;
    
        //play it
        explosionEffect.loop = false;
        explosionEffect.Play();
        
        Debug.Log("Move child 0");
        child = (GameObject)Instantiate(replaceATPWith, transform.position, Quaternion.identity);
        Debug.Log("Move child 1");
        child.GetComponent<Rigidbody2D> ().isKinematic  = true;
        Debug.Log("Move child 2");
        child.transform.parent = parentObject.transform;
        Debug.Log("Move child 3");
    
        //destroy the particle system when its duration is up, right
        //it would play a second time.
        Destroy(explosionEffect.gameObject, explosionEffect.duration);
    
        //destroy our game object
        Destroy(other.gameObject);
    }
}




