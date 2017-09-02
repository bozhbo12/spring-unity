using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
/********************************************************************************************************
 * 版本号 : 2.0.0
 * 
 * 类 ： 游戏场景, 动态加载地形区域
 * 注 ： 动态单位的销毁和创建由服务端视野决定，静态单位的创建由切片创建，静态单位
 * 的销毁由自身的可视距离处理
 * 客户端需要视野原因
 * 1、服务端视野超出屏幕范围，客户端不需要
 * 2、服务端延时导致对象未删除,增加客户端渲染压力
 * 
 * 服务端刷新视野优化需求
 * 1、客户端主角从A->B->A位置移动时,延时删除消息发送,避免短时内对象从B返回无效删除
 *********************************************************************************************************/

public class GameScene
{

#if UNITY_EDITOR
    public List<int> shareIds = new List<int>();
    /** 地形数据 */
    public TerrainData terrainData;

    /******************************************************************************************************
  * 功能 ：判断当前坐标（世界坐标）到目标坐标（世界坐标）是否有阻挡
  *canMoveHeight:可移动高度；每帧之间可移动最大高度差值
  *collisionSize:碰撞尺寸
  *modeHeight:模型高度
  * 返回 : [true 可行走], [false 不可行走]
  * 注解 : 默认0,0地域0,0切片（世界坐标(0,0)为）
  *******************************************************************************************************/
    private int srcX = 0;
    private int srcZ = 0;
    private int dstX = 0;
    private int dstZ = 0;

    public DetailRenderer detailRenderer;

    public CullingData cullData;

    public void GetCullingData()
    {
        string path = "Assets/Res/Scenes/" + GameScene.mainScene.sceneID + "/CullingData.asset";
        cullData = AssetDatabase.LoadAssetAtPath<CullingData>(path);
    }
#endif

    /** 场景ID */
    public int sceneID = 0;

    /** 场景加载完毕监听 */
    public delegate void SceneLoadCompleBack();
    public SceneLoadCompleBack sceneLoadCompleListener;

    /// <summary>
    /// 资源读取完成
    /// </summary>
    public static System.Action<GameScene> sceneByteReadComplete;

    public float loadProgress = 0f;

    public bool loadSceneComplate = false;
    /// <summary>
    /// 当前加载数量
    /// </summary>
    private int sceneLoadUnitTick = 0;
    /// <summary>
    /// 加载总数量
    /// </summary>
    private int sumSceneLoadUnitTick = 0;

    /** 判断是否播放器状态 */
    static public bool isPlaying = false;

    /** 游戏场景的随机码 */
    public float randomCode = 0f;

    /** 当前场景的帧频 */
    private float updateInterval = 0.2f;
    private float lastInterval = Time.realtimeSinceStartup;
    private int frames = 0;
    public float fps = 30f;
    //private float oldfps = 1f;
    public float ms = 1f;
    public float time = 0f;
    private int tick = 0;
    private int viewRect = 1;

//#if UNITY_EDITOR
//    /** 场景目标帧频 */
//    public int targetFrameRate = 30;
//#else
//    /** 场景目标帧频 */
//    public int targetFrameRate = 25;
//#endif

    public static int targetFrameRate
    {
        get
        {
            if (Config.bWin && Config.isSCLimitEffect) // 编辑器模式下   或者  市场pc包.bat启动模式下
                return 30;
            else
                return 25;
        }
    }


    private Dictionary<int, Region> regionsMap = null;
    public List<Region> regions;
	public List<LODTerrain> mBakeTerrains = new List<LODTerrain>(9);
    // 场景更新参照的坐标
    public Vector3 eyePos = Vector3.zero;

    public int curRegionX = 0;
    public int curRegionY = 0;

    /** 创建的单元键值对,key 实例ID， value 所创建的单元 */
    public Dictionary<int, GameObjectUnit> unitsMap;
    public List<GameObjectUnit> units;

    public int unitCount = 0;
    public int unitIdCount = 0;

    /** 地形配置 */
    private TerrainConfig _terrainConfig;

    /** 切片 */
    private Dictionary<uint, Tile> tilesMap;
    public List<Tile> tiles;

    static public bool dontCullUnit = true;

    /** 判断是否在编辑器中 */
    static public bool isEditor = false;

    /** 行走路径 */
    private MapPath _mapPath;

    public TerrainWalkableData terrainWalkableData;

    /** 场景中的摄像机 */
    private List<Camera> cameras = new List<Camera>();

    /** 主单位 */
    public GameObjectUnit mainUnit;

    public Camera mainCamera;
    static private int mainCameraCullingMask = -1;

    /** 场景高度采样数据 */
    public short[,] heights;

    /** 主场景 */
    static public GameScene mainScene = null;

    static public bool IsNewDynamicUnit = false;

    static public GameObject oRoot;

    /// <summary>
    /// 最大跳跃高度差
    /// </summary>
    static public int fJumpMaxHeightDiff = 100;

    /// <summary>
    /// 环境贴图
    /// </summary>
    private ShadowMapData mSMData = null;

    /** 采样模式 */
    static public bool SampleMode = false;

    public bool loadFromAssetBund = false;


    /** 统计模式 */
    public bool statisticMode = true;

    /** 是否启用实例缓存模式 */
    public bool cacheMode = false;

#if UNITY_EDITOR
    /// <summary>
    /// 外网
    /// </summary>
    private SnailFogPro mSnailFogPro;
#endif

    /// <summary>
    /// bump变化回调
    /// </summary>
    public UserDelegate mBumpChangeDgate = new UserDelegate();

    /** 静态单位缓存列表 */
    public List<GameObjectUnit> staticUnitcCache = new List<GameObjectUnit>(16);
    public List<DynamicUnit> dynamicUnitsCache = new List<DynamicUnit>(8);

    public Dictionary<int, GameObjectUnit> curSceneUnitsMap = new Dictionary<int, GameObjectUnit>();

    /** 解析队列 */
    public List<ParserBase> parsers = new List<ParserBase>();

    public List<string> postEffectsList = new List<string>();

    private Dictionary<string, byte[]> peConfig = new Dictionary<string, byte[]>();


    /// <summary>
    /// 当前场景lightmap管理
    /// </summary>
    private SceneLightMap mSceneLightMap = null;

    /** 是否开启光照贴图校正 */
    static public bool lightmapCorrectOn = false;


    /// <summary>
    /// 目的为了区分lightmap加载路径
    /// </summary>
    public float unityVersion = 5.3f;

    /// <summary>
    /// 当前中心地块
    /// </summary>
    private Tile mtCurCenterTile = null;

    public GameScene(bool createNew = false)
    {
        randomCode = UnityEngine.Random.value;
        regions = new List<Region>();
        regionsMap = new Dictionary<int, Region>();

        tilesMap = new Dictionary<uint, Tile>();
        tiles = new List<Tile>();

        unitsMap = new Dictionary<int, GameObjectUnit>();
        units = new List<GameObjectUnit>();

        mainScene = this;
        isPlaying = Application.isPlaying;
        isEditor = Application.isEditor;
        _terrainConfig = new TerrainConfig();

        // 编辑中需要实时调节效果,不参与烘焙
        //if (isEditor == false)
        //Terrainmapping.mapsCount = 9;

        if (createNew == true)
        {
            readCompalte = true;
        }

        // 设置默认后期效果
        //大气散射
        //postEffectsList.Add("AtmosphericScattering");
        //postEffectsList.Add("Fog");

        //容积光
        postEffectsList.Add("SunShafts");

        //景射
        //postEffectsList.Add("AdvanceDepthOfField");

        //泛光
        postEffectsList.Add("Bloom");
        //高级泛光
        //postEffectsList.Add("AdvanceBloom");

        //postEffectsList.Add("FilmicTonemapping");

        //色彩校正
        postEffectsList.Add("TopGradualColor");
        //屏幕渐变(压边)
        postEffectsList.Add("Vignetting");

        //全局幕
        //postEffectsList.Add("GlobalFog");
        //屏幕环境光(确定不用这个效果)
        //postEffectsList.Add("SSAO");

    }

    /// <summary>
    /// 只是用于协程
    /// </summary>
    static private Root _staticInsRoot;

    /// <summary>
    /// 只是用于协程
    /// </summary>
    static public Root StaticInsRoot
    {
        get
        {
            if (_staticInsRoot == null)
            {
                GameObject obj = new GameObject("staticInsRoot");
                _staticInsRoot = obj.AddComponent<Root>();
            }
            return _staticInsRoot;
        }
    }
#if SNAILGRASS
	public static GrassManager GrassMagager
	{
		get
		{
			return mGrassManager;
		}
	}

	private static GrassManager mGrassManager = null;
#endif

    /// <summary>
    /// 更新动态单位踢除距离
    /// </summary>
    /// <param name="fDynamicDistance"></param>
    public void UpdateDynamicDistance(float fDynamicDistance)
    {
        if (units == null)
            return;

        DynamicUnit unit = null;
        for (int i = 0; i < units.Count; i++)
        {
            if (units[i] == null || !(units[i] is DynamicUnit))
                continue;

            unit = units[i] as DynamicUnit;
            unit.SetCullingDistance(fDynamicDistance);
        }
    }

