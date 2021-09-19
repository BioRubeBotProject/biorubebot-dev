using UnityEngine;
using System.Collections;

public class PanelOnOff : MonoBehaviour , Tutorial.SwitchOnOff {

  void Tutorial.SwitchOnOff.enable () {
    this.gameObject.SetActive(true);
  }

  void Tutorial.SwitchOnOff.transparent(bool value) {
  }
  
  void Tutorial.SwitchOnOff.disable() {
    this.gameObject.SetActive(false);
  }

}
