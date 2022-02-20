/*  File:       GDP_CmdCtrl
    Purpose:    this file contains the code for the GDP's movement and removal
                from the game
*/
using UnityEngine;
using System.Collections;

public class GDP_CmdCtrl : MonoBehaviour
{
    //these variables are set in the Prefab
    public ParticleSystem destructionEffect; //'poof' special effect for 'expended' GDP
    public GameObject     parentObject;      //Parent object used for unity editor Tree Hierarchy

    private bool winconditionactivated = false;
    private Roamer r;                             //an object that holds the values for the roaming (random movement) methods


    /*  Function:   Start()
        Purpose:    this function is called upon instantiation and initializes
                    the parentObject member variable
    */
    void Start()
    {
        parentObject = GameObject.FindGameObjectWithTag ("MainCamera");
        r = new Roamer();
    }
    
    /*  Function:   FixedUpdate()
        Purpose:    This function checks for the GameObject's tag to have been
                    set to ReleasedGDP. Once that is the case, calls the
                    functions necessary to release this GDP from whatever parent
                    it may be riding with and to destroy it in an explosion
    */
    void FixedUpdate()
    {
        if (tag == "ReleasedGDP")
        {
            r.Roaming(this.gameObject);
            tag = "DyingGDP";
            StartCoroutine(ReleasingGDP());
            StartCoroutine(DestroyGDP());//Destroy GDP
            //determine if win condition has been reached
            if (!winconditionactivated && GameObject.FindWithTag("Win_ReleasedGDP"))
            {
                WinScenario.dropTag("Win_ReleasedGDP");
                winconditionactivated = true;
            }
        }
        else if (tag == "DyingGDP")
        {
            //do nothing
        }
        else if (tag == "DockedGDP")
        {
            //do nothing
        }
        else
        {

            r.Roaming(this.gameObject);  //this should only happen if someone makes one from the menu
        }
    }

    /*  Function:   ReleasingGDP() IEnumerator
        Purpose:    this function waits three seconds and releases the GDP
                    from its parent, enabling its collider
    */
    public IEnumerator ReleasingGDP()
    {
        yield return new WaitForSeconds (3f);
        transform.parent                                    = parentObject.transform;
        transform.GetComponent<Rigidbody2D> ().isKinematic  = false;
        transform.GetComponent<CircleCollider2D> ().enabled = true;
    } 

    /*  Function:   DestroyGDP() IEnumerator
        Purpose:    6 seconds after the GDP is released it will be destroyed in a puff of smoke (of sorts)
                    Also sets explosion effect to be under the parent object.
    */
    public IEnumerator DestroyGDP()
    {
        yield return new WaitForSeconds (6f);
        ParticleSystem explosionEffect     = Instantiate(destructionEffect) as ParticleSystem;
        explosionEffect.transform.parent   = parentObject.transform;
        explosionEffect.transform.position = transform.position;
        explosionEffect.loop               = false;

        explosionEffect.Play();
        Destroy(explosionEffect.gameObject, explosionEffect.duration);
        Destroy(gameObject);
    }
}