    /// <summary>
    /// 修改静态单位踢除距离
    /// </summary>
    public void UpdateStaticUnitDistance()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] == null)
                continue;

            tiles[i].UpdateViewRange();
        }
    }

    /// <summary>
    /// 设置地型乘数因子
    /// </summary>
    /// <param name="value"></param>
    public void SetTrrainCullingRate(float value)
    {
        if (!readCompalte)
        {
            LogSystem.LogWarning("GameScene::SetTrrainCullingRate readComplete is false!!");
        }
        _terrainConfig.SetCullingRate(value);
    }

    /************************************************************************************************
     * 功能 : 场景资源延时销毁
     ************************************************************************************************/

    /** 避免每个场景存储的值不一样长 */
    public long lightDataLength = -1;


    public void Destroy()
    {
        mtCurCenterTile = null;

        //取消所有烘焙
		mBakeTerrains.Clear();

        mBumpChangeDgate.ClearCalls();

        // 避免加载中销毁(还原摄像机设置)
        //#if WUFENG
        if (mainCamera != null && !DelegateProxy.bWuFengMapIng())
        {
            //LogSystem.LogError("mainCamera.cullingMask  = " + mainCamera.cullingMask);
            mainCamera.cullingMask = mainCameraCullingMask;
        }
        //#else
        //        if (mainCamera != null)
        //            mainCamera.cullingMask = mainCameraCullingMask;
        //#endif
        destroyed = true;
        
        Region.ClearBump();

        if (oRoot != null)
        {
            if (isPlaying == true)
                CacheObjects.DestoryPoolObject(oRoot);
            else
                GameObject.DestroyImmediate(oRoot);

            oRoot = null;
        }

        if (_staticInsRoot != null)
        {
            if (isPlaying == true)
                GameObject.DestroyObject(_staticInsRoot.gameObject);
            else
                GameObject.DestroyImmediate(_staticInsRoot.gameObject);

            _staticInsRoot = null;
        }

        if (mSMData != null)
        {
            mSMData = null;
        }

        skybox = null;
        RenderSettings.skybox = null;
        if (this._mapPath != null)
            this._mapPath.Clear();

        _mapPath = null;

        heights = null;
        // 销毁单位
        if (units != null)
        {
            while (units.Count > 0)
            {
                if (units[0].isStatic == true)
                    RemoveUnit(units[0], true, false);
                else
                    RemoveDynamicUnit(units[0] as DynamicUnit, false, true);
            }
            units.Clear();
            units = null;
        }

        mainUnit = null;
        dynamicUnits.Clear();
        dynamicUnits = null;

        int i = 0;
        // 清理区域数据
        for (; i < regions.Count; i++)
        {
            regions[i].Destroy();
        }
        regions.Clear();
        regions = null;

        // 销毁地形切片
        while (tiles.Count > 0)
        {
            RemoveTile(tiles[0], true);
        }
        tiles = null;

        unitsMap.Clear();
        unitsMap = null;
        tilesMap.Clear();
        tilesMap = null;

        if (curSceneUnitsMap != null)
        {
            curSceneUnitsMap.Clear();
            curSceneUnitsMap = null;
        }
        if (sunLightObj != null)
        {
            if (isPlaying == true)
                CacheObjects.DestoryPoolObject(sunLightObj);
            else
                GameObject.DestroyImmediate(sunLightObj);

            sunLightObj = null;
        }
        sunLight = null;

        dirLight = null;
        customLight = null;



        _terrainConfig = null;

        cameras.Clear();
        cameras = null;



        lightDataLength = -1;
        peConfig.Clear();
        peConfig = null;


        postEffectsList.Clear();
        postEffectsList = null;


        for (i = 0; i < peComponentList.Count; i++)
        {
            if (peComponentList[i] != null)
            {
                GameObject.Destroy(peComponentList[i]);
            }

        }
        peComponentList.Clear();
        peComponentList = null;

        mainCamera = null;

        // 去除监听
        sceneLoadCompleListener = null;

        // 释放光照贴图
        if (mSceneLightMap != null)
        {
            mSceneLightMap.Dispose();
            mSceneLightMap = null;
        }
        else
        {
            LogSystem.LogWarning("GameScene::lightmap UnReleaseed!!!");
        }

        if (GameScene.mainScene == this)
            GameScene.mainScene = null;

        //Debug.Log("静态单位缓存数量:" + staticUnitcCache.Count);
        //Debug.Log("动态单位缓存数量:" + dynamicUnitsCache.Count);


        parsers.Clear();
        parsers = null;

        staticUnitcCache.Clear();
        staticUnitcCache = null;

        dynamicUnitsCache.Clear();
        dynamicUnitsCache = null;

        LightmapSettings.lightmaps = null;


#if UNITY_EDITOR
        if (mSnailFogPro != null)
        {
            GameObject.Destroy(mSnailFogPro.gameObject);
            mSnailFogPro = null;
        }
        //if (terrainData != null)
        //    terrainData.Release();
#endif
        //Terrainmapping.Clear();
        // 删除所有资源
        AssetLibrary.RemoveAllAsset();
        //Debug.Log("资源库资源数量:" + AssetLibrary.getBundle().count);
    }
    private bool destroyed = false;

    /*************************************************************************************************
     * 功能 : 是否开启后期处理效果
     *************************************************************************************************/
    public static bool postEffectEnable = false;
    private bool peSet = false;

    /** 记录后期效果组件,避免enable等获取不到情况 */
    public List<MonoBehaviour> peComponentList = new List<MonoBehaviour>();

    /// <summary>
    /// 将postEffectsList中配置的后期效果挂载到peComponentList中
    /// </summary>
    public void CheckPostEffectComponentAppend()
    {
        if (peComponentList.Count < postEffectsList.Count)
        {
            MonoBehaviour component = null;
            for (int i = 0; i < postEffectsList.Count; i++)
            {
                string peName = postEffectsList[i];
                component = mainCamera.GetComponent(peName) as MonoBehaviour;
                if (component == null)
                {
                    component = mainCamera.gameObject.AddComponent(System.Type.GetType(peName)) as MonoBehaviour;
                }
                else
                {
                    component.enabled = !Application.isPlaying;
                }
                peComponentList.Add(component);
            }
        }
    }

    /// <summary>
    /// 高中低配效果
    /// </summary>
    static public List<string[]> levelEffects = new List<string[]>();

    static public void AddEffectsLevel(string[] effects)
    {
        levelEffects.Add(effects);
    }

    static public void RemoveEffectsLevelConfig()
    {
        levelEffects.Clear();
    }

    /// <summary>
    /// 设置高中低配置
    /// </summary>
    /// <param name="index"></param>
    public void SetEffectsLevel(GameQuality gameQuality)
    {
            //设置高光法线
        if (_terrainConfig != null)
            _terrainConfig.bumpCount = gameQuality == GameQuality.HIGH || gameQuality == GameQuality.MIDDLE ? 2 : 0;

        switch (gameQuality)
        {
            case GameQuality.HIGH:
                Shader.EnableKeyword("REALTIME_LIGHTING_ON");
                Shader.DisableKeyword("REALTIME_LIGHTING_OFF");
                Shader.globalMaximumLOD = 500;
                break;
            case GameQuality.MIDDLE:
                Shader.EnableKeyword("REALTIME_LIGHTING_ON");
                Shader.DisableKeyword("REALTIME_LIGHTING_OFF");
                Shader.globalMaximumLOD = 400;
                break;
            case GameQuality.LOW:
            case GameQuality.SUPER_LOW:
                Shader.DisableKeyword("REALTIME_LIGHTING_ON");
                Shader.EnableKeyword("REALTIME_LIGHTING_OFF");
                Shader.globalMaximumLOD = 300;
                break;
        }
        

        SetPeEffect(gameQuality);

        mBumpChangeDgate.ExecuteCalls();
    }

    /// <summary>
    /// 设置后期效果
    /// </summary>
    /// <param name="gameQuality"></param>
    private void SetPeEffect(GameQuality gameQuality)
    {

        if (peComponentList.Count == 0)
            return;

        int index = (int)gameQuality;
        index--;
        if (index >= levelEffects.Count || index < 0)
            return;

        string[] needActiveEffectsList = levelEffects[index];
        if (needActiveEffectsList == null)
            return;

        for (int j = 0; j < postEffectsList.Count; j++)
        {
            bool bEnabled = false;
            for (int i = 0; i < needActiveEffectsList.Length; i++)
            {
                if (postEffectsList[j] == needActiveEffectsList[i])
                {
                    bEnabled = true;
                    break;
                }
            }

            if (peComponentList[j].enabled != bEnabled)
            {
                peComponentList[j].enabled = bEnabled;
            }
        }
    }

    /// <summary>
    /// 开关后期
    /// </summary>
    /// <param name="strPostEffect"></param>
    /// <param name="bEndabled"></param>
    private void EnabeldPostEffect(string strPostEffect, bool bEndabled)
    {
        int index = postEffectsList.IndexOf(strPostEffect);
        if (index < 0 || index >= peComponentList.Count)
            return;

        if (peComponentList[index] == null)
            return;

        if (peComponentList[index].enabled == bEndabled)
            return;

        peComponentList[index].enabled = bEndabled;
    }

    /*************************************************************************************************
     * (无用)功能 : 是否开启后期处理效果
     *************************************************************************************************/
    public void ActivePostEffect(bool value)
    {
        if (mainCamera == null)
            return;

        postEffectEnable = value;

        if (peSet == value)
            return;
        peSet = value;

        // 检测后期组件已经被添加
        CheckPostEffectComponentAppend();

        MonoBehaviour component = null;
        for (int i = 0; i < postEffectsList.Count; i++)
        {
            string peName = postEffectsList[i];
            component = peComponentList[i];

            if (component != null)
            {
                if (value == true)
                    if (peConfig.ContainsKey(peName) == true)
                        component.enabled = true;
                    else
                        component.enabled = false;
                //Debug.Log("加载效果 : " + peName + " open : " + component.enabled);
            }
            else
            {
                //Debug.Log("丢失效果: " + peName);
            }
        }
    }

    private bool peConfigLoaded = false;

    /// <summary>
    /// 1.将配置后期效果挂载到Camera上
    /// 2.读取效果配置
    /// </summary>
    public void LoadPostEffectConfig()
    {
        if (peConfigLoaded == true)
            return;

        // 检测后期组件已经被添加
        CheckPostEffectComponentAppend();

        int i = 0;
        int j = 0;
        PEBase pe = null;
        for (i = 0; i < postEffectsList.Count; i++)
        {
            string peName = postEffectsList[i];
            pe = peComponentList[i] as PEBase;

            if (pe != null)
            {
                if (peConfig.ContainsKey(peName))
                {
                    byte[] bytes = peConfig[peName];
                    if (bytes != null)
                    {
                        MemoryStream me = new MemoryStream(bytes);
                        BinaryReader br = new BinaryReader(me);

                        // 是否开启当前效果
                        pe.enabled = br.ReadBoolean();
                        int paramCount = br.ReadInt32();
                        for (j = 0; j < paramCount; j++)
                        {
                            int type = br.ReadInt32();
                            string paramName = br.ReadString();
                            // int类型
                            if (type == 0)
                                pe.varData.SetInt(paramName, br.ReadInt32());
                            // float类型
                            else if (type == 1)
                                pe.varData.SetFloat(paramName, br.ReadSingle());
                            // color类型
                            else if (type == 2)
                                pe.varData.SetColor(paramName, new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                            // vector4类型
                            else if (type == 3)
                                pe.varData.SetVector(paramName, new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                        }
                        br.Close();
                        me.Close();
                        pe.LoadParams();
                    }
                }
                else
                {
                    pe.enabled = false;
                }
            }
        }
        peConfigLoaded = true;
    }

    /****************************************************************************************************
     * 功能 : 首次运行
     ****************************************************************************************************/
    //public void FirstRun()
    //{
    //    if (staticInsRoot == null)
    //    {
    //        staticInsRoot = new GameObject("staticInsRoot");
    //        _rootIns = staticInsRoot.AddComponent<Root>();
    //    }
    //}

    /** 获取地形的配置 */
    public TerrainConfig terrainConfig
    {
        get
        {
            return _terrainConfig;
        }
    }

    /** 获取地形高光法线 */
    public int SceneBumpCount
    {
        get
        {
            if (_terrainConfig != null)
                return _terrainConfig.bumpCount;

            return 0;
        }
    }

    /*****************************************************************************************************
     * 功能 : 
     *****************************************************************************************************/

    public MapPath mapPath
    {
        get { return _mapPath; }
        set
        {
            _mapPath = value;
        }
    }

    /// <summary>
    /// 打断寻路(避免发起无用寻路)
    /// </summary>
    /// <returns></returns>
    public bool BreakAutoMove()
    {
        if (_mapPath == null)
            return false;

        _mapPath.pathFindEnd = true;
        return true;
    }

    #region 获取坐标点的路径阻塞情况


    /******************************************************************************************************
     * 功能 ：获取坐标点的路径阻塞情况
     * 返回 : [true 可行走], [false 不可行走]
     * 注解 : 默认0,0地域0,0切片（世界坐标(0,0)为）
     *******************************************************************************************************/
    public bool IsValidForWalk(Vector3 worldPostion, int collisionSize)
    {
        if (_mapPath == null)
            return false;

        int gx = getGridX(worldPostion.x);
        int gy = getGridY(worldPostion.z);

        if (gx < 0 || gy < 0 || gx >= _mapPath.grids.GetLength(0) || gy >= _mapPath.grids.GetLength(1))
            return false;

        int val = (_mapPath.grids[gx, gy] >> collisionSize) & 1;
        if (val == 1)
            return false;
        else
        {
            val = (_mapPath.grids[gx, gy] >> (collisionSize - 1)) & 1;
            if (val == 1)
                return false;
            else
                return IsDynamicValidForWalk(worldPostion, worldPostion, mCanMoveHeight, collisionSize);
        }
    }
    
    /// <summary>
    /// 获取坐标点的路径阻塞情况
    /// </summary>
    /// <param name="vFormPos"></param>
    /// <param name="vDestPos"></param>
    /// <param name="fCanMoveHeight"></param>
    /// <param name="collisionSize"></param>
    /// <returns></returns>
    public bool IsValidForWalk(Vector3 vFormPos, Vector3 vDestPos, float fCanMoveHeight, int collisionSize)
    {
        if (_mapPath == null)
            return false;

        int gx = getGridX(vDestPos.x);
        int gy = getGridY(vDestPos.z);

        if (gx < 0 || gy < 0 || gx >= _mapPath.grids.GetLength(0) || gy >= _mapPath.grids.GetLength(1))
            return false;

        int val = (_mapPath.grids[gx, gy] >> collisionSize) & 1;
        if (val == 1)
            return false;
        else
        {
            val = (_mapPath.grids[gx, gy] >> (collisionSize - 1)) & 1;
            if (val == 1)
                return false;
            else
                return IsHeightValidForWalk(vFormPos, vDestPos, fCanMoveHeight, collisionSize);
        }
    }

    /// <summary>
    /// 飞行检测
    /// </summary>
    /// <param name="vFormPos"></param>
    /// <param name="vDestPos"></param>
    /// <param name="fCanMoveHeight"></param>
    /// <param name="collisionSize"></param>
    /// <returns></returns>
    public bool IsValidForFly(Vector3 vFormPos, Vector3 vDestPos, float fCanMoveHeight, int collisionSize)
    {
        //return IsHeightValidForWalk(vFormPos, vDestPos, fCanMoveHeight, collisionSize);
        return isHeightValiadForFly(vFormPos, vDestPos, fCanMoveHeight, collisionSize);
    }

    /// <summary>
    /// 跳跃检测
    /// 优先判断抛物线高度是否满足条件（抛物线不满足，格子满足也不可走）
    /// 只要抛物满足条件那么返回值必然为true
    /// bCanStand 格子可站立 && 起点与终点高度差小于不可跳跃值
    /// </summary>
    /// <param name="vFormPos"></param>
    /// <param name="vDestPos"></param>
    /// <param name="fCanMoveHeight"></param>
    /// <param name="collisionSize"></param>
    /// <param name="fJumpHeight"></param>
    /// <param name="fJumpDistance"></param>
    /// <param name="fDistance"></param>
    /// <param name="iResult">-1未知0可走1格子不可走</param>
    /// <returns></returns>
    public bool IsValidForJump(Vector3 vFormPos, Vector3 vDestPos, float fCanMoveHeight,
        int collisionSize, float fJumpHeight, float fJumpDistance, float fDistance, ref bool bCanStand)
    {
        bCanStand = false;
        if (_mapPath == null)
            return false;

        //优先判断抛物线高度是否满足条件
        if (!IsParabolaValidForWalk(vFormPos, vDestPos, fCanMoveHeight, collisionSize, fJumpHeight, fJumpDistance, fDistance))
        {
            return false;
        }

        int gx = getGridX(vDestPos.x);
        int gy = getGridY(vDestPos.z);

        if (gx < 0 || gy < 0 || gx >= _mapPath.grids.GetLength(0) || gy >= _mapPath.grids.GetLength(1))
            return false;

        int val = (_mapPath.grids[gx, gy] >> collisionSize) & 1;
        //格子可站立
        if (val != 1)
        {
            val = (_mapPath.grids[gx, gy] >> (collisionSize - 1)) & 1;
            //格子可站立 为什么要-1再判断？
            if (val != 1)
            {
                float fDestHeight = SampleHeight(vDestPos);
                //格子可站立 && 起点与终点高度差小于不可跳跃值
                bCanStand = (vFormPos.y - fDestHeight < fJumpMaxHeightDiff);
            }
        }
        return true;
    }

    /*********************************************************************************************
     * 功能 : 只检测动态碰撞
     * 注解 : 只计算静态的格子
     *********************************************************************************************/
    public bool IsValidForWalk(Vector3 worldPostion, int collisionSize, out int gridType)
    {
        int x = getGridX(worldPostion.x);
        int y = getGridY(worldPostion.z);

        gridType = 0;

        // 超出范围返回无效数据
        if (x < 0 || y < 0 || x >= _mapPath.grids.GetLength(0) || y >= _mapPath.grids.GetLength(1))
            return false;

        int type = _mapPath.grids[x, y] >> mapPath.gridTypeMask;
        if (type > 0)
        {
            gridType = type;
            return true;
        }
        gridType = 0;
        int val = (_mapPath.grids[x, y] >> collisionSize) & 1;
        if (val == 1)
            return false;
        else
        {
            val = (_mapPath.grids[x, y] >> (collisionSize - 1)) & 1;
            if (val == 1)
                return false;
            else
                return true;
        }
    }

    #endregion

    public int getGridType(Vector3 worldPostion)
    {
        int x = getGridX(worldPostion.x);
        int y = getGridY(worldPostion.z);

        // 超出范围返回无效数据
        if (x < 0 || y < 0 || x >= _mapPath.grids.GetLength(0) || y >= _mapPath.grids.GetLength(1))
            return 0;

        int type = _mapPath.grids[x, y] >> mapPath.gridTypeMask;
        return type;
    }


    public int getGridX(float value)
    {
        if (_terrainConfig == null || mapPath == null)
        {
            return -1;
        }
        return Mathf.FloorToInt(value / _terrainConfig.gridSize) + mapPath.halfWidth;
    }

    public int getGridY(float value)
    {
        if (_terrainConfig == null || mapPath == null)
        {
            return -1;
        }
        return Mathf.FloorToInt(value / _terrainConfig.gridSize) + mapPath.halfHeight;
    }

    /*******************************************************************************************************
     * 功能 : 创建空单位
     *******************************************************************************************************/

    public GameObjectUnit CreateEmptyUnit(int createID = -1)
    {
        GameObjectUnit unit = null;
        if (createID < 0)
        {
            unitIdCount += 1;
            createID = unitCount;
        }
        if (staticUnitcCache.Count > 0)
        {
            unit = staticUnitcCache[0];
            unit.destroyed = false;
            staticUnitcCache.RemoveAt(0);
        }
        else
        {
            unit = new GameObjectUnit(createID);
        }
        return unit;
    }

    public void RemoveEmptyUnit(GameObjectUnit unit, bool bDestroy = false)
    {
        //LogSystem.LogError("staticUnitcCache:" + staticUnitcCache.Count);
        if (bDestroy)
        {
            //如果是删除场景,不处理缓存
        }
        else
        {
            if (staticUnitcCache.Contains(unit) == false)
            {
                staticUnitcCache.Add(unit);
            }
        }
    }

    #region 创建动态单元

    private const int dynamicUnitStartCount = 500000;

    /*************************************************************************************************************************
     * 功能 ：添加动态的单位到场景中
     * 注解 : 动态单位需要指定剔除因子
     *************************************************************************************************************************/
    public DynamicUnit CreateDynamicUnit(Vector3 pos, string prePath, float radius, int uType = 0, float dynamiCullingDistance = -1f,
        GameObjectUnit.CreateInsListener createIns = null, GameObjectUnit.ActiveListener active = null, GameObjectUnit.DestroyInsListener destroy = null,
        DynamicUnit.MoveListener move = null, bool bNeedSampleHeight = true)
    {
        unitIdCount += 1;
        DynamicUnit unit = null;

        /** 
        if (dynamicUnitsCache.Count > 0)
        {
            unit = dynamicUnitsCache[0];
            unit.createID = unitIdCount + dynamicUnitStartCount;
            unit.prePath = prePath;
            unit.destroyed = false;
            unit.isMainUint = false;
            unit.willRemoved = false;
            unit.scene = this;
            unit.type = uType;
            unit.isStatic = false;
            AddUnit(unit);
            if (dynamiCullingDistance > 0f)
                unit.near = dynamiCullingDistance;
            else
                unit.near = terrainConfig.dynamiCullingDistance;
            unit.far = unit.near + 2f;
            dynamicUnitsCache.RemoveAt(0);
        }
        else
       **/
        {
            unit = DynamicUnit.Create(this, pos, unitIdCount + dynamicUnitStartCount, prePath, radius, dynamiCullingDistance, bNeedSampleHeight);
            unit.isStatic = false;
            AddUnit(unit);
            unit.type = uType;
        }

        unit.activeListener = active;
        unit.destroyInsListener = destroy;
        unit.createInsListener = createIns;
        unit.moveListener = move;

        unit.Position = pos;
        if (unit.needSampleHeight)
        {
            unit.SetPostionY(this.SampleHeight(pos));
        }
        return unit;
    }

    /*************************************************************************************************************************
     * 功能 ：立即创建动态单位,如技能等
     *************************************************************************************************************************/
    public DynamicUnit CreateDynamicUnitImmediately(Vector3 pos, string prePath, float radius, int uType = 0, float dynamiCullingDistance = -1f,
        GameObjectUnit.CreateInsListener createIns = null, GameObjectUnit.ActiveListener active = null, GameObjectUnit.DestroyInsListener destroy = null,
        DynamicUnit.MoveListener move = null, bool bNeedSampleHeight = true)
    {
        DynamicUnit unit = this.CreateDynamicUnit(pos, prePath, radius, uType, dynamiCullingDistance, createIns, active, destroy, move, bNeedSampleHeight);

        float dx = unit.Position.x - eyePos.x;
        float dz = unit.Position.z - eyePos.z;
        unit.viewDistance = dx * dx + dz * dz;

        // 避免动态单位载入时场景没有加载, 需要重新采样高度
        if (unit.Position.y > 99999999f)
            unit.SetPostionY(SampleHeight(unit.Position));

        if (unit.viewDistance < (unit.far * unit.far))            // 动态单位缓和剔除
        {
            unit.Visible();
        }
        return unit;
    }

    public DynamicUnit CreateDynamicUnitImmediately(GameObject ins, string prePath, float radius, int uType = 0, float dynamiCullingDistance = -1f,
        GameObjectUnit.CreateInsListener createIns = null, GameObjectUnit.ActiveListener active = null, GameObjectUnit.DestroyInsListener destroy = null,
        DynamicUnit.MoveListener move = null, bool bNeedSampleHeight = true)
    {
        DynamicUnit unit = this.CreateDynamicUnit(ins.transform.position, prePath, radius, uType, dynamiCullingDistance, createIns, active, destroy, move);

        float dx = unit.Position.x - eyePos.x;
        float dz = unit.Position.z - eyePos.z;
        unit.viewDistance = dx * dx + dz * dz;

        unit.Ins = ins;

        // 避免动态单位载入时场景没有加载, 需要重新采样高度
        if (unit.Position.y > 99999999f)
            unit.SetPostionY(this.SampleHeight(unit.Position));

        if (unit.viewDistance < (unit.far * unit.far))            // 动态单位缓和剔除
        {
            unit.Visible();
        }
        return unit;
    }

    #endregion

    /**************************************************************************************************************
     * 功能 : 在可视列表中查找单位
     **************************************************************************************************************/
    public GameObjectUnit FindUnit(string name)
    {
        if (units == null)
            return null;

        int i = 0;
        for (; i < units.Count; i++)
        {
            if (units[i] != null && units[i].Ins != null)
            {
                if (units[i].Ins.name == name)
                    return units[i];
            }
        }
        return null;
    }

    /***************************************************************************************
     * 功能 : 是否包含指定单位0
     ***************************************************************************************/
    public bool ContainUnit(GameObjectUnit unit)
    {
        return unitsMap.ContainsKey(unit.createID);
    }

    /*****************************************************************************************************
     * 功能 ： 读取切片数据的时候创建单位
     ******************************************************************************************************/
    public bool AddUnit(GameObjectUnit unit)
    {
        if (unitsMap.ContainsKey(unit.createID))
            return false;

        units.Add(unit);
        unitsMap.Add(unit.createID, unit);
        unitCount++;
        if (unit.isStatic)
        {
            sumSceneLoadUnitTick++;
        }
        return true;
    }

    /**************************************************************************************************
     * 功能 : 销毁动态单位
     * 修改人：zhangrj
     * 日期：20160229
     * 功能：增加移除link数据(1046-1050)
     ****************************************************************************************************/
    public void RemoveDynamicUnit(DynamicUnit unit, bool cache = true, bool immediately = false)
    {
        if (unit == null) return;
        if (unit.mDynState == DynamicState.LINK_PARENT || unit.mDynState == DynamicState.LINK_PARENT_CHILD)
        {
            unit.RemoveAllLinkDynamic();
        }

        if (mapPath != null)
            mapPath.SetDynamicCollision(unit.Position, unit.collisionSize, true);

        unit.Invisible();
        unit.Destroy();
        unitsMap.Remove(unit.createID);
        units.Remove(unit);
        unitCount--;
        unit.createID = -1;         // 无效ID
        //if (cache == true)
        //{
        //    if (dynamicUnitsCache.Contains(unit) == false)
        //    {
        //        dynamicUnitsCache.Add(unit);
        //    }
        //}
    }

    /****************************************************************************************************
     * 功能 ： 在运行时销毁单位
     ******************************************************************************************************/
    public void RemoveUnit(GameObjectUnit unit, bool destroy = false, bool cache = false)
    {
        unit.Invisible();
        unitsMap.Remove(unit.createID);
        units.Remove(unit);
        if (destroy == true)
            unit.Destroy();
        unitCount--;

        if (cache == true)
        {
            if (staticUnitcCache.Contains(unit) == false)
                staticUnitcCache.Add(unit);
        }
    }

    #region tile相关方法

    /*******************************************************************************************************
     * 功能 ： 添加地形切片到场景中
     *******************************************************************************************************/
    public void AddTile(Tile tile)
    {
        uint ukey = tile.GetKey();
        if (tilesMap.ContainsKey(ukey))
            return;
        // tile.Visible();
        tilesMap.Add(ukey, tile);
        tiles.Add(tile);
        tile.SetNeighborTile();
        tile.SetNeighborTerrain();
    }

    public bool ContainTile(Tile tile)
    {
        uint ukey = tile.GetKey();
        if (tilesMap != null && tilesMap.ContainsKey(ukey))
            return true;
        return false;
    }

    public void RemoveTile(Tile tile, bool destroy = false)
    {
        uint ukey = tile.GetKey();
        if (tilesMap != null && tilesMap.ContainsKey(ukey))
        {
            tile.Invisible();
            if (destroy == true)
                tile.Destroy();
            tilesMap.Remove(ukey);
            tiles.Remove(tile);
        }
    }
    public static uint StringToKey(string strKey)
    {
        string[] strGroup = strKey.Split('_');
        if (strGroup.Length >= 4)
        {
            int iregionX, iregionY, iTilex, iTileY;
            int.TryParse(strGroup[0], out iregionX);
            int.TryParse(strGroup[0], out iregionY);
            int.TryParse(strGroup[0], out iTilex);
            int.TryParse(strGroup[0], out iTileY);
            uint uKey = GameScene.GetKey(iregionX, iregionY, iTilex, iTileY);
        }

        return 0;
    }
    public Tile FindTile(uint ukey)
    {
        Tile tile;
        tilesMap.TryGetValue(ukey, out tile);
        return tile; 
    }

    #endregion
    private GameObject sunLightObj;
    public Light sunLight = null;
    public Light dirLight;
    public DirectionalLightForCharacter customLight;

    /// <summary>
    /// 获取角色平行光
    /// </summary>
    /// <returns></returns>
    public Light GetDirLight()
    {
        if (dirLight != null)
        {
            return dirLight;
        }
        else
        {
            LogSystem.LogWarning("dirLight is missed");
            return null;
        }
    }

    /// <summary>
    /// 获取角色虚拟光
    /// </summary>
    /// <returns></returns>
    public DirectionalLightForCharacter GetCustomLight()
    {
        if (customLight != null)
        {
            return customLight;
        }
        return null;
    }

    /// <summary>
    /// 更新光照方向
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public bool UpdateSingleLightData(GameObject role)
    {
        if (customLight == null)
            return false;

        customLight.UpdateSingleLightData(role);
        return true;
    }

    private int miSunLightLayer = 0;
    /// <summary>
    /// 设置太阳光(目前只用于照射主角产生投影)
    /// </summary>
    public int iSunLightLayer
    {
        get
        {
            return miSunLightLayer;
        }
        set
        {
            miSunLightLayer = value;
            if (sunLight != null)
            {
                sunLight.cullingMask = miSunLightLayer;
            }
        }
    }



    public Material skybox;

    public int lightmapCount = 0;

    public static float mfCurTime = 0f;
    private static string mstrCurName = string.Empty;
    public static void OutPutTime(string strPreName, bool bStart = false)
    {
        if (mstrCurName == strPreName)
            return;
        if (bStart)
        {
            mfCurTime = Time.realtimeSinceStartup;
        }
        mstrCurName = strPreName;
        //Debug.Log(strPreName + ":" + (Time.realtimeSinceStartup - mfCurTime));
    }
    /********************************************************************************************************
     * 功能 ： 读取场景数据
     ********************************************************************************************************/
    public GameScene Read(byte[] bytes)
    {
        int i = 0;
        int j = 0;
        long pos = 0;

        sceneLoadUnitTick = 0;
        sumSceneLoadUnitTick = 0;

        MemoryStream me = new MemoryStream(bytes);
        BinaryReader br = new BinaryReader(me);

        #region 解压缩

        //解压缩
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 999999)
        {
            if (isPlaying)
            {
                LogSystem.LogWarning("场景不支持压缩!!");
                return this;
            }
            long len = br.ReadInt64();
            byte[] zipbytes = br.ReadBytes((int)len);
            MemoryStream zipStream = new MemoryStream(zipbytes);
            MemoryStream outStream = new MemoryStream();
            StreamZip.Unzip(zipStream, outStream);
            me = outStream;
            zipStream.Dispose();
            br.Close();

            br = new BinaryReader(me);
            br.BaseStream.Position = 0;
            //Debug.Log("解压场景文件: " + me.Length);
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        #endregion

        sceneID = br.ReadInt32();

        #region 读取terrainConfig

        _terrainConfig = new TerrainConfig();
        // 读取地形配置数据
        _terrainConfig.regionSize = br.ReadInt32();
        _terrainConfig.tileSize = br.ReadInt32();

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 60001)
        {
            _terrainConfig.gridCountPerUnit = br.ReadInt32();
        }
        else
        {
            _terrainConfig.gridCountPerUnit = 1;
            br.BaseStream.Position = pos;
        }

        // 设置地形切片的格子分辨率
        _terrainConfig.gridResolution = _terrainConfig.tileSize * _terrainConfig.gridCountPerUnit;

        _terrainConfig.heightmapResolution = _terrainConfig.gridResolution;
        _terrainConfig.sceneHeightmapResolution = _terrainConfig.gridResolution * 5 * 3;
        _terrainConfig.gridSize = (float)_terrainConfig.tileSize / (float)_terrainConfig.gridResolution;        // 计算格子尺寸
        //Debug.Log("区域尺寸: " + _terrainConfig.regionSize + " 切片尺寸: " + _terrainConfig.tileSize);

        // 计算场景大小
        _terrainConfig.sceneWidth = _terrainConfig.regionSize * 3;
        _terrainConfig.sceneHeight = _terrainConfig.regionSize * 3;


        //int ununseint = br.ReadInt32();
        //ununseint = br.ReadInt32();
        _terrainConfig.defaultTerrainHeight = br.ReadSingle();
        _terrainConfig.maxTerrainHeight = br.ReadSingle();

        // 区域尺寸
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10001)
        {
            br.ReadInt32();
            br.ReadInt32();
        }
        else
            br.BaseStream.Position = pos;

        // 
        _terrainConfig.cameraLookAt.x = br.ReadSingle();
        _terrainConfig.cameraLookAt.y = br.ReadSingle();
        _terrainConfig.cameraLookAt.z = br.ReadSingle();

        _terrainConfig.cameraFarClip = br.ReadSingle();
        if (isPlaying == false)
            _terrainConfig.cameraFarClip = (_terrainConfig.cameraFarClip < 2000) ? 2000 : _terrainConfig.cameraFarClip;
        else
            _terrainConfig.cameraFarClip = (_terrainConfig.cameraFarClip < 200) ? 200 : _terrainConfig.cameraFarClip;

        // 读取游戏场景摄像机视角
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10031)
        {
            _terrainConfig.cameraRotationX = br.ReadSingle();
            _terrainConfig.cameraRotationY = br.ReadSingle();
            _terrainConfig.cameraRotationZ = br.ReadSingle();
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 33111)
        {
            int scount = br.ReadInt32();
            br.BaseStream.Position += (scount * 4);
            ////Debug.Log ("share ins count : " + scount);
            //for (i = 0; i < scount; i++)
            //{
            //    int sID = br.ReadInt32();
            //}
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        // 读取系统雾效设置
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 60003)
        {
            _terrainConfig.bumpCount = br.ReadInt32();
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 60004)
        {
            _terrainConfig.terrainShininess = br.ReadSingle();
            _terrainConfig.terrainGloss = br.ReadSingle();
            _terrainConfig.terrainSpecColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        #endregion

        // 读取系统雾效设置
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10032)
        {
            br.BaseStream.Position += (1 + 7 * 4);
            //RenderSettings.fog = br.ReadBoolean();
            //RenderSettings.fogMode = (FogMode)br.ReadInt32();
            //RenderSettings.fogColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            //RenderSettings.fogDensity = br.ReadSingle();
            //RenderSettings.fogStartDistance = br.ReadSingle();
            //RenderSettings.fogEndDistance = br.ReadSingle();
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10012)
        {
            if (br.ReadInt32() == 1)
            {
                string sbMatPath = br.ReadString();
                GameObjectUnit.ThridPardLoad(sbMatPath, OnSkyBoxLoad, null);
            }
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        // 读取是否显示地形
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 13001)
        {
            _terrainConfig.enableTerrain = br.ReadBoolean();
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        // 读取天气
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10041)
            _terrainConfig.weather = br.ReadInt32();
        else
            br.BaseStream.Position = pos;

        //if (GameObjectUnit.thridPardResourManager != null)
        //needPreload = false;

        #region 环境光
        // 环境光
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 660001)
        {
            RenderSettings.ambientMode = (UnityEngine.Rendering.AmbientMode)br.ReadInt32();

            RenderSettings.ambientLight = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

            RenderSettings.ambientSkyColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            RenderSettings.ambientEquatorColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            RenderSettings.ambientGroundColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

            RenderSettings.ambientIntensity = br.ReadSingle();
        }
        else
        {
            br.BaseStream.Position = pos;
            RenderSettings.ambientLight = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }

        #endregion

        #region 太阳光

        LensFlare fl = null;
        // 太阳光
        GameObject sunLightObjPre = Resources.Load("Textures/LightFlares/Sun01", typeof(GameObject)) as GameObject;
        if (sunLightObjPre == null)
        {
            sunLightObj = new GameObject();
            sunLight = sunLightObj.AddComponent<Light>();

        }
        else
        {
            sunLightObj = GameObject.Instantiate(sunLightObjPre);
            sunLight = sunLightObj.GetComponent<Light>();
            fl = sunLightObj.GetComponent<LensFlare>();
            fl.fadeSpeed = 0.05f;
        }
        sunLightObj.name = "sunLight";
        if (isPlaying)
        {
            sunLight.cullingMask = iSunLightLayer;
        }

        // 记录光照烘焙属性
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 660002)
        {
            int m_Lightmapping = br.ReadInt32();
#if UNITY_EDITOR
            SerializedObject so = new SerializedObject(sunLight);
            SerializedProperty property = so.FindProperty("m_Lightmapping");
            property.intValue = m_Lightmapping;//"Baked";
            so.ApplyModifiedProperties();
#endif
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        sunLight.type = LightType.Directional;
        sunLight.color = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        sunLight.transform.rotation = new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        sunLight.intensity = br.ReadSingle();

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 500001)
        {
            //sunLight.shadows = (LightShadows)br.ReadInt32();
            br.BaseStream.Position += 4;
            sunLight.shadows = LightShadows.Hard;
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        sunLight.shadowStrength = br.ReadSingle();
        sunLight.shadowBias = br.ReadSingle();

        /*sunLight.shadowSoftness = */
        br.ReadSingle();
        /*sunLight.shadowSoftnessFade =*/
        br.ReadSingle();
        sunLight.renderMode = LightRenderMode.Auto;

        Shader.SetGlobalVector("_WorldSpaceSceneLightForCharacter", -sunLight.transform.forward);
        Shader.SetGlobalColor("_WorldSpaceSceneLightColorForCharacter", sunLight.color * sunLight.intensity);
        sunLight.enabled = false;
        #endregion

        #region 读取水面设置

        // 载入默认水面设置
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10010)
        {
            _terrainConfig.waveScale = br.ReadSingle();
            _terrainConfig.waveSpeed = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            _terrainConfig.horizonColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            _terrainConfig.defaultWaterHeight = br.ReadSingle();
            _terrainConfig.waterVisibleDepth = br.ReadSingle();
            _terrainConfig.waterDiffValue = br.ReadSingle();
            _terrainConfig.colorControl = AssetLibrary.Load(br.ReadString(), AssetType.Texture2D).texture2D;
            _terrainConfig.waterBumpMap = AssetLibrary.Load(br.ReadString(), AssetType.Texture2D).texture2D;
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        // 载入水面高光设置
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10017)
        {
            _terrainConfig.waterSpecRange = br.ReadSingle();
            _terrainConfig.waterSpecStrength = br.ReadSingle();
            _terrainConfig.sunLightDir = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        #endregion

        // 判断是否在unity4.6中保存
        unityVersion = br.ReadSingle();
        if (unityVersion < 4.0f)
            unityVersion = 5.3f;

        //br.ReadSingle(); br.ReadSingle(); br.ReadSingle();// br.ReadSingle ();
        //br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
        //br.ReadSingle(); br.ReadSingle(); br.ReadSingle(); br.ReadSingle();
        br.BaseStream.Position += (11 * 4);

        #region 读取点光效果

        // 读取点光效果
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10021)
        {
            //_terrainConfig.pointLightRangeMax = br.ReadSingle();
            //_terrainConfig.pointLightRangeMin = br.ReadSingle();
            //_terrainConfig.pointLightIntensity = br.ReadSingle();
            br.BaseStream.Position += 3 * 4;
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10051)
        {
            //_terrainConfig.enablePointLight = br.ReadBoolean();
            //_terrainConfig.enablePointLight = false;
            //_terrainConfig.pointLightColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            br.BaseStream.Position += 1 + 3 * 4;
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        #endregion

        #region 读取角色光照设置

        // 读取角色光照设置
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10022)
        {
            //_terrainConfig.rolePointLightPostion = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            //_terrainConfig.rolePointLightColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            //_terrainConfig.rolePointLightRange = br.ReadSingle();
            //_terrainConfig.rolePointLightIntensity = br.ReadSingle();
            br.BaseStream.Position += 8 * 4;
        }
        else
        {
            br.BaseStream.Position = pos;
        }
        #endregion

        peConfig.Clear();

        #region 读取后期属性

        // 读取后期参数
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 70000)
        {
            // 后期效果数量
            int peCount = br.ReadInt32();
            for (i = 0; i < peCount; i++)
            {
                string peName = br.ReadString();
                int len = br.ReadInt32();

                byte[] pebytes = br.ReadBytes(len);
                peConfig.Add(peName, pebytes);
            }
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        // 烘焙冷暖色
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 70001)
        {
            //_terrainConfig.coolColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            //_terrainConfig.warmColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            br.BaseStream.Position += 8 * 4;
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        // 设置冷暖色调
        //Shader.SetGlobalVector("coolColor", _terrainConfig.coolColor);
        //Shader.SetGlobalVector("warmColor", _terrainConfig.warmColor);

        // 读取后期属性
        //pos = br.BaseStream.Position;
        //if (br.ReadInt32() == 10017)
        //{
        //    string p = br.ReadString();
        //    if (p.Contains("Terrain") == false)
        //    {
        //        br.ReadSingle();
        //        br.ReadSingle();
        //        br.ReadSingle();
        //        br.ReadSingle();

        //        br.ReadSingle();
        //        br.ReadSingle();
        //        br.ReadSingle();

        //        br.ReadInt32();
        //    }
        //    else
        //    {
        //        br.BaseStream.Position = pos;
        //        br.ReadInt32();
        //    }
        //}
        //else
        //{
        //    br.BaseStream.Position = pos;
        //}

        #endregion

        // 基础地形纹理
        string baseSplatPath = br.ReadString();
        //Asset baseTileAsset = AssetLibrary.Load(baseSplatPath, AssetType.Texture2D);
        //_terrainConfig.baseSplat.texture = baseTileAsset.texture2D;
        //_terrainConfig.baseSplat.key = baseSplatPath;

        //_terrainConfig.baseSplat.tilingOffset.x = br.ReadSingle();
        //_terrainConfig.baseSplat.tilingOffset.y = br.ReadSingle();
        //_terrainConfig.baseSplat.tilingOffset.z = br.ReadSingle();
        //_terrainConfig.baseSplat.tilingOffset.w = br.ReadSingle();
        br.BaseStream.Position += (4 * 4);

        _terrainConfig.tileCountPerSide = _terrainConfig.regionSize / _terrainConfig.tileSize;
        _terrainConfig.tileCountPerRegion = _terrainConfig.tileCountPerSide * _terrainConfig.tileCountPerSide;

        // 创建的单位数量标示
        unitIdCount = br.ReadInt32();

        #region 配置相关,踢除距离


        // 切片剔除因子
        //br.ReadSingle(); 
        //修改将tileculling配置放在SceneConfig.xml
#if UNITY_EDITOR
        if (GameScene.isPlaying)
        {
            _terrainConfig.tileCullingDistance = br.ReadSingle();
        }
        else
        {
            br.BaseStream.Position += 4;
        }
#else
        br.BaseStream.Position += 4;
#endif

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 60001)
            _terrainConfig.tileRange = br.ReadInt32();
        else
            br.BaseStream.Position = pos;

        //由于场景异步加载外部设置会晚一些，在这边就不能设置值了
        // 单位剔除因子
        br.BaseStream.Position += 4;  //_terrainConfig.unitCullingDistance = br.ReadSingle();
        _terrainConfig.unitCullingDistance = 1f;
        _terrainConfig.cullingBaseDistance = br.ReadSingle();

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10018)
        {
            _terrainConfig.dynamiCullingDistance = br.ReadSingle();
        }
        else
            br.BaseStream.Position = pos;

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10023)
        {
            _terrainConfig.cullingAngleFactor = br.ReadSingle();
        }
        else
            br.BaseStream.Position = pos;

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10024)
        {
            br.BaseStream.Position += 4; //_terrainConfig.viewAngleLodFactor = br.ReadSingle();
        }
        else
            br.BaseStream.Position = pos;

        if (_terrainConfig.cullingAngleFactor < 0.001f)
            _terrainConfig.cullingAngleFactor = 3f;

        // 游戏运行时强制参数(目测无用方法)
        //if (isEditor == false && isPlaying == false)
        //{
        //    _terrainConfig.tileCullingDistance = 280f;
        //    _terrainConfig.cullingBaseDistance = 20.5f;
        //    _terrainConfig.unitCullingDistance = 0.7f;
        //    _terrainConfig.dynamiCullingDistance = 16f;
        //}

        #endregion

