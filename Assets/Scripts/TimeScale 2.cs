using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using UnityEngine.EventSystems;

public class TimeScale : MonoBehaviour
{
	private static float savedTimeScale;


	public void Start()
	{
        //Pause game when scene loads
		Time.timeScale = 0;

        //Set savedTimeScale to 1 so 'PlayButton' will operate correctly 
        savedTimeScale = 1;

        Debug.Log("Start -> " + Time.timeScale);
    }


	public void PlayButton()
	{
        //IF game is paused THEN set timeScale to saved time
        if(Time.timeScale == 0)
        {
            Time.timeScale = savedTimeScale;
        }

        //IF timeScale is '2' THEN set timeScale to '1'
        else if(Time.timeScale == 2)
        {
            Time.timeScale = 1;
        }

        Debug.Log("PlayButton -> " + Time.timeScale);
    }


	public void PauseButton()
	{
        //IF game is NOT paused THEN save time and pause game
        if(Time.timeScale != 0)
        {
            savedTimeScale = Time.timeScale;
            Time.timeScale = 0;     
        }

        Debug.Log("PauseButton -> " + Time.timeScale);
    }


	public void FastForwardButton()
    {
        //IF timeScale is not 2 THEN double speed to 2 AND save time
        if(Time.timeScale != 2)
        {
            Time.timeScale = 2;
            savedTimeScale = Time.timeScale;
        }

        //IF timeScale is 2 THEN restore speed to 1 AND save time
        else if (Time.timeScale == 2)
        {
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

}
