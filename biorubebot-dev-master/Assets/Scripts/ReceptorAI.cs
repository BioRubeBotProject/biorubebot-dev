using Pathfinding;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Seeker))]
//[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody2D))]
public class ReceptorAI : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates + Enums

    public float directionChangeInterval = 1;
    public ForceMode2D fMode;

    public float maxHeadingChange = 30;

    //The max distance form the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDisance = 3f;

    //The Calculated Path
    public Path path;

    [HideInInspector]
    public bool pathfinding = false;

    public float seekTime = 1f;

    //The AI's Speed [per second]
    public float speed = 300f;

    //What to seek
    public Transform target;

    //How many times each second we update our path
    public float updateRate = 2f;

    #endregion Public Fields + Properties + Events + Delegates + Enums

    //private CharacterController controller;

    #region Private Fields + Properties + Events + Delegates + Enums

    //The waypoint we are currently moving towards;
    private int currentWaypoint = 0;

    private float heading;
    private bool pathIsEnded = false;
    private Rigidbody2D rb;
    private bool searchingForItem = false;

    //Caching
    private Seeker seeker;

    //private float timeToMove;

    #endregion Private Fields + Properties + Events + Delegates + Enums

    #region Public Methods

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    #endregion Public Methods

    #region Private Methods

    private void FixedUpdate()
    {
        if (target == null)
        {
            if (!searchingForItem)
            {
                searchingForItem = true;
                StartCoroutine(SearchForObject());
            }

            return;
        }

        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
        {
            if (pathIsEnded)
            {
                return;
            }

            pathIsEnded = true;
        }

        pathIsEnded = false;

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;

        dir *= speed * Time.fixedDeltaTime;

        //Move the AI:
        rb.AddForce(dir, fMode);

        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);

        if (dist < nextWaypointDisance)
        {
            currentWaypoint++;
            return;
        }
    }

    private void Move()
    {
        //timeToMove = Time.fixedTime + seekTime;
        //while (Time.fixedTime >= timeToMove)
        //{
        transform.position = new Vector3((speed * Time.deltaTime), 0, 0);
        //}
    }

    private void Roam()
    {
        transform.position += transform.up * Time.deltaTime * 10;
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
        transform.eulerAngles = new Vector3(0, 0, heading);

        //Move();
    }

    private IEnumerator SearchForObject()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("ExternalReceptor");
        //Component co = GetComponent("Extracellular Signal Body");
        //co.GetComponent<Renderer>().material.color = Color.blue;

        bool FoundObject = false;

        foreach (GameObject go in gos)
        {
            ExtraCellularProperties objProps = (ExtraCellularProperties)go.GetComponent("ExtraCellularProperties");

            if (!objProps.isActive)
            {
                objProps = null;
                continue;
            }
            else
            {
                FoundObject = true;
                objProps = null;
                searchingForItem = false;
                target = go.transform;
                StartCoroutine(UpdatePath());
                break;
            }
        };

        gos = null;

        if (!FoundObject)
        {
            Roam();
            yield return new WaitForSeconds(seekTime);
            StartCoroutine(SearchForObject());
        }
    }

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (target == null)
        {
            if (!searchingForItem)
            {
                searchingForItem = true;
                StartCoroutine(SearchForObject());
            }

            return;
        }

        StartCoroutine(UpdatePath());
    }

    private IEnumerator UpdatePath()
    {
        if (target == null)
        {
            if (!searchingForItem)
            {
                searchingForItem = true;
                StartCoroutine(SearchForObject());
            }
        }
        else
        {
            ExtraCellularProperties objProps = (ExtraCellularProperties)target.GetComponent("ExtraCellularProperties");

            if (!objProps.isActive)
            {
                searchingForItem = true;
                StartCoroutine(SearchForObject());
            }
            else
            {
                //Component co = this.GetComponent("Extracellular Signal Body");
                //co.GetComponent<Renderer>().material.color = Color.red;

                seeker.StartPath(transform.position, target.position, OnPathComplete);

                StartCoroutine(UpdatePath());

                yield return new WaitForSeconds(1 / updateRate);
            }

            objProps = null;
        }
    }

    #endregion Private Methods
}