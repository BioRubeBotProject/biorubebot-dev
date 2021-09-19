using System.Collections;
using UnityEngine;


class GameWon : MonoBehaviour {

    //Array in which to place all win condition tags. Please read accompanying document "WinConditionInstruction".
    public static string[] WinConditionTags = {
            "Win_TFactorEntersNPC",                                                             //NPC Win Conditions
            "Win_GProteinFreed", "Win_DockedGTP",                                               //G-Protein Win Conditions
            "Win_TranscriptionFactorCompleted", "Win_T_Reg_Complete",                           //T-Reg Win Conditions
            "Win_Kinase_TReg_dock",
            "Win_FullReceptorActivated", "Win_ReceptorSitesOpen", "Win_ReceptorCompleted",      //Receptor Win Conditions
            "Win_ReceptorPhosphorylation", "Win_LeftReceptorWithProtein",
            "Win_ReceptorsCollideWithProtein",
            "Win_ReleasedGDP",                                                                  //GDP Win Conditions
            "Win_KinaseTransformation"                                                          //Kinase Win Conditions
        };


    private static bool Won;

    public void Start()
    {
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

