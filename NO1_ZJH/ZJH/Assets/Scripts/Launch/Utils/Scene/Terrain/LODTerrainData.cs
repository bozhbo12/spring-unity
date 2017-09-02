using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

/***********************************************************************************************
 * 功能 ： LOD地形数据
 ***********************************************************************************************/
public class LODTerrainData
{
    /** 高度图的分辨率 */
    public int heightmapResolution = 32;

    /** 混合纹理的分辨率 */
    public int splatmapResolution = 64;

#if UNITY_EDITOR
    /** 地形在世界坐标系中的尺寸, 最大宽度、最大长度、最大高度 */
    public Vector3 size = Vector3.zero;

    /** 灰度图 */
    public float[,] heightmap;

    public Texture2D _splatsmapTex;

    /** 混合纹理图 x y value */
    public float[, ,] splatsmap;
#endif

    /** 混合纹理相关属性 */
    public Splat[] splats;

    /** 线段数 */
    public const int segmentsW = 16;
    public const int segmentsH = 16;

   
    /** 混合纹理层数 */
    public int spaltsmapLayers = 0;

    private bool _heightmapDirty = false;
    private bool _splatsmapDirty = false;

    /** 地形当前的顶点 */
    public Vector3[] vertices;

    public Vector3[] normals;

    public Vector4[] tangents;

    /** 地形当前的UV */
    public Vector2[] uvs;

    /** 地形三角形 */
    public int[] triangles;

    /** 地形配置, */
    public TerrainConfig terrainConfig;

    /// <summary>
    /// 当前加载数据
    /// </summary>
    private int miCurLoadSplatCount = 0;

    /// <summary>
    /// splat加载完成回调
    /// </summary>
    private System.Action<LODTerrainData> OnSplatLoadComplete = null;

    /*************************************************************
     * 功能 ： 根据地形配置创建新的地形数据
     **************************************************************/
    public LODTerrainData()
    {
        this.terrainConfig = GameScene.mainScene.terrainConfig;

        this.heightmapResolution = terrainConfig.heightmapResolution;
        this.splatmapResolution = terrainConfig.splatmapResolution;
        this.spaltsmapLayers = terrainConfig.spaltsmapLayers;

#if UNITY_EDITOR
        this.size.x = terrainConfig.tileSize;
        this.size.z = terrainConfig.tileSize;
        this.size.y = terrainConfig.maxTerrainHeight;
#endif

#if UNITY_EDITOR
        // 默认设置基础纹理
        splats = new Splat[spaltsmapLayers];
        splats[0] = terrainConfig.baseSplat;
        if (GameScene.isPlaying == false)
        {
            heightmap = new float[heightmapResolution, heightmapResolution];

            int i = 0;
            int j = 0;
            int k = 0;

            // 填充初始高度图
            for (i = 0; i < heightmapResolution; i++)
            {
                for (j = 0; j < heightmapResolution; j++)
                {
                    heightmap[i, j] = terrainConfig.defaultTerrainHeight;
                }
            }

            // 填充初始溅斑纹理图
            if (splatsmap == null)
            {
                splatsmap = new float[splatmapResolution, splatmapResolution, spaltsmapLayers];
                for (i = 0; i < splatmapResolution; i++)
                {
                    for (j = 0; j < splatmapResolution; j++)
                    {
                        splatsmap[i, j, 0] = 1f;
                        for (k = 1; k < spaltsmapLayers; k++)
                        {
                            splatsmap[i, j, k] = 0f;
                        }
                    }
                }
            }
            //
            GenerateSplatsmapTex();
        }
#endif

    }

    /// <summary>
    /// 发起加载
    /// </summary>
    /// <param name="bNeedLoadNmp"></param>
    /// <param name="splatLoadComplete"></param>
    public void StartLoadSplatTexture(bool bNeedLoadNmp, System.Action<LODTerrainData> splatLoadComplete)
    {
        miCurLoadSplatCount = 0;
        OnSplatLoadComplete = splatLoadComplete;
        for (int i = 0; i < splats.Length; i++ )
        {
            if (splats[i] == null)
            {
                LogSystem.LogWarning("LODTerrainData StartLoadSplatTexture splats[i] is null!! ", i);
                continue;
            }
            splats[i].StartLoadTexture(bNeedLoadNmp, SplatTexLoadComplete);
        }
    }

    /// <summary>
    /// 贴图加载完成回调
    /// </summary>
    /// <param name="splat"></param>
    private void SplatTexLoadComplete(Splat splat)
    {
        if (++miCurLoadSplatCount == splats.Length)
        {
            if (OnSplatLoadComplete != null)
            {
                OnSplatLoadComplete(this);
                OnSplatLoadComplete = null;
            }
            else
            {
                LogSystem.LogWarning("LODTerrainData OnSplatLoadComplete is null!!!");
            }
        }
    }

