/*  File:       PKAMovement
    Purpose:    This file handles the movement of the PKA in Level 2 before
                it is activated and breaks into two pieces. The PKA moves
                about the Cell Membrane aimlessly until two cAMPs have
                bound to it. At that point, it transforms and breaks into two
                pieces.
    Author:     Ryan Wood
    Created:    Fall 2021
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PKAMovement : MonoBehaviour
{
    //these public members are set in the Prefab in Unity
    public GameObject activePKA;         // Game Object to spawn once activated
    public float      maxHeadingChange = 20;  // max possible rotation angle at a time
    public int        minSpeed;          // slowest the GTP will move
    public int        maxSpeed;          // fastest the GTP will move

    private Roamer r;                             //an object that holds the values for the roaming (random movement) methods
    private bool     isSeparated    = false;//whether the PKA has separated from the Kinase
    private int      numCamps       = 0;



    /*  Function:   getPkaWhite() GameObject
        Purpose:    this function retrieves the child of this Game Object
                    that is named "Protein Kinase". This is the part of the
                    PKA that is white. The other part is green
        Return:     the Protein Kinase
    */
    private GameObject getPkaWhite()
    {
        GameObject pka   = null;
        bool       found = false;

        foreach(Transform child in transform)
        {
            if(child.gameObject.name == "Protein Kinase")
            {
                pka   = child.gameObject;
                found = true;
                break;
            }
        }
        if(!found)
            pka = null;

        return pka;
    }

    /*  Function:   getDoc() GameObject
        Purpose:    this function retrieves one of the two docs that are
                    used by the cAMPs for docking with the PKA. Once one
                    cAMP has already docked, returns docStation2. Otherwise
                    returns docStation1.
        Return:     the appropriate doc station
    */
    private GameObject getDoc()
    {
        GameObject pka   = null;
        GameObject doc   = null;
        bool       found = false;

        //get the white part of the PKA. The doc stations are a child of this
        pka = getPkaWhite();
        if(null != pka)
        {
            //loop over children of PKAWhite and return appropriate doc
            foreach(Transform child in pka.transform)
            {
                if(child.gameObject.name == "docStation" + (numCamps+1))
                {
                    doc   = child.gameObject;
                    found = true;
                    break;
                }
            }
        }

        if(!found)
            doc = null;

        return doc;
    }

    // Start is called before the first frame update
    void Start()
    {
        r = new Roamer(minSpeed, maxSpeed, maxHeadingChange);
    }

    /*  Function:   OnTriggerEnter2D(Collider2D)
        Purpose:    this function handles the event that the PKA collided
                    with a cAMP Game Object. What happens then is that, if
                    we have less than 2 cAMPs already, we essentiallly absorb
                    the cAMP with which we collided. This function retrieves
                    the appropriate doc station depending on the nubmer of cAMPs
                    we have, and makes the cAMP with which we collided our
                    child. Sets the cAMP's dockedWithPKA variable true so it
                    knows it has been doced already
        Parameters: the Collider of the Object with which PKA collided
    */
    private void OnTriggerEnter2D(Collider2D other)
    {
      //  GameObject newCamp  = null;
        GameObject doc      = null;
       // GameObject pka      = null;

        if(other.gameObject.name == "cAMP(Clone)" && numCamps < 2)
        {
            doc = getDoc();
            if(null != doc)
            {
                other.gameObject.transform.parent = doc.transform;
                other.transform.position = doc.transform.position;
                other.GetComponent<cAmpMovement>().dockedWithPKA = true;
                other.GetComponent<CircleCollider2D>().enabled = false;
                numCamps++;
                if(numCamps > 1)
                    this.GetComponent<ActivationProperties>().isActive = true;
            }
        }
    }

    /*  Function:   getInactivePka()
        Purpose:    this function retrieves the child of this Game Object that
                    is tagged inactivePKA. This is the green part of the PKA that
                    will separate off and transform once this PKA has two cAMPs
                    attached
        Return:     the PKA that will transform
    */
    public GameObject getInactivePka()
    {
        GameObject oldPka = null;
        bool       found  = false;


        foreach(Transform child in this.transform)
        {
            if(child.tag == "inactivePKA")
            {
                oldPka = child.gameObject;
                found  = true;
                break;
            }
        }
        if(!found)
            oldPka = null;

        return oldPka;
    }

    /*  Function:   FixedUpdate()
        Purpose:    this function is called once per physics update. Generally, it just
                    calls the Roam function so that the PKA roams around the
                    Cell Membrane awaiting activation. But, once the PKA becomes
                    acitve, this function will kick off the process of separation
                    for the two parts of the PKA, instantiating the Active Kinase
                    and freeing it from the white portion of the PKA that will
                    continue to roam around with its cAMPs attached
    */
    void FixedUpdate()
    {
        r.Roaming(this.gameObject);
        if(this.gameObject.GetComponent<ActivationProperties>().isActive && !isSeparated)
        {
            GameObject oldPKA = getInactivePka();
            if(null != oldPKA)
            {
                isSeparated = true;
                this.gameObject.GetComponent<ActivationProperties>().isActive = false;

                GameObject parentObject = this.gameObject;
                GameObject newPKA       = (GameObject)Instantiate(activePKA, oldPKA.transform.position, oldPKA.transform.rotation);
                newPKA.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
                newPKA.transform.rotation = new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);

                GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(newPKA);
                oldPKA.gameObject.SetActive(false);
                if(GameObject.FindWithTag("Win_PKA_Separates"))
                    WinScenario.dropTag("Win_PKA_Separates");
            }
        }
    }
}
