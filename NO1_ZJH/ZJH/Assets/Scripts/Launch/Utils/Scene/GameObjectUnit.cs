using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
using System.IO;

/*******************************************************************************************************
 * �� �� ��Ϸ����Ԫ���ݣ��������ݱ���
 * ע ����̬��λ���������Զ���
 *      ���������Ĺ�����ͼ����������ͼ���ӵ�������û��Ӧ�õ�ʱ�򽫹�����ͼ�ӳ�����ɾ��
 *******************************************************************************************************/
public class GameObjectUnit
{
    public bool isMainUint = false;

#region �������ű�
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

    /** ��Ԫ������ */
    public string name = "";

    /** �������ŵİ�Χ��ߴ�, �Ѿ����� */
    public Vector3 size = Vector3.zero;

    /** �ϲ���λ�б� */
    public List<int> combinUnitIDs = new List<int>();

    /** ������Ԫ����Ƭ */
    public Tile mainTile;

    /** ��¼��ǰ��λʹ�õĲ��� */
    public List<Material> materials = new List<Material>();

    /// <summary>
    /// �Ƿ��ֶ������߳�����
    /// </summary>
    public bool mbManualCullingFactor = false;
#endif
#endregion

    public const int genRippleDelayTick = 50;

    /** ʵ������������ */
    public delegate void CreateInsListener(GameObjectUnit unit);
    public CreateInsListener createInsListener;

    /** ʵ�����ټ����� */
    public delegate void DestroyInsListener();
    public DestroyInsListener destroyInsListener;

    public delegate void ActiveListener(bool value);
    public ActiveListener activeListener;

    /** ��Ϸ���󴴽���ID */
    public int createID = 0;

    /** �Ƿ�����ײ���� ��ײ��������������, �������ƶ����󲻽�����ײ��� */
    public bool isCollider = true;

    /** ��Ԫ��Ⱦʵ�� */
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


    /** �Ƿ��Ǿ�̬��Ԫ, ��̬��Ԫ����ʱ�����������Ӽ��㡢�ռ���� */
    public bool isStatic = true;

    /** ������Ԫ��Ԥ������Դ·��������Ŀ¼�е�·�� */
    public string prePath = "";
	public string prePath2= string.Empty;
    /** ��Ԫ��ת����Ϣ */
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
    /// �޳�����(������)
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



    /** ��Ϸ�����������Դ·�� */
    private int dependResCount = 0;

    public bool visible = false;

    private float _naar = 0f;
    /** �޳����뷶Χ */
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
            //    LogSystem.Log("��̬��_naar:" + value);
            //}
            //else
            //{
            //    LogSystem.Log("��̬ _naar:" + value);
            //}
            _naar = value;
        }
    }
    
    public float far = 0f;

    public float viewDistance = 0f;

    public float viewAngle = 0f;

    /** ��ǰ��Ԫռ�õ���Ƭ */
    public List<Tile> tiles = new List<Tile>();

    /** ��ǰ��λ�Ƿ�ռ�˸��� */
    public bool hasCollision = false;

    /************************************************************************
     * ���� �� ��ײ���, ����������ײ, ����������ռ�ĸ��ӳߴ�
     * ------------
     * |    ��    |
     * |��radius��|
     * |    ��    |
     * ------------
     * ��������ݸ���ռ��������뾶, �ȼ����ΧȦ����ײ���ڼ�����ϸ���ӵ���ײ    
     ************************************************************************/
    public float radius = 1f;

    /** ��ײ���ӳߴ� */
    public int collisionSize = 1;

    /** ������ռ���� */
    private int[,] _grids;

    /** ������ռ���� */
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
     * ���� ����λ��Ⱦ�������Ĺ�����ͼλ��
     ******************************************************************************/
    public List<LightmapPrototype> lightmapPrototypes = new List<LightmapPrototype>();

    /** �Ƿ���Ҫ��Ļ���� */
    public bool needScreenPoint = true;

    /** ��Ļ����λ�� */
    public Vector3 screenPoint;

    /** ���ʰȡ�ж� */
    public bool mouseEnable = true;

    /** ��λ����, Ĭ���� */
    public int type = UnitType.UnitType_General;

    public UnitParser unitParser;


    /** ��λ�Ƿ�������Ч�� */
    public bool genRipple = false;
    protected Vector3 ripplePos;


    public GameScene scene;

    /** ��λ�Ƿ���ʾ���� */
    public bool active = false;

    protected Vector3 scenePoint = Vector3.zero;
    public float scenePointBias = 1f;

    /** �ж��Ƿ���Ҫ���õ�ǰ�߶� */
    public bool needSampleHeight = true;


    /** �ϲ��󸸼���λID */
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
     * ���� �� �Ӵ洢�ļ��ж�ȡ��Ԫ��Ϣ
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

            // �ڱ༭״̬�½��д�����
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

            // ��Ϸ����ʱ, �����ƹ��ȡ
            if (GameScene.isPlaying == true && type == UnitType.UnitType_Light && scene.lightDataLength > 0)
            {
                br.BaseStream.Position += scene.lightDataLength;
                return;
            }

            if (type == UnitType.UnitType_Light)
                lightPos = br.BaseStream.Position;

            // ������λ����
            unitParser.Read(br);
        }
        else
        {
            br.BaseStream.Position = pos;
            unitParser = new UnitParser();      // ����Ϊ��ͨ�ĵ�λ����
        }

        // ��ȡ��Ϸ����������Դ����
        dependResCount = br.ReadInt32();
        for (i = 0; i < dependResCount; i++)
        {
            br.ReadInt32();
        }

        // ��ȡ�決����
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

        // ��ȡ��Ϸ�����λ����Ϣ
        Position = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        Rotation = new Quaternion(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        localScale = new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

        // ��ȡ�ϲ���Ϣ
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
                // �������ٺ��ظ�����ʱ����
                if (combinUnitIDs == null)
                    combinUnitIDs = new List<int>(count);
                for (i = 0; i < count; i++)
                {
                    this.combinUnitIDs.Add(br.ReadInt32());
                }
            }