#if UNITY_EDITOR
        // 载入寻路数据 --------------------------------------------------------------
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10003)
        {
            if (isPlaying)
            {
                LogSystem.LogWarning("(无用的载入寻路数据)请用策划工程，重新何存场景");
                return this;
            }
            MapPath p = new MapPath(_terrainConfig.gridSize);
            p.Read(br);
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        // 读取预加载资源
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 20003)
        {
            if (isPlaying)
            {
                LogSystem.LogWarning("(无用的预加载资源)请用策划工程，重新何存场景");
                return this;
            }
            int assetCount = br.ReadInt32();
            LogSystem.LogWarning("Error!!不应有预加载资源");
            return this;
            //for (i = 0; i < assetCount; i++)
            //{
            //    br.ReadInt32();
            //    br.ReadString();
            //}
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        // 读取场景使用的地形文件, 并且预加载地形纹理
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 20004)
        {
            if (isPlaying)
            {
                LogSystem.LogWarning("(无用的预加载资源)请用策划工程，重新何存场景");
                return this;
            }
            int assetCount = br.ReadInt32();
            for (i = 0; i < assetCount; i++)
            {
                //preloadUnitAssetTypeList.Add((int)AssetType.Texture2D);
                br.ReadString();
            }
        }
        else
        {
            br.BaseStream.Position = pos;
        }
