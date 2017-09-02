using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;


/***********************************************************************************************
 * 类 : 涟漪
 ***********************************************************************************************/
public class Ripple : MonoBehaviour
{
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

    public int segmentsW = 1;
    public int segmentsH = 1;

    private Material matrial;

    public MeshRenderer rippleRenderer;

    private float tick = 0;

    /** 涟漪生命周期 */
    public float life = 70;
    static public List<Ripple> destroyRipples = new List<Ripple>();

    /***************************************************************************************
     * 功能：涟漪效果
     ***************************************************************************************/
    public Ripple()
    {

    }

    /****************************************************************************************
     * 功能 ： 创建涟漪对象
     ****************************************************************************************/
    static public Ripple CreateRippleGameObject(Vector3 worldPostion)
    {
        GameObject ins;
        Ripple ripple = null;
        if (destroyRipples.Count > 0)
        {
            ripple = destroyRipples[0];
            ins = ripple.gameObject;
            ripple.gameObject.SetActive(true);
            destroyRipples.RemoveAt(0);
        }
        else
        {
            ins = new GameObject();
            ripple = ins.AddComponent<Ripple>();
            ins.isStatic = false;                            // 水面是动态单位，不进行烘焙
            ins.name = "Ripple";
            // 创建共享的几何体模型
            if (mesh == null)
                ripple.BuildShareWaterMesh();

            ins.AddComponent<MeshFilter>().sharedMesh = mesh;
            ripple.rippleRenderer = ins.AddComponent<MeshRenderer>();
            ripple.rippleRenderer.castShadows = false;
            ripple.rippleRenderer.receiveShadows = false;
            ripple.gameObject.layer = GameLayer.Layer_Water;         //
            ripple.BuildMaterial();
        }

        //worldPostion.x += UnityEngine.Random.value * 0.5f;
        //worldPostion.z += UnityEngine.Random.value * 0.5f;
        ins.transform.position = worldPostion;

        ripple.Reset();

        return ripple;
    }

    /** 涟漪效果重置 */
    public void Reset()
    {
        scale = 1f + UnityEngine.Random.value * 0.2f;
        tick = 0;
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
                x = ((float)xi / (float)_segmentsW - .5f) * 1.5f;
                z = ((float)zi / (float)_segmentsH - .5f) * 1.5f;
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


    /**************************************************************************************************
     * 功能 : 创建材质
     ***************************************************************************************************/

    private Shader shader = Shader.Find("Snail/Ripple");

    public void BuildMaterial()
    {
        // 创建地形材质
        matrial = new Material(shader);
        GetComponent<Renderer>().material = matrial;

        Texture2D tex = Resources.Load("Textures/Shader/ripple", typeof(Texture2D)) as Texture2D;

        // 设置深度纹理
        matrial.SetTexture("_MainTex", tex);

        // 设置渲染材质
        this.rippleRenderer.material = matrial;
    }


    private float waveSpeed = 0.1f;
    private Vector4 waveScale = new Vector4(0.1f, 0.2f, 0.3f, 0.4f);
    private float scale = 1f;

    /*********************************************************************************************************************
     * 功能 : 涟漪更新
     *********************************************************************************************************************/
    void Update()
    {
        if (!GetComponent<Renderer>())
            return;

        // 生命结束后回收涟漪
        if (tick > life)
        {
            Reset();
            this.gameObject.SetActive(false);
            destroyRipples.Add(this);
            return;
        }

        Material mat = GetComponent<Renderer>().sharedMaterial;
        if (!mat)
            return;

        scale += 0.08f;
        // 涟漪缩放偏移
        mat.SetFloat("_Scale", scale);
        mat.SetVector("_Color", new Color(0.5f, 0.5f, 0.5f, (life - tick) / life));

        tick += 2;
    }

}
