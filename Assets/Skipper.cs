using System;
using System.Net.Mime;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Skipper : MonoBehaviour
{

	private float acceleration = -2;
	private float velocity;
	private bool started = false;
	public bool dead = false;
	public int skipCount;
	public float skipDistance;
	public float maxHeight;
	//public float airTime;
	public TimeSpan airTime;

	public Text seconds;
	public Text millis;
	public Text textSkipQuality;

	private bool skippedThisFall = false;

	public AudioSource audioSource;
	public AudioClip skip;
	

	//AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) {
	//	var newAudio = gameObject.AddComponent<AudioSource>();
	//	newAudio.clip = clip;
	//	newAudio.loop = loop;
	//	newAudio.playOnAwake = playAwake;
	//	newAudio.volume = vol;
	//	return newAudio;
	//}

	//void Awake()
	//{
	//	//audioSkip = AddAudio(clipSkip, false, false, 1.0f);
	//	audioSource = gameObject.AddComponent<AudioSource>();
	//	audioSource.volume = 1.0f;
	//	audioSource.maxDistance = 0;
	//}

	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (started && !dead)
		{
			velocity += acceleration * Time.deltaTime;
			//print(velocity);
			transform.position = new Vector3(transform.position.x, transform.position.y + velocity * Time.deltaTime, 0f);

			transform.eulerAngles = new Vector3(0, 0, Math.Min(velocity, 3) / 3 * 25);

			airTime += TimeSpan.FromSeconds(Time.deltaTime);
			seconds.text = ((int)airTime.TotalSeconds).ToString();
			millis.text = (airTime.Milliseconds / 10).ToString("00");

			// Prevent jumping twice in one fall
			if (velocity > 0)
			{
				//console.log("skip reset");
				skippedThisFall = false;
			}
		}
	}


	public void StartFlight()
	{
		print("Flight Started");
		velocity = 3.32f;
		skipCount = 0;
		dead = false;
		airTime = TimeSpan.Zero;

		audioSource.PlayOneShot(skip);

		Animator animator = GetComponent<Animator>();
		animator.SetTrigger("Air");

		// Even though the flight animation sequence doesn't touch the transform, I need to disable this in order to be able to modify the transform manually
		//animator.enabled = false;
		started = true;
	}


	public void Skip(float depth)
	{
		if (!started || skippedThisFall) return;

		const float perfectSkip = 0;
		//const float perfectSkip = 0;

		//var percentPerfect = 1 - (Math.abs(depth - 10) / 20);  // ORIG
		float percentPerfect = 1 - (Math.Abs(depth - perfectSkip) / 20);

		//print(depth);
		//print(percentPerfect);
		//Time.timeScale = Time.timeScale >= 0.9 ? 0 : 1;


		//if(this._velocity.y < 0 && depth < 30 && depth > -10){	// ORIG
		if (velocity < 0 && depth < (20 + perfectSkip) && depth > -(20 - perfectSkip))
		{
			velocity = -(velocity * 1.4f * percentPerfect) - (30f / 75f);
			skipCount++;

			var jumpQuality = "";
			if (percentPerfect > 0.90)
			{
				jumpQuality = "Perfect!";
			}
			else if (percentPerfect > 0.75)
			{
				jumpQuality = "Great!";
			}
			else if (percentPerfect > 0.6)
			{
				jumpQuality = "Good";
			}
			else if (percentPerfect > 0.4)
			{
				jumpQuality = "Not Great";
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

			//print(jumpQuality);
			textSkipQuality.text = jumpQuality;
			textSkipQuality.color = new Color(textSkipQuality.color.r, textSkipQuality.color.g, textSkipQuality.color.b, 1);
			skippedThisFall = true;

			audioSource.PlayOneShot(skip);
		}
	}
}
