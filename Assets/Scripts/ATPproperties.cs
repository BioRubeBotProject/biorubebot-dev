// **************************************************************
// **** Updated on 10/08/15 by Kevin Means
// **** 1.) added public variable for rotation
// **** 2.) rotates opposite direction for left receptor leg
// **************************************************************
// **** Updated on 10/09/15 by Kevin Means
// **** 1.) Rotation is dynamic based on the incident angle
// **************************************************************

using UnityEngine;
using System.Collections;

public class ATPproperties : MonoBehaviour , ActivationProperties
{
  
    #region Public Fields + Properties + Events + Delegates + Enums
  
    public Color      ActiveColor         = Color.white;
    public Color      NonActiveColor      = Color.gray;
    public bool       allowMovement       = true;
    public bool       spin                = false;
    public bool       m_isActive          = true;
    public Quaternion rotation;
    public float      rotationalDegrees;
  
    #endregion Public Fields + Properties + Events + Delegates + Enums
    #region Public Methods
  
    public bool isActive
    {
        get => m_isActive;
        set => m_isActive = value;
    }

    public void changeState(bool message)
    {
        this.isActive = message;
        if (this.isActive == false)
        {
            this.allowMovement = false;
            this.GetComponent<ATPpathfinding>().enabled = false;
            foreach (Transform child in this.transform)
            {
                if (child.name == "Phosphate Transport Body")
                {
                    child.GetComponent<Renderer>().material.color = NonActiveColor;
                    break;
                }
            }
        }
        else
        {
            this.allowMovement = true;
            foreach (Transform child in this.transform)
            {
                if (child.name == "Phosphate Transport Body")
                {
                    child.GetComponent<Renderer>().material.color = ActiveColor;
                    break;
                }
            }
            this.GetComponent<ATPpathfinding>().enabled = true;
        }
    }
    
    public void ResetState() // Sets state back to starting state
    {
        this.GetComponent<CircleCollider2D>().enabled = true;
        changeState(true);
        this.gameObject.tag = "ATP";
    }

    public void dropOff(string name)
    {
        float rotate = 0;
        float degrees = this.GetComponent<ATPpathfinding>().angleToRotate;
        if(name == "_InnerReceptorFinalLeft")
        {
            rotate = degrees - rotationalDegrees;
        }
        else
        {
            rotate = rotationalDegrees + degrees;
        }

        spin = true;
        rotation = transform.rotation * Quaternion.AngleAxis(rotate, Vector3.back); 
        this.gameObject.GetComponent<ATPpathfinding> ().droppedOff = true;
    }
  
    #endregion Public Methods
  
    #region Private Methods
  
    private void Start()
    {
        ATPproperties objProps = (ATPproperties)this.GetComponent("ATPproperties");
        changeState(objProps.isActive);
    }
  
    private void Update()
    {
        GameObject tracking = this.GetComponent<ATPpathfinding>().trackThis;
        if(tracking != null && tracking.name == "Adenylyl_cyclase-B(Clone)" && !tracking.GetComponent<ActiveAdenylylCyclaseProperties>().isActive)
        {
            ResetState();
        }
        if(this.isActive == false)
        {
            this.allowMovement = false;
        }
        if(this.allowMovement == false)
        {
            this.GetComponent<ATPpathfinding> ().enabled = false;
        }
        if(this.isActive == true) 
        { 
            this.allowMovement = true;
            this.GetComponent<ATPpathfinding> ().enabled = true;
        }
        if(spin) 
        {
            transform.rotation = Quaternion.Slerp (transform.rotation, rotation, 2 * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation,rotation)==0 ) { spin = false; }
        }
    }
  
    #endregion Private Methods
}