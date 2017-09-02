using System;
using System.Collections.Generic;
using UnityEngine;

/*******************************************************************************************************
 * 类 : 应用数据加载
 *******************************************************************************************************/
public class AppDataLoader : LoaderBase
{
    /********************************************************************************
     * 功能 : 加载
     ********************************************************************************/
    override public void Load()
    {
        // 如果有回调执行加载回调
        if (asset.loadedListener != null)
            asset.loadedListener(asset);
    }

    /** 判断资源是否加载完毕 */
    override public bool loaded
    {
        get
        {
            return true;
        }
    }

    /** 获取场景资源 */
    override public GameScene scene
    {
        get
        {
            if (_scene == null)
            {
                byte[] bytes = QFileUtils.ReadBinary(asset.assetPath);
                _scene = new GameScene();
                _scene.Read(bytes);
            }
            return _scene;
        }
    }

    /** 获取地域资源 */
    override public Region region
    {
        get
        {
            if (_region == null)
            {
                byte[] bytes = QFileUtils.ReadBinary(asset.assetPath);

                _region = new Region();
                _region.Read(bytes);
            }
            return _region;
        }
    }

    /** 读取地形文件 */
    override public LODTerrain terrain
    {
        get
        {
            if (_terrain == null)
            {
                byte[] bytes = QFileUtils.ReadBinary(asset.assetPath);

                LODTerrainData terData = new LODTerrainData();
                terData.Read(bytes);
                _terrain = LODTerrain.CreateTerrainGameObject(terData, true);
            }
            return _terrain;
        }
    }
    
    /** 获取纹理 */
    override public Texture2D texture
    {
        get 
        {
            if (_texture == null)
            {
                byte[] bytes = QFileUtils.ReadBinary(asset.assetPath);

                _texture = new Texture2D(1, 1);
                _texture.LoadImage(bytes);
            }
            return _texture;
        }
    }
}