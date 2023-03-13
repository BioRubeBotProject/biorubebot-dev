using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonOnOff : MonoBehaviour , Tutorial.SwitchOnOff
{

	void Tutorial.SwitchOnOff.enable ()
    {
        this.GetComponent<Button> ().interactable = true;
    }

    void Tutorial.SwitchOnOff.transparent (bool value)
    {
        if (value == true)
        {
            this.GetComponent<Button>().transition = Selectable.Transition.ColorTint;
        }

        else
            this.GetComponent<Button>().transition = Selectable.Transition.None;
    }

    void Tutorial.SwitchOnOff.disable ()
    {
        this.GetComponent<Button> ().interactable = false;
    }

}
