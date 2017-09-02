using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


/*******************************************************************************
 * 功能 ： 地域
 * 地域在载入场景的时候，也会加载光照贴图
 *******************************************************************************/
public class Region
{

#region 加载法线相关

    ///----------------------------------
    ///用途:统一加载法线图，不交给各区域加载
    ///1.第一次进入场景判断是否有变化再加载法线
    ///2.发生变化时只加载一次
    ///----------------------------------

    /// <summary>
    /// 加载贴图回调
    /// </summary>
    private static UserDelegate LoadCombinTexCallback = new UserDelegate();
    /// <summary>
    /// 0.未加载
    /// 1.加载中
    /// 2.加载完成
    /// </summary>
    private static int iCombinLoadState = 0;

    /// <summary>
    /// 高光法线贴图
    /// </summary>
    private static UnityEngine.Object texCombinTerrain = null;
    
    /// <summary>
    /// 当前设置法线
    /// </summary>
    private static int iCurrentBumpCount = 0;

    /// <summary>
    /// 设置当前法线.
    /// 用途:避免出现重复加载(当场景设置与高中低配设置一值则不加载)
    /// </summary>
    /// <param name="iBumpCount"></param>
    public static void SetCurrentBumpCount(int iBumpCount)
    {
        iCurrentBumpCount = iBumpCount;
    }

    /// <summary>
    /// update设置bump
    /// </summary>
    /// <param name="BumpCount"></param>
    public static void SetBumpCount(int iBumpCount)
    {
        if (iCurrentBumpCount == iBumpCount)
            return;

        iCurrentBumpCount = iBumpCount;
        iCombinLoadState = 0;
        LoadBumpCount();
    }

    /// <summary>
    /// 清除信息
    /// </summary>
    public static void ClearBump()
    {
        if (texCombinTerrain != null)
        {
            CacheObjects.PopCache(texCombinTerrain);
            texCombinTerrain = null;
        }
        iCurrentBumpCount = 0;
        iCombinLoadState = 0;
        LoadCombinTexCallback.ClearCalls();
    }

    /// <summary>
    /// 加载
    /// </summary>
    private static void LoadBumpCount()
    {
        string strFileName = "";
        if (iCurrentBumpCount > 0)
            strFileName = "Scenes/" + GameScene.mainScene.sceneID + "/Textures/CombinTerrainWithoutBump";
        else
            strFileName = "Scenes/" + GameScene.mainScene.sceneID + "/Textures/CombinTerrainWithBump";

        iCombinLoadState = 1;

        if (texCombinTerrain != null)
            CacheObjects.PopCache(texCombinTerrain);

        GameObjectUnit.ThridPardLoad(strFileName, OnLoadMTexComplete, null, true);
    }

    /// <summary>
    /// 加载tex
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="strFileName"></param>
    /// <param name="varStore"></param>
    private static void OnLoadMTexComplete(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        iCombinLoadState = 2;
        texCombinTerrain = oAsset;
        VarStore varStore1 = VarStore.CreateVarStore();
        varStore1 += oAsset;
        LoadCombinTexCallback.ExecuteCalls(varStore1);
        varStore1.Destroy();
    }

    /// <summary>
    /// 设置回调
    /// </summary>
    /// <param name="CallBack"></param>
    private static void AddBumpChange(UserDelegate.OnCallBack CallBack)
    {
        LoadCombinTexCallback.AppendCalls(CallBack);
        if (iCombinLoadState == 2)
        {
            VarStore varStore = VarStore.CreateVarStore();
            varStore += texCombinTerrain;
            CallBack(varStore);
            varStore.Destroy();
        }
    }
#endregion


    MemoryStream editorme = null;
    BinaryReader editorbr = null;

    public GameScene scene;

    public Vector3 postion;

    public int regionX = -1;
    public int regionY = -1;

    public float actualX = 0;
    public float actualY = 0;

    /** 地域光照贴图数据 */
    public int lightmapCount = 0;

    /** 地域切片数据 */
    public Tile[] tiles;

    public string regionDataPath = "";

    /// <summary>
    /// lod材质
    /// </summary>
    public Material matrial;
    public string mstrKey
    {
        private set;
        get;
    }

