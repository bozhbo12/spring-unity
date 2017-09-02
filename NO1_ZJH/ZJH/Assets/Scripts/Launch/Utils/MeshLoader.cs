using UnityEngine;

/// <summary>
/// 网格模型部件加载器
/// </summary>
public class MeshLoader : MonoBehaviour
{
    public enum MeshLoadType
    {
        MLT_SKIN=1,    ///蒙皮网格
        MLT_FILTER=2,  ///静态模型
        MLT_MAX
    }
    [SerializeField]
    public string strMeshAssetName;
    [SerializeField]
    public int iType = 0;
    
    #region 释放
    void Start()
    {
        if (!string.IsNullOrEmpty(strMeshAssetName))
        {
            string strMeshName = strMeshAssetName;
            strMeshAssetName = string.Empty;
            ChangeMesh(strMeshName);
        }
    }
    /// <summary>
    /// 加载模型
    /// </summary>
    /// <param name="strMeshName">模型名</param>
    public void ChangeMesh(string strMeshName)
    {
        if (!strMeshAssetName.Equals(strMeshName))
        {
            strMeshAssetName = strMeshName;
            if (monLoadAsset != null)
            {
                monLoadAsset(strMeshName, OnFileLoaded,null);
            }
            else 
            {
                Object o = Resources.Load(strMeshName);
                OnFileLoaded(o,strMeshName);
            }
        }
    }
	public delegate void CallLoadAsset(string strFileName, AssetCallback callback, VarStore varStore = null, bool bAsync = false);
    public static CallLoadAsset monLoadAsset = null;
    public static void SetLoadAssetCall(CallLoadAsset call)
    {
        monLoadAsset = call;
    }
    /// <summary>
    /// 网格加载结束
    /// </summary>
    /// <param name="args"></param>
    private void OnFileLoaded(UnityEngine.Object oAsset, string strFileName, VarStore varStore=null)
    {
        MeshAsset meshAsset = oAsset as MeshAsset;
        if (meshAsset == null)
            return;

        switch (iType)
        {
            case 1://MeshLoadType.MLT_SKIN:
                {
                    ///填充到网格上
                    SkinnedMeshRenderer meshRender = this.GetComponent<SkinnedMeshRenderer>();
                    if (meshRender != null)
                    {
                        meshRender.material = meshAsset.material;
                        meshRender.sharedMesh = meshAsset.mesh;
                    }
                }
                break;
            case 2://MeshLoadType.MLT_FILTER:
                {
                    ///填充网格信息
                    MeshFilter meshFilter = this.GetComponent<MeshFilter>();
                    if (meshFilter != null)
                    {
                        meshFilter.mesh = meshAsset.mesh;
                    }
                    MeshRenderer meshRender = this.GetComponent<MeshRenderer>();
                    if (meshRender != null)
                    {
                        meshRender.material = meshAsset.material;
                    }
                }
                break;
        }
    }
    //add by chenfei 20150824
    void OnDestroy()
    {
        //meshAsset = null;
    }
    #endregion;

}
