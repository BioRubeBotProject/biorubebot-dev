using System.Collections;
using UnityEngine;

public class Wander : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates + Enums

    public float directionChangeInterval = 1;

    public ForceMode2D fMode;
    public float maxHeadingChange = 30;

    public float speed = 5;

    #endregion Public Fields + Properties + Events + Delegates + Enums

    #region Private Fields + Properties + Events + Delegates + Enums

    private CharacterController controller;

    private float heading;
    //private Rigidbody2D rb;
    //private Vector3 targetRotation;

    #endregion Private Fields + Properties + Events + Delegates + Enums

    #region Private Methods

    private void Awake()
    {
        //rb = GetComponent<Rigidbody2D>();

        // Set random initial rotation 
        heading = Random.Range(0, 360);
        transform.eulerAngles = new Vector3(0, 0, heading);

        StartCoroutine(NewHeading());
    }

    private void FixedUpdate()
    {
        //rb.AddForce(targetRotation * speed, fMode);
        //controller.SimpleMove(targetRotation);
    }

    /// <summary>
    /// Repeatedly calculates a new direction to move towards. Use this instead of
    /// MonoBehaviour.InvokeRepeating so that the interval can be changed at runtime.
    /// </summary>
    private IEnumerator NewHeading()
    {
        while (true)
        {
            NewHeadingRoutine();
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }

    /// <summary>
    /// Calculates a new direction to move towards. 
    /// </summary>
    private void NewHeadingRoutine()
    {
        var floor = Mathf.Clamp(heading - maxHeadingChange, 0, 360);
        var ceil = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
        heading = Random.Range(floor, ceil);
        //targetRotation = new Vector3(0, 0, heading);
    }

    #endregion Private Methods
}