using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ab包
/// </summary>
public abstract class AssetBundleBase
{
    /// <summary>
    /// 资源包加载器
    /// </summary>
    protected AssetBundleLoad mABLoad;

    /// <summary>
    /// 配置文件
    /// </summary>
    protected AssetBundleManifest mABManifest;

    ///// <summary>
    ///// 引用计数(resources/action/cg.unity3d -> num)
    ///// </summary>
    //protected static Dictionary<string/**合并包资源路径 如:resources/action/cg.unity3d*/, int> mDependsToTimes = new Dictionary<string, int>();

    ///// <summary>
    ///// 资源与路径对应(oAsset -> resources/action/cg.unity3d)
    ///// </summary>
    //protected Dictionary<UnityEngine.Object/**Asset资源*/, string/**合并包资源路径 如:resources/action/cg.unity3d*/> mObjectsToDepends = new Dictionary<UnityEngine.Object, string>();


    /// <summary>
    /// ab缓存
    /// </summary>
    private Dictionary<string/**resources/action/cg.unity3d*/, ABCacheInfo> mAbCacheInfoDic = new Dictionary<string, ABCacheInfo>();

    /// <summary>
    /// asset->路径对应关系
    /// </summary>
    private Dictionary<UnityEngine.Object/**Asset资源*/, string/**合并包资源路径 如:resources/action/cg.unity3d*/> mABAssetPathDic = new Dictionary<UnityEngine.Object, string>();


    public AssetBundleBase(AssetBundleLoad abLoad)
    {
        this.mABLoad = abLoad;
    }

    /// <summary>
    /// 读取ab包所需的配置文件
    /// </summary>
    /// <param name="callBack"></param>
    public abstract void ReadConfig(System.Action callBack);

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="strFileName"></param>
    /// <param name="lCallback"></param>
    /// <param name="strOrignFileName"></param>
    /// <param name="callback"></param>
    /// <param name="varStore"></param>
    /// <param name="bAsync"></param>
    /// <returns></returns>
    public abstract bool LoadAsset(string strFileName, OnLoadCallBack lCallback, string strOrignFileName, AssetCallback callback, VarStore varStore, bool bAsync = false);

    /// <summary>
    /// ResourceInfoList.xml 是否配置的该文件
    /// </summary>
    /// <param name="strFileName"></param>
    /// <returns></returns>
    public abstract bool IsContainAsset(string strFileName);

    /// <summary>
    /// 是否有该资源包
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public abstract bool IsContainBundle(string bundleName);

    /// <summary>
    /// 获取资源文件（只针对场景字节与Terrain文件）
    /// -资源包为合并包(小包)ab包没有后缀
    /// -资源包为散包时ab包后缀.unity
    /// </summary>
    /// <param name="strFileName">资源路径（注：不带.unity后缀路径）</param>
    /// <returns></returns>
    public abstract string GetResABPath(string strFileName);


    public virtual void RemoveObjectsDepends(UnityEngine.Object oAsset)
    {

    }

    public virtual string GetAbPathFromResPath(string strResPath)
    {
        LogSystem.LogWarning("GetAbPathFromResPath !!!");
        return string.Empty;
    }

    public virtual void Update()
    {

    }

    /// <summary>
    /// 移除资源
    /// </summary>
    /// <param name="bundle"></param>
    protected virtual bool RemoveAssetBundle(AssetBundle bundle)
    {
        if (bundle == null)
            return false;

        try
        {
            bundle.Unload(false);
        }
        catch (System.Exception e)
        {
            LogSystem.LogWarning("RemoveAssetBundle:", e.ToString());
        }
        return true;
    }

    /// <summary>
    /// 加载主对象时，添加对应的依赖包列表引用计数
    /// </summary>
    protected void PushObjects(UnityEngine.Object oAsset, string strABName)
    {
        if (oAsset == null || string.IsNullOrEmpty(strABName))
            return;

        if (!mABAssetPathDic.ContainsKey(oAsset))
        {
            mABAssetPathDic.Add(oAsset, strABName);
        }
    }

    /// <summary>
    /// 获取ab包
    /// </summary>
    /// <param name="strABName"></param>
    /// <returns></returns>
    protected AssetBundle GetAssetbundleByAbName(string strABName)
    {
        ABCacheInfo abInfo = null;
        if (!mAbCacheInfoDic.TryGetValue(strABName, out abInfo))
        {
            return null;
        }
        return abInfo.mAssetBundle;
    }

    /// <summary>
    /// 是否为通用资源
    /// </summary>
    /// <param name="strAbName"></param>
    /// <returns></returns>
    protected virtual bool IsCommonRes(string strAbName)
    {
        return false;
    }

