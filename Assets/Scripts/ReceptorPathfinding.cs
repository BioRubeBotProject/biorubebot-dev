using UnityEngine;

public class ReceptorPathfinding : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates + Enums

    public bool displayPath = true;
    public float maxHeadingChange = 60;
    public Transform SightEnd;
    public Transform sightStart;
    public int speed = 100;
    public bool spotted = false;

    #endregion Public Fields + Properties + Events + Delegates + Enums

    #region Private Fields + Properties + Events + Delegates + Enums

    private float heading;
    private GameObject[] myFoundObjs;
    private GameObject myTarget;

    #endregion Private Fields + Properties + Events + Delegates + Enums

    #region Private Methods

    private void Raycasting()
    {
        //while (true) {


        int x = 0;
        myFoundObjs = GameObject.FindGameObjectsWithTag("ExternalReceptor");
        while (x < myFoundObjs.Length && myFoundObjs[x].GetComponent<ExternalReceptorProperties>().isActive == false)
        {
            x++;
        }

        int count = myFoundObjs.GetUpperBound(0);

        if (x <= count)
        {
			if (myFoundObjs [x].GetComponent<ExternalReceptorProperties> ().isActive == true)
            {
				//Debug.Log("We found a receptor!");
				sightStart = myFoundObjs [x].transform;
				transform.position += transform.up * Time.deltaTime * speed;
				if (displayPath == true) {
					Debug.DrawLine (sightStart.position, SightEnd.position, Color.green);
				}
				spotted = Physics2D.Linecast (sightStart.position, SightEnd.position);


				Quaternion rotation = Quaternion.LookRotation (SightEnd.position - sightStart.position, sightStart.TransformDirection (Vector3.up));
				transform.rotation = new Quaternion (0, 0, rotation.z, rotation.w);
			}
		} 
		else {
			sightStart = null;
			spotted = false;
		}
    }

    private void Roam()
    {
        transform.position += transform.up * Time.deltaTime * 10;
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
        transform.eulerAngles = new Vector3(0, 0, heading);
    }

    // Use this for initialization 
    private void Start()
    {
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
    }

    // Update is called once per frame 
    private void Update()
    {
        Raycasting();
        if (!spotted)
        {
            Roam();
        }
    }

    #endregion Private Methods
}