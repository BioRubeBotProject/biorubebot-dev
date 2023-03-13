/*  File:       WinScenario
    Purpose:    this file contains the dropTag function which will retag the
                checkbox from whatever it was to be Condition_Met. This is
                a trigger for the WinBoxChange file, which will spawn a
                checked box in place of the unchecked box.
*/
using UnityEngine;
using System.Collections;


public class WinScenario : MonoBehaviour
{

    public static GameObject WinCondition;

    /*  Function:   dropTag(string)
        Purpose:    This function retags the GameObject having the given
                    name to have the tag "Condition_Met". This will
                    cause the checkbox to be replaced with a checked box
                    by WinBoxChange, which is continuously listening for
                    this tag
    */
    public static void dropTag (string GameObjectName)
    {
      
        WinCondition     = GameObject.FindWithTag(GameObjectName);
        WinCondition.tag = "Condition_Met";
    }
}

