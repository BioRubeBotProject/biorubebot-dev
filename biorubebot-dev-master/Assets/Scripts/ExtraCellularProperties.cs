using UnityEngine;

public class ExtraCellularProperties : MonoBehaviour
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
            this.allowMovement = false;
            this.GetComponent<ReceptorPathfinding>().enabled = false;
            foreach (Transform child in this.transform)
            {
                if (child.name == "Extracellular Signal Body")
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

    private void Start()
    {
        ExtraCellularProperties objProps = (ExtraCellularProperties)this.GetComponent("ExtraCellularProperties");
        changeState(objProps.isActive);
    }
	private void Update()
	{
		if (this.isActive == false) {
			this.allowMovement = false;
			foreach (Transform child in this.transform)
			{
				if (child.name == "Extracellular Signal Body")
				{
					child.GetComponent<Renderer>().material.color = NonActiveColor;
					break;
				}
			}
		}
		if (this.allowMovement == false) {
			this.GetComponent<ReceptorPathfinding> ().enabled = false;
		}
		if (this.isActive == true) {
			this.allowMovement = true;
			this.GetComponent<ReceptorPathfinding> ().enabled = true;
			foreach (Transform child in this.transform)
			{
				if (child.name == "Extracellular Signal Body")
				{
					child.GetComponent<Renderer>().material.color = ActiveColor;
					break;
				}
			}
		}
	}

    #endregion Private Methods
}