#endif

        #region lightmap(无用)

        // 读取场景烘焙数据
        lightmapCount = br.ReadInt32();
        //int lightmapsMode = 0;
        //pos = br.BaseStream.Position;
        //if (br.BaseStream.Position < br.BaseStream.Length)
        //{
        //    if (br.ReadInt32() == 10008)
        //        br.ReadInt32();
        //    else
        //        br.BaseStream.Position = pos;
        //}

        /**
        List<LightmapData> lmds = new List<LightmapData>();
        if (lightmapsMode == 0)
        {
            for (i = 0; i < count; i++)
            {
                string farPath = "Scenes/" + sceneID + "/lightmap/LightmapFar-" + i;
                LightmapData lmd = new LightmapData();
                Asset asset = null;
                if (loadFromAssetBund == false)
                    asset = AssetLibrary.Load(farPath, AssetType.Texture2D, LoadType.Type_Resources);
                else
                    asset = AssetLibrary.Load(farPath, AssetType.Texture2D, LoadType.Type_AssetBundle);
                if (asset.texture2D != null)
                {
                    if (lightmapCorrectOn == true)
                    {
                        LightmapCorrection.mapsCount++;
                        lmd.lightmapFar = LightmapCorrection.Bake(asset.texture2D);
                        AssetLibrary.RemoveAsset(farPath);
                    }
                    else
                    {
                        lmd.lightmapFar = asset.texture2D;
                    }
                    lmds.Add(lmd);
                }
                br.ReadString();
            }
            LightmapSettings.lightmapsMode = LightmapsMode.Single;
            // 烘焙图
            if (lightmapCorrectOn == true)
                LightmapCorrection.Clear();
        }

        LightmapData[] lms = lmds.ToArray();
        LightmapSettings.lightmaps = lms;
        **/
        #endregion

        for (i = 0; i < lightmapCount; i++)
        {
            br.ReadString();
        }

        // 编辑器模式下如果存在烘焙贴图则禁止实时光的作用
        //if (count > 0 && isPlaying == false)
        //    sunLight.enabled = false;

        #region 读取高度数据

        // 读取高度数据
        if (br.BaseStream.Position < br.BaseStream.Length)
        {
            pos = br.BaseStream.Position;
            if (br.ReadInt32() == 20011)
            {
                // 存储高度数据
                int w = br.ReadInt32();
                int h = br.ReadInt32();
                if (heights == null)
                    heights = new short[w, h];
                float val = 0;
                for (i = 0; i < w; i++)
                {
                    for (j = 0; j < h; j++)
                    {
                        // 读取原始格子数据
                        val = br.ReadSingle();
                        heights[i, j] = ByteUtils.floatConverShort(val);
                    }
                }
            }
            else
            {
                br.BaseStream.Position = pos;
            }
        }

        // 读取高度数据
        if (br.BaseStream.Position < br.BaseStream.Length)
        {
            pos = br.BaseStream.Position;
            if (br.ReadInt32() == 20012)
            {
                // 存储高度数据
                int w = br.ReadInt32();
                int h = br.ReadInt32();
                if (heights == null)
                    heights = new short[w, h];
                for (i = 0; i < w; i++)
                {
                    for (j = 0; j < h; j++)
                    {
                        // 读取原始格子数据
                        heights[i, j] = br.ReadInt16();
                    }
                }
            }
            else
            {
                br.BaseStream.Position = pos;
            }
        }

        #endregion

        #region 读取TerrainData

#if UNITY_EDITOR
        if (!isPlaying)
        {
            terrainData = new TerrainData(_terrainConfig.sceneHeightmapResolution, _terrainConfig.sceneHeightmapResolution, 4, _terrainConfig.maxTerrainHeight, _terrainConfig.defaultTerrainHeight);
        }
#endif
        if (br.BaseStream.Position < br.BaseStream.Length)
        {
            pos = br.BaseStream.Position;
            if (br.ReadInt32() == 20005)
            {
                // 读取原始格子数据 ------------------------------------------------------
                int w = br.ReadInt32();
                int h = br.ReadInt32();

                if (isPlaying)
                {
                    br.BaseStream.Position += (w * h * 4);
                }
                else
                {
                    float val = 0;
                    for (i = 0; i < w; i++)
                    {
                        for (j = 0; j < h; j++)
                        {
                            // 读取原始格子数据
                            val = br.ReadSingle();
#if UNITY_EDITOR
                            terrainData.heightmap[i, j] = val;
#endif
                        }
                    }
                }
            }
            else
            {
                br.BaseStream.Position = pos;
                ReadTBEditorData();
            }
        }
        else
        {
            ReadTBEditorData();
        }

        //只在编译器中才会有terrainData
        // 游戏运行时不需要地形数据,地形已生成模型
        //if (isPlaying)
        //{
        //    terrainData.Release();
        //    terrainData = null;
        //}

        // -------------------------------------------------------------------------------------
        //  读取细节数据
        if (br.BaseStream.Position < br.BaseStream.Length)
        {
            pos = br.BaseStream.Position;
            if (br.ReadInt32() == 2016)
            {
                int detalDataLen = br.ReadInt32();
                byte[] detailData = br.ReadBytes(detalDataLen);
#if UNITY_EDITOR
                if (!isPlaying)
                {
                    terrainData.detailDatabase.Read(detailData);
                }
#endif
            }
            else
            {
                br.BaseStream.Position = pos;
            }
        }

        #endregion

        #region 读取行走区域数据

        // 从存储文件中读取路径
        if (br.BaseStream.Position < br.BaseStream.Length)
        {
            pos = br.BaseStream.Position;
            if (br.ReadInt32() == 10005)
            {
                // 读取原始格子数据 ------------------------------------------------------
                int w = br.ReadInt32();
                int h = br.ReadInt32();

                _mapPath = new MapPath(_terrainConfig.gridSize, w, h);
                for (i = 0; i < w; i++)
                {
                    for (j = 0; j < h; j++)
                    {
                        _mapPath.grids[i, j] = (byte)br.ReadInt32();
                    }
                }
            }
            else
            {
                br.BaseStream.Position = pos;
            }
        }
        // 从存储文件中读取路径
        if (br.BaseStream.Position < br.BaseStream.Length)
        {
            pos = br.BaseStream.Position;
            if (br.ReadInt32() == 10006)
            {
                // 读取原始格子数据 ------------------------------------------------------
                int w = br.ReadInt32();
                int h = br.ReadInt32();

                _mapPath = new MapPath(_terrainConfig.gridSize, w, h);
                for (i = 0; i < w; i++)
                {
                    for (j = 0; j < h; j++)
                    {
                        _mapPath.grids[i, j] = br.ReadByte();
                    }
                }
            }
            else
            {
                br.BaseStream.Position = pos;
            }
        }

        // 读取地图行走数据
        if (br.BaseStream.Position < br.BaseStream.Length)
        {
            pos = br.BaseStream.Position;
            if (br.ReadInt32() == 55555)
            {
                LogSystem.LogWarning("Scene data not support");
            }
            else
            {
                br.BaseStream.Position = pos;
            }
        }
        #endregion

        br.Close();
        me.Flush();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        //System.GC.WaitForPendingFinalizers();

        #region 加载Root && 雾效

#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObjectUnit.ThridPardLoad("Scenes/" + sceneID + "/Prefabs/SnailFog", OnFogLoadComplete, null);
        }
