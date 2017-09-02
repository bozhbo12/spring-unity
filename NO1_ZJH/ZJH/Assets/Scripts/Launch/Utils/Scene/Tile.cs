using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;


/**************************************************************************************************
 * 类 : 地形切片
 **************************************************************************************************/
public class Tile
{
    public string key = "";

    public int tileX = 0;
    public int tileY = 0;

    public float viewDistance = 0f;

    /** 地形切片的位置,也是切片的中心点 */
    public Vector3 position = Vector3.zero;

    public float far = 0f;

    /** 切片包含的单位 */
    private Dictionary<int, GameObjectUnit> unitsMap = new Dictionary<int, GameObjectUnit>();

    //切片中  单元
    public List<GameObjectUnit> units = new List<GameObjectUnit>();

    public int unitCount = 0;

    public Region region;

    public string path = "";

    public bool visible = false;

#if UNITY_EDITOR
    public Bounds bounds;
#endif

    public short[,] heights;
    public byte[,] grids;

    public Tile left;
    public Tile right;
    public Tile top;
    public Tile bot;
    public Tile top_left;
    public Tile top_right;
    public Tile bot_left;
    public Tile bot_right;


    public LODTerrain terrain;

	public Vector4 lightmapScaleOffset;

    /** 地形烘焙属性 */
    public LightmapPrototype _lightmapPrototype = new LightmapPrototype();

    /** 水面 */
    public Water water;
    private WaterData waterData;

    private int tick = 0;

    /** 判断地形切片是否预加载 */
    public bool preload = false;

    public GameScene scene;

    public Tile(Region region)
    {
        this.region = region;
        this.scene = region.scene;
    }
    public uint GetKey()
    {
        uint ukey = 0;
        if (region != null)
        {
            ukey = GameScene.GetKey(region.regionX, region.regionY, tileX, tileY);
        }
        return ukey;
    }
    /// <summary>
    /// 设置Tile的邻居关系
    /// </summary>
    public void SetNeighborTile()
    {
        if (left == null)
        {
            left = scene.GetNeighborTile(this, -1, 0);
            if( left != null)
            {
                left.right = this;
            }
        }
        if (right == null)
        {
            right = scene.GetNeighborTile(this, 1, 0);
            if (right != null)
            {
                right.left = this;
            }
        }

        if (top == null)
        {
            top = scene.GetNeighborTile(this, 0, 1);
            if (top != null)
            {
                top.bot = this;
            }
        }

        if (bot == null)
        {
            bot = scene.GetNeighborTile(this, 0, -1);
            if (bot != null)
            {
                bot.top = this;
            }
        }

        if (top_left == null)
        {
            top_left = scene.GetNeighborTile(this, -1, 1);
            if (top_left != null)
            {
                top_left.bot_right = this;
            }
        }

        if (top_right == null)
        {
            top_right = scene.GetNeighborTile(this, 1, 1);
            if (top_right != null)
            {
                top_right.bot_left = this;
            }
        }

        if (bot_left == null)
        {
            bot_left = scene.GetNeighborTile(this, -1, -1);
            if (bot_left != null)
            {
                bot_left.top_right = this;
            }
        }

        if (bot_right == null)
        {
            bot_right = scene.GetNeighborTile(this, 1, -1);
            if (bot_right != null)
            {
                bot_right.top_left = this;
            }
        }
    }
    /// <summary>
    /// 设置当前Terrain的连接地形
    /// </summary>
    public void SetNeighborTerrain()
    {
        if (terrain == null)
            return;

        if (left != null)
        {
            terrain.left = left.terrain;
            if( left.terrain != null)
            {
                left.terrain.right = terrain;
            }
        }
        if (right != null)
        {
            terrain.right = right.terrain;
            if (right.terrain != null)
            {
                right.terrain.left = terrain;
            }
        }
        if (top != null)
        {
            terrain.top = top.terrain;
            if (top.terrain != null)
            {
                top.terrain.bot = terrain;
            }
        }
        if (bot != null)
        {
            terrain.bot = bot.terrain;
            if (bot.terrain != null)
            {
                bot.terrain.top = terrain;
            }
        }

        if (top_left != null)
        {
            terrain.top_left = top_left.terrain;
            if (top_left.terrain != null)
            {
                top_left.terrain.bot_right = terrain;
            }
        }

        if (top_right != null)
        {
            terrain.top_right = top_right.terrain;
            if (top_right.terrain != null)
            {
                top_right.terrain.bot_left = terrain;
            }
        }

        if (bot_left != null)
        {
            terrain.bot_left = bot_left.terrain;
            if (bot_left.terrain != null)
            {
                bot_left.terrain.top_right = terrain;
            }
        }

        if (bot_right != null)
        {
            terrain.bot_right = bot_right.terrain;
            if (bot_right.terrain != null)
            {
                bot_right.terrain.top_left = terrain;
            }
        }

    }
    /*************************************************************************************
     * 功能 ： 获取地形的烘焙属性
     ******************************************************************************************/
    public LightmapPrototype lightmapPrototype
    {
        get
        {
            GameObject obj = GameObject.Find(key);
            if (obj != null)
                terrain = obj.GetComponent<LODTerrain>();

            if (terrain != null)
            {   
                _lightmapPrototype.lightmapIndex = terrain.terrainRenderer.lightmapIndex;
                _lightmapPrototype.lightmapTilingOffset = terrain.terrainRenderer.lightmapScaleOffset;
            }
            return _lightmapPrototype;
        }
    }

