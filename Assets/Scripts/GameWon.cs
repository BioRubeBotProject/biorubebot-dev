﻿using System.Collections;
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
            "Win_GPCR_Activated",
            "Win_TGP_Bound_to_GPCR",
            "Win_KinaseTransformation"                                                          //Kinase Win Conditions
        };


    private static bool Won;

    public void Start()
    {
        Won = false;
    }

    /*  Function:   Set_WinConditions()
        Purpose:    this function loops through all the tags in the WinConditionTags
                    array, and checks to see if any of them can be found. Whenever
                    a win condition is checked, the tag is removed. So, if it can't
                    be found, it is either not a part of this level or it has been
                    acheived already. If no tags are found, then the level has
                    has been won.
    */
    public static void Set_WinConditions()
    {
        bool WinBool = true;
        foreach(string WinConString in WinConditionTags)
        {
            if(GameObject.FindWithTag(WinConString))
                WinBool = false;
        }
            
        Set_Won(WinBool);
    }

    public static bool IsWon()
    {
        return Won;
    }

    private static void Set_Won(bool val)
    {
        Won = val;
    }
}

