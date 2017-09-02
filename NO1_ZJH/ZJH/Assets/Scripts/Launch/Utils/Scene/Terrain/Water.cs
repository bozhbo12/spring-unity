using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

/***********************************************************************************************
 * 类 : 水面
 * 注解 : 所有水面的顶点、法线、三角索引及UV都是同样的
 ***********************************************************************************************/
public class Water : MonoBehaviour
{
    public float[,] depthMap;

    /** 地形网格数据 */
    static public Mesh mesh;

    /** 地形当前的顶点 */
    public Vector3[] vertices;
    /** 水域法线 */
    public Vector3[] normals;
    /** 地形当前的UV */
    public Vector2[] uvs;


    /** 地形三角形 */
    public int[] triangles;

    public int segmentsW = 16;
    public int segmentsH = 16;

    private Material matrial;

    public MeshRenderer waterRenderer;

    /** 环境采样器 */
    public AmbienceSampler ambienceSampler;

    public WaterData waterData;

    /***************************************************************************************
     * 功能：创建新的地形
     ***************************************************************************************/
    public Water()
    {

    }

    /*****************************************************************************************
     * 功能 ： 销毁水面
     *******************************************************************************************/
    public void Destroy()
    {
        this.waterData.Release();
        this.waterData = null;

        this.uvs = null;
        this.vertices = null;
        this.normals = null;
        this.triangles = null;

        this.waterRenderer.material = null;
        this.waterRenderer = null;

        // 销毁材质
        GameObject.Destroy(matrial);
        matrial = null;

        // 销毁MESH
        GameObject.Destroy(mesh);
        mesh = null;
    }

    /****************************************************************************************
     * 功能 ： 从地形数据中创建地形对象
     ****************************************************************************************/
    static public Water CreateWaterGameObject()
    {
        GameObject go = new GameObject();
        Water water = go.AddComponent<Water>();
        go.isStatic = false;                            // 水面是动态单位，不进行烘焙

        // 创建共享的几何体模型
        if (mesh == null)
            water.BuildShareWaterMesh();

        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        water.waterRenderer = go.AddComponent<MeshRenderer>();
        water.waterRenderer.castShadows = false;
        water.waterRenderer.receiveShadows = false;
        water.gameObject.layer = GameLayer.Layer_Water;         //
        water.waterData = new WaterData();

        // water.BuildMaterial();

        return water;
    }

    /*****************************************************************************************
     * 功能 ：通过水面数据创建水面
     *****************************************************************************************/
    static public Water CreateWaterGameObject(WaterData waterData)
    {
        Water water = CreateWaterGameObject();
        water.waterData = waterData;


        water.BuildMaterial();
        return water;
    }


    /*****************************************************************************************
     * 功能 ： 创建共享的水面平面几何体
     *****************************************************************************************/
    public void BuildShareWaterMesh()
    {
        BuildGeometry();
        BuildUVs();
    }