    public int miKey
    {
        get
        {
            return Region.GetRegionKey(regionX, regionY);
        }
    }

    /// <summary>
    /// 获取regionkey
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static int GetRegionKey(int x, int z)
    {
        int iKey = x & 0xffff;
        iKey |= z << 16;
        return iKey;
    }

    /// <summary>
    /// 标记是否删除
    /// </summary>
    private bool mbDestroy = false;

    /// <summary>
    /// 加载状态
    /// 0未开始
    /// 1加载中
    /// 2加载完成
    /// </summary>
    private int miLoadCombinState = 0;

    public Region()
    {

    }


    /** 获取指定位置的地形切片 */
    private float localX = 0f;
    private float localY = 0f;
    private int r = 0;
    private int c = 0;
    private int tileInd = 0;

    public Tile GetTile(Vector3 worldPosition)
    {
        try
        {
            localX = worldPosition.x - actualX + scene.terrainConfig.regionSize * 0.5f;
            localY = worldPosition.z - actualY + scene.terrainConfig.regionSize * 0.5f;

            r = Mathf.FloorToInt(localX / scene.terrainConfig.tileSize);
            c = Mathf.FloorToInt(localY / scene.terrainConfig.tileSize);

            if (r > scene.terrainConfig.tileCountPerSide)
                r = scene.terrainConfig.tileCountPerSide;
            if (c > scene.terrainConfig.tileCountPerSide)
                c = scene.terrainConfig.tileCountPerSide;

            tileInd = c * scene.terrainConfig.tileCountPerSide + r;

            if (tileInd > (tiles.Length - 1))
                return null;

            return tiles[tileInd];
        }
        catch (Exception e)
        {
            LogSystem.Log("获取地形切片错误 :  " + e.ToString());
            return null;
        }
    }

    public Tile GetTile(int tileX, int tileY)
    {
        return tiles[tileX + tileY * scene.terrainConfig.tileCountPerSide];
    }

    /**************************************************************************************************
     * 功能 : 查找单位
     * 注 ：地域没有载入到场景中，切片读取单位判断单位是否已经被创建过，次数需要直接调用地域获取单位
     **************************************************************************************************/
    public GameObjectUnit FindUint(int createID)
    {
        int count = tiles.Length;
        GameObjectUnit unit = null;
        for (int i = 0; i < count; i++)
        {
            if (tiles[i] != null)
            {
                unit = tiles[i].FindUnit(createID);
                if (unit != null)
                    return unit;
            }
        }
        return null;
    }
#if UNITY_EDITOR
    /**************************************************************************************************
     * 功能 ： 创建新的地域
     * 编译器使用
     **************************************************************************************************/
    static public Region Create(GameScene scene, int regionX, int regionY)
    {
        Region region = new Region();
        region.scene = scene;
        region.tiles = new Tile[scene.terrainConfig.tileCountPerRegion];
        region.regionX = regionX;
        region.regionY = regionY;
        region.mstrKey = regionX + "_" + regionY;
        region.regionDataPath = "Scenes/" + scene.sceneID + "/" + regionX + "_" + regionY + "/Region";

        // 计算地域实际的位置
        region.actualX = regionX * scene.terrainConfig.regionSize;
        region.actualY = regionY * scene.terrainConfig.regionSize;

        region.postion = Vector3.zero;
        region.postion.x = region.actualX;
        region.postion.y = 0;
        region.postion.z = region.actualY;

        int div = Mathf.FloorToInt(scene.terrainConfig.tileCountPerSide * 0.5f);

        // 创建地形切片
        for (int i = 0; i < scene.terrainConfig.tileCountPerSide; i++)
        {
            for (int j = 0; j < scene.terrainConfig.tileCountPerSide; j++)
            {
                Tile tile = Tile.Create(region, i - div, j - div);
                region.tiles[j * scene.terrainConfig.tileCountPerSide + i] = tile;
            }
        }

        return region;
    }

#endif

