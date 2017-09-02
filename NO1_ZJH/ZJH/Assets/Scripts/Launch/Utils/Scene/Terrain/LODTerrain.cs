using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

/***********************************************************************************************
 * 功能 ：LOD地形
 * 注解 : 地形平滑处理
 ***********************************************************************************************/
[ExecuteInEditMode]
public class LODTerrain : MonoBehaviour
{
    public static string name1024 = string.Empty;
    /// <summary>
    /// 根节点
    /// </summary>
    static private GameObject terRoot;

    /** 地形模型三角形可以进行共享 */
    static public Vector3[] shareVertices;

    /** 地形模型三角形可以进行共享 */
    static public int[] shareTriangles;

    /** 地形纹理的UV可以进行共享 */
    static public Vector2[] shareUVS;

    /** 相邻关系 */
    public LODTerrain left;
    public LODTerrain right;
    public LODTerrain top;
    public LODTerrain bot;

    public LODTerrain top_left;
    public LODTerrain top_right;
    public LODTerrain bot_left;
    public LODTerrain bot_right;

    /// <summary>
    /// 是否重建需要生成RT
    /// </summary>
    private bool mbReBuildRT = false;

    /// <summary>
    /// 是否是当前中心地块(用于判断Bake大小(1024or512))
    /// </summary>
    private bool mbCenterTerrain = false;
    public bool bCenterTerrain
    {
        get
        {
            return mbCenterTerrain;
        }
        set
        {
            if (value == mbCenterTerrain)
                return;

            mbReBuildRT = true;
            mbCenterTerrain = value;
        }
    }

    /** 地形数据 */
    public LODTerrainData terrainData;

    /** 地形当前的顶点 */
    public Vector3[] vertices;

    public Vector3[] normals;

    /** 地形当前的UV */
    public Vector2[] uvs;

    /** 正切值 */
    public Vector4[] tangents;

    /** 地形三角形 */
    public int[] triangles;

    /// <summary>
    /// 对象材质
    /// 高配置：使用
    /// 低配置: 拍照后隐藏
    /// </summary>
    public Material matrial;

    public MeshRenderer terrainRenderer;

    private TerrainConfig terrainConfig;

    private int fullSegments = 16;

    private int tick = 0;

    //--------------------------
    private int oldBumpCount = -1;

    /** 是否启用LOD */
    public bool enableLOD = false;

    /** 当前地形LOD级别 */
    public int lodLevel = 0;
    private int oldlodLevel = -1;

    private int leftLodLevel = 0;
    private int rightLodLevel = 0;
    private int topLodLevel = 0;
    private int botLodLevel = 0;

    //--------------------------
    public bool FocusUpdateLod = false;

    //------------------------

    public string splatsMapPath = "";

    /// <summary>
    /// RT的Shader
    /// </summary>
    private static Shader bakeShader = null;

    private static Shader realLightShader = null;

    private static Shader Shader_RealLightWithBumped4 = null;

    /// <summary>
    /// backMap索引。 -1没有back，反之……
    /// </summary>
    private Material bakeMat;

    /// <summary>
    /// 低配bake
    /// </summary>
    private RenderTexture mlodRT = null;

    //-----------------------
    /** 地形网格数据 */
    public Mesh mesh;

    void Awake()
    {
        if (bakeShader == null)
            bakeShader = Shader.Find("Snail/TerrainBake");

        if (realLightShader == null)
            realLightShader = Shader.Find("Snail/TerrainSplat4");

        if (Shader_RealLightWithBumped4 == null)
            Shader_RealLightWithBumped4 = Shader.Find("Snail/Bumped/TerrainSplat4_Normal_Specular");


        if (GameScene.mainScene != null)
        {
            GameScene.mainScene.mBumpChangeDgate.AppendCalls(QaualityChange);
        }
        
#if UNITY_EDITOR
            bakeNoShadowShader = Shader.Find("Snail/TerrainBakeNoShadow");
#endif
    }

    /// <summary>
    /// 高中低配变化
    /// </summary>
    /// <param name="varStore"></param>
    private void QaualityChange(VarStore varStore)
    {
        if(terrainConfig == null)
            return;

        if (oldBumpCount == terrainConfig.bumpCount)
            return;

        oldBumpCount = terrainConfig.bumpCount;
        LoadSplatRes();
    }

    /// <summary>
    /// 加载splat资源
    /// </summary>
    private void LoadSplatRes()
    {
        terrainData.StartLoadSplatTexture(oldBumpCount == 2, OnLoadTexComplete);
    }

