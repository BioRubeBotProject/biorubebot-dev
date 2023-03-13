/*File:     ScrollToggle
     Purpose:  When a molecule inside the scroll menu is clicked,
               the scroll toggle script should also be included. It disables the
               scroll functionality while the mouse is dragging a molecule, so that
               the spawner.cs script puts the molecule back in the correct position
               on mouseup. This script is separate from spawner.cs so that levels
               without a scroll bar won't be affected.

     Author: Codey Sivley
     Date: 3 / 12 / 2023
     Branch Implemented: Spring2023 - Dev
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollToggle : MonoBehaviour
{

     void OnMouseDown()
     {
          GameObject.Find("ScrollContainer").GetComponent<ScrollRect>().enabled = false;
     }

     void OnMouseUp()
     {
          GameObject.Find("ScrollContainer").GetComponent<ScrollRect>().enabled = true;
     }

}

    

