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
        Application.LoadLevel(nextScene);
        GameWon.Set_WinConditions();
    }
    
    public void loadHomeMenu()
    {
        Application.LoadLevel(homeMenuScene);
        GameWon.Set_WinConditions();
    }


}