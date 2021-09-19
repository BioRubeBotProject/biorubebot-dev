using UnityEngine;
using System.Collections;
using System;

public class DialogueBox : MonoBehaviour {
  public bool dialogue;
  public string text;
  public GUIStyle style;
  public string buttonText;

  private int _FontSize;
  public int Ratio = 40;

  Rect Box;
  void Start () {
    Box = new Rect (200,375,400,150);
  }

  
  void Update () {
    _FontSize = Mathf.Min(Screen.width, Screen.height) / Ratio;
    style.fontSize = _FontSize;
    Box = ResizeGUI(new Rect (200,375,400,150));
    style.fixedHeight = Box.height;
    style.fixedWidth = Box.width;
  }
  
  void OnGUI() {

    if(dialogue == true) {
      GUI.BeginGroup (Box);

      string newText = text.Replace("\\n","\n");
      GUI.Box(new Rect(0,0,Box.width - 5,Box.height - 5),"");


      GUI.Label(new Rect(0,0,Box.width-10,Box.height-10),newText,style);

        GUI.BeginGroup(ResizeGUI(new Rect(325,118,65,20)));
          Rect button = ResizeGUI(new Rect(0,0,65,20));
          
          if(GUI.Button(button,buttonText)) {
            GameObject.Find ("EventSystem").GetComponent<Tutorial>().NextScene();
          }
          //GUI.Label (button,buttonText,style);
        GUI.EndGroup ();
      GUI.EndGroup ();
    }
  }
  
  Rect ResizeGUI(Rect _rect)
  {
    float FilScreenWidth = _rect.width / 800;
    float rectWidth = FilScreenWidth * Screen.width;
    float FilScreenHeight = _rect.height / 600;
    float rectHeight = FilScreenHeight * Screen.height;
    float rectX = (_rect.x / 800) * Screen.width;
    float rectY = (_rect.y / 600) * Screen.height;
    
    return new Rect(rectX,rectY,rectWidth,rectHeight);
  }
}