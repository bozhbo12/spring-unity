using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 合并包加载功能实现
/// </summary>
public class NormAssetbundle : AssetBundleBase 
{
    /// <summary>
    /// resources/scenes/110001/prefabs/110001_01.unity3d, 110001_01
    /// </summary>
    private Dictionary<string, string> mBundleTable = new Dictionary<string, string>();

    /// <summary>
    /// scenes/110001/prefabs/110001_01, resources/scenes/110001/prefabs/110001_01.unity3d
    /// </summary>
    private Dictionary<string, string> mAssetTable = new Dictionary<string, string>();

    /// <summary>
    /// 所有资源的小写路径 scenes/110001/prefabs/110001_01
    /// </summary>
    private List<string> mAllAssetTable = new List<string>();

    private StringComparer strCompare = new StringComparer();

    /// <summary>
    /// 将工程资源路径转换为资源包格式(转换大小写)
    /// Sounds/amb/amb_23cheer_sp -> sounds/amb/amb_23cheer_sp
    /// </summary>
    /// <param name="strAsset"></param>
    /// <returns></returns>
    public override string GetAbPathFromResPath(string strResPath)
    {
        int iIndex = mAllAssetTable.BinarySearch(strResPath, strCompare);
        if (iIndex > -1)
        {
            return mAllAssetTable[iIndex];
        }
        return string.Empty;
    }

