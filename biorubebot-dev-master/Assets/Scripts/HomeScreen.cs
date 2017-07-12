using UnityEngine;
using System.Collections;


//Script populates and animates red blood cell prefabs on the main menu (home screen)
public class HomeScreen : MonoBehaviour {

	private bool spawningCells;
	private int delay;
	public Transform cell1;
	public Transform cell2;
	public Transform cell3;
	public Transform cell4;
	public Transform cell5;
	public Transform cell6;
	public Vector3 cellPos = new Vector3 (0,0,0);
	public GameObject infoBtn;
	public GameObject playBtn;

	void Start()
	{
		Time.timeScale = 1;
		delay = 0;
		spawningCells = true;
		playBtn.SetActive (false);
		infoBtn.SetActive (false);
	}

	void FixedUpdate ()
	{
		//The followin block of code instantiates cell prefabs approximately 1 every 0.5 seconds
		//A 0.5 second delay is added to give time for the Unity screen to execute at startup
		//Without the delay, the cells spawn prior to displaying the home screen;

		if (delay++ < 128 && spawningCells)
		{
			switch (delay)
			{
				case 38:
					PopupCell (cell2, 9.50f, -33.0f, 44f);
					break;
				case 51:
					PopupCell (cell3, 55.9f,  28.8f, 46f);
					break;
				case 64:
					PopupCell (cell4, 10.0f,  2.50f, 48f);
					break;
				case 78: 
					PopupCell (cell5, 32.0f,  20.0f, 50f);
					break;
				case 128:
					playBtn.SetActive(true);
					infoBtn.SetActive(true);
					break;
			}
		} 
		else
		{	//set spawning to false and begin animation
			spawningCells = false;
			if (delay++ > 100)//Blink every two seconds
			{
				delay = 0;
				Blink ();
			}
		}
	}

	public void PopupCell(Transform cell, float x, float y, float z)
	{
		AudioSource pop = GetComponent<AudioSource> ();
	
		Instantiate (cell, new Vector3 (x, y, z), transform.rotation = Quaternion.identity);
		pop.Play ();
	}

	public void Blink()
	{	//If understood correctly, the random number generator generates a float
		//number between a min and max inclusive.  Typecasting the float to int
		//results in a less than desirable probability the random value will equal
		// the maximum value.  Therefore, maximum of 5, therefore a random value
		// between 1 and 6 (inclusive) is used.

		int cell = (int)Random.Range (1f, 6f);

		cell6.localScale = new Vector3 (5, 5, 0);
		cellPos = new Vector3 (0, 0, 0);
	
		switch (cell) {
		case 1:	//largest cell in the lower right corner of the home screen
			cellPos = new Vector3 (45f, -23f, 39);
			cell6.localScale = new Vector3 (10F, 10f, 0);//scale blinking cell to match the larger cell1
			break;
		case 2: 
			cellPos = new Vector3 (9.50f,-33.0f, 43f);
			break;
		case 3: 
			cellPos = new Vector3 (55.9f, 28.8f, 45f);
			break;
		case 4: 
			cellPos = new Vector3 (10.0f, 2.50f, 47f);
			break;
		default: //case 5 and the unlikely case 6: 
			cellPos = new Vector3 (32.0f, 20.0f, 49);
			break;
		}
		Transform cloneCell = Instantiate (cell6, cellPos, cell6.rotation = Quaternion.identity) as Transform;
		StartCoroutine (destroyBlinkCell (cloneCell));
	}

	public IEnumerator destroyBlinkCell(Transform cloneCell)
	{
		//Destroys the instantiated blinking cell giving the cells
		//the illusion of blinking.
		yield return new WaitForSeconds (0.1f);
		if (cloneCell != null)
			Destroy (cloneCell.gameObject);
	}
}
