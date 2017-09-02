//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright 漏 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Similar to UIButtonColor, but adds a 'disabled' state based on whether the collider is enabled or not.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	
    /// <summary>当前按钮的Label </summary>
    public UILabel mBtnLabel;                                                    //按钮Label
    protected UILabel.Effect mPrevLabelEffect=UILabel.Effect.None;               //保存之前的Label描边效果（也可能没有描边效果）
    protected Color mPrevLabelColor=new Color(1,1,1);                            //保存之前的Label描边颜色

    /// <summary>
    /// Current button that sent out the onClick event.
    /// </summary>
	static public UIButton current;
	/// <summary>
	/// Whether the button will highlight when you drag something over it.
	/// </summary>
	public bool dragHighlight = false;

	/// <summary>
	/// Name of the hover state sprite.
	/// </summary>

	public string hoverSprite;

	/// <summary>
	/// Name of the pressed sprite.
	/// </summary>

	public string pressedSprite;

	/// <summary>
	/// Name of the disabled sprite.
	/// </summary>

	public string disabledSprite;

	/// <summary>
	/// Whether the sprite changes will elicit a call to MakePixelPerfect() or not.
	/// </summary>

	public bool pixelSnap = false;

	/// <summary>
	/// Click event listener.
	/// </summary>

	public List<EventDelegate> onClick = new List<EventDelegate>();

	// Cached value
	string mNormalSprite;
	UISprite mSprite;

	/// <summary>
	/// Whether the button should be enabled.
	/// </summary>

	public override bool isEnabled
	{
		get
		{
			if (!enabled) return false;
			Collider col = GetComponent<Collider>();
			return col && col.enabled;
		}
		set
		{
			Collider col = GetComponent<Collider>();

			if (col != null)
			{
				col.enabled = value;
                if (value)
                {
                    SeteEnableBtnLabel();
                }
                else
                {
                    SetDisabledBtnLabel();
                }

				SetState(value ? State.Normal : State.Disabled, false);
			}
			else enabled = value;
		}
	}

	/// <summary>
	/// Convenience function that changes the normal sprite.
	/// </summary>

	public string normalSprite
	{
		get
		{
			if (!mInitDone) OnInit();
			return mNormalSprite;
		}
		set
		{
			mNormalSprite = value;
			if (mState == State.Normal) SetSprite(value);
		}
	}

	/// <summary>
	/// Cache the sprite we'll be working with.
	/// </summary>

	protected override void OnInit ()
	{
		base.OnInit();
		mSprite = (mWidget as UISprite);
		if (mSprite != null) mNormalSprite = mSprite.spriteName;

        FindBtnLabel();
	}

    /// <summary>
    /// 查找UIButton下的Label
    /// </summary>
    private void FindBtnLabel()
    {
        if (mBtnLabel == null)             //编辑器中赋值失败
        {
            UILabel[] lblArray = gameObject.GetComponentsInChildren<UILabel>();

            if (lblArray.Length == 0)
            {
                //LogSystem.LogError("------Find BtnLabel is Null!");
                mBtnLabel = null;
            }
            else
            {
                //LogSystem.LogError("------Find BtnLabel is  YES!");
                mBtnLabel = lblArray[0];
                mPrevLabelEffect=lblArray[0].effectStyle;                         //保存初始化时的BtnLabel的描边样式
                mPrevLabelColor = lblArray[0].effectColor;                        //保存初始化时的BtnLabel的描边颜色
            }
        }
        else                               //编辑器中赋值成功
        {
            //LogSystem.LogError("------Find BtnLabel is  YES!");
            mPrevLabelEffect = mBtnLabel.effectStyle;                             //保存初始化时的BtnLabel的描边样式
            mPrevLabelColor = mBtnLabel.effectColor;                              //保存初始化时的BtnLabel的描边颜色
        }
    }

    /// <summary>
    /// 设置激活状态时的Label样式
    /// </summary>
    private void SeteEnableBtnLabel()
    {
        if (mBtnLabel != null)
        {
            mBtnLabel.effectStyle =mPrevLabelEffect;
            mBtnLabel.effectColor =mPrevLabelColor;
        }
    }

    /// <summary>
    /// 设置禁用状态时的Label样式
    /// </summary>
    private void SetDisabledBtnLabel()
    {
        if (mBtnLabel!=null)
        {
            mBtnLabel.effectStyle = UILabel.Effect.Outline;
            mBtnLabel.effectColor = new Color(76/255.0f,76/255.0f,76/255.0f);
        }
    }

	/// <summary>
	/// Set the initial state.
	/// </summary>
	protected override void OnEnable ()
	{
        if (isEnabled)
        {
            if (mInitDone)
            {
                if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
                {
                    OnHover(UICamera.selectedObject == gameObject);
                }
                else if (UICamera.currentScheme == UICamera.ControlScheme.Mouse)
                {
                    OnHover(UICamera.hoveredObject == gameObject);
                }
                else
                {
                    SetState(State.Normal, false);
                }
            }
        }
        else
        {
            SetState(State.Disabled, true);
        }
	}

	/// <summary>
	/// Drag over state logic is a bit different for the button.
	/// </summary>
	
	protected override void OnDragOver ()
	{
		if (isEnabled && (dragHighlight || UICamera.currentTouch.pressed == gameObject))
			base.OnDragOver();
	}

	/// <summary>
	/// Drag out state logic is a bit different for the button.
	/// </summary>
	
	protected override void OnDragOut ()
	{
		if (isEnabled && (dragHighlight || UICamera.currentTouch.pressed == gameObject))
			base.OnDragOut();
	}

	/// <summary>
	/// Call the listener function.
	/// </summary>

	protected virtual void OnClick ()
	{
		if (isEnabled)
		{
			current = this;
			EventDelegate.Execute(onClick);
			current = null;
		}
	}

	/// <summary>
	/// Change the visual state.
	/// </summary>

	protected override void SetState (State state, bool immediate)
	{
		base.SetState(state, immediate);

		switch (state)
		{
			case State.Normal:
                SetSprite(mNormalSprite); 
                break;
			case State.Hover: 
                SetSprite(hoverSprite); 
                break;
			case State.Pressed: 
                SetSprite(pressedSprite); 
                break;
			case State.Disabled:
                SetSprite(disabledSprite); 
                break;
		}
	}

	/// <summary>
	/// Convenience function that changes the sprite.
	/// </summary>

	protected void SetSprite (string sp)
	{
		if (mSprite != null && !string.IsNullOrEmpty(sp) && mSprite.spriteName != sp)
		{
			mSprite.spriteName = sp;
			if (pixelSnap) mSprite.MakePixelPerfect();
		}
	}
}
