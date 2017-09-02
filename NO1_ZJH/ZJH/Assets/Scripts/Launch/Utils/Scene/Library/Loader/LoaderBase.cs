using System.Collections.Generic;
using UnityEngine;

/*******************************************************************************************************
 * 类 : 资源加载
 *******************************************************************************************************/
public class LoaderBase
{
    public Asset asset;

    public byte[] bytes;

    /** 解析的资源 */
    protected Region _region;
    protected GameScene _scene;
    protected LODTerrain _terrain;
    protected Texture2D _texture;
    protected GameObject _gameObject;
    protected AssetBundle _assetBundle;
    protected Mesh _mesh;
    protected GameObject _model;

    /** 判断资源是否加载完毕 */
    virtual public bool loaded
    {
        get {
            return false;
        }
    }

    public LoaderBase()
    {
        
    }

    /********************************************************************************
     * 功能 : 加载
     ********************************************************************************/
    virtual public void Load() { }

    /********************************************************************************
     * 功能 : 资源释放
     ********************************************************************************/
    virtual public void Release() { }

    /********************************************************************************
     * 属性 : 资源
     ********************************************************************************/
    virtual public AssetBundle assetBundle { get { return null; } }
    virtual public GameObject gameObject { get { return null; } }
    virtual public GameScene scene { get { return null; } }
    virtual public Region region { get { return null; } }
    virtual public LODTerrain terrain { get { return null; } }
    virtual public Texture2D texture { get { return null; } }
    virtual public Mesh mesh { get { return null; } }
    virtual public GameObject model { get { return null; } }
}