    /// <summary>
    /// 增加计数
    /// </summary>
    /// <param name="strABName"></param>
    /// <param name="assetBundle"></param>
    /// <param name="strDependAssets"></param>
    /// <returns></returns>
    protected bool PushCacheInfo(string strABName, AssetBundle assetBundle, string[] strDependAssets = null)
    {
        ABCacheInfo abInfo = null;
        if (!mAbCacheInfoDic.TryGetValue(strABName, out abInfo))
        {
            abInfo = ABCacheInfo.GetAbCache();
            abInfo.mABName = strABName;
#if DEV
            abInfo.mfCreateTime = UnityEngine.Time.realtimeSinceStartup;
#endif
            abInfo.miUseTimes = 0;
            abInfo.mAssetBundle = assetBundle;
            abInfo.mstrDependAsset = strDependAssets;
            mAbCacheInfoDic.Add(strABName, abInfo);
        }
        abInfo.miUseTimes++;
        return true;
    }

    /// <summary>
    /// 移除资源
    /// </summary>
    /// <param name="oAsset"></param>
    /// <returns></returns>
    protected bool PopCacheInfo(UnityEngine.Object oAsset)
    {
        string strABName = string.Empty;
        if (!mABAssetPathDic.TryGetValue(oAsset, out strABName))
        {
            //LogSystem.LogWarning("AssetBundleBase::PopCahceInfo ", oAsset);
            return false;
        }
        //移除资源
        mABAssetPathDic.Remove(oAsset);
        return PopCacheInfo(strABName);
    }

    /// <summary>
    /// 计数
    /// </summary>
    /// <param name="strABName"></param>
    /// <returns></returns>
    protected bool PopCacheInfo(string strABName)
    {
        ABCacheInfo abInfo = null;
        if (!mAbCacheInfoDic.TryGetValue(strABName, out abInfo))
        {
            LogSystem.LogWarning("AssetBundleBase::PopCahceInfo2 ", strABName);
            return false;
        }

        //移除依赖资源
        PopCacheInfo(abInfo.mstrDependAsset);
        abInfo.miUseTimes--;
        if(abInfo.miUseTimes <= 0)
        {
            //不移除通用资源
            if (!IsCommonRes(strABName))
            {
                //缺裁assetbundle
                RemoveAssetBundle(abInfo.mAssetBundle);
                //移除引用关系
                mAbCacheInfoDic.Remove(strABName);
                //释放对象
                abInfo.Dispose();
            }
        }
        return true;
    }

    public void PrintLog()
    {
        LogSystem.LogWarning("=========================AssetBundleBase Total Info Start===========================");
        List<ABCacheInfo> list = new List<ABCacheInfo>(mAbCacheInfoDic.Values);
        list.Sort(Comparison);
        ABCacheInfo cInfo = null;
        for (int i = 0; i < list.Count; i++)
        {
            cInfo = list[i];
#if DEV
            LogSystem.LogWarning("Asset -> ", cInfo.mABName, "  UseCount:", cInfo.miUseTimes, "  CreateTime:", cInfo.mfCreateTime, "  UsedTime:", cInfo.fUsedTime);
#else
            LogSystem.LogWarning("Asset -> ", cInfo.mABName, "  UseCount:", cInfo.miUseTimes);
#endif
        }
    }

    private static int Comparison(ABCacheInfo cInfo1, ABCacheInfo cInfo2)
    {
#if DEV
        return (int)(cInfo1.mfCreateTime - cInfo2.mfCreateTime);
#endif
        return 0;
    }


    /// <summary>
    /// 移除依赖资源
    /// </summary>
    /// <param name="strAbNames"></param>
    /// <returns></returns>
    protected bool PopCacheInfo(string[] strAbNames)
    {
        if (strAbNames == null || strAbNames.Length == 0)
            return false;

        for (int i = 0; i < strAbNames.Length; i++)
        {
            PopCacheInfo(strAbNames[i]);
        }
        return true;
    }

    /// <summary>
    /// ab包缓存
    /// </summary>
    public class ABCacheInfo : CacheObject
    {
        public static ABCacheInfo GetAbCache()
        {
            return CacheObjects.SpawnCache<ABCacheInfo>();
        }

        /// <summary>
        /// 资源包的路径resources/action/cg.unity3d
        /// </summary>
        public string mABName = string.Empty;

#if DEV
        public float mfCreateTime = 0.0f;
#endif

        /// <summary>
        /// 使用次数
        /// </summary>
        public int miUseTimes = 0;

        /// <summary>
        /// 资源
        /// </summary>
        public AssetBundle mAssetBundle = null;

        /// <summary>
        /// 依赖资源
        /// </summary>
        public string[] mstrDependAsset = null;

#if DEV
        public float fUsedTime
        {
            get
            {
                return Time.realtimeSinceStartup - mfCreateTime;
            }
        }
#endif
        public void Dispose()
        {
            mABName = string.Empty;
            miUseTimes = 0;
            mAssetBundle = null;
            mstrDependAsset = null;
#if DEV
            mfCreateTime = 0f;
#endif
            CacheObjects.DespawnCache(this);
        }
    }
}