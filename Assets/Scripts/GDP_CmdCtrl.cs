using UnityEngine;
using System.Collections;

public class GDP_CmdCtrl : MonoBehaviour
{

	public ParticleSystem destructionEffect;	// 'poof' special effect for 'expended' GDP
	public GameObject parentObject; //Parent object used for unity editor Tree Hierarchy

    // Use this for initialization
    void Start ()
    {
        //Get reference for parent object in UnityEditor
		parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		if (tag == "ReleasedGDP")
        {
			tag = "DyingGDP";
			StartCoroutine (ReleasingGDP ());
			StartCoroutine (DestroyGDP ()); //Destroy GDP
            //determine if win condition has been reached
            if (GameObject.FindWithTag("Win_ReleasedGDP")) WinScenario.dropTag("Win_ReleasedGDP");
        }

		Roam.Roaming ( this.gameObject );
	}

	//	ReleasingGDP waits for 3 seconds after docking before actually releasing the GDP  */
	public IEnumerator ReleasingGDP ()
	{
		yield return new WaitForSeconds (3f);
		transform.parent = parentObject.transform;
		transform.GetComponent<Rigidbody2D> ().isKinematic = false;
		transform.GetComponent<CircleCollider2D> ().enabled = true;
	} 

	//	6 seconds after the GDP is released it will be destroyed in a puff of smoke (of sorts)
    // Also sets explosion effect to be under the parent object.
	public IEnumerator DestroyGDP()
	{
		yield return new WaitForSeconds (6f);
		ParticleSystem explosionEffect = Instantiate(destructionEffect) as ParticleSystem;
		explosionEffect.transform.parent = parentObject.transform;
		explosionEffect.transform.position = transform.position;
		explosionEffect.loop = false;
		explosionEffect.Play();
		Destroy(explosionEffect.gameObject, explosionEffect.duration);
		Destroy(gameObject);
	}

}
