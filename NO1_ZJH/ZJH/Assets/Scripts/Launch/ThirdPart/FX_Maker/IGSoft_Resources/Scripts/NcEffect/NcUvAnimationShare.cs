// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class NcUvAnimationShare : NcEffectAniBehaviour
{
    /// <summary>
    /// 启动延迟时间
    /// </summary>
    public float m_delay_timer = 0.0f;
    /// <summary>
    /// 计时器
    /// </summary>
    private float m_timer_count = 0.0f;
	// Attribute ------------------------------------------------------------------------
    public      float       m_fScrollSpeedX     = 0.0f;
	public		float		m_fScrollSpeedY		= 0.0f;

    public      float       m_fTilingSpeedX     = 0.0f;
    public      float       m_fTilingSpeedY     = 0.0f;

	public		float		m_fTilingX			= 1;
	public		float		m_fTilingY			= 1;

	public		float		m_fOffsetX			= 0;
	public		float		m_fOffsetY			= 0;

	public		bool		m_bFixedTileSize	= false;
	public		bool		m_bRepeat			= true;
	public		bool		m_bAutoDestruct		= false;

    protected   Vector3     m_OriginalScale = Vector3.zero;
    protected   Vector2     m_OriginalTiling = Vector2.zero;
    protected   Vector2     m_EndOffset         = Vector2.zero;
    protected   Vector2     m_RepeatOffset      = Vector2.zero;
	protected	Renderer	m_Renderer;

    //保存编译器输入值
    private float m_fTilingX_Inspector = 0f;
    private float m_fTilingY_Inspector = 0f;
    private float m_fOffsetX_Inspector = 0f;
    private float m_fOffsetY_Inspector = 0f;

	// Property -------------------------------------------------------------------------
	public void SetFixedTileSize(bool bFixedTileSize)
	{
		m_bFixedTileSize = bFixedTileSize;
	}

#if UNITY_EDITOR
	public override string CheckProperty()
	{
		if (1 < gameObject.GetComponents(GetType()).Length)
			return "SCRIPT_WARRING_DUPLICATE";
		if (1 < GetEditingUvComponentCount())
			return "SCRIPT_DUPERR_EDITINGUV";
		if (GetComponent<Renderer>() == null || GetComponent<Renderer>().sharedMaterial == null)
			return "SCRIPT_EMPTY_MATERIAL";

		return string.Empty;	// no error
	}
#endif

	public override int GetAnimationState()
	{
		int re;
		if (m_bRepeat == false)
		{
			if (enabled && IsActive(gameObject) && IsEndAnimation() == false)
				re = 1;
			re = 0;
		}
		re = -1;
// 		LogSystem.Log("bNcAni " + re);
		return re;
	}

	public override void ResetAnimation()
	{
		if (enabled == false)
			enabled = true;

        m_fTilingX = m_fTilingX_Inspector;
        m_fTilingY = m_fTilingY_Inspector;
        m_fOffsetX = m_fOffsetX_Inspector;
        m_fOffsetY = m_fOffsetY_Inspector;
		Start();
	}

    void Awake()
    {
        m_fTilingX_Inspector = m_fTilingX;
        m_fTilingY_Inspector = m_fTilingY;
        m_fOffsetX_Inspector = m_fOffsetX;
        m_fOffsetY_Inspector = m_fOffsetY;
    }

	// Loop Function --------------------------------------------------------------------
	void Start()
	{
		m_Renderer = GetComponent<Renderer>();
		if (m_Renderer == null || m_Renderer.sharedMaterial == null || m_Renderer.sharedMaterial.mainTexture == null)
		{
			enabled = false;
		} 
        else
        {
            Vector2 vTemp = Vector2.zero;
            vTemp.x = m_fTilingX;
            vTemp.y = m_fTilingY;
            m_Renderer.sharedMaterial.mainTextureScale = vTemp;

			// 0~1 value
			float offset;
			offset = m_fOffsetX + m_fTilingX;
			m_RepeatOffset.x	= offset - (int)(offset);
			if (m_RepeatOffset.x < 0)
				m_RepeatOffset.x += 1;
			offset = m_fOffsetY + m_fTilingY;
			m_RepeatOffset.y	= offset - (int)(offset);
			if (m_RepeatOffset.y < 0)
				m_RepeatOffset.y += 1;
			m_EndOffset.x = 1 - (m_fTilingX - (int)(m_fTilingX) + ((m_fTilingX - (int)(m_fTilingX)) < 0 ? 1:0));
			m_EndOffset.y = 1 - (m_fTilingY - (int)(m_fTilingY) + ((m_fTilingY - (int)(m_fTilingY)) < 0 ? 1:0));

			InitAnimationTimer();

            m_timer_count = 0;
		}
	}

	void Update()
	{
        if (m_Renderer == null || m_Renderer.sharedMaterial == null || m_Renderer.sharedMaterial.mainTexture == null || m_Timer == null )
			return;

        m_timer_count += m_Timer.GetDeltaTime();
        if (m_timer_count < m_delay_timer) return;

        Vector2 vTemp = Vector2.zero;
		if (m_bFixedTileSize)
		{
            //if (m_fTilingSpeedX != 0 && m_OriginalScale.x != 0)
            //    m_fTilingX = m_OriginalTiling.x * (transform.lossyScale.x / m_OriginalScale.x);
            //if (m_fTilingSpeedY != 0 && m_OriginalScale.y != 0)
            //    m_fTilingY = m_OriginalTiling.y * (transform.lossyScale.y / m_OriginalScale.y);

            m_fTilingX += m_Timer.GetDeltaTime() * m_fTilingSpeedX;
            m_fTilingY += m_Timer.GetDeltaTime() * m_fTilingSpeedY;
            vTemp.x = m_fTilingX;
            vTemp.y = m_fTilingY;
            m_Renderer.sharedMaterial.mainTextureScale = vTemp;
		}

		m_fOffsetX += m_Timer.GetDeltaTime() * m_fScrollSpeedX;
		m_fOffsetY += m_Timer.GetDeltaTime() * m_fScrollSpeedY;

		bool bCallEndAni = false;
		if (m_bRepeat == false)
		{
			m_RepeatOffset.x	+= m_Timer.GetDeltaTime() * m_fScrollSpeedX;
			if (m_RepeatOffset.x < 0 || 1 < m_RepeatOffset.x)
			{
				m_fOffsetX	= m_EndOffset.x;
				enabled		= false;
				bCallEndAni	= true;
			}
			m_RepeatOffset.y += m_Timer.GetDeltaTime() * m_fScrollSpeedY;
			if (m_RepeatOffset.y < 0 || 1 < m_RepeatOffset.y)
			{
				m_fOffsetY	= m_EndOffset.y;
				enabled		= false;
				bCallEndAni	= true;
			}
		}

        vTemp.x = m_fOffsetX;
        vTemp.y = m_fOffsetY;
        m_Renderer.sharedMaterial.mainTextureOffset = vTemp;
		if (bCallEndAni)
		{
			OnEndAnimation();
			if (m_bAutoDestruct)
				DestroyObject(gameObject);
		}
	}

	// Control Function -----------------------------------------------------------------
	// Event Function -------------------------------------------------------------------
	public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
	{
		m_fScrollSpeedX		*= fSpeedRate;
		m_fScrollSpeedY		*= fSpeedRate;
	}

	public override void OnUpdateToolData()
	{
		m_OriginalScale		= transform.lossyScale;
		m_OriginalTiling.x	= m_fTilingX;
		m_OriginalTiling.y	= m_fTilingY;
	}
}

