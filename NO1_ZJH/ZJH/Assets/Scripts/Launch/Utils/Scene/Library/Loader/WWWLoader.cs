using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*******************************************************************************************************
 * 类 : 资源加载
 *******************************************************************************************************/
public class WWWLoader : LoaderBase
{
    /********************************************************************************
     * 功能 : 资源释放
     ********************************************************************************/
    override public void Release()
    {
        _region = null;
        _scene = null;
        _terrain = null;
        _texture = null;
        _gameObject = null;
        _mesh = null;

        if (_assetBundle != null)
        {
            _assetBundle.Unload(true);
            _assetBundle = null;
        }
    }

    /********************************************************************************
     * 功能 : 异步加载资源
     ********************************************************************************/
    override public void Load()
    {
        //已经加载过资源
        if (_assetBundle != null)
            return;

        DelegateProxy.LoadAssetBundle(asset.assetPath, LoadAssetBundleComplete, null);
        //GameScene.StaticInsRoot.StartCoroutine(wwwLoad());
    }

    /// <summary>
    /// 加载完成
    /// </summary>
    /// <param name="ab"></param>
    /// <param name="varStore"></param>
    private void LoadAssetBundleComplete(AssetBundle ab, VarStore varStore)
    {
        if (ab == null)
        {
            LogSystem.LogWarning("www load asset.assetPath " + asset.assetPath + " error !!!");
            return;
        }
        _assetBundle = ab;
        if (asset.loadedListener != null)
        {
            asset.loadedListener(asset);
        }
    }

    /** 判断资源是否加载完毕 */
    override public bool loaded
    {
        get
        {
            return _assetBundle != null;
        }
    }

    /** 获取资源包资源 */
    override public AssetBundle assetBundle
    {
        get
        {
            return _assetBundle;
        }
    }

    /** 获取纹理 */
    override public Texture2D texture
    {
        get
        {
            LogSystem.LogWarning("not support !!!!");
            return null;
        }
    }
}