#endif

        GameObjectUnit.ThridPardLoad("Scenes/" + sceneID + "/Prefabs/Root", OnRootLoadComplete, null);

        GameObjectUnit.ThridPardLoad("Scenes/" + sceneID + "/Prefabs/ShadowMap", OnShadowMapComplete, null);
#if SNAILGRASS
		GameObjectUnit.ThridPardLoad("Scenes/" + sceneID + "/GrassData/GrassManagerData", OnGrassDataComplete, null);
#endif
        #endregion

        #region 角色光照
        if (oRoot != null)
        {
            Transform dmLight = oRoot.transform.Find("DM_Light/directionLight");
            if (dmLight != null)
            {
                dirLight = dmLight.GetComponent<Light>();
                if(dirLight != null)
                    dirLight.shadows = LightShadows.Hard;
            }
            else
            {
                LogSystem.LogWarning("dirLight is null");
            }

            Transform cLight = oRoot.transform.Find("DM_Light/customLight");
            if (cLight != null)
            {
                customLight = cLight.GetComponent<DirectionalLightForCharacter>();
            }
            else
            {
                LogSystem.LogWarning("customLight is null");
            }

        }
        if (customLight == null || dirLight == null)
        {
            LogSystem.LogWarning("Light missed");
        }
        #endregion


        // 将当前场景置为主场景
        mainScene = this;
        readCompalte = true;

        if (sceneByteReadComplete != null)
            sceneByteReadComplete(this);
        // 缓存面片数大的物件, 一开始运行场景时缓存，避免动态加载时的性能消耗
        return this;
    }

#if SNAILGRASS
	/// <summary>
	/// 草资源加载完成
	/// </summary>
	/// <param name="oAsset"></param>
	/// <param name="strfileName"></param>
	/// <param name="varStore"></param>
	private static void OnGrassDataComplete(UnityEngine.Object oAsset, string strfileName, VarStore varStore)
	{
		if (oAsset == null)
			return;

		CacheObjects.PopCache(oAsset);

		GrassData grassData = oAsset as GrassData;
		if (grassData == null)
		{
			LogSystem.LogWarning("OnGrassDataComplete::找不到程序草!!");
			return;
		}
		GameObject grassMgrIns = new GameObject("GrassManager", typeof(GrassManager));
		mGrassManager = grassMgrIns.GetComponent<GrassManager>();
		mGrassManager.Init(grassData, new MLGrassTerrain());
	}
#endif

    

    private void ReadTBEditorData()
    {
        if (Application.isPlaying)
        {
            return;
        }
#if UNITY_EDITOR
        //读取编辑数据
        UnityEngine.Object oAsset = AssetDatabase.LoadAssetAtPath("Assets/Res/Scenes/" + sceneID + "/TerrainData.bytes", typeof(UnityEngine.Object));
        if (oAsset != null)
        {
            TextAsset textAsset = oAsset as TextAsset;
            if (textAsset != null && textAsset.bytes != null)
            {
                if (terrainData == null)
                {
                    terrainData = new TerrainData(_terrainConfig.sceneWidth, _terrainConfig.sceneHeight, 4, _terrainConfig.maxTerrainHeight, _terrainConfig.defaultTerrainHeight);
                }
                MemoryStream tbme = new MemoryStream(textAsset.bytes);
                BinaryReader tbbr = new BinaryReader(tbme);
                long pos = tbbr.BaseStream.Position;
                if (tbbr.ReadInt32() == 20006)
                {
                    // 读取原始格子数据 ------------------------------------------------------
                    int w = tbbr.ReadInt32();
                    int h = tbbr.ReadInt32();

                    for (int i = 0; i < w; i++)
                    {
                        for (int j = 0; j < h; j++)
                        {
                            if (!Application.isPlaying)
                                // 读取原始格子数据
                                terrainData.heightmap[i, j] = tbbr.ReadSingle();
                            else
                                tbbr.ReadSingle();
                        }
                    }
                }
                else
                {
                    tbbr.BaseStream.Position = pos;
                }
                tbbr.Close();
                tbme.Close();
            }
        }
#endif
    }

    #region root加载 && 雾加载完成

#if UNITY_EDITOR

    private void OnFogLoadComplete(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        if (oAsset == null)
            return;

        //场景对象加载不缓存
        CacheObjects.PopCache(oAsset);

        if (destroyed)
            return;

        GameObject oFog = GameObject.Instantiate(oAsset) as GameObject;
        if (oFog == null)
        {
            LogSystem.LogWarning("oFog is null!!");
            return;
        }
        mSnailFogPro = oFog.GetComponent<SnailFogPro>();
    }

