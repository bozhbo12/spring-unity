using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
/*******************************************************************************************************
 * 类 : 资源加载
 *******************************************************************************************************/
public class ResourceLoader : LoaderBase
{
    /** 源 */
    private UnityEngine.Object source;

    private bool _loaded = false;               // 避免需要清理一些源文件所占内存无法获得源文件

    private bool _bDestroy = false;
    /********************************************************************************
     * 功能 : 加载
     ********************************************************************************/
    override public void Load()
    {
        if(asset.type == AssetType.GameScene && GameScene.isPlaying)
        {
            LoadSceneAsset();
        }
        else
        {
            LoadAsset();
        }
    }

    /// <summary>
    /// 加载非场景资源
    /// </summary>
    private void LoadAsset()
    {
        source = Resources.Load(asset.assetPath, typeof(UnityEngine.Object));
        if (gameObject != null || model != null || scene != null || region != null || terrain != null || texture != null)
        {
            // 如果有回调执行加载回调
            if (asset.loadedListener != null)
                asset.loadedListener(asset);
        }
    }

    /// <summary>
    /// 加载场景资源
    /// </summary>
    private void LoadSceneAsset()
    {
        TimerManager.AddTimer("LoadGameScene", 0.2f, StartLoadScene);
    }

    /// <summary>
    /// 开始加载场景
    /// </summary>
    private void StartLoadScene()
    {
        if (_bDestroy)
            return;

        source = Resources.Load(asset.assetPath, typeof(UnityEngine.Object));
        if (source == null)
        {
            LogSystem.LogWarning("ResourceLoader::StartLoadScene source is null");
            return;
        }
        ReadSceneByte();
    }

    /// <summary>
    /// 读取场景配置文件
    /// </summary>
    private void ReadSceneByte()
    {
        if (source == null)
            return;

        if (_scene == null)
        {
            LogSystem.LogWarning("ResourceLoader::Scene is null");
            return;
        }

        TextAsset textAsset = source as TextAsset;
        if (textAsset == null)
        {
            LogSystem.LogWarning("ReadSceneByte::textAsset is null");
            return;
        }
        else
        {
            byte[] bytes = textAsset.bytes;
            if (bytes == null)
            {
                LogSystem.LogWarning("ReadSceneByte::textAsset is null");
                return;
            }
            try
            {
                _scene.Read(bytes);
            }
            catch (Exception e)
            {
                LogSystem.Log("场景文件读取失败,请检查场景文件是否损坏。文件:" + asset.assetPath + " " + e.ToString());
            }

            source = null;                  // 读取完毕后不需要数据源
            bytes = null;
            _loaded = true;
            Resources.UnloadAsset(textAsset);
            textAsset = null;
        }
    }

    /********************************************************************************
     * 功能 : 资源释放
     ********************************************************************************/
    override public void Release()
    {
        _bDestroy = true;
        // 清空所以引用
        source = null;
        _region = null;
        _scene = null;
        _terrain = null;
        _texture = null;
        _gameObject = null;
        _assetBundle = null;
        _model = null;
    }

    /** 判断资源是否加载完毕 */
    override public bool loaded
    {
        get
        {
            return _loaded;
        }
    }

    /** 获取游戏对象 */
    override public GameObject gameObject
    {
        get
        {
            if ((asset.type == AssetType.Prefab || asset.type == AssetType.GameObject) &&  _gameObject == null)
            {
                _gameObject = source as GameObject;
                _loaded = true;
            }
            return _gameObject;
        }
    }

    private SkinnedMeshParser smParser;

    private MeshParser mParser;

