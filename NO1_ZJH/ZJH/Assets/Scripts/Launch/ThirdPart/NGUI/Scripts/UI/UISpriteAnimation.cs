//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Very simple sprite animation. Attach to a sprite and specify a common prefix such as "idle" and it will cycle through them.
/// </summary>

///zhangrj
///修改时间 20160806
///修改原因：原先是直接Atlas或者UIFont直接保存的，现在是保存name然后load进来的
///                     所以外部调用的时候Atlas和Font一开始的时候会为空
[ExecuteInEditMode]
[RequireComponent(typeof(UISprite))]
[AddComponentMenu("NGUI/UI/Sprite Animation")]
public class UISpriteAnimation : MonoBehaviour
{
	[HideInInspector][SerializeField] int mFPS = 30;
	[HideInInspector][SerializeField] string mPrefix = string.Empty;
	[HideInInspector][SerializeField] bool mLoop = true;

	UISprite mSprite;
	float mDelta = 0f;
	int mIndex = 0;
	bool mActive = true;
	List<string> mSpriteNames = new List<string>();


    /// <summary>
    /// 增加循环次数;
    /// chenjunyi;
    /// </summary>
    private int loopTime = 1;
    //循环计数器;
    private int loopCounter;

    /// <summary>
    /// 增加事件回调
    /// </summary>
    public OnActionCallback m_callBack;
    /// <summary>
    /// 是否完美化
    /// </summary>
    public bool perfect = true;

	/// <summary>
	/// Number of frames in the animation.
	/// </summary>

	public int frames { get { return mSpriteNames.Count; } }

	/// <summary>
	/// Animation framerate.
	/// </summary>

	public int framesPerSecond { get { return mFPS; } set { mFPS = value; } }

	/// <summary>
	/// Set the name prefix used to filter sprites from the atlas.
	/// </summary>

	public string namePrefix { get { return mPrefix; } set { if (mPrefix != value) { mPrefix = value; RebuildSpriteList(); } } }

	/// <summary>
	/// Set the animation to be looping or not
	/// </summary>

	public bool loop { get { return mLoop; } set { mLoop = value; } }

	/// <summary>
	/// Returns is the animation is still playing or not
	/// </summary>

	public bool isPlaying { get { return mActive; } }

	/// <summary>
	/// Rebuild the sprite list first thing.
	/// </summary>

	void Start ()  { RebuildSpriteList(); }

    void OnDestroy()
    {
        if (mSprite != null)
            mSprite.onMarkAsChangedEvent -= OnLoadAtlasComplete;
    }

	/// <summary>
	/// Advance the sprite animation process.
	/// </summary>

	void Update ()
	{
		if (mActive && mSpriteNames.Count > 1 && Application.isPlaying && mFPS > 0f)
		{
			mDelta += RealTime.deltaTime;
			float rate = 1f / mFPS;

			if (rate < mDelta)
			{
				mDelta = (rate > 0f) ? mDelta - rate : 0f;
				if (++mIndex >= mSpriteNames.Count)
				{
                    mIndex = 0;
                    if (!loop)
                    {
                        loopCounter++;
                        if (loopCounter >= loopTime)//超过规定的循环播放次数;
                        {
                            mActive = false;
                            if (m_callBack != null)
                                m_callBack();
                        }
                    }
                    else
                    {
                        mActive = true;
                    }
				}

				if (mActive)
				{
					mSprite.spriteName = mSpriteNames[mIndex];
                    if (perfect)
                        mSprite.MakePixelPerfect();
				}
			}
		}
	}

	/// <summary>
	/// Rebuild the sprite list after changing the sprite name.
	/// </summary>

	void RebuildSpriteList ()
	{
		if (mSprite == null) mSprite = GetComponent<UISprite>();
		
        if (!GetSpriteName())
        {
            mSprite.onMarkAsChangedEvent += OnLoadAtlasComplete;
        }
	}

    void OnLoadAtlasComplete()
    {
        GetSpriteName();
    }

    bool GetSpriteName()
    {
        if (mSprite != null)
        {
            if (mSprite.atlas != null)
            {
                List<UISpriteData> sprites = mSprite.atlas.spriteList;
                mSpriteNames.Clear();
                for (int i = 0, imax = sprites.Count; i < imax; ++i)
                {
                    UISpriteData sprite = sprites[i];

                    if (string.IsNullOrEmpty(mPrefix) || sprite.name.StartsWith(mPrefix))
                    {
                        mSpriteNames.Add(sprite.name);
                    }
                }
                mSpriteNames.Sort();
            }
            else
                return false;
        }
        return true;
    }
	
	/// <summary>
	/// Reset the animation to frame 0 and activate it.
	/// </summary>
	
	public void Reset(int loopTime = 1)
	{
        //循环次数计数器初始化;
        this.loopCounter = 0;
        this.loopTime = loopTime;

		mActive = true;
		mIndex = 0;

		if (mSprite != null && mSpriteNames.Count > 0)
		{
			mSprite.spriteName = mSpriteNames[mIndex];
            if (perfect)
                mSprite.MakePixelPerfect();
		}
	}

    /// <summary>
    /// 根据当前设置的loopTime状态重新播放;
    /// chenjunyi;
    /// </summary>
    public void Replay()
    {
        this.loopCounter = 0;
        mActive = true;
        mIndex = 0;

        if (mSprite != null && mSpriteNames.Count > 0)
        {
            mSprite.spriteName = mSpriteNames[mIndex];
            if (perfect)
                mSprite.MakePixelPerfect();
        }
    }
}
