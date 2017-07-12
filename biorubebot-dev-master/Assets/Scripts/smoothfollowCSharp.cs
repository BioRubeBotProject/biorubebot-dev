using System.Collections;
using UnityEngine;

public class smoothfollowCSharp : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates

    public float damping = 5.0f;

    public float distance = 3.0f;

    public bool followBehind = true;

    public float height = 3.0f;

    public bool is2DCamera = true;

    public Transform target;

    #endregion Public Fields + Properties + Events + Delegates

    #region Private Methods

    // Use this for initialization
    private void Start()
    {
        if (is2DCamera)
        {
            height = 0;
        }
    }

    //This code was found at: http://wiki.unity3d.com/index.php/SmoothFollow2 and was modified to meet the needs of this program.
    private void Update()
    {
        Vector3 wantedPosition;
        if (followBehind)
            wantedPosition = target.TransformPoint(0, height, -distance);
        else
            wantedPosition = target.TransformPoint(0, height, distance);

        transform.position = Vector3.Lerp(transform.position, wantedPosition, Time.deltaTime * damping);

        transform.LookAt(target, target.up);

        if (is2DCamera)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    #endregion Private Methods
}