//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// This class is meant to be used only internally. It's like LogSystem.Log, but prints using OnGUI to screen instead.
/// </summary>

[AddComponentMenu("NGUI/Internal/Debug")]
public class NGUIDebug : MonoBehaviour
{
    /// <summary>
    /// 0不开打印 1开打印
    /// </summary>
    public static bool bOpenLog = true;
	public static bool mRayDebug = false;
	public static List<string> mLines = new List<string>();
	static NGUIDebug mInstance = null;

	/// <summary>
	/// Set by UICamera. Can be used to show/hide raycast information.
	/// </summary>

	static public bool debugRaycast
	{
		get
		{
			return mRayDebug;
		}
		set
		{
			if (Application.isPlaying)
			{
				mRayDebug = value;
				//if (value) CreateInstance();
			}
		}
	}

	/// <summary>
	/// Ensure we have an instance present.
	/// </summary>

	static public void CreateInstance ()
	{
		if (mInstance == null)
		{
			GameObject go = new GameObject("_NGUI Debug");
			mInstance = go.AddComponent<NGUIDebug>();
			DontDestroyOnLoad(go);
		}
	}

	/// <summary>
	/// Add a new on-screen log entry.
	/// </summary>

	static void LogString (string text)
	{
        if (!bOpenLog)
        {
            return;
        }
        if (mLines.Count > 20) mLines.RemoveAt(0);
        mLines.Add(text);
        CreateInstance();
	}

	/// <summary>
	/// Add a new log entry, printing all of the specified parameters.
	/// </summary>

	static public void Log (params object[] objs)
	{
        if (!bOpenLog)
	    {
            return;
	    }

		string text = string.Empty;

		for (int i = 0; i < objs.Length; ++i)
		{
            if (objs[i] == null)
            {
                continue;
            }
			if (i == 0)
			{
				text += objs[i].ToString();
			}
			else
			{
				text += ", " + objs[i].ToString();
			}
		}
		LogString(text);
	}

	/// <summary>
	/// Draw bounds immediately. Won't be remembered for the next frame.
	/// </summary>

	static public void DrawBounds (Bounds b)
	{
		Vector3 c = b.center;
		Vector3 v0 = b.center - b.extents;
		Vector3 v1 = b.center + b.extents;
		Debug.DrawLine(new Vector3(v0.x, v0.y, c.z), new Vector3(v1.x, v0.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v0.x, v0.y, c.z), new Vector3(v0.x, v1.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v1.x, v0.y, c.z), new Vector3(v1.x, v1.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v0.x, v1.y, c.z), new Vector3(v1.x, v1.y, c.z), Color.red);
	}

    /// <summary>
    /// 样式
    /// </summary>
    private GUIStyle style = null;

    /// <summary>
    /// 字体大小
    /// </summary>
    private int miFontSize = 12;

    private Vector2 scrollPosition = Vector2.zero;

    private Rect posRect = new Rect(0, 0, 800, 500);

    private Rect viewRect = new Rect(0, 0, 900, 600);

    private bool bDisplayLog = true;
    void Start()
    {
        if (Config.bEditor || Config.bWin)
        {
            miFontSize = 12;
        }
        else
        {
            miFontSize = 26;
        }
        style = new GUIStyle();
        style.fontSize = miFontSize;
        style.normal.textColor = Color.red;
    }

    void OnGUI()
    {
        if (!bOpenLog)
            return;

        if (GUI.Button(new Rect(Screen.width - 150, 0, 100, 30), (!bDisplayLog).ToString()))
        {
            bDisplayLog = !bDisplayLog;
        }

        if (!bDisplayLog)
            return;

        if (GUI.Button(new Rect(Screen.width - 150, 50, 100, 30), "CLEAR"))
        {
            mLines.Clear();
        }
        if (GUI.Button(new Rect(Screen.width - 150, 100, 100, 30), "+"))
        {
            miFontSize += 2;
            style.fontSize = miFontSize;
        }
        if (GUI.Button(new Rect(Screen.width - 150, 150, 100, 30), "-"))
        {
            miFontSize -= 2;
            style.fontSize = miFontSize;
        }

        scrollPosition = GUI.BeginScrollView(posRect, scrollPosition, viewRect);
        if (mLines.Count == 0)
        {
            if (mRayDebug && UICamera.hoveredObject != null && Application.isPlaying)
            {
                GUILayout.Label("Last Hit: " + NGUITools.GetHierarchy(UICamera.hoveredObject).Replace("\"", string.Empty));
            }
        }
        else
        {
            for (int i = 0, imax = mLines.Count; i < imax; ++i)
            {
                GUILayout.Label(mLines[i], style);
            }
        }
        GUI.EndScrollView();
        
    }
}
