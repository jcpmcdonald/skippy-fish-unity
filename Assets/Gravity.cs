using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour
{

	private float acceleration = -4;
	private float velocity = 4;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		velocity += acceleration * Time.deltaTime;
		transform.position = new Vector3(transform.position.x, transform.position.y + velocity * Time.deltaTime, 0f);

		Animator animator = GetComponent<Animator>();
		if (transform.position.y > 0)
		{
			animator.SetTrigger("StartAirborn");
		}
		else
		{
			animator.SetTrigger("StartScared");
		}
	}
}
