using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
using System.IO;

/*******************************************************************************************************
 * 类 ： 游戏对象元数据，地形数据保存
 * 注 ：静态单位的销毁是自动的
 *      如有新增的光照贴图，将光照贴图增加到场景，没有应用的时候将光照贴图从场景中删除
 *******************************************************************************************************/
public class GameObjectUnit
{
    public bool isMainUint = false;

#region 编译器脚本
#if UNITY_EDITOR

    string shaderName = "";
    static protected string diffuseShaderName = "Legacy Shaders/Diffuse";
    static protected string diffuseCutoutShaderName = "Legacy Shaders/Transparent/Cutout/Diffuse";
    static protected string snailDiffuseShaderName = "Snail/Diffuse";
    static protected string snailDiffuseCutoutShaderName = "Snail/Transparent/Cutout/Diffuse";
    static protected string snailDiffusePointShaderName = "Snail/Diffuse-PointLight";
    static protected string snailDiffusePointCutoutShaderName = "Snail/Diffuse-PointLight-Cutout";
    static protected Shader snailDiffuseShader = Shader.Find(snailDiffuseShaderName);
    static protected Shader snailDiffuseCutoutShader = Shader.Find(snailDiffuseCutoutShaderName);

    public int lightmapSize = 1024;

    public Vector4 lightmapTilingOffset;

    public int lightmapIndex;

    public GameObject combinIns;

    /** 单元的名字 */
    public string name = "";

    /** 经过缩放的包围体尺寸, 已经废弃 */
    public Vector3 size = Vector3.zero;

    /** 合并单位列表 */
    public List<int> combinUnitIDs = new List<int>();

    /** 创建单元的切片 */
    public Tile mainTile;

    /** 记录当前单位使用的材质 */
    public List<Material> materials = new List<Material>();

    /// <summary>
    /// 是否手动设置踢除距离
    /// </summary>
    public bool mbManualCullingFactor = false;
#endif
#endregion

    public const int genRippleDelayTick = 50;

    /** 实例创建监听器 */
    public delegate void CreateInsListener(GameObjectUnit unit);
    public CreateInsListener createInsListener;

    /** 实例销毁监听器 */
    public delegate void DestroyInsListener();
    public DestroyInsListener destroyInsListener;

    public delegate void ActiveListener(bool value);
    public ActiveListener activeListener;

    /** 游戏对象创建的ID */
    public int createID = 0;

    /** 是否是碰撞器， 碰撞器将填充格子数据, 第三方移动对象不进行碰撞检测 */
    public bool isCollider = true;

    /** 单元渲染实例 */
    private GameObject ins;
    public GameObject Ins
    {
        get { return ins; }
        set {ins = value; }
    }

    public Vector3 InsPosition
    {
        get { return Ins.transform.position; }
        set {Ins.transform.position = value; }
    }

    public Vector3 InsLocalPosition
    {
        get { return Ins.transform.localPosition; }
        set { Ins.transform.localPosition = value; }
    }

    public Quaternion InsRotation
    {
        get { return Ins.transform.rotation; }
        set { Ins.transform.rotation = value; }
    }


    /** 是否是静态单元, 静态单元运行时避免阻塞格子计算、空间计算 */
    public bool isStatic = true;

    /** 创建单元的预定义资源路径，工程目录中的路径 */
    public string prePath = "";
	public string prePath2= string.Empty;
    /** 单元的转化信息 */
    private Vector3 position;
    public Vector3 Position
    {
        get { return position; }
        set { position = value;}
    }

    public void SetPostionY(float fy)
    {
        position.y = fy;
    }

    //public CGameObject parent;

    private Quaternion rotation;
    public Quaternion Rotation
    {
        get { return rotation; }
        set 
        {
            //if (this is DynamicUnit)
            //{
            //    DynamicUnit dUnit = this as DynamicUnit;
            //    if (dUnit.isMainUint)
            //    {
            //        LogSystem.LogError("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx value " + value.eulerAngles.y);
            //    }
            //}
            rotation = value;
        }
    }

    public Vector3 forward
    {
        get
        {
            return Rotation * Vector3.forward;
        }
    }
    public Vector3 localScale;

    public Vector3 center;