    /// <summary>
    /// splat贴图加载完成
    /// </summary>
    /// <param name="lodTerrainData"></param>
    private void OnLoadTexComplete(LODTerrainData lodTerrainData)
    {
        //创建mater
        BuildMaterial();
    }

    //---------------------------

    void Update()
    {
        if (GameScene.mainScene == null)
            return;

        tick++;
        if (tick == 3 && uvs == null)
        {
            BuildUVs();
        }
        if (tick == 4 && triangles == null)
        {
            BuildTriangles();
        }
        if (tick == 5 && matrial == null)
        {
            //oldBumpCount = GameScene.mainScene.terrainConfig.bumpCount;
            //BuildMaterial();
            QaualityChange(null);
        }

        if (tick == 6)
            terrainRenderer.enabled = true;


        UpdateLOD();

        if (matrial != null && terrainConfig != null)
        {
            if (terrainConfig.bumpCount > 0)
            {
                matrial.SetColor("_SpecColor", terrainConfig.terrainSpecColor);
                matrial.SetFloat("_Shininess", terrainConfig.terrainShininess);
                matrial.SetFloat("_Glossiness", terrainConfig.terrainGloss);
            }
        }
    }

    /*****************************************************************************************
    * 功能 ：创建UV
    *****************************************************************************************/
    private void BuildUVs()
    {
        if (shareUVS == null)
            BuildShareUVS();

        mesh.uv = shareUVS;
        this.uvs = shareUVS;
    }

    private void BuildTriangles()
    {
        if (shareTriangles == null)
        {
            BuildShareTriangles();
        }
        this.triangles = shareTriangles;
        mesh.triangles = triangles;
    }

    /****************************************************************************************
     * 功能 ： 更新LOD
     ****************************************************************************************/

    void UpdateLOD()
    {
        if (enableLOD == false)
            return;

        //TerrainLODConfig lodCfg = GameScene.mainScene.terrainConfig.terrainLODConfig;

        if ((lodLevel != oldlodLevel) || FocusUpdateLod || LodDirty())
        {
            oldlodLevel = lodLevel;
            UpdateLodMesh();
        }
    }

    private bool LodDirty()
    {
        bool result = false;

        if (left != null && left.lodLevel != leftLodLevel)
            result = true;
        if (right != null && right.lodLevel != rightLodLevel)
            result = true;
        if (top != null && top.lodLevel != topLodLevel)
            result = true;
        if (bot != null && bot.lodLevel != botLodLevel)
            result = true;

        if (left)
            leftLodLevel = left.lodLevel;
        if (right)
            rightLodLevel = right.lodLevel;
        if (top)
            topLodLevel = top.lodLevel;
        if (bot)
            botLodLevel = bot.lodLevel;

        return result;
    }


    /*****************************************************************************************
     * 功能 ：
     *****************************************************************************************/

    public bool NeighborDifferenLevel()
    {
        if (left != null && left.lodLevel > lodLevel)
            return true;
        if (right != null && right.lodLevel > lodLevel)
            return true;
        if (top != null && top.lodLevel > lodLevel)
            return true;
        if (bot != null && bot.lodLevel > lodLevel)
            return true;

        /**
        if (top_left != null && top_left.lodLevel != lodLevel)
            return true;
        if (top_right != null && top_right.lodLevel != lodLevel)
            return true;
        if (bot_left != null && bot_left.lodLevel != lodLevel)
            return true;
        if (bot_right != null && bot_right.lodLevel != lodLevel)
            return true;
        **/

        return false;
    }

    public void GenerateLODMesh(int level, int edgeMask, out Vector3[] lodVertices, out Vector2[] lodUVs, out Vector3[] lodNormals, out Vector4[] lodTangents, out int[] lodTriangles)
    {
        TerrainLODConfig lodCfg = GameScene.mainScene.terrainConfig.terrainLODConfig;

        int lodSegmentH = lodCfg.lodSegments[level];
        int lodSegmentW = lodCfg.lodSegments[level];
        int numVerts = (lodSegmentH + 1) * (lodSegmentW + 1);
        int step = lodCfg.lodSteps[level];

        lodVertices = new Vector3[numVerts];
        lodNormals = new Vector3[numVerts];
        lodTangents = new Vector4[numVerts];
        lodUVs = new Vector2[numVerts];
        lodTriangles = TerrainIndexGenerator.GetTriangles(edgeMask, lodCfg.lodPatchSize[level], 0, 0);

        numVerts = 0;


        // 更新位置、法线、正切信息
        for (int zi = 0; zi <= lodSegmentH; ++zi)
        {
            for (int xi = 0; xi <= lodSegmentW; ++xi)
            {
                int vertexInd = zi * step + (xi * step) * (fullSegments + 1);

                lodVertices[numVerts] = vertices[vertexInd];

                lodNormals[numVerts] = normals[vertexInd];
                lodTangents[numVerts] = tangents[vertexInd];
                lodUVs[numVerts] = uvs[vertexInd];
                numVerts++;
            }
        }

    }

