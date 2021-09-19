// Altered by CS452 Spring 2017 Project Team
// Date: March 18, 2017

//Functionality Added: Replaced IF statements with SWITCH statments for efficency

using UnityEngine;

public class ExternalReceptorProperties : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates + Enums

    public Color ActiveColor = Color.white;
    public bool allowMovement = true;
    public bool isActive = true;
    public Color NonActiveColor = Color.gray;

    #endregion Public Fields + Properties + Events + Delegates + Enums

    #region Public Methods

    public void changeState(bool message)
    {
        this.isActive = message;
        if (this.isActive == false)
        {
            foreach (Transform child in this.transform)
            {
                switch(child.name)
                {
                    case "Receptor Body":
                        child.GetComponent<Renderer>().material.color = NonActiveColor;
                        break;
                    case "Right_Receptor":
                        child.GetComponent<Renderer>().material.color = NonActiveColor;
                        break;
                    case "Left_Receptor":
                        child.GetComponent<Renderer>().material.color = NonActiveColor;
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            this.allowMovement = true;
            foreach (Transform child in this.transform)
            {
                switch (child.name)
                {
                    case "Receptor Body":
                        child.GetComponent<Renderer>().material.color = ActiveColor;
                        break;
                    case "Right_Receptor":
                        child.GetComponent<Renderer>().material.color = ActiveColor;
                        break;
                    case "Left_Receptor":
                        child.GetComponent<Renderer>().material.color = ActiveColor;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    #endregion Public Methods

    #region Private Methods

    private void Start()
    {
        changeState(true);
    }

    #endregion Private Methods
}