    /// <summary>
    /// 剔除距离(非因子)
    /// </summary>
    private float mfCullingFactor = 0f;
    public float cullingFactor
    {
        get
        {
            return mfCullingFactor;
        }
        set
        {
            mfCullingFactor = value;
            UpdateViewRange();
        }
    }



    /** 游戏对象所需的资源路径 */
    private int dependResCount = 0;

    public bool visible = false;

    private float _naar = 0f;
    /** 剔除距离范围 */
    public float near
    {
        get
        {
            return _naar;
        }
        set
        {
            //if (this is DynamicUnit)
            //{
            //    LogSystem.Log("动态　_naar:" + value);
            //}
            //else
            //{
            //    LogSystem.Log("静态 _naar:" + value);
            //}
            _naar = value;
        }
    }
    
    public float far = 0f;

    public float viewDistance = 0f;

    public float viewAngle = 0f;

    /** 当前单元占用的切片 */
    public List<Tile> tiles = new List<Tile>();

    /** 当前单位是否占了格子 */
    public bool hasCollision = false;

    /************************************************************************
     * 功能 ： 碰撞体积, 用来计算碰撞, 计算物体所占的格子尺寸
     * ------------
     * |    ↑    |
     * |←radius→|
     * |    ↓    |
     * ------------
     * 建筑物根据格子占用来计算半径, 先计算包围圈的碰撞，在计算详细格子的碰撞    
     ************************************************************************/
    public float radius = 1f;

    /** 碰撞格子尺寸 */
    public int collisionSize = 1;

    /** 对象所占格子 */
    private int[,] _grids;

    /** 对象所占格子 */
    public int[,] grids
    {
        set
        {
            if (value == null)
                _grids = null;
            else
            {
                if (scene != null && scene.mapPath != null && value != null)
                    _grids = scene.mapPath.CheckCustomGrids(value);
                else
                    _grids = value;
            }
        }

        get
        {
            return _grids;
        }
    }

    /******************************************************************************
     * 功能 ：单位渲染器依赖的光照贴图位置
     ******************************************************************************/
    public List<LightmapPrototype> lightmapPrototypes = new List<LightmapPrototype>();

    /** 是否需要屏幕坐标 */
    public bool needScreenPoint = true;

    /** 屏幕坐标位置 */
    public Vector3 screenPoint;

    /** 鼠标拾取判断 */
    public bool mouseEnable = true;

    /** 单位类型, 默认是 */
    public int type = UnitType.UnitType_General;

    public UnitParser unitParser;


    /** 单位是否激起涟漪效果 */
    public bool genRipple = false;
    protected Vector3 ripplePos;


    public GameScene scene;

    /** 单位是否显示激活 */
    public bool active = false;

    protected Vector3 scenePoint = Vector3.zero;
    public float scenePointBias = 1f;

    /** 判断是否需要采用当前高度 */
    public bool needSampleHeight = true;


    /** 合并后父级单位ID */
    public int combineParentUnitID = -1;


    private bool readed = false;

    private long dataLength = 0;

    public bool destroyed = false;

    protected bool _rotationDirty = false;

    protected bool _rootDirty = false;


	public delegate void ThridPardResourManager(string strFileName, AssetCallback back, VarStore varStore=null, bool bAsync = false);
    static public ThridPardResourManager thridPardResourManager;

    public GameObjectUnit(int createID)
    {
        this.createID = createID;
    }

    /****************************************************************************************
     * 功能 ： 从存储文件中读取单元信息
     ****************************************************************************************/
    public void Read(BinaryReader br, int cID)
    {
        if (readed == true && dataLength > 0)
        {
            br.BaseStream.Position += dataLength;
            return;
        }
        int i = 0;
        long pos;
        long startPos = 0;
        long lightPos = 0;
        long endPos;
        createID = cID;

        pos = br.BaseStream.Position;
        startPos = pos;
        prePath = br.ReadString();
		if (prePath.Contains ("/Prefabs/")) 
		{
			prePath2 = prePath.Replace ("/Prefabs/", "/Prefabs2/");
		}
		else
		{
			prePath2 = prePath;
		}
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10009)
        {
            type = br.ReadInt32();

            // 在编辑状态下进行错误检测
            /**
            if (GameScene.isPlaying == false)
            {
                if (prePath.Contains("Light") || prePath.Contains("light")) 
                    type = UnitType.UnitType_Light;
            }
            **/
            //if (prePath.Contains("_light"))
            //type = UnitType.UnitType_General;

            unitParser = UnitType.GenUnitParser(type);
            unitParser.unit = this;

            // 游戏运行时, 跳出灯光读取
            if (GameScene.isPlaying == true && type == UnitType.UnitType_Light && scene.lightDataLength > 0)
            {
                br.BaseStream.Position += scene.lightDataLength;
                return;
            }

            if (type == UnitType.UnitType_Light)
                lightPos = br.BaseStream.Position;

            // 解析单位数据
            unitParser.Read(br);
        }
        else
        {
            br.BaseStream.Position = pos;
            unitParser = new UnitParser();      // 设置为普通的单位解析
        }