    /*****************************************************************************************
     * 功能 ： 创建几何体
     *****************************************************************************************/
    public void BuildGeometry()
    {
        TerrainConfig terCfg = GameScene.mainScene.terrainConfig;

        if (mesh == null)
        {
            mesh = new Mesh();
        }
        int _segmentsW = segmentsW;
        int _segmentsH = segmentsH;

        float x, z, y;
        int numInds = 0;
        int baseInd;
        int tw = _segmentsW + 1;
        int numVerts = (_segmentsH + 1) * tw;

        vertices = new Vector3[numVerts];
        triangles = new int[_segmentsH * _segmentsW * 6];
        normals = new Vector3[numVerts];

        numVerts = 0;

        for (int zi = 0; zi <= _segmentsH; ++zi)
        {
            for (int xi = 0; xi <= _segmentsW; ++xi)
            {
                // -.5f 以中心点为顶点原点
                x = ((float)xi / (float)_segmentsW - 0.5f) * terCfg.tileSize;
                z = ((float)zi / (float)_segmentsH - 0.5f) * terCfg.tileSize;
                y = 0;

                vertices[numVerts] = new Vector3(x, y, z);

                numVerts++;

                if (xi != _segmentsW && zi != _segmentsH)
                {
                    baseInd = xi + zi * tw;
                    triangles[numInds++] = baseInd;
                    triangles[numInds++] = baseInd + tw;
                    triangles[numInds++] = baseInd + tw + 1;
                    triangles[numInds++] = baseInd;
                    triangles[numInds++] = baseInd + tw + 1;
                    triangles[numInds++] = baseInd + 1;
                }
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        // 创建地形碰撞体
        if (Application.isEditor)
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

    /*****************************************************************************************
     * 功能 ：创建UV
     *****************************************************************************************/
    public void BuildUVs()
    {
        int numUvs = (segmentsH + 1) * (segmentsW + 1);
        uvs = new Vector2[numUvs];

        numUvs = 0;
        for (int yi = 0; yi <= segmentsH; ++yi)
        {
            for (int xi = 0; xi <= segmentsW; ++xi)
            {
                uvs[numUvs++] = new Vector2(((float)xi / (float)segmentsW), (1f - (float)yi / (float)segmentsH));
            }
        }
        mesh.uv = uvs;
    }

	static private Shader waterShader = Shader.Find("Snail/Water");

    /**************************************************************************************************
     * 功能 : 创建材质
     ***************************************************************************************************/
    public void BuildMaterial()
    {
        TerrainConfig terCifg = GameScene.mainScene.terrainConfig;

        // 创建地形材质
        if (matrial == null)
        {
			matrial = new Material(waterShader);
            GetComponent<Renderer>().material = matrial;
        }

        // 设置深度纹理
        matrial.SetTexture("_DeepTex", waterData.depthMap);
        matrial.SetTexture("_BumpMap", waterData.bumpMap);

        /**
        if (GameScene.mainScene.sceneID == 1200010)
        {
            matrial.SetVector("_SunLightDir", new Vector3(0.76f, 0.44f, -0.43f));
            matrial.SetFloat("_WaterSpecStrength", 1.316f);
            matrial.SetFloat("_WaterSpecRange", 0.8f);
        }
        else if (GameScene.mainScene.sceneID == 1200009)
        {
            matrial.SetVector("_SunLightDir", new Vector3(1.38f, 0.08f, 1.22f));
            matrial.SetFloat("_WaterSpecRange", 1f);
            matrial.SetFloat("_WaterSpecStrength", 0.665f);
        }
        else if (GameScene.mainScene.sceneID == 1100005)
        {
            matrial.SetVector("_SunLightDir", new Vector3(0.17f, 0.72f, -0.43f));
            matrial.SetFloat("_WaterSpecRange", 0.1f);
            matrial.SetFloat("_WaterSpecStrength", 0.263f);
        }
        else if (GameScene.mainScene.sceneID == 1100012)
        {
            matrial.SetVector("_SunLightDir", new Vector3(5.7f, -6.7f, 27.8f));
            matrial.SetFloat("_WaterSpecStrength", 0.6f);
            matrial.SetFloat("_WaterSpecRange", 0.074f);
        }
        else
        {
            matrial.SetVector("_SunLightDir", terCifg.sunLightDir);
            matrial.SetFloat("_WaterSpecRange", terCifg.waterSpecRange);
            matrial.SetFloat("_WaterSpecStrength", terCifg.waterSpecStrength);
        }
        **/
        //matrial.SetVector("_SunLightDir", terCifg.sunLightDir);
        //matrial.SetFloat("_WaterSpecRange", terCifg.waterSpecRange);
        //matrial.SetFloat("_WaterSpecStrength", terCifg.waterSpecStrength);
        // 从默认水面设置中读取参数
        matrial.SetFloat("_WaveScale", waterData.waveScale /**0.05f**/);
        matrial.SetTexture("_BumpMap", waterData.bumpMap);
        matrial.SetTexture("_ColorControl", waterData.colorControlMap);
        matrial.SetColor("_horizonColor", waterData.horizonColor);
        matrial.SetFloat("_RefrDistort", waterData.refrDistort);
        matrial.SetFloat("_Alpha", waterData.alpha);

        matrial.SetVector("_SunLightDir", terCifg.sunLightDir);
        matrial.SetFloat("_WaterSpecStrength", terCifg.waterSpecStrength);
        matrial.SetFloat("_WaterSpecRange", terCifg.waterSpecRange);
    }


    /*******************************************************************************************
     * 功能 : 更新水面深度
     ********************************************************************************************/
    public void UpdateDepthMap()
    {
        TerrainConfig terCfg = GameScene.mainScene.terrainConfig;

        int heightmapResolution = 64;
        int i = 0;
        int j = 0;
        float terHeight = 0f;

        depthMap = new float[heightmapResolution, heightmapResolution];

        Vector3 origin = new Vector3(0f, 500, 0f);
        RaycastHit hit;
        Ray ray = new Ray();
        ray.direction = Vector3.down;

        float step = (float)terCfg.tileSize / (float)heightmapResolution;
        // 高度图数据
        for (i = 0; i < heightmapResolution; i++)
        {
            for (j = 0; j < heightmapResolution; j++)
            {
                origin.x = i * step + transform.position.x - terCfg.tileSize * 0.5f;
                origin.z = j * step + transform.position.z - terCfg.tileSize * 0.5f;
                ray.origin = origin;
                // 
                Physics.Raycast(ray, out hit, 2000, GameLayer.Mask_Ground);
                if (hit.transform != null)
                    terHeight = hit.point.y;
                float diff = waterData.height - terHeight;
                depthMap[i, j] = (diff / waterData.waterVisibleDepth);
            }
        }

        // 更新深度纹理
        waterData.depthMap = new Texture2D(heightmapResolution, heightmapResolution, TextureFormat.ARGB32, false);
        waterData.depthMap.wrapMode = TextureWrapMode.Clamp;
        for (i = 0; i < heightmapResolution; i++)
        {
            for (j = 0; j < heightmapResolution; j++)
            {
                float val = depthMap[i, j];
                waterData.depthMap.SetPixel(i, heightmapResolution - j - 1, new Color(val, val, val, val));
            }
        }
        waterData.depthMap.Apply();
    }


    /******************************************************************************************************************
     * 功能 : 判断位置是否在水面中
     ******************************************************************************************************************/
    public bool Underwater(float height)
    {
        if (height < (waterData.height - waterData.waterDiffValue))
            return true;
        else
            return false;
    }

    /******************************************************************************************************************
     * 功能 : 强制更新,当水面不更新的时候
     ******************************************************************************************************************/
    public void ForcedUpdate()
    {
        //Update();
    }

    /*********************************************************************************************************************
     * 功能 : 水面更新
     *********************************************************************************************************************/
    /**
    void Update()
    {
    		
        if (GameScene.mainScene == null)
            return;

        if (!GetComponent<Renderer>())
            return;

        Material mat = GetComponent<Renderer>().sharedMaterial;
        if (!mat)
            return;

        float waveScale = mat.GetFloat("_WaveScale");
        float t = 0f;
        t = GameScene.mainScene.time;

        Vector4 offset4 = waterData.waveSpeed * (t * waveScale);

        // 设置水波偏移在(0-1)之间循环
        Vector4 offsetClamped = new Vector4(Mathf.Repeat(offset4.x, 1.0f), Mathf.Repeat(offset4.y, 1.0f), Mathf.Repeat(offset4.z, 1.0f), Mathf.Repeat(offset4.w, 1.0f));
        mat.SetVector("_WaveOffset", offsetClamped);

        // 更新水面光照(编辑器中实时更新)
        if (GameScene.isPlaying == false)
        {
            mat.SetVector("_SunLightDir", GameScene.mainScene.terrainConfig.sunLightDir);
            mat.SetFloat("_WaterSpecStrength", GameScene.mainScene.terrainConfig.waterSpecStrength);
            mat.SetFloat("_WaterSpecRange", GameScene.mainScene.terrainConfig.waterSpecRange);
        }
    }
    **/
}
