using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

/// <summary>
/// 播放类型
/// </summary>
public enum EActionPlayType
{
    UNKNOW_TYPE = 0,
    /// <summary>
    /// 普通播放
    /// </summary>
    ACTION_PLAYTYPE_NORAML = 1,
    /// <summary>
    /// 过渡播放
    /// </summary>
    ACTION_PLAYTYPE_CROSSFADE = 2,
}


/// <summary>
/// 动作代理
/// </summary>
[System.Serializable]
public class AnimationProxy : MonoBehaviour
{
    [System.Serializable]
    public class AnimationInfo
    {
        public string strName = string.Empty;
        public string strPath = string.Empty;
    }

    [SerializeField]
    public AnimationInfo[] mAnimations;
    [SerializeField]
    public AnimationInfo mMainClip;

    // 人物上半身骨骼
    [SerializeField]
    public Transform upPathBodyBone = null;

    // 人物全身骨骼
    //[SerializeField]
    //public Transform allPathBodyBone = null;

    /// <summary>
    /// 动作播放时参数
    /// </summary>
    public struct ActionInfo
    {
        /// <summary>
        /// 播放模式
        /// </summary>
        public EActionPlayType ePlayType;
        public string strActionName;
        public float fSpeed;
        public float fNormalize;
        public string strActionNext;
        public float fFadeTime;
        public OnWatchAnimationPlayed onWatch;
        public OnAnimationPlayCallBack onCallback;                                               //是否播放上半身动作
        public object args;
        public ActionInfo(EActionPlayType ePlayType, string strActionName, object args=null)
        {
            this.ePlayType = ePlayType;
            this.strActionName = strActionName;
            fSpeed = 1.0f;
            fNormalize = 1.0f;
            strActionNext = string.Empty;
            fFadeTime = 1.0f;
            onWatch = null;
            onCallback = null;
            this.args = args;
        }
        public static ActionInfo Zero = new ActionInfo(EActionPlayType.ACTION_PLAYTYPE_NORAML, string.Empty, null);
    }

    /// <summary>
    /// 动画事件委托
    /// </summary>
    public AniamtionEventDelegate mAnimationEvent;
    /// <summary>
    /// 得到当前正在播放的动作
    /// </summary>
    /// <param name="aGroup"></param>
    /// <returns></returns>
    public static string GetAniaNamePlaying(AnimationProxy aGroup)
    {
        if (aGroup == null || aGroup.mAnimation == null)
            return string.Empty;

        if (!aGroup.mAnimation.isPlaying)
            return string.Empty;

        return aGroup.CurrentPlayActionName;
    }

    /// <summary>
    /// 得到当前正在播放的动作
    /// </summary>
    /// <param name="aGroup"></param>
    /// <returns></returns>
    public static AnimationState GetAniaStatePlaying(AnimationProxy aGroup)
    {
        if (aGroup == null || aGroup.mAnimation == null)
            return null;

        if (!aGroup.mAnimation.isPlaying || string.IsNullOrEmpty(aGroup.CurrentPlayActionName))
            return null;

        return aGroup.mAnimation[aGroup.CurrentPlayActionName];
    }

    /// <summary>
    /// 最大动画数
    /// </summary>
    public const int miActionListMaxCount = 5;

    /// <summary>
    /// 动画剪辑
    /// </summary>
    public Animation mAnimation;

    /// <summary>
    /// 动作列表
    /// </summary>
    public List<string> mActionList = new List<string>();

    /// <summary>
    /// 当前动作的播放队列
    /// </summary>
    private List<ActionInfo> mActionWaitQueue = new List<ActionInfo>(10);

    /// <summary>
    /// 当前暂停的动画(被修改过speed)
    /// </summary>
    private AnimationState maCurAnimationState = null;

    /// <summary>
    /// 当前正在播放的动画名
    /// </summary>
    public string CurrentPlayActionName
    {
        get;
        private set;
    }

	public delegate void CallLoadAsset(string strFileName, AssetCallback callback, VarStore varStore = null, bool bAsync = false);
    public static CallLoadAsset monLoadAsset = null;
    //设置动画加载代理
    public static void SetLoadAssetCall(CallLoadAsset call)
    {
        monLoadAsset = call;
    }

