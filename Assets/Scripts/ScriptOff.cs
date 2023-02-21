using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptOff : MonoBehaviour
{
    public GameObject Canvas;
    public DisableScreen diss;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        Canvas.SetActive(true);
        diss.HidePopup = true;
    }
}