    public class StringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.Compare(x, y, true);
        }
    }

    public NormAssetbundle(AssetBundleLoad abLoad)
        : base(abLoad)
        
    {

    }

    public override void ReadConfig(System.Action callBack)
    {
        ReadABConfig(callBack);
    }

    /// <summary>
    /// 读取assetbundle配置文件
    /// </summary>
    /// <param name="callBack"></param>
    private void ReadABConfig(System.Action callBack)
    {
        mABLoad.LoadAB(AssetBundleManager.mABConfigName, (AssetBundle assetBundle, VarStore varStore) =>
        {
            if (assetBundle != null)
            {
                mABManifest = assetBundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                string[] arrAllAssetBundle = mABManifest.GetAllAssetBundles();
                assetBundle.Unload(false);
                assetBundle = null;
                if (arrAllAssetBundle != null && arrAllAssetBundle.Length > 0)
                {
                    for (int i = 0; i < arrAllAssetBundle.Length; i++)
                    {
                        //resources/scenes/110001/prefabs/110001_01.unity3d
                        string strAsset = arrAllAssetBundle[i];

                        //resources/scenes/110001/prefabs/110001_01
                        strAsset = strAsset.Replace(".unity3d", string.Empty);
                        int index = strAsset.LastIndexOf("/");
                        if (index > -1)
                        {
                            string strSubAsset = strAsset.Substring(index + 1);
                            //resources/scenes/110001/prefabs/110001_01.unity3d, 110001_01
                            mBundleTable.Add(arrAllAssetBundle[i], strSubAsset);
                        }
                        else
                        {
                            //resources/scenes/110001/prefabs/110001_01.unity3d, resources/scenes/110001/prefabs/110001_01
                            mBundleTable.Add(arrAllAssetBundle[i], strAsset);
                        }

                        if (strAsset.StartsWith("resources/"))
                        {
                            //scenes/110001/prefabs/110001_01
                            strAsset = strAsset.Replace("resources/", string.Empty);
                            //scenes/110001/prefabs/110001_01, resources/scenes/110001/prefabs/110001_01.unity3d
                            if (mAssetTable.ContainsKey(strAsset))
                            {
                                mAssetTable[strAsset] = arrAllAssetBundle[i];
                                LogSystem.LogWarning("Key::Repeat!!", strAsset);
                            }
                            else
                            {
                                mAssetTable.Add(strAsset, arrAllAssetBundle[i]);
                            }
                            
                            
                            //scenes/110001/prefabs/110001_01
                            mAllAssetTable.Add(strAsset);
                        }
                        else
                        {
                            mAssetTable.Add(strAsset, arrAllAssetBundle[i]);
                            mAllAssetTable.Add(strAsset);
                        }
                    }
                    mAllAssetTable.Sort();
                }
            }
            if (callBack != null)
            {
                callBack();
            }
        }, null);
    }

    public override bool LoadAsset(string strFileName, OnLoadCallBack lCallback, string strOrignFileName, AssetCallback callback, VarStore varStore, bool bAsync = false)
    {
        try
        {
            if (mABManifest == null || !mAssetTable.ContainsKey(strFileName))
            {
                if (lCallback != null)
                {
                    lCallback(null, strOrignFileName, callback, varStore);
                }
                return false;
            }
            //strBundleName = resources/scenes/110001/prefabs/110001_01.unity3d
            string strBundleName = mAssetTable[strFileName];
            Hash128 hash = mABManifest.GetAssetBundleHash(strBundleName);
            if (!hash.isValid)
            {
                if (lCallback != null)
                {
                    lCallback(null, strOrignFileName, callback, varStore);
                }
                return false;
            }

            UnityEngine.Object obj = null;
            AssetBundle mMainAB = GetAssetbundleByAbName(strBundleName);
            if (mMainAB != null)
            {
                //理论上不应该走到这里面，因为CacheObject中oAsset与这里面的bundle应该是一对一的关系
                //07/25 注:出现在这种情况是 某个资源包即是依赖包又是主资源包
                //LogSystem.LogWarning("Warning:", strBundleName);
                obj = LoadAsset(strBundleName, mMainAB);
                if (lCallback != null)
                {
                    lCallback(obj, strOrignFileName, callback, varStore);
                }
                return true;
            }

            int finishedCount = 0;
            int count = 0;
            string[] arrAllDepend = mABManifest.GetAllDependencies(strBundleName);
            if (arrAllDepend != null && arrAllDepend.Length > 0)
            {
                count = arrAllDepend.Length;
                AssetBundle abDepend = null; 
                for (int i = 0; i < count; ++i)
                {
                    string strDepend = arrAllDepend[i];
                    abDepend = GetAssetbundleByAbName(strDepend);
                    if (abDepend != null)
                    {
                        PushCacheInfo(strDepend, abDepend);
                        finishedCount++;
                        continue;
                    }
                    mABLoad.LoadAB(strDepend, (AssetBundle ab, VarStore _varStore) =>
                    {
                        if (ab != null)
                        {
                            PushCacheInfo(strDepend, ab);
                        }
                        else
                        {
                            LogSystem.LogWarning("NormAssetbundle.LoadAsset 加载依赖资源失败strFileName：" + strFileName + "     strDepend:" + strDepend);
                        }

                        finishedCount++;
                        if (finishedCount == count + 1)
                        {
                            obj = LoadAsset(strBundleName, mMainAB);
                            PushObjects(obj, strBundleName);
                            if (lCallback != null)
                            {
                                lCallback(obj, strOrignFileName, callback, _varStore);
                            }
                        }
                    }, varStore);
                }
            }

            mABLoad.LoadAB(strBundleName, (AssetBundle ab, VarStore _varStore) =>
            {
                mMainAB = ab;
                PushCacheInfo(strBundleName, mMainAB, arrAllDepend);
                finishedCount++;
                if (finishedCount == count + 1)
                {
                    obj = LoadAsset(strBundleName, mMainAB);
                    PushObjects(obj, strBundleName);
                    if (lCallback != null)
                    {
                        lCallback(obj, strOrignFileName, callback, _varStore);
                    }
                }
            }, varStore);
        }
        catch (System.Exception ex)
        {
            LogSystem.LogWarning(ex.ToString());
            if (lCallback != null)
            {
                lCallback(null, strOrignFileName, callback, varStore);
            }
            return false;
        }
        return true;
    }

    /// <summary>
    /// 是否包含此资源
    /// </summary>
    /// <param name="strFileName">scenes/110001/prefabs/110001_01</param>
    /// <returns></returns>
    public override bool IsContainAsset(string strFileName)
    {
        return mAssetTable.ContainsKey(strFileName);
    }

    public override bool IsContainBundle(string bundleName)
    {
        if (mABManifest == null)
            return false;

        Hash128 hash = mABManifest.GetAssetBundleHash(bundleName);
        return hash.isValid;
    }

    /// <summary>
    /// 移除asset资源
    /// </summary>
    /// <param name="oAsset"></param>
    public override void RemoveObjectsDepends(UnityEngine.Object oAsset)
    {
        PopCacheInfo(oAsset);
    }

    /// <summary>
    /// 从Assetbundle加载资源
    /// </summary>
    /// <param name="strPath">resources/scenes/110001/prefabs/110001_01.unity3d</param>
    /// <param name="assetBundle"></param>
    /// <returns></returns>
    private UnityEngine.Object LoadAsset(string strPath, AssetBundle assetBundle)
    {
        if (assetBundle == null)
        {
            return null;
        }

        if (!mBundleTable.ContainsKey(strPath))
        {
            return null;
        }

        string strAssetName = mBundleTable[strPath];
        return assetBundle.LoadAsset(strAssetName);
    }

    /// <summary>
    /// 获取资源文件（只针对场景字节与Terrain文件）
    /// -资源包为合并包(小包)ab包没有后缀
    /// -资源包为散包时ab包后缀.unity
    /// </summary>
    /// <param name="strFileName">资源路径（注：不带.unity后缀路径）</param>
    /// <returns></returns>
    public override string GetResABPath(string strFileName)
    {
        return UtilTools.StringBuilder(strFileName, AssetBundleManager.strAssetTail);
    }
}



