/*  File:       PKABMovement
    Purpose:    This file was created to handle the PKA-B Prefab's movement
                about the Cell Membrane. It has the PKA-B, which is what the
                Kinase transforms into once two cAMP have attached to the PKA,
                move about aimlessly. However, when it came time to update it
                to deal with the Transcription Regulator binding to it, we ran
                out of time to deal with how tightly coupled the Transcription
                Regulator and the Kinase from Level 1 are.
                This file could potentially be used in the future if the desire
                is to move toward using the blue PKA that looks more like what
                is in the Youtube videos. Will need some changes, and so will
                the Transcription Regulator
    Author:     Ryan Wood
    Created:    Fall 2021
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PKABMovement : MonoBehaviour
{
    public Transform  origin;            // origin location/rotation is the physical PKA
    public float      maxHeadingChange;  // max possible rotation angle at a time
    public int        maxRoamChangeTime; // how long before changing heading/speed
    public int        minSpeed;          // slowest the GTP will move
    public int        maxSpeed;          // fastest the GTP will move

    private float    heading;            // roaming direction
    private float    headingOffset;      //used for smooth rotation while roaming
    private int      movementSpeed  = 0; // roaming velocity
    private int      roamInterval   = 0; // how long until heading/speed change while roaming
    private int      roamCounter    = 0; // time since last heading speed change while roaming

    private void Roam()
    {
        if(Time.timeScale != 0)// if game not paused
        {
            roamCounter++;                                      
            if(roamCounter > roamInterval)                         
            {
                roamCounter   = 0;
                var floor     = Mathf.Clamp(heading - maxHeadingChange, 0, 360);  
                var ceiling   = Mathf.Clamp(heading + maxHeadingChange, 0, 360);
                roamInterval  = UnityEngine.Random.Range(5, maxRoamChangeTime);   
                movementSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);

                RaycastHit2D collision = Physics2D.Raycast(origin.position, origin.up);
                if(collision.collider != null && collision.collider.name == "Cell Membrane(Clone)" &&
                   collision.distance < 2)
                {
                    if(heading <= 180)
                        heading = heading + 180;
                    else
                        heading = heading - 180;

                    movementSpeed = maxSpeed;
                    roamInterval  = maxRoamChangeTime;
                }
                else
                    heading = UnityEngine.Random.Range(floor, ceiling);

                headingOffset = (transform.eulerAngles.z - heading) / (float)roamInterval;
            }
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - headingOffset);
            transform.position += transform.up * Time.deltaTime * movementSpeed;
        }
    }

    void Start()
    {
    }

    void Update()
    {
        Roam();
    }
}
