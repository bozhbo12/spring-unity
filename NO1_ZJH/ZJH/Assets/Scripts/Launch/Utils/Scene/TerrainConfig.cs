using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*************************************************************************************************
 * 功能 ： 地形配置
 *************************************************************************************************/
public class TerrainConfig
{
    /** 场景尺寸 */
    public int sceneWidth = 480;
    public int sceneHeight = 480;

    public int reginRows = 3;
    public int reginColumns = 3;
    /** 场景高度图分辨率尺寸 */
    public int sceneHeightmapResolution = 480;

#if UNITY_EDITOR
    /** 基础地形纹理, 创建场景时必须存在的纹理 */
    public Splat baseSplat = new Splat();
#endif

    /** 编辑器中摄像机位置和角度 */
    public Vector3 cameraLookAt = Vector3.zero;

	public float cameraFarClip = 0f;

    public float cameraDistance = 0f;

    public float cameraRotationX = 0f;
    public float cameraRotationY = 0f;
    public float cameraRotationZ = 0f;


    /** 全局雾效设置 */
    public Color fogColor = new Color(68f / 255f, 208f / 255f, 1f);
    public float startDistance = 6f;
    public float globalDensity = 1f;
    public float heightScale = 1f;
    public float height = 110f;
    public Vector4 fogIntensity = new Vector4(1f, 1f, 1f, 1f);

    public int gridCountPerUnit = 1;

    /** 切块尺寸 */
    public int tileSize = 32;

	public int tileRange = -1;

    /** 地域每边的切片数量 */
    public int tileCountPerSide = 0;

    /** 每个地域包含的切片数量 */
    public int tileCountPerRegion = 0;

    /** 地域尺寸 */ 
    public int regionSize = 160;

    /** 切片地形高度图分辨率 */
    public int heightmapResolution = 32;                  

    /** 水面深度图分辨率 */
    public int waterDepthmapResolution = 64;

    /** 切片格子分辨率 */
    public int gridResolution = 32;

    /** 溅斑图分辨率 */
    public int splatmapResolution = 32;
    public int sceneSplatsmapWidth = 0;

    public int sceneSplatsmapHeight = 0;

    /** 溅斑层数 */
    public int spaltsmapLayers = 4;

    /** 阻塞计算高度差 */
    public float blockHeight = 1f;
    public float maxReachTerrainHeight = 200f;

    /** 初始格子尺寸，细分后格子尺寸0.5f */
    public float gridSize = 1f;
        
    /** 初始地形高度 */
    public float defaultTerrainHeight = 50f;

    /** 最高地形高度 */
    public float maxTerrainHeight = 200f;

    /** 切片LOD距离 */
    public float tileCullingDistance = 100f;

#if UNITY_EDITOR
    /// <summary>
    /// 单位剔除距离基数, 如果单位剔除因子为0， 则超过这个距离将被剔除
    /// 因子(乘数)
    /// </summary>
    public float unitCullingDistance = 30f;
#else
    /// <summary>
    /// 单位剔除距离基数, 如果单位剔除因子为0， 则超过这个距离将被剔除
    /// 因子(乘数)
    /// </summary>
    public float unitCullingDistance = 1f;
#endif

    /// <summary>
    /// 踢除因子
    /// </summary>
    /// <param name="fRate"></param>
    /// <param name="bImmediately"></param>
    public void SetUnitCullingDistance(float fRate, bool bImmediately)
    {
        unitCullingDistance = fRate;
        if (bImmediately)
        {
            if (GameScene.mainScene != null)
                GameScene.mainScene.UpdateStaticUnitDistance();
        }
    }


    /// <summary>
    /// 附加乘数因子
    /// 用途播CG，飞行设置
    /// </summary>
    private float unitCullingAddedValue = 1f;

    /// <summary>
    /// 因子*乘数因子
    /// </summary>
    public float unitCullingMultiplier
    {
        get
        {
            if (GameScene.isPlaying)
            {
                return unitCullingAddedValue * unitCullingDistance;
            }
            else
            {
                //编译器中不踢除场景物件
                return 100;
            }
        }
    }

