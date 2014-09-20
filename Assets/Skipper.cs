using System;
using UnityEngine;
using System.Collections;

public class Skipper : MonoBehaviour
{

	private float acceleration = -3;
	private float velocity = 4;
	private bool started = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (started)
		{
			velocity += acceleration * Time.deltaTime;
			//print(velocity);
			transform.position = new Vector3(transform.position.x, transform.position.y + velocity * Time.deltaTime, 0f);

			transform.eulerAngles = new Vector3(0, 0, Math.Min(velocity, 6) / 6 * 25);

			//Animator animator = GetComponent<Animator>();
			//if (transform.position.y > 0)
			//{
			//	animator.SetTrigger("StartAirborn");
			//}
			//else
			//{
			//	animator.SetTrigger("StartScared");
			//}
		}
	}


	public void StartFlight()
	{
		print("Flight Started");
		velocity = 4;
		//Animator animator = GetComponent<Animator>();
		//animator.SetTrigger("StartFlight");

		// Even though the flight animation sequence doesn't touch the transform, I need to disable this in order to be able to modify the transform manually
		//animator.enabled = false;
		started = true;
	}


	public void Skip(float depth)
	{
		if (!started) return;

		print("Skip! Depth = " + depth);
		
		const float perfectSkip = 5;

		//var percentPerfect = 1 - (Math.abs(depth - 10) / 20);  // ORIG
		float percentPerfect = 1 - (Math.Abs(depth - perfectSkip) / 20);



		//if(this._velocity.y < 0 && depth < 30 && depth > -10){	// ORIG
		if (velocity < 0 && depth < (20 + perfectSkip) && depth > -(20 - perfectSkip))
		{
			velocity = -(velocity * 1.4f * percentPerfect) - (30 / 120f);

			var jumpQuality = "";
			if (percentPerfect > 0.90)
			{
				jumpQuality = "Perfect!";
				print("percent perfect: " + percentPerfect);
				print("velocity: " + velocity);
			}
			else if (percentPerfect > 0.8)
			{
				jumpQuality = "Great!";
				print("percent perfect: " + percentPerfect);
				print("velocity: " + velocity);
			}
			else if (percentPerfect > 0.6)
			{
				jumpQuality = "Good";
				print("percent perfect: " + percentPerfect);
				print("velocity: " + velocity);
			}
			else if (percentPerfect > 0.4)
			{
				jumpQuality = "Not Great";
				print("percent perfect: " + percentPerfect);
				print("velocity: " + velocity);
			}
			else
			{
				if (depth - perfectSkip > 0)
				{
					jumpQuality = "Too Late";
				}
				else
				{
					jumpQuality = "Too Soon";
				}
			}

			print(jumpQuality);
		}
	}
}
