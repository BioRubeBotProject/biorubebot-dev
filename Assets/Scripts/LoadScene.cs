/*  File:       LoadScene
    Purpose:    This file loads the scene depending on the button that
                was chosen. To change which scene is loaded by a button
                in the Congratulations Window or the Main Menu, the
                LoadScene Prefab, not the Button itself is modified. There
                the nextScene and homeMenuScene variables are set as
                necessary to load the correct scene. Counterintuitive,
                yes. Could be refactored in the future to be modified on
                the button perhaps
*/
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string nextScene;
    public string homeMenuScene;

    //Restart current scene
    public void restartbutton()
    {    
        Application.LoadLevel(Application.loadedLevel);
        GameWon.Set_WinConditions();
    }

    //Load next scene
    public void loadNextScene()
    {
        print("Load" + nextScene);
        Application.LoadLevel(nextScene);
        GameWon.Set_WinConditions();
    }
    
    public void loadHomeMenu()
    {
        print("Home = " + homeMenuScene);
        print("Next = " + nextScene);
        Application.LoadLevel(homeMenuScene);
        GameWon.Set_WinConditions();
    }


}