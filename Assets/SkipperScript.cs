using UnityEngine;
using System.Collections;

public class SkipperScript : MonoBehaviour {

	private float acceleration = -4;
	private float velocity = 4;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		velocity += acceleration * Time.deltaTime;
		this.transform.position.Set(transform.position.x, transform.position.y + velocity * Time.deltaTime, 0);
	}
}
