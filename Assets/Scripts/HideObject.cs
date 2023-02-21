using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObject : MonoBehaviour
{
    public GameObject Congratulations;
    void Start() { }
    void Update() { }

    public void whenButtonClicked()
    {
        if (Congratulations.activeInHierarchy==true)
            Congratulations.SetActive(false);
        else
            Congratulations.SetActive(true);
    }
}
