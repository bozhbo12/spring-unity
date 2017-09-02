using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
/**************************************************************************************************
 * 功能 ： 资源库的管理
 **************************************************************************************************/
public class AssetLibrary
{
    /// <summary> 
    /// key = 111001
    /// list<string> = [平台路径/resource/scene/111001,....]
    /// </summary>
    static private Dictionary<string, List<string>> dependsMap;

    /// <summary>
    /// Assetbundle下载目录(不含前缀)
    /// </summary>
    static public string downloadPath = "";

    /// <summary>
    /// streamAsset本地目录(不含前缀)
    /// </summary>
    static public string streamAssetsPath = "";

    public AssetLibrary()
    {
        //dependAsstsPaths.Add("file:///" + Application.persistentDataPath + "/scenes/1100004.unity3d");
    }

    static public AssetLibraryBundle getBundle()
    {
        return AssetLibraryBundle.getInstance();
    }

    /// <summary>
    /// 字面上添加依赖
    /// 传入场景所在目录，遍历目录下所有资源，将路径添加到dependsMap字典
    /// </summary>
    /// <param name="sceneDicPath">(包含前缀)平台路径/resource/scene/</param>
    static private void AddDepends(string sceneDicPath)
    {
        if (!Directory.Exists(sceneDicPath))
        {
            return;
        }
        string[] fileName = Directory.GetFiles(sceneDicPath);

        int iU3dIndex = -1;
        int iNameIndex = -1;
        string file = string.Empty;
        for (int i = 0; i < fileName.Length; i++)
        {
            file = fileName[i];
            iU3dIndex = file.IndexOf(".unity3d");
            if (iU3dIndex > 0)
            {
                iNameIndex = file.LastIndexOf("/");
                int length = iU3dIndex - iNameIndex;
                string filename = file.Substring(iNameIndex + 1, length - 1);
                if (filename.Equals("terrainTexture"))
                {
                    filename = "Textures/Terrain";
                }
                if (dependsMap!=null &&  !dependsMap.ContainsKey(filename))
                {
                    List<string> dps = new List<string>();
                    dps.Add(file);
                    dependsMap.Add(filename, dps);
                }
            }
        }
    }
    /***************************************************************************************
    * 功能 ：获取依赖资源列表
    ***************************************************************************************/
    static public List<string> FindDepends(string key)
    {
        
        if (dependsMap == null)
            return null;

        //if (dependsMap.ContainsKey(key))
        //    return dependsMap[key];

        //if (dependsMap == null)
        //{
        //    dependsMap = new Dictionary<string, List<string>>();

        //    AddDepends(Config.GetStreamSuffix()+downloadPath + "/Scenes/");

        //    AddDepends(Config.GetPreSuffix()+streamAssetsPath + "/Scenes/");

        //}

        foreach (var item in dependsMap)
        {
            if (key.Contains(item.Key))
            {
                return item.Value;
            }
        }

        return null;
    }

    /// <summary>
    /// 将场景路径传入(每次进入场景会调用)
    /// </summary>
    /// <param name="sceneID">场景资源id</param>
    /// <param name="fileName">前缀+平台路径+resources/scenes/111001</param>
    static public void InsertSceneDepends(string sceneID, string fileName)
    {
        if (dependsMap == null)
            dependsMap = new Dictionary<string, List<string>>(1);
        if (dependsMap.ContainsKey(sceneID) == false)
        {
            List<string> dps = new List<string>();
            dps.Add(fileName);
            dependsMap.Add(sceneID, dps);
        }
    }
   
    /***************************************************************************************
     * 功能 ：加载资源，
     * 如果资源已经在加载,或者未加载完返回false, 
     * 如果已经加载完则返回true
     ***************************************************************************************/
    static public Asset Load(string path, AssetType type, LoadType loadType = LoadType.Type_Resources)
    {
        Asset asset = getBundle().GetAsset(path);
        if (asset == null)
        {
            // 创建新的资源
            asset = new Asset(path, loadType, type);
            getBundle().AddAsset(asset);
            return asset;
        }
        return asset;
    }

    /// <summary>
    /// 功能 ：释放所有资源
    /// 场景被删除时调用
    /// </summary>
    static public void RemoveAllAsset()
    {
        getBundle().RemoveAllAssets();
    }
}
