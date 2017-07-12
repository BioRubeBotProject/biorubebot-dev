using UnityEngine;

public class MouseActions : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates + Enums

    public float damping = 5.0f;
    public float distance = 3.0f;
    public bool followBehind = true;
    public float height = 3.0f;
    public bool is2DCamera = true;
    public float panSpeed = 4.0f;

    public Transform target;

    public float zoomSpeed = 4.0f;

    #endregion Public Fields + Properties + Events + Delegates + Enums

    #region Private Fields + Properties + Events + Delegates + Enums

    private bool isPanning;

    private Vector3 mouseOrigin;

    #endregion Private Fields + Properties + Events + Delegates + Enums

    #region Private Methods

    // Use this for initialization 
    private void Start()
    {
        if (is2DCamera)
        {
            height = 0;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                target = hit.collider.gameObject.transform;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            mouseOrigin = Input.mousePosition;
            isPanning = true;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Camera.main.orthographicSize += zoomSpeed;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.orthographicSize >= 0)
            {
                Camera.main.orthographicSize -= zoomSpeed;
            }
        }

        if (!Input.GetMouseButton(1)) isPanning = false;

        if (isPanning)
        {
            target = null;
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
            transform.Translate(move, Space.Self);
        }

        if (target != null)
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
    }

    #endregion Private Methods
}