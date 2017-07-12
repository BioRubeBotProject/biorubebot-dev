using UnityEngine;
using System.Collections;

public class congratsOnOff : MonoBehaviour, Tutorial.SwitchOnOff
{

    void Tutorial.SwitchOnOff.enable()
    {
        if (tag == "T_Reg_Complete")
        {
            this.gameObject.SetActive(true);
        }
    }

    void Tutorial.SwitchOnOff.transparent(bool value)
    {
    }

    void Tutorial.SwitchOnOff.disable()
    {
        this.gameObject.SetActive(false);
    }

}
