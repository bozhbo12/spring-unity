using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 合并包加载功能实现
/// </summary>
public class MergeAssetbundle : AssetBundleBase
{

    /// <summary>
    /// 通用资源
    /// </summary>
    public static string[] commonRes = new string[]{"res/allsky.unity3d", 
													"res/effect.unity3d", 
													"res/fonts.unity3d", 
													"res/model.unity3d", 
													"res/model0.unity3d", 
													"res/scenes.unity3d",
													"res/textures.unity3d",
													"resources/action/role.unity3d",
                                                    "resources/config.unity3d",//不卸载目的:加载资源包下载速度
													"resources/shaders.unity3d",
													"resources/sounds.unity3d",
													"resources/textures.unity3d",
													"resources/textures112.unity3d",
													"resources/prefabs/effect.unity3d",
													"resources/action/npc.unity3d",
													"resources/action/role.unity3d",
													"resources/action/horse.unity3d",
													"resources/prefabs/ui.unity3d"};

    private string mResConfigName = "/ResourceInfoList.xml";

    /// <summary>
    /// 描述资源对应的资源包信息(合并包使用)(Sounds/amb/sp -> (AssetBundleInfo))
    /// </summary>
    private Dictionary<string/**资源路径 如:Sounds/amb/sp*/, AssetBundleInfo> mResABInfoDic = null;

    /// <summary>
    /// 异步加载资源列表
    /// </summary>
    private List<ABRequestLoadInfo> listABRequestListInfo = new List<ABRequestLoadInfo>(16);


    public MergeAssetbundle(AssetBundleLoad abLoad)
        : base(abLoad)
    {

    }