    /** 获取模型对象 */
    override public GameObject model
    {
        get
        {
            if (asset.type == AssetType.Model && _model == null && smParser == null)
            {
                TextAsset textAsset = source as TextAsset;
                if (textAsset != null)
                {
                    byte[] bytes = textAsset.bytes;
                    if (bytes != null)
                    {
                        string type;
                        using (MemoryStream stream = new MemoryStream(bytes))
                        {
                            BinaryReader br = new BinaryReader(stream);
                            type = br.ReadString();
                            br.Close();
                            stream.Close();
                        }
                        //string type = "skeletonModel";
                        if (type == "skeletonModel")
                        {
                            smParser = new SkinnedMeshParser();
                            smParser.ParseAsync(bytes);
                            smParser.parseCompleteListener = OnSkeletonModelParseComplate;
                        }
                        else if (type == "model")
                        {
                            mParser = new MeshParser();
                            mParser.ParseAsync(bytes);
                            mParser.parseCompleteListener = OnModelParseComplate;
                        }
                        Resources.UnloadAsset(textAsset);
                        textAsset = null;
                    }
                }
            }
            return _model;
        }
    }

    private void OnModelParseComplate(ParserBase parser)
    {
        _model = (parser as MeshParser).go;
        mParser.parseCompleteListener = null;
        _loaded = true;
        source = null;
        // 如果有回调执行加载回调
        if (asset.loadedListener != null)
            asset.loadedListener(asset);

    }

    private void OnSkeletonModelParseComplate(ParserBase parser)
    {
        _model = (parser as SkinnedMeshParser).go;
        smParser.parseCompleteListener = null;
        _loaded = true;
        source = null;
        // 如果有回调执行加载回调
        if (asset.loadedListener != null)
            asset.loadedListener(asset);

    }

    /** 获取场景资源 */
    override public GameScene scene
    {
        get
        {
            if (asset.type == AssetType.GameScene &&  _scene == null)
            {
                _scene = new GameScene();
                ReadSceneByte();
            }
            return _scene;
        }
    }

    /** 获取地域资源 */
    override public Region region
    {
        get
        {
            if (asset.type == AssetType.Region && _region == null)
            {
                TextAsset textAsset = source as TextAsset;
                if (textAsset != null)
                {
                    byte[] bytes = textAsset.bytes;
                    if (bytes != null)
                    {
                        _region = new Region();
                        try
                        {
                            _region.Read(bytes);
                        }
                        catch (Exception e)
                        {
                            LogSystem.Log("地域文件读取失败,请检查地域文件是否损坏。文件:" + asset.assetPath + " " + e.ToString());
                        }
                        source = null;                  // 读取完毕后不需要数据源
                        bytes = null;
                        _loaded = true;
                        Resources.UnloadAsset(textAsset);
                        textAsset = null;
                    }
                }
                else
                {
                    // Debug.Log("地域文件丢失 : " + asset.assetPath);
                }
            }
            return _region;
        }
    }

    /** 读取地形文件 */
    override public LODTerrain terrain
    {
        get
        {
            if (asset.type == AssetType.Terrain && _terrain == null)
            {
                TextAsset textAsset = source as TextAsset;
                if (textAsset != null)
                {
                    byte[] bytes = textAsset.bytes;
                    if (bytes != null)
                    {
                        LODTerrainData terData = new LODTerrainData();
                        try
                        {
                            terData.Read(bytes);
                        }
                        catch (Exception e)
                        {
                            LogSystem.Log("地形文件读取失败,请检查地域文件是否损坏。文件:" + asset.assetPath + " " + e.ToString());
                        }
                        _terrain = LODTerrain.CreateTerrainGameObject(terData, true);
                        Resources.UnloadAsset(textAsset);
                        source = null;                  // 读取完毕后不需要数据源
                        terData = null;
                        textAsset = null;
                        bytes = null;
                        _loaded = true;
                    }
                }
                else
                {
                    // Debug.Log("地形文件丢失 : " + asset.assetPath);
                }
            }
            return _terrain;
        }
    }

    /** 获取纹理 */
    override public Texture2D texture
    {
        get
        {
            if (asset.type == AssetType.Texture2D && _texture == null)
            {
                _texture = source as Texture2D;
                _loaded = true;
            }
            return _texture;
        }
    }

}