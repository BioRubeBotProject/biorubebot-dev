using UnityEngine;
using System.Collections;

public class Phosphate : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		Roam.Roaming (this.gameObject);
	}
}