    private void UpdateLodMesh()
    {
        if (enableLOD == false)
            return;

        Vector3[] lodVertices;
        Vector2[] lodUVs;
        Vector3[] lodNormals;
        Vector4[] lodTangents;
        int[] lodTriangles;

        GenerateLODMesh(lodLevel, GetEdgeMask(), out lodVertices, out lodUVs, out lodNormals, out lodTangents, out lodTriangles);

        Mesh lodMesh = new Mesh();
        lodMesh.vertices = lodVertices;
        lodMesh.uv = lodUVs;
        lodMesh.normals = lodNormals;
        lodMesh.tangents = lodTangents;
        lodMesh.triangles = lodTriangles;
        lodMesh.name = "Mesh Lod " + lodLevel;
        MeshFilter mf = this.GetComponent<MeshFilter>();
        mf.sharedMesh = lodMesh;

    }

    private int GetEdgeMask()
    {
        int kDirectionLeftFlag = 1 << 0;
        int kDirectionRightFlag = 1 << 1;
        int kDirectionUpFlag = 1 << 2;
        int kDirectionDownFlag = 1 << 3;

        if (left != null && left.lodLevel > lodLevel)
            kDirectionLeftFlag = 0;
        if (right != null && right.lodLevel > lodLevel)
            kDirectionRightFlag = 0;
        if (top != null && top.lodLevel > lodLevel)
            kDirectionUpFlag = 0;
        if (bot != null && bot.lodLevel > lodLevel)
            kDirectionDownFlag = 0;

        return (kDirectionLeftFlag | kDirectionRightFlag | kDirectionUpFlag | kDirectionDownFlag);
    }

   
    /****************************************************************************************
     * 功能 ： 从地形数据中创建地形对象
     ****************************************************************************************/
    static public LODTerrain CreateTerrainGameObject(LODTerrainData terrainData, bool useTrrainData = false)
    {
        if (terrainData == null) return null;
        if (terRoot == null)
            terRoot = new GameObject("TerrainRoot");
        GameObject go = new GameObject();
        go.transform.parent = terRoot.transform;
        LODTerrain terrain = go.AddComponent<LODTerrain>();
        go.isStatic = true;
        terrain.terrainData = terrainData;
        terrain.terrainConfig = GameScene.mainScene.terrainConfig;

        terrain.mesh = new Mesh();
        if (GameScene.isPlaying == true)
        {
            if (terrainData.vertices != null)
                terrain.mesh.vertices = terrainData.vertices;
            if (terrainData.normals != null)
                terrain.mesh.normals = terrainData.normals;
            if (terrainData.tangents != null)
                terrain.mesh.tangents = terrainData.tangents;

            terrain.vertices = terrainData.vertices;
            terrain.uvs = terrainData.uvs;
            terrain.normals = terrainData.normals;
            terrain.tangents = terrainData.tangents;

            terrain.BuildUVs(); 
            //terrain.BuildTriangles();
        }
        else
        {
#if UNITY_EDITOR
            if (shareVertices == null)
                BuildShareVertices();
            if (terrainData.vertices != null)
                terrain.vertices = terrainData.vertices;
            else
                terrain.vertices = shareVertices;
            terrain.normals = terrainData.normals;
            terrain.tangents = terrainData.tangents;

            terrain.mesh.vertices = terrain.vertices;
            terrain.BuildUVs();                                     // UV缓冲数据启用共享

            terrain.mesh.normals = terrain.normals;
            terrain.mesh.tangents = terrain.tangents;
            terrain.BuildTriangles();
#endif
        }

        go.AddComponent<MeshFilter>().mesh = terrain.mesh;
        terrain.terrainRenderer = go.AddComponent<MeshRenderer>();
        if (GameScene.isPlaying == true)
        {
            terrain.terrainRenderer.enabled = false;
            terrain.terrainRenderer.receiveShadows = true;
            terrain.terrainRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        return terrain;
    }

    /*******************************************************************************************
     * 功能 ： 销毁地形
     *******************************************************************************************/
    public void Destroy()
    {
        mbReBuildRT = false;
		if (mlodRT != null)
		{
            Terrainmapping.Cancel(mlodRT);
			if (bakeMat != null) 
			{
				bakeMat.SetTexture ("_MainTex", null);
				UnityEngine.Object.DestroyImmediate (bakeMat);
				bakeMat = null;
			}
            mlodRT = null;
		}

        if (GameScene.mainScene != null)
        {
            GameScene.mainScene.mBumpChangeDgate.RemoveCalls(QaualityChange);
        }

        this.terrainData.Release();
        this.terrainData = null;

        this.uvs = null;
        this.vertices = null;
        this.normals = null;
        this.triangles = null;
        this.tangents = null;

        // 销毁材质
        GameObject.Destroy(matrial);
        matrial = null;

        // 销毁MESH
        GameObject.Destroy(mesh);
        mesh = null;
    }

    /*****************************************************************************************
     * 功能 ：LOD 0 的三角形索引
     *****************************************************************************************/
    static private void BuildShareTriangles()
    {
        int _segmentsW = 16;
        int _segmentsH = 16;

        int numInds = 0;
        int baseInd;


        int[] triangles = new int[_segmentsH * _segmentsW * 6];

        int step = 1;           // 原始step
        int hStep = step;
        int vStep = (_segmentsW + 1) * step;

        for (int zi = 0; zi <= _segmentsH; zi += step)
        {
            for (int xi = 0; xi <= _segmentsW; xi += step)
            {
                if (xi != _segmentsW && zi != _segmentsH)
                {
                    baseInd = xi + zi * (_segmentsW + 1);
                    triangles[numInds++] = baseInd;
                    triangles[numInds++] = baseInd + vStep;
                    triangles[numInds++] = baseInd + vStep + hStep;
                    triangles[numInds++] = baseInd;
                    triangles[numInds++] = baseInd + vStep + hStep;
                    triangles[numInds++] = baseInd + hStep;
                }
            }
        }
        shareTriangles = triangles;

        // shareTriangles = TerrainIndexGenerator.GetTriangles(15, 0, 0);
    }

    static private void BuildShareUVS()
    {
        int _segmentsH = 16;
        int _segmentsW = 16;

        int numUvs = (_segmentsH + 1) * (_segmentsW + 1);
        Vector2[] uvs = new Vector2[numUvs];

        numUvs = 0;
        for (int yi = 0; yi <= _segmentsH; ++yi)
        {
            for (int xi = 0; xi <= _segmentsW; ++xi)
            {
                uvs[numUvs++] = new Vector2(((float)xi / (float)_segmentsW), (1f - (float)yi / (float)_segmentsH));
            }
        }
        shareUVS = uvs;
    }

    /***********************************************************************************************
     * 功能 : 创建材质
     ************************************************************************************************/
    public void BuildMaterial(WaterData waterData = null)
    {
        int bumpCount = terrainConfig.bumpCount;

        // 创建地形材质
        if (bumpCount > 0)
            matrial = new Material(Shader_RealLightWithBumped4);
        else
            matrial = new Material(realLightShader);

        Texture2D splatsmapTex = null;

        // 游戏运行时从本地读取溅斑纹理
        if (GameScene.isPlaying == true)
        {
            splatsmapTex = AssetLibrary.Load(splatsMapPath, AssetType.Texture2D, LoadType.Type_Auto).texture2D;
        }
#if UNITY_EDITOR
        else
        {
            splatsmapTex = terrainData.splatsmapTex;
        }
#endif
        if (splatsmapTex != null)
        {
            // 设置溅斑纹理
            matrial.SetTexture("_Control", splatsmapTex);
        }

        if (bumpCount == 2)
            bumpCount = 4;

        for (int i = 0; i < terrainData.splats.Length; i++)
        {
            Splat splat = terrainData.splats[i];
            if (splat != null)
            {
                string strKey = DelegateProxy.StringBuilder("_Splat", i);
                matrial.SetTexture(strKey, splat.texture);
                matrial.SetTextureScale(strKey, new Vector2(splat.tilingOffset.x, splat.tilingOffset.y));
                matrial.SetTextureOffset(strKey, Vector2.zero);
                matrial.SetVector("m_uv_Splat" + i, splat.tilingOffset);

                if (i < bumpCount)
                    matrial.SetTexture("_Normal" + i, splat.normalMap);
            }
            else
            {
                LogSystem.LogWarning("LODTerrain OnLoadTexComplete is null");
            }
        }

        if (bumpCount > 0)
        {
            matrial.SetColor("_SpecColor", terrainConfig.terrainSpecColor);
            matrial.SetFloat("_Shininess", terrainConfig.terrainShininess);
            matrial.SetFloat("_Glossiness", terrainConfig.terrainGloss);
        }
        GetComponent<Renderer>().material = matrial;
    }

    /// <summary>
    /// 是否需要bake
    /// </summary>
    public bool bBake
    {
        get
        {
            //高中配不bake
            if (terrainConfig.bumpCount==2)
                return false;

            //不显示不bake
            if(!terrainRenderer.enabled)
                return false;

            //已bake过 && 不用重build
            if (mlodRT != null && !mbReBuildRT)
                return false;

            return true;
        }
    }

    /// <summary>
    /// 是否需要立即bake(为修复bake时画面闪的问题,原因是1024RT只有一张，在下次使用时必需先释放)
    /// </summary>
    public bool bImmediatelyBake
    {
        get
        {
            return mlodRT != null && mbReBuildRT && !mbCenterTerrain;
        }
    }

    /// <summary>
    /// bake
    /// </summary>
    public void Bake()
    {
        //已经bake过&&不用rebuild不处理
        if (mlodRT != null && !mbReBuildRT)
            return;

        if (mlodRT != null)
        {
            Terrainmapping.Cancel(mlodRT);
        }
        mlodRT = Terrainmapping.Bake(this, RendererSize);
        if (mlodRT != null)
        {
            bakeMat = new Material(bakeShader);
            bakeMat.SetTexture("_MainTex", mlodRT);
            GetComponent<Renderer>().material = bakeMat;
        }
        mbReBuildRT = false;
    }

    /// <summary>
    /// 贴图大小
    /// </summary>
    public int RendererSize
    {
        get
        {
            if (SystemSetting.ImageQuality == GameQuality.SUPER_LOW)
            {
                return (int)Terrainmapping.RT_Type.MIN;
            }
            //不是中心地块使用512
            if (!mbCenterTerrain)
            {
                return (int)Terrainmapping.RT_Type.MIN;
            }
            return (int)Terrainmapping.RT_Type.MAX;
        }
    }

    public void CancelBake()
    {
        if (mlodRT != null)
        {
            GetComponent<Renderer>().material = matrial;
            Terrainmapping.Cancel(mlodRT);
            bakeMat.SetTexture("_MainTex", mlodRT);
            UnityEngine.Object.DestroyImmediate(bakeMat);
            bakeMat = null;
            mlodRT = null;
        }
        mbReBuildRT = false;
    }

    #region 编译器

#if UNITY_EDITOR

    static private Dictionary<string, Vector3> sideNormals = new Dictionary<string, Vector3>();

    private Shader bakeNoShadowShader = null;

    private Vector3 nCenter;

    private int TriCount_Lod_0 = 16 * 16 * 2 * 3;

   

    /** 地形中是否包含水 */
    public bool hasWater = false;

    public Vector4 lightmapTilingOffset;


    private float[] _faceNormals;
    private float[] _faceWeights;
    private float[] _faceTangents;

    /// <summary>
    /// 编译器方法
    /// </summary>
    static public void ClearSideNormals()
    {
        sideNormals.Clear();
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    static private void BuildShareVertices()
    {
        int _segmentsW = 16;
        int _segmentsH = 16;
        float x, y, z;
        int numVerts = (_segmentsH + 1) * (_segmentsW + 1);
        shareVertices = new Vector3[numVerts];

        numVerts = 0;

        for (int zi = 0; zi <= _segmentsH; ++zi)
        {
            for (int xi = 0; xi <= _segmentsW; ++xi)
            {
                // -.5f 以中心点为顶点原点
                x = ((float)xi / (float)_segmentsW - 0.5f) * 32f;
                z = ((float)zi / (float)_segmentsH - 0.5f) * 32f;
                y = GameScene.mainScene.terrainConfig.defaultTerrainHeight;

                shareVertices[numVerts] = new Vector3(x, y, z);
                numVerts++;
            }
        }
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    public void Init()
    {
        this.BuildGeometry();
        this.BuildTriangles();
        this.BuildUVs();
        this.BuildNormals();
        this.BuildTangents();
        this.BuildMaterial();
    }

    /*****************************************************************************************
     * 功能 ： 创建几何体
     * 编译器方法
     *****************************************************************************************/
    public void BuildGeometry()
    {
        if (mesh == null)
            mesh = new Mesh();

        int _segmentsW = LODTerrainData.segmentsW;
        int _segmentsH = LODTerrainData.segmentsH;

        float x, z;
        int tw = _segmentsW + 1;
        int numVerts = (_segmentsH + 1) * tw;
        //float uDiv = (float)(terrainData.heightmapResolution) / _segmentsW;
        //float vDiv = (float)(terrainData.heightmapResolution) / _segmentsH;
        //float u, v;
        float y;

        vertices = new Vector3[numVerts];

        numVerts = 0;

        TerrainData sceneTerrainData = GameScene.mainScene.terrainData;
        float halfSceneWidth = (float)terrainConfig.sceneWidth * 0.5f;
        float halfSceneHeight = (float)terrainConfig.sceneHeight * 0.5f;
        float wScale = (float)sceneTerrainData.heightmapResolution / (float)terrainConfig.sceneWidth;
        float hScale = (float)sceneTerrainData.heightmapResolution / (float)terrainConfig.sceneHeight;

        for (int zi = 0; zi <= _segmentsH; ++zi)
        {
            for (int xi = 0; xi <= _segmentsW; ++xi)
            {
                // -.5f 以中心点为顶点原点
                x = ((float)xi / (float)_segmentsW - 0.5f) * terrainData.size.x;
                z = ((float)zi / (float)_segmentsH - 0.5f) * terrainData.size.z;

                y = GameScene.mainScene.terrainData.GetHeight(Mathf.RoundToInt((x + transform.position.x + halfSceneWidth) * wScale), Mathf.RoundToInt((z + transform.position.z + halfSceneHeight) * hScale));

                vertices[numVerts] = new Vector3(x, y, z);
                numVerts++;
            }
        }

        mesh.vertices = vertices;

        // 更新碰撞体
        if (GameScene.isEditor == true)
        {
            if (gameObject != null)
            {
                MeshCollider mc = gameObject.GetComponent<MeshCollider>();
                if (mc != null)
                {
                    DestroyImmediate(mc);
                    gameObject.AddComponent<MeshCollider>();
                }
            }
        }
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    private void BuildTangents()
    {
        UpdateFaceTangents();
        UpdateVertexTangents();
        mesh.tangents = tangents;
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    public void BuildNormals()
    {
        UpdateFaceNormals();
        UpdateVertexNormals();
        mesh.normals = normals;
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    /// <param name="center"></param>
    /// <param name="range"></param>
    public void BuildNormals(Vector3 center, float range)
    {
        nCenter = center;

        UpdateFaceNormals();
        UpdateVertexNormals(range);

        mesh.normals = normals;
    }

    /**************************************************************************************************
     * 功能 : 更新顶点的切线
     * 编译器方法
     ***************************************************************************************************/
    private void UpdateVertexTangents()
    {
        int i = 0, k = 0;
        int lenV = vertices.Length;

        tangents = new Vector4[lenV];

        while (i < lenV)
        {
            tangents[i] = new Vector4(0f, 0f, 0f, -1f);
            i++;
        }

        int lenI = TriCount_Lod_0;
        int index;
        float weight;
        int f1 = 0, f2 = 1, f3 = 2;

        i = 0;
        while (i < lenI)
        {
            weight = _faceWeights[k++];

            index = triangles[i++];
            tangents[index].x += _faceTangents[f1] * weight;
            tangents[index].y += _faceTangents[f2] * weight;
            tangents[index].z += _faceTangents[f3] * weight;

            index = triangles[i++];
            tangents[index].x += _faceTangents[f1] * weight;
            tangents[index].y += _faceTangents[f2] * weight;
            tangents[index].z += _faceTangents[f3] * weight;

            index = triangles[i++];
            tangents[index].x += _faceTangents[f1] * weight;
            tangents[index].y += _faceTangents[f2] * weight;
            tangents[index].z += _faceTangents[f3] * weight;
            f1 += 3;
            f2 += 3;
            f3 += 3;
        }

        i = 0;
        while (i < lenV)
        {
            float vx = tangents[i].x;
            float vy = tangents[i].y;
            float vz = tangents[i].z;
            float d = 1.0f / Mathf.Sqrt(vx * vx + vy * vy + vz * vz);
            tangents[i].x = vx * d;
            tangents[i].y = vy * d;
            tangents[i].z = vz * d;
            i++;
        }
    }

    /*******************************************************************************************
     * 功能 : 更新面的切线
     * 编译器方法
     *******************************************************************************************/
    protected void UpdateFaceTangents()
    {
        int i = 0;
        int index1, index2, index3;
        int len = TriCount_Lod_0;

        float v0;
        float dv1, dv2;
        float denom;
        float x0, y0, z0;
        float dx1, dy1, dz1;
        float dx2, dy2, dz2;
        float cx, cy, cz;

        _faceTangents = new float[len];

        while (i < len)
        {
            index1 = triangles[i];
            index2 = triangles[i + 1];
            index3 = triangles[i + 2];

            // 计算UV跨度(w, h)
            v0 = uvs[index1].y;
            dv1 = uvs[index2].y - v0;
            dv2 = uvs[index3].y - v0;

            // 计算三角形ABC向量 AB AC
            x0 = vertices[index1].x;
            y0 = vertices[index1].y;
            z0 = vertices[index1].z;

            dx1 = vertices[index2].x - x0;
            dy1 = vertices[index2].y - y0;
            dz1 = vertices[index2].z - z0;

            dx2 = vertices[index3].x - x0;
            dy2 = vertices[index3].y - y0;
            dz2 = vertices[index3].z - z0;

            cx = dv2 * dx1 - dv1 * dx2;
            cy = dv2 * dy1 - dv1 * dy2;
            cz = dv2 * dz1 - dv1 * dz2;

            // 标准化切线
            denom = 1 / Mathf.Sqrt(cx * cx + cy * cy + cz * cz);
            _faceTangents[i++] = denom * cx;
            _faceTangents[i++] = denom * cy;
            _faceTangents[i++] = denom * cz;
        }
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    /// <param name="range"></param>
    protected void UpdateVertexNormals(float range = -1f)
    {
        int v1 = 0;
        int f1 = 0, f2 = 1, f3 = 2;
        int lenV = vertices.Length;

        if (normals == null)
        {
            normals = new Vector3[lenV];
            while (v1 < lenV)
            {
                normals[v1] = new Vector3(0f, 0f, 0f);
                v1++;
            }
        }

        int i = 0, k = 0;
        int lenI = TriCount_Lod_0;
        int index;
        float weight;

        while (i < lenI)
        {
            weight = _faceWeights[k++];

            index = triangles[i++];
            if (InRange(index, range))
            {
                normals[index].x += _faceNormals[f1] * weight;
                normals[index].y += _faceNormals[f2] * weight;
                normals[index].z += _faceNormals[f3] * weight;
            }

            index = triangles[i++];
            if (InRange(index, range))
            {
                normals[index].x += _faceNormals[f1] * weight;
                normals[index].y += _faceNormals[f2] * weight;
                normals[index].z += _faceNormals[f3] * weight;
            }
            index = triangles[i++];
            if (InRange(index, range))
            {
                normals[index].x += _faceNormals[f1] * weight;
                normals[index].y += _faceNormals[f2] * weight;
                normals[index].z += _faceNormals[f3] * weight;
            }
            f1 += 3;
            f2 += 3;
            f3 += 3;
        }

        v1 = 0;
        while (v1 < lenV)
        {
            Vector3 vert = vertices[v1];

            if (Mathf.Abs(vert.x) > 15f || Mathf.Abs(vert.z) > 15f)
            {
                vert = transform.localToWorldMatrix.MultiplyPoint3x4(vert);
                if (range > 0f)
                {
                    float dx = vert.x - nCenter.x;
                    float dz = vert.z - nCenter.z;
                    float dis = Mathf.Sqrt(dx * dx + dz * dz);                 // 加上剔除因子倍数

                    if (dis > range)
                    {
                        v1++;
                        continue;
                    }
                }

                if (HasSideNormal(vert.x, vert.z) == false)
                {
                    float vx = normals[v1].x;
                    float vy = normals[v1].y;
                    float vz = normals[v1].z;
                    float d = 1.0f / Mathf.Sqrt(vx * vx + vy * vy + vz * vz);
                    normals[v1].x = vx * d;
                    normals[v1].y = vy * d;
                    normals[v1].z = vz * d;

                    AddSideNormal(vert.x, vert.z, normals[v1]);
                }
                else
                {
                    normals[v1].x = GetSideNormal(vert.x, vert.z).x;
                    normals[v1].y = GetSideNormal(vert.x, vert.z).y;
                    normals[v1].z = GetSideNormal(vert.x, vert.z).z;
                }
            }
            else
            {
                vert = transform.localToWorldMatrix.MultiplyPoint3x4(vert);
                if (range > 0f)
                {
                    float dx = vert.x - nCenter.x;
                    float dz = vert.z - nCenter.z;
                    float dis = Mathf.Sqrt(dx * dx + dz * dz);                 // 加上剔除因子倍数

                    if (dis > range)
                    {
                        v1++;
                        continue;
                    }
                }
                float vx = normals[v1].x;
                float vy = normals[v1].y;
                float vz = normals[v1].z;
                float d = 1.0f / Mathf.Sqrt(vx * vx + vy * vy + vz * vz);
                normals[v1].x = vx * d;
                normals[v1].y = vy * d;
                normals[v1].z = vz * d;
            }
            v1++;
        }
    }

    /*****************************************************************************************
     * 功能 ： 复制地形数据
     * 编译器方法
     *****************************************************************************************/
    private void CopyTo(TerrainData target)
    {
        float s = 17f / 32f;
        int i = 0;
        int j = 0;
        int k = 0;
        int l = 0;
        for (i = 0; i < 32; i++)
        {
            for (j = 0; j < 32; j++)
            {
                k = (int)((float)i * s);
                l = (int)((float)j * s);
                terrainData.heightmap[j, i] = this.vertices[l * 17 + k].y;
            }
        }

        int startX = (int)transform.position.x - 16 + terrainConfig.sceneWidth / 2;
        int startY = (int)transform.position.z - 16 + terrainConfig.sceneHeight / 2;
        target.SetHeights(startX, startY, terrainData.heightmap);
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    private void WithoutShadow()
    {
        terrainRenderer.receiveShadows = false;
        if (bakeMat != null)
            bakeMat.shader = bakeNoShadowShader;
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    private void ReceiveShadow()
    {
        terrainRenderer.receiveShadows = true;
        if (bakeMat != null)
            bakeMat.shader = bakeShader;
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private bool HasSideNormal(float x, float z)
    {
        string key = x + "_" + z;
        if (sideNormals.ContainsKey(key))
            return true;
        return false;
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    /// <param name="ind"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    private bool InRange(int ind, float range)
    {

        Vector3 vert = vertices[ind];
        vert = transform.localToWorldMatrix.MultiplyPoint3x4(vert);
        if (range > 0f)
        {
            float dx = vert.x - nCenter.x;
            float dz = vert.z - nCenter.z;
            float dis = Mathf.Sqrt(dx * dx + dz * dz);                 // 加上剔除因子倍数

            if (dis > range)
            {
                return false;
            }
        }
        return true;

    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    private void UpdateFaceNormals()
    {
        int i = 0, j = 0, k = 0;
        int index;
        int len = TriCount_Lod_0;
        float x1, x2, x3;
        float y1, y2, y3;
        float z1, z2, z3;
        float dx1, dy1, dz1;
        float dx2, dy2, dz2;
        float cx, cy, cz;
        float d;

        _faceNormals = new float[len];
        _faceWeights = new float[len / 3];

        while (i < len)
        {
            index = triangles[i++];

            // 顶点1
            x1 = vertices[index].x;
            y1 = vertices[index].y;
            z1 = vertices[index].z;

            // 顶点2
            index = triangles[i++];
            x2 = vertices[index].x;
            y2 = vertices[index].y;
            z2 = vertices[index].z;

            // 顶点3
            index = triangles[i++];
            x3 = vertices[index].x;
            y3 = vertices[index].y;
            z3 = vertices[index].z;

            // 计算叉乘后结果
            dx1 = x3 - x1;
            dy1 = y3 - y1;
            dz1 = z3 - z1;
            dx2 = x2 - x1;
            dy2 = y2 - y1;
            dz2 = z2 - z1;
            cx = dz1 * dy2 - dy1 * dz2;
            cy = dx1 * dz2 - dz1 * dx2;
            cz = dy1 * dx2 - dx1 * dy2;

            d = Mathf.Sqrt(cx * cx + cy * cy + cz * cz);

            float w = d * 10000;
            if (w < 1)
                w = 1;
            _faceWeights[k++] = w;

            d = 1 / d;
            _faceNormals[j++] = cx * d;
            _faceNormals[j++] = cy * d;
            _faceNormals[j++] = cz * d;
        }
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="normal"></param>
    private void AddSideNormal(float x, float z, Vector3 normal)
    {
        string key = x + "_" + z;
        if (sideNormals.ContainsKey(key))
            return;

        sideNormals[key] = normal;
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private Vector3 GetSideNormal(float x, float z)
    {
        string key = x + "_" + z;
        if (sideNormals.ContainsKey(key))
            return sideNormals[key];
        else
            return Vector3.zero;
    }

#endif
    #endregion
}
