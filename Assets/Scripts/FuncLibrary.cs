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
}