    /**************************************************************************************
     * 功能 : 追加单位到此切片中, 由单位包围盒与切片相交计算判断
     ****************************************************************************************/
    public void AddUnit(GameObjectUnit unit)
    {
        if (unitsMap.ContainsKey(unit.createID))
            return;

        unitsMap.Add(unit.createID, unit);
        units.Add(unit);
        unitCount++;
    }

    public void RemoveUnit(GameObjectUnit unit)
    {
        if (unitsMap.ContainsKey(unit.createID))
        {
            unitsMap.Remove(unit.createID);
            units.Remove(unit);
            unitCount--;
        }
    }

    public GameObjectUnit FindUnit(int createID)
    {
        if (unitsMap.ContainsKey(createID))
            return unitsMap[createID];
        else
            return null;
    }

    public bool ContainUnit(GameObjectUnit unit)
    {
        if (unitsMap.ContainsKey(unit.createID))
            return true;
        else
            return false;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 编译器使用
    /// </summary>
    /// <param name="region"></param>
    /// <param name="tileX"></param>
    /// <param name="tileY"></param>
    /// <returns></returns>
    static public Tile Create(Region region, int tileX, int tileY)
    {
        TerrainConfig terrainConfig = region.scene.terrainConfig;

        Tile tile = new Tile(region);
        tile.tileX = tileX;
        tile.tileY = tileY;

        tile.key = region.mstrKey + "_" + tileX + "_" + tileY;

        tile.unitCount = 0;

        tile.position.x = tileX * terrainConfig.tileSize + region.actualX;
        tile.position.y = terrainConfig.defaultTerrainHeight;
        tile.position.z = tileY * terrainConfig.tileSize + region.actualY;

        tile.far = terrainConfig.tileCullingDistance;

        tile.bounds = new Bounds();
        tile.bounds.min = new Vector3(tile.position.x - terrainConfig.tileSize * 0.5f, 0, tile.position.z - terrainConfig.tileSize * 0.5f);
        tile.bounds.max = new Vector3(tile.position.z + terrainConfig.tileSize * 0.5f, 300f, tile.position.z + terrainConfig.tileSize * 0.5f);

        tile.path = "Scenes/" + region.scene.sceneID + "/" + region.mstrKey + "/" + tileX + "_" + tileY;
        tile.SetNeighborTile();
        return tile;
    }

#endif

    /************************************************************************************************
     * 功能 : 更新剔除距离
     * (编译器使用)
     *************************************************************************************************/
    public void UpdateViewRange()
    {
        this.far = region.scene.terrainConfig.tileCullingDistance;

        for (int i = 0; i < units.Count; i++)
        {
            units[i].UpdateViewRange();
        }
    }

    public TileWalkableData mTileWalkableData;

    /***************************************************************************************************
     * 功能 : 读取切片数据
     ***************************************************************************************************/
    public void Read(BinaryReader br, BinaryReader editorbr)
    {
        long pos = 0;
        int i = 0;
        int j = 0;

        TerrainConfig terrainConfig = region.scene.terrainConfig;

        tileX = br.ReadInt32();
        tileY = br.ReadInt32();

        key = DelegateProxy.StringBuilder(region.mstrKey, "_", MathUtils.GetIntString(tileX), "_", MathUtils.GetIntString(tileY));

        far = terrainConfig.tileCullingDistance;

        br.BaseStream.Position += 3 * 4;
        //br.ReadSingle();
        //br.ReadSingle();
        //br.ReadSingle();

        bool hasHeightData = br.ReadBoolean();

        // 读取高度分辨率
        pos = br.BaseStream.Position;
        int heightmapResolution = terrainConfig.heightmapResolution;
	    if (br.ReadInt32() == 10003)
        {
            heightmapResolution = terrainConfig.heightmapResolution;
            position.x = tileX * terrainConfig.tileSize + region.actualX;
            position.y = terrainConfig.defaultTerrainHeight;
            position.z = tileY * terrainConfig.tileSize + region.actualY;

            ReadEditorData(hasHeightData, editorbr, heightmapResolution);

        }
        else
        {
            br.BaseStream.Position = pos;

            if (br.ReadInt32() == 10001)
                heightmapResolution = 32;       // 旧数据兼容

            pos = br.BaseStream.Position; 
            if (br.ReadInt32() == 10002)
                heightmapResolution = terrainConfig.heightmapResolution;
            else
                br.BaseStream.Position = pos;

            position.x = tileX * terrainConfig.tileSize + region.actualX;
            position.y = terrainConfig.defaultTerrainHeight;
            position.z = tileY * terrainConfig.tileSize + region.actualY;

            // 载入采样后的地面高度
            if (hasHeightData == true)	
            {
                if (GameScene.isPlaying == true)
                {
                    //br.BaseStream.Position += (heightmapResolution * heightmapResolution * 4);
                    for (i = 0; i < heightmapResolution; i++)
                    {
                        for (j = 0; j < heightmapResolution; j++)
                        {
                            br.ReadSingle();
                        }
                    }
                }
                else
                {
                    heights = new short[heightmapResolution, heightmapResolution];
                    for (i = 0; i < heightmapResolution; i++)
                    {
                        for (j = 0; j < heightmapResolution; j++)
                        {
                            heights[j, i] = ByteUtils.floatConverShort(br.ReadSingle());
                        }
                    }
                }
            }   
            // 读取阻塞数据
            pos = br.BaseStream.Position;

            if (br.ReadInt32() == 10002)
            {
                if (GameScene.isPlaying == true)
                {
                    br.BaseStream.Position += (heightmapResolution * heightmapResolution * 4);
                    //for (i = 0; i < heightmapResolution; i++)
                    //{
                    //    for (j = 0; j < heightmapResolution; j++)
                    //    {
                    //        br.ReadSingle();
                    //    }
                    //}
                }
                else
                {
                    grids = new byte[heightmapResolution, heightmapResolution];
                    for (i = 0; i < heightmapResolution; i++)
                    {
                        for (j = 0; j < heightmapResolution; j++)
                        {
                            grids[j, i] = ByteUtils.intConverByte(br.ReadInt32());
                        }
                    }
                }
            }
            else
            {
                br.BaseStream.Position = pos;
            }
        }

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 55557)
        {
            if (mTileWalkableData == null)
            {
                mTileWalkableData = new TileWalkableData(key, position, terrainConfig);
            }
            mTileWalkableData.ReadFloorInfo(br);
            mTileWalkableData.ReadWalkMarker(br);
            mTileWalkableData.ReadFloorHeight(br);
            mTileWalkableData.ReadSpaceHeight(br);
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        int _insCount = br.ReadInt32();
        int readCount = 0;
        if (_insCount > 0)
        {
            while (br.ReadString() == "GameObject")
            {	
                int createID = br.ReadInt32();
			
                GameObjectUnit unit = null;

                // 从正在读取的地域中查找, 正在读取的地域不会被载入场景
                // 存在隐患,如果scene文件被删除将引起createID丢失引起重复ID,导致读取失误
                if (scene.curSceneUnitsMap.ContainsKey(createID))
                {
                    unit = scene.curSceneUnitsMap[createID];
                }

                if (unit == null)
                {
                    // 如果场景已经创建则直接引用，否则重新创建
                    unit = scene.CreateEmptyUnit(createID);
                }
                unit.scene = region.scene;
                unit.Read(br, createID);

                if (GameScene.SampleMode == true)
                {
                    // 灯光和合并单位游戏运行时不添加到容器
                    AddUnit(unit);

                    // 当前单元存储切片数据
                    unit.tiles.Add(this);

                    if (scene.curSceneUnitsMap.ContainsKey(createID) == false)
                    {
                        scene.curSceneUnitsMap.Add(createID, unit);
                    }
                        
                }
                else
                {
                    // 灯光和合并单位游戏运行时不添加到容器
                    if ((unit.combineParentUnitID < 0 && unit.type != UnitType.UnitType_Light) || GameScene.isPlaying == false)
                    {
                        AddUnit(unit);
                        unit.tiles.Add(this);
                        if (!scene.curSceneUnitsMap.ContainsKey(createID))
                        {
                            scene.curSceneUnitsMap.Add(createID, unit);
                        }
                    }
                    else
                    {
                        // 干掉单位内存
                        if (unit != null)
                        {
                            unit.Destroy();
                            scene.RemoveEmptyUnit(unit);
                        }
                    }
                }

                readCount++;

                if (readCount >= _insCount)
                    break;
            }
        }

		if (readCount < _insCount)
			Debug.Log ("read error!");

        // 判断是否存储了水面
        // 从存储文件中读取路径
        if (br.BaseStream.Position < br.BaseStream.Length)
        {
            pos = br.BaseStream.Position;
            if (br.ReadInt32() == 10011)
            {
                if (br.ReadInt32() == 1)
                {
                    // 读取水面数据
                    waterData = new WaterData();
                    waterData.Read(br);
                }
            }
            else
            {
                br.BaseStream.Position = pos;
            }
        }

        bool hasLightmapData = br.ReadBoolean();
        if (hasLightmapData == true)
        {
            _lightmapPrototype.lightmapIndex = br.ReadInt32();


            pos = br.BaseStream.Position;
            if (br.ReadInt32() == 10007)
                _lightmapPrototype.scale = br.ReadSingle();
            else
                br.BaseStream.Position = pos;

            _lightmapPrototype.lightmapTilingOffset.x = br.ReadSingle();
            _lightmapPrototype.lightmapTilingOffset.y = br.ReadSingle();
            _lightmapPrototype.lightmapTilingOffset.z = br.ReadSingle();
            _lightmapPrototype.lightmapTilingOffset.w = br.ReadSingle();

        }

        if (GameScene.isPlaying)
        {
            path = DelegateProxy.StringBuilder("Scenes/" , region.scene.sceneID , "/" , region.mstrKey , "/" , tileX , "_" , tileY);
        }
        else
        {
#if UNITY_EDITOR
            if (!GameScene.isPlaying)
            {
                // 计算地形的包围体
                bounds = new Bounds();
                bounds.min = new Vector3(position.x - terrainConfig.tileSize * 0.5f, 0, position.z - terrainConfig.tileSize * 0.5f);
                bounds.max = new Vector3(position.x + terrainConfig.tileSize * 0.5f, 300f, position.z + terrainConfig.tileSize * 0.5f);
            }
#endif
            path = "Scenes/" + region.scene.sceneID + "/" + region.mstrKey + "/" + tileX + "_" + tileY;
        }
    }

    private void ReadEditorData(bool hasHeightData, BinaryReader br, int heightmapResolution)
    {
        if (br == null)
        {
            return;
        }
        if (br.ReadInt32() == 10003)
        {
            // 载入采样后的地面高度
            if (hasHeightData == true)
            {
                if (GameScene.isPlaying == true)
                {
                    for (int i = 0; i < heightmapResolution; i++)
                    {
                        for (int j = 0; j < heightmapResolution; j++)
                        {
                            br.ReadInt16();
                        }
                    }
                }
                else
                {
                    heights = new short[heightmapResolution, heightmapResolution];
                    for (int i = 0; i < heightmapResolution; i++)
                    {
                        for (int j = 0; j < heightmapResolution; j++)
                        {
                            heights[j, i] = br.ReadInt16();
                        }
                    }
                }
            }

            if (GameScene.isPlaying == true)
            {
                for (int i = 0; i < heightmapResolution; i++)
                {
                    for (int j = 0; j < heightmapResolution; j++)
                    {
                        br.ReadByte();
                    }
                }
            }
            else
            {
                grids = new byte[heightmapResolution, heightmapResolution];
                for (int i = 0; i < heightmapResolution; i++)
                {
                    for (int j = 0; j < heightmapResolution; j++)
                    {
                        grids[j, i] = br.ReadByte();
                    }
                }
            }
        }

    }


    public void CheckNeighborTerrain()
    {
        if (terrain == null)
            return;

        if (terrain.left == null && left != null)
            terrain.left = left.terrain;
        if (terrain.right == null && right != null)
            terrain.right = right.terrain;
        if (terrain.top == null && top != null)
            terrain.top = top.terrain;
        if (terrain.bot == null && bot != null) 
            terrain.bot = bot.terrain;
        if (terrain.top_left == null && top_left != null)
            terrain.top_left = top_left.terrain;
        if (terrain.top_right == null && top_right != null)
            terrain.top_right = top_right.terrain;
        if (terrain.bot_left == null && bot_left != null)
            terrain.bot_left = bot_left.terrain;
        if (terrain.bot_right == null && bot_right != null)
            terrain.bot_right = bot_right.terrain;
    }
    

    public void UpdateTerrainLOD()
    {
        if (terrain == null)
            return;

        TerrainLODConfig lodCfg = scene.terrainConfig.terrainLODConfig;

        int _lodLevel = 0;
        for (int i = 0; i < lodCfg.lodDistanceRanges.Length; i++)
        {
            if (viewDistance > (lodCfg.lodDistanceRanges[i] * lodCfg.lodDistanceRanges[i]))
                _lodLevel++;
            else
            {
                break;
            }
        }
        terrain.lodLevel = _lodLevel;
    }

    /**********************************************************************************************
     * 功能 : 更新切片
     **********************************************************************************************/

    public void Update(Vector3 eyePos)
    {
        if (GameScene.isEditor == true && GameScene.isPlaying == false)
        {
            if (water != null)
                water.ForcedUpdate();

            GameObjectUnit gUnit = null;
            for (int unitInd = 0; unitInd < unitCount; unitInd++)
            {
                gUnit = units[unitInd];
                if (gUnit.active == false)
                {
                    scene.AddUnit(gUnit);
                    gUnit.Visible();
                }
            }
        }
        else
        {
            //资源加载完成时，需要分帧更新
            //加载过程，需要提高加载速度
            if (scene.loadSceneComplate)
            {
                UpdateViewDistance(eyePos);
            }
            else
            {
                CountLoadUnitAmount(eyePos);
            }
        }
        tick++;
    }


    /// <summary>
    /// 加载过程中，统计要加载的资源数量
    /// </summary>
    /// <param name="eyePos"></param>
    private void CountLoadUnitAmount(Vector3 eyePos)
    {
        GameObjectUnit gUnit = null;
        for (int unitInd = 0; unitInd < unitCount; unitInd++)
        {
            gUnit = units[unitInd];
            if (gUnit.active == false && (gUnit.combineParentUnitID < 0 || GameScene.SampleMode))
            {
                float dx = gUnit.center.x - eyePos.x;
                float dz = gUnit.center.z - eyePos.z;
                gUnit.viewDistance = dx * dx + dz * dz;
                if (gUnit.viewDistance < gUnit.near * gUnit.near)
                    scene.AddUnit(gUnit);            // 插入到场景列表中
            }
        }
    }

    private int lastInd = 0;

    /// <summary>
    /// 资源加载完成－帧循环更新贴片中物件距离
    /// </summary>
    private void UpdateViewDistance(Vector3 eyePos)
    {
        if (unitCount == 0)
            return;

        // 计算单位与切片距离
        if (tick % 2 == 0)
        {
            //优化代码，每次执行5次判断
            GameObjectUnit gUnit = null;
            for (int i = 0; i < 5; i++)
            {
                gUnit = units[lastInd++ % unitCount];
                if (gUnit.active == false && (gUnit.combineParentUnitID < 0 || GameScene.SampleMode))
                {
                    float dx = gUnit.center.x - eyePos.x;
                    float dz = gUnit.center.z - eyePos.z;
                    gUnit.viewDistance = dx * dx + dz * dz;
                    if (gUnit.viewDistance < gUnit.near * gUnit.near)
                        scene.AddUnit(gUnit);            // 插入到场景列表中
                }
            }
        }
    }

    /*****************************************************************************************************
     * 功能 ： 显示地形切片
     ******************************************************************************************************/
    public void Visible()
    {
        if (visible == false)
        {
            // 创建水面 ----------------------------------------------------------------------------------------------
            if (waterData != null)
            {
                if (water == null)
                {
                    // 载入水面
                    //water = Water.CreateWaterGameObject(waterData);
                    //water.gameObject.name = "water" + key;
                    //water.transform.position = new Vector3(this.position.x, water.waterData.height, this.position.z);
                }
            }

            // 创建地形单位 ----------------------------------------------------------------------------------------------
            TerrainConfig terrainConfig = region.scene.terrainConfig;
            if (terrainConfig.enableTerrain == true)
            {
                if (terrain == null)
                {
                    // 获取地形资源
                    Asset asset = null;
                    if (region.scene.loadFromAssetBund == true)
                        asset = AssetLibrary.Load(path + "_terrainData", AssetType.Terrain, LoadType.Type_AssetBundle);
                    else
                        asset = AssetLibrary.Load(path + "_terrainData", AssetType.Terrain, LoadType.Type_Resources);

                    if (asset != null)
                    {
                        if (asset.loaded == true)
                            OnTerrainLoadCompate(asset);
                        else
                            asset.loadedListener = OnTerrainLoadCompate;

                        // 编辑器中创建地形
                        if (GameScene.isPlaying == false)
                            if (asset.loaded == false)
                                OnTerrainLoadCompate(asset);
                    }
                }
                else
                {
                    terrain.gameObject.SetActive(true);
                }

                if (water != null)
                    water.gameObject.SetActive(true);
            }
        }

        visible = true;
    }

    /** 加载地形数据 */
    private void OnTerrainLoadCompate(Asset asset)
    {
        if (asset.loaded == true)
        {
            terrain = asset.terrain;
            terrain.name = key;
            // 定位地形位置
            terrain.transform.position = new Vector3(position.x, 0, position.z);
            terrain.gameObject.layer = GameLayer.Layer_Ground;
            SetNeighborTerrain();
            // 引用烘焙贴图
            if (_lightmapPrototype.lightmapIndex >= 0)
            {
                Renderer terrainRenderer = terrain.GetComponent<Renderer>();
                if (terrainRenderer != null)
                {
                    terrainRenderer.lightmapIndex = _lightmapPrototype.lightmapIndex;
                    terrainRenderer.lightmapScaleOffset = _lightmapPrototype.lightmapTilingOffset;
                }
            }
            terrain.splatsMapPath = "Scenes/" + region.scene.sceneID + "/" + region.mstrKey + "/" + tileX + "_" + tileY + "Splats";
            //// 创建地形材质
            //if (GameScene.isPlaying == false)
            //    terrain.BuildMaterial();
        }
        else
        {
#if UNITY_EDITOR
            // 创建新的地形
            LODTerrainData td = new LODTerrainData();
            terrain = LODTerrain.CreateTerrainGameObject(td);
            terrain.name = key;
            // 定位地形位置
            terrain.transform.position = new Vector3(position.x, 0, position.z);
            terrain.gameObject.layer = GameLayer.Layer_Ground;
            terrain.Init();
            SetNeighborTerrain();
#endif
        }


        // 项目运行时不开启地形碰撞
        if (GameScene.isPlaying == false)
        {
            if (terrain != null)
                terrain.gameObject.AddComponent<MeshCollider>();
        }
    }

    /**************************************************************************************************
     * 功能 : 隐藏地形切片
     *****************************************************************************************************/
    public void Invisible()
    {
        if (visible == true)
        {
            if (terrain != null)
            {
                terrain.CancelBake();
                terrain.gameObject.SetActive(false);
            }
            if (water != null)
            {
                water.gameObject.SetActive(false);
            }
            visible = false;
        }
    }

    /**************************************************************************************************
     * 功能 : 消耗地形切片
     *****************************************************************************************************/
    public void Destroy()
    {
        if (GameScene.isPlaying == true)
        {
			if (waterData != null)
				waterData.Release();
            // 销毁地形及水面
            if (terrain != null)
            {
                terrain.Destroy();
                GameObject.Destroy(terrain);
                GameObject.Destroy(terrain.gameObject);
                this.terrain = null;
            }
            if (water != null)
            {
                water.Destroy();
                GameObject.Destroy(water);
                GameObject.Destroy(water.gameObject);
                this.water = null;
            }
        }
        else
        {
            if (terrain != null)
                GameObject.DestroyImmediate(terrain.gameObject);
            if (water != null)
                GameObject.DestroyImmediate(water.gameObject);
        }

        // 销毁地形中的静态单位
        if (units != null)
        {
            while (units.Count > 0)
            {
                GameObjectUnit unit = units[0];
                if (scene.ContainUnit(unit))
                    scene.RemoveUnit(unit, true, false);
                else
                {
                    unit.Destroy();
                    scene.RemoveEmptyUnit(unit, true);
                }
            }
            units.Clear();
            units = null;
        }

        unitsMap.Clear();
        unitsMap = null;


        this.left = null;
        this.right = null;
        this.top = null;
        this.bot = null;
        this.top_left = null;
        this.top_right = null;
        this.bot_left = null;
        this.bot_right = null;

        this.region = null;
        this.scene = null;

        this._lightmapPrototype = null;
        waterData = null;
        this.heights = null;
        this.grids = null;

        mTileWalkableData = null;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 删除水面
    /// 编译器方法
    /// </summary>
    public void RemoveWater()
    {
        if (water != null)
        {
            if (GameScene.isPlaying == true)
                GameObject.Destroy(water.gameObject);
            else
                GameObject.DestroyImmediate(water.gameObject);
            water = null;
            waterData = null;
            if (terrain != null)
            {
                terrain.hasWater = false;
                terrain.BuildMaterial();
            }
        }
    }


    /*************************************************************************************************
     * 功能 ： 采样切片高度，取缔碰撞体计算行走高度
     * 注解 : 采样高度只能选取最高度为最终高度, 避免角色与物件穿插
     *        阻塞状态计算 :
     *        1、根据高度差计算初始格子阻塞状态
     *        2、根据碰撞对象层级判断
     * @resolution 检测的分辨率
     * @mask 检测层级
     * @pHeights 输出高度
     * @pGrids 输出格子数据
     * 编译器使用
     **************************************************************************************************/
    public void ComputeHeights(int resolution, int mask, int occlusionMask, GameObjectUnit selectObject = null)
    {
        TerrainConfig terrainConfig = region.scene.terrainConfig;

        int groundMask = GameLayer.Mask_Ground;

        Ray ray = new Ray();
        Vector3 origin0 = new Vector3(0f, 500, 0f);
        Vector3 origin1 = new Vector3(1f, 500, 0f);
        Vector3 origin2 = new Vector3(0f, 500, 1f);
        Vector3 origin3 = new Vector3(1f, 500, 1f);

        Vector3 origin4 = new Vector3(0.5f, 500, 0.5f);

        Vector3 origin = new Vector3(-0.5f, 500, -0.5f);

        Vector3[] origins = new Vector3[5];
        origins[0] = origin0;
        origins[1] = origin1;
        origins[2] = origin2;
        origins[3] = origin3;
        origins[4] = origin4;

        ray.direction = Vector3.down;
        RaycastHit hit;

        int w = resolution;
        int h = resolution;
        float m = (float)terrainConfig.tileSize / (float)resolution;
        heights = new short[w, h];
        grids = new byte[w, h];

        bool insertToUnit = false;

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                heights[i, j] = 0;
                float min = 1000f;
                float max = -1000f;

                int block = 0;

                insertToUnit = false;

                // 检测阻塞状态(通过高度差 或者 阻塞物体)
                for (int k = 0; k < origins.Length; k++)
                {
                    origin.x = i * m + position.x + origins[k].x - terrainConfig.tileSize * 0.5f;
                    origin.z = j * m + position.z + origins[k].z - terrainConfig.tileSize * 0.5f;

                    ray.origin = origin;

                    // 
                    Physics.Raycast(ray, out hit, 2000, mask);
                    if (hit.transform != null)
                    {
                        if (block == 0)
                        {
                            if (((1 << hit.transform.gameObject.layer) & occlusionMask) >= 1)
                                block = 1;
                        }
                        if (min > hit.point.y)
                        {
                            min = hit.point.y;
                        }
                        if (max < hit.point.y)
                            max = hit.point.y;

                        if (selectObject != null && selectObject.Ins != null)
                        {
                            if (hit.transform.gameObject.name == selectObject.Ins.name)
                            {
                                insertToUnit = true;
                            }
                        }
                    }
                    else
                    {
                        min = -999999f;
                    }
                }

                grids[i, j] = ByteUtils.intConverByte(block);

                // 通过高度差计算阻塞
                if (block == 0)
                {
                    if ((max - min) > terrainConfig.blockHeight)
                        grids[i, j] = 1;
                    else
                        grids[i, j] = 0;
                }

                // 检测高度 ----------------------------------------------------------------------------------
                heights[i, j] = 0;
                min = 1000;
                max = -1000;

                // 检测阻塞状态(通过高度差 或者 阻塞物体)
                for (int k = 0; k < origins.Length; k++)
                {
                    origin.x = i * m + position.x + origins[k].x - terrainConfig.tileSize * 0.5f;
                    origin.z = j * m + position.z + origins[k].z - terrainConfig.tileSize * 0.5f;

                    ray.origin = origin;

                    // 
                    Physics.Raycast(ray, out hit, 2000, groundMask);
                    if (hit.transform != null)
                    {
                        if (min > hit.point.y)
                        {
                            min = hit.point.y;
                        }
                        if (max < hit.point.y)
                            max = hit.point.y;
                    }
                    else
                    {
                        //  min = max = terrainConfig.defaultTerrainHeight;
                    }
                }

                // 如果超出可到达高度,将被阻塞
                if (max > terrainConfig.maxReachTerrainHeight)
                    grids[i, j] = 1;

                // 采用最大的高度值为当前高度
                heights[i, j] = ByteUtils.floatConverShort(max);


                // 计算指定对象所占的格子
                if (insertToUnit == true && grids[i, j] == 1)
                {
                    float gwx = i * m + position.x + origins[4].x - terrainConfig.tileSize * 0.5f;
                    float gwy = j * m + position.z + origins[4].z - terrainConfig.tileSize * 0.5f;

                    int gridX = Mathf.FloorToInt(gwx / (terrainConfig.gridSize));
                    int gridY = Mathf.FloorToInt(gwy / (terrainConfig.gridSize));

                    selectObject.AppendGrid(gridX, gridY);

                    // Debug.Log("grids-> x " + gridX + " - y " + gridY);
                }
            }
        }
    }