    /******************************************************************************
     * 功能 ： 释放地形数据
     ******************************************************************************/
    public void Release()
    {
        this.vertices = null;
        this.triangles = null;
        this.normals = null;
        uvs = null;
        terrainConfig = null;
        OnSplatLoadComplete = null;
        if (splats != null)
        {
            for (int i = 0; i < splats.Length; i++)
            {
                if (splats[i] != null)
                {
                    splats[i].Dispose();
                }
                else
                {
                    LogSystem.LogWarning("LODTerrainData Release splats[i] is null!!" ,i);
                }
            }
            splats = null;
        }
        else
        {
            LogSystem.LogWarning("LODTerrain Release splats is null !!!!");
        }

#if UNITY_EDITOR
        this.heightmap = null;
        this.splatsmap = null;
#endif
    }

    /******************************************************************************
     * 功能 ： 读取地形高度
     ******************************************************************************/
    public void Read(byte[] bytes)
    {
        long pos = 0;

        MemoryStream m = new MemoryStream(bytes);
        BinaryReader br = new BinaryReader(m);

        int i = 0;
        int j = 0;
        int k = 0;

        pos = br.BaseStream.Position;
        if (br.BaseStream.Position < br.BaseStream.Length)
        {
            if (br.ReadInt32() == 10016)
            {
                // 读取地形纹理
                int splatsCount = br.ReadInt32();
                splats = new Splat[splatsCount];
                for (i = 0; i < splatsCount; i++)
                {
                    Splat splat = Splat.GetSplat();
                    //splat.texture = AssetLibrary.Load(path, AssetType.Texture2D, LoadType.Type_Auto).texture2D;
                    splat.key = br.ReadString();
                    splat.tilingOffset.x = br.ReadSingle();
                    splat.tilingOffset.y = br.ReadSingle(); 
                    splat.tilingOffset.z = br.ReadSingle();
                    splat.tilingOffset.w = br.ReadSingle();
                    splats[i] = splat;
                }

                // 读取顶点位置信息
                int vertCount = br.ReadInt32();

                // 游戏运行状态下不重复创建对象,编辑器状态下要保存顶点不可复用
                /**
                if (GameScene.isPlaying == true)
                {
                    if (temporaryVertices == null)
                    {
                        temporaryVertices = new Vector3[vertCount];
                        for (i = 0; i < 289; i++)
                        {
                            temporaryVertices[i] = Vector3.zero;
                        }
                    }
                    for (i = 0; i < vertCount; i++)
                    {
                        temporaryVertices[i].x = br.ReadSingle();
                        temporaryVertices[i].y = br.ReadSingle();
                        temporaryVertices[i].z = br.ReadSingle();
                    }
                    vertices = temporaryVertices;
                }
                else
                **/
                {
                    vertices = new Vector3[vertCount];
                    for (i = 0; i < vertCount; i++)
                    {
                        Vector3 vert = Vector3.zero;
                        vert.x = br.ReadSingle();
                        vert.y = br.ReadSingle();
                        vert.z = br.ReadSingle();
                        vertices[i] = vert;
                    }
                }

                // 游戏运行中不继续读取数据 ============================================================================
                //if (GameScene.isPlaying == true)
                //{
                //    br.Close();
                //    m.Flush();
                //    return;
                //}
								
                // 读取三角形
                int triIndCount = br.ReadInt32();
                triangles = new int[triIndCount];
                for (i = 0; i < triIndCount; i++)
                {
                    triangles[i] = br.ReadInt32();
                }

                // 读取UV
                int uvCount = br.ReadInt32();
                uvs = new Vector2[uvCount];
                for (i = 0; i < uvCount; i++)
                {
                    Vector2 uv = Vector2.zero;
                    uv.x = br.ReadSingle();
                    uv.y = br.ReadSingle();
                    uvs[i] = uv;
                }

                // 读取法线
                int normalCount = br.ReadInt32();
                normals = new Vector3[normalCount];
                for (i = 0; i < normalCount; i++)
                {
                    Vector3 normal = Vector3.zero;
                    normal.x = br.ReadSingle();
                    normal.y = br.ReadSingle();
                    normal.z = br.ReadSingle();
                    normals[i] = normal;
                }

                // 读取切线值
                int tangentCount = br.ReadInt32();
                tangents = new Vector4[tangentCount];
                for (i = 0; i < tangentCount; i++)
                {
                    Vector4 tangent = Vector4.zero;
                    tangent.x = br.ReadSingle();
                    tangent.y = br.ReadSingle();
                    tangent.z = br.ReadSingle();
                    tangent.w = br.ReadSingle();
                    tangents[i] = tangent;
                }
            }
        }

        if (GameScene.isPlaying == true)
        {
            br.Close();
            m.Flush();
            return;
        }

#if UNITY_EDITOR
        // 高度图数据
        for (i = 0; i < heightmapResolution; i++)
        {
            for (j = 0; j < heightmapResolution; j++)
            {
                heightmap[j, i] = br.ReadSingle();
            }
        }
 

        int layers = 0;

        // 旧数据兼容
        pos = br.BaseStream.Position;
        if (br.BaseStream.Position < br.BaseStream.Length)
        {
            if (br.ReadInt32() == 10013)
            {
                layers = spaltsmapLayers;
            }
            else
            {
                br.BaseStream.Position = pos;
            }
        }

        // 写入混合纹理数据
        for (i = 0; i < splatmapResolution; i++)
        {
            for (j = 0; j < splatmapResolution; j++)
            {
                for (k = 0; k < layers; k++)
                {
                    splatsmap[j, i, k] = br.ReadSingle();
                }
            }
        }

        // 生成溅斑纹理
        GenerateSplatsmapTex();
#endif

    }

    
#region 编译器
#if UNITY_EDITOR