    /************************************************************************************************
     * 功能 : 更新剔除距离
     * (编译器使用)
     *************************************************************************************************/
    public void UpdateViewRange()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i] != null)
                tiles[i].UpdateViewRange();
        }
    }

    /***************************************************************************************************
     * 功能 ： 读取地域数据
     ***************************************************************************************************/
    public Region Read(byte[] bytes)
    {
        miLoadCombinState = 0;
        SetBumpChangeCallback();
        long pos = 0;
        MemoryStream ms = new MemoryStream(bytes);
        BinaryReader br = new BinaryReader(ms);
        int sceneID = 0;
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10013)
        {
            sceneID = br.ReadInt32();
        }
        else
        {
            sceneID = GameScene.mainScene.sceneID;
            br.BaseStream.Position = pos;
        }

        this.scene = GameScene.mainScene;
        this.tiles = new Tile[scene.terrainConfig.tileCountPerRegion];
        regionX = br.ReadInt32();
        regionY = br.ReadInt32();
        mstrKey = regionX + "_" + regionY;

        regionDataPath = DelegateProxy.StringBuilder("Scenes/", sceneID, "/", mstrKey, "/Region");
        string editorRegionDataPath = string.Empty;
#if UNITY_EDITOR
        editorRegionDataPath = DelegateProxy.StringBuilder("Assets/Res/Scenes/", sceneID, "/", MathUtils.GetIntString(regionX), "_", MathUtils.GetIntString(regionY), "/Region");
#endif

        // 计算地域实际的位置
        actualX = regionX * scene.terrainConfig.regionSize;
        actualY = regionY * scene.terrainConfig.regionSize;
            
        lightmapCount = br.ReadInt32();

        int count = 0;
        int div = Mathf.FloorToInt(scene.terrainConfig.tileCountPerSide * 0.5f);

        ReadEditorData(sceneID, editorRegionDataPath);

        // 创建地形切片
		/**
		for (int i = 0; i < scene.terrainConfig.tileCountPerSide; i++)
		{
			for (int j = 0; j < scene.terrainConfig.tileCountPerSide; j++)
			{
				Tile tile = Tile.Create(this, i - div, j - div);
				this.tiles[j * scene.terrainConfig.tileCountPerSide + i] = tile;
			}
		}
        **/
        // 读取地形域中的切片元数据
        while (br.BaseStream.Position < br.BaseStream.Length && br.ReadString() == "Tile")
        {
            Tile tile = new Tile(this);

            tile.Read(br, editorbr);
            tile.SetNeighborTile();
            int r = tile.tileX + div;
            int c = tile.tileY + div;
            tiles[c * scene.terrainConfig.tileCountPerSide + r] = tile;

            count++;
            // 防止读到文件尾，报错		
            if (count >= scene.terrainConfig.tileCountPerRegion)
                break;
        }

        if (count < scene.terrainConfig.tileCountPerRegion)
            LogSystem.LogWarning("tile data missing!");
        postion = Vector3.zero;
        postion.x = actualX;
        postion.y = 0;
        postion.z = actualY;

        br.Close();
        ms.Flush();
        if (editorbr != null)
        {
            editorbr.Close();
        }
        if (editorme != null)
        {
            editorme.Close();
        }
        return this;
    }

    private void ReadEditorData(int sceneID, string path)
    {
        if (Application.isPlaying)
        {
            return;
        }
#if  UNITY_EDITOR
        UnityEngine.Object oAsset = AssetDatabase.LoadAssetAtPath(path + ".bytes", typeof(UnityEngine.Object));
        if (oAsset != null)
        {
            TextAsset textAsset = oAsset as TextAsset;
            if (textAsset != null && textAsset.bytes != null)
            {
                editorme = new MemoryStream(textAsset.bytes);
                editorbr = new BinaryReader(editorme);
            }
        }
#endif


    }


    /************************************************************************************************
     * 功能 ： 销毁区域
     ************************************************************************************************/
    public void Destroy()
    {
        if (tiles == null)
            return;

        mbDestroy = true;
        miLoadCombinState = 0;
        Tile tile;
        for (int i = 0; i < tiles.Length; i++)
        {
            tile = tiles[i];
            if (tile == null)
                continue;

            // 如果场景保护地形切片则从场景中销毁
            if (scene.ContainTile(tile) == true)
                scene.RemoveTile(tile, true);
            else
                tile.Destroy();
        }

        matrial = null;

        if (combinTerObj != null)
        {
            CacheObjects.DestoryPoolObject(combinTerObj);
            combinTerObj = null;
        }
        this.tiles = null;
        this.scene = null;
    }

    /************************************************************************************************
     * 功能 ： 更新地域
     ************************************************************************************************/

    //private int tilePerFrame = 0;
    private float dx = 0;
    private float dz = 0;
    public void Update(Vector3 eyePos)
    {
        //1.6kb
        if (tiles == null)
            return;
        //tilePerFrame = 0;
        int len = tiles.Length;
        Tile tile;
        for (int i = 0; i < len; i++)
        {
            tile = tiles[i];

            if (tile == null)
                continue;

            // 每次更新只创建指定数量地形
            dx = eyePos.x - tile.position.x;
            dz = eyePos.z - tile.position.z;
            // 检测切片是否在视野中
            tile.viewDistance = dx * dx + dz * dz;

            // 场景编辑状态下直接加载地形
            if (GameScene.isEditor == true && GameScene.isPlaying == false)
            {
                if (tile != null && tile.visible == false)
                {
                    if (scene.ContainTile(tile) == false)
                    {
                        scene.AddTile(tile);
                    }
                }
            }
            else
            {
                if (tile != null && tile.visible == false)
                {
#if NOLOD
                    // 已经包含切片不重复创建
                    if (scene.ContainTile(tile) == false)
                    {
                        scene.AddTile(tile);
                    }
#else
                     // 已经包含切片不重复创建
                    if (scene.ContainTile(tile) == false)
                    {
                        if (tile.viewDistance < (tile.far * tile.far))
                        {
                            scene.AddTile(tile);
                        }
                    }
#endif
                }
            }
        }

        if (miLoadCombinState == 0)
        {
#if NOLOD

#else
            LoadCombinTer();
#endif
            return;
        }
        // 检查是否可以显示当前区域的合并地形
        /**
        if (CanCombinTerrainMesh())
        {
            VisibleCombinTerrain();
        }
        else
        {
            InvisibleCombinTerrain();
        }**/

    }

    

    private bool CanCombinTerrainMesh()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].terrain != null)
            {
                if (tiles[i].terrain.NeighborDifferenLevel())
                    return false;
            }
        }
        return true;
    }

    /// <summary>
    /// lod模型
    /// </summary>
    public GameObject combinTerObj;

    private int lowestLevel = 1;