    /***********************************************************************************************
     * 功能 ：插值采样高度
     * 编译器方法
     ************************************************************************************************/
    public float SampleHeightInterpolation(Vector3 worldPosition)
    {
        TerrainConfig terrainConfig = region.scene.terrainConfig;

        if (heights == null)
            return terrainConfig.defaultTerrainHeight;

        float x = worldPosition.x - position.x + terrainConfig.tileSize * 0.5f;
        float y = worldPosition.z - position.z + terrainConfig.tileSize * 0.5f;
        float m = ((float)terrainConfig.tileSize / (float)terrainConfig.heightmapResolution);
        float pX = (x % m) / m;
        float pY = (y % m) / m;
        int intX1 = Mathf.FloorToInt(x / m);
        int intY1 = Mathf.FloorToInt(y / m);

        intX1 = Math.Max(0, intX1);
        intY1 = Math.Max(0, intY1);

        int intX2 = intX1;
        int intY2 = intY1;

        if (intX2 < terrainConfig.heightmapResolution - 1)
            intX2 += 1;
        if (intY2 < terrainConfig.heightmapResolution - 1)
            intY2 += 1;

        float r1 = ByteUtils.shortConverFloat(heights[intX1, intY1]) * (1 - pX) + ByteUtils.shortConverFloat(heights[intX2, intY1]) * pX;
        float r2 = ByteUtils.shortConverFloat(heights[intX1, intY2]) * (1 - pX) + ByteUtils.shortConverFloat(heights[intX2, intY2]) * pX;
        float r3 = r2 * pY + r1 * (1 - pY);
        return r3;

    }

