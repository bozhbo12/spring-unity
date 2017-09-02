using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 资源包加载器
/// </summary>
public class AssetBundleLoad : MonoBehaviour
{
    private string mLocalRootPath = string.Empty;
    private string mStreamingAssetPath = string.Empty;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);


		if (Config.bEditor || Config.bWin || Config.bMac)
		{
			mLocalRootPath = Application.dataPath + "/../AssetBundle";
			mStreamingAssetPath = Application.dataPath + "/StreamingAssets";
		}
		else if (Config.bIPhone)
		{
			mLocalRootPath = Application.persistentDataPath;
			mStreamingAssetPath = Application.streamingAssetsPath;
		}
		else if (Config.bAndroid)
		{
			mLocalRootPath = Application.persistentDataPath;
			mStreamingAssetPath = Application.streamingAssetsPath;
		}



//#if UNITY_STANDALONE_WIN || UNITY_EDITOR
//        mLocalRootPath = UtilTools.StringBuilder(Application.dataPath, "/../AssetBundle");
//        mStreamingAssetPath = UtilTools.StringBuilder(Application.dataPath, "/StreamingAssets");
//#elif UNITY_IPHONE
//        mLocalRootPath = Application.persistentDataPath;
//        mStreamingAssetPath = Application.streamingAssetsPath;
//#elif UNITY_ANDROID
//        mLocalRootPath = Application.persistentDataPath;
//        mStreamingAssetPath = Application.streamingAssetsPath;
//#endif
    }

    void Update()
    {
        AssetBundleManager.Update();
    }

    /// <summary>
    /// 加载AB包
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="callback"></param>
    /// <param name="varStore"></param>
    public void LoadAB(string bundleName, AssetBundleCallBackDelegateProxy callback, VarStore varStore)
    {
        string strPath = Path.Combine(mLocalRootPath, bundleName);
        //文件是否在streamAsset中
        bool IsAtStreamAsset = !File.Exists(strPath);
        if (IsAtStreamAsset)
        {
            strPath = Path.Combine(mStreamingAssetPath, bundleName);
        }
        if (IsAtStreamAsset && Config.bAndroid)
        {
            StartCoroutine(LoadABToWWW(strPath, callback, varStore));
            return;
        }
        if (callback != null)
        {
            callback(AssetBundle.LoadFromFile(strPath), varStore);
        }
    }

    /// <summary>
    /// 安卓加载StreamAsset中资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="callback"></param>
    /// <param name="varStore"></param>
    /// <returns></returns>
    private IEnumerator LoadABToWWW(string path, AssetBundleCallBackDelegateProxy callback, VarStore varStore)
    {
        WWW www = WWW.LoadFromCacheOrDownload(path, 0);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            LogSystem.LogError("Bundle [", path, "] Error:", www.error);
            if (callback != null)
            {
                callback(null, varStore);
            }
        }
        else if (callback != null)
        {
            callback(www.assetBundle, varStore);
        }
        www.Dispose();
        www = null;
    }
   
    /// <summary>
    /// 加载 ResourceInfoList (合并包使用)
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="callback"></param>
    public void ReadResourceListXml(string resName, System.Action<string> callback)
    {        
        string strPath = UtilTools.StringBuilder(mLocalRootPath, resName);
        if (!File.Exists(strPath))
        {
            strPath = UtilTools.StringBuilder(mStreamingAssetPath, resName);
        }
        else
        {
            strPath = UtilTools.StringBuilder("File://", strPath);
        }
        StartCoroutine(LoadResourceList(strPath, callback));
    }

    private IEnumerator LoadResourceList(string path, System.Action<string> callback)
    {
        WWW www = new WWW(path);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            LogSystem.LogError("Bundle [", path, "] Error:", www.error);
            if (callback != null)
            {
                callback(null);
            }
        }
        else
        {
            if(callback != null)
            {
                callback(www.text);
            }
        }
        www.Dispose();
        www = null;
    }
}
