using System.Collections;
using UnityEngine;

public class KeyFunction : MonoBehaviour
    {
    // Use this for initialization 
    private void Start()
        {
        }

    private GameObject o = null;

    // Update is called once per frame 
    private void Update()
        {
        if (Input.GetKey(KeyCode.F))
            {
            o = GameObject.Find("_ExtraCellularProteinSignaller");

            Debug.Log("Found: " + o.name);
            }

        if (Input.GetKey(KeyCode.M))
            {
            ExtraCellularProperties myScript = o.GetComponent<ExtraCellularProperties>();
            Debug.Log("Changing movememnt to: " + !myScript.allowMovement);

            myScript.allowMovement = !myScript.allowMovement;
            }
        }
    }