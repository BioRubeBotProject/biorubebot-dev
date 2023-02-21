using UnityEngine;
using System.Collections;
using System;

public class DisableScreen : MonoBehaviour
{
    public Rigidbody test;
    static GameObject[] gratsPanel;
    public bool HidePopup = false;
    // Use this for initialization

    void Start()
    {
        gratsPanel = GameObject.FindGameObjectsWithTag("Congratulations");
        bool isGameBegin = true;


        if(isGameBegin == true)
        {
            // Set active congratulations to ative. Unity does not support activating so had to update
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Congratulations"))
                obj.SetActive(false);
        }
        isGameBegin = false;
    }

    private Rigidbody GetCompnent<T>()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        if (HidePopup == false)
        {
            if (gratsPanel != null)
            {
                GameWon.Set_WinConditions();
                //Set active congratulations to active. Unity does not support activating so had to update
                if (GameWon.IsWon() == true)
                {
                    foreach (GameObject obj2 in gratsPanel)
                    {
                        if (obj2 != null)
                            obj2.SetActive(true);
                    }
                }
            }
            else
            {
                gratsPanel = GameObject.FindGameObjectsWithTag("Congratulations");
            }
        }
    }
}
