using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PKAMovement : MonoBehaviour
{
    public GameObject activePKA;
    public Transform  origin;            // origin location/rotation is the physical PKA
    public float      maxHeadingChange;  // max possible rotation angle at a time
    public int        maxRoamChangeTime; // how long before changing heading/speed
    public int        minSpeed;          // slowest the GTP will move
    public int        maxSpeed;          // fastest the GTP will move

    private float    heading;               // roaming direction
    private float    headingOffset;         //used for smooth rotation while roaming
    private bool     isSeparated    = false;//whether the PKA has separated from the Kinase
    private int      movementSpeed  = 0;    // roaming velocity
    private int      roamInterval   = 0;    // how long until heading/speed change while roaming
    private int      roamCounter    = 0;    // time since last heading speed change while roaming
    private int      numCamps       = 0;

    private void Roam()
    {
        if(Time.timeScale != 0)// if game not paused
        {
            roamCounter++;                                      
            if(roamCounter > roamInterval)                         
            {
                roamCounter   = 0;
                var floor     = Mathf.Clamp(heading - maxHeadingChange, 0, 360);  
                var ceiling   = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
                roamInterval  = UnityEngine.Random.Range(5, maxRoamChangeTime);   
                movementSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);

                RaycastHit2D collision = Physics2D.Raycast(origin.position, origin.up);
                if(collision.collider != null && collision.collider.name == "Cell Membrane(Clone)" &&
                   collision.distance < 2)
                {
                    if(heading <= 180)
                        heading = heading + 180;
                    else
                        heading = heading - 180;

                    movementSpeed = maxSpeed;
                    roamInterval  = maxRoamChangeTime;
                }
                else
                    heading = UnityEngine.Random.Range(floor, ceiling);

                headingOffset = (transform.eulerAngles.z - heading) / (float)roamInterval;
            }
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - headingOffset);
            transform.position += transform.up * Time.deltaTime * movementSpeed;
        }
    }

    /*  Function:   getPkaWhite() GameObject
        Purpose:    this function retrieves the child of this Game Object
                    that is named "Protein Kinase". THis is the part of the
                    PKA that is white. The other part is blue
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

        pka = getPkaWhite();
        if(null != pka)
        {
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject newCamp  = null;
        GameObject doc      = null;
        GameObject pka      = null;

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
    /*  Function:   getOldPka()
        Purpose:    this function retrieves the child of this Game Object that
                    is tagged inactivePKA. This is the blue part of the PKA that
                    will separate off and transform once this PKA has two cAMPs
                    attached
        Return:     the PKA that will transform
    */
    public GameObject getOldPka()
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

    // Update is called once per frame
    void Update()
    {
        Roam();
        if(this.gameObject.GetComponent<ActivationProperties>().isActive && !isSeparated)
        {
            GameObject oldPKA = getOldPka();
            if(null != oldPKA)
            {
                isSeparated = true;
                this.gameObject.GetComponent<ActivationProperties>().isActive = false;

                GameObject parentObject = this.gameObject;
                GameObject newPKA       = (GameObject)Instantiate(activePKA, oldPKA.transform.position, oldPKA.transform.rotation);
                newPKA.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;

                GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Add(newPKA);
                oldPKA.gameObject.SetActive(false);
                if(GameObject.FindWithTag("Win_PKA_Separates"))
                    WinScenario.dropTag("Win_PKA_Separates");
            }
        }
    }
}