#endif

    /// <summary>
    /// root加载
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="strFileName"></param>
    /// <param name="varStore"></param>
    private void OnRootLoadComplete(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        if (oAsset == null)
            return;

        //场景对象加载不缓存
        //CacheObjects.PopCache(oAsset);

        if (destroyed)
            return;

        oRoot = CacheObjects.InstantiatePool(oAsset) as GameObject;
        oRoot.transform.position = (oAsset as GameObject).transform.position;
    }

    /// <summary>
    /// 天空盒加载
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="strFileName"></param>
    /// <param name="varStore"></param>
    private void OnSkyBoxLoad(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        if (oAsset == null)
            return;

        //天空盒加载不缓存
        CacheObjects.PopCache(oAsset);
        if (destroyed)
            return;

        Material materSkyBox = oAsset as Material;
        RenderSettings.skybox = materSkyBox;
    }

    /// <summary>
    /// 环境贴图加载完成
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="strFileName"></param>
    /// <param name="varStore"></param>
    private void OnShadowMapComplete(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        if (oAsset == null)
            return;

        //场景对象加载不缓存
        CacheObjects.PopCache(oAsset);

        if (destroyed)
            return;

        ShadowMapData shadowMapData = oAsset as ShadowMapData;
        if (shadowMapData == null)
        {
            LogSystem.LogWarning("OnShadowMapComplete::找不到环境映射资源!!", shadowMapData);
            return;
        }

        mSMData = shadowMapData;

        if (mSMData.shadowMap != null)
        {
            Shader.SetGlobalTexture("_ShadowMapTex", mSMData.shadowMap);
            Shader.SetGlobalFloat("_ShadowMapPosX", mSMData.m_Pos.x);
            Shader.SetGlobalFloat("_ShadowMapPosZ", mSMData.m_Pos.z);
            Shader.SetGlobalInt("_ShadowMapSize", mSMData.m_Size);
            Shader.SetGlobalFloat("_ShadowMapAdd", mSMData.m_AddLight);
        }

    }

    /// <summary>
    /// 获取环境映射贴图
    /// </summary>
    /// <returns></returns>
    public ShadowMapData GetEnvironmentMapData()
    {
        if (mSMData != null)
        {
            return mSMData;
        }
        else
        {
            return null;
        }
    }

    #endregion

    /***********************************************************************************
     * 功能 : 请求寻路
     ***********************************************************************************/
    public void RequestPaths(Vector3 startPoint, Vector3 endPoint, int collisionSize, out List<Vector3> paths)
    {
        if (DelegateProxy.bIsWayInitOk())
        {
            paths = new List<Vector3>();
            LogSystem.LogWarning("寻路调用了这个代码，修改一下，修改一下");
        }
        else
        {
            if (mapPath != null)
            {
                mapPath.RequestPaths(startPoint, endPoint, collisionSize, out paths);
            }
            else
                paths = new List<Vector3>();
        }
    }

    /// <summary>
    /// 判断是否字节流读取完成
    /// </summary>
    public bool readCompalte = false;

    /***********************************************************************************
     * 功能 : 设置着色器常量值
     ***********************************************************************************/
    public void UpdateShaderConstant(Vector3 eyePos)
    {
        /**
        if (lightmapCorrectOn != _oldLightmapCorrectOn)
        {
            if (lightmapCorrectOn == true)
            {
                Shader.EnableKeyword("LightmapCorrectOn");
                Shader.DisableKeyword("LightmapCorrectOff");
            }
            else
            {
                Shader.DisableKeyword("LightmapCorrectOn");
                Shader.EnableKeyword("LightmapCorrectOff");
            }
            _oldLightmapCorrectOn = lightmapCorrectOn;
        }
        // 设置冷暖色调
        Shader.SetGlobalVector("coolColor", _terrainConfig.coolColor);
        Shader.SetGlobalVector("warmColor", _terrainConfig.warmColor);


        // 设置人物点光源位置()
        worldSpaceLightPosition = _terrainConfig.rolePointLightPostion + eyePos;// 避免CG没有主角，调用视图中心点
        worldSpaceLightPosition.w = 1;

        Shader.SetGlobalVector("_worldSpaceLightPosition", worldSpaceLightPosition);
        if (Camera.main != null)
            Shader.SetGlobalVector("_worldSpaceViewPos", Camera.main.transform.position);
        Shader.SetGlobalVector("_lightColor", _terrainConfig.rolePointLightColor);
        Shader.SetGlobalFloat("_lightRange", _terrainConfig.rolePointLightRange);
        Shader.SetGlobalFloat("_lightIntensity", _terrainConfig.rolePointLightIntensity * 100);
        **/

#if UNITY_EDITOR
        if (sunLight != null)
        {
            Shader.SetGlobalVector("_WorldSpaceSceneLightForCharacter", -sunLight.transform.forward);
            Shader.SetGlobalColor("_WorldSpaceSceneLightColorForCharacter", sunLight.color * sunLight.intensity);
        }
#endif



    }

    /// <summary>
    /// 加载lightmap
    /// </summary>
    public void LoadLightmap()
    {
        if (mSceneLightMap == null)
            mSceneLightMap = SceneLightMap.GetSceneLightMap();

        string strScene = sceneID.ToString();
        int lightCategory = DelegateProxy.GetLightMapCount(strScene);
        mSceneLightMap.SetLightmapInfo(sceneID.ToString(), lightCategory, lightmapCount);
        mSceneLightMap.DisplayLightmap(0);
    }

    /// <summary>
    /// 设置显示lightmap
    /// </summary>
    /// <param name="index"></param>
    public void SetDisplayLightmap(int index)
    {
        if (mSceneLightMap != null)
            mSceneLightMap.DisplayLightmap(index);
    }

    /** 视野方向 */
    public Vector3 viewDir = Vector3.zero;
    private int loadComplateWaitTick = 0;
    private int WaitLoadSceneTick = 0;

    /******************************************************************************
     * 功能： 可视列表更新
     ******************************************************************************/
    public void UpdateView(Vector3 eyePos)
    {
        if (destroyed == true)
            return;

        // 场景文件未读取前不更新
        if (isPlaying == true && readCompalte == false)
            return;

        this.eyePos = eyePos;
        // 计算主角与灯光的距离计算衰减
        if (mainCamera == null)
        {
            mainCamera = Camera.main;

            if (mainCamera.gameObject.GetComponent<FlareLayer>() == null)
                mainCamera.gameObject.AddComponent<FlareLayer>();

            if (!DelegateProxy.bWuFengMapIng())
            {
                mainCamera.backgroundColor = RenderSettings.fogColor;
                if (mainCameraCullingMask < 0)
                    mainCameraCullingMask = mainCamera.cullingMask;
            }
        }

        if (mainCamera == null)
            return;

        if (PEBase.useDepthTick < 1)
        {
            if (mainCamera != null)
                mainCamera.depthTextureMode = DepthTextureMode.None;
        }

#if UNITY_EDITOR
        if (loadSceneComplate == false)
            Application.targetFrameRate = 50;
        else
        {
            Application.targetFrameRate = targetFrameRate;
        }
#endif

        // 实例化初始视野对象 ------------------------------------------------------------------------
        if (loadSceneComplate == false)
        {
            WaitLoadSceneTick++;
            if (WaitLoadSceneTick < 30)
            {
                if (WaitLoadSceneTick == 20)
                {
                    Resources.UnloadUnusedAssets();
                    // 加载完场景后对垃圾再进行一次回收
                    System.GC.Collect();
                }
                return;
            }
            if (isPlaying && !DelegateProxy.bWuFengMapIng())
            {
                mainCamera.cullingMask = 0;
            }

            if (sceneLoadUnitTick >= sumSceneLoadUnitTick)
            {
                loadComplateWaitTick++;
                if (!DelegateProxy.bWuFengMapIng())
                {
                    mainCamera.cullingMask = mainCameraCullingMask;
                }

                if (loadComplateWaitTick > 10)
                {
                    loadSceneComplate = true;

                    //Debug.Log("加载场景" + sceneID + "完毕, 本次总共加载静态单位数量为:" + sceneLoadUnitTick);
                    try
                    {
                        loadProgress = 1f;
                        if (sceneLoadCompleListener != null)
                        {
                            sceneLoadCompleListener.Invoke();
                            //修复CG播放完成后 Loading条不归零
                            loadProgress = 0f;
                        }

                    }
                    catch (Exception e)
                    {
                        LogSystem.LogError("场景加载完毕回调错误! 错误信息: " + e.ToString());
                    }
                }
            }
            else
            {
                loadProgress = (float)sceneLoadUnitTick / (float)sumSceneLoadUnitTick;
            }
        }

        if (isPlaying == true)
            time += 0.002f;

        eyePos.y = this.SampleHeight(eyePos);

        // 计算摄像机到角色向量
        viewDir = eyePos - mainCamera.transform.position;
        viewDir.Normalize();

        // 更新渲染常量值
        UpdateShaderConstant(eyePos);

        // 游戏运行状态下进行场景帧频管理
        if (isPlaying == true)
        {
            frames++;
            var timeNow = Time.time;
            if (timeNow > lastInterval + updateInterval)
            {
                fps = frames / (timeNow - lastInterval);
                ms = 1000.0f / Mathf.Max(fps, 0.00001f);
                frames = 0;
                lastInterval = timeNow;
                // 平滑fps
                // fps = Mathf.Lerp(oldfps, fps, 0.2f);
                //oldfps = fps;

                if (fps < 5f)
                    fps = 5f;
            }
        }
        else
        {
            this.fps = 30;
        }



        if (tick == 0)
        {
            LoadLightmap();

            lastInterval = Time.time;

            //FirstRun();

            if (_terrainConfig != null)
            {
                //避免重复加载
                int iTemp = SystemSetting.ImageQuality == GameQuality.HIGH || SystemSetting.ImageQuality == GameQuality.MIDDLE ? 2 : 0;
                if (iTemp != _terrainConfig.bumpCount)
                {
                    _terrainConfig.bumpCount = iTemp;
                    Region.SetCurrentBumpCount(-1);
                }
                else
                {
                    Region.SetCurrentBumpCount(iTemp);
                }
            }

            UpdateRegions();

            if (isPlaying == true)
                dontCullUnit = false;
        }

        if (mapPath != null)
            mapPath.Update();

        // 理论上有游戏高低中配置读取
        postEffectEnable = true;
        if (tick % 10 == 0)
        {
            if (isPlaying == true)
            {
                // 加载后期效果
                if (peConfigLoaded == false)
                {
                    LoadPostEffectConfig();
                    SetEffectsLevel(SystemSetting.ImageQuality);
                }
            }
            else
            {
                CheckPostEffectComponentAppend();

                // 加载后期效果
                if (peConfigLoaded == false)
                    LoadPostEffectConfig();
            }

            if (PEBase.useDepthTick < 1)
            {
                if (mainCamera != null)
                    mainCamera.depthTextureMode = DepthTextureMode.None;
            }
        }

        UpdateTiles();
        UpdateBake();
        UpdateUnits();

        if (isPlaying == true)
        {
            if (tick % 5 == 0)
            {
                // if (Vector3.SqrMagnitude(eyePos - lastPos) > (6f * 6f))
                {
                    // 更新区域
                    UpdateRegions();
                }
            }
        }
        // 编辑状态时进行频繁更新,方便加载地形
        else
        {
            UpdateRegions();
        }

        // 更新地形烘焙,减少GPU消耗
        /**
        if (isPlaying == true)
        {
            if (loadSceneComplate == true)
            {
                if (tick % 10 == 0)
                {
                    for (int i = 0; i < tiles.Count; i++)
                    {
                        if (tiles[i].terrain != null && tiles[i].terrain.terrainRenderer.enabled == true)
                        {
                            if (tiles[i].terrain.terrainMapIndex < 0)
                            {
                                if (tiles[i].viewDistance < 1089f)
                                {
                                    tiles[i].terrain.Bake();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        **/

#if UNITY_EDITOR
        //if (detailRenderer == null && terrainData != null)
        //    detailRenderer = new DetailRenderer(terrainData, Vector3.zero, 0);
        //if (detailRenderer != null)
        //    detailRenderer.Render(1000f, 1f);

#endif
        tick++;
    }

    public void ResetLoad()
    {
        sceneLoadCompleListener = null;
        loadProgress = 0f;
        loadComplateWaitTick = 0;
        loadSceneComplate = false;
        tick = 0;
        sceneLoadUnitTick = 0;
        sumSceneLoadUnitTick = 0;
    }

    /**********************************************************************************************************
     * 功能 : 更新区域
     **********************************************************************************************************/
    private void UpdateRegions()
    {
        int i = 0;
        int j = 0;
        // 更新可视的Region
        float absX = Mathf.Abs(eyePos.x);
        float absY = Mathf.Abs(eyePos.z);
        float sigX = absX / eyePos.x;
        float sigY = absY / eyePos.z;

        if (eyePos.x == 0)
            curRegionX = 0;
        else
            curRegionX = (int)(Mathf.Ceil(absX / _terrainConfig.regionSize) * sigX);

        if (eyePos.z == 0)
            curRegionY = 0;
        else
            curRegionY = (int)(Mathf.Ceil(absY / _terrainConfig.regionSize) * sigY);

        int startRegX = curRegionX - viewRect;
        int endRegX = curRegionX + viewRect;
        int startRegY = curRegionY - viewRect;
        int endRegY = curRegionY + viewRect;

        startRegX = -1;
        startRegY = -1;
        endRegX = 1;
        endRegY = 1;

        // 重置可视区域
        if (dontCullUnit == false)
            regions.Clear();

        bool hasRegion = false;
        for (i = startRegX; i <= endRegX; i++)
        {
            for (j = startRegY; j <= endRegY; j++)
            {
                //if (!bUpdateRegions)
                //    continue;

                Region region = null;
                int iKey = Region.GetRegionKey(i, j);
                hasRegion = regionsMap.ContainsKey(iKey);
                if (hasRegion == true)
                    region = regionsMap[iKey];

                if (region == null)
                {
                    string regPath = "";

                    // 获取区域资源
                    Asset asset = null;
                    if (isPlaying == false)
                    {
                        regPath = "Scenes/" + sceneID + "/" + i + "_" + j + "/Region";
                        asset = AssetLibrary.Load(regPath, AssetType.Region, LoadType.Type_Resources);
                    }
                    else
                    {
                        regPath = DelegateProxy.StringBuilder("Scenes/", sceneID, "/", i, "_", j, "/Region");
                        //regPath = "Scenes/" + sceneID + "/" + i + "_" + j + "/Region";
                        if (this.loadFromAssetBund == true)
                            asset = AssetLibrary.Load(regPath, AssetType.Region, LoadType.Type_AssetBundle);
                        else
                            asset = AssetLibrary.Load(regPath, AssetType.Region, LoadType.Type_Resources);
                    }
                    // 编辑器编辑的时候已经存在地形数据
                    if (asset.loaded == true)
                    {
                        region = asset.region;
                        regions.Add(region);
                        regionsMap.Add(region.miKey, region);

                    }
                    // 本地存储不包含地域数据时候, 动态创建新的区域
                    else
                    {
                        // 游戏运行时不创建新地域,避免性能消耗
#if UNITY_EDITOR
                        if (isPlaying == false)
                        {
                            region = Region.Create(this, i, j);
                            regions.Add(region);
                            regionsMap.Add(region.miKey, region);
                        }
#endif
                    }
                }
                else
                {
                    if (dontCullUnit == true)
                    {
                        if (hasRegion == false)
                            regions.Add(region);
                    }
                    else
                        regions.Add(region);
                }
            }
        }

        // 更新
        if (dontCullUnit == false)
            regionsMap.Clear();

        for (i = 0; i < regions.Count; i++)
        {
            int iKey = regions[i].miKey;
            if (regionsMap.ContainsKey(iKey) == false)
                regionsMap.Add(iKey, regions[i]);

            regions[i].Update(eyePos);
        }

        Region.SetBumpCount(SceneBumpCount);
    }

    private void UpdateBake()
    {
        //TestListLog();
        if (mBakeTerrains.Count > 0)
        {
            LODTerrain terrain = mBakeTerrains[0];
            mBakeTerrains.RemoveAt(0);
            if (terrain != null)
            {
                terrain.Bake();
            }
        }
    }

    private void TestListLog()
    {
        if (mBakeTerrains.Count == 0)
            return;

        Debug.LogError("====="+mBakeTerrains.Count+"=======");
        for (int i = 0; i < mBakeTerrains.Count; i++)
        {
            Debug.LogError(i+" "+mBakeTerrains[i].bCenterTerrain);
        }
        Debug.LogError("=====" + "end" + "=======");
    }


    /********************************************************************************
     * 功能 : 场景销毁已载入的切片
     ********************************************************************************/
    private int visibleTilePerFrame = 0;
    private int visTileCount = 0;
    private byte flag = 0;
    private void UpdateTiles()
    {
        int count = tiles.Count;
        if (tick % 1 == 0)
            visTileCount = 0;

#if NOLOD
        if (isEditor == true && isPlaying == false)
            visibleTilePerFrame = 9999;
#endif

        Tile centerTile = GetTileAt(eyePos);
        if (mtCurCenterTile != centerTile)
        {
            if (mtCurCenterTile != null && mtCurCenterTile.terrain != null)
            {
                mtCurCenterTile.terrain.bCenterTerrain = false;
            }
        }

        mtCurCenterTile = centerTile;

        if (mtCurCenterTile != null)
        {
            if (mtCurCenterTile.terrain != null)
            {
                mtCurCenterTile.terrain.bCenterTerrain = true;
            }
            mtCurCenterTile.Update(eyePos);
        }

        flag = 0;
        for (int i = 0; i < count; i++)
        {
#if UNITY_EDITOR
            //显示所有tile(制作LOG)
            if (isPlaying == false)
            {
                if (tiles[i].visible == false)
                    tiles[i].Visible();
                tiles[i].Update(eyePos);
                continue;
            }
#endif

#if NOLOD
            if (tiles[i].visible == false)
                tiles[i].Visible();
            tiles[i].Update(eyePos);
            continue;
#endif
            Tile curTile = tiles[i];

            if (tiles[i] != null && tiles[i].terrain != null)
            {
                tiles[i].terrain.lodLevel = 1;
            }

            dx = eyePos.x - tiles[i].position.x;
            dz = eyePos.z - tiles[i].position.z;

            tiles[i].viewDistance = dx * dx + dz * dz;  //Vector3.Distance(eyePos, tiles[i].position);
            
            if (tiles[i].viewDistance > (tiles[i].far + 8f) * (tiles[i].far + 8f))
            {
                //tiles[i].Invisible();
            }
            else
            {
                tiles[i].Update(eyePos);
            }


            if (mtCurCenterTile != null)
            {
                if (mtCurCenterTile != null && mtCurCenterTile == curTile)
                {
                    if (curTile.terrain != null && curTile.terrain.gameObject != null)
                    {
                        flag++;
                    }
                    curTile.Visible();
                    BakeTerrain(curTile);
                }
                else if (mtCurCenterTile.left != null && mtCurCenterTile.left == curTile)
                {
                    if (curTile.terrain != null && curTile.terrain.gameObject != null)
                    {
                        flag++;
                    }
                    curTile.Visible();
                    BakeTerrain(curTile);
                }
                else if (mtCurCenterTile.right != null && mtCurCenterTile.right == curTile)
                {
                    if (curTile.terrain != null && curTile.terrain.gameObject != null)
                    {
                        flag++;
                    }
                    curTile.Visible();
                    BakeTerrain(curTile);

                }
                else if (mtCurCenterTile.top != null && mtCurCenterTile.top == curTile)
                {
                    if (curTile.terrain != null && curTile.terrain.gameObject != null)
                    {
                        flag++;
                    }
                    curTile.Visible();
                    BakeTerrain(curTile);

                }
                else if (mtCurCenterTile.bot != null && mtCurCenterTile.bot == curTile)
                {
                    if (curTile.terrain != null && curTile.terrain.gameObject != null)
                    {
                        flag++;
                    }
                    curTile.Visible();
                    BakeTerrain(curTile);

                }
                else if (mtCurCenterTile.top_left != null && mtCurCenterTile.top_left == curTile)
                {
                    if (curTile.terrain != null && curTile.terrain.gameObject != null)
                    {
                        flag++;
                    }
                    curTile.Visible();
                    BakeTerrain(curTile);
                }
                else if (mtCurCenterTile.top_right != null && mtCurCenterTile.top_right == curTile)
                {
                    if (curTile.terrain != null && curTile.terrain.gameObject != null)
                    {
                        flag++;
                    }
                    curTile.Visible();
                    BakeTerrain(curTile);
                }
                else if (mtCurCenterTile.bot_left != null && mtCurCenterTile.bot_left == curTile)
                {
                    if (curTile.terrain != null && curTile.terrain.gameObject != null)
                    {
                        flag++;
                    }
                    curTile.Visible();
                    BakeTerrain(curTile);
                }
                else if (mtCurCenterTile.bot_right != null && mtCurCenterTile.bot_right == curTile)
                {
                    if (curTile.terrain != null && curTile.terrain.gameObject != null)
                    {
                        flag++;
                    }
                    curTile.Visible();
                    BakeTerrain(curTile);
                }
                else
                {
#if NOLOD

#else
                    //当前tile不在相邻的九宫格中隐藏(这时是否应该去除物件?)
					curTile.Invisible();
// 					if (curTile.terrain != null)
// 						curTile.terrain.CancelBake();
#endif

                }
            }

        }

        if (mtCurCenterTile != null)
        {
            _TileCenterPos.x = mtCurCenterTile.position.x;
            _TileCenterPos.z = mtCurCenterTile.position.z;
        }

        if(flag == 9 && tick - visibleTilePerFrame > 5)
        {
            _TileCenterPos.w = 1.4f * (float)_terrainConfig.tileSize;
        }
        else
        {
            if(flag != 9)
                visibleTilePerFrame = tick;
            _TileCenterPos.w = (float)_terrainConfig.tileSize;
        }
            
        Shader.SetGlobalVector("_TileCenterPos", _TileCenterPos);
    }

    private void BakeTerrain(Tile tile)
    {
        if (tile == null || tile.terrain == null)
            return;

        LODTerrain terrain = tile.terrain;
        if (terrainConfig.bumpCount < 1 && terrain.terrainRenderer.enabled == true)
        {
            if (terrain.bBake)
            {
                if (terrain.bImmediatelyBake)
                {
                    terrain.Bake();
                }
                else
                {
                    if (!mBakeTerrains.Contains(terrain))
                    {
                        mBakeTerrains.Add(terrain);
                    }

                }
            }
            return;
        }

        //取消烘焙
        if (mBakeTerrains.Contains(terrain))
        {
            mBakeTerrains.Remove(terrain);
        }
        else
        {
            terrain.CancelBake();
        }
    }


    private Vector4 _TileCenterPos;

#if UNITY_EDITOR
    public int visibleDynamicUnitCount = 0;
    public int visibleStaticUnitCount = 0;
    public int visibleStaticTypeCount = 0;
