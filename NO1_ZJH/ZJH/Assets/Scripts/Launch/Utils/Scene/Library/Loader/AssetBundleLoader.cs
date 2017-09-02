using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*******************************************************************************************************
 * 类 : 资源加载
 *******************************************************************************************************/
public class AssetBundleLoader : LoaderBase
{
    /** 源数据, 源数据已经存在与内存中 */
    private UnityEngine.AssetBundle source;

    private Asset mainDependAsset;

    private int dependsLoadedCount = 0;

    /** 依赖资源 */
    private List<Asset> dependAssets = new List<Asset>();

    public AssetBundleLoader()
    {

    }

    /********************************************************************************
     * 功能 : 异步加载资源
     ********************************************************************************/
    override public void Load()
    {
        List<string> dependAsstsPaths = AssetLibrary.FindDepends(asset.assetPath);
        if (dependAsstsPaths == null || dependAsstsPaths.Count == 0)
        {
            LogSystem.LogWarning("AssetBundleLoader::Load AssetLibrary.FindDepends null ", asset.assetPath);
            return;
        }
        //其实就是由"Scenes/111001" 转换成 "前缀+平台路径+resource/scenes/111001"
        for (int i = 0; i < dependAsstsPaths.Count; i++)
        {
            // www 加载资源包资源
            Asset ass = AssetLibrary.Load(dependAsstsPaths[i], AssetType.AssetBundle, LoadType.Type_WWW);
            dependAssets.Add(ass);
            if (ass.loaded == false)
                ass.loadedListener = dependLoaded;
            else
                dependsLoadedCount++;
        }
        mainDependAsset = dependAssets[0];
        CheckLoadedComplate();
    }

    /** 依赖资源加载完毕 */
    private void dependLoaded(Asset asset)
    {
        dependsLoadedCount++;
        CheckLoadedComplate();
    }

    /** 检测依赖资源是否加载完毕 */
    private void CheckLoadedComplate()
    {
       
        if (dependsLoadedCount == dependAssets.Count)
        {
            if (asset.type == AssetType.GameScene)
            {
                if (_scene != null)
                {
                    //strAsset = "assets/resources/scenes/111001.bytes";
                    string strAsset = "assets/resources/" + asset.assetPath.ToLower() + ".bytes";
                    TextAsset textAsset = mainDependAsset.assetBundle.LoadAsset(strAsset) as TextAsset;
                    byte[] bytes = textAsset.bytes;
                    _scene.Read(bytes);
                }
            }

            // 游戏场景
            if (asset.loadedListener != null)
            {
                asset.loadedListener(asset);
            }
        }
    }

    /** 判断资源是否加载完毕 */
    override public bool loaded
    {
        get
        {
            for (int i = 0; i < dependAssets.Count; i++)
            {
                if (dependAssets[i].loaded == false)
                    return false;
            }
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
                _scene = new GameScene();
                _scene.loadFromAssetBund = true;
            }
            //Asset(mainDependAsset)其中loadbase为WWWLoader
            if (mainDependAsset != null && mainDependAsset.assetBundle != null && _scene.readCompalte == false)
            {
                string strAsset = "assets/resources/" + asset.assetPath.ToLower() + ".bytes";
                TextAsset textAsset = mainDependAsset.assetBundle.LoadAsset(strAsset) as TextAsset;
                byte[] bytes = textAsset.bytes;
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
                string strAsset = "assets/resources/" + asset.assetPath.ToLower() + ".bytes";
                TextAsset textAsset = mainDependAsset.assetBundle.LoadAsset(strAsset) as TextAsset;
                if (textAsset != null)
                {
                    byte[] bytes = textAsset.bytes;
                    _region = new Region();
                    _region.Read(bytes);
                }
                else
                {
                    Debug.Log("terrain asset is missing.");
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
            if (_terrain == null)
            {
                string strAsset = "assets/resources/" + asset.assetPath.ToLower() + ".bytes";
                TextAsset textAsset = mainDependAsset.assetBundle.LoadAsset(strAsset) as TextAsset;
                byte[] bytes = textAsset.bytes;

                LODTerrainData terData = new LODTerrainData();

				try
				{
					terData.Read(bytes);
				}
				catch (Exception )
				{
					// Debug.Log("地形文件读取失败,请检查地域文件是否损坏。文件:" + asset.assetPath + " " + e.Message);
				}


                _terrain = LODTerrain.CreateTerrainGameObject(terData, true);
            }
            return _terrain;
        }
    }
    private int GetResType(string strFilePath)
    {
        if (strFilePath.Contains("terraindata"))
        {
            return 1;
        }
        else if (strFilePath.Contains("splats"))
        {
            return 2; 
        }
        else if (strFilePath.Contains("terrain"))
        {
			if (strFilePath.Contains ("_nmp")) {
				return 3;
			} else {
				return 4;
			}
             
        }
        else if (strFilePath.Contains("/lightmap/"))
        {
            return 4; 
        }

        return 0;
    }
    /** 获取纹理 */
    override public Texture2D texture
    {
        get
        {
            if (_texture == null)
            {
                string strAsset = asset.assetPath.ToLower();
                switch (GetResType(strAsset))
                {
                    case 1:
                        strAsset = "assets/resources/" + strAsset + ".bytes";
                        break;
                    case 2:
                        strAsset = "assets/resources/" + strAsset + ".png";
                        break;
                    case 3:
                        strAsset = "assets/resources/" + strAsset + ".tga";
                        break;
                    case 4:
                        strAsset = "assets/resources/" + strAsset + ".png";
                        break;
                }

                UnityEngine.Object o2d = mainDependAsset.assetBundle.LoadAsset(strAsset);
                if (o2d != null)
                {
                    _texture = o2d as Texture2D;
                }
            }
            return _texture;
        }
    }

}