using UnityEngine;
using System.Collections;


public class WinScenario : MonoBehaviour
{

    public static GameObject WinCondition;

    //Changes the tag for WinCondition boxes (obejects on screen/scene) to indicate the Win Condition met, and can be changed (WinBoxChange.cs)
    public static void dropTag (string GameObjectName)
    {
        WinCondition = GameObject.FindWithTag(GameObjectName);
        WinCondition.tag = "Condition_Met";
    }

}