        // 获取游戏对象依赖资源数量
        dependResCount = br.ReadInt32();
        for (i = 0; i < dependResCount; i++)
        {
            br.ReadInt32();
        }

        // 读取烘焙数据
        int count = br.ReadInt32();

        lightmapPrototypes = new List<LightmapPrototype>(count);
        for (i = 0; i < count; i++)
        {
            LightmapPrototype pro = new LightmapPrototype();
            pro.rendererChildIndex = br.ReadInt32();
            pro.lightmapIndex = br.ReadInt32();

            pos = br.BaseStream.Position;
            if (br.ReadInt32() == 10006)
                pro.scale = br.ReadSingle();
            else
                br.BaseStream.Position = pos;

            pro.lightmapTilingOffset.x = br.ReadSingle();
            pro.lightmapTilingOffset.y = br.ReadSingle();
            pro.lightmapTilingOffset.z = br.ReadSingle();
            pro.lightmapTilingOffset.w = br.ReadSingle();

            lightmapPrototypes.Add(pro);
        }

        br.ReadSingle();

        // 读取游戏对象的位置信息
        Position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        Rotation = new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        localScale = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

        // 读取合并信息
        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 20001)
        {
            bool isCombinUnit = false;
#if UNITY_EDITOR
            this.combineParentUnitID = br.ReadInt32();
            isCombinUnit = br.ReadBoolean();
            if (isCombinUnit == true)
            {
                count = br.ReadInt32();
                // 避免销毁后重复利用时出错
                if (combinUnitIDs == null)
                    combinUnitIDs = new List<int>(count);
                for (i = 0; i < count; i++)
                {
                    this.combinUnitIDs.Add(br.ReadInt32());
                }
            }
#else
            //避免字节流索引出错
            br.ReadInt32();
            br.ReadBoolean();
            if(isCombinUnit)
            {
                count = br.ReadInt32();
                for (i = 0; i < count; i++)
                {
                    br.ReadInt32();
                }
            }
#endif
        }
        else
        {
            br.BaseStream.Position = pos;
        }


        //是否手动设置踢除范围
        pos = br.BaseStream.Position;
        if (br.ReadUInt16() == 60127)
        {
#if UNITY_EDITOR
            mbManualCullingFactor = br.ReadBoolean();
            mbManualCullingFactor = true;
#else
            br.BaseStream.Position += 1;
#endif
        }
        else
        {
#if UNITY_EDITOR
            mbManualCullingFactor = true;
#endif
            br.BaseStream.Position = pos;
        }

        // 读取剔除LOD因子
#if UNITY_EDITOR

        mfCullingFactor = br.ReadSingle();

        if (GameScene.mainScene.cullData != null)
        {
            if (GameScene.mainScene.cullData.cullingKey.Contains(createID))
            {
                int index = GameScene.mainScene.cullData.cullingKey.IndexOf(createID);
                mfCullingFactor = GameScene.mainScene.cullData.cullingValue[index];
            }
        }

#else
       mfCullingFactor = br.ReadSingle();
