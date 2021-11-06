using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGProteinProperties : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates + Enums
  
    public bool       allowMovement       = true;
    public bool       spin                = false;
    public bool       m_isActive          = true;//GDP is attached
    public Quaternion rotation;
    public float      rotationalDegrees;
  
    #endregion Public Fields + Properties + Events + Delegates + Enums
    #region Public Methods
  
    public bool isActive
    {
        get => m_isActive;
        set => m_isActive = value;
    }

    /*public void changeState(bool message)
    {
        this.isActive = message;
        if(this.isActive == false)
        {
            this.allowMovement = false;
            this.GetComponent<TGProteinMovement>().enabled = false;
            foreach(Transform child in this.transform)
            {
                if(child.name == "Phosphate Transport Body")
                {
                    //child.GetComponent<Renderer>().material.color = NonActiveColor;
                    break;
                }
            }
        }
        else
        {
            this.allowMovement = true;
            foreach(Transform child in this.transform)
            {
                if (child.name == "Phosphate Transport Body")
                {
                    //child.GetComponent<Renderer>().material.color = ActiveColor;
                    break;
                }
            }
            this.GetComponent<ATPpathfinding>().enabled = true;
        }
    }
  
    public void dropOff(string name)
    {
        float rotate  = 0;
        float degrees = this.GetComponent<ATPpathfinding>().angleToRotate;

        if(name == "_InnerReceptorFinalLeft")
        {
            rotate = degrees - rotationalDegrees;
        }
        else
        {
            rotate = rotationalDegrees + degrees;
        }

        spin     = true;
        rotation = transform.rotation * Quaternion.AngleAxis(rotate, Vector3.back); 
        this.gameObject.GetComponent<ATPpathfinding> ().droppedOff = true;
    }
  
    #endregion Public Methods
    #region Private Methods
  
    private void Start()
    {
        changeState(false);
    }
  
    private void Update()
    {
        if(this.isActive == false)
        {
            this.allowMovement = false;
        }

        if(this.allowMovement == false)
        {
            this.GetComponent<TGProteinMovement>().enabled = false;
        }

        if(this.isActive == true) 
        { 
            this.allowMovement = true;
            this.GetComponent<ATPpathfinding> ().enabled = true;
        }

        if(spin) 
        {
            transform.rotation = Quaternion.Slerp (transform.rotation, rotation, 2 * Time.deltaTime);
            if(Quaternion.Angle(transform.rotation,rotation) == 0)
                spin = false;
        }
    }*/
    #endregion Private Methods
}
