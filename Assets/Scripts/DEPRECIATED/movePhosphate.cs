using UnityEngine;
using System.Collections;

public class movePhosphate : MonoBehaviour
{
	//private Vector2 randomDirection;	//new direction vector
	public float maxX, maxY, minX, minY;//min/max vector values
	private float randomX, randomY;		//random number between minX maxX and minY and maxY

	public void FixedUpdate() //runs every 20ms
	{
		if (Time.timeScale > 0)// if simulation is running
		{
				randomX = Random.Range(minX,maxX); //get random x vector coordinate
				randomY = Random.Range(minY,maxY); //get random y vector coordinate
				//apply a force to the object in direction (x,y)
				GetComponent<Rigidbody2D>().AddForce(new Vector2(randomX, randomY), ForceMode2D.Force);
		}
	}
	
}