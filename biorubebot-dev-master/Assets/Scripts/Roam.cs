using UnityEngine;
using System.Collections;


public class Roam : MonoBehaviour {
	public static float _max = 150f;
	public static float _min = -150f;
	public static float _speed = 5.0f;

	public static void Roaming(GameObject Obj) {
		if (Time.timeScale > 0)// if simulation is running
		{
			float randomX, randomY;		//random number between minX/maxX and minY/maxY
			randomX = Random.Range (_min,_max); //get random x vector coordinate
			randomY = Random.Range (_min, _max); //get random y vector coordinate
			//apply a force to the object in direction (x,y):
			Obj.GetComponent<Rigidbody2D> ().AddForce (new Vector2(randomX, randomY), ForceMode2D.Force);

			//Obj.Transform.rotation = Quaternion.LookRotation (dir);
		}
	}

	public static void RoamingTandem(GameObject Obj1, GameObject Obj2, Vector3 Offset) {
		if (Time.timeScale > 0)// if simulation is running
		{
			float randomX, randomY;		//random number between minX/maxX and minY/maxY
			randomX = Random.Range (_min,_max); //get random x vector coordinate
			randomY = Random.Range (_min, _max); //get random y vector coordinate
			//apply a force to the object in direction (x,y):
			Obj1.GetComponent<Rigidbody2D> ().AddForce (new Vector2(randomX, randomY), ForceMode2D.Force);
			Obj2.transform.position = Obj1.transform.position + Offset;
		}
	}

	public static Vector3 CalcMidPoint ( GameObject obj_1, GameObject obj_2 ) {
		float[] temp = new float[2];
		temp [0] = (obj_1.transform.position.x + obj_2.transform.position.x)/2.0f;
		temp [1] = (obj_1.transform.position.y + obj_2.transform.position.y)/2.0f;
		Vector3 meetingPoint = new Vector3 (temp [0], temp [1], obj_1.transform.position.z);

		return meetingPoint;
	}

	public interface CollectObject {
		void GetObject(GameObject obj,string newTag);
	}

	public static void FindAndWait<T>(T obj, GameObject self, ref Transform myTarget, ref float delay, string changeTag) where T : MonoBehaviour, CollectObject {
		if( obj != null && myTarget == null) {
			delay = 0;
			obj.GetObject(self,changeTag);
			myTarget = obj.transform;
		}
		if (myTarget != null && (delay += Time.deltaTime) >= 5) {

		} 
		else {
			Roam.Roaming (self.gameObject);
		}
	}

	public static GameObject FindClosest(Transform my, string objTag) {
		float distance = Mathf.Infinity; //initialize distance to 'infinity'
		
		GameObject[] gos; //array of gameObjects to evaluate
		GameObject closestObject = null;
		//populate the array with the objects you are looking for
		gos = GameObject.FindGameObjectsWithTag(objTag);
		
		//find the nearest object ('objectTag') to me:
		foreach (GameObject go in gos)
		{	
			//calculate square magnitude between objects
			float curDistance = Vector3.Distance(my.position,go.transform.position);
			if (curDistance < distance)
			{
				closestObject = go; //update closest object
				distance = curDistance;//update closest distance
			}
		}
		return closestObject;
	}/* end FindClosest */

	public static bool ApproachMidpoint (GameObject obj1,GameObject obj2, bool[] midpointAchieved, Vector3 midpoint, Vector3 Offset, float Restraint) {
		if (!midpointAchieved [0]) {
			midpointAchieved [0] = Roam.ApproachVector (obj1, midpoint, Offset, Restraint);
		}
		
		if (!midpointAchieved [1]) {
			midpointAchieved [1] = Roam.ApproachVector (obj2, midpoint, -1 * Offset, Restraint);
		}
		return (midpointAchieved [0] && midpointAchieved [1]);
	}

	public static bool ApproachVector(GameObject obj, Vector3 destination, Vector3 offset, float restraint) {
		if (Vector3.Distance (obj.transform.position, destination) > restraint) {
			Roaming (obj);
		}
		return ProceedToVector (obj, destination + offset);
	}

	public static bool ProceedToVector (GameObject obj, Vector3 approachVector) {
		float step = _speed * Time.deltaTime;
		obj.transform.position = Vector3.MoveTowards (obj.transform.position, approachVector, step);
		return (approachVector == obj.transform.position);
	}

  public static void setAlpha (GameObject obj, float alpha ){
    if (obj.GetComponent<Renderer> () != null) {
      Color a = obj.gameObject.GetComponent<Renderer> ().material.color;
      // ().material.color;
      a.a = alpha;
    
      obj.gameObject.GetComponent<Renderer> ().material.color = a;
    } else if (obj.GetComponentInChildren<Renderer> () != null){
      Color a = obj.gameObject.GetComponentInChildren<Renderer> ().material.color;
      // ().material.color;
      a.a = alpha;
      
      obj.gameObject.GetComponentInChildren<Renderer> ().material.color = a;
      for( int i = 0; i < obj.gameObject.transform.childCount; i++ ) {
        setAlpha (obj.gameObject.transform.GetChild(i).gameObject, alpha);
      }
    }
  }
}