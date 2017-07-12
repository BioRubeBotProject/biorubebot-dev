using System.Collections;
using UnityEngine;


class GameWon : MonoBehaviour {

    //Array in which to place all win condition tags. Please read accompanying document "WinConditionInstruction".
    public static string[] WinConditionTags = {
            "Win_TFactorEntersNPC",                                                              //NPC Win Conditions
            "Win_GProteinFreed",                                                                 //G-Protein Win Conditions
            "Win_TranscriptionFactorCompleted", "Win_T_Reg_Complete",                           //T-Reg Win Conditions
            "Win_FullReceptorActivated", "Win_ReceptorSitesOpen", "Win_ReceptorCompleted"        //Receptor Win Conditions
        };


    private static bool Win_FullReceptorActivated;
    private static bool Win_TFactorEntersNPC;
    private static bool Win_GProteinFreed;
    private static bool Win_T_Reg_Complete;
    private static bool Won;

    public void Start()
    {
        Win_FullReceptorActivated = false;
        Win_TFactorEntersNPC = false;
        Win_GProteinFreed = false;
        Win_T_Reg_Complete = false;
        Won = false;
    }

    public static void Set_WinConditions()
    {
        bool WinBool = true;
        foreach (string WinConString in WinConditionTags) if (GameObject.FindWithTag(WinConString)) WinBool = false;
            
        Set_Won(WinBool);
    }

    public static bool IsWon() { return Won; }
    private static void Set_Won(bool val) { Won = val; }
}

