using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectCollection : MonoBehaviour {
  public List<GameObject> Objects = new List<GameObject>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
    Objects = Objects.Where(item => item != null).ToList();
	}

  void Update() {
    Objects = Objects.Where(item => item != null).ToList();
  }

  public void Add (GameObject obj) {
    Objects.Add (obj);
  }

  public void Clear() {
    for (int i = 0; i < Objects.Count; i++) {
      GameObject obj = Objects [i];
      GameObject.Destroy (obj);
    }
  }
}
