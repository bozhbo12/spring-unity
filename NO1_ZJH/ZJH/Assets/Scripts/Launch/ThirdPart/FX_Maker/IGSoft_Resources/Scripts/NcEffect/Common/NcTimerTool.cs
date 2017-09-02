using UnityEngine;
using System.Collections;

// frame timer
// 일정 시간 후 알림
// 일정 시간 후 반복 알림
// 일정 시간 후 반복횟수만큼 알림

public class NcTimerTool
{
	// Attribute ------------------------------------------------------------------------
	protected	bool		m_bEnable;
	private		float		m_fStartTime;
	private		float		m_fPauseTime;

#if EDITOR_EFFECT // 功能： 编辑器模式下播放特效。
    public static bool hasEditorPlayFun;
    public static System.Diagnostics.Stopwatch sw;
    public static double deltaTimeInEditor;
    public static double timeSinceStarupInEditor;
    public static double timeInEditor;

    public static void Update()
    {
        deltaTimeInEditor = (float)sw.ElapsedMilliseconds / 1000.0f - timeSinceStarupInEditor;
        timeSinceStarupInEditor = (float)sw.ElapsedMilliseconds / 1000.0f;
        timeInEditor += deltaTimeInEditor;
    }
#endif

    // Property -------------------------------------------------------------------------
    public static float GetEngineTime()
    {
        float time = 0;

#if EDITOR_EFFECT // 功能： 编辑器模式下播放特效。
        if (Application.isPlaying)
        {
            if (Time.time == 0)
                time = 0.000001f;
            else
                time = Time.time;
        }
        else
        {
            if (timeInEditor == 0)
                time = 0.000001f;
            else
                time = (float)timeInEditor;
        }

#else
        if (Time.time == 0)
			time = 0.000001f;
        else
            time = (float)Time.time;
#endif
        return time;
	}

	public float GetTime()
	{
		float fEngineTime = NcTimerTool.GetEngineTime();
		if (m_bEnable == false && m_fPauseTime != fEngineTime)
		{
			m_fStartTime	+= NcTimerTool.GetEngineTime() - m_fPauseTime;
			m_fPauseTime	= fEngineTime;
		}
		return NcTimerTool.GetEngineTime() - m_fStartTime;
	}

	public float GetDeltaTime()
	{
	    if (m_bEnable)
	    {
#if EDITOR_EFFECT // 功能： 编辑器模式下播放特效。
            if (Application.isPlaying)
            {
                return Time.deltaTime;
            }
            else
            {
                return (float)deltaTimeInEditor;
            }
#else
            return Time.deltaTime;
#endif
        }
        return 0;
	}

	public bool IsEnable()
	{
		return m_bEnable;
	}

	public void Start()
	{
		m_bEnable		= true;
		m_fStartTime	= GetEngineTime() - 0.000001f;
	}

	public void Reset(float fAdjustTime)
	{
		m_fStartTime	= GetEngineTime() - fAdjustTime;
	}

	public void Pause()
	{
		m_bEnable		= false;
		m_fPauseTime	= NcTimerTool.GetEngineTime();
	}

	public void Resume()
	{
		GetTime();
		m_bEnable = true;
	}

	// Control Function -----------------------------------------------------------------
}
