using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;


public class TweenColorWrap
{
	private SpriteRenderer c;
	public TweenColorWrap(SpriteRenderer c)
	{
		this.c = c;
	}


	public float R
	{
		get { return c.color.r; }
		set
		{
			c.color = new Color(value, c.color.g, c.color.b, c.color.a);
		}
	}

	public float G
	{
		get { return c.color.g; }
		set
		{
			c.color = new Color(c.color.r, value, c.color.b, c.color.a);
		}
	}
	public float B
	{
		get { return c.color.b; }
		set
		{
			c.color = new Color(c.color.r, c.color.g, value, c.color.a);
		}
	}
	public float A
	{
		get { return c.color.a; }
		set
		{
			c.color = new Color(c.color.r, c.color.g, c.color.b, value);
		}
	}
}
