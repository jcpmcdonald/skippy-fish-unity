using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;


public class TweenTransformWrap
{
	private Transform v3;
	public TweenTransformWrap(Transform v3)
	{
		this.v3 = v3;
	}


	public float X
	{
		get { return v3.position.x; }
		set
		{
			v3.position = new Vector3(value, v3.position.y, v3.position.z);
		}
	}

	public float Y
	{
		get { return v3.position.y; }
		set { v3.position = new Vector3(v3.position.x, value, v3.position.z); }
	}

	public float Z
	{
		get { return v3.position.z; }
		set { v3.position = new Vector3(v3.position.x, v3.position.y, value); }
	}

	public float EulerAnglesZ
	{
		get { return v3.eulerAngles.z; }
		set { v3.eulerAngles = new Vector3(v3.eulerAngles.x, v3.eulerAngles.y, value); }
	}
}
