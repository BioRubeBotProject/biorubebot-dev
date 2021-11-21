/*  File:       ReceptorPathfinding
    Purpose:    Perhaps misnamed, this file deals with the movement of the
                Protein Signaler, which in Level 2 seeks out a G-Protein Coupled
                Receptor on the outside of the Cell Membrane and in other levels
                seeks out an External Receptor. This Object colliding with one
                of these Objects will activate that Object
*/

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

    /*  Function:   Raycasting()
        Purpose:    this function has the Protien Signaler actively search out
                    the most nearby target. In level2, that will be the GPCR,
                    but in all other levels, it will be the ExternalReceptor.
                    The Protein Signaler tracks to the Object it needs to collide
                    with and moves toward it.
    */
    private void Raycasting()
    {
        Quaternion rotation;
        string     strLvl     = null;
        string     strFind    = null;
        bool       found      = false;
        bool       activeFind = false;
        int        index      = 0;//index into the game Ojbect array
        int        count      = 0;//number of found receptors

        strLvl = SceneManager.GetActiveScene().name;
        /* In Level2 looking for GPCR. Other levels, looking for ExternalReceptor
           for some reason, previous group uses isActive in a non-intuitive way,
           so to activate the ExternalReceptor, we look for an ExternalReceptor
           GameObject with isActive set to true. For the new level, we look
           for a GPCR with isActive set to false because we want to activate it
        */
        if(strLvl == "Level2")
        {
            strFind    = "GPCR";
            activeFind = false;
        }
        else
        {
            strFind    = "ExternalReceptor";
            activeFind = true;
        }

        myFoundObjs = GameObject.FindGameObjectsWithTag(strFind);
        count       = myFoundObjs.Length;

        for(index = 0; index < count; index++)
        {
            if((myFoundObjs[index].GetComponent<ActivationProperties>().isActive == activeFind))
            {
                myTarget = myFoundObjs[index];
                found    = true;
                break;
            }
        }

        if(found)
        {
            sightStart = myTarget.GetComponent<CircleCollider2D>().transform;

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

    /*  Function:   Roam()
        Purpose:    this function has the Protein Signaler Roam around, which
                    it does until there is a target in place for it to seek
    */
    private void Roam()
    {
        transform.position += transform.up * Time.deltaTime * 10;
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil  = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading   = Random.Range(floor, ceil);

        transform.eulerAngles = new Vector3(0, 0, heading);
    }

    /*  Function:   Start()
        Purpose:    Called upon initial instantiation. Does some setup
    */
    private void Start()
    {
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil  = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading   = Random.Range(floor, ceil);
    }

    /*  Function:   Update()
        Purpose:    called once per frame. Calls Raycasting. If the Raycasting
                    function does not spot a target, calls roam. This
                    causes the Protein Signaler to either seek out a target
                    or roam around aimlessly
    */
    private void Update()
    {
        Raycasting();
        if(!spotted)
        {
            Roam();
        }
    }

    #endregion Private Methods
}