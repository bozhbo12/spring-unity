using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/***********************************************************************************************
 * 功能 ： LOD地形数据
 ***********************************************************************************************/
public class Splat:CacheObject
{

    public static Splat GetSplat()
    {
        return CacheObjects.SpawnCache<Splat>();
    }

    /// <summary>
    /// 地形纹理
    /// </summary>
    public Texture2D texture;

    /// <summary>
    /// 是否加载过
    /// </summary>
    private bool mbTexLoaded = false;

    /// <summary>
    /// texture贴图路径
    /// </summary>
    public string key = "";

    /// <summary>
    /// 贴图的平铺和偏移值
    /// </summary>
    public Vector4 tilingOffset = new Vector4(2f, 2f, 0f, 0f);

    /// <summary>
    /// 法线图
    /// </summary>
    private Texture2D _normalMap;

    /// <summary>
    /// 是否加载过
    /// </summary>
    private bool mbTexNmpLoaded = false;

    /// <summary>
    /// 是否需要加载法线
    /// </summary>
    private bool mbNeedLoadNmp = false;

    /// <summary>
    /// 地形纹理的法线
    /// </summary>
    public Texture2D normalMap
    {
        get {
            return _normalMap;
        }
    }

    /// <summary>
    /// 贴图加载完成回调
    /// </summary>
    private System.Action<Splat> mLoadCompleteCallBack;

    
    /// <summary>
    /// 发起加载贴图
    /// </summary>
    /// <param name="bNeedLoadNmp"></param>
    /// <param name="loadCallBack"></param>
    public void StartLoadTexture(bool bNeedLoadNmp, System.Action<Splat> loadCallBack)
    {
        mbNeedLoadNmp = bNeedLoadNmp;
        if (!mbNeedLoadNmp)
        {
            //移除
            PopCacheNmpTexture();
        }
        mLoadCompleteCallBack = loadCallBack;
        if (bResLoadComplete)
        {
            CallLoadComplete();
            return;
        }

        if (!mbTexLoaded)
        {
            LoadMainTexture();
        }

        if (!mbTexNmpLoaded)
        {
            LoadNmpTexture();
        }
    }

    /// <summary>
    /// 加载主贴图
    /// </summary>
    private void LoadMainTexture()
    {
        DelegateProxy.LoadAsset(key, OnLoadAssetCallBack, null, true);
    }

    /// <summary>
    /// 贴图加载完成回调
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="strFileName"></param>
    /// <param name="varStore"></param>
    private void OnLoadAssetCallBack(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        if (oAsset != null)
        {
            texture = oAsset as Texture2D;
        }
        mbTexLoaded = true;
        if (bResLoadComplete)
        {
            CallLoadComplete();
        }
    }

    /// <summary>
    /// 加载法线贴图
    /// </summary>
    private void LoadNmpTexture()
    {
        //不需加载
        if (!mbNeedLoadNmp)
        {
            return;
        }
        DelegateProxy.LoadAsset(DelegateProxy.StringBuilder(key, "_nmp"), OnLoadAssetNmpCallBack, null, true);
    }

    /// <summary>
    /// 法线贴图加载完成
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="strFileName"></param>
    /// <param name="varStore"></param>
    private void OnLoadAssetNmpCallBack(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        if (oAsset != null)
        {
            _normalMap = oAsset as Texture2D;
        }
        mbTexNmpLoaded = true;
        //无需法线，删除
        if (!mbNeedLoadNmp)
        {
            PopCacheNmpTexture();
        }
        if (bResLoadComplete)
        {
            CallLoadComplete();
        }
    }

    /// <summary>
    /// 资源是否加载完成
    /// </summary>
    private bool bResLoadComplete
    {
        get
        {
            return (!mbNeedLoadNmp && mbTexLoaded) || (mbTexLoaded && mbTexNmpLoaded);
        }
    }

    /// <summary>
    /// 加载完成回调
    /// </summary>
    private void CallLoadComplete()
    {
        if (mLoadCompleteCallBack != null)
        {
            mLoadCompleteCallBack(this);
        }
        else
        {
            LogSystem.LogWarning("Splat CallBack is null!!");
        }
        mLoadCompleteCallBack = null;
    }

    /// <summary>
    /// 清除
    /// </summary>
    public void Dispose()
    {
        mLoadCompleteCallBack = null;
        CacheObjects.PopCache(texture);
        texture = null;
        CacheObjects.PopCache(_normalMap);
        _normalMap = null;
        mbTexLoaded = false;
        mbTexNmpLoaded = false;
        CacheObjects.DespawnCache(this);
    }

    /// <summary>
    /// 移除法线贴图
    /// </summary>
    private void PopCacheNmpTexture()
    {
        CacheObjects.PopCache(_normalMap);
        _normalMap = null;
        mbTexNmpLoaded = false;
    }
}