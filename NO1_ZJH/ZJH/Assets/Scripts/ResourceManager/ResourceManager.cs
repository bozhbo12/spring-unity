using System;
using UnityEngine;
using System.Collections.Generic;
using TinyBinaryXml;

/// <summary>
/// 资源加载管理器，负责资源加载与缓存，需手动取消缓存
/// </summary>
public class ResourceManager
{
    /// <summary>
    /// 缓存资源加载回调
    /// </summary>
    private static Dictionary<string, List<AssetCallback>> mLoadedCallBack = new Dictionary<string, List<AssetCallback>>();

    /// <summary>
    /// 缓存技能物资路径列表
    /// </summary>
    //private static Dictionary<string/*job*/, List<string>/*路径*/> mJobEffectDic = new Dictionary<string, List<string>>();

 
 
    public delegate void ModelCallBackArgs(GameObject oModel, VarStore varStore);

    public delegate void ModelCallBack(GameObject oModel);
    public delegate void OnCallBack();
    /// <summary>
    /// 清理当前加载回调列表
    /// </summary>
    public static void Clear()
    {
        mLoadedCallBack.Clear();
    }
    //public static void LoadCacheConfig(string strFileName, OnCallBack callback)
    //{
    //    LoadAsset(strFileName, (UnityEngine.Object oAsset, string strFileName2, VarStore varStore)=>
    //    {
    //        TextAsset ta = oAsset as TextAsset;
    //        if (ta != null)
    //        {
    //            ParseCache(ta);

    //            UnloadAsset(oAsset, strFileName2);
    //        }
    
    //        if (callback != null)
    //            callback();
    //    });
    //}
  
    /// <summary>
    /// 设置职业与特效对应关系
    /// </summary>
    /// <param name="job"></param>
    /// <param name="path"></param>
    //public static void SetEffectInfo(string job, string path)
    //{
    //    List<string> paths;
    //    if (!mJobEffectDic.TryGetValue(job, out paths))
    //    {
    //        paths = new List<string>();
    //        mJobEffectDic.Add(job, paths);
    //    }
    //    if (!paths.Contains(path))
    //    {
    //        paths.Add(path);
    //    }
    //}

//    static void ParseCache(TextAsset tAsset)
//    {
//        TbXmlNode docNode = TbXml.Load(tAsset.bytes).docNode;
//        //List<TbXmlNode> nodelist = docNode.GetNodes("CacheAsset/Asset");

////         List<TbXmlNode> xmlNodeList = docNode.GetNodes("CacheAsset");
////         if (xmlNodeList != null)
////         {
////             ///解析首段中的类型定义
//// 
////             //int iCacheMax = xmlNodeList[0].GetIntValue("Cache");
////         }
//        List<TbXmlNode> nodelist = docNode.GetNodes("CacheAsset/Asset");
//        mJobEffectDic.Clear();
//        if (nodelist != null)
//        {
//            int iCount = nodelist.Count;
//            for (int i = 0; i < iCount; i++)// (TbXmlNode n in nodelist)
//            {
//                TbXmlNode n = nodelist[i];
//                string strName = n.GetStringValue("Module");

//                string strRun = n.GetStringValue("SRun");

//                string strJob = n.GetStringValue("Job");
//                if (!string.IsNullOrEmpty(strJob))
//                {
//                    SetEffectInfo(strJob, strName);
//                }
//                strRun = strRun.ToLower();
//                if (strRun.Equals("true"))
//                {
//                    ///启动缓存的对象,加载并缓存
//                    LoadAsset(strName, OnCacheFileLoaded);
//                }
//            }
//        }
//    }
    //static void OnCacheFileLoaded(UnityEngine.Object oAsset, string strFileName,VarStore varStore)
    //{
    //    UnityEngine.Object o = oAsset as UnityEngine.Object;
    //    if (o != null)
    //    {
    //        ///永久缓存对象
    //        CacheObjects.PushForeverCache(strFileName, o);
    //    }
    //    ///此处释放，缓存器中缓存
    //    UnloadAsset(oAsset, strFileName);
    //}

    /// <summary>
    /// 加载动作模型接口
    /// </summary>
    /// <param name="strModel"></param>
    /// <param name="strActionName"></param>
    /// <param name="callback"></param>
    /// <param name="bCacheObject"></param>
    public static void LoadModel(string strModel, ModelCallBackArgs callback, bool bCacheObject = true, VarStore varStore = null)
    {
        VarStore _varStore = VarStore.CreateVarStore();
        _varStore+=callback;
        _varStore+=varStore;
        LoadAsset(strModel, OnLoadeModelLoaded, _varStore);
    }

