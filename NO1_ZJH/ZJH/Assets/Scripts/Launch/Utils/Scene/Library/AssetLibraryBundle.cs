using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 类 ： 资源库绑定
/// </summary>
public class AssetLibraryBundle
{
    /// <summary>
    /// key 资源路径 Scenes/111001/lightmap/LightmapFar-i
    /// value Asset
    /// </summary>
    private Dictionary<string, Asset> _assetDictionary = new Dictionary<string, Asset>();

    static private AssetLibraryBundle _ins;
    public int count
    {
        get
        {
            return _assetDictionary.Count;
        }
    }

    /// <summary>
    /// 功能 : 重置
    /// </summary>
    public void Reset()
    {
        if(_assetDictionary.Count == 0)
            return;

        List<Asset> list = new List<Asset>(_assetDictionary.Values);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].Release();
        }
        _assetDictionary.Clear();
    }

    /// <summary>
    /// 功能 : 获取实例
    /// </summary>
    /// <returns></returns>
    public static AssetLibraryBundle getInstance()
    {
        if (_ins == null)
        {
            _ins = new AssetLibraryBundle();
        }
        return _ins;
    }

    /// <summary>
    /// 功能 : 获取资源
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Asset GetAsset(string path)
    {
        if (_assetDictionary.ContainsKey(path))
            return _assetDictionary[path];
        return null;
    }

    /// <summary>
    /// 功能 : 添加资源
    /// </summary>
    /// <param name="asset"></param>
    public void AddAsset(Asset asset)
    {
        _assetDictionary[asset.assetPath] = asset;
    }

    /// <summary>
    /// 功能 : 删除资源
    /// </summary>
    /// <param name="asset"></param>
    public void RemoveAsset(Asset asset)
    {
        if (_assetDictionary != null  &&  _assetDictionary.ContainsKey(asset.assetPath))
        {
            _assetDictionary.Remove(asset.assetPath);
        }
    }

    /// <summary>
    /// 功能 : 销毁所有资源
    /// </summary>
    public void RemoveAllAssets()
    {
        Reset();
    }
}