#endif

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 39999)
        {
            //br.ReadInt32();
            br.BaseStream.Position += 4;
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 40001)
        {
            center = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        }
        else
        {
            center = Position;
            br.BaseStream.Position = pos;

            br.ReadSingle();
            br.ReadSingle();
            br.ReadSingle();
        }

        /**
        if (Mathf.Abs(position.x) > 10f || Mathf.Abs(position.y) > 10f || Mathf.Abs(position.z) > 10f)
        {
            if (Mathf.Abs(position.x - center.x) < 10f || Mathf.Abs(position.y - center.y) < 10f || Mathf.Abs(position.z - center.z) < 10f)
                center = position;
        }
        **/

        br.ReadSingle();
        br.ReadSingle();

        //if (cullingFactor <= 0.01f)
        //    cullingFactor = scene.terrainConfig.defautCullingFactor;

        UpdateViewRange();

        endPos = br.BaseStream.Position;
        dataLength = endPos - startPos;

        if (type == UnitType.UnitType_Light)
            scene.lightDataLength = endPos - lightPos;
        readed = true;
    }

    public float LookAt(Vector3 target)
    {
        if ((Mathf.Abs(target.z - Position.z) < 0.01f) && (Mathf.Abs(target.x - Position.x) < 0.01f))
            return 0f;

        Vector3 vEuler = Vector3.zero;
        vEuler.y = -180.0f / Mathf.PI * Mathf.Atan2(target.z - Position.z, target.x - Position.x) + 90f;
        Rotation = Quaternion.Euler(vEuler);
        return vEuler.y;
    }
   // float rotationSpeed = 0;
    float rotationTime = 0;


    /**************************************************************************************************
     * 功能 : 单位Y轴旋转角度，传入是弧度值
     * 注解 : 可设置是否立即改变Y轴旋转
     ****************************************************************************************************/
    public void RotaionY(float deg, bool immediately = false)
    {
        

        if (_rootDirty) return;
        //if (Ins != null)
        //{
        //    LogSystem.LogError("Ins.name = " + Ins.name + " , ===RotaionY ==" + deg * Mathf.Rad2Deg + " , tag = " + InsRotation.eulerAngles.y);
        //}
        Rotation = Quaternion.Euler(0, deg * Mathf.Rad2Deg, 0);
        if (immediately == true && Ins != null)
        {
            InsRotation = this.Rotation;
        }
        else
        {
            _rotationDirty = true;
        }
    }

    /**************************************************************************************************
    * 功能 : 单位Y轴旋转角度,传入是角度值
    * 注解 : 可设置是否立即改变Y轴旋转
    ****************************************************************************************************/
    public void RotaionAngleY(float deg, bool immediately = false)
    {
        //if (Ins != null) 
        //{
        //    LogSystem.LogError("Ins.name = " + Ins.name + " , ===RotaionAngleY ==" + deg);
        //}
        Rotation = Quaternion.Euler(0, deg, 0);
        if (immediately == true && Ins != null)
            InsRotation = this.Rotation;
    }

    public void SetRotation(Quaternion rot, bool immediately = false)
    {
        //if (Ins != null)
        //{
        //    LogSystem.LogError("Ins.name = " + Ins.name + " , ===SetRotation ==" + rot.eulerAngles.y);
        //}
        _rootDirty = false;
 
        this.Rotation = rot;
        if (immediately == true && Ins != null)
            InsRotation = this.Rotation;
        else
            _rotationDirty = true;
    }

    /*************************************************************************
     * 功能 ： 静态单位的更新
     *************************************************************************/
    virtual public void Update()
    {
#if UNITY_EDITOR
        // 单位更新处理 -----------------------------------------------------------------------
        if (Ins != null)
        {
            if (GameScene.isPlaying == false)
            {
                // 记录编辑器中静态单位的位置
                if (isStatic == true)
                {
                    // 位置发生改变重新计算单位所属
                    if ((Mathf.Abs(InsPosition.x - Position.x) > 0.01f) || (Mathf.Abs(InsPosition.z - Position.z) > 0.01f))
                        ComputeTiles();

                    Position = InsPosition;
                    Rotation = InsRotation;
                    localScale = Ins.transform.localScale;

                    // 存储单位的属性
                    unitParser.Update(Ins);
                }
            }
        }
#endif
    }

    /************************************************************************************
     * 功能 : 第三方资源获取
     **************************************************************************************/
    private void OnThridPartAssetLoaded(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {

//#if UNITY_EDITOR
        if (oAsset == null)
        {
            LogSystem.LogWarning("OnThridPartAssetLoaded is null:" + strFileName);
//            return;
        }
//#endif
        //已删除 || 不显示
        if (destroyed || !visible)
        {
            //不缓存场景实例
            CacheObjects.PopCache(oAsset);
            return;
        }

        // 初始化对象
        Initialize(oAsset);

        //激活动态单位
        ActiveDynaUnit();
    }


    /// <summary>
    /// 资源加载
    /// </summary>
    /// <param name="strFileName"></param>
    /// <param name="back"></param>
    /// <param name="varStore"></param>
    /// <returns></returns>
	public static bool ThridPardLoad(string strFileName, AssetCallback back, VarStore varStore, bool bAsync = false)
    {
        if (thridPardResourManager == null)
        {
            UnityEngine.Object o = Resources.Load(strFileName);
            if (back != null)
                back(o, strFileName, varStore);
            return false;
        }
		thridPardResourManager(strFileName, back, varStore, bAsync);
        return true;
    }

    /// <summary>
    /// 物体实例化
    /// </summary>
    private void Initialize(UnityEngine.Object oAsset)
    {
        if (destroyed == true)
            return;

        if (Ins != null)
            return;

        // 实例化对象
        Ins = CacheObjects.InstantiatePool(oAsset) as GameObject;

        // 编辑器状态下重新判断单位类型,避免编辑者修改单位类型
        if (GameScene.isPlaying == false)
        {
            type = UnitType.GetType(Ins.layer);
        }

        if (isStatic == false && needSampleHeight == true)
            SetPostionY(scene.SampleHeight(Position));

        InsPosition = Position;

        OnInitialize();

        // 更名方便美术编辑
        Renamme();
    }

    /// <summary>
    /// 被实列化
    /// </summary>
    protected virtual void OnInitialize()
    {
        InsRotation = Rotation;
        Ins.transform.localScale = localScale;

        // 渲染器读取光照贴图(无光照贴图时不用设置)
        if (LightmapSettings.lightmaps.Length > 0)
        {
            for (int i = 0; i < lightmapPrototypes.Count; i++)
            {
                LightmapPrototype pro = lightmapPrototypes[i];
                Renderer renderer = null;
                if (pro.rendererChildIndex == -1)
                {
                    renderer = Ins.GetComponent<Renderer>();
                }
                else
                {
                    if (pro.rendererChildIndex < Ins.transform.childCount)
                        renderer = Ins.transform.GetChild(pro.rendererChildIndex).GetComponent<Renderer>();
                }
                if (renderer != null)
                {
                    renderer.lightmapIndex = pro.lightmapIndex;
                    renderer.lightmapScaleOffset = pro.lightmapTilingOffset;
                }
            }
        }
        #region 编译器脚本

#if UNITY_EDITOR
        // 编辑中选择
        if (GameScene.isPlaying == false)
        {
            List<Renderer> renderers = new List<Renderer>();
            if (Ins.GetComponent<Renderer>() != null)
                renderers.Add(Ins.GetComponent<Renderer>());
            for (int i = 0; i < Ins.transform.childCount; i++)
            {
                Renderer renderer = Ins.transform.GetChild(i).GetComponent<Renderer>();
                if (renderer != null)
                    renderers.Add(renderer);
            }
            // 
            // if (ins.isStatic == false)
            // this.center = this.position;

            for (int j = 0; j < renderers.Count; j++)
            {
                // 游戏已经有烘焙贴图后不投射阴影
                //renderers[j].receiveShadows = true;
                //renderers[j].castShadows = true;

                for (int i = 0; i < renderers[j].sharedMaterials.Length; i++)
                {
                    Material mt = renderers[j].sharedMaterials[i];

                    // 统计材质
                    if (scene.statisticMode == true)
                    {
                        Statistic.Push(mt, AssetType.Material);
                    }

                    if (mt != null)
                    {
                        shaderName = mt.shader.name;

                        if (shaderName == diffuseShaderName || shaderName == snailDiffusePointShaderName)
                            mt.shader = snailDiffuseShader;
                        if (shaderName == diffuseCutoutShaderName || shaderName == snailDiffusePointCutoutShaderName)
                            mt.shader = snailDiffuseCutoutShader;
                    }
                }
            }
        }

        // 在编辑模式下计算LOD距离, 为了方便编辑对象拾取
        if (GameScene.isPlaying == false)
        {
            // 收集材质,统计信息
            CollectMaterials();

            // 追加MESH碰撞到物件,方便摄像拾取
            AddMeshRenderColliders();

            if (cullingFactor <= 0.01f)
                cullingFactor = scene.terrainConfig.defautCullingFactor;

            // 动态单位不需要计算所属切片
            if (isStatic == true)
                ComputeTiles();
        }
#endif
        #endregion
    }

    
    /// <summary>
    /// 激活动态单位
    /// </summary>
    private void ActiveDynaUnit()
    {
        if (this.Ins == null)
        {
            LogSystem.LogWarning("GameObjectUnit::ActiveDynaUnit ins is null");
            return;
        }

        Ins.SetActive(true);
        OnActiveUnit(true);
    }

    /// <summary>
    /// 被激活或隐藏
    /// </summary>
    /// <param name="bValue"></param>
    protected virtual void OnActiveUnit(bool bValue)
    {

    }


    /*******************************************************************************************************
     * 功能 : 更新显示
     * 注：静态物体显示不回调
     *  　 动态物体需要回调
     *********************************************************************************************************/
    public void Visible()
    {
        if (destroyed)
            return;

        if (visible)
            return;

        visible = true;
        if (Ins == null)
        {
			string strPrefab = prePath;

			if (GameScene.mainScene != null && GameScene.mainScene.terrainConfig.bumpCount == 0 && !string.IsNullOrEmpty(prePath2) )
			{
				strPrefab = prePath2;
            }

			ThridPardLoad(strPrefab, OnThridPartAssetLoaded, null, true);
        }
        else
        {
            ActiveDynaUnit();
        }
    }

    /****************************************************************************************************************
     * 功能 : 重新定义名称
     ****************************************************************************************************************/
    public void Renamme()
    {
        if (GameScene.isPlaying == false)
        {
            // 显示包含文件名称及烘焙缩放的 实例名称
            string[] split = prePath.Split(SplitChars.chBS);
            Ins.name = split[split.Length - 1] + "_Unit_" + createID;
        }
        else
        {
            Ins.name = "Unit_" + createID;
        }
    }

    /** 当地形切片隐藏 */
    public void Invisible()
    {
        if (visible == true)
        {
            if (Ins != null)
            {
                // 静态单位先缓存,再释放
                Ins.SetActive(false);
                // 动态单位激活处理
                OnActiveUnit(false);
                if (isStatic)
                {
                    //静态物体隐藏即删除.动态物体等删除回调再删除
                    CacheObjects.DestoryPoolObject(Ins);
                    Ins = null;
                }
            }
        }
        visible = false;
    }

    /**************************************************************************************
     * 功能 ： 单位进行销毁
     **************************************************************************************/
    virtual public void Destroy()
    {
        if (destroyInsListener != null)
        {
            try
            {
                destroyInsListener.Invoke();
            }
            catch (Exception e)
            {
                LogSystem.LogError("监听创建单位函数中出错!" + e.ToString());
            }
        }

        if (Ins != null)
        {
            //直接删除，没必要再用缓存池
            if (GameScene.isPlaying == true)
                CacheObjects.DestoryPoolObject(Ins);
            else
                GameObject.DestroyImmediate(Ins);

            Ins = null;
        }

#if UNITY_EDITOR
        if (combinUnitIDs != null)
        {
            this.combinUnitIDs.Clear();
            //this.combinUnitIDs = null;
        }
#endif

        combineParentUnitID = -1;
        if (tiles != null)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                tiles[i].RemoveUnit(this);
            }
            tiles.Clear();
            //tiles = null;
        }

