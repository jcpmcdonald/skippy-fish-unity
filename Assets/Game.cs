using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Glide;
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
	public RectTransform TitleTextTransform;
	public RectTransform TextClickAnywhereTransform;
	public Canvas UICanvas;

	public Animator SkipperAnimator;
	public Transform SkipperTransform;
	public Skipper skipper;

	private Tweener Tweener = new Tweener();

	private TweenTransformWrap SkipperTransformWrap;
	private TweenTransformWrap CameraTrasnsformWrap;
	private TweenTransformWrap TextTitleTrasnsformWrap;
	private TweenTransformWrap TextClickAnywhereTransformWrap;

	private int maxSkips;
	private float maxDistance;
	private float maxAltitude;
	private float maxAirTime;
	private int gamesFinished;

	private bool soundOn;

	// Use this for initialization
	private void Start()
	{
		SkipperTransformWrap = new TweenTransformWrap(SkipperTransform);
		CameraTrasnsformWrap = new TweenTransformWrap(camera.transform);
		TextTitleTrasnsformWrap = new TweenTransformWrap(TitleTextTransform);
		TextClickAnywhereTransformWrap = new TweenTransformWrap(TextClickAnywhereTransform);
		
		SkipperTransform.position = new Vector3(camera.ViewportToWorldPoint(new Vector3(0, 0.5f)).x - 0.5f, 0, 0);
		camera.transform.position = new Vector3(camera.transform.position.x, 1, camera.transform.position.z);

		maxSkips = PlayerPrefs.GetInt("maxSkips", 0);
		maxDistance =PlayerPrefs.GetFloat("maxDistance", 0);
		maxAltitude = PlayerPrefs.GetFloat("maxAltitude", 0);
		maxAirTime = PlayerPrefs.GetFloat("maxAirTime", 0);
		gamesFinished = PlayerPrefs.GetInt("gamesFinished", 0);
		soundOn = (PlayerPrefs.GetInt("soundOn", 1) == 1);

		//Tweener.Tween(SkipperTrasnsformWrap, new { X = 0.5f }, 2)
		//	.OnComplete(() => Tweener.Tween(SkipperTrasnsformWrap, new { X = 0f }, 1));

		//Tweener.Tween(CameraTrasnsformWrap, new { Y = 1f }, 1f).Ease(Ease.CubeInOut);
	}


	// Update is called once per frame
	void Update ()
	{

		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }

		Tweener.Update(Time.deltaTime);

		int i = 0;
		bool tap = Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0);
		while (i < Input.touchCount)
		{
			tap = tap || Input.GetTouch(i).phase == TouchPhase.Began || Input.GetTouch(i).phase == TouchPhase.Ended;
			i++;
		}

		float depth = -SkipperTransform.position.y; /* + height */ //- Game.waterSurface();
		depth = depth * 75; // Convert to the units I used in the old JS version to keep the remaining logic the same


		switch (state)
		{
			case State.mainMenu:
				if (tap)
				{
					state = State.introScene;

					Tweener.Tween(TextTitleTrasnsformWrap, new{ X = TitleTextTransform.rect.width + UICanvas.pixelRect.width }, 0.9f).Ease(Ease.QuadInOut);
					Tweener.Tween(TextClickAnywhereTransformWrap, new { X = TextClickAnywhereTransform.rect.width + UICanvas.pixelRect.width }, 0.6f).Ease(Ease.QuadInOut);
					//TitleTextAnimator.SetTrigger("TextExit");
					//SkipperAnimator.SetTrigger("StartIntro");

					skipper.renderer.enabled = true;


					SkipperTransformWrap.Y = -2;
					SkipperTransformWrap.X = camera.ViewportToWorldPoint(new Vector3(-0.1f, 0)).x;
					SkipperAnimator.SetTrigger("Swim");

					Tweener.Tween(CameraTrasnsformWrap, new{ Y = -1.8 }, 2f).Ease(Ease.CubeInOut);
					Tweener.Timer(1.5f)
					       .OnComplete(() =>
					                   Tweener.Tween(SkipperTransformWrap, new{ X = camera.ViewportToWorldPoint(new Vector3(1f / 5f, 0)).x }, 2).Ease(Ease.QuadOut)
					                          .OnComplete(() =>
					                                      {
						                                      SkipperAnimator.SetTrigger("Scared");
						                                      Tweener.Tween(SkipperTransformWrap, new{ X = camera.ViewportToWorldPoint(new Vector3(0.5f, 0)).x }, 1).Ease(Ease.QuadOut);
						                                      Tweener.Tween(SkipperTransformWrap, new{ Y = 0, EulerAnglesZ = 35 }, 0.9f).Ease(Ease.ExpoIn);
						                                      Tweener.Tween(CameraTrasnsformWrap, new{ Y = 0 }, 0.9f); //.Ease(Ease.CubeInOut);
					                                      }));
				}

				break;

			case State.introScene:
				if (depth <= 0)
				{
					state = State.skipping;
					skipper.StartFlight();
				}
				break;


			case State.skipping:

				CameraTrasnsformWrap.Y = SkipperTransform.position.y / 4;
				//print(depth);

				if (depth > 20)
				{
					skipper.dead = true;
					state = State.outroScene;
					PlayOutro();

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


	void PlayOutro()
	{
		if (skipper.skipCount == 0)
		{
			// Show them some instructions
			//fail.show();
		}


		if(skipper.skipCount > maxSkips){
			maxSkips = skipper.skipCount;
			PlayerPrefs.SetInt("maxSkips", maxSkips);
			//Crafty.storage('maxSkips', Game.maxSkips);
			//$("#skipRecord").show();
		} else {
			//$("#skipRecord").hide();
		}
	
		if(skipper.skipDistance > maxDistance){
			maxDistance = skipper.skipDistance;
			PlayerPrefs.SetFloat("maxDistance", maxDistance);
			//Crafty.storage('maxDistance', Game.maxDistance);
			//$("#distanceRecord").show();
		} else {
			//$("#distanceRecord").hide();
		}
	
		if(skipper.maxHeight < maxAltitude){
			maxAltitude = skipper.maxHeight;
			PlayerPrefs.SetFloat("maxAltitude", maxAltitude);
			//Crafty.storage('maxAltitude', Game.maxAltitude);
			//$("#altitudeRecord").show();
		} else {
			//$("#altitudeRecord").hide();
		}
	
		if(skipper.airTime.TotalSeconds > maxAirTime){
			maxAirTime = (float)skipper.airTime.TotalSeconds;
			PlayerPrefs.SetFloat("maxAirTime", maxAirTime);
			//Crafty.storage('maxAirTime', Game.maxAirTime);
			//$("#newAirTimeRecord").show();
			//$("#scoreBestAirTime").hide();
		} else {
			//$("#newAirTimeRecord").hide();
			//$("#scoreBestAirTime").show();
		}

		gamesFinished ++;
		PlayerPrefs.SetInt("gamesFinished", gamesFinished);
		PlayerPrefs.Save();


		SkipperAnimator.SetTrigger("Scared");
		//killer.animate("mouthOpen");
		//Tweener.Tween(Crafty.viewport).to({y: -(300 * Game.scale)}, 2);
		Tweener.Tween(CameraTrasnsformWrap, new{ Y = -1.8 }, 2f).Ease(Ease.CubeInOut);

		//Game.playSound("scream");
		//Game.playSound("swim");

		Tweener.Tween(SkipperTransformWrap, new { EulerAnglesZ = 360 }, 0.5f).Ease(Ease.CubeInOut).OnComplete(() => { SkipperTransformWrap.EulerAnglesZ = 0; });
		Tweener.Tween(SkipperTransformWrap, new { X = camera.ViewportToWorldPoint(new Vector3(0.7f, 0)).x, Y = -2 }, 1.9f).Ease(Ease.QuadOut)
		       .OnComplete(() =>
		                   {
			                   skipper.renderer.enabled = false;
		                   });
		Tweener.Timer(2.8f)
		       .OnComplete(() =>
		                   {
			                   Tweener.Tween(CameraTrasnsformWrap, new{ Y = 1 }, 1f).Ease(Ease.CubeInOut);
			                   TextTitleTrasnsformWrap.X = -(TitleTextTransform.rect.width + UICanvas.pixelRect.width);
			                   TextClickAnywhereTransformWrap.X = -(TextClickAnywhereTransform.rect.width + UICanvas.pixelRect.width);
			                   Tweener.Tween(TextTitleTrasnsformWrap, new{ X = UICanvas.pixelRect.width / 2 }, 0.9f).Ease(Ease.QuadOut);
			                   Tweener.Tween(TextClickAnywhereTransformWrap, new{ X = UICanvas.pixelRect.width / 2 }, 0.6f).Ease(Ease.QuadOut).OnComplete(() => state = State.mainMenu);
		                   });



	}


	#region Break on exception
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
	#endregion
}
