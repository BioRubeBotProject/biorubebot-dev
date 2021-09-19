using UnityEngine;
using System.Collections;

public class ReceptorLegProperties : MonoBehaviour
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
				if (child.name == "Inner Receptor Body Final")
				{
					child.GetComponent<Renderer>().material.color = NonActiveColor;
				}
			}
		}
		else
		{
			this.allowMovement = true;
			foreach (Transform child in this.transform)
			{
				if (child.name == "Inner Receptor Body Final")
				{
					child.GetComponent<Renderer>().material.color = ActiveColor;
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

	private void Update()
	{
		if (this.isActive == false)
        {
			this.allowMovement = false;
			foreach (Transform child in this.transform)
			{
				if (child.name == "Inner Receptor Body Final")
				{
					child.GetComponent<Renderer>().material.color = NonActiveColor;
				}
			}
			this.GetComponent<ReceptorLegScript>().enabled = false;
		}

		if (this.isActive == true)
        {
			this.allowMovement = true;
			foreach (Transform child in this.transform)
			{
				if (child.name == "Inner Receptor Body Final")
				{
					child.GetComponent<Renderer>().material.color = ActiveColor;
				}
			}
			this.GetComponent<ReceptorLegScript>().enabled = true;
		}


	}
	
	#endregion Private Methods
}