#endif

    private int visibleDynaUnitPerFrame = 0;

    private int visibleStaticUnitPerFrame = 0;
    private int hideStaticUnitPerFrame = 0;

    static public int maxDynaUnit = 50;

    public List<GameObjectUnit> dynamicUnits = new List<GameObjectUnit>();

    /**********************************************************************************
     * 功能 ：更新单位, 当单位超出其显示范围时将隐藏
     **********************************************************************************/
    private float dx = 0;
    private float dz = 0;
    private int maxVisibleStaticUnitPerFrame = 1;

    public void UpdateUnits()
    {

        int count = units.Count;
        int i = 0;
        int j = 0;
        GameObjectUnit unit = null;

#if UNITY_EDITOR
        visibleDynamicUnitCount = 0;
        visibleStaticUnitCount = 0;

        if (isEditor == true && isPlaying == false)
            maxVisibleStaticUnitPerFrame = 9999999;
#endif

        // 动态单位延时创建
        if (tick % 3 == 0)
            visibleDynaUnitPerFrame = 0;

        visibleStaticUnitPerFrame = 0;
        hideStaticUnitPerFrame = 0;

        // 动态单位列表
        dynamicUnits.Clear();

        GameObjectUnit viewDynUnit = null;
        for (i = 0; i < count; i++)
        {
            unit = units[i];
            // 编辑器中更新单位方便编辑
            if (isPlaying == false)
            {
                if (unit.isStatic == true)
                    unit.Update();
            }

            if (unit.isStatic == true
                && (hideStaticUnitPerFrame < 1 || visibleStaticUnitPerFrame < maxVisibleStaticUnitPerFrame))
            {
                // 计算视距半径距离 (当更新点位置发生一定平移)
                dx = unit.center.x - eyePos.x;
                dz = unit.center.z - eyePos.z;
                unit.viewDistance = dx * dx + dz * dz;                 // 加上剔除因子倍数

                //处理超出视距
                if (unit.visible == true && hideStaticUnitPerFrame < 1 && unit.viewDistance > (unit.far * unit.far))           // 静态单位缓和剔除
                {
                    //认为超出视距可以隐藏
                    if (dontCullUnit == false)
                    {
                        RemoveUnit(unit);
                        count--;
                        i--;
                        hideStaticUnitPerFrame++;
                    }
                    unit.active = false;
                }

                //处理进入视距
                if (unit.visible == false && visibleStaticUnitPerFrame < maxVisibleStaticUnitPerFrame
                                && unit.viewDistance < (unit.near * unit.near)
                                && (unit.combineParentUnitID < 0 || GameScene.SampleMode))
                {           // 静态单位缓和剔除
                    unit.active = true;
                    if (isEditor == true && isPlaying == false)
                    {
                        unit.Visible();
                        visibleStaticUnitPerFrame++;
                    }
                    else {
                        // 计算与视野夹角判断是否在背面 
                        Vector3 viewToUnit = unit.center - mainCamera.transform.position;
                        viewToUnit.Normalize();

                        // 计算屏幕上方的摄像机向量
                        unit.viewAngle = Mathf.Acos(Vector3.Dot(viewToUnit, viewDir)) / (unit.cullingFactor);

                        if (unit.viewAngle < terrainConfig.cullingAngleFactor)
                        {
                            unit.Visible();
                            visibleStaticUnitPerFrame++;

                            if (loadSceneComplate == false)
                                sceneLoadUnitTick++;
                            // Debug.Log("载入场景单位 " + tick + " - " + sceneLoadUnitTick++);
                        }
                    }
#if UNITY_EDITOR
                    visibleStaticUnitCount++;
#endif
                }
            }

            // 动态单位的状态更新 =========================================================================================
            if (unit.isStatic == false)
            {

#if UNITY_EDITOR
                if (unit.visible == true)
                    visibleDynamicUnitCount++;
#endif

                // 计算视距半径距离
                dx = unit.Position.x - eyePos.x;
                dz = unit.Position.z - eyePos.z;
                unit.viewDistance = dx * dx + dz * dz;

                // 避免动态单位载入时场景没有加载, 需要重新采样高度
                if (unit.Position.y > 99999999f)
                    unit.SetPostionY(this.SampleHeight(unit.Position));

                // 排序离视野中心最近的隐藏动态单位
                if (viewDynUnit == null)
                {
                    if (unit.visible == false)
                        viewDynUnit = unit;
                }
                else
                {
                    if (unit.visible == false && (unit.viewDistance < viewDynUnit.viewDistance))
                        viewDynUnit = unit;
                }

                dynamicUnits.Add(unit);


                // 超出视野直接隐藏
                if (unit.viewDistance > (unit.far * unit.far))
                {
                    unit.Invisible();
                }

                // 格子阻塞数据更新 ==========================================================================================
                // 更新动态单位, 动态单位需要一直更新
                // 清理格子数据
                if (mapPath != null && unit.hasCollision == true)
                {
                    mapPath.SetDynamicCollision(unit.Position, unit.collisionSize, true);
                    unit.hasCollision = false;
                }

                // 更新动态单位, 动态单位需要一直更新
                unit.Update();

                // 重新计算指定范围的格子数据
                if (enableDynamicGrid == true)
                {
                    if (unit.viewDistance < (_terrainConfig.collisionComputeRange * _terrainConfig.collisionComputeRange))
                    {
                        if (mapPath != null && unit.isCollider == true && unit.grids == null)
                        {
                            mapPath.SetDynamicCollision(unit.Position, unit.collisionSize, false);
                            unit.hasCollision = true;
                        }
                    }
                }
            }
        }

        if (viewDynUnit != null)
        {
            if (viewDynUnit.viewDistance < viewDynUnit.near * viewDynUnit.near)
            {
                if (visibleDynaUnitPerFrame < 1)
                {
                    viewDynUnit.Visible();
                    visibleDynaUnitPerFrame++;
                }
            }
        }
    }

    public bool enableDynamicGrid = false;

    /***************************************************************************************
     * 功能 : 获取触碰的动态单位
     ****************************************************************************************/
    public GameObjectUnit GetTouchDynamicUnit(float touchRange = 700f)
    {
        int count = dynamicUnits.Count;
        float dx = 0f;
        float dy = 0f;
        float mx = Input.mousePosition.x;
        float my = Input.mousePosition.y;
        GameObjectUnit unit = null;
        for (int i = 0; i < count; i++)
        {
            unit = dynamicUnits[i];

            if (unit == null || unit.scene == null || unit.scene.mainCamera == null)
                continue;

            unit.screenPoint = unit.scene.mainCamera.WorldToScreenPoint(new Vector3(unit.Position.x, unit.Position.y + unit.scenePointBias, unit.Position.z));

            // 如果单位可见,并且支持鼠标事件
            if (unit.visible == true && unit.mouseEnable == true)
            {
                dx = mx - unit.screenPoint.x;
                dy = my - unit.screenPoint.y;
                if ((dx * dx + dy * dy) < touchRange)
                    return unit;
            }
        }
        return null;
    }

    /************************************************************************************
     * 功能 ：获取指定位置中的地形切片
     ************************************************************************************/
    public Tile GetTileAt(Vector3 worldPosition)
    {
        int regX = (int)Mathf.Floor((worldPosition.x + _terrainConfig.regionSize * 0.5f) / _terrainConfig.regionSize);
        int regY = (int)Mathf.Floor((worldPosition.z + _terrainConfig.regionSize * 0.5f) / _terrainConfig.regionSize);

        int iKey = Region.GetRegionKey(regX, regY);
        Region region = null;
        if (regionsMap.ContainsKey(iKey))
            region = regionsMap[iKey];

        if (region == null)
            return null;

        return region.GetTile(worldPosition);
    }

    /*************************************************************************************
     * 功能 ：获取相邻的切片
     *************************************************************************************/

    public Tile GetNeighborTile(Tile tile, int dirX, int dirY)
    {
        uint key = GetNeighborKey(tile.region.regionX, tile.region.regionY, tile.tileX, tile.tileY, dirX, dirY);

        if (tilesMap != null && tilesMap.ContainsKey(key))
            return tilesMap[key];
        else
            return null;
    }

    public uint GetNeighborKey(int regX, int regY, int tileX, int tileY, int dirX, int dirY)
    {
        int maxlen = (_terrainConfig.tileCountPerSide - 1) / 2;
        int neighborTileX = tileX + dirX;
        int neighborTileY = tileY + dirY;
        int neighborRegX = regX;
        int neighborRegY = regY;
        if (neighborTileX < -maxlen)
        {
            neighborTileX = maxlen;
            neighborRegX -= 1;
        }
        if (neighborTileX > maxlen)
        {
            neighborTileX = -maxlen;
            neighborRegX += 1;
        }
        if (neighborTileY < -maxlen)
        {
            neighborTileY = maxlen;
            neighborRegY -= 1;
        }
        if (neighborTileY > maxlen)
        {
            neighborTileY = -maxlen;
            neighborRegY += 1;
        }
        return GetKey(neighborRegX, neighborRegY, neighborTileX, neighborTileY);
    }
    public class Key
    {
        public int iRegX = 0;
        public int iRegY = 0;
        public int ilastX = 0;
        public int ilastY = 0;
        public string strKey = string.Empty;
        public Key(int iRegX, int iRegY, int ilastX, int ilastY)
        {
            this.iRegX = iRegX;
            this.iRegY = iRegY;
            this.ilastX = ilastX;
            this.ilastY = ilastY;
            if (GameScene.isPlaying)
            {
                this.strKey = DelegateProxy.StringBuilder(iRegX, "_", iRegY, "_", ilastX, "_", ilastY);
            }
            else
            {
                this.strKey = iRegX + "_" + iRegY + "_" + ilastX + "_" + ilastY;
            }

        }
        public bool Equals(int iRegX, int iRegY, int ilastX, int ilastY)
        {
            if (this.iRegX == iRegX && this.iRegY == iRegY && this.ilastX == ilastX && this.ilastY == ilastY)
            {
                return true;
            }
            return false;
        }

    }
    static List<Key> listKey = new List<Key>();
