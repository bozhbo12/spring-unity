using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AssetBundleManager
{
    public static string strAssetTail = ".unity3d";

    public static string mABConfigName = "AssetBundles";

    private static AssetBundleManifest mABManifest = null;  // 资源列表信息

    private static AssetBundleLoad _ABLoadIns;
    public static AssetBundleLoad ABLoadIns
    {
        get
        {
            if (_ABLoadIns == null)
            {
                UnityEngine.GameObject go = UnityEngine.GameObject.Find("ABLoadRoot");
                if (go == null)
                {
                    go = new UnityEngine.GameObject("ABLoadRoot");
                }
                _ABLoadIns = go.AddComponent<AssetBundleLoad>();
            }
            return _ABLoadIns;
        }
    }

    /// <summary>
    /// 是否为合并资源
    /// </summary>
    public static bool bResourceMerge = false;

    /// <summary>
    /// ab加载器
    /// </summary>
    private static AssetBundleBase mAssetBundleBase = null;

    /// <summary>
    /// 读取 ResourceInfoList (合并包使用)
    /// </summary>
    /// <param name="callBack"></param>
    public static void ReadConfig(System.Action callBack)
    {
        try
        {
            string strResMerge = Config.GetUpdaterConfig("ActionResourceUpdata", "ResourceMerge");
            bResourceMerge = ("1".Equals(strResMerge) || "true".Equals(strResMerge));
            if (bResourceMerge)
            {
                mAssetBundleBase = new MergeAssetbundle(ABLoadIns);
            }
            else
            {
                mAssetBundleBase = new NormAssetbundle(ABLoadIns);
            }

            CacheObjects.onremoveObject = RemoveObjectsDepends;
            mAssetBundleBase.ReadConfig(callBack);
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError("ReadConfig:", ex.ToString());
        }
        
    }

    public static void Update()
    {
        mAssetBundleBase.Update();
    }

    public static void PrintLog()
    {
        mAssetBundleBase.PrintLog();
    }

    // <summary>
    /// 将工程资源路径转换为资源包格式(转换大小写)
    /// </summary>
    /// <param name="strAsset"></param>
    /// <returns></returns>
    public static string GetAbPathFromResPath(string strAsset)
    {
        return mAssetBundleBase.GetAbPathFromResPath(strAsset);
    } 

    /// <summary>
    /// 异步加载资源列表
    /// </summary>
    /// <param name="strFileName"></param>
    /// <param name="lCallback"></param>
    /// <param name="strOrignFileName"></param>
    /// <param name="callback"></param>
    /// <param name="varStore"></param>
    /// <param name="bAsync"></param>
    /// <returns></returns>
    public static bool LoadAsset(string strFileName, OnLoadCallBack lCallback, string strOrignFileName, AssetCallback callback, VarStore varStore, bool bAsync = false)
    {
        return mAssetBundleBase.LoadAsset(strFileName, lCallback, strOrignFileName, callback, varStore, bAsync);
    }

 
    public static bool IsContainAsset(string strFileName)
    {
        return mAssetBundleBase.IsContainAsset(strFileName);
    }

	public static bool IsContainBundle(string bundleName)
	{
        return mAssetBundleBase.IsContainBundle(bundleName);
	}

        /// <summary>
    /// 删除加载出来的资源对象时，需要清除对应依赖包的引用计数
    /// </summary>
    public static void RemoveObjectsDepends(Object oAsset)
    {
        mAssetBundleBase.RemoveObjectsDepends(oAsset);
    }

    /// <summary>
    /// 当包系统清理的时候，去除所有依赖包
    /// </summary>
    public static void ClearAllDepends()
    {
        //mObjectsToDepends.Clear();
        //mDependsToTimes.Clear();
        //BundleCache.ClearAllBundleCache();
    }

    public static void Clear()
    {
        mABManifest = null;  
        //if(mResABInfoDic != null)
        //{
        //    mResABInfoDic.Clear();
        //}
        //mResABInfoDic = null;
    }

    /// <summary>
    /// 获取资源文件（只针对场景字节与Terrain文件）
    /// -资源包为合并包(小包)ab包没有后缀
    /// -资源包为散包时ab包后缀.unity
    /// </summary>
    /// <param name="strFileName">资源路径（注：不带.unity后缀路径）</param>
    /// <returns></returns>
    public static string GetResABPath(string strFileName)
    {
        return mAssetBundleBase.GetResABPath(strFileName);
    }
}
