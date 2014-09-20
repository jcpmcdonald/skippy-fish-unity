using System;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
	enum State
	{
		mainMenu,
		introScene,
		skipping,
		outroScene,
		scoreMenu
	}

	private State state = State.mainMenu;
	public Animator TitleTextAnimator;
	//public Animator SkipperAnimator;
	public Transform SkipperTransform;
	public Skipper skipper;

	// Use this for initialization
	void Start () {
		SkipperTransform.position = new Vector3(camera.rect.xMin, 0, 0);
	}
	
	// Update is called once per frame
	void Update ()
	{

		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }

		int i = 0;
		bool tap = Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0);
		while (i < Input.touchCount)
		{
			tap = tap || Input.GetTouch(i).phase == TouchPhase.Began || Input.GetTouch(i).phase == TouchPhase.Ended;
			i++;
		}

		float depth = -SkipperTransform.position.y; /* + height */ //- Game.waterSurface();
		depth = depth * 150; // Convert to the units I used in the old JS version to keep the remaining logic the same


		switch (state)
		{
			case State.mainMenu:
				if (tap)
				{
					state = State.introScene;
					TitleTextAnimator.SetTrigger("TextExit");
					//SkipperAnimator.SetTrigger("StartIntro");

				}

				break;

			case State.introScene:
				if (depth < 0)
				{
					state = State.skipping;
				}
				break;


			case State.skipping:
				if (depth > 20)
				{
					print("Dead!");
					//SkipperAnimator.enabled = true;
					//SkipperAnimator.SetTrigger("StartOutro");
					state = State.outroScene;
				}
				else if (tap)
				{
					skipper.Skip(depth);
				}
				break;
		}

		
		//print(depth);


		//while (i < Input.touchCount)
		//{
		//	if (Input.GetTouch(i).phase == TouchPhase.Began)
		//		clone = Instantiate(projectile, transform.position, transform.rotation) as GameObject;

		//	++i;
		//}
	}


	void OnEnable()
	{
		Application.RegisterLogCallback(OnLogException);
	}
	void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}


	static private void OnLogException(string _message, string _stackTrace, LogType _logType)
	{
		if (_logType == LogType.Exception)
		{
			if (Application.isEditor)
			{
				// Only break in editor to allow examination of the current scene state.
				Debug.Break();
			}
			else
			{
				// There's no standard way to return an error code to the OS,
				// so just quit regularly.
				Application.Quit();
			}
		}
	} 
}
