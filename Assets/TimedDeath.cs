using UnityEngine;
using System.Collections;

public class TimedDeath : MonoBehaviour
{

	public float delay;
	private float duration;

	// Use this for initialization
	void Start () {
		Destroy(gameObject, delay);
	}
	
	// Update is called once per frame
	void Update ()
	{
		//duration += Time.deltaTime;
		//if (duration > delay)
		//{
		//	Destroy(this);
		//}
	}
}
