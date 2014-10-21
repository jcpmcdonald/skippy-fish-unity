using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Glide;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	private bool firstPlay = true;
	private State state = State.mainMenu;
	public Canvas UICanvas;
	public Animator SkipperAnimator;

	public Transform SkipperTransform;
	public RectTransform TitleTextTransform;
	public Text ClickAnywhere;
	public Text CurrentScoreText;
	public Text LastScore;
	public Text LastScoreSecs;
	public Text LastScoreMillis;
	public Text BestScore;
	public Text BestScoreSecs;
	public Text BestScoreMillis;
	public Text NewRecord;

	public Image NewRecordStar1;
	public Image NewRecordStar2;

	private TweenTransformWrap SkipperTransformWrap;
	private TweenTransformWrap SnappyTransformWrap;
	private TweenTransformWrap CameraTransformWrap;
	private TweenTransformWrap TextTitleTransformWrap;
	private TweenTransformWrap TextClickAnywhereTransformWrap;
	private TweenTransformWrap CurrentScoreTextTransformWrap;
	private TweenTransformWrap LastTransformWrap;
	private TweenTransformWrap BestTransformWrap;
	private TweenTransformWrap NewRecordTransformWrap;

	public Skipper skipper;
	public Animator SnappyAnimator;
	public Transform SnappyTransform;

	private Tweener Tweener = new Tweener();

	private int maxSkips;
	private float maxDistance;
	private float maxAltitude;
	private TimeSpan maxAirTime;
	private int gamesFinished;

	private AudioSource audioSource;
	public AudioClip scream;
	public AudioClip chew;
	public AudioClip waterReentry;
	public AudioClip swim;
	public AudioClip bubbles;

	public RectTransform soundPanel;

	public ToggleButton soundButton;
	public ToggleButton muteButton;

	public SpriteRenderer fail;
	private TweenColorWrap failColorWrap;

	public GoogleMobileAdsDemoScript Ads;

	private float targetCameraY;

	private float titleOnscreenCenter;
	private float titleOffscreenRight;
	private float titleOffscreenLeft;

	private float clickAnywhereOnscreenCenter;
	private float clickAnywhereOffscreenRight;
	private float clickAnywhereOffscreenLeft;

	private float scoreOnscreenCenter;
	private float scoreOffscreenRight;
	private float scoreOffscreenLeft;

	private float recordOnscreenCenter;
	private float recordOffscreenRight;
	private float recordOffscreenLeft;

	void Awake()
	{
	//	//audioSkip = AddAudio(clipSkip, false, false, 1.0f);
	//	audioSource = gameObject.AddComponent<AudioSource>();
	//	audioSource.volume = 1.0f;

		
	}


	public void SoundSettingsChanged()
	{
		PlayerPrefs.SetInt("soundOn", audioSource.mute ? 0 : 1);
	}


	// Use this for initialization
	private void Start()
	{
		audioSource = gameObject.GetComponent<AudioSource>();

		bool soundOn = (PlayerPrefs.GetInt("soundOn", 1) == 1);
		if (!soundOn)
		{
			soundButton.Enabled = false;
			muteButton.Enabled = true;
			audioSource.mute = true;
		}
		
		titleOnscreenCenter = TitleTextTransform.position.x;
		titleOffscreenRight = TitleTextTransform.position.x + UICanvas.pixelRect.xMax;
		titleOffscreenLeft = TitleTextTransform.position.x - UICanvas.pixelRect.xMax;

		clickAnywhereOnscreenCenter = ClickAnywhere.transform.position.x;
		clickAnywhereOffscreenRight = ClickAnywhere.transform.position.x + UICanvas.pixelRect.xMax;
		clickAnywhereOffscreenLeft = ClickAnywhere.transform.position.x - UICanvas.pixelRect.xMax;

		scoreOnscreenCenter = LastScore.transform.position.x;
		scoreOffscreenRight = LastScore.transform.position.x + UICanvas.pixelRect.xMax;
		scoreOffscreenLeft = LastScore.transform.position.x - UICanvas.pixelRect.xMax;

		recordOnscreenCenter = NewRecord.transform.position.x;
		recordOffscreenRight = NewRecord.transform.position.x + UICanvas.pixelRect.xMax;
		recordOffscreenLeft = NewRecord.transform.position.x - UICanvas.pixelRect.xMax;

		// Move the score off-screen left
		CurrentScoreText.transform.position = new Vector3(CurrentScoreText.transform.position.x - UICanvas.pixelRect.width, CurrentScoreText.transform.position.y);
		LastScore.transform.position = new Vector3(scoreOffscreenLeft, LastScore.transform.position.y);
		BestScore.transform.position = new Vector3(scoreOffscreenLeft, BestScore.transform.position.y);
		NewRecord.transform.position = new Vector3(recordOffscreenLeft, NewRecord.transform.position.y);

		//-(SnappyAnimator.renderer.bounds.size.x)
		SnappyTransform.position = new Vector3(camera.ViewportToWorldPoint(new Vector3(-1f, 0)).x, -1);


		SkipperTransformWrap = new TweenTransformWrap(SkipperTransform);
		SnappyTransformWrap = new TweenTransformWrap(SnappyTransform);
		CameraTransformWrap = new TweenTransformWrap(camera.transform);
		TextTitleTransformWrap = new TweenTransformWrap(TitleTextTransform);
		TextClickAnywhereTransformWrap = new TweenTransformWrap(ClickAnywhere.transform);
		CurrentScoreTextTransformWrap = new TweenTransformWrap(CurrentScoreText.transform);
		LastTransformWrap = new TweenTransformWrap(LastScore.transform);
		BestTransformWrap = new TweenTransformWrap(BestScore.transform);
		NewRecordTransformWrap = new TweenTransformWrap(NewRecord.transform);

		failColorWrap = new TweenColorWrap(fail);


		SkipperTransform.position = new Vector3(camera.ViewportToWorldPoint(new Vector3(0, 0)).x - 0.5f, 0, 0);
		//SnappyTransform.position = new Vector3(camera.ViewportToWorldPoint(new Vector3(0, 0)).x - 0.5f, 0, 0);
		camera.transform.position = new Vector3(camera.transform.position.x, 1, camera.transform.position.z);

		maxSkips = PlayerPrefs.GetInt("maxSkips", 0);
		maxDistance =PlayerPrefs.GetFloat("maxDistance", 0);
		maxAltitude = PlayerPrefs.GetFloat("maxAltitude", 0);
		maxAirTime = TimeSpan.FromSeconds(PlayerPrefs.GetFloat("maxAirTime", 0));
		gamesFinished = PlayerPrefs.GetInt("gamesFinished", 0);


		//Tweener.Tween(SkipperTrasnsformWrap, new { X = 0.5f }, 2)
		//	.OnComplete(() => Tweener.Tween(SkipperTrasnsformWrap, new { X = 0f }, 1));

		//Tweener.Tween(CameraTrasnsformWrap, new { Y = 1f }, 1f).Ease(Ease.CubeInOut);
	}


	// Update is called once per frame
	void Update ()
	{

		

		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }

		Tweener.Update(Time.deltaTime);

		//print(camera.WorldToViewportPoint(Input.mousePosition));

		int i = 0;

		
		//print(RectTransformUtility.RectangleContainsScreenPoint(soundPanel, Input.mousePosition, camera));
		//print(RectTransformUtility.PixelAdjustRect(soundPanel, UICanvas));//.Contains(Input.mousePosition));
		//print(camera.ViewportToScreenPoint(soundPanel.anchorMax).x - soundPanel.rect.width);

		Vector2 topLeft = (Vector2)soundPanel.transform.position - (soundPanel.sizeDelta / 2f);
		Rect soundRect = new Rect(topLeft.x, topLeft.y, soundPanel.sizeDelta.x, soundPanel.sizeDelta.y);


		bool tap = (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0)) && !soundRect.Contains(Input.mousePosition);
		while (i < Input.touchCount)
		{
			tap = tap || ((Input.GetTouch(i).phase == TouchPhase.Began || Input.GetTouch(i).phase == TouchPhase.Ended) && !soundRect.Contains(Input.GetTouch(i).position));
			i++;
		}

		float depth = -SkipperTransform.position.y; /* + height */ //- Game.waterSurface();
		depth = depth * 90; // Convert to the units I used in the old JS version to keep the remaining logic the same


		switch (state)
		{
			case State.mainMenu:
				if (tap)
				{
					if (!firstPlay)
					{
						Ads.Hide();
					}

					state = State.introScene;


					//Tweener.Tween(TextTitleTransformWrap, new{ X = TitleTextTransform.position.x + UICanvas.pixelRect.xMax }, 0.9f).Ease(Ease.QuadInOut);
					//Tweener.Tween(TextClickAnywhereTransformWrap, new { X = ClickAnywhere.rectTransform.rect.width + UICanvas.pixelRect.width }, 0.6f).Ease(Ease.QuadInOut);

					Tweener.Tween(TextTitleTransformWrap, new { X = titleOffscreenRight }, 0.9f).Ease(Ease.QuadInOut);
					Tweener.Tween(TextClickAnywhereTransformWrap, new { X = clickAnywhereOffscreenRight }, 0.6f).Ease(Ease.QuadInOut);

					if (!firstPlay)
					{
						//Tweener.Tween(LastTransformWrap, new { X = LastTransformWrap.X + UICanvas.pixelRect.width }, 0.6f).Ease(Ease.QuadInOut).OnComplete(() => { LastTransformWrap.X -= (UICanvas.pixelRect.width * 2f); });
						//Tweener.Tween(BestTransformWrap, new { X = BestTransformWrap.X + UICanvas.pixelRect.width }, 0.6f).Ease(Ease.QuadInOut).OnComplete(() => { BestTransformWrap.X -= (UICanvas.pixelRect.width * 2f); });
						//Tweener.Tween(NewRecordTransformWrap, new { X = NewRecordTransformWrap.X + UICanvas.pixelRect.width }, 0.6f).Ease(Ease.QuadInOut).OnComplete(() => { NewRecordTransformWrap.X -= (UICanvas.pixelRect.width * 2f); });

						Tweener.Tween(LastTransformWrap, new { X = scoreOffscreenRight }, 0.6f).Ease(Ease.QuadInOut).OnComplete(() => { LastTransformWrap.X = scoreOffscreenLeft; });
						Tweener.Tween(BestTransformWrap, new { X = scoreOffscreenRight }, 0.6f).Ease(Ease.QuadInOut).OnComplete(() => { BestTransformWrap.X = scoreOffscreenLeft; });
						Tweener.Tween(NewRecordTransformWrap, new { X = recordOffscreenRight }, 0.6f).Ease(Ease.QuadInOut).OnComplete(() => { NewRecordTransformWrap.X = recordOffscreenLeft; });
					}

					skipper.renderer.enabled = true;


					SkipperTransformWrap.Y = -2;
					SkipperTransformWrap.X = camera.ViewportToWorldPoint(new Vector3(-0.1f, 0)).x;
					SkipperAnimator.SetTrigger("Swim");
					audioSource.PlayOneShot(swim);

					SnappyAnimator.SetTrigger("Open");
					SnappyTransform.position = new Vector3(camera.ViewportToWorldPoint(new Vector3(-1f, 0)).x, -2);

					Tweener.Timer(3.3f).OnComplete(() => SnappyAnimator.SetTrigger("Close"));

					Tweener.Tween(CameraTransformWrap, new{ Y = -1.6 }, 2f).Ease(Ease.CubeInOut);
					Tweener.Timer(1.5f)
					       .OnComplete(() =>
					                   {
						                   Tweener.Tween(SnappyTransformWrap, new{ X = camera.ViewportToWorldPoint(new Vector3(-0.07f, 0)).x }, 2.1f).Ease(Ease.QuadOut)
						                          .OnComplete(() => Tweener.Tween(SnappyTransformWrap, new{ X = camera.ViewportToWorldPoint(new Vector3(-0.12f, 0)).x }, 0.5f).Ease(Ease.QuadInOut))
						                          .OnComplete(() => Tweener.Tween(SnappyTransformWrap, new{ Y = -1 }, 3.5f).Ease(Ease.QuadInOut));
						                   Tweener.Tween(SkipperTransformWrap, new{ X = camera.ViewportToWorldPoint(new Vector3(1f / 5f, 0)).x }, 2).Ease(Ease.QuadOut)
						                          .OnComplete(() =>
						                                      {
							                                      Tweener.Tween(CurrentScoreTextTransformWrap, new{ X = CurrentScoreTextTransformWrap.X + UICanvas.pixelRect.width }, 1).Ease(Ease.ExpoIn);
							                                      SkipperAnimator.SetTrigger("Scared");
							                                      audioSource.PlayOneShot(bubbles);
							                                      Tweener.Tween(SkipperTransformWrap, new{ X = camera.ViewportToWorldPoint(new Vector3(0.5f, 0)).x }, 1).Ease(Ease.QuadOut);
							                                      Tweener.Tween(SkipperTransformWrap, new{ Y = 0, EulerAnglesZ = 35 }, 0.9f).Ease(Ease.ExpoIn);
							                                      Tweener.Tween(CameraTransformWrap, new{ Y = 0 }, 0.9f); //.Ease(Ease.CubeInOut);
						                                      });
					                   });
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

				float cameraFixedPos = SkipperTransform.position.y - 1.5f;
				float cameraFlowPos = SkipperTransform.position.y / 4;
				if (cameraFixedPos > cameraFlowPos)
				{
					targetCameraY = cameraFixedPos;
				}
				else
				{
					targetCameraY = cameraFlowPos;
				}
				CameraTransformWrap.Y = Mathf.Lerp(CameraTransformWrap.Y, targetCameraY, Time.deltaTime * 6);
				//CameraTransformWrap.Y = Mathf.Lerp(CameraTransformWrap.Y, targetCameraY, Time.deltaTime * Math.Abs(CameraTransformWrap.Y - targetCameraY) * 30);
				
				//print(CameraTransformWrap.Y + ", " + SkipperTransform.position.y);

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


		//UICanvas.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, UICanvas.transform.position.z);
	}


	void PlayOutro()
	{

		audioSource.PlayOneShot(waterReentry);
		audioSource.PlayOneShot(scream);
		audioSource.PlayOneShot(swim);

		if (skipper.skipCount == 0)
		{
			// Show them some instructions
			//fail.show();
			fail.enabled = true;
		}

		if(skipper.skipCount > maxSkips){
			maxSkips = skipper.skipCount;
			PlayerPrefs.SetInt("maxSkips", maxSkips);
		}
	
		if(skipper.skipDistance > maxDistance){
			maxDistance = skipper.skipDistance;
			PlayerPrefs.SetFloat("maxDistance", maxDistance);
		}
	
		if(skipper.maxHeight < maxAltitude){
			maxAltitude = skipper.maxHeight;
			PlayerPrefs.SetFloat("maxAltitude", maxAltitude);
		}
	
		if(skipper.airTime > maxAirTime){
			maxAirTime = skipper.airTime;
			PlayerPrefs.SetFloat("maxAirTime", (float)maxAirTime.TotalSeconds);
			NewRecord.gameObject.SetActive(true);
			BestScore.gameObject.SetActive(false);
			//NewRecord.enabled = true;
			//NewRecordStar1.enabled = true;
			//NewRecordStar2.enabled = true;
			//BestScore.enabled = false;
		} else {
			NewRecord.gameObject.SetActive(false);
			BestScore.gameObject.SetActive(true);
			//NewRecord.enabled = false;
			//NewRecordStar1.enabled = false;
			//NewRecordStar2.enabled = false;
			//BestScore.enabled = true;
		}

		LastScoreSecs.text = ((int)skipper.airTime.TotalSeconds).ToString();
		LastScoreMillis.text = (skipper.airTime.Milliseconds / 10).ToString("00");

		BestScoreSecs.text = ((int)maxAirTime.TotalSeconds).ToString();
		BestScoreMillis.text = (maxAirTime.Milliseconds / 10).ToString("00");


		gamesFinished ++;
		PlayerPrefs.SetInt("gamesFinished", gamesFinished);
		PlayerPrefs.Save();


		SkipperAnimator.SetTrigger("Scared");
		//killer.animate("mouthOpen");
		//Tweener.Tween(Crafty.viewport).to({y: -(300 * Game.scale)}, 2);
		Tweener.Tween(CameraTransformWrap, new{ Y = -1.6 }, 2f).Ease(Ease.CubeInOut);

		//Game.playSound("scream");
		//Game.playSound("swim");

		Ads.Show();

		Tweener.Tween(SkipperTransformWrap, new { EulerAnglesZ = 360 }, 0.5f).Ease(Ease.CubeInOut).OnComplete(() => { SkipperTransformWrap.EulerAnglesZ = 0; });
		Tweener.Tween(SkipperTransformWrap, new { X = camera.ViewportToWorldPoint(new Vector3(0.7f, 0)).x, Y = -2 }, 1.7f).Ease(Ease.QuadOut)
		       .OnComplete(() =>
		                   {
			                   skipper.renderer.enabled = false;
		                   });

		SnappyAnimator.SetTrigger("Open");
		Tweener.Tween(SnappyTransformWrap, new{ X = camera.ViewportToWorldPoint(new Vector3(0.7f, 0)).x, Y = -2 }, 2.1f).Ease(Ease.QuadOut);
		Tweener.Timer(1.0f)
		       .OnComplete(() =>
		                   {
			                   SnappyAnimator.SetTrigger("Chew");
			                   audioSource.PlayOneShot(chew);
			                   Tweener.Timer(1.0f).OnComplete(() => Tweener.Tween(SnappyTransformWrap, new{ X = camera.ViewportToWorldPoint(new Vector3(1.3f, 0)).x, Y = -2 }, 0.7f).Ease(Ease.QuadInOut));
			                   Tweener.Timer(1.5f).OnComplete(() => SnappyAnimator.SetTrigger("Close"));
		                   });

		Tweener.Timer(2.8f)
		       .OnComplete(() =>
		                   {
			                   firstPlay = false;

			                   // Hide the current score
			                   Tweener.Tween(CurrentScoreTextTransformWrap, new{ X = CurrentScoreTextTransformWrap.X + UICanvas.pixelRect.width }, 1).Ease(Ease.QuadOut).OnComplete(() => { CurrentScoreTextTransformWrap.X -= (UICanvas.pixelRect.width * 2f); });

			                   // Show your score
							   //Tweener.Tween(LastTransformWrap, new{ X = LastTransformWrap.X + UICanvas.pixelRect.width }, 1f).Ease(Ease.QuadOut);
							   //Tweener.Tween(BestTransformWrap, new{ X = BestTransformWrap.X + UICanvas.pixelRect.width }, 1f).Ease(Ease.QuadOut);
							   //Tweener.Tween(NewRecordTransformWrap, new{ X = NewRecordTransformWrap.X + UICanvas.pixelRect.width }, 1f).Ease(Ease.QuadOut);

							   Tweener.Tween(LastTransformWrap, new { X = scoreOnscreenCenter }, 1f).Ease(Ease.QuadOut);
							   Tweener.Tween(BestTransformWrap, new { X = scoreOnscreenCenter }, 1f).Ease(Ease.QuadOut);
							   Tweener.Tween(NewRecordTransformWrap, new { X = recordOnscreenCenter }, 1f).Ease(Ease.QuadOut);

			                   Tweener.Tween(failColorWrap, new{ A = 0 }, 0.5f).OnComplete(() =>
			                                                                               {
				                                                                               failColorWrap.A = 1f;
				                                                                               fail.renderer.enabled = false;
			                                                                               });

			                   // Move the camera up and show the main menu
			                   ClickAnywhere.text = "(tap anywhere to restart)";
			                   Tweener.Tween(CameraTransformWrap, new{ Y = 1 }, 1f).Ease(Ease.CubeInOut);

							   //TextTitleTransformWrap.X = -(TitleTextTransform.rect.width + UICanvas.pixelRect.width);
							   //TextClickAnywhereTransformWrap.X = -(ClickAnywhere.rectTransform.rect.width + UICanvas.pixelRect.width);
							   //Tweener.Tween(TextTitleTransformWrap, new{ X = UICanvas.pixelRect.width / 2 }, 0.9f).Ease(Ease.QuadOut);
							   //Tweener.Tween(TextClickAnywhereTransformWrap, new{ X = UICanvas.pixelRect.width / 2 }, 0.6f).Ease(Ease.QuadOut).OnComplete(() => state = State.mainMenu);

							   TextTitleTransformWrap.X = titleOffscreenLeft;
							   TextClickAnywhereTransformWrap.X = clickAnywhereOffscreenLeft;
							   Tweener.Tween(TextTitleTransformWrap, new { X = titleOnscreenCenter }, 0.9f).Ease(Ease.QuadOut);
							   Tweener.Tween(TextClickAnywhereTransformWrap, new { X = clickAnywhereOnscreenCenter }, 0.6f).Ease(Ease.QuadOut).OnComplete(() => state = State.mainMenu);
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
