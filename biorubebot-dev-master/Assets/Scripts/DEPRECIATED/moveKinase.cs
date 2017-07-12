using UnityEngine;
using System.Collections;

public class moveKinase : MonoBehaviour
{
	private Vector2 randomDirection;	//new direction vector
	private static float _max = 100f;
	private static float _min = -100f;
	private float randomX, randomY;		//random number between minX/maxX and minY/maxY

	public void FixedUpdate() //runs every 20ms
	{
		if (Time.timeScale > 0)// if simulation is running
		{
			randomX = Random.Range (_min,_max); //get random x vector coordinate
			randomY = Random.Range (_min, _max); //get random y vector coordinate
			//apply a force to the object in direction (x,y):
			GetComponent<Rigidbody2D> ().AddForce (new Vector2(randomX, randomY), ForceMode2D.Force);
		}
	}
}