//     private static string GetKey(int iRegX, int iRegy, int ilastX, int ilastY)
//     {
//         int iCount = listKey.Count;
//         Key keyItem;
//         for (int i = 0; i < iCount; i++)
//         {
//             keyItem = listKey[i];
//             if (keyItem.Equals(iRegX, iRegy, ilastX, ilastY))
//             {
//                 return keyItem.strKey;
//             }
//         }
// 
//         Key newkey = new Key(iRegX, iRegy, ilastX, ilastY);
//         listKey.Add(newkey);
//         if (listKey.Count > 100)
//         {
//             listKey.RemoveAt(0);
//         }
//         return newkey.strKey;
//     }

    public static uint GetKey(int iRegX, int iRegy, int ilastX, int ilastY)
    {
        byte rx = (byte)iRegX;
        byte ry = (byte)iRegy;
        byte ix = (byte)ilastX;
        byte iY = (byte)ilastY;
        return (uint)(rx << 24 | ry << 16 | ix << 8 | iY);
    }
    /********************************************************************************************
     * 功能 ： 取样高度
     ********************************************************************************************/
    public float SampleHeight(Vector3 worldPosition, bool interpolation = true)
    {
        /**
        if (IsNewDynamicUnit)
        {
            return SampleMainHeight(worldPosition, interpolation);
        }
        **/

        // 如果场景保存了高度数据,则使用场景的高度采样
        if (terrainConfig != null && heights != null)
        {
            float scale = (float)terrainConfig.heightmapResolution / terrainConfig.tileSize;

            int resolution = (int)_terrainConfig.sceneHeightmapResolution;
            float x = worldPosition.x * scale + terrainConfig.sceneHeightmapResolution * 0.5f;
            float y = worldPosition.z * scale + terrainConfig.sceneHeightmapResolution * 0.5f;
            float pX = (x % 1f);
            float pY = (y % 1f);
            int intX1 = Mathf.FloorToInt(x);
            int intY1 = Mathf.FloorToInt(y);

            int intX2 = intX1 + 1;
            int intY2 = intY1 + 1;

            if (intX1 < 0 || intY1 < 0 || intX1 >= resolution || intY1 >= resolution)
                return _terrainConfig.defaultTerrainHeight;

            if (intX2 < 0 || intY2 < 0 || intX2 >= resolution || intY2 >= resolution)
                return _terrainConfig.defaultTerrainHeight;

            float r1 = ByteUtils.shortConverFloat(heights[intX1, intY1]) * (1 - pX) + ByteUtils.shortConverFloat(heights[intX2, intY1]) * pX;
            float r2 = ByteUtils.shortConverFloat(heights[intX1, intY2]) * (1 - pX) + ByteUtils.shortConverFloat(heights[intX2, intY2]) * pX;
            float r3 = r2 * pY + r1 * (1 - pY);
            return SampleDynamicHeight(worldPosition, r3);
        }
        return 999999999f;
    }

    /********************************************************************************************
     * 功能 ： 判断位置是否在水面上
     ********************************************************************************************/
    public bool Underwater(Vector3 postion)
    {
        int regX = (int)Mathf.Floor((postion.x + _terrainConfig.regionSize * 0.5f) / _terrainConfig.regionSize);
        int regY = (int)Mathf.Floor((postion.z + _terrainConfig.regionSize * 0.5f) / _terrainConfig.regionSize);

        Region region = null;
        int ikey = Region.GetRegionKey(regX, regY);
        if (regionsMap.ContainsKey(ikey))
            region = regionsMap[ikey];

        if (region == null)
            return false;

        Tile tile = region.GetTile(postion);

        // 如果该区域没有水面将返回
        if (tile == null || tile.water == null)
            return false;

        // 判断单位是否在水面下
        if (tile.water.Underwater(postion.y) == true)
        {
            waterHeight = tile.water.waterData.height;
            return true;
        }
        return false;
    }

    /** 水面高度 */
    public float waterHeight = 0f;

    public void SetDynamicHeight(bool isOpen, float moveHeight, int layer)
    {
        IsOpenDynamicHeight = isOpen;
        mCanMoveHeight = moveHeight;
        mLayerMask = layer;
    }


    /**************************************************************************
    * 功能：设置动态高度 
    ***************************************************************************/
    private float RaycastHeight = 130;
    private float RaycastLenght = 500;
    private Vector3 mRaycastOrigin;

    private int mLayerMask = 0;       // 
    private float mCanMoveHeight = 0;
    public bool IsOpenDynamicHeight = false;
    public bool IsFlying = false;

    /// <summary>
    /// 采样当前高度
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private float SampleDynamicHeight(Vector3 worldPosition, float height)
    {
        IsFlying = false;
        if (!IsOpenDynamicHeight)
        {
            return height;
        }

        RaycastHit hitdist;
        mRaycastOrigin = worldPosition;
        mRaycastOrigin.y += RaycastHeight;
        if (Physics.Raycast(mRaycastOrigin, -Vector3.up, out hitdist, RaycastLenght, mLayerMask))
        {
            if (hitdist.point.y <= height)
            {
                return height;
            }
            else
            {
                IsFlying = true;
                return hitdist.point.y;
            }
        }
        else
        {
            return height;
        }
    }

    /// <summary>
    /// 是否动态阻挡
    /// </summary>
    /// <param name="worldPostion"></param>
    /// <param name="collisionSize"></param>
    /// <returns></returns>
    private bool IsDynamicValidForWalk(Vector3 vFormPos, Vector3 vDestPos, float fCanMoveHeight, int collisionSize)
    {
        if (!IsOpenDynamicHeight)
        {
            return true;
        }
        RaycastHit hitdist;
        mRaycastOrigin = vDestPos;
        mRaycastOrigin.y += RaycastHeight;
        if (Physics.Raycast(mRaycastOrigin, -Vector3.up, out hitdist, RaycastLenght, mLayerMask))
        {
            if (hitdist.point.y - vFormPos.y >= fCanMoveHeight)
            {
                return false;
            }
        }
        else
        {
            float height = SampleHeight(vDestPos);
            if (Math.Abs(height - vFormPos.y) >= fCanMoveHeight)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 高度差行走判断
    /// </summary>
    /// <param name="vForm">起点</param>
    /// <param name="vDestPos">终点</param>
    /// <param name="fCanMoveHeight"></param>
    /// <returns></returns>
    private bool IsHeightValidForWalk(Vector3 vFormPos, Vector3 vDestPos, float fCanMoveHeight, int collisionSize)
    {
        if (vDestPos.y - vFormPos.y > fCanMoveHeight)
            return false;
        return IsDynamicValidForWalk(vFormPos, vDestPos, fCanMoveHeight, collisionSize);
    }

    private bool isHeightValiadForFly(Vector3 vFormPos, Vector3 vDestPos, float fCanMoveHeight, int collisionSize)
    {
        float fCurfloorHeight = SampleHeight(vDestPos);
        //动态高度
        fCurfloorHeight = SampleDynamicHeight(vDestPos, fCurfloorHeight);
        if (fCurfloorHeight > vDestPos.y)
            return false;
        return true;
    }

    /// <summary>
    /// 测试抛物线高度是否可走
    /// </summary>
    /// <param name="vFormPos"></param>
    /// <param name="vDestPos"></param>
    /// <param name="fCanMoveHeight"></param>
    /// <param name="collisionSize"></param>
    /// <param name="fJumpHeight"></param>
    /// <param name="fJumpDistance"></param>
    /// <param name="fDistance"></param>
    /// <returns></returns>
    private bool IsParabolaValidForWalk(Vector3 vFormPos, Vector3 vDestPos, float fCanMoveHeight, int collisionSize,
        float fJumpHeight, float fJumpDistance, float fDistance)
    {
        float fCurParabolaHeight = MathUtils.GetParabolaHeight(fJumpHeight, fJumpDistance, fDistance);
        float fCurfloorHeight = SampleHeight(vDestPos);
        //动态高度
        fCurfloorHeight = SampleDynamicHeight(vDestPos, fCurfloorHeight);
        if (fCurfloorHeight - vFormPos.y - fCurParabolaHeight > fCanMoveHeight)
            return false;

        return true;
    }

    #region 编译器代码
#if UNITY_EDITOR

    /// <summary>
    /// 编译器使用
    /// </summary>
    /// <param name="worldPostion"></param>
    /// <param name="collisionSize"></param>
    /// <returns></returns>
    public bool IsValidForWalkByEditor(Vector3 worldPostion, int collisionSize)
    {
        if (_mapPath == null)
            return false;

        int gx = getGridX(worldPostion.x);
        int gy = getGridY(worldPostion.z);

        if (gx < 0 || gy < 0 || gx >= _mapPath.grids.GetLength(0) || gy >= _mapPath.grids.GetLength(1))
            return false;

        int val = (_mapPath.grids[gx, gy] >> collisionSize) & 1;
        if (val == 1)
            return false;
        else
        {
            val = (_mapPath.grids[gx, gy] >> (collisionSize - 1)) & 1;
            if (val == 1)
                return false;
            else
                return true;
        }
    }

    /*********************************************************************************
     * 功能 : 在编辑器中删除准备删除单位及切片引用
     * 编辑器代码
     *************************************************************************************/
    public void RemoveUnitInEditor(string name)
    {
        GameObjectUnit unit = FindUnit(name);
        if (unit == null)
            return;

        unit.ClearTiles();

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].RemoveUnit(unit);
        }

        if (unit.Ins != null)
            GameObject.DestroyImmediate(unit.Ins);

        unitsMap.Remove(unit.createID);
        units.Remove(unit);
        unitCount--;
    }

    //编辑器代码
    public void RemoveUnitInEditor(GameObjectUnit unit)
    {
        if (unit == null)
            return;

        unit.ClearTiles();

        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].RemoveUnit(unit);
        }

        if (unit.Ins != null)
            GameObject.DestroyImmediate(unit.Ins);

        unitsMap.Remove(unit.createID);
        units.Remove(unit);
        unitCount--;
    }

    /*********************************************************************************
     * 功能 : 采集场景中使用的地形纹理
     * 编译器使用
     **********************************************************************************/
    public List<string> CollectTerrainSplatsAssetPath()
    {
        List<string> assetPaths = new List<string>();
        int count = tiles.Count;
        for (int i = 0; i < count; i++)
        {
            if (tiles[i].terrain != null)
            {
                Splat[] splats = tiles[i].terrain.terrainData.splats;
                for (int j = 0; j < splats.Length; j++)
                {
                    if (splats[j] != null)
                    {
                        if (assetPaths.Contains(splats[j].key) == false)
                        {
                            assetPaths.Add(splats[j].key);
                        }
                    }
                }
            }
        }
        return assetPaths;
    }

    /// <summary>
    /// 编译器使用
    /// </summary>
    public void UpdateViewRange()
    {
        int i = 0;
        int j = 0;

        for (i = 0; i < regions.Count; i++)
        {
            regions[i].UpdateViewRange();
        }

        GameObjectUnit unit = null;

        for (i = 0; i < units.Count; i++)
        {
            if (units[i].isStatic == true)
            {
                units[i].UpdateViewRange();
            }
        }

        for (j = 0; j < dynamicUnits.Count; j++)
        {
            unit = dynamicUnits[j];

            unit.near = terrainConfig.dynamiCullingDistance;
            unit.far = unit.near + 2f;
        }
    }

    /*****************************************************************************************************
     * 功能 : 场景中添加摄像机
     * 编译器方法
     *****************************************************************************************************/

    public void AddCamera(Camera camera)
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            if (cameras[i] == camera)
                return;
        }
        if (mainCamera == null)
            mainCamera = camera;
        //       camera.backgroundColor = RenderSettings.fogColor;
        cameras.Add(camera);
    }

    /// <summary>
    /// 编译器方法
    /// </summary>
    /// <param name="worldPostion"></param>
    /// <returns></returns>
    public int getGridValue(Vector3 worldPostion)
    {
        int x = getGridX(worldPostion.x);
        int y = getGridY(worldPostion.z);

        // 超出范围返回无效数据
        if (x < 0 || y < 0 || x >= _mapPath.grids.GetLength(0) || y >= _mapPath.grids.GetLength(1))
            return 1;
        return _mapPath.grids[x, y];
    }

    /*******************************************************************************************************
     * 功能 : 获取格子数据
     * 无用方法
     *******************************************************************************************************/

    public int getGridValue(int x, int y)
    {
        // 超出范围返回无效数据
        if (x >= _mapPath.grids.GetLength(0) || y >= _mapPath.grids.GetLength(1))
            return 0;
        return _mapPath.grids[x, y];
    }

    /*******************************************************************************************************
     * 功能 : 获取高度数据
     * 无用方法
     *******************************************************************************************************/

    public float getHeightValue(int x, int y)
    {
        // 超出范围返回无效数据
        if (x >= heights.GetLength(0) || y >= heights.GetLength(1))
            return 0;
        return ByteUtils.shortConverFloat(heights[x, y]);
    }

    /********************************************************************************************************
     * 编译器使用
     * 功能 ： 场景中实体相关操作
     * 注解：单位插入场景的时候不需要计算所属切片，因为切片有可能不在可视范围，还没有创建，　获取切片应该是动态的
     ********************************************************************************************************/
    public GameObjectUnit CreateUnit(Vector3 pos, string prePath, bool isStatic = true)
    {
        unitIdCount += 1;
        GameObjectUnit unit = GameObjectUnit.Create(this, pos, unitIdCount, prePath);
        unit.name = "Unit_" + unit.createID;                // 对象单位和实体名称一致
        unit.isStatic = isStatic;

        // 获取预定义资源的初始缩放和旋转
        UnityEngine.Object pre = Resources.Load(prePath, typeof(UnityEngine.Object));
        GameObject ins = UnityEngine.Object.Instantiate(pre) as GameObject;
        ins.transform.position = pos;
        unit.localScale = ins.transform.localScale;
        unit.Rotation = ins.transform.rotation;
        unit.type = UnitType.GetType(ins.layer);
        unit.unitParser = UnitType.GenUnitParser(unit.type);
        unit.unitParser.unit = unit;
        unit.Ins = ins;

        unit.ComputeTiles();
        unit.UpdateViewRange();

        GameObject.DestroyImmediate(ins);
        AddUnit(unit);

        return unit;
    }


    /// <summary>
    /// 编译器使用
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="prePath"></param>
    /// <param name="rotation"></param>
    /// <param name="isStatic"></param>
    /// <param name="parser"></param>
    /// <returns></returns>
    public GameObjectUnit CreateUnit(Vector3 pos, string prePath, Quaternion rotation, bool isStatic = true, UnitParser parser = null)
    {
        unitIdCount += 1;
        GameObjectUnit unit = GameObjectUnit.Create(this, pos, unitIdCount, prePath);
        unit.name = "Unit_" + unit.createID;                // 对象单位和实体名称一致
        unit.isStatic = isStatic;
        // 获取预定义资源的初始缩放和旋转
        UnityEngine.Object pre = Resources.Load(prePath, typeof(UnityEngine.Object));
        GameObject ins = UnityEngine.Object.Instantiate(pre) as GameObject;
        ins.transform.position = pos;
        unit.localScale = ins.transform.localScale;
        unit.Rotation = rotation;
        unit.type = UnitType.GetType(ins.layer);

        if (parser == null)
            unit.unitParser = UnitType.GenUnitParser(unit.type);
        else
            unit.unitParser = parser;

        unit.unitParser.unit = unit;

        unit.Ins = ins;
        unit.ComputeTiles();
        unit.UpdateViewRange();

        GameObject.DestroyImmediate(ins);

        AddUnit(unit);
        return unit;
    }

    /******************************************************************************************************************
    * 功能 : 在可视地域中查找单位
     * 无用方法
    ******************************************************************************************************************/
    public GameObjectUnit FindUnitInRegions(int createID)
    {
        int count = regions.Count;
        GameObjectUnit unit = null;
        for (int i = 0; i < count; i++)
        {
            unit = regions[i].FindUint(createID);
            if (unit != null)
                return unit;
        }
        return null;
    }

    /************************************************************************************************************
    * 功能 : 在可视切片中查找单位
     * 编译器使用
    ************************************************************************************************************/
    public GameObjectUnit FindUnitInTiles(int createID)
    {
        int count = tiles.Count;
        GameObjectUnit unit = null;
        for (int i = 0; i < count; i++)
        {
            unit = tiles[i].FindUnit(createID);
            if (unit != null)
                return unit;
        }
        return null;
    }

    //编译器使用
    public GameObjectUnit FindFirstUnit(int type)
    {
        int len = units.Count;
        for (int i = 0; i < len; i++)
        {
            if (units[i].type == type)
                return units[i];
        }
        return null;
    }

    /*************************************************************************************
     * 功能 : 在可视列表中查找单位
     * 编译器使用
     ****************************************************************************************/
    public GameObjectUnit FindUnit(int cID)
    {
        if (unitsMap.ContainsKey(cID))
            return unitsMap[cID];
        else
            return null;
    }

    /*********************************************************************************
     * 功能 : 采集场景中静态单位资源集
     * 编译器方法
     **********************************************************************************/
    public List<string> CollectStaticUnitAssetPath()
    {
        List<string> assetPaths = new List<string>();
        int count = units.Count;
        for (int i = 0; i < count; i++)
        {
            if (units[i].isStatic == true)
            {
                if (assetPaths.Contains(units[i].prePath) == false)
                {
                    if (units[i].Ins != null)
                        assetPaths.Add(units[i].prePath);
                }
            }
        }
        return assetPaths;
    }

    /// <summary>
    /// 无用方法
    /// </summary>
    /// <param name="srcPos"></param>
    /// <param name="dstPos"></param>
    /// <param name="canMoveHeight"></param>
    /// <param name="collisionSize"></param>
    /// <param name="modelHeight"></param>
    /// <returns></returns>
    public bool IsValidForWalk(Vector3 srcPos, Vector3 dstPos, float canMoveHeight, int collisionSize, float modelHeight)
    {
        if (Math.Abs(dstPos.y - srcPos.y) > canMoveHeight)
        {
            return false;
        }
        if (terrainWalkableData == null)
        {
            return false;
        }
        srcX = Mathf.FloorToInt(srcPos.x);
        srcZ = Mathf.FloorToInt(srcPos.z);
        dstX = Mathf.FloorToInt(dstPos.x);
        dstZ = Mathf.FloorToInt(dstPos.z);

        if (Math.Abs(srcX) >= terrainWalkableData.HalfWidth ||
            Math.Abs(srcZ) >= terrainWalkableData.HalfHeight ||
            Math.Abs(dstX) >= terrainWalkableData.HalfWidth ||
            Math.Abs(dstZ) >= terrainWalkableData.HalfHeight)
            return false;

        srcX += terrainWalkableData.HalfWidth;
        srcZ += terrainWalkableData.HalfHeight;
        dstX += terrainWalkableData.HalfWidth;
        dstZ += terrainWalkableData.HalfHeight;

        // 获取2点所经历的所有格子
        List<Vector3> gridPosList = GetTowPosAllIntersectGrid(srcX, srcPos.y, srcZ, dstX, dstPos.y, dstZ);
        if (gridPosList == null)
        {
            return false;
        }
        float height = 0;
        int floor = 0;
        int x = 0;
        int z = 0;
        float y = 0f;
        int count = gridPosList.Count;
        for (int i = 0; i < count; ++i)
        {
            x = (int)gridPosList[i].x;
            y = gridPosList[i].y;
            z = (int)gridPosList[i].z;
            // 注:返回的height ==0则传入坐标错误
            floor = GetApexFloor(x, z, y, canMoveHeight, out height);
            if (height <= 0)
            {
                return false;
            }
            if (!HasMotionSpace(x, z, floor, y - height, modelHeight))
            {
                return false;
            }
            // 如果是腾空状态则不检测阻挡
            if (Math.Abs(y - height) <= canMoveHeight && !GetFloorCanMove(x, z, floor, collisionSize))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    ///  获取2点之间所有相交格子
    ///  注：最少返回起点和目标点
    ///  无用方法
    /// </summary>
    public List<Vector3> GetTowPosAllIntersectGrid(int srcX, float srcY, int srcZ, int dstX, float dstY, int dstZ)
    {
        List<Vector3> gridPosList = new List<Vector3>();
        int diffX = Math.Abs(dstX - srcX);
        float diffY = Math.Abs(dstY - srcY);
        int diffZ = Math.Abs(dstZ - srcZ);
        int iSignX = (dstX - srcX) >= 0 ? 1 : -1;
        int iSignY = (dstY - srcY) >= 0 ? 1 : -1;
        int iSignZ = (dstZ - srcZ) >= 0 ? 1 : -1;

        bool flag = false;
        if (diffZ > diffX)
        {
            diffZ += diffX;
            diffX = diffZ - diffX;
            diffZ = diffZ - diffX;
            flag = true;
        }
        else
        {
            flag = false;
        }

        int nError = 2 * diffZ - diffX;

        int x = srcX;
        int z = srcZ;
        float y = srcY;
        Vector3 vec = new Vector3(x, y, z);
        gridPosList.Add(vec);

        for (int i = 0; i < diffX; ++i)
        {
            if (nError >= 0)
            {
                if (flag)
                {
                    x += iSignX;
                }
                else
                {
                    z += iSignZ;
                }
                nError = nError - 2 * diffX;
            }

            if (flag)
            {
                z += iSignZ;
            }
            else
            {
                x += iSignX;
            }
            nError += 2 * diffZ;

            y += diffY * iSignY / diffX;
            vec = new Vector3(x, y, z);
            gridPosList.Add(vec);
        }
        return gridPosList;
    }

    /// <summary>
    ///  判断是否有空间可以容纳玩家
    ///  无用方法
    /// </summary>
    /// <param name="row">x坐标</param>
    /// <param name="col">z坐标</param>
    /// <param name="floor">所处层</param>
    /// <param name="diffY">所处高度； 注：当前位置到当前层的高度</param>
    /// <param name="modeHeight">模型高度</param>
    /// <returns></returns>
    public bool HasMotionSpace(int row, int col, int floor, float diffY, float modeHeight)
    {
        if (terrainWalkableData == null)
        {
            return false;
        }
        return (diffY + modeHeight) <= terrainWalkableData.GetFloorSpace(row, col, floor);
    }

    /// <summary>
    /// 获得层是否可移动
    /// 无用方法
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="floor"></param>
    /// <param name="collisionSize"></param>
    /// <returns></returns>
    public bool GetFloorCanMove(int row, int col, int floor, int collisionSize)
    {
        if (terrainWalkableData == null)
        {
            return false;
        }
        int walkMarker = terrainWalkableData.GetWalkMarker(row, col, floor);
        int val = (walkMarker >> collisionSize) & 1;
        if (val == 1)
            return false;
        else
        {
            val = (walkMarker >> (collisionSize - 1)) & 1;
            if (val == 1)
                return false;
            else
                return true;
        }
    }

    /// <summary>
    /// 获取点所在层
    /// 无用方法
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="y"></param>
    /// <param name="height">该层的层高</param>
    /// <returns></returns>
    public int GetApexFloor(int row, int col, float y, float canMoveHeight, out float height)
    {
        height = 0;
        if (terrainWalkableData == null)
        {
            return 0;
        }
        int floorSum = terrainWalkableData.GetFloorCount() - 1;
        for (int i = floorSum; i >= 0; --i)
        {
            height = terrainWalkableData.GetFloorHeight(row, col, i);
            if (Math.Abs(y - height) <= canMoveHeight)
            {
                return i;
            }
        }
        //height = 0;
        return 0;
    }

    /// <summary>
    ///  取样高度
    ///  无用方法
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="floor"></param>
    /// <param name="interpolation"></param>
    /// <returns></returns>
    public float SampleHeight(Vector3 worldPosition, out int floor, float canMoveHeight, bool interpolation = true)
    {
        float height = 0;
        floor = 0;
        if (terrainWalkableData == null)
        {
            return height;
        }
        int x = Mathf.FloorToInt(worldPosition.x);
        int z = Mathf.FloorToInt(worldPosition.z);
        if (Math.Abs(x) >= terrainWalkableData.HalfWidth || Math.Abs(z) >= terrainWalkableData.HalfHeight)
        {
            return terrainConfig != null ? terrainConfig.defaultTerrainHeight : 1000f;
        }

        x += terrainWalkableData.HalfWidth;
        z += terrainWalkableData.HalfHeight;

        floor = GetApexFloor(x, z, worldPosition.y, canMoveHeight, out height);
        if (height == 0)
        {
            // 插值采样高度
            // 普通采样高度
            // 这个功能暂缺
        }
        return height;
    }

    public float SampleMainHeight(Vector3 worldPosition, bool interpolation = true)
    {
        if (terrainWalkableData == null)
        {
            return 99999f;
        }
        int x = Mathf.FloorToInt(worldPosition.x);
        int z = Mathf.FloorToInt(worldPosition.z);
        if (Math.Abs(x) >= terrainWalkableData.HalfWidth || Math.Abs(z) >= terrainWalkableData.HalfHeight)
        {
            return terrainConfig != null ? terrainConfig.defaultTerrainHeight : 99999f;
        }
        x += terrainWalkableData.HalfWidth;
        z += terrainWalkableData.HalfHeight;

        return terrainWalkableData.GetFloorHeight(x, z, 0);
    }
#endif
    #endregion

}
