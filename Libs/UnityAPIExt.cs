
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using XLua;

[LuaCallCSharp]
public static class UnityAPIExt
{
	/// <summary>
	/// UnityEngine.Object重载的==操作符，当一个对象被Destroy，未初始化等情况，obj == null返回true，但这C#对象并不为null，可以通过System.Object.ReferenceEquals(null, obj)来验证下。
	/// </summary>
	/// <param name="o"></param>
	/// <returns></returns>
	public static bool is_null(this UnityEngine.Object o)
	{
		return o == null;
	}

	public static GameObject active(this GameObject go, bool active)
	{
		go.SetActive(active);
		return go;
	}

	public static Component active(this Component com, bool active)
	{
		GameObject go = com.gameObject;
		if (go != null)
		{
			go.SetActive(active);
		}
		return com;
	}

	public static Transform mount(this Transform trans, string path, GameObject go)
	{
		return mount(trans, path, go.transform);
	}

	public static Transform mount(this Transform trans, string path, Transform child)
	{
		if (child == null) return trans;

		if (string.IsNullOrEmpty(path))
		{
			trans.add_child(child);
		}
		else
		{
			Transform anchor = trans.Find(path);
			if (anchor != null)
			{
				anchor.add_child(child);
			}
			else
			{
				Debug.LogError("Can't not find path node:" + path);
			}
		}

		return trans;
	}

	public static Transform add_child(this GameObject parent, GameObject child)
	{
		if (child != null)
		{
			return parent.add_child(child.transform);
		}

		return null;
	}

	public static Transform add_child(this GameObject parent, Transform child)
	{
		return parent.transform.add_child(child);
	}

	public static Transform add_child(this Transform parent, GameObject child)
	{
		return parent.add_child(child.transform);
	}

	public static Transform add_child(this Transform parent, Transform child)
	{
		if (child != null)
		{
			child.SetParent(parent, false);
			child.localPosition = Vector3.zero;
			child.localRotation = Quaternion.identity;
			child.localScale = Vector3.one;
		}

		return child;
	}

	/// <summary>
	/// 替换原节点，若原节点存在返回对应Transform
	/// </summary>
	/// <param name="trans"></param>
	/// <param name="path"></param>
	/// <param name="child"></param>
	/// <returns></returns>
	public static Transform replace(this Transform trans, string path, Transform child)
	{
		string parentPath = Path.GetDirectoryName(path);
		string childName = Path.GetFileName(path);
		Transform parent = trans.Find(parentPath);
		Transform oldChild = null;
		if (parent != null)
		{
			oldChild = parent.Find(childName);
			child.name = childName;
			parent.add_child(child);
		}
		return oldChild;
	}

	public static float world_pos(this Transform trans, out float y, out float z)
	{
		var worldPos = trans.position;
		y = worldPos.y;
		z = worldPos.z;
		return worldPos.x;
	}

	public static Transform add_pos(this Transform trans, float x, float y, float z = 0f, bool worldPos = false)
	{
		return trans.add_pos(new Vector3(x, y, z), worldPos);
	}

	public static Transform add_pos(this Transform trans, Vector3 pos, bool worldPos = false)
	{
		if (worldPos)
		{
			trans.position += pos;
		}
		else
		{
			trans.localPosition += pos;
		}

		return trans;
	}

	public static Component ps(this Component com, float x = 0f, float y = 0f, float z = 0, float scale = 1f)
	{
		Transform trans = com as Transform;
		if (trans == null)
		{
			trans = com.transform;
		}
		trans.ps(x, y, z, scale);
		return com;
	}

	public static Transform ps(this Transform trans, float x = 0f, float y = 0f, float z = 0, float scale = 1f)
	{
		trans.localPosition = new Vector3(x, y, z);
		trans.localScale = new Vector3(scale, scale, scale);
		return trans;
	}

	public static Transform sr(this Transform trans, float sx = 1f, float sy = 1f, float sz = 1f)
	{
		trans.localScale = new Vector3(sx, sy, sz);
		return trans;
	}

	public static Transform rot(this Transform trans, float rx = 0f, float ry = 0f, float rz = 0f)
	{
		trans.localEulerAngles = new Vector3(rx, ry, rz);
		return trans;
	}

	public static RectTransform set_size(this RectTransform trans, float w, float h)
	{
		trans.sizeDelta = new Vector2(w, h);
		return trans;
	}

	public static int to_hex(this Color32 color)
	{
		int hex = color.a << 24 | color.r << 16 | color.g << 8 | color.b;
		return hex;
	}

	public static Color itoc(uint hex)
	{
		byte b = (byte)(hex & 255);
		hex = hex >> 8;
		byte g = (byte)(hex & 255);
		hex = hex >> 8;
		byte r = (byte)(hex & 255);
		hex = hex >> 8;

		byte a = byte.MaxValue;
		if (hex > 0)
		{
			a = (byte)(hex & 255);
		}

		return new Color32(r, g, b, a);
	}

	public static Color stoc(string colorStr)
	{
		Color c = Color.white;
		ColorUtility.TryParseHtmlString(colorStr, out c);
		return c;
	}

	/// <summary>
	/// #RGBA,#RGB 示例红色#FF0000,半透红色#FF00007D
	/// </summary>
	/// <param name="com"></param>
	/// <param name="colorStr"></param>
	/// <returns></returns>
	public static Graphic SetColor(this Graphic com, string colorStr)
	{
		com.color = stoc(colorStr);
		return com;
	}

	/// <summary>
	/// 0xARGB 0xRGB
	/// </summary>
	/// <param name="com"></param>
	/// <param name="colorHex"></param>
	/// <returns></returns>
	public static Graphic SetColor(this Graphic com, uint colorHex)
	{
		com.color = itoc(colorHex);
		return com;
	}

	public static Graphic SetAlpha(this Graphic com, float alpha)
	{
		var rawColor = com.color;
		rawColor.a = alpha;
		com.color = rawColor;
		return com;
	}

	public static CanvasGroup SetAlpha(this CanvasGroup group, float alpha)
	{
		group.alpha = alpha;
		return group;
	}

	public static SpriteRenderer SetColor(this SpriteRenderer com, string colorStr)
	{
		com.color = stoc(colorStr);
		return com;
	}

	public static SpriteRenderer SetColor(this SpriteRenderer com, uint colorHex)
	{
		com.color = itoc(colorHex);
		return com;
	}

	public static SpriteRenderer SetAlpha(this SpriteRenderer com, float alpha)
	{
		var rawColor = com.color;
		rawColor.a = alpha;
		com.color = rawColor;
		return com;
	}

	public static Vector2 to_v2(this Vector3 v3)
	{
		return v3;
	}

	public static Vector3 to_v3(this Vector2 v2)
	{
		return v2;
	}

	public static void AddListener(this EventTrigger trigger, EventTriggerType type, UnityAction<BaseEventData> callback)
	{
		EventTrigger.Entry entry = null;
		foreach (var e in trigger.triggers)
		{
			if (e.eventID == type)
			{
				entry = e;
				break;
			}
		}

		if (entry == null)
		{
			entry = new EventTrigger.Entry { eventID = type };
			trigger.triggers.Add(entry);
		}
		entry.callback.AddListener(callback);
	}
}
