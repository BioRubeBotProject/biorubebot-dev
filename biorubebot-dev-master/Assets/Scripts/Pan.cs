using UnityEngine;
using System.Collections;

public class Pan : MonoBehaviour
{
	public float speed = 10F;
	//public bool panning = Spawner.panning;

	void Update() 
	{

		//Debug.Log (Spawner.panning);
		if (Spawner.panning && Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			if (touchDeltaPosition.x > -300f && touchDeltaPosition.x < 250f) 
			transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);
		}
	}
}
