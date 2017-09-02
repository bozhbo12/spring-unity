using UnityEngine;
using System.Collections;

/// <summary>
/// 加载特效脚本
/// </summary>
public class ModelLoadEffect : EffectLevel
{
    public enum EffectType
    {
        NONE = -1,
        FBX =0,
        WEAPON = 2,
    }

    public EffectType mEffectType = EffectType.NONE;

    /// <summary>
    /// 加载模型完成回调
    /// </summary>
    public System.Action<GameObject> LoadModelComplete;

    /// <summary>
    /// 对象显示改变
    /// </summary>
    public OnActionCallback gameObjectActiveCallBack;

    public string mstrPath = string.Empty;

    public Vector3 mvecPos = Vector3.zero;

    public Vector3 mvecRot = Vector3.zero;

    public Vector3 mvecScale = Vector3.one;

    /// <summary>
    /// 当前节点(特效的父节点)
    /// </summary>
    private GameObject mGo;

    /// <summary>
    /// 特效
    /// </summary>
    private GameObject meffectObject;

    /// <summary>
    /// 是否界面
    /// </summary>
    public bool bUIViev = false;
    
    /// <summary>
    /// 特效原先的层级
    /// </summary>
    private int layer;

    /// <summary>
    /// 是否在加载中
    /// </summary>
    private bool mbLoading = false;

    /// <summary>
    /// 是否删除
    /// </summary>
    private bool mbDestroy = false;

    /// <summary>
    /// 加载出来的特效id
    /// </summary>
    private string mstrEffectName = string.Empty;
    void Awake()
    {
        mGo = gameObject;
    }

    /// <summary>
    /// 获取组件特效  重新设定引用
    /// </summary>
    public void GetEffectObject()
    {
        Transform tranEffect;
        if (TryGetChildEffect(mstrEffectName, out tranEffect))
            meffectObject = tranEffect.gameObject;
    }

    protected override void OnSetActive(bool bShow, bool bInitiative)
    {
        if (bShow)
        {
            mbDestroy = false;
            Transform tranEffect;
            if (TryGetChildEffect(mstrEffectName, out tranEffect))
            {
                meffectObject = tranEffect.gameObject;
                if (mGo != null)
                {
                    SetLayer(meffectObject, mGo.layer);
                    meffectObject.SetActive(true);
                }
            }
            else
            {
                if (mbLoading)
                    return;

                mbLoading = true;
                DelegateProxy.LoadAsset(mstrPath, OnLoadEffectComplete, null);
            }
        }
        else
        {
            LoadModelComplete = null;
            gameObjectActiveCallBack = null;
            mbDestroy = true;
            if (mbLoading)
                return;

            DestoryEffect(bInitiative);
        }
    }

    /// <summary>
    /// 通过名称获取特效
    /// </summary>
    /// <param name="strName"></param>
    /// <param name="oTransEffect"></param>
    /// <returns></returns>
    private bool TryGetChildEffect(string strName, out Transform oTransEffect)
    {
        oTransEffect = null;
        if (string.IsNullOrEmpty(strName))
            return false;

        oTransEffect = transform.Find(strName);
        return oTransEffect != null;
    }

    /// <summary>
    /// 删除效果
    /// 1.根节点被放入缓存池
    ///     a.遍历子节点将meffectObject放入缓存池，再把自身放缓存池(缓存池处理)
    ///     b.触发OnDisable回调清除引用
    /// 2.根节点隐藏(不支持该种情况)
    ///     以前方案:触发OnDisable回调清除引用，并调用到DestoryEffect。但是unity隐藏不支持在隐藏时设置parent,所以不能执行(废弃)
    ///     现在方案:触发OnDisable回调清除引用
    /// </summary>
    private void DestoryEffect(bool bDestroy)
    {
        if (meffectObject == null)
            return;
        if (bDestroy)
        {
            CacheObjects.DestoryPoolObject(meffectObject);
        }
        else
        {
            meffectObject.SetActive(false);
            SetLayer(meffectObject, layer);
        }
        //如果父节点不是ModelLoadEffect 则可能已经被删除了,不需要再做重复删除
        //if (meffectObject.transform.parent == transform)
        //{
        //    CacheObjects.DestoryPoolObject(meffectObject);
        //}
        //else
        //{
        //    //特效在显示对象，但根节点不一致
        //    if (CacheObjects.IsSpawnObject(meffectObject.transform))
        //    {
        //        CacheObjects.DestoryPoolObject(meffectObject);
        //        LogSystem.LogWarning("ModelLoadEffect::DestoryEffect 特效在显示对象，但根节点不一致");
        //    }
        //}
        meffectObject = null;
    }

    /// <summary>
    /// 特效加载完成
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="strFileName"></param>
    /// <param name="varStore"></param>
    private void OnLoadEffectComplete(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        mbLoading = false;
        if(mbDestroy)
        {
            CacheObjects.PopCache(oAsset);
            return;
        }

        if (mGo == null)
        {
            CacheObjects.PopCache(oAsset);
            return;
        }

        if (oAsset == null)
            return;

        meffectObject = CacheObjects.InstantiatePool(oAsset, Vector3.zero, Quaternion.identity) as UnityEngine.GameObject;
        if (meffectObject != null)
        {
            meffectObject.transform.parent = transform;
            meffectObject.transform.localPosition = mvecPos;
            meffectObject.transform.localRotation = Quaternion.Euler(mvecRot);
            meffectObject.transform.localScale = mvecScale;
            layer = meffectObject.layer;
            SetLayer(meffectObject, mGo.layer);
            mstrEffectName = meffectObject.name;

            if (!meffectObject.activeSelf)
                meffectObject.SetActive(true);

            if (LoadModelComplete != null)
                LoadModelComplete(meffectObject);

            if (gameObjectActiveCallBack != null)
                gameObjectActiveCallBack();
        }
    }

    /// <summary>
    /// 设置层级
    /// </summary>
    /// <param name="go"></param>
    /// <param name="layer"></param>
    public static void SetLayer(GameObject go, int layer)
    {
        if (go == null) return;
        go.layer = layer;
        if (go.transform.childCount > 0)
        {
            int len = go.transform.childCount;
            for (int i = 0; i < len; i++)
            {
                SetLayer(go.transform.GetChild(i).gameObject, layer);
            }
        }
    }
}