#if UNITY_EDITOR
    static private Shader terrainShader = Shader.Find("Snail/TerrainMobile");
    private Mesh lowLevelCombinMesh;
    private Material terrainMat = new Material(terrainShader);
    public Texture2D combinSplatsmapTex;
    private List<Splat> splats;
    public void GenCombinTerrainMesh()
    {
        TerrainConfig terCfg = GameScene.mainScene.terrainConfig;

        int segment = terCfg.terrainLODConfig.lodSegments[lowestLevel];
        int numVerts = (segment + 1) * (segment + 1);
        int kPatchSize = terCfg.terrainLODConfig.lodPatchSize[lowestLevel];

        int combinVertexCount = numVerts * terCfg.tileCountPerRegion;
        int combinTriangleCount = (kPatchSize * kPatchSize * 6) * terCfg.tileCountPerRegion;

        Vector3[] combinVertices = new Vector3[combinVertexCount];
        Vector2[] combinUVs = new Vector2[combinVertexCount];
        Vector2[] combinUV2s = new Vector2[combinVertexCount];
        Vector3[] combinNormals = new Vector3[combinVertexCount];
        Vector4[] combinTanents = new Vector4[combinVertexCount];
        int[] combinTriangles = new int[combinTriangleCount];

        lowLevelCombinMesh = new Mesh();

        int ind = 0;
        int triInd = 0;
        int triOffset = 0;
        Vector3 temVertex = new Vector3();
        Vector2 temUV = new Vector2();
        int half = (terCfg.tileCountPerSide - 1) / 2;
        float uvScale = 1f / (float)terCfg.tileCountPerSide;
        for (int i = 0; i < tiles.Length; i++)
        {
            LODTerrain terrain = tiles[i].terrain;

            Vector3[] lodVertices;
            Vector2[] lodUVs;
            Vector3[] lodNormals;
            Vector4[] lodTangents;
            int[] lodTriangles;

            float offX = tiles[i].tileX * terCfg.tileSize;
            float offZ = tiles[i].tileY * terCfg.tileSize;

            float uvOffX = (float)(tiles[i].tileX + half) / (float)terCfg.tileCountPerSide;
            float uvOffY = (float)(tiles[i].tileY + half) / (float)terCfg.tileCountPerSide;
            float per = 1f / (float)terCfg.tileCountPerSide;

            terrain.GenerateLODMesh(lowestLevel, 15, out lodVertices, out lodUVs, out lodNormals, out lodTangents, out lodTriangles);
            int vertexCount = lodVertices.Length;
            for (int j = 0; j < vertexCount; j++)
            {
                temVertex.x = lodVertices[j].x + offX;
                temVertex.z = lodVertices[j].z + offZ;
                temVertex.y = lodVertices[j].y;

                combinVertices[ind] = temVertex;

                temUV.x = lodUVs[j].x * uvScale + uvOffX;
                temUV.y = (1 - lodUVs[j].y) * uvScale + uvOffY;

                combinUVs[ind] = temUV;

                temUV.x = lodUVs[j].x * uvScale + uvOffX;
                temUV.y = lodUVs[j].y * uvScale + 1 - uvOffY - per;

                combinUV2s[ind] = temUV;
                combinNormals[ind] = lodNormals[j];
                combinTanents[ind] = lodTangents[j];
                ind++;
            }

            int triangleCount = lodTriangles.Length;
            for (int k = 0; k < triangleCount; k++)
            {
                combinTriangles[triInd] = lodTriangles[k] + triOffset;
                triInd++;
            }
            triOffset += vertexCount;
        }

        lowLevelCombinMesh.vertices = combinVertices;
        lowLevelCombinMesh.uv = combinUVs;
        lowLevelCombinMesh.uv2 = combinUV2s;
        lowLevelCombinMesh.normals = combinNormals;
        lowLevelCombinMesh.tangents = combinTanents;
        lowLevelCombinMesh.triangles = combinTriangles;

        if (combinTerObj == null)
        {
            combinTerObj = new GameObject();
            combinTerObj.transform.position = new Vector3(postion.x, postion.y, postion.z);
            combinTerObj.AddComponent<MeshFilter>();
            combinTerObj.AddComponent<MeshRenderer>();
        }

        MeshFilter mf = combinTerObj.GetComponent<MeshFilter>();
        mf.sharedMesh = lowLevelCombinMesh;

        BuildMaterial();
    }

    /// <summary>
    /// 编译器使用
    /// </summary>
    private void BuildMaterial()
    {
        TerrainConfig terrainConfig = GameScene.mainScene.terrainConfig;
        matrial = terrainMat;

        string p = "Scenes/" + GameScene.mainScene.sceneID + "/Textures/CombinTerrain";
        Texture2D mainTex = Resources.Load(p, typeof(Texture2D)) as Texture2D;
        matrial.SetTexture("_MainTex", mainTex);

        Vector2 scale = new Vector2();
        scale.x = 1f / 3f;
        scale.y = 1f / 3f;

        Vector2 offset = new Vector2();
        offset.x = (regionX + 1) * scale.x;
        offset.y = (regionY + 1) * scale.y;
        matrial.mainTextureScale = scale;
        matrial.mainTextureOffset = offset;

        MeshRenderer mr = combinTerObj.GetComponent<MeshRenderer>();
        mr.sharedMaterial = matrial;

        SetLightmap();
    }