#if UNITY_EDITOR
        if (materials != null)
        {
            materials.Clear();
            materials = null;
        }
#endif
        this.active = false;
        this.grids = null;
       
        if (unitParser != null)
        {
            unitParser.Destroy();
            unitParser = null;
        }

        if (lightmapPrototypes != null)
        {
            lightmapPrototypes.Clear();
            // lightmapPrototypes = null;
        }
        this.dataLength = -1;
        this.prePath = "";
        this.createInsListener = null;
        this.destroyInsListener = null;
        this.activeListener = null;
        this.visible = false;
        this.destroyed = true;
        this.scene = null;
        this.readed = false;
    }

    /******************************************************************************************
     * 功能 : 更新剔除距离  
     ******************************************************************************************/
    public void UpdateViewRange()
    {
#if NOLOD
        near = float.MaxValue;
        far = float.MaxValue;
#else
        near = scene.terrainConfig.unitCullingMultiplier * mfCullingFactor + scene.terrainConfig.cullingBaseDistance;
        //Debug.Log("unitCullingDistance:" + scene.terrainConfig.unitCullingDistance + " cullingFactor:" + cullingFactor + " cullingBaseDistance:" + scene.terrainConfig.cullingBaseDistance);
        far = near + 2f;
#endif
    }

    #region 编译器方法