    /*******************************************************************************
     * 属性 : 获取溅斑纹理
     * 编译器：方法
     *******************************************************************************/
    public Texture2D splatsmapTex
    {
        get
        {
            if (_splatsmapDirty == true)
                GenerateSplatsmapTex();
            return _splatsmapTex;
        }
    }

    /*****************************************************************************************
      * 功能 ： 生成溅斑图纹理
     * 编译器：方法
      *****************************************************************************************/
    public void GenerateSplatsmapTex()
    {
        float[, ,] map = splatsmap;
        _splatsmapTex = new Texture2D(splatmapResolution, splatmapResolution, TextureFormat.ARGB32, false);
        _splatsmapTex.wrapMode = TextureWrapMode.Clamp;
        for (int i = 0; i < splatmapResolution; i++)
        {
            for (int j = 0; j < splatmapResolution; j++)
            {
                Color c = new Color(0f, 0f, 0f, 0f);
                c.r = map[j, i, 0];
                if (terrainConfig.spaltsmapLayers > 1)
                    c.g = map[j, i, 1];
                if (terrainConfig.spaltsmapLayers > 2)
                    c.b = map[j, i, 2];
                if (terrainConfig.spaltsmapLayers > 3)
                    c.a = map[j, i, 3];
                _splatsmapTex.SetPixel(i, j, c);
            }
        }
        _splatsmapTex.Apply();
    }


    /*********************************************************************************
    * 功能 ：应用地形高度
     * 编译器方法(无用方法)
    *********************************************************************************/
    public void SetHeights(int xBase, int yBase, float[,] heights)
    {
        if (heights == null) return;
        int width = heights.GetLength(1);
        int height = heights.GetLength(0);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                heightmap[yBase + j, xBase + i] = heights[j, i];
            }
        }
        _heightmapDirty = true;
    }

    /// <summary>
    /// 编译器方法(无用方法)
    /// </summary>
    /// <param name="xBase"></param>
    /// <param name="yBase"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="value"></param>
    public void SetHeights(int xBase, int yBase, int width, int height, float value)
    {
        // i 列号, j行号
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                heightmap[yBase + j, xBase + i] = value;
            }
        }
        _heightmapDirty = true;
    }

    /**********************************************************************************
     * 功能 ：采样指定像素点高度
     * 注 ： 高度在二维数组中排列顺序 heightmap【行号， 列号】
     * heightmap[0, 1] heightmap[0, 2]  heightmap[0, 3]
     * heightmap[1, 1] heightmap[1, 2]  heightmap[1, 3]
     * heightmap[3, 1] heightmap[3, 2]  heightmap[3, 3]
     * 编译器方法(无用方法)
     **********************************************************************************/
    public float GetHeight(int x, int z)
    {
        return heightmap[z, x];
    }

    /**********************************************************************************
     * 功能 ：获取指定区域的溅斑纹理
     * 编译器方法(无用方法)
     ***********************************************************************************/
    public float[, ,] GetSplatsmap(int xBase, int yBase, int width, int height)
    {
        float[, ,] result = new float[height, width, spaltsmapLayers];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < spaltsmapLayers; k++)
                {
                    result[j, i, k] = splatsmap[yBase + j, xBase + i, k];
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    public int splatsCount
    {
        get
        {
            for (int i = 0; i < spaltsmapLayers; i++)
            {
                if (splats[i] == null)
                {
                    return i;
                }
            }
            return spaltsmapLayers;
        }
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    public bool heightmapDirty
    {
        get
        {
            return _heightmapDirty;
        }
        set
        {
            _heightmapDirty = value;
        }
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    public bool splatsmapDirty
    {
        get
        {
            return _splatsmapDirty;
        }
    }
    

    /*********************************************************************************
     * 功能 ：设置溅斑纹理
     * 编译器方法
     *********************************************************************************/
    public void SetSplasmap(int xBase, int yBase, float[, ,] map)
    {
        if (map == null) return;
        int width = map.GetLength(1);
        int height = map.GetLength(0);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < spaltsmapLayers; k++)
                {
                    splatsmap[yBase + j, xBase + i, k] = map[j, i, k];
                }
            }
        }
        _splatsmapDirty = true;
    }

   
    /**********************************************************************************
     * 功能 ：获取地形的高度
     * 编译器方法(无用方法)
     ***********************************************************************************/
    public float[,] GetHeights(int xBase, int yBase, int width, int height)
    {
        float[,] result = new float[height, width];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                result[j, i] = heightmap[yBase + j, xBase + i];
            }
        }
        return result;
    }
#endif
#endregion

}