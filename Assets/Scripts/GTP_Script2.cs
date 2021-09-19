using UnityEngine;
using System.Collections;

public class GTPScript2 : MonoBehaviour 
{
	GameObject GTP;
	GameObject GDP;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		GTP.transform.position = GDP.transform.position;
	}
}