    void Awake()
    {
        mAnimation = gameObject.GetComponent<Animation>();
        if (mAnimation == null)
        {
            mAnimation = gameObject.AddComponent<Animation>();
        }
        else
        {
            if(mActionList != null)
            {
                if (mActionList.Count != 0)
                {
                    //对象被克隆，删除动画文件(不能走缓存池，要不然计数会有问题)
                    for (int i = 0; i < mActionList.Count; i++)
                    {
                        AnimationClip aClip = mAnimation.GetClip(mActionList[i]);
                        mAnimation.RemoveClip(aClip);
                    }
                    mActionList.Clear();
                    if (mAnimation.clip != null)
                    {
                        mAnimation.RemoveClip(mAnimation.clip);
                        mAnimation.clip = null;
                    }
                }
                else
                {
                    LogSystem.LogWarning("AnimationProxy::动画被直接挂在对象上", gameObject.name);
                }
            }
            
        }

        if (mAnimation == null)
        {
            LogSystem.LogWarning("mAnimation is NULL " + gameObject.name);
            return;
        }
    }

    void OnEnable()
    {
        if (mMainClip != null && !string.IsNullOrEmpty(mMainClip.strName))
        {
            //如果没有播放过任何动作，播放主动作
            if (string.IsNullOrEmpty(GetAniaNamePlaying(this)) && mActionWaitQueue.Count == 0)
            {
                PlayAnimationByPlay(mMainClip.strName, -1.0f, -1.0f, "", null, null);
                //先加载默认动作
                LoadAnimationClip(mMainClip.strName, 0, false);
            }
        }
    }

    /// <summary>
    /// 播放普通动画(只是将要播放的动画信息存在列表中)
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public bool PlayAnimationByPlay(string strActionName, float fSpeed, float fNormalize
        , string strNextAnim, OnWatchAnimationPlayed onWatch, OnAnimationPlayCallBack onCallBack,object args = null)
    {
        ActionInfo actionInfo = ActionInfo.Zero;
        actionInfo.strActionName = strActionName;
        actionInfo.ePlayType = EActionPlayType.ACTION_PLAYTYPE_NORAML;
        actionInfo.fSpeed = fSpeed;
        actionInfo.fNormalize = fNormalize;
        actionInfo.strActionNext = strNextAnim;
        actionInfo.onWatch = onWatch;
        actionInfo.onCallback = onCallBack;
        actionInfo.args = args;
        if (!ContainsAnimations(strActionName))
        {
            PlayClipFail(actionInfo);
            return false;
        }

        for (int i = 0; i < mActionWaitQueue.Count; i++)
        {
            ActionInfo aInfo = mActionWaitQueue[i];
            if (strActionName == aInfo.strActionName)
            {
                mActionWaitQueue.RemoveAt(i);
                break;
            }
        }

        mActionWaitQueue.Add(actionInfo);
        return true;
    }

    /// <summary>
    /// 播放融合动画(只是将要播放的动画信息存在列表中)
    /// </summary>
    /// <param name="strActionName"></param>
    /// <param name="fFadeTime"></param>
    /// <param name="fSpeed"></param>
    /// <param name="strNextAnim"></param>
    /// <param name="onWatch"></param>
    /// <param name="onCallBack"></param>
    /// <returns></returns>
    public bool PlayAnimationByCross(string strActionName, float fFadeTime, float fSpeed
        , string strNextAnim, OnWatchAnimationPlayed onWatch, OnAnimationPlayCallBack onCallBack,  object args)
    {
        ActionInfo actionInfo = ActionInfo.Zero;
        actionInfo.ePlayType = EActionPlayType.ACTION_PLAYTYPE_CROSSFADE;
        actionInfo.strActionName = strActionName;
        actionInfo.fSpeed = fSpeed;
        actionInfo.fFadeTime = fFadeTime;
        actionInfo.strActionNext = strNextAnim;
        actionInfo.onWatch = onWatch;
        actionInfo.onCallback = onCallBack;
        actionInfo.args = args;
        if (!ContainsAnimations(strActionName))
        {
            PlayClipFail(actionInfo);
            return false;
        }

        for (int i = 0; i < mActionWaitQueue.Count; i++)
        {
            ActionInfo aInfo = mActionWaitQueue[i];
            if (strActionName == aInfo.strActionName)
            {
                mActionWaitQueue.RemoveAt(i);
                break;
            }
        }

        mActionWaitQueue.Add(actionInfo);
        return true;
    }

