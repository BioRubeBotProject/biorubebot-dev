//I think the author created this to try and refactor the existing GameObjects
//to more generically wander about the Cell Membrane, but they did not finish
//their work or comment their code very well. 

//candidate for deletion. Check for references and remove.
//OR expand, improve and refactor GameObjects to use?
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

    #endregion Private Fields + Properties + Events + Delegates + Enums

    #region Private Methods

    private void Awake()
    {
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