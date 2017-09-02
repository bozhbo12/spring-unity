using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 自定义缓动基类
/// </summary>
public abstract class CTweener:CacheObject
{
    public delegate void Callback();

    public enum Method
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        BounceIn,
        BounceOut,
    }

    public enum Style
    {
        Once,
        Loop,
        PingPong,
    }

    public bool steeperCurves = false;
    public Method method = Method.Linear;

    public Style style = Style.Once;

    public float delay = 0f;

    public float duration = 1f;

    public EventDelegate.Callback callback = null;

    public bool finishCallDone = false;
    public EventDelegate.Callback removeCallback = null;

    public delegate void UpdateCallBack(float factor);
    public UpdateCallBack updateCallback = null;

    public Transform mTrans;

    bool mStarted = false;
    float mStartTime = 0f;
    float mDuration = 0f;
    float mAmountPerDelta = 1000f;
    float mFactor = 0f;

    public bool enabled = true;

    public float Factor
    {
        get
        {
            return mFactor;
        }
    }

    /// <summary>
    /// Amount advanced per delta time.
    /// </summary>
    public float amountPerDelta
    {
        get
        {
            if (mDuration != duration)
            {
                mDuration = duration;
                mAmountPerDelta = Mathf.Abs((duration > 0f) ? 1f / duration : 1000f);
            }
            return mAmountPerDelta;
        }
    }

    public void Init()
    {
        mStarted = false;
        mStartTime = 0f;
        mDuration = 0f;
        mAmountPerDelta = 1000f;
        mFactor = 0f;
        updateCallback = null;
        removeCallback = null;
        finishCallDone = false;
    }

    /// <summary>
    /// 加入回收站调用
    /// </summary>
    public override void OnDespawn()
    {
        if (mTrans != null)
            mTrans = null;
        callback = null;
    }
    /// <summary>
    /// 从回收站取出时调用
    /// </summary>
    public override void OnSpawn()
    {
        if (mTrans != null)
            mTrans = null;
        callback = null;
        Init();
    }

    /// <summary>
    /// Update the tweening factor and call the virtual update function.
    /// </summary>
    public void Update()
    {
        if (mTrans == null) return;

        if (!enabled) return;

        float delta = Time.deltaTime;
        float time = Time.time;

        if (!mStarted)
        {
            mStarted = true;
            mStartTime = time + delay;
        }

        if (time < mStartTime)
            return;

        // Advance the sampling factor
        mFactor += amountPerDelta * delta;

        // Loop style simply resets the play factor after it exceeds 1.
        if (style == Style.Loop)
        {
            if (mFactor > 1f)
            {
                mFactor -= Mathf.Floor(mFactor);
            }
        }
        else if (style == Style.PingPong)
        {
            // Ping-pong style reverses the direction
            if (mFactor > 1f)
            {
                mFactor = 1f - (mFactor - Mathf.Floor(mFactor));
                mAmountPerDelta = -mAmountPerDelta;
            }
            else if (mFactor < 0f)
            {
                mFactor = -mFactor;
                mFactor -= Mathf.Floor(mFactor);
                mAmountPerDelta = -mAmountPerDelta;
            }
        }

        

        // If the factor goes out of range and this is a one-time tweening operation, disable the script
        if ((style == Style.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f))
        {
            mFactor = Mathf.Clamp01(mFactor);
            Sample(mFactor, true);

            // Disable this script unless the function calls above changed something
            if (duration == 0f || (mFactor == 1f && mAmountPerDelta > 0f || mFactor == 0f && mAmountPerDelta < 0f))
                enabled = false;

            if (callback != null && SafeHelper.IsTargetValid(callback.Target))
            {
                callback();
                finishCallDone = true;
            }
        }
        else Sample(mFactor, false);        
    }


    /// <summary>
    /// Sample the tween at the specified factor.
    /// </summary>

    public void Sample(float factor, bool isFinished)
    {
        // Calculate the sampling value
        float val = Mathf.Clamp01(factor);        

        if (method == Method.EaseIn)
        {
            val = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - val));
            if (steeperCurves) val *= val;
        }
        else if (method == Method.EaseOut)
        {
            val = Mathf.Sin(0.5f * Mathf.PI * val);

            if (steeperCurves)
            {
                val = 1f - val;
                val = 1f - val * val;
            }
        }
        else if (method == Method.EaseInOut)
        {
            const float pi2 = Mathf.PI * 2f;
            val = val - Mathf.Sin(val * pi2) / pi2;

            if (steeperCurves)
            {
                val = val * 2f - 1f;
                float sign = Mathf.Sign(val);
                val = 1f - Mathf.Abs(val);
                val = 1f - val * val;
                val = sign * val * 0.5f + 0.5f;
            }
        }
        else if (method == Method.BounceIn)
        {
            val = BounceLogic(val);
        }
        else if (method == Method.BounceOut)
        {
            val = 1f - BounceLogic(1f - val);
        }
        if (updateCallback != null)
            updateCallback(val);
        // Call the virtual update
        OnUpdate(val, isFinished);
    }

    /// <summary>
    /// Main Bounce logic to simplify the Sample function
    /// </summary>
    float BounceLogic(float val)
    {
        if (val < 0.363636f) // 0.363636 = (1/ 2.75)
        {
            val = 7.5685f * val * val;
        }
        else if (val < 0.727272f) // 0.727272 = (2 / 2.75)
        {
            val = 7.5625f * (val -= 0.545454f) * val + 0.75f; // 0.545454f = (1.5 / 2.75) 
        }
        else if (val < 0.909090f) // 0.909090 = (2.5 / 2.75) 
        {
            val = 7.5625f * (val -= 0.818181f) * val + 0.9375f; // 0.818181 = (2.25 / 2.75) 
        }
        else
        {
            val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f; // 0.9545454 = (2.625 / 2.75) 
        }
        return val;
    }

    public void Play() { Play(true); }

    public void PlayForward() { Play(true); }

    public void PlayReverse() { Play(false); }

    public void Play(bool forward)
    {
        Init();
        mAmountPerDelta = Mathf.Abs(amountPerDelta);
        if (!forward) mAmountPerDelta = -mAmountPerDelta;
        enabled = true;
        Update();
    }

    abstract protected void OnUpdate(float factor, bool isFinished);
}