#if UNITY_EDITOR

    //编译器使用
    public void ClearTiles()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].RemoveUnit(this);
        }
        tiles.Clear();
    }

    /// <summary>
    /// 编译器使用
    /// </summary>
    /// <param name="tile"></param>
    public void AddTile(Tile tile)
    {
        tile.AddUnit(this);
        if (tiles.Contains(tile) == false)
            tiles.Add(tile);
    }

    /**********************************************************************************
     * 功能 : 给MESH渲染器增加碰撞体,方便在编辑器中拾取
     * 编译器使用
     *************************************************************************************/
    private void AddMeshRenderColliders()
    {
        if (Ins != null)
        {
            if (Ins.transform.GetComponent<Renderer>())
            {
                if (Ins.GetComponent<MeshCollider>() == null)
                    Ins.AddComponent<MeshCollider>();
            }
            int count = Ins.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                Renderer renderer = Ins.transform.GetChild(i).GetComponent<Renderer>();
                if (renderer != null)
                {
                    if (renderer.gameObject.GetComponent<MeshCollider>() == null)
                        renderer.gameObject.AddComponent<MeshCollider>();
                }
            }
        }
    }

    /*******************************************************************************************
     * 功能 : 为单位计算AABB包围体
     * 编译器使用
     **********************************************************************************************/
    public BoxCollider ComputeBounds()
    {
        if (Ins == null)
            return null;
        MeshRenderer[] renderers = Ins.GetComponentsInChildren<MeshRenderer>();

        if (renderers.Length < 1)
            return null;

        Bounds mainBounds = new Bounds();

        for (int i = 0; i < renderers.Length; i++)
        {
            Bounds bounds = renderers[i].bounds;

            if (i == 0)
                mainBounds = bounds;
            if (i > 0)
                mainBounds.Encapsulate(bounds);
        }

        Vector3 min = Ins.transform.InverseTransformPoint(mainBounds.min);
        Vector3 max = Ins.transform.InverseTransformPoint(mainBounds.max);

        mainBounds.min = min;
        mainBounds.max = max;

        BoxCollider bc = Ins.AddComponent<BoxCollider>();
        bc.center = mainBounds.center;
        bc.size = mainBounds.size;

        center = Ins.transform.localToWorldMatrix.MultiplyPoint3x4(mainBounds.center);

        return bc;
    }

    /*******************************************************************************************
     * 编译器使用
     * 功能 ： 合并单位
     *******************************************************************************************/
    public void CombineUnits(List<GameObjectUnit> units)
    {
        int i = 0;
        combinUnitIDs.Clear();
        for (i = 0; i < units.Count; i++)
        {
            combinUnitIDs.Add(units[i].createID);
            units[i].combineParentUnitID = this.createID;
            units[i].active = false;
            scene.RemoveUnit(units[i]);
        }
    }

    /*******************************************************************************************
     * 编译器使用
     * 功能 ： 分解单位
     *******************************************************************************************/
    public bool DeCombineUnits()
    {
        if (combinUnitIDs.Count < 1)
            return false;

        int i = 0;
        for (i = 0; i < combinUnitIDs.Count; i++)
        {
            GameObjectUnit unit = GameScene.mainScene.FindUnitInTiles(combinUnitIDs[i]);
            if (unit != null)
            {
                unit.combineParentUnitID = -1;
                // unit.Visible();
            }
            else
            {
                return false;
            }
        }
        combinUnitIDs.Clear();
        return true;
    }

    /*******************************************************************************************
     * 功能 ： 动态插入游戏对象单元到场景
     * 编译器使用
     *******************************************************************************************/
    static public GameObjectUnit Create(GameScene scene, Vector3 pos, int createID, string prePath)
    {
        GameObjectUnit unit = new GameObjectUnit(createID);
        unit.scene = scene;
        // 读取游戏对象的位置信息
        unit.Position = pos;
        unit.center = pos;
        unit.prePath = prePath;

        // 更新可视范围
        unit.UpdateViewRange();

        return unit;
    }

    /*****************************************************************************************
     * 编译器使用
     * 功能 : 计算单位所属的切片
     *****************************************************************************************/
    public void ComputeTiles()
    {
        ClearTiles();
        mainTile = scene.GetTileAt(Position);

        // 查找单元的归属 ------------------------------------------------------------
        if (mainTile != null)
        {
            // 计算主属
            AddTile(mainTile);

            if (Ins != null)
            {
                BoxCollider bc = ComputeBounds();

                //不需要自动计算剔除因子
                if (!mbManualCullingFactor)
                {
                    // 重新计算剔除因子
                    if (bc != null)
                        cullingFactor = bc.bounds.size.magnitude;
                    if (cullingFactor <= 0.01f)
                        cullingFactor = scene.terrainConfig.defautCullingFactor;
                }

                UpdateViewRange();


                // 计算所属
                for (int i = 0; i < scene.tiles.Count; i++)
                {
                    Tile tile = scene.tiles[i];
                    if (bc != null)
                    {
                        if (bc.bounds.Intersects(tile.bounds))
                            AddTile(tile);
                    }
                }
                if (bc != null)
                    GameObject.DestroyImmediate(bc);
            }
        }

        if (tiles.Count < 1)
            LogSystem.Log("该单位没有所属切片, 无法进行存储! 单位编号[" + this.createID + "]");
    }

    /****************************************************************************************************************
     * 功能 : 收集材质
     * 编译器
     ****************************************************************************************************************/
    public void CollectMaterials()
    {
        materials = new List<Material>();

        if (Ins.GetComponent<Renderer>() != null)
        {
            // 避免美术有些渲染器材质球丢失
            if (Ins.GetComponent<Renderer>().sharedMaterial != null)
                materials.Add(Ins.GetComponent<Renderer>().sharedMaterial);
        }

        int count = lightmapPrototypes.Count;

        for (int i = 0; i < count; i++)
        {
            LightmapPrototype pro = lightmapPrototypes[i];
            Renderer renderer = null;
            if (pro.rendererChildIndex > -1)
            {
                if (pro.rendererChildIndex < Ins.transform.childCount)
                    renderer = Ins.transform.GetChild(pro.rendererChildIndex).GetComponent<Renderer>();

                if (renderer != null)
                {
                    // 避免美术有些渲染器材质球丢失
                    if (renderer.sharedMaterial != null)
                        materials.Add(renderer.sharedMaterial);
                }
            }
        }
    }
    /***********************************************************************************
    * 功能 : 追加自定义格子数据到单位
    *************************************************************************************/
    private int gridCount = 0;
    private int maxGridCount = 2000;
    public void AppendGrid(int gridX, int gridY)
    {
        int i = 0;
        if (grids == null)
        {
            ResetGrids();
        }
        for (i = 0; i < gridCount; i++)
        {
            if (grids[i, 0] == gridX && grids[i, 1] == gridY)
                return;
        }
        grids[gridCount, 0] = gridX;
        grids[gridCount, 1] = gridY;
        gridCount++;
    }

    public void ResetGrids()
    {
        _grids = new int[maxGridCount, 2];
        int[,] grids2 = new int[222, 111];
        int i = 0;
        LogSystem.Log(" : maxGridCount - " + maxGridCount + " grids : " + grids2.GetLength(0) + "-" + grids2.GetLength(1));

        try
        {

            for (i = 0; i < maxGridCount; i++)
            {
                _grids[i, 0] = 0;
                _grids[i, 1] = 0;
            }
        }
        catch (Exception e)
        {
            LogSystem.Log("err : index - " + i + " grids : " + grids.GetLength(0) + "-" + grids.GetLength(1));
        }
    }

    public int[,] GetCleanGrids()
    {
        int[,] cleanGrids = new int[gridCount, 2];
        for (int i = 0; i < gridCount; i++)
        {
            cleanGrids[i, 0] = grids[i, 0];
            cleanGrids[i, 1] = grids[i, 1];
        }
        return cleanGrids;
    }
#endif
    #endregion
}