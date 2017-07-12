using UnityEngine;
using System.Collections;


public class WinScenario : MonoBehaviour
{

    public static GameObject WinCondition;

    public static void dropTag (string GameObjectName)
    {
        WinCondition = GameObject.FindWithTag(GameObjectName);
        WinCondition.tag = "Condition_Met";
    }

}