    /// <summary>
    /// 读取资源
    /// </summary>
    /// <param name="callBack"></param>
    public override void ReadConfig(System.Action callBack)
    {
        mABLoad.ReadResourceListXml(mResConfigName, (string strContent) =>
        {
            if (ParsingResListXml(strContent, ref mResABInfoDic))
            {
                ReadABConfig(callBack);
            }
            else
            {
                if (callBack != null)
                    callBack();
                LogSystem.LogWarning("ResourceInfo.xml Erorr!!", strContent);
            }
        });
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
                assetBundle.Unload(false);
                assetBundle = null;
            }
            if (callBack != null)
            {
                callBack();
            }
        }, null);
    }

    /// <summary>
    /// 获取资源路径(合并包原路径返回无需转换)
    /// </summary>
    /// <param name="strResPath"></param>
    /// <returns></returns>
    public override string GetAbPathFromResPath(string strResPath)
    {
        return strResPath;
    }

    public override bool LoadAsset(string strFileName, OnLoadCallBack lCallback, string strOrignFileName, AssetCallback callback, VarStore varStore, bool bAsync = false)
    {
        try
        {
            if (mABManifest == null || !mResABInfoDic.ContainsKey(strFileName))
            {
                LogSystem.LogWarning("M::LoadAsset:abInfo is null!", strFileName);
                if (lCallback != null)
                {
                    lCallback(null, strOrignFileName, callback, varStore);
                }
                return false;
            }

            AssetBundleInfo abInfo = mResABInfoDic[strFileName];
            if (abInfo == null)
            {
                LogSystem.LogWarning("M::LoadAsset:abInfo is null!", strFileName);
                if (lCallback != null)
                {
                    lCallback(null, strOrignFileName, callback, varStore);
                }
            }
            Hash128 hash = mABManifest.GetAssetBundleHash(abInfo.ABName);
            if (!hash.isValid)
            {
                if (lCallback != null)
                {
                    lCallback(null, strOrignFileName, callback, varStore);
                }
                return false;
            }
            AssetBundle mMainAB = null;
            //a b c 三个资源, ab在被打在1包,c被打在2包中.
            //假如先加载a资源，1包加载进内存 ，再去加载b资源时发现1包在内存中，就直接去获取b资源，但是依赖的2包不会被加载
            //AssetBundle mMainAB = GetAssetbundleByAbName(abInfo.ABName);
            //if (mMainAB != null)
            //{
            //    PushCacheInfo(abInfo.ABName, mMainAB);
            //    if (bAsync)
            //    {
            //        AssetBundleRequest abRequest = mMainAB.LoadAssetAsync(abInfo.AssetName);
            //        ABRequestLoadInfo abLoadInfo = ABRequestLoadInfo.GetAbRequestLoadInfo(abRequest, lCallback, strOrignFileName, callback, varStore, true, abInfo.ABName);
            //        listABRequestListInfo.Add(abLoadInfo);
            //    }
            //    else
            //    {
            //        obj = mMainAB.LoadAsset(abInfo.AssetName);
            //        PushObjects(obj, abInfo.ABName);
            //        if (lCallback != null)
            //        {
            //            lCallback(obj, strOrignFileName, callback, varStore);
            //        }
            //    }
            //    return true;
            //}

            int finishedCount = 0;
            int count = 0;
            string[] arrAllDepend = mABManifest.GetAllDependencies(abInfo.ABName);
            if (arrAllDepend != null && arrAllDepend.Length > 0)
            {
                count = arrAllDepend.Length;
                AssetBundle abDepend = null;
                for (int i = 0; i < count; ++i)
                {
                    string strDepend = arrAllDepend[i];
                    //从缓存中取资源
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
                            LogSystem.LogWarning("BundleManager.LoadAsset 加载依赖资源失败strFileName：" + strFileName + "     strDepend:" + strDepend);
                        }

                        finishedCount++;
                        if (finishedCount == count + 1)
                        {
                            LoadAssetObject(mMainAB, abInfo, lCallback, strOrignFileName, callback, _varStore, bAsync);
                        }
                    }, varStore);
                }
            }

            mMainAB = GetAssetbundleByAbName(abInfo.ABName);
            if (mMainAB != null)
            {
                PushCacheInfo(abInfo.ABName, mMainAB, arrAllDepend);
                finishedCount++;
                if (finishedCount == count + 1)
                {
                    LoadAssetObject(mMainAB, abInfo, lCallback, strOrignFileName, callback, varStore, bAsync);
                }
            }
            else
            {
                mABLoad.LoadAB(abInfo.ABName, (AssetBundle ab, VarStore _varStore) =>
                {
                    mMainAB = ab;
                    PushCacheInfo(abInfo.ABName, mMainAB, arrAllDepend);
                    finishedCount++;
                    if (finishedCount == count + 1)
                    {
                        LoadAssetObject(mMainAB, abInfo, lCallback, strOrignFileName, callback, _varStore, bAsync);
                    }
                }, varStore);
            }
            
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

    private void TestPrint(string[] strDepend, string fileName)
    {
        for (int i = 0; i < strDepend.Length; i++)
        {
            if (strDepend[i].Contains("221") || strDepend[i].Contains("33") || strDepend[i].Contains("allsky"))
            {
                //Debug.Log("testPrint:" + fileName);
            }
        }
    }
    
    /// <summary>
    /// 从ab包中加载资源
    /// </summary>
    /// <param name="mMainAB"></param>
    /// <param name="abInfo"></param>
    /// <param name="lCallback"></param>
    /// <param name="strOrignFileName"></param>
    /// <param name="callback"></param>
    /// <param name="varStore"></param>
    /// <param name="bAsync"></param>
    private void LoadAssetObject(AssetBundle mMainAB, AssetBundleInfo abInfo, OnLoadCallBack lCallback, string strOrignFileName, AssetCallback callback, VarStore varStore, bool bAsync = false)
    {
        if (mMainAB == null)
        {
            if (lCallback != null)
            {
                lCallback(null, strOrignFileName, callback, varStore);
            }
            return;
        }

        if (bAsync)
        {
            AssetBundleRequest abRequest = mMainAB.LoadAssetAsync(abInfo.AssetName);
            ABRequestLoadInfo abLoadInfo = ABRequestLoadInfo.GetAbRequestLoadInfo(abRequest, lCallback, strOrignFileName, callback, varStore, abInfo.ABName);
            listABRequestListInfo.Add(abLoadInfo);
        }
        else
        {
            UnityEngine.Object obj = mMainAB.LoadAsset(abInfo.AssetName);
            PushObjects(obj, abInfo.ABName);
            if (lCallback != null)
            {
                lCallback(obj, strOrignFileName, callback, varStore);
            }
        }
    }

    /// <summary>
    /// 是否包含此资源
    /// </summary>
    /// <param name="strFileName">Sounds/amb/sp</param>
    /// <returns></returns>
    public override bool IsContainAsset(string strFileName)
    {
        if (mResABInfoDic == null)
            return false;

        return mResABInfoDic.ContainsKey(strFileName);
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

    protected override bool RemoveAssetBundle(AssetBundle bundle)
    {

        return base.RemoveAssetBundle(bundle);
    }

    /// <summary>
    /// 是否是通用资源
    /// </summary>
    /// <param name="strName"></param>
    /// <returns></returns>
    protected override bool IsCommonRes(string strABName)
    {
        if (!string.IsNullOrEmpty(strABName) && commonRes != null)
        {
            for (int i = 0; i < commonRes.Length; i++)
            {
                if (strABName.Equals(commonRes[i]))
                    return true;
            }
        }
        return false;
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
        return strFileName;
    }

    public override void Update()
    {
        if (listABRequestListInfo.Count == 0)
            return;

        ABRequestLoadInfo abRequestInfo;
        for (int i = listABRequestListInfo.Count - 1; i >= 0; i--)
        {
            abRequestInfo = listABRequestListInfo[i];
            if (abRequestInfo == null)
            {
                listABRequestListInfo.RemoveAt(i);
                continue;
            }

            if (abRequestInfo.bAbRequestLoadOK)
            {
                PushObjects(abRequestInfo.oAsset, abRequestInfo.mstrABName);
                abRequestInfo.Execute();
                listABRequestListInfo.RemoveAt(i);
                abRequestInfo.Dispose();
            }
        }
    }

    /// <summary>
    /// ResourceInfo 资源解析
    /// </summary>
    /// <param name="xml"></param>
    /// <param name="resInfoList"></param>
    private bool ParsingResListXml(string xml, ref Dictionary<string, AssetBundleInfo> resInfoList)
    {
        if (string.IsNullOrEmpty(xml))
        {
            LogSystem.LogWarning("ResourceInfo.xml Error!!! - 1");
            return false;
        }

        if (resInfoList == null)
        {
            resInfoList = new Dictionary<string, AssetBundleInfo>();
        }
        resInfoList.Clear();

        int index = xml.IndexOf('<');
        xml = xml.Substring(index);
        XMLParser parse = new XMLParser();
        XMLNode rootNode = parse.Parse(xml);
        if (rootNode == null)
        {
            LogSystem.LogWarning("ResourceInfo.xml Error!!! - 2");
            return false;
        }

        XMLNodeList xmlNodeList = (XMLNodeList)rootNode["Assets"];
        if (xmlNodeList == null)
        {
            LogSystem.LogWarning("ResourceInfo.xml Error!!! - 3");
            return false;
        }

        ///解析首段中的类型定义
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            XMLNode xmlnode = xmlNodeList[i] as XMLNode;
            if (xmlnode == null)
                continue;

            XMLNodeList childNodeList = xmlnode.GetNodeList("Assets");
            if (childNodeList != null)
            {
                for (int j = 0; j < childNodeList.Count; j++)
                {
                    XMLNode childnode = childNodeList[j] as XMLNode;
                    if (childnode == null)
                        continue;
                    AssetBundleInfo abInfo = new AssetBundleInfo();
                    abInfo.AssetName = childnode.GetValue("@Asset");
                    abInfo.ABName = childnode.GetValue("@BName").ToLower();
                    string resPath = abInfo.AssetName;
                    resPath = resPath.Replace("Assets/Resources/", string.Empty);
                    string[] arrRes = resPath.Split('.');
                    if (arrRes != null || arrRes.Length == 2)
                    {
                        if (resInfoList.ContainsKey(arrRes[0]))
                        {
                            resInfoList[arrRes[0]] = abInfo;
                            LogSystem.LogWarning("AssetbundleInfo key repeat!!!", arrRes[0]);
                        }
                        else
                        {
                            resInfoList.Add(arrRes[0], abInfo);
                        }
                        
                    }

                }
            }
        }
        return true;
    }


    class AssetBundleInfo
    {
        /// <summary>
        /// 资源所在的资源包 如:resources/action/cg.unity3d
        /// </summary>
        public string ABName = string.Empty;

        /// <summary>
        /// 资源在资源包中路径(从资源包中取资源使用) 如:Assets/Resources/Action/CG/Boss_033/CG2.anim
        /// </summary>
        public string AssetName = string.Empty;
    }

    public class ABRequestLoadInfo : CacheObject
    {
        /// <summary>
        /// 生成AbRequest
        /// </summary>
        /// <param name="abRequest"></param>
        /// <param name="abRequest"></param>
        /// <param name="lCallback">异步读取完成回调</param>
        /// <param name="strOrignFileName">真实路径</param>
        /// <param name="callback">回调参数</param>
        /// <param name="varStore">回调参数</param>
        /// <param name="ABName">引用路径</param>
        /// <returns></returns>
        public static ABRequestLoadInfo GetAbRequestLoadInfo(AssetBundleRequest abRequest, OnLoadCallBack lCallback, string strOrignFileName, AssetCallback callback, VarStore varStore, string ABName)
        {
            ABRequestLoadInfo abLoadInfo = CacheObjects.SpawnCache<ABRequestLoadInfo>();
            abLoadInfo.mABRequest = abRequest;
            abLoadInfo.mstrOrignFileName = strOrignFileName;
            abLoadInfo.mlCallback = lCallback;
            abLoadInfo.mCallback = callback;
            abLoadInfo.mVarStore = varStore;
            abLoadInfo.mstrABName = ABName;
            return abLoadInfo;
        }

        public string mstrOrignFileName = string.Empty;
        public AssetCallback mCallback = null;
        public OnLoadCallBack mlCallback = null;
        public VarStore mVarStore;
        public AssetBundleRequest mABRequest = null;
        public string mstrABName = string.Empty;

        public bool bAbRequestLoadOK
        {
            get
            {
                if (mABRequest == null)
                {
                    return true;
                }
                return mABRequest.isDone;
            }
        }

        public UnityEngine.Object oAsset
        {
            get
            {
                if (mABRequest == null)
                    return null;

                if (!mABRequest.isDone)
                    return null;

                return mABRequest.asset;
            }
        }

        public void Execute()
        {
            if (mlCallback != null)
            {
                UnityEngine.Object oAsset = null;
                if (mABRequest != null)
                {
                    oAsset = mABRequest.asset;
                }
                mlCallback(oAsset, mstrOrignFileName, mCallback, mVarStore);
            }
        }

        public void Dispose()
        {
            mlCallback = null;
            mstrOrignFileName = string.Empty;
            mCallback = null;
            mVarStore = null;
            mABRequest = null;
            CacheObjects.DespawnCache(this);
        }
    }
}