/// <summary>
/// 位子移动
/// </summary>
public class CTweenPosition : CTweener
{

    public Vector3 from;
    public Vector3 to;

    public bool worldSpace = false;

    UIRect mRect;

    public UIRect cachedRect
    {
        get
        {
            if (mTrans == null) return null;
            if (mRect == null)
            {
                mRect = mTrans.GetComponent<UIRect>();
                if (mRect == null) mRect = mTrans.GetComponentInChildren<UIRect>();
            }
            return mRect;
        }
    }

    public Transform cachedTransform { get { return mTrans; } }

    /// <summary>
    /// Tween's current value.
    /// </summary>

    public Vector3 value
    {
        get
        {
            if (cachedTransform == null) return Vector3.zero;
            return worldSpace ? cachedTransform.position : cachedTransform.localPosition;
        }
        set
        {
            if (cachedTransform == null) return;
            if (cachedRect == null || !cachedRect.isAnchored || worldSpace)
            {
                if (worldSpace) cachedTransform.position = value;
                else cachedTransform.localPosition = value;
            }
            else
            {
                value -= cachedTransform.localPosition;
                if(cachedRect != null)
                    NGUIMath.MoveRect(cachedRect, value.x, value.y);
            }
        }
    }


    /// <summary>
    /// Tween the value.
    /// </summary>
    protected override void OnUpdate(float factor, bool isFinished) { value = from * (1f - factor) + to * factor; }

    public override void OnDespawn()
    {
        mRect = null;
        base.OnDespawn();
    }

    public override void OnSpawn()
    {
        mRect = null;
        base.OnSpawn();
    }
}


public class CTweenAlpha : CTweener
{

    public float from = 0;
	public float to = 1f;


	UIRect mRect;

	public UIRect cachedRect
	{
		get
		{
            if (mTrans  == null) return null;
			if (mRect == null)
			{
                mRect = mTrans.GetComponent<UIRect>();
                if (mRect == null) mRect = mTrans.GetComponentInChildren<UIRect>();
			}
			return mRect;
		}
	}


	public float value 
    { 
        get
        {
            if (cachedRect == null) return 0;
            return  cachedRect.alpha; 
        } 
        set
        {
            if (cachedRect == null) return;
            cachedRect.alpha = value;
        } 
    }


	protected override void OnUpdate (float factor, bool isFinished) { value = Mathf.Lerp(from, to, factor); }

