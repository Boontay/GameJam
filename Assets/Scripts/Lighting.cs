using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour {
	public float speed = 1;
	public bool lit = true;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 pos = GameObject.Find("Player").transform.position;
		pos = new Vector3 (pos.x, pos.y, pos.z -1);
		transform.position = pos;

	}
}
