using UnityEngine;
using UnityEngine.SceneManagement;

public class ReceptorPathfinding : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates + Enums

    public Transform SightEnd;
    public Transform sightStart;
    public float     maxHeadingChange = 60;
    public int       speed            = 100;
    public bool      displayPath      = true;
    public bool      spotted          = false;

    #endregion Public Fields + Properties + Events + Delegates + Enums

    #region Private Fields + Properties + Events + Delegates + Enums

    private float heading;
    private GameObject[] myFoundObjs;
    private GameObject myTarget;

    #endregion Private Fields + Properties + Events + Delegates + Enums

    #region Private Methods

    private void Raycasting()
    {
        Quaternion rotation;
        string     strLvl    = null;
        string     strFind   = null;
        bool       found     = false;
        int        index     = 0;//index into the game Ojbect array
        int        count     = 0;//number of found receptors

        strLvl = SceneManager.GetActiveScene().name;
        if(strLvl == "Level1")
            strFind = "ExternalReceptor";
        else if(strLvl == "Level2")
            strFind = "GPCR";

        myFoundObjs = GameObject.FindGameObjectsWithTag(strFind);
        count = myFoundObjs.Length;

        for(index = 0; index < count; index++)
        {
            if((strLvl == "Level1" && myFoundObjs[index].GetComponent<ActivationProperties>().isActive == true) ||
                (strLvl == "Level2" && myFoundObjs[index].GetComponent<ActivationProperties>().isActive == false))
            {
                myTarget = myFoundObjs[index];
                found = true;
                break;
            }
        }

        if(found)
        {
            //Debug.Log("We found a receptor!");
            sightStart = myTarget.GetComponent<CircleCollider2D>().transform;
            Debug.Log(sightStart.position);

            transform.position += transform.up * Time.deltaTime * speed;
            if(displayPath == true)
            {
                Debug.DrawLine(sightStart.position, SightEnd.position, Color.green);
            }
            spotted  = Physics2D.Linecast(sightStart.position, SightEnd.position);
            rotation = Quaternion.LookRotation(SightEnd.position - sightStart.position, sightStart.TransformDirection (Vector3.up));

            transform.rotation = new Quaternion (0, 0, rotation.z, rotation.w);
        } 
        else
        {
            sightStart = null;
            spotted    = false;
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