    public override void OnDespawn()
    {
        mRect = null;
        base.OnDespawn();
    }

    public override void OnSpawn()
    {
        mRect = null;
        base.OnSpawn();
    }
}

public class CTweenColor : CTweener
{

    public Color from = Color.white;
    public Color to = Color.white;

    bool mCached = false;
    UIWidget mWidget;
    Material mMat;
    Light mLight;

    void Cache()
    {
        mCached = true;
        if (mTrans == null) return;
        mWidget = mTrans.GetComponent<UIWidget>();
        Renderer ren = mTrans.GetComponent<Renderer>();
        if (ren != null) mMat = ren.material;
        mLight = mTrans.GetComponent<Light>();
        if (mWidget == null && mMat == null && mLight == null)
            mWidget = mTrans.GetComponentInChildren<UIWidget>();
    }


    /// <summary>
    /// Tween's current value.
    /// </summary>

    public Color value
    {
        get
        {
            if (!mCached) Cache();
            if (mWidget != null) return mWidget.color;
            if (mLight != null) return mLight.color;
            if (mMat != null) return mMat.color;
            return Color.black;
        }
        set
        {
            if (!mCached) Cache();
            if (mWidget != null) mWidget.color = value;
            if (mMat != null) mMat.color = value;

            if (mLight != null)
            {
                mLight.color = value;
                mLight.enabled = (value.r + value.g + value.b) > 0.01f;
            }
        }
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished) { value = Color.Lerp(from, to, factor); }

    public override void OnDespawn()
    {
        mWidget = null;
        mMat = null;
        mLight = null;
        base.OnDespawn();
    }

    public override void OnSpawn()
    {
        mWidget = null;
        mMat = null;
        mLight = null;
        base.OnSpawn();
    }
}

public class CTweenScale : CTweener
{

    public Vector3 from = Vector3.one;
    public Vector3 to = Vector3.one;

    public Transform cachedTransform { get { return mTrans; } }

    public Vector3 value 
    { 
        get
        {
            if (mTrans == null) return Vector3.zero;
            return cachedTransform.localScale; 
        } 
        set 
        {
            if (mTrans == null) return;
            cachedTransform.localScale = value;
        } 
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished)
    {
        value = from * (1f - factor) + to * factor;
    }
}

public class CTweenRotation : CTweener
{
    public Vector3 from;
    public Vector3 to;

    public Transform cachedTransform { get { return mTrans; } }


    /// <summary>
    /// Tween's current value.
    /// </summary>

    public Quaternion value
    { 
        get 
        {
            if (mTrans == null) return Quaternion.Euler(Vector3.zero);
            return cachedTransform.localRotation; 
        } 
        set 
        {
            if (mTrans == null) return;
            cachedTransform.localRotation = value;
        } 
    }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished)
    {
        Vector3 vTemp = Vector3.zero;
        vTemp.x = Mathf.Lerp(from.x, to.x, factor);
        vTemp.y = Mathf.Lerp(from.y, to.y, factor);
        vTemp.z = Mathf.Lerp(from.z, to.z, factor);
        value = Quaternion.Euler(vTemp);
    }
}

public class CTweenWidth : CTweener
{
    public int from = 100;
    public int to = 100;

    UIWidget mWidget;


    public UIWidget cachedWidget
    { 
        get
        {
            if (mTrans == null) return null;
            if (mWidget == null)
                mWidget = mTrans.GetComponent<UIWidget>(); 
            return mWidget; 
        } 
    }


    public int value
    {
        get
        {
            if (cachedWidget == null) return 0;
            return cachedWidget.width;
        } 
        set
        {
            if (cachedWidget == null) return;
            cachedWidget.width = value; 
        } 
    }


    protected override void OnUpdate(float factor, bool isFinished)
    {
        value = Mathf.RoundToInt(from * (1f - factor) + to * factor);
    }

    public override void OnDespawn()
    {
        mWidget = null;
        base.OnDespawn();
    }

    public override void OnSpawn()
    {
        mWidget = null;
        base.OnSpawn();
    }
}

public class CTweenHeight :CTweener
{
    public int from = 100;
    public int to = 100;

    UIWidget mWidget;


    public UIWidget cachedWidget
    {
        get
        {
            if (mTrans == null) return null;
            if (mWidget == null)
                mWidget = mTrans.GetComponent<UIWidget>();
            return mWidget;
        }
    }


