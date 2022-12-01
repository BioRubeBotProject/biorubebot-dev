//Candidate for deltion. Check for references and remove
using UnityEngine;
using System.Collections;

public class Phosphate : MonoBehaviour
{
    private Roamer r;                             //an object that holds the values for the roaming (random movement) methods

    // Use this for initialization
    void Start ()
	{
        r = new Roamer();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		r.Roaming (this.gameObject);
	}
}

