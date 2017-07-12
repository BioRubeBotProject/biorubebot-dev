using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToggleOnOff : MonoBehaviour , Tutorial.SwitchOnOff {

	void Tutorial.SwitchOnOff.enable () {
		this.GetComponent<Toggle> ().interactable = true;
	}

  void Tutorial.SwitchOnOff.transparent (bool value) {
    if (value == true) {
      this.GetComponent<Toggle>().transition = Selectable.Transition.ColorTint;
    }
    else this.GetComponent<Toggle>().transition = Selectable.Transition.None;
  }

	void Tutorial.SwitchOnOff.disable () {
		this.GetComponent<Toggle> ().interactable = false;
	}
}