    public int value
    {
        get
        {
            if (cachedWidget == null) return 0;
            return cachedWidget.height;
        }
        set
        {
            if (cachedWidget == null) return;
            cachedWidget.height = value;
        }
    }

    protected override void OnUpdate(float factor, bool isFinished)
    {
        value = Mathf.RoundToInt(from * (1f - factor) + to * factor);
    }

    public override void OnDespawn()
    {
        mWidget = null;
        base.OnDespawn();
    }

    public override void OnSpawn()
    {
        mWidget = null;
        base.OnSpawn();
    }
}

public class CTweenFillAmount : CTweener
{
#if UNITY_3_5
	public float from = 1f;
	public float to = 1f;
#else
    [Range(0f, 1f)]
    public float from = 1f;
    [Range(0f, 1f)]
    public float to = 1f;
#endif

    UISprite mSprite;

    public UISprite catchedSprite
    {
        get
        {
            if (mTrans == null) return null;
            if (mSprite == null)
                mSprite = mTrans.GetComponent<UISprite>();
            return mSprite;
        }
    }


    [System.Obsolete("Use 'value' instead")]
    public float mfillAmount { get { return this.value; } set { this.value = value; } }

    /// <summary>
    /// Tween's current value.
    /// </summary>

    public float value { get { return catchedSprite.fillAmount; } set { catchedSprite.fillAmount = value; } }

    /// <summary>
    /// Tween the value.
    /// </summary>

    protected override void OnUpdate(float factor, bool isFinished)
    {
        value = Mathf.Lerp(from, to, factor);
    }
    public override void OnDespawn()
    {
        mSprite = null;
        base.OnDespawn();
    }

    public override void OnSpawn()
    {
        mSprite = null;
        base.OnSpawn();
    }
}

/// <summary>
/// 缓动管理器
/// </summary>
public class TweenerManager : MonoBehaviour
{
    private DictionaryEx<string, List<CTweener>> viewTweens = new DictionaryEx<string, List<CTweener>>();

    private DictionaryEx<string, List<CTweener>> removeList = new DictionaryEx<string, List<CTweener>>();

