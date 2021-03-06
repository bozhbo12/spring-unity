//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Tween the object's local scale.
/// </summary>

[AddComponentMenu("NGUI/Tween/Tween Scale")]
public class TweenScale : UITweener
{
	public Vector3 from = Vector3.one;
	public Vector3 to = Vector3.one;
	public bool updateTable = false;

	Transform mTrans;
	UITable mTable;

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

	public Vector3 value { get { return cachedTransform.localScale; } set { cachedTransform.localScale = value; } }

	[System.Obsolete("Use 'value' instead")]
	public Vector3 scale { get { return this.value; } set { this.value = value; } }

	/// <summary>
	/// Tween the value.
	/// </summary>

	protected override void OnUpdate (float factor, bool isFinished)
	{
		value = from * (1f - factor) + to * factor;

		if (updateTable)
		{
			if (mTable == null)
			{
				mTable = NGUITools.FindInParents<UITable>(gameObject);
				if (mTable == null) 
                { 
                    updateTable = false; 
                    return; 
                }
			}
			mTable.repositionNow = true;
		}
	}

	/// <summary>
	/// Start the tweening operation.
	/// </summary>
    static Vector3 ZeroVec3 = Vector3.one* 0.001f;
	static public TweenScale Begin (GameObject go, float duration, Vector3 scale)
	{
       
		TweenScale comp = UITweener.Begin<TweenScale>(go, duration);
		comp.from = comp.value;
		comp.to = scale;
        if (comp.from.Equals(Vector3.zero))
        {
            comp.from = ZeroVec3;
        }
        if (comp.to.Equals(Vector3.zero))
        {
            comp.to = ZeroVec3;
        }
		if (duration <= 0f)
		{
			comp.Sample(1f, true);
			comp.enabled = false;
		}
		return comp;
	}

    static public TweenScale Begin(GameObject go, float duration, Vector3 startScale, Vector3 endScale)
    {

        TweenScale comp = UITweener.Begin<TweenScale>(go, duration);
        comp.from = startScale;
        comp.to = endScale;
        if (comp.from.Equals(Vector3.zero))
        {
            comp.from = ZeroVec3;
        }
        if (comp.to.Equals(Vector3.zero))
        {
            comp.to = ZeroVec3;
        }
        if (duration <= 0f)
        {
            comp.Sample(1f, true);
            comp.enabled = false;
        }
        return comp;
    }

    [ContextMenu("Set 'From' to current value")]
	public override void SetStartToCurrentValue () { from = value; }

	[ContextMenu("Set 'To' to current value")]
	public override void SetEndToCurrentValue () { to = value; }

	[ContextMenu("Assume value of 'From'")]
	void SetCurrentValueToStart () { value = from; }

	[ContextMenu("Assume value of 'To'")]
    void SetCurrentValueToEnd() { value = to; }//
    #region 释放
    //add by chenfei 20150824
    new void OnDestroy()
    {
        mTrans = null;
        mTable = null;
    }
    #endregion;
}
