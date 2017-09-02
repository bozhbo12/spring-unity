// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcEffectAniBehaviour : NcEffectBehaviour, IResetAnimation
{
    // Attribute ------------------------------------------------------------------------
    protected NcTimerTool		m_Timer;
	protected	GameObject		m_OnEndAniGameObject	= null;	
	protected	bool			m_bEndAnimation			= false;
	public		string			m_OnEndAniFunction		= "OnEndAnimation";	

	// Property -------------------------------------------------------------------------
	public void SetCallBackEndAnimation(GameObject callBackGameObj)
	{
		m_OnEndAniGameObject	= callBackGameObj;
	}

	public void SetCallBackEndAnimation(GameObject callBackGameObj, string nameFunction)
	{
		m_OnEndAniGameObject	= callBackGameObj;
		m_OnEndAniFunction		= nameFunction;
	}
	public bool IsEndAnimation()
	{
		return m_bEndAnimation;
	}

	protected void InitAnimationTimer()
	{
		if (m_Timer == null)
			m_Timer = new NcTimerTool();
		m_bEndAnimation = false;
		m_Timer.Start();
	}

    bool bParentIsNcDelay = false;
    /// <summary>
    /// 设置父对象是否nc
    /// </summary>
    /// <param name="bValue"></param>
    public void SetParentIsNcDelay(bool bValue)
    {
        bParentIsNcDelay = bValue;
    }

    /// <summary>
    /// 获取父对象是否是nc
    /// </summary>
    /// <returns></returns>
    public bool GetParentIsNcDelay()
    {
        return bParentIsNcDelay;
    }

	public virtual void ResetAnimation()
	{
		m_bEndAnimation = false;
		if (m_Timer != null)
			m_Timer.Reset(0);
	}

	public virtual void PauseAnimation()
	{
		if (m_Timer != null)
			m_Timer.Pause();
	}

	public virtual void ResumeAnimation()
	{
		if (m_Timer != null)
			m_Timer.Resume();
	}

	public virtual void MoveAnimation(float fRate)
	{
		if (m_Timer != null)
			m_Timer.Reset(fRate);
	}

	// Loop Function --------------------------------------------------------------------
	// Control Function -----------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
	protected void OnEndAnimation()
	{
		m_bEndAnimation = true;

//		test code 
// 		Debug.Log("OnEndAnimation");
// 		ResetAnimation();

		if (m_OnEndAniGameObject != null)
			m_OnEndAniGameObject.SendMessage(m_OnEndAniFunction, this, SendMessageOptions.DontRequireReceiver);
	}
}

