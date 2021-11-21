/*  File:       GameWon
    Purpose:    the game contains a number of checkboxes. When all the check
                boxes have been checked and there are no unchecked boxes remaining,
                the level has been won. Each checkbox is initialized with a tag
                and is unchecked. When the condition is reached, such as the
                Alpha subunit Bindint to the Adenylyl Cyclase, the tag is removed
                from the checkbox. This causes the checkbox to actually become
                checked. To add a new win checkbox, an unchecked Win checkbox
                Prefab is dragged onto the Game's surface. The Prefab is then
                renamed and given a new Tag. This tag needs to be added to the
                array defined in this file.
*/
using System.Collections;
using UnityEngine;


class GameWon : MonoBehaviour
{

    //Array in which to place all win condition tags. Please read accompanying document "WinConditionInstruction".
    public static string[] WinConditionTags =
        {
            "Win_TFactorEntersNPC",
            "Win_GProteinFreed", "Win_DockedGTP",
            "Win_TranscriptionFactorCompleted", "Win_T_Reg_Complete",
            "Win_Kinase_TReg_dock",
            "Win_FullReceptorActivated", "Win_ReceptorSitesOpen", "Win_ReceptorCompleted",
            "Win_ReceptorPhosphorylation", "Win_LeftReceptorWithProtein",
            "Win_ReceptorsCollideWithProtein",
            "Win_ReleasedGDP",
            "Win_GPCR_Activated",
            "Win_TGP_Bound_to_GPCR",
            "Win_GTP_Binds_to_Alpha",
            "Win_Alpha_Binds_to_Cyclase",
            "Win_Alpha_Rejoins_GProtein",
            "Win_PKA_Separates",
            "Win_TR_Enters_Nucleus",
            "Win_KinaseTransformation"
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