#endif

    private void SetLightmap()
    {
        if (combinTerObj == null)
            return;

        TerrainConfig terrainConfig = GameScene.mainScene.terrainConfig;

        float sizeW = 1f / (float)terrainConfig.reginColumns;
        float sizeH = 1f / (float)terrainConfig.reginRows;
        float ox = (regionX + (terrainConfig.reginColumns - 1) / 2) * sizeW;
        float oy = (terrainConfig.reginRows - regionY - (terrainConfig.reginRows - 1) / 2 - 1) * sizeH;

        MeshRenderer mr = combinTerObj.GetComponent<MeshRenderer>();
        mr.lightmapIndex = tiles[0].lightmapPrototype.lightmapIndex;
        Vector4 tiof = Vector4.zero;
        tiof.x = sizeW;
        tiof.y = sizeH;
        tiof.z = ox;
        tiof.w = oy;

        mr.lightmapScaleOffset = tiof;

    }

    /// <summary>
    /// 显示load
    /// </summary>
    private void VisibleCombinTerrain()
    {
        if (miLoadCombinState == 0)
        {
#if NOLOD
            
#else
            LoadCombinTer();
#endif
            return;
        }
        
        //if (combinTerObj == null)   
            //GenCombinTerrainMesh();

        if (combinTerObj != null)
        {
            combinTerObj.SetActive(true);
            Tile tile;
            for (int j = 0; j < tiles.Length; j++)
            {
                tile = tiles[j];
                if (tile.terrain != null)
                {
                    tile.terrain.gameObject.SetActive(false);
                }
            } 
        }
    }

    /// <summary>
    /// 隐藏lod
    /// </summary>
    private void InvisibleCombinTerrain()
    {
        if (combinTerObj != null)
        {
            //combinTerObj.SetActive(false);
            Tile tile;
            for (int j = 0; j < tiles.Length; j++)
            {
                tile = tiles[j];
                if (tile.terrain != null)
                {
                    if (tile.terrain.lodLevel < lowestLevel)
                        tile.terrain.gameObject.SetActive(true);
                    else
                        tile.terrain.gameObject.SetActive(false);

                }
            }
        }
    }

    /// <summary>
    /// 加载lod
    /// </summary>
    private void LoadCombinTer()
    {
        string dir = string.Empty;
        if (GameScene.isPlaying)
        {
            dir = DelegateProxy.StringBuilder("Scenes/", GameScene.mainScene.sceneID, "/Prefabs/", mstrKey, "CombinTer");
        }
        else
        {
            dir = "Scenes/" + GameScene.mainScene.sceneID + "/Prefabs/" + mstrKey + "CombinTer";
        }
        miLoadCombinState = 1;
        GameObjectUnit.ThridPardLoad(dir, OnLoadCombinComplete, null, true);
    }

    /// <summary>
    /// 加载完成
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="strFileName"></param>
    /// <param name="varStore"></param>
    private void OnLoadCombinComplete(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        miLoadCombinState = 2;
        if (oAsset == null)
            return;

        if (mbDestroy)
        {
            CacheObjects.PopCache(oAsset);
            return;
        }
        
        combinTerObj = CacheObjects.InstantiatePool(oAsset) as GameObject;
        combinTerObj.transform.position = postion;
        MeshRenderer mr = combinTerObj.GetComponent<MeshRenderer>();
        matrial = mr.sharedMaterial;
        SetLightmap();
        //法线已加载完成
        if (Region.texCombinTerrain != null && matrial != null)
        {
            matrial.SetTexture("_MainTex", Region.texCombinTerrain as Texture);
        }
    }

#region 法线设置相关

    /// <summary>
    /// 回调变化材质
    /// </summary>
    private void SetBumpChangeCallback()
    {
        Region.AddBumpChange(BumpCountChanage);
    }

    /// <summary>
    /// 法线加载完成回调
    /// </summary>
    /// <param name="args"></param>
    private void BumpCountChanage(VarStore varStore)
    {
        if (varStore == null || varStore.GetVarCount() == 0)
        {
            //LogSystem.LogWarning("Region::BumpCountChanage mainTex is null 1");
            return;
        }

        Texture tex = varStore.GetUnityObject(0) as Texture;
        if(tex == null)
        {
            //LogSystem.LogWarning("Region::BumpCountChanage mainTex is null 2");
            return;
        }

        if (mbDestroy)
            return;

        if (matrial == null)
            return;

        matrial.SetTexture("_MainTex", tex);
    }
#endregion

}