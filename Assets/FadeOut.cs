using UnityEngine;
using System.Collections;

public class FadeOut : MonoBehaviour
{

	public float rate;
	public GUIText text;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (text.color.a > 0)
		{
			text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (rate * Time.deltaTime));
		}
	}
}
