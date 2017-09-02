using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 异步加载Resource下的资源,
/// 除loading，初始化等特殊阶段，建议使用此代替Resource.Load
/// </summary>
public class ResourceLoadAsync : MonoBehaviour {
    private static ResourceLoadAsync instance;
    public static ResourceLoadAsync Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("AsyncLoad");
                instance = obj.AddComponent<ResourceLoadAsync>();
                UnityEngine.Object.DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
    public delegate void OnResourceLoadCallBack(string filename, UnityEngine.Object o, AssetCallback assetcall, VarStore varStore);
    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="callback"></param>
    public void Load(string filename ,string strOrignFileName, OnResourceLoadCallBack Localcallback, AssetCallback assetcall, VarStore varStore)
    {
        LoadContext context = new LoadContext();
        context.varStore = varStore;
        context.callback = Localcallback;
        context.assetcall = assetcall;
        //context.strFileName = filename;
        context.LoadAsset(filename, strOrignFileName);
        mWaitContext.Add(context);
        //StartCoroutine(LoadAsste(filename, callback, args));
    }
    List<LoadContext> mWaitContext = new List<LoadContext>();
    void LateUpdate()
    {
        if ( mWaitContext.Count== 0)
            return;

        for (int i = 0; i < mWaitContext.Count; i++)
        {
            LoadContext LoadContext = mWaitContext[i];
            if (LoadContext.CheckAssetLoaded())
            {
                mWaitContext.Remove(LoadContext);
                i--;
                continue;
            }
        }
    }
   
    public class LoadContext
    {
        public string strFileName = string.Empty;
        public OnResourceLoadCallBack callback = null;
        public AssetCallback assetcall = null;
        public VarStore varStore = null;
        private string strOrignFileName = string.Empty;
        private ResourceRequest resourceRequest = null;
        public void LoadAsset(string strFileName, string strOrignFileName)
        {
            this.strFileName = strFileName;
            this.strOrignFileName = strOrignFileName;
            resourceRequest = Resources.LoadAsync(strFileName);
        }
        public bool CheckAssetLoaded()
        {
            if ( resourceRequest == null )
                return false;

            if (resourceRequest.isDone)
            {
                if (callback != null)
                {
                    callback(strOrignFileName, resourceRequest.asset, assetcall, varStore);
                }
                return true;
            }
           
            return false;
        }
    }
    
}

public class AssetCallBack
{
}