    public static void OnLoadeModelLoaded(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        if (varStore == null)
            return;

        UnityEngine.Object go = oAsset as UnityEngine.Object;
        VarStore _varStore = varStore.GetObject(1) as VarStore;
        ModelCallBackArgs callback = varStore.GetObject(0) as ModelCallBackArgs;

        if (callback == null)
        {
            CacheObjects.PopCache(oAsset);
            varStore.Destroy();
            return;
        }

        GameObject clone = null;
        if (go != null)
        {
            clone= CacheObjects.InstantiatePool(go, Vector3.zero, Quaternion.identity) as GameObject;
        }
        callback(clone, _varStore);
        varStore.Destroy();
    }

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="strFileName">资源全路径文件名</param>
    /// <param name="callback">完成后回调</param>
    public static void LoadAsset(string strFileName, AssetCallback callback ,VarStore varStore = null, bool bAsync = false)
    {
        if (string.IsNullOrEmpty(strFileName))
        {
            callback(null, strFileName, varStore);
            return;
        }
        CacheObjects.CacheInfo cacheInfo = CacheObjects.GetCacheObject(strFileName);
        if (cacheInfo != null)
        {
            ///再进入缓存池，只是添加引用次数
            CacheObjects.PushCache(strFileName, cacheInfo.oAsset);
            if (callback != null)
            {
                callback(cacheInfo.oAsset, strFileName, varStore);
            }
            return;
        }

        string strNewFileName = AssetBundleManager.GetAbPathFromResPath(strFileName);
        ///资源包中是否包含此文件
        if (AssetBundleManager.IsContainAsset(strNewFileName))//BundleManager.IsContainAsset(strNewFileName))
        {
            bool bBundleLoad = false;
            List<AssetCallback> list;
            ///是否有人正在加载
			if (!mLoadedCallBack.TryGetValue(strFileName, out list))
            {
                list = new List<AssetCallback>();
                list.Add(callback);
				mLoadedCallBack[strFileName] = list;

                ///发动加载
                bBundleLoad = AssetBundleManager.LoadAsset(strNewFileName, OnAssetLoaded, strFileName, callback, varStore, bAsync); 
                    //BundleManager.LoadAsset(strNewFileName, OnAssetLoaded,strFileName,callback, varStore);
            }
            else
            {
                list.Add(callback);
                bBundleLoad = true;
            }

            ///包系统中无此资源或加载失败，改用本地加载
            if (!bBundleLoad)
            {
                ResourceLoadAsync.Instance.Load(strFileName, strFileName, onAssetAsyncLoaded, callback, varStore);
            }
        }
        else
        {
            //ResourceLoadAsync.Instance.Load(strFileName, onAssetAsyncLoaded, callback);
           // long lTick = DateTime.Now.Ticks;
            UnityEngine.Object o = Resources.Load(strFileName);
			onLocalAssetLoaded(strFileName, o, callback, varStore);
          //  long lTick2 = DateTime.Now.Ticks;
           // LogSystem.LogWarning("Resources.Load\t",strFileName,"\t", (lTick2 - lTick)/10000,Time.realtimeSinceStartup);
        }
    }
    private static void onAssetAsyncLoaded(string filename, UnityEngine.Object o,AssetCallback assetCall, VarStore varStore)
    {
        if (o != null)
        {
            /// 已加载并缓存
            CacheObjects.PushCache(filename, o);
        }
      
        if (assetCall != null)
        {
            assetCall(o, filename, varStore);
        }
    }

    /// <summary>
    /// 本地资源加载回调
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="o"></param>
    /// <param name="callback"></param>
    /// <param name="varStore"></param>
    private static void onLocalAssetLoaded(string filename, UnityEngine.Object o, AssetCallback callback, VarStore varStore )
    {
        if (o != null)
        {
            /// 已加载并缓存
            CacheObjects.PushCache(filename, o);
        }
      
        if (callback != null)
        {
            callback(o, filename, varStore);
        }
    }
    /// <summary>
    /// 包系统资源加载返回
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="args"></param>
    private static void OnAssetLoaded(UnityEngine.Object oAsset,string strOrignFileName, AssetCallback callback,VarStore varStore)
    {
        if (!mLoadedCallBack.ContainsKey(strOrignFileName))
        {
            try
            {
                if (callback != null && AsyncTrigger.IsTargetValid(callback.Target))
                {
                    if (oAsset != null)
                    {
                        ///缓存加载对象
                        CacheObjects.PushCache(strOrignFileName, oAsset);
                    }
                    callback(oAsset, strOrignFileName, varStore);
                }
            }
            catch (Exception ex)
            {
                ///出现异常，先将回调列表删除
                LogSystem.LogError("Asset[", strOrignFileName, "]callback Error!", ex.ToString());
            }
        }
        else
        {
            List<AssetCallback> callList = mLoadedCallBack[strOrignFileName];

            int count = callList.Count;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    AssetCallback aCall = callList[i];
                    if (aCall != null && AsyncTrigger.IsTargetValid(aCall.Target))
                    {
                        if (oAsset != null)
                        {
                            ///缓存加载对象
                            CacheObjects.PushCache(strOrignFileName, oAsset);
                        }
                        aCall(oAsset, strOrignFileName, varStore);
                    }
                }
                catch (Exception ex)
                {
                    ///出现异常，先将回调列表删除
                    LogSystem.LogError("Asset[", strOrignFileName, "]callback Error!", ex.ToString());
                }
            }
            mLoadedCallBack.Remove(strOrignFileName);
        }
        oAsset = null;
    }
    /// <summary>
    /// 卸载资源对象,使用动态缓存机制，此接口暂时失效
    /// </summary>
    /// <param name="strFileName">资源名</param>
    public static void UnloadAsset(UnityEngine.Object oAsset,string strFileName = "")
    {
        if (oAsset != null)
        {
            Type type = oAsset.GetType();
            if (type == typeof(TextAsset))
            {
                //textAsset不在缓存池中
                CacheObjects.PopCache(oAsset);
                Resources.UnloadAsset(oAsset);
            }
        }
    }
}