    /********************************************************************************************
     * 无用功能
     * 功能 : 普通采样高度
     ********************************************************************************************/
    public float SampleHeight(Vector3 worldPosition, float curHeight = 0f)
    {
        TerrainConfig terrainConfig = region.scene.terrainConfig;
        if (heights == null)
            return terrainConfig.defaultTerrainHeight;

        float x = worldPosition.x - position.x + terrainConfig.tileSize * 0.5f;
        float y = worldPosition.z - position.z + terrainConfig.tileSize * 0.5f;
        float m = (float)terrainConfig.tileSize / (float)terrainConfig.heightmapResolution;

        int intX1 = (int)Mathf.Floor(x / m);
        int intY1 = (int)Mathf.Floor(y / m);

        int intX2 = intX1 + 1;
        int intY2 = intY1 + 1;

        float r1 = heights[intX1, intY1] * 0.5f + heights[intX2, intY1] * 0.5f;
        float r2 = heights[intX1, intY2] * 0.5f + heights[intX2, intY2] * 0.5f;
        float r3 = r1 * 0.5f + r2 * 0.5f; ;
        return ByteUtils.shortConverFloat(heights[intX1, intY1]);
    }

    /*********************************************************************************************
     * 功能 : 阻塞数据调试
     **********************************************************************************************/
    private GameObject gridDataGO;
    private Mesh gridDataMesh = new Mesh();
    private Shader gridDataShader;
    private Material gridDataMat;
    private float padding = 0.15f;
    /// <summary>
    /// 编译器方法
    /// </summary>
    public void UnDrawGridData()
    {
        if (gridDataGO != null)
            GameObject.DestroyImmediate(gridDataGO);
    }

