using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADP_CmdCtrl : MonoBehaviour
{
    private Roamer r;                             //an object that holds the values for the roaming (random movement) methods
    public float maxHeadingChange;              // max possible rotation angle at a time
    public float angleToRotate;                 // stores the angle in degrees between ATP and dock
    public int minSpeed;                        // slowest the ATP will move
    public int maxSpeed;                        // fastest the ATP will move
    public string trackingTag;                  // objects of this tag are searched for and tracked
    public GameObject trackThis;                // the object with which to dock
     public  ParticleSystem destructionEffect;
     private GameObject parentObject;   
     public GameObject other;
    

    // Start is called before the first frame update
    void Start()
    {
        r = new Roamer(minSpeed, maxSpeed, maxHeadingChange);
    }

    // Update is called once per frame
    public void Update()
    {
         r.Roaming(this.gameObject);

          Object.Destroy(this.gameObject, 6.0f);


      

       /* FuncLibrary fl = new FuncLibrary();
        StartCoroutine(fl.Explode(other.gameObject, parentObject.gameObject, destructionEffect));
        Debug.Log("destroy ATP here"); //prints to console to see if func was successfully called */

        
    }
}
