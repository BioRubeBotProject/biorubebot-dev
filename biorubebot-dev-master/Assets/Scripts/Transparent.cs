using UnityEngine;
using System.Collections;

public class Transparent : MonoBehaviour {

	public float alpha = 0.25f;

  public void setAlpha ( float a ) {
    alpha = a;
  }

  public void setAlpha (  ){
   
      Color a = this.gameObject.GetComponentInChildren<Renderer> ().material.color;
      // ().material.color;
      a.a = alpha;
  		
      this.gameObject.GetComponentInChildren<Renderer> ().material.color = a;
  }

  void update () {
    setAlpha();
  }
	
}