    /// <summary>
    /// 踢除距离乘数
    /// </summary>
    /// <param name="value"></param>
    public void SetCullingRate(float value)
    {
        //优化
        if (Mathf.Abs(value - unitCullingAddedValue) < 1f)
        {
            return;
        }
        unitCullingAddedValue = value;
        if (GameScene.mainScene != null)
            GameScene.mainScene.UpdateStaticUnitDistance();
    }

   
    /** 剔除因子的强度值 */
    public float cullingBaseDistance = 10f;

    public float cullingAngleFactor = 3f;

    /** 体积对物体视角的影响 */
    public float viewAngleLodFactor = 2f;

    private float _dynamiCullingDistance = 15f;

    public float dynamiCullingDistance
    {
        get
        {
            return _dynamiCullingDistance;
        }
        set
        {
            _dynamiCullingDistance = value;
            if (GameScene.mainScene != null)
                GameScene.mainScene.UpdateDynamicDistance(_dynamiCullingDistance);
        }
    }
    public float defautCullingFactor = 2f;

    /** 水面设置 */
    public Vector4 sunLightDir = new Vector4(-0.41f, 0.74f, 0.18f, 0f);                     // 太阳光方向
    public float waterSpecRange = 46.3f;                                                    // 水面高光范围
    public float waterSpecStrength = 0.84f;                          // 水面高光强度

    public float waterDiffRange = 0f;                               // 水面高光范围
    public float waterDiffStrength = 0f;                            // 水面高光强度

    public Vector4 waveSpeed = new Vector4(2f, 2f, -2f, -2f);
    public float waveScale = 0.02f;                                 // 水面缩放
    public Color horizonColor;                                      // 映射天边颜色
    public Texture2D colorControl;                                  // 水面颜色控制纹理
    public Texture2D waterBumpMap;                                  // 水面法线
    public float defaultWaterHeight = 48f;
    public float waterVisibleDepth = 0.5f;                          // 水面能见深度
    public float waterAlpha = 1f;
    public float reflDistort = 0.44f;                               // 水面反射值
    public float refrDistort = 0.2f;                                // 水面折射强度


    public float waterDiffValue = 0f;

    /** 动态单位碰撞计算范围 */
    public float collisionComputeRange = 3f;

    /** 点光范围 */
    public bool enablePointLight = true;
    public float pointLightRangeMin = 2f;
    public float pointLightRangeMax = 5.6f;
    public float pointLightIntensity = 1f;
    public Color pointLightColor = new Color(1f, 1f, 1f);

    /** 角色点光源设置 */
    public Vector3 rolePointLightPostion = new Vector3(-100.12f, -12.86f, 270.2f);
    public Color rolePointLightColor = new Color(1f, 1f, 1f);
    public float rolePointLightRange = 19.7f;
    public float rolePointLightIntensity = 2.68f;

    /** 烘焙灯光冷暖色 */
    public Color coolColor = new Color(1f, 1f, 1f, 1f);
    public Color warmColor = new Color(1f, 1f, 1f, 1f);

    /**  天气 1 下雨 2 下雪 3 沙尘暴  */
    public int weather = 0;

    /** 是否显示地形  */
    public bool enableTerrain = true;

    public TerrainLODConfig terrainLODConfig = new TerrainLODConfig();

    public int bumpCount = 0;
    
    public Color terrainSpecColor = new Color(1f, 1f, 1f, 1f);
    public float terrainShininess = 0.078125f;

    public float terrainGloss = 1f;
}


public class TerrainLODConfig
{
    /**
    public float[] lodDistanceRanges = new float[4] { 30f, 90f, 130, 9999999999999999f };
    public int[] lodSegments = new int[4] { 16, 8, 4 , 2};
    public int[] lodSteps = new int[4] { 1, 2, 4 , 8};               
    public int[] lodPatchSize = new int[4] { 17, 9, 5, 3};
    **/

    public float[] lodDistanceRanges = new float[2] {60f, 9999999999999999f };
    public int[] lodSegments = new int[2] { 16, 8};
    public int[] lodSteps = new int[2] { 1, 2};               // level*2 
    public int[] lodPatchSize = new int[2] { 17, 9};
}