using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using SceneCore;
public class testMemory : MonoBehaviour {

    public Vector2 target = new Vector2(3, 1);

	private GameScene scene;

	private Vector3 pos = new Vector3();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void OnGUI() {
		if (GUI.Button (new Rect (20, 20, 200, 50), "path find")) {
		}

	}


}
