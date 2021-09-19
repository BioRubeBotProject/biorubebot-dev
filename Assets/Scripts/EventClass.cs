using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct UIObj {
  public GameObject EventObject;
  public bool transparent;
  public bool flag;
}

[System.Serializable]
public struct spawnParams {
  public Vector3 spawnLocation;
  public Vector3 spawnScale;
}

[System.Serializable]
public struct EventObj {
  public GameObject EventObject;
  public List<spawnParams> spawn_params;
}

[System.Serializable]
public class EventClass : Tutorial.SwitchOnOff {
  public UIObj[] UIObjects;
  private bool enabled;
  public EventObj[] EventObjects;
  public string text;
  
  void Start () {
    enabled = false;
  }
  
  public void enable () {
    for (int i = 0; i < UIObjects.Length; i++) {
      if (UIObjects [i].EventObject != null) {
        Tutorial.SwitchOnOff Interface = (Tutorial.SwitchOnOff)UIObjects [i].EventObject.GetComponent<Tutorial.SwitchOnOff> ();
        Interface.enable ();
        Interface.transparent(UIObjects[i].transparent);
        if (UIObjects [i].flag == false) {
          Interface.disable ();
        } else {
          Interface.enable ();
        }
        Interface.transparent(UIObjects[i].transparent);
      }
    }
    
    if (enabled == false) {
      for (int i = 0; i < EventObjects.Length; i++) {
        for (int j = 0; j < EventObjects[i].spawn_params.Count; j++) {
          GameObject clone;
          clone = GameObject.Instantiate (EventObjects [i].EventObject, EventObjects [i].spawn_params [j].spawnLocation, Quaternion.identity) as GameObject;
          clone.name = clone.name.Replace ("(Clone)", " ");
          clone.name = "Tutorial_" + clone.name;
          if(EventObjects[i].spawn_params[j].spawnScale.x != 0.0f) {
            if(EventObjects[i].spawn_params[j].spawnScale.y != 0.0f) {
              clone.transform.localScale = EventObjects[i].spawn_params[j].spawnScale;
            }
          }
          GameObject.Find ("EventSystem").GetComponent<ObjectCollection> ().Add (clone);
        }
      }
    }
    enabled = true;
  }
  
  void Tutorial.SwitchOnOff.transparent (bool value) {
  }
  
  public void render () {
    enable ();
    DialogueBox dialogueBox = GameObject.Find ("DialogBox").GetComponent<DialogueBox> ();
    dialogueBox.dialogue = true;
    dialogueBox.text = text;
  }
  
  public void disable () {
    if (enabled == true) {
      GameObject.Find("EventSystem").GetComponent<ObjectCollection>().Clear ();
    }
    
    for (int i = 0; i < UIObjects.Length; i++) {
      if (UIObjects [i].EventObject != null) {
        Tutorial.SwitchOnOff Interface = (Tutorial.SwitchOnOff)UIObjects [i].EventObject.GetComponent<Tutorial.SwitchOnOff> ();
        Interface.enable();
        Interface.transparent(!UIObjects[i].transparent);
        if(UIObjects[i].EventObject.name == "Drop Down panel") {
          Interface.disable ();
        }
        else {
          Interface.enable ();
        }
        
        
      }
    }
    
    GameObject.Find ("DialogBox").GetComponent<DialogueBox> ().dialogue = false;
    enabled = false;
  }
}