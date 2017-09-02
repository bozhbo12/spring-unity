using System.Collections.Generic;
using UnityEngine;

/*******************************************************************************************************
 * 类 : 资源
 * 注解 : 如果资源已经打包，每个资源都映射一个资源包
 *******************************************************************************************************/
public class Asset
{
    /** 资源加载完毕监听器 */
    public delegate void LoadedListener(Asset asset);
    public LoadedListener loadedListener;

    /** 资源类型 */
    public AssetType type;

    /** 资源路径 */
    public string assetPath = "";

    //private LoadType loadType;

    public LoaderBase loader;

    /** 释放资源 */
    public void Release()
    {
        loadedListener = null;
        loader.Release();
    }
    
    public Asset(string path, LoadType loadType, AssetType type)
    {
        this.assetPath = path;
//        this.loadType = loadType;
        this.type = type;

        if (loadType == LoadType.Type_AssetBundle)
            loader = new AssetBundleLoader();
        else if (loadType == LoadType.Type_AppData)
            loader = new AppDataLoader();
        else if (loadType == LoadType.Type_Resources)
            loader = new ResourceLoader();
        else if (loadType == LoadType.Type_Auto)
        {
            // 如果资源有依赖资源包,则使用依赖加载
            if (AssetLibrary.FindDepends(path) != null)
                loader = new AssetBundleLoader();
            else
                loader = new ResourceLoader();
        }
        else if (loadType == LoadType.Type_WWW)
            loader = new WWWLoader();

        if (loader != null)
        {
            loader.asset = this;
            loader.Load();
        }
    }

    /** 判断资源是否已经加载完毕 */
    public bool loaded
    {
        get {
            return loader.loaded;
        }
    }

    /** 获取游戏对象资源 */
    public GameObject gameObject
    {
        get {
            return loader.gameObject;
        }
    }

    /** 获取2D纹理资源 */
    public Texture2D texture2D
    {
        get {
            return loader.texture;
        }
    }

    /** 获取地域资源 */
    public Region region
    {
        get {
            return loader.region;
        }
    }

    /** 获取场景资源 */
    public GameScene scene
    { 
       get{
           return loader.scene;
       }
    }

    /** 获取地形资源 */
    public LODTerrain terrain
    {
        get
        {
            return loader.terrain;
        }
    }

    /** 资源包资源 */
    public AssetBundle assetBundle
    {
        get
        {
            return loader.assetBundle;
        }
    }

    /** 模型资源 */
    public Mesh mesh
    {
        get
        {
            return loader.mesh;
        }
    }

    public GameObject model
    {
        get
        {
            return loader.model;
        }
    }
}