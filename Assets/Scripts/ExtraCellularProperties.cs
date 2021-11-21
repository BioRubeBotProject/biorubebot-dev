/*  File:       ExtraCellularProperties
    Purpose:    This file holds the functions and definitions for the properties
                of the Protein Signaler Game Object
*/
using UnityEngine;

public class ExtraCellularProperties : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates + Enums

    public Color ActiveColor    = Color.white;
    public bool  allowMovement  = true;
    public bool  isActive       = true;
    public Color NonActiveColor = Color.gray;

    #endregion Public Fields + Properties + Events + Delegates + Enums

    private bool changedState = false;
    #region Public Methods

    /*  Function:   changeState(bool)
        Purpose:    This function changes the isActive variable to the given
                    value. If the value is true, allowMovement and the
                    ReceptorPathfinding enabled public property are set to true,
                    and the color is set to the ActiveColor.
                    If it is false, allowMovement and ReceptorPathfinding.enabled
                    are set false and the color is set to the inactive color
                    This stops the Game Object from moving anymore
        Parameters: whether the Protein Signaler is active
    */
    public void changeState(bool message)
    {
        this.isActive = message;
        if(this.isActive == false)
        {
            this.allowMovement = false;
            this.GetComponent<ReceptorPathfinding>().enabled = false;
            foreach(Transform child in this.transform)
            {
                if(child.name == "Extracellular Signal Body")
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
                if (child.name == "Extracellular Signal Body")
                {
                    child.GetComponent<Renderer>().material.color = ActiveColor;
                    break;
                }
            }
            this.GetComponent<ReceptorPathfinding>().enabled = true;
        }
    }

    #endregion Public Methods

    #region Private Methods

    /*  Function:   Start()
        Purpose:    this function is called when initially instantiated.
                    does some initialization
    */
    private void Start()
    {
        ExtraCellularProperties objProps = (ExtraCellularProperties)this.GetComponent("ExtraCellularProperties");
        changeState(objProps.isActive);
    }

    /*  Function:   Update()
        Purpose:    this function checks for the Protien Signaler's state to
                    have changed to inactive. At that time, calls the changeState
                    function, which will cause it to stop moving around and change
                    color to the inactive color
    */
    private void Update()
    {
        //if we have becom inactive, change color
        if(this.isActive == false)
        {
            if(!changedState)
            {
                changeState(this.isActive);
                changedState = true;
            }
        }
    }

    #endregion Private Methods
}