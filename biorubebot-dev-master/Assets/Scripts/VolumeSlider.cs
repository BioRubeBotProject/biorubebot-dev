using UnityEngine;
using System.Collections;

public class VolumeSlider : MonoBehaviour 
{
	float s = 1.0F;
	AudioListener main;
	
	void Start()
	{
		main = Camera.main.GetComponent<AudioListener>();
	}
	
	void Update()
	{
		main.GetComponent<AudioSource>().volume = s;
	}
	
	void OnGUI()  //creates a horizontal slider to function as the volume control 
	{
		s = GUI.HorizontalSlider (new Rect(20, 30, 75, 75), s, 0.0F, 1.0F);
	}
}