    private static TweenerManager _instance = null;
    public static TweenerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("TweenerManager");
                _instance = go.AddComponent<TweenerManager>();
            }
            return _instance;
        }
    }

    void ClearRemoveList()
    {
        if (removeList != null && removeList.Count > 0)
        {
            for (int i = 0; i < removeList.Count; ++i)
            {
                List<CTweener> tweens = removeList.GetValue(i);
                if (tweens != null)
                {
                    for (int j = 0; j < tweens.Count; ++j)
                    {
                        CTweener cTweener = tweens[j];
                        if (cTweener == null)
                            continue;

                        if (!cTweener.enabled || cTweener.mTrans == null)
                        {
                            CacheObjects.DespawnCache(cTweener);
                            string key = removeList.GetKey(i);

                            List<CTweener> listView;
                            if ( viewTweens.TryGetValue(key ,out listView) && listView != null )
                            {
                                if (cTweener.removeCallback != null && SafeHelper.IsTargetValid(cTweener.removeCallback) && !cTweener.finishCallDone)
                                {
                                    cTweener.removeCallback();
                                }

                                listView.Remove(cTweener);
                                if (listView.Count == 0)
                                {
                                    viewTweens.Remove(key);
                                }
                            }
                        }
                    }
                }
            }
            removeList.Clear();
        }
    }

    void Update()
    {
        ClearRemoveList();

        if (viewTweens != null && viewTweens.Count > 0)
        {
            for (int i = 0; i < viewTweens.Count; ++i)
            {
                List<CTweener> tweens = viewTweens.GetValue(i);
                if (tweens != null && tweens.Count > 0)
                {
                    for (int j = 0; j < tweens.Count; ++j)
                    {
                        if (tweens[j] != null)
                        {
                            if (!tweens[j].enabled || tweens[j].mTrans == null)
                            {
                                string key = viewTweens.GetKey(i);
                                if (!removeList.ContainsKey(key))
                                {
                                    removeList.Add(key, new List<CTweener>());
                                }
                                if (!removeList[key].Contains(tweens[j]))
                                    removeList[key].Add(tweens[j]);
                                continue;
                            }
                            tweens[j].Update();
                        }
                    }
                }
            }
        }

        
    }


    public CTweenPosition BeginPosition(GameObject go, float duration, Vector3 to, string view = "", float delay = 0)
    {

        if (go == null) return null;
        CTweenPosition tween = null;
        bool find = false;
        if (viewTweens.ContainsKey(view))
        {
            for (int i = 0; i < viewTweens[view].Count; ++i)
            {
                if (viewTweens[view][i] != null)
                {
                    if (go.transform == viewTweens[view][i].mTrans)
                    {
                        if (viewTweens[view][i] is CTweenPosition)
                        {
                            tween = viewTweens[view][i] as CTweenPosition;
                            find = true;
                            break;
                        }
                    }
                }
            }
        }
        if (!find)
        {
            tween = CacheObjects.SpawnCache<CTweenPosition>();
            if (string.IsNullOrEmpty(view))
                view = "Custom";

            if (!viewTweens.ContainsKey(view))
            {
                viewTweens.Add(view, new List<CTweener>());
            }

            if (!viewTweens[view].Contains(tween))
                viewTweens[view].Add(tween);
        }
        tween.callback = null;
        tween.style = CTweener.Style.Once;
        tween.method = CTweener.Method.Linear;
        tween.mTrans = go.transform;
        tween.delay = delay;
        tween.duration = duration;
        tween.from = tween.value;
        tween.to = to;
        tween.Play();

        return tween;
    }


    public CTweenAlpha BeginAlpha(GameObject go, float duration, float to, string view = "", float delay = 0)
    {
        if (go == null) return null;
        CTweenAlpha tween = null;
        bool find = false;
        if (viewTweens.ContainsKey(view))
        {
            for (int i = 0; i < viewTweens[view].Count; ++i)
            {
                if (viewTweens[view][i] != null)
                {
                    if (go.transform == viewTweens[view][i].mTrans)
                    {
                        if (viewTweens[view][i] is CTweenAlpha)
                        {
                            tween = viewTweens[view][i] as CTweenAlpha;
                            find = true;
                            break;
                        }
                    }
                }
            }
        }
        if (!find)
        {
            tween = CacheObjects.SpawnCache<CTweenAlpha>();
            //tween = new CTweenAlpha();
            if (string.IsNullOrEmpty(view))
                view = "Custom";

            if (!viewTweens.ContainsKey(view))
            {
                viewTweens.Add(view, new List<CTweener>());
            }

            if (!viewTweens[view].Contains(tween))
                viewTweens[view].Add(tween);
        }
        tween.callback = null;
        tween.style = CTweener.Style.Once;
        tween.method = CTweener.Method.Linear;
        tween.mTrans = go.transform;
        tween.delay = delay;
        tween.duration = duration;
        tween.from = tween.value;
        tween.to = to;
        tween.Play();

        return tween;
    }

    public CTweenColor BeginColor(GameObject go, float duration, Color to, string view = "", float delay = 0)
    {
        if (go == null) return null;
        CTweenColor tween = null;
        bool find = false;
        if (viewTweens.ContainsKey(view))
        {
            for (int i = 0; i < viewTweens[view].Count; ++i)
            {
                if (viewTweens[view][i] != null)
                {
                    if (go.transform == viewTweens[view][i].mTrans)
                    {
                        if (viewTweens[view][i] is CTweenColor)
                        {
                            tween = viewTweens[view][i] as CTweenColor;
                            find = true;
                            break;
                        }
                    }
                }
            }
        }
        if (!find)
        {
            tween = CacheObjects.SpawnCache<CTweenColor>();
            if (string.IsNullOrEmpty(view))
                view = "Custom";

            if (!viewTweens.ContainsKey(view))
            {
                viewTweens.Add(view, new List<CTweener>());
            }

            if (!viewTweens[view].Contains(tween))
                viewTweens[view].Add(tween);
        }
        tween.callback = null;
        tween.style = CTweener.Style.Once;
        tween.method = CTweener.Method.Linear;
        tween.mTrans = go.transform;
        tween.delay = delay;
        tween.duration = duration;
        tween.from = tween.value;
        tween.to = to;
        tween.Play();

        return tween;
    }

    public CTweenScale BeginScale(GameObject go, float duration, Vector3 to, string view = "", float delay = 0)
    {
        if (go == null) return null;
        CTweenScale tween = null;
        bool find = false;
        if (viewTweens.ContainsKey(view))
        {
            for (int i = 0; i < viewTweens[view].Count; ++i)
            {
                if (viewTweens[view][i] != null)
                {
                    if (go.transform == viewTweens[view][i].mTrans)
                    {
                        if (viewTweens[view][i] is CTweenScale)
                        {
                            tween = viewTweens[view][i] as CTweenScale;
                            find = true;
                            break;
                        }
                    }
                }
            }
        }
        if (!find)
        {
            tween = CacheObjects.SpawnCache<CTweenScale>();
            if (string.IsNullOrEmpty(view))
                view = "Custom";

            if (!viewTweens.ContainsKey(view))
            {
                viewTweens.Add(view, new List<CTweener>());
            }

            if (!viewTweens[view].Contains(tween))
                viewTweens[view].Add(tween);
        }
        tween.callback = null;
        tween.style = CTweener.Style.Once;
        tween.method = CTweener.Method.Linear;
        tween.mTrans = go.transform;
        tween.delay = delay;
        tween.duration = duration;
        tween.from = tween.value;
        tween.to = to;
        tween.Play();

        return tween;
    }

    public CTweenRotation BeginRotation(GameObject go, float duration, Quaternion to, string view = "", float delay = 0)
    {
        if (go == null) return null;

        CTweenRotation tween = null;
        bool find = false;
        if (viewTweens.ContainsKey(view))
        {
            for (int i = 0; i < viewTweens[view].Count; ++i)
            {
                if (viewTweens[view][i] != null)
                {
                    if (go.transform == viewTweens[view][i].mTrans)
                    {
                        if (viewTweens[view][i] is CTweenRotation)
                        {
                            tween = viewTweens[view][i] as CTweenRotation;
                            find = true;
                            break;
                        }
                    }
                }
            }
        }
        if (!find)
        {
            tween = CacheObjects.SpawnCache<CTweenRotation>();
            if (string.IsNullOrEmpty(view))
                view = "Custom";

            if (!viewTweens.ContainsKey(view))
            {
                viewTweens.Add(view, new List<CTweener>());
            }

            if (!viewTweens[view].Contains(tween))
                viewTweens[view].Add(tween);
        }
        tween.callback = null;
        tween.style = CTweener.Style.Once;
        tween.method = CTweener.Method.Linear;
        tween.mTrans = go.transform;
        tween.delay = delay;
        tween.duration = duration;
        tween.from = tween.value.eulerAngles;
        tween.to = to.eulerAngles;
        tween.Play();

        return tween;
    }

    public CTweenWidth BeginWidth(GameObject go, float duration, int to, string view = "", float delay = 0)
    {
        if (go == null) return null;
        CTweenWidth tween = null;
        bool find = false;
        if (viewTweens.ContainsKey(view))
        {
            for (int i = 0; i < viewTweens[view].Count; ++i)
            {
                if (viewTweens[view][i] != null)
                {
                    if (go.transform == viewTweens[view][i].mTrans)
                    {
                        if (viewTweens[view][i] is CTweenWidth)
                        {
                            tween = viewTweens[view][i] as CTweenWidth;
                            find = true;
                            break;
                        }
                    }
                }
            }
        }
        if (!find)
        {
            tween = CacheObjects.SpawnCache<CTweenWidth>();
            if (string.IsNullOrEmpty(view))
                view = "Custom";

            if (!viewTweens.ContainsKey(view))
            {
                viewTweens.Add(view, new List<CTweener>());
            }

            if (!viewTweens[view].Contains(tween))
                viewTweens[view].Add(tween);
        }
        tween.callback = null;
        tween.style = CTweener.Style.Once;
        tween.method = CTweener.Method.Linear;
        tween.mTrans = go.transform;
        tween.delay = delay;
        tween.duration = duration;
        tween.from = tween.value;
        tween.to = to;
        tween.Play();

        return tween;
    }

    public CTweenHeight BeginHeight(GameObject go, float duration, int to, string view = "", float delay = 0)
    {
        if (go == null) return null;
        CTweenHeight tween = null;
        bool find = false;
        if (viewTweens.ContainsKey(view))
        {
            for (int i = 0; i < viewTweens[view].Count; ++i)
            {
                if (viewTweens[view][i] != null)
                {
                    if (go.transform == viewTweens[view][i].mTrans)
                    {
                        if (viewTweens[view][i] is CTweenHeight)
                        {
                            tween = viewTweens[view][i] as CTweenHeight;
                            find = true;
                            break;
                        }
                    }
                }
            }
        }
        if (!find)
        {
            tween = CacheObjects.SpawnCache<CTweenHeight>();
            if (string.IsNullOrEmpty(view))
                view = "Custom";

            if (!viewTweens.ContainsKey(view))
            {
                viewTweens.Add(view, new List<CTweener>());
            }

            if (!viewTweens[view].Contains(tween))
                viewTweens[view].Add(tween);
        }
        tween.callback = null;
        tween.style = CTweener.Style.Once;
        tween.method = CTweener.Method.Linear;
        tween.mTrans = go.transform;
        tween.delay = delay;
        tween.duration = duration;
        tween.from = tween.value;
        tween.to = to;
        tween.Play();

        return tween;
    }
    public CTweenFillAmount BeginFillAmount(GameObject go, float duration, float fillamount, string view = "", float delay = 0)
    {
        if (go == null) return null;
        CTweenFillAmount tween = null;
        bool find = false;
        if (viewTweens.ContainsKey(view))
        {
            for (int i = 0; i < viewTweens[view].Count; ++i)
            {
                if (viewTweens[view][i] != null)
                {
                    if (go.transform == viewTweens[view][i].mTrans)
                    {
                        if (viewTweens[view][i] is CTweenFillAmount)
                        {
                            tween = viewTweens[view][i] as CTweenFillAmount;
                            find = true;
                            break;
                        }
                    }
                }
            }
        }
        if (!find)
        {
            tween = CacheObjects.SpawnCache<CTweenFillAmount>();
            if (string.IsNullOrEmpty(view))
                view = "Custom";

            if (!viewTweens.ContainsKey(view))
            {
                viewTweens.Add(view, new List<CTweener>());
            }

            if (!viewTweens[view].Contains(tween))
                viewTweens[view].Add(tween);
        }
        tween.callback = null;
        tween.style = CTweener.Style.Once;
        tween.method = CTweener.Method.Linear;
        tween.mTrans = go.transform;
        tween.delay = delay;
        tween.duration = duration;
        tween.from = tween.value;
        tween.to = fillamount;
        tween.Play();

        return tween;
    }

    public void Remove(CTweener tween)
    {
        if (tween == null) return;
        tween.enabled = false;
        if (!removeList.ContainsKey("Custom"))
            removeList["Custom"] = new List<CTweener>();
        removeList["Custom"].Add(tween);
    }

    public void Remove(string view, CTweener tween)
    {
        if (tween == null) return;
        tween.enabled = false;
        if (string.IsNullOrEmpty(view))
            view = "Custom";
        if (!removeList.ContainsKey(view))
            removeList[view] = new List<CTweener>();
        removeList[view].Add(tween);
    }

    public void Remove(string view)
    {
        if (string.IsNullOrEmpty(view))
            return;

        if (viewTweens.ContainsKey(view))
        {
            List<CTweener> tweenList = viewTweens[view];
            if (tweenList == null)
                return;
            for (int i = 0; i < tweenList.Count; ++i)
            {
                tweenList[i].enabled = false;
            }
            if (!removeList.ContainsKey(view))
            {
                removeList[view] = new List<CTweener>();
            }
            removeList[view] = viewTweens[view];
        }   
    }

    /// <summary>
    /// 切场景清理
    /// </summary>
    public void Clear()
    {
        if (viewTweens != null && viewTweens.Count > 0)
        {
            for (int i = 0; i < viewTweens.Count; ++i)
            {
                List<CTweener> tweens = viewTweens.GetValue(i);
                if (tweens != null && tweens.Count > 0)
                {
                    for (int j = 0; j < tweens.Count; ++j)
                    {
                        if (tweens[j] != null)
                        {
                            CacheObjects.DespawnCache(tweens[j]);
                            tweens[j] = null;
                        }
                    }
                }
            }
            viewTweens.Clear();
        }

        if (removeList != null && removeList.Count > 0)
        {
            for (int i = 0; i < removeList.Count; ++i)
            {
                List<CTweener> tweens = removeList.GetValue(i);
                for (int j = 0; j < tweens.Count; ++j)
                {
                    if (tweens[j] != null)
                    {
                        CacheObjects.DespawnCache(tweens[j]);
                        tweens[j] = null;
                    }
                }
            }
            removeList.Clear();
        }
    }
}
