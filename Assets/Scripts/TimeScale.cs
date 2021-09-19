using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine.EventSystems;

public class TimeScale : MonoBehaviour
{
	private static float savedTimeScale;
    public GameObject play;
    public GameObject pause;
    public GameObject fastforward;

    public void Start()
	{
        play = GameObject.FindWithTag("PlayButton");
        pause = GameObject.FindWithTag("PauseButton");
        fastforward = GameObject.FindWithTag("FastForwardButton");

        //Pause game when scene loads
        Time.timeScale = 0;

        //Set savedTimeScale to 1 so 'PlayButton' will operate correctly 
        savedTimeScale = 1;

        Debug.Log("Start -> " + Time.timeScale);
    }


	public void PlayButton()
	{
        play = GameObject.FindWithTag("PlayButton");
        pause = GameObject.FindWithTag("PauseButton");
        fastforward = GameObject.FindWithTag("FastForwardButton");

        //IF game is paused THEN set timeScale to saved time
        if (Time.timeScale == 0)
        {
            Time.timeScale = savedTimeScale;
            pause.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            if (Time.timeScale == 2) fastforward.GetComponent<UnityEngine.UI.Image>().color = Color.green;
            else play.GetComponent<UnityEngine.UI.Image>().color = Color.green;
        }

        else if (Time.timeScale == 1)
        {
            play.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            PauseButton();
        }

        //IF timeScale is '2' THEN set timeScale to '1'
        else if(Time.timeScale == 2)
        {
            play.GetComponent<UnityEngine.UI.Image>().color = Color.green;
            Time.timeScale = 1;
            fastforward.GetComponent<UnityEngine.UI.Image>().color = Color.white;
        }

        Debug.Log("PlayButton -> " + Time.timeScale);
    }


	public void PauseButton()
	{
        //IF game is NOT paused THEN save time and pause game
        if(Time.timeScale != 0)
        {
            pause.GetComponent<UnityEngine.UI.Image>().color = Color.grey;
            play.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            fastforward.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0;     
        }
        else
        {
            pause.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            PlayButton();
        }

        Debug.Log("PauseButton -> " + Time.timeScale);
    }


	public void FastForwardButton()
    {

        //IF timeScale is not 2 THEN double speed to 2 AND save time
        if (Time.timeScale != 2)
        {
            pause.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            play.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            fastforward.GetComponent<UnityEngine.UI.Image>().color = Color.green;
            Time.timeScale = 2;
            savedTimeScale = Time.timeScale;
        }

        //IF timeScale is 2 THEN restore speed to 1 AND save time
        else if (Time.timeScale == 2)
        {
            fastforward.GetComponent<UnityEngine.UI.Image>().color = Color.white;
            play.GetComponent<UnityEngine.UI.Image>().color = Color.green;
            Time.timeScale = 1.0f;
            savedTimeScale = Time.timeScale;

            //Remove highlight from button
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("FastForwardButton -> " + Time.timeScale);
    }


    public void ResetTime()
    {
        //Pause game and set savedTime to 1 so play button will work correctly
        Time.timeScale = 0;
        savedTimeScale = 1;

        Debug.Log("ResetButton -> " + Time.timeScale);
    }


    //Used to govern behavior for Rules Message dialog that appears at start of game and must be dismissed by user
    public void StartGameButton()
    {
        //Hide the Rule Message dialog and components
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Rules_Message"))
            obj.SetActive(false);
        //Press Play button so object "roaming" begins when users drop objects/sprites on screen
        //Eliminates need for user to press play button to start game
        PlayButton();
    }

    //Used to allow Rules Message dialog to be redisplayed by the user
    public void DisplayRules()
    {
        //First pause the game
        PauseButton();
        //Display the Rule Message dialog and components
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Rules_Message"))
            obj.SetActive(true);
    }

}
