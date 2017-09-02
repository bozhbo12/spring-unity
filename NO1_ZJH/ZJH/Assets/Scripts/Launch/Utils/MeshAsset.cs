using UnityEngine;

public class MeshAsset : ScriptableObject
{
    public Material material;
    public Mesh mesh;
    #region 释放
    //add by chenfei 20150824
    void OnDestroy()
    {
        if (material != null)
        {
            material = null;
        }
        
        if( mesh != null)
            mesh = null;
    }
    #endregion;

}
