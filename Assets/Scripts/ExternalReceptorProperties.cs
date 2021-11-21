/*  File:       ExternalReceptorProperties
    Purpose:    this file contains properties and functions for the
                External Receptor. Implements the ActivationProperies Interface,
                which allows the isActive property to be retrieved and set
                through GameObject.GetComponent<ActivationProperties>().isActive
*/

using UnityEngine;

public class ExternalReceptorProperties : MonoBehaviour , ActivationProperties
{
    #region Public Fields + Properties + Events + Delegates + Enums

    public  Color ActiveColor    = Color.white;
    public  bool  allowMovement  = true;
    public  Color NonActiveColor = Color.gray;

    private bool  m_isActive     = true;//odd choice to begin this Active. Maybe refactor at some point

    #endregion Public Fields + Properties + Events + Delegates + Enums

    #region Public Methods
    public bool isActive
    {
        get => m_isActive;
        set => m_isActive = value;
    }

    /*  Function:   changeState(bool)
        Purpose:    this function changes the state of the Receptor to be the
                    given value, modifying the isActive property to be the value.
                    In implementation, its weird, though because it is already set
                    by the isActive property by an external source, and this function
                    is really just called to update the color of the parts of
                    the prefab. When isActive is true, the active color is used
                    and when isActive is false, the inactive color is used
    */
    public void changeState(bool message)
    {
        this.isActive = message;
        if(this.isActive == false)
        {
            foreach(Transform child in this.transform)
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

    /*  Function:   Start()
        Purpose:    initializes the isActive state to true upon instantiation
    */
    private void Start()
    {
        changeState(true);
    }

    #endregion Private Methods
}