#else
            //�����ֽ�����������
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


        //�Ƿ��ֶ������߳���Χ
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

        // ��ȡ�޳�LOD����
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
     * ���� : ��λY����ת�Ƕȣ������ǻ���ֵ
     * ע�� : �������Ƿ������ı�Y����ת
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
    * ���� : ��λY����ת�Ƕ�,�����ǽǶ�ֵ
    * ע�� : �������Ƿ������ı�Y����ת
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
     * ���� �� ��̬��λ�ĸ���
     *************************************************************************/
    virtual public void Update()
    {
#if UNITY_EDITOR
        // ��λ���´��� -----------------------------------------------------------------------
        if (Ins != null)
        {
            if (GameScene.isPlaying == false)
            {
                // ��¼�༭���о�̬��λ��λ��
                if (isStatic == true)
                {
                    // λ�÷����ı����¼��㵥λ����
                    if ((Mathf.Abs(InsPosition.x - Position.x) > 0.01f) || (Mathf.Abs(InsPosition.z - Position.z) > 0.01f))
                        ComputeTiles();

                    Position = InsPosition;
                    Rotation = InsRotation;
                    localScale = Ins.transform.localScale;

                    // �洢��λ������
                    unitParser.Update(Ins);
                }
            }
        }
#endif
    }

    /************************************************************************************
     * ���� : ��������Դ��ȡ
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
        //��ɾ�� || ����ʾ
        if (destroyed || !visible)
        {
            //�����泡��ʵ��
            CacheObjects.PopCache(oAsset);
            return;
        }

        // ��ʼ������
        Initialize(oAsset);

        //���̬��λ
        ActiveDynaUnit();
    }


    /// <summary>
    /// ��Դ����
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
    /// ����ʵ����
    /// </summary>
    private void Initialize(UnityEngine.Object oAsset)
    {
        if (destroyed == true)
            return;

        if (Ins != null)
            return;

        // ʵ��������
        Ins = CacheObjects.InstantiatePool(oAsset) as GameObject;

        // �༭��״̬�������жϵ�λ����,����༭���޸ĵ�λ����
        if (GameScene.isPlaying == false)
        {
            type = UnitType.GetType(Ins.layer);
        }

        if (isStatic == false && needSampleHeight == true)
            SetPostionY(scene.SampleHeight(Position));

        InsPosition = Position;

        OnInitialize();

        // �������������༭
        Renamme();
    }

    /// <summary>
    /// ��ʵ�л�
    /// </summary>
    protected virtual void OnInitialize()
    {
        InsRotation = Rotation;
        Ins.transform.localScale = localScale;

        // ��Ⱦ����ȡ������ͼ(�޹�����ͼʱ��������)
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
        #region �������ű�

#if UNITY_EDITOR
        // �༭��ѡ��
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
                // ��Ϸ�Ѿ��к決��ͼ��Ͷ����Ӱ
                //renderers[j].receiveShadows = true;
                //renderers[j].castShadows = true;

                for (int i = 0; i < renderers[j].sharedMaterials.Length; i++)
                {
                    Material mt = renderers[j].sharedMaterials[i];

                    // ͳ�Ʋ���
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

        // �ڱ༭ģʽ�¼���LOD����, Ϊ�˷���༭����ʰȡ
        if (GameScene.isPlaying == false)
        {
            // �ռ�����,ͳ����Ϣ
            CollectMaterials();

            // ׷��MESH��ײ�����,��������ʰȡ
            AddMeshRenderColliders();

            if (cullingFactor <= 0.01f)
                cullingFactor = scene.terrainConfig.defautCullingFactor;

            // ��̬��λ����Ҫ����������Ƭ
            if (isStatic == true)
                ComputeTiles();
        }
#endif
        #endregion
    }

    
    /// <summary>
    /// ���̬��λ
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
    /// �����������
    /// </summary>
    /// <param name="bValue"></param>
    protected virtual void OnActiveUnit(bool bValue)
    {

    }


    /*******************************************************************************************************
     * ���� : ������ʾ
     * ע����̬������ʾ���ص�
     *  �� ��̬������Ҫ�ص�
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
     * ���� : ���¶�������
     ****************************************************************************************************************/
    public void Renamme()
    {
        if (GameScene.isPlaying == false)
        {
            // ��ʾ�����ļ����Ƽ��決���ŵ� ʵ������
            string[] split = prePath.Split(SplitChars.chBS);
            Ins.name = split[split.Length - 1] + "_Unit_" + createID;
        }
        else
        {
            Ins.name = "Unit_" + createID;
        }
    }

    /** ��������Ƭ���� */
    public void Invisible()
    {
        if (visible == true)
        {
            if (Ins != null)
            {
                // ��̬��λ�Ȼ���,���ͷ�
                Ins.SetActive(false);
                // ��̬��λ�����
                OnActiveUnit(false);
                if (isStatic)
                {
                    //��̬�������ؼ�ɾ��.��̬�����ɾ���ص���ɾ��
                    CacheObjects.DestoryPoolObject(Ins);
                    Ins = null;
                }
            }
        }
        visible = false;
    }

    /**************************************************************************************
     * ���� �� ��λ��������
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
                LogSystem.LogError("����������λ�����г���!" + e.ToString());
            }
        }

        if (Ins != null)
        {
            //ֱ��ɾ����û��Ҫ���û����
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
     * ���� : �����޳�����  
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

    #region ����������

#if UNITY_EDITOR

    //������ʹ��
    public void ClearTiles()
    {
        for (int i = 0; i < tiles.Count; i++)
        {
            tiles[i].RemoveUnit(this);
        }
        tiles.Clear();
    }

    /// <summary>
    /// ������ʹ��
    /// </summary>
    /// <param name="tile"></param>
    public void AddTile(Tile tile)
    {
        tile.AddUnit(this);
        if (tiles.Contains(tile) == false)
            tiles.Add(tile);
    }

    /**********************************************************************************
     * ���� : ��MESH��Ⱦ��������ײ��,�����ڱ༭����ʰȡ
     * ������ʹ��
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
     * ���� : Ϊ��λ����AABB��Χ��
     * ������ʹ��
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
     * ������ʹ��
     * ���� �� �ϲ���λ
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
     * ������ʹ��
     * ���� �� �ֽⵥλ
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
     * ���� �� ��̬������Ϸ����Ԫ������
     * ������ʹ��
     *******************************************************************************************/
    static public GameObjectUnit Create(GameScene scene, Vector3 pos, int createID, string prePath)
    {
        GameObjectUnit unit = new GameObjectUnit(createID);
        unit.scene = scene;
        // ��ȡ��Ϸ�����λ����Ϣ
        unit.Position = pos;
        unit.center = pos;
        unit.prePath = prePath;

        // ���¿��ӷ�Χ
        unit.UpdateViewRange();

        return unit;
    }

    /*****************************************************************************************
     * ������ʹ��
     * ���� : ���㵥λ��������Ƭ
     *****************************************************************************************/
    public void ComputeTiles()
    {
        ClearTiles();
        mainTile = scene.GetTileAt(Position);

        // ���ҵ�Ԫ�Ĺ��� ------------------------------------------------------------
        if (mainTile != null)
        {
            // ��������
            AddTile(mainTile);

            if (Ins != null)
            {
                BoxCollider bc = ComputeBounds();

                //����Ҫ�Զ������޳�����
                if (!mbManualCullingFactor)
                {
                    // ���¼����޳�����
                    if (bc != null)
                        cullingFactor = bc.bounds.size.magnitude;
                    if (cullingFactor <= 0.01f)
                        cullingFactor = scene.terrainConfig.defautCullingFactor;
                }

                UpdateViewRange();


                // ��������
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
            LogSystem.Log("�õ�λû��������Ƭ, �޷����д洢! ��λ���[" + this.createID + "]");
    }

    /****************************************************************************************************************
     * ���� : �ռ�����
     * ������
     ****************************************************************************************************************/
    public void CollectMaterials()
    {
        materials = new List<Material>();

        if (Ins.GetComponent<Renderer>() != null)
        {
            // ����������Щ��Ⱦ��������ʧ
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
                    // ����������Щ��Ⱦ��������ʧ
                    if (renderer.sharedMaterial != null)
                        materials.Add(renderer.sharedMaterial);
                }
            }
        }
    }
    /***********************************************************************************
    * ���� : ׷���Զ���������ݵ���λ
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