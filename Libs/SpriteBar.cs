using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBar : MonoBehaviour
{
	public SpriteRenderer fg;
	public SpriteRenderer bg;

	public Vector2 size = new Vector2(32f, 5f);
	public Vector2 bg_scale = new Vector2(4f, 3f);

	[Range(0, 1)]
	public float value = 1f;

	// Use this for initialization
	void Start()
	{

	}

	void OnValidate()
	{
		Reset(size.x, size.y, value);
	}

	public void Set(float v)
	{
		this.value = v;
		if (fg != null)
		{
			fg.size = new Vector2(size.x * v, size.y);
		}
	}

	public void Reset(float sx, float sy, float val)
	{
		this.size = new Vector2(sx, sy);
		if (fg != null)
		{
			fg.drawMode = SpriteDrawMode.Sliced;
			fg.size = this.size;
			fg.transform.localPosition = new Vector3(-this.size.x / 2, 0);
		}

		if (bg != null)
		{
			bg.drawMode = SpriteDrawMode.Sliced;
			bg.size = this.size + bg_scale;
			bg.transform.localPosition = new Vector3(-bg.size.x / 2, 0);
		}

		this.Set(val);
	}
}
