using System.Net.Mime;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour {
	private bool enabled1;


	public bool defaultState = true;

	public Image imageToTint;
	public Color enabledColor;
	public Color disabledColor;

	public Display.DisplaysUpdatedDelegate asdf;


	// Use this for initialization
	void Start ()
	{
		Enabled = defaultState;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public bool Enabled
	{
		get
		{
			return enabled1;
		}
		set
		{
			imageToTint.CrossFadeColor(value ? enabledColor : disabledColor, 0.2f, false, false);
			enabled1 = value;
		}
	}
}