    /// <summary>
    /// 播放动画(在待播放列表中播放最新添加的动画，并且移除之前的队列 这里主要解决异步问题)
    /// </summary>
    public void CheckAnimationPlay()
    {
        //如果动作播放队列中有动作需要播放，检查对应的动作是否加载完毕,从后先前检查
        if (mActionWaitQueue.Count == 0)
            return;

        if (mAnimation == null)
            return;

        int iIndex = mActionWaitQueue.Count - 1;
        while (iIndex >= 0)
        {
            ActionInfo aInfo = mActionWaitQueue[iIndex];
            //此动作已经加载完毕，可以播放
            if (mAnimation.GetClip(aInfo.strActionName) != null)
            {
                mActionWaitQueue.RemoveRange(0, iIndex + 1);
                PlayClip(aInfo);
                break;
            }
            iIndex--;
        }
    }

    /// <summary>
    /// 动画播放失败
    /// </summary>
    /// <param name="args"></param>
    private void PlayClipFail(ActionInfo aInfo)
    {
        string strAnimationName = aInfo.strActionName;
        if (aInfo.onWatch != null)
        {
            aInfo.onWatch(this, strAnimationName, aInfo.strActionNext);
        }
        if (aInfo.onCallback != null)
        {
            aInfo.onCallback(strAnimationName, this);
        }
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="args"></param>
    private void PlayClip(ActionInfo aInfo)
    {
        CurrentPlayActionName = aInfo.strActionName;
        if (aInfo.onWatch != null)
        {
            aInfo.onWatch(this, CurrentPlayActionName, aInfo.strActionNext);
        }

        if (aInfo.onCallback != null)
        {
            aInfo.onCallback(CurrentPlayActionName, this, aInfo.args);
        }

        //mAnimation[aClip.name].layer = actionInfoConfig.iLayer;
        //if (actionInfoConfig.bUpperAction)
        //{
        //    if (upPathBodyBone == null)
        //    {
        //        upPathBodyBone = transform.FindChild("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1");
        //    }
        //    else
        //        mAnimation[aClip.name].AddMixingTransform(upPathBodyBone);
        //}
        if (aInfo.ePlayType == EActionPlayType.ACTION_PLAYTYPE_NORAML)
        {
            PlayAnimationClip(CurrentPlayActionName, aInfo.fSpeed, aInfo.fNormalize);
        }
        else
        {
            CrossFadeClip(CurrentPlayActionName, aInfo.fFadeTime, aInfo.fSpeed);
        }
    }

    /// <summary>
    /// 播放动画 不融合
    /// </summary>
    /// <param name="strAnimation"></param>
    /// <param name="fSpeed"></param>
    private void PlayAnimationClip(string strAnimation, float fSpeed = -1.0f, float fNormalized = -1.0f)
    {
        if (mAnimation == null)
            return;

        SequenceList(strAnimation);
        mAnimation.enabled = false;
        mAnimation.enabled = true;
        if (mAnimation.Play(strAnimation, PlayMode.StopAll))
        {
            if (fSpeed >= 0)
            {
                mAnimation[strAnimation].speed = fSpeed;
            }
            if (fNormalized >= 0)
            {
                mAnimation[strAnimation].normalizedTime = fNormalized;
            }
        }
        else
        {
            LogSystem.LogWarning("Play Animation Error AnimationName -->", strAnimation);
        }
    }

    /// <summary>
    /// 动画播放 融合
    /// </summary>
    /// <param name="strAnimation"></param>
    /// <param name="fFade"></param>
    /// <param name="fSpeed"></param>
    private void CrossFadeClip(string strAnimation, float fFade, float fSpeed = -1.0f)
    {
        if (mAnimation == null)
            return;

        SequenceList(strAnimation);
        mAnimation.enabled = false;
        mAnimation.enabled = true;
        mAnimation.CrossFade(strAnimation, fFade);
        if (fSpeed >= 0)
        {
            mAnimation[strAnimation].speed = fSpeed;
        }
    }

    /// <summary>
    /// 动画事件
    /// </summary>
    /// <param name="strEventID"></param>
    void OnAnimationEvent(string strEffectGroupID)
    {
        if (mAnimationEvent != null)
            mAnimationEvent(strEffectGroupID);
    }

    /// <summary>
    /// 加载动画
    /// </summary>
    /// <param name="strAnimationName"></param>
    public void LoadAnimationClip(string strAnimationName, int layer, bool bUpperAction)
    {
        if (mAnimation != null && mAnimation.GetClip(strAnimationName) != null)
        {
            CheckAnimationPlay();
        }
        else
        {
            string strFile = GetAnimationPath(strAnimationName);
            if (string.IsNullOrEmpty(strFile))
            {
                return;
            }

            if (monLoadAsset != null)
            {
                VarStore _var = VarStore.CreateVarStore();
                _var+=layer;
                _var+=bUpperAction;
                monLoadAsset(strFile, (UnityEngine.Object oAsset, string strFileName, VarStore varStore) =>
                {
                    AnimationClip aClip = oAsset as AnimationClip;
                    int layerTemp = 0;
                    bool bUpperActionTemp = false;
                    if (varStore != null)
                    {
                        layerTemp = varStore.GetInt(0);
                        bUpperActionTemp = varStore.GetBool(1);
                        _var.Destroy();
                    }
                    if (aClip != null)
                    {
                        AddAnimationClip(aClip, layerTemp, bUpperActionTemp);
                    }
                    //开始检查并播放最佳一个可播放动作
                    CheckAnimationPlay();
                }, _var);//
            }
        }
    }

    /// <summary>
    /// 获取动作路径
    /// </summary>
    /// <param name="strName"></param>
    /// <returns></returns>
    public string GetEditorAnimationPath(string strName)
    {
        if (mAnimations == null)
        {
            return string.Empty;
        }
        for (int i = 0; i < mAnimations.Length; i++)
        {
            if (mAnimations[i] != null && mAnimations[i].strName == strName)
            {
                return mAnimations[i].strPath;
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// 获取动作名
    /// </summary>
    /// <param name="strName"></param>
    /// <returns></returns>
    public string GetAnimationPath(string strName)
    {
        if (mAnimations == null)
            return string.Empty;

        for (int i = 0; i < mAnimations.Length; ++i)
        {
            if (mAnimations[i] != null)
            {
                if (strName.Equals(mAnimations[i].strName))
                    return mAnimations[i].strPath;
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// 是否有动画剪辑
    /// </summary>
    /// <param name="strActionID"></param>
    public bool IsAnimationClipLoaded(string strActionID)
    {
        return mAnimation.GetClip(strActionID) != null;
    }

    /// <summary>
    /// 将加载的动画加入到动画控制器中
    /// </summary>
    /// <param name="aClip"></param>
    private void AddAnimationClip(AnimationClip aClip, int layer, bool bUpperAction)
    {
        if (mAnimation == null || aClip == null)
            return;

        //已经有此动画
        if (IsAnimationClipLoaded(aClip.name))
        {
            return;
        }

        if (mMainClip != null && mMainClip.strName == aClip.name)
        {
            if (mAnimation.clip != null && mAnimation.clip != aClip)
            {
                CacheObjects.PopCacheAnimation(mAnimation.clip);
                mAnimation.clip = null;
            }
            mAnimation.clip = aClip;
            mAnimation.AddClip(aClip, aClip.name);
        }
        else
        {
            mActionList.Add(aClip.name);
            mAnimation.AddClip(aClip, aClip.name);
            PopAnimationControl();
        }
        mAnimation[aClip.name].layer = layer;
        if (bUpperAction)
        {
            if (upPathBodyBone == null)
                upPathBodyBone = transform.Find("Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1");

            //if (allPathBodyBone == null)
                //allPathBodyBone = transform.FindChild("Bip01");

            mAnimation[aClip.name].AddMixingTransform(upPathBodyBone);
        }
    }

    /// <summary>
    /// 移出动画
    /// </summary>
    private void PopAnimationControl()
    {
        if (mActionList.Count > miActionListMaxCount)
        {
            if (mAnimation == null)
                return;

            string strClipName = mActionList[0];
            mActionList.RemoveAt(0);
            AnimationClip aClip = mAnimation.GetClip(strClipName);
            mAnimation.RemoveClip(strClipName);
            CacheObjects.PopCacheAnimation(aClip);
        }
    }

    
    public bool IsPlaying(string strAnimation)
    {
        if (mAnimation == null)
            return false;

        return mAnimation.IsPlaying(strAnimation);
    }

    /// <summary>
    /// 排序 播动作 放到数组后面
    /// </summary>
    /// <param name="strAnimation"></param>
    private bool SequenceList(string strAnimation)
    {
        if (mActionList == null)
            return false;

        int index = mActionList.IndexOf(strAnimation);
        if (index == -1)
        {
            //主动作不在actin list 中
            if (mMainClip != null && mMainClip.strName == strAnimation)
                return true;

        }
        if (index >= 0 && index != mActionList.Count - 1)
        {
            mActionList.RemoveAt(index);
            mActionList.Add(strAnimation);
        }
        return true;
    }

    public void Stop()
    {
        if (mAnimation == null)
            return;

        CurrentPlayActionName = string.Empty;
        mAnimation.Stop();
    }

    /// <summary>
    /// 设置动画从某个时间播放
    /// </summary>
    /// <param name="strAnimation"></param>
    /// <param name="normalized"></param>
    public void SetAnimaNormalized(string strAnimation, float normalized)
    {
        if (mAnimation == null)
            return;

        CurrentPlayActionName = strAnimation;
        if (mAnimation.GetClip(CurrentPlayActionName) == null)
            return;

        SequenceList(CurrentPlayActionName);
        mAnimation.Play(CurrentPlayActionName);
        mAnimation[CurrentPlayActionName].normalizedTime = normalized;
    }

    /// <summary>
    /// 获取动画播放时间
    /// </summary>
    /// <param name="strAnimation"></param>
    /// <returns></returns>
    public float GetAnimaNormalized(string strAnimation)
    {
        if (mAnimation == null)
            return 0;

        if (mAnimation.GetClip(strAnimation) == null)
            return 0;

        return mAnimation[strAnimation].normalizedTime;
    }

    /// <summary>
    /// 设置动画播放
    /// </summary>
    /// <param name="fNormalized"></param>
    public void SetAnimationNormalized(float fNormalized)
    {
        //LogSystem.LogError("设置动画播放:" + fNormalized);
        AnimationState aCurAnimationState = GetAniaStatePlaying(this);
        if (aCurAnimationState == null)
            return;

        aCurAnimationState.normalizedTime = fNormalized;
    }

    /// <summary>
    /// 是否包含该动画
    /// </summary>
    /// <param name="strAnim"></param>
    /// <returns></returns>
    public bool ContainsAnimations(string strAnim)
    {
        if (mAnimations != null)
        {
            for (int i = 0; i < mAnimations.Length; ++i)
            {
                if (mAnimations[i] != null)
                {
                    if (strAnim.Equals(mAnimations[i].strName))
                        return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 设置动画播放速度
    /// </summary>
    /// <param name="strAnimation"></param>
    /// <param name="fSpeed"></param>
    public void SetAnimaSpeed(string strAnimation, float fSpeed)
    {
        if (mAnimation == null)
            return;

        if (mAnimation.GetClip(strAnimation) == null)
            return;

        SequenceList(strAnimation);
        mAnimation[strAnimation].speed = fSpeed;
    }

    /// <summary>
    /// 动作暂停
    /// </summary>
    public void Pause()
    {
        maCurAnimationState = GetAniaStatePlaying(this);
        if (maCurAnimationState == null)
        {
            LogSystem.LogWarning("AnimationProxy::Pause no play any actions");
            return;
        }
        maCurAnimationState.speed = 0;
    }

    /// <summary>
    /// 动作恢复
    /// </summary>
    public void Resume()
    {
        if (maCurAnimationState == null)
        {
            LogSystem.LogWarning("AnimationProxy::Resume no play any actions");
            return;
        }
        maCurAnimationState.speed = 1;
    }

    /// <summary>
    /// 设置速度
    /// </summary>
    public float fSpeed
    {
        get
        {
            AnimationState aCurAnimationState = GetAniaStatePlaying(this);
            if (aCurAnimationState == null)
                return 0f;
            return aCurAnimationState.speed;
        }

        set
        {

            AnimationState aCurAnimationState = GetAniaStatePlaying(this);
            if (aCurAnimationState != null)
            {
                aCurAnimationState.speed = value;
            }
        }
    }


    /// <summary>
    /// 动画播放长度
    /// </summary>
    /// <param name="strAnimation"></param>
    /// <returns></returns>
    public float GetNormalized(string strAnimation)
    {
        if (mAnimation == null)
            return 1f;
        AnimationState astate = mAnimation[strAnimation];
        if (astate != null)
            return astate.normalizedTime;
        return 1f;
    }


    void OnDestroy()
    {
        ClearAnimationClips();
        mActionList = null;
        mActionWaitQueue = null;
        mAnimations = null;
        mMainClip = null;
        if (mAnimation != null)
        {
            GameObject.Destroy(mAnimation);
            mAnimation = null;
        }
    }

    /// <summary>
    /// 清除所有动作引用(对象被放回缓存池中调用)
    /// </summary>
    public void ClearAnimationClips()
    {
        CurrentPlayActionName = string.Empty;
        mbCaton = false;
        if (mActionWaitQueue != null)
            mActionWaitQueue.Clear();

        maCurAnimationState = null;
        mAnimationEvent = null;
        upPathBodyBone = null;

        if (mAnimation == null)
        {
            if (mActionList != null)
                mActionList.Clear();

            return;
        }

        if (mActionList != null && mActionList.Count != 0)
        {
            //清除所有一次性动作
            for (int i = 0; i < mActionList.Count; i++)
            {
                string strActionName = mActionList[i];
                AnimationClip aClip = mAnimation.GetClip(strActionName);
                if (aClip != null)
                {
                    mAnimation.RemoveClip(aClip);
                    CacheObjects.PopCacheAnimation(aClip);
                }
            }
            mActionList.Clear();
        }

        //清除主动作
        if (mAnimation.clip != null)
        {
            mAnimation.RemoveClip(mAnimation.clip);
            CacheObjects.PopCacheAnimation(mAnimation.clip);
            mAnimation.clip = null;
        }
    }

    /// <summary>
    /// 方便取存值
    /// </summary>
    /// <param name="tkey"></param>
    /// <returns></returns>
    public AnimationState this[string tkey]
    {
        get
        {
            if (mAnimation == null)
                return null;

            return mAnimation[tkey];
        }
    }

    #region 卡顿
    /// <summary>
    /// 是否卡顿
    /// </summary>
    private bool mbCaton = false;
    /// <summary>
    /// 卡顿时间
    /// </summary>
    private float mfCatonTime = 0f;
    /// <summary>
    /// 恢复速度
    /// </summary>
    private float mfResumeSpeed = 0f;
    
    private void Update()
    {
        if (!mbCaton)
            return;

        mfCatonTime -= Time.deltaTime;
        if (mfCatonTime > 0)
            return;

        mbCaton = false;
        if (maCurAnimationState != null)
        {
            maCurAnimationState.speed = mfResumeSpeed;
            maCurAnimationState = null;
        }
    }

    /// <summary>
    /// 卡顿
    /// </summary>
    /// <param name="fCatonSpeed"></param>
    /// <param name="fCatonTime"></param>
    /// <param name="fResumeSpeed"></param>
    public void AnimationCaton(float fCatonSpeed, float fCatonTime, float fResumeSpeed)
    {
        maCurAnimationState = GetAniaStatePlaying(this);
        if (maCurAnimationState == null)
            return;

        mbCaton = true;
        mfResumeSpeed = fResumeSpeed;
        mfCatonTime = fCatonTime;
        maCurAnimationState.speed = fCatonSpeed;
    }

    #endregion
}