    /// <summary>
    /// 编译器使用
    /// </summary>
    public void DrawGridData()
    {
        TerrainConfig terrainConfig = region.scene.terrainConfig;
        if (gridDataGO == null)
        {
            gridDataGO = new GameObject();
            gridDataGO.name = "GridData_" + key;
            gridDataGO.AddComponent<MeshFilter>();
            gridDataGO.AddComponent<MeshRenderer>();
            gridDataGO.transform.position = new Vector3(position.x, 0f, position.z);

            gridDataShader = Shader.Find("Snail/Grid");
            gridDataMat = new Material(gridDataShader);
            string girdTex = "Textures/Shader/Grid";
            gridDataMat.mainTexture = Resources.Load(girdTex, typeof(UnityEngine.Object)) as Texture2D;

            gridDataGO.GetComponent<Renderer>().material = gridDataMat;
        }

        int w = terrainConfig.gridResolution;
        int h = terrainConfig.gridResolution;
        int count = w * h;

        int vertCount = count * 4;
        int triCount = count * 2;
        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        Color[] colors = new Color[vertCount];

        int[] triangles = new int[triCount * 3];
        int i = 0;
        int j = 0;
        int ind = 0;
        //int gridCountPerSide = terrainConfig.regionSize;
        float gridSize = terrainConfig.tileSize / (float)terrainConfig.gridResolution;

        // 构建顶点 -----------------------------------------------
        //int len = count;

        Color redColor = new Color(1f, 0f, 0f, 1f);
        Color blueColor = new Color(0f, 1f, 0f, 1f);
        Color color = new Color();

        bool block = false;
        for (i = 0; i < w; i++)
        {
            for (j = 0; j < h; j++)
            {
                // 0 --- 1
                // |     |
                // 2 --- 3
                float gx = i;
                float gy = j;
                float vx = gx * gridSize - terrainConfig.tileSize * 0.5f;
                // float vy = 0;
                float vz = gy * gridSize - terrainConfig.tileSize * 0.5f;
                float vy = region.scene.SampleHeight(new Vector3(vx + position.x, 100f, vz + position.z)) + 0.1f;

                bool canWalk = GameScene.mainScene.IsValidForWalkByEditor(new Vector3(vx + position.x, 100f, vz + position.z), 1);

                if (canWalk == false)
                {
                    color = redColor;
                    block = true;
                }
                else
                {
                    color = blueColor;
                    block = false;
                }



                // vy = heights[i, j];

                Vector3 pos1 = new Vector3(vx + position.x, vy, vz + position.z);
                pos1.x = vx + padding;
                pos1.y = vy;//terrain.SampleHeight(pos1) + 0.2f;
                pos1.z = vz + padding;
                vertices[ind] = pos1;
                colors[ind] = color;
                Vector3 uv1 = new Vector2(0, 0);
                uvs[ind] = uv1;
                ind++;

                Vector3 pos2 = new Vector3(vx + position.x + gridSize, vy, vz + position.z);
                pos2.x = vx + gridSize - padding;
                pos2.y = vy; //terrain.SampleHeight(pos2) + 0.2f;
                pos2.z = vz + padding;
                vertices[ind] = pos2;
                Vector3 uv2 = new Vector2(1, 0);
                uvs[ind] = uv2;
                colors[ind] = color;
                ind++;

                Vector3 pos3 = new Vector3(vx + position.x, vy, vz + position.z + gridSize);
                pos3.x = vx + padding;
                pos3.y = vy; //terrain.SampleHeight(pos3) + 0.2f;
                pos3.z = vz + gridSize - padding;
                vertices[ind] = pos3;
                Vector3 uv3 = new Vector2(0, 1);
                uvs[ind] = uv3;
                colors[ind] = color;
                ind++;

                Vector3 pos4 = new Vector3(vx + position.x + gridSize, vy, vz + position.z + gridSize);
                pos4.x = vx + gridSize - padding;
                pos4.y = vy;///terrain.SampleHeight(pos4) + 0.5f;
                pos4.z = vz + gridSize - padding;
                vertices[ind] = pos4;
                Vector3 uv4 = new Vector2(1, 1);
                uvs[ind] = uv4;
                colors[ind] = color;
                ind++;
            }
        }

        // 构建三角形 ------------------------------------------------------
        ind = 0;
        for (i = 0; i < count; i++)
        {
            triangles[ind * 6] = i * 4;
            triangles[ind * 6 + 1] = i * 4 + 2;
            triangles[ind * 6 + 2] = i * 4 + 3;

            triangles[ind * 6 + 3] = i * 4;
            triangles[ind * 6 + 4] = i * 4 + 3;
            triangles[ind * 6 + 5] = i * 4 + 1;
            ind++; ;
        }

        gridDataMesh.vertices = vertices;
        gridDataMesh.uv = uvs;
        gridDataMesh.colors = colors;
        gridDataMesh.triangles = triangles;
        gridDataMesh.RecalculateBounds();
        gridDataMesh.bounds = new Bounds(position, new Vector3(1000f, 1000f, 1000f));
        gridDataGO.GetComponent<MeshFilter>().mesh = gridDataMesh;
    }
#endif
}