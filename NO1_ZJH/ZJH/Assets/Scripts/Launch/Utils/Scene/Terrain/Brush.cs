using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


/******************************************************************************************
 * 功能 ： 笔刷工具，用来修改地形高度和地形混合纹理
 ******************************************************************************************/
public class Brush
{
    /** 笔刷的尺寸 */
    public int size = 32;

    /** 笔刷强度 */
    public float[] strength;

    /**笔刷增强 */
    public float strengthen = 1f;

    /** 笔刷纹理 */
    private Texture2D m_Brush = null;

    public LODTerrain terrain;
    public TerrainData terrainData;
		
    public float splatMaxValue = 0f;

    /** 地形工具 */
    public LODTerrainTool tool;

    private Projector m_BrushProjector;

    private Texture2D m_Preview;

    public float targetHeight = 0f;

    public float targetAlpha = 1f;

    public Brush()
    {

    }

    /*****************************************************************************
     * 功能 ： 创建笔刷预览
     *****************************************************************************/
    private void CreatePreviewBrush()
    {
#if UNITY_EDITOR

		GameObject obj2 = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags ("TerrainInspectorBrushPreview", HideFlags.HideAndDontSave, typeof (Projector));
        //obj2.name = "brushProject";
        this.m_BrushProjector = obj2.GetComponent<Projector>();
        this.m_BrushProjector.enabled = true;
        this.m_BrushProjector.nearClipPlane = -1000f;
        this.m_BrushProjector.farClipPlane = 1000f;
        this.m_BrushProjector.orthographic = true;
        this.m_BrushProjector.orthographicSize = 30f;
        this.m_BrushProjector.transform.Rotate((float)90f, 0f, (float)0f);
		Material material = new Material(Shader.Find("Snail/Terrain Brush Preview"));
		material.shader.hideFlags = HideFlags.HideAndDontSave;
		material.hideFlags = HideFlags.HideAndDontSave;
		material.SetTexture("_FalloffTex", (Texture2D)UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/Editor/GameSceneEditor/Brushes/brush_cutout.png", typeof(Texture2D)));
		m_BrushProjector.material = material;
		m_BrushProjector.enabled = false;
        // 设置笔刷纹理
        this.m_BrushProjector.material = material;
		this.m_BrushProjector.enabled = false;
#endif
    }

    public void setBrushVisible(bool value)
    {
        if (m_BrushProjector != null)
            m_BrushProjector.enabled = value;
    }

    public void Dispose()
    {
        if (this.m_BrushProjector != null)
        {
            GameObject.DestroyImmediate(this.m_BrushProjector.material.shader);
            GameObject.DestroyImmediate(this.m_BrushProjector.material);
            GameObject.DestroyImmediate(this.m_BrushProjector.gameObject);
            this.m_BrushProjector = null;
        }
        GameObject.DestroyImmediate(this.m_Preview);
        this.m_Preview = null;
    }

    public Projector previewProjector
    {
        get
        {
            return this.m_BrushProjector;
        }
    }

    /******************************************************************************
     * 功能 ： 加载笔刷纹理
     ********************************************************************************/
    public bool Load(Texture2D brushTex, int size)
    {
        if (((this.m_Brush == brushTex) && (size == this.size)) && (this.strength != null))
        {
            return true;
        }
        if (brushTex != null)
        {
            float num = size;
            this.size = size;
            this.strength = new float[this.size * this.size];
            if (this.size > 3)
            {
                for (int j = 0; j < this.size; j++)
                {
                    for (int k = 0; k < this.size; k++)
                    {
                        this.strength[(j * this.size) + k] = brushTex.GetPixelBilinear((k + 0.5f) / num, ((float)j) / num).a;
                    }
                }
            }
            else
            {
                for (int m = 0; m < this.strength.Length; m++)
                {
                    this.strength[m] = 1f;
                }
            }
            GameObject.DestroyImmediate(this.m_Preview);
            this.m_Preview = new Texture2D(this.size, this.size, TextureFormat.ARGB32, false);
            this.m_Preview.hideFlags = HideFlags.HideAndDontSave;
			this.m_Preview.wrapMode = TextureWrapMode.Clamp;
            this.m_Preview.filterMode = FilterMode.Point;
            Color[] colors = new Color[this.size * this.size];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = new Color(0f, 0f, 0f, this.strength[i]);
            }
            this.m_Preview.SetPixels(0, 0, this.size, this.size, colors, 0);
            this.m_Preview.Apply();
            if (this.m_BrushProjector == null)
            {
                this.CreatePreviewBrush();
            }
            m_BrushProjector.orthographicSize = (float)size * (30f / 64f);
            // this.m_BrushProjector.material.SetTexture("_CutoutTex", m_Preview);

			// this.m_BrushProjector.material.mainTexture = m_Preview;
			// this.m_BrushProjector.material.SetTexture("_ShadowTex", brushTex);
			this.m_Brush = brushTex;
            return true;
        }
        this.strength = new float[] { 1f };
        this.size = 1;
        return false;
    }


    /********************************************************************************
     * 功能 ： 获取笔刷指定位置的强度
     ********************************************************************************/
    public float GetStrengthInt(int ix, int iy)
    {
        ix = Mathf.Clamp(ix, 0, this.size - 1);
        iy = Mathf.Clamp(iy, 0, this.size - 1);
        return this.strength[(iy * this.size) + ix];
    }

    public float GetStrength(int ix, int iy)
    {

        ix = Mathf.Clamp(ix, 0, this.size - 1);
        iy = Mathf.Clamp(this.size - iy, 0, this.size - 1);
        return this.strength[(iy * this.size) + ix];
    }


	public Vector3 worldMaxHeightPoint = Vector3.zero;
	public Vector3 worldMinHeightPoint = Vector3.zero;

    /*********************************************************************************
     * 功能 ： 绘制高度图
     *********************************************************************************/
    public void PaintHeightmap(TerrainData terrainData, Vector3 worldPostion)
    {
        if (terrainData == null) return;
        this.terrainData = terrainData;

        int centerX = Mathf.CeilToInt(worldPostion.x + GameScene.mainScene.terrainConfig.sceneWidth * 0.5f);
        int centerY = Mathf.CeilToInt(worldPostion.z + GameScene.mainScene.terrainConfig.sceneHeight * 0.5f);

        int halfSize = this.size / 2;
        int mod = this.size % 2;

        int xBase = Mathf.Clamp(centerX - halfSize, 0, this.terrainData.heightmapResolution);
        int yBase = Mathf.Clamp(centerY - halfSize, 0, this.terrainData.heightmapResolution);
        int xEnd = Mathf.Clamp((centerX + halfSize) + mod, 0, this.terrainData.heightmapResolution);
        int yEnd = Mathf.Clamp((centerY + halfSize) + mod, 0, this.terrainData.heightmapResolution);

		if (this.tool == LODTerrainTool.Slop) {
			maxHeightPoint.x = Mathf.Clamp(Mathf.CeilToInt(worldMaxHeightPoint.x + GameScene.mainScene.terrainConfig.sceneWidth * 0.5f), 0, this.terrainData.heightmapResolution);
			maxHeightPoint.z = Mathf.Clamp(Mathf.CeilToInt(worldMaxHeightPoint.z + GameScene.mainScene.terrainConfig.sceneHeight * 0.5f), 0, this.terrainData.heightmapResolution);

			minHeightPoint.x = Mathf.Clamp(Mathf.CeilToInt(worldMinHeightPoint.x + GameScene.mainScene.terrainConfig.sceneWidth * 0.5f), 0, this.terrainData.heightmapResolution);
			minHeightPoint.z = Mathf.Clamp(Mathf.CeilToInt(worldMinHeightPoint.z + GameScene.mainScene.terrainConfig.sceneHeight * 0.5f), 0, this.terrainData.heightmapResolution);
		}

        // 计算绘制范围
        int width = xEnd - xBase;
        int height = yEnd - yBase;

        // 获取绘制区域的地形高度
        float[,] heights = this.terrainData.GetHeights(xBase, yBase, width, height);

        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                // 采样笔刷的强度
                int ix = (xBase + i) - (centerX - halfSize);
                int iy = (yBase + j) - (centerY - halfSize);
                float strengthInt = GetStrengthInt(ix, iy);
                float val = heights[j, i];
                val = ApplyBrush(val, strengthInt * this.strengthen, i + xBase, j + yBase);
                heights[j, i] = val;
            }
        }

        terrainData.SetHeights(xBase, yBase, heights);
    }




    /** 应用刷的地形高度 */
    private float ApplyBrush(float height, float brushStrength, int x, int y)
    {
        if (this.tool == LODTerrainTool.PaintHeight)
        {
            float h = height + brushStrength;
			return h;
        }
        if (this.tool == LODTerrainTool.SetHeight)
        {
            if (targetHeight > height)
            {
                height += brushStrength;
                height = Mathf.Min(height, targetHeight);
                return height;
            }
            height -= brushStrength;
            height = Mathf.Max(height, targetHeight);
            return height;
        }

        if (this.tool == LODTerrainTool.SmoothHeight)
        {
            return Mathf.Lerp(height, this.Smooth(x, y), brushStrength);
        }
		if (this.tool == LODTerrainTool.Slop) {
			// 先插值坡度再平滑高度
			float lerpH = Lerp (x, y);
			return lerpH;// Mathf.Lerp(lerpH, this.Smooth(x, y), brushStrength);
		}
        return height;
    }

	private Vector3 maxHeightPoint = Vector3.zero;
	private Vector3 minHeightPoint = Vector3.zero;

	private float Lerp(int x, int y)
	{
		Vector3 v = new Vector3 (x, 0f, y);
		Vector3 v1 = v - minHeightPoint;
		Vector3 v2 = maxHeightPoint - minHeightPoint;
		Vector3 v3 = Vector3.Project (v1, v2);

		float minx = Mathf.Min (minHeightPoint.x, maxHeightPoint.x);
		float maxx = Mathf.Max (minHeightPoint.x, maxHeightPoint.x);

		float minz = Mathf.Min (minHeightPoint.z, maxHeightPoint.z);
		float maxz = Mathf.Max (minHeightPoint.z, maxHeightPoint.z);

		float minH = this.terrainData.GetHeight((int)minHeightPoint.x, (int)minHeightPoint.z);
		float maxH = this.terrainData.GetHeight((int)maxHeightPoint.x, (int)maxHeightPoint.z); 

		float angle = Vector2.Angle (new Vector2(v2.x, v2.z), new Vector2(v3.x, v3.z));
		if (angle > 0.1f)
			return minH;
		
		float t = v3.magnitude / v2.magnitude;
		return Mathf.Lerp (minH, maxH, t);

	}

    private float Smooth(int x, int y)
    {
        float num = 0f;

        num += this.terrainData.GetHeight(x, y);
        num += this.terrainData.GetHeight(x + 1, y);
        num += this.terrainData.GetHeight(x - 1, y);
        num += (this.terrainData.GetHeight(x + 1, y + 1)) * 0.75f;
        num += (this.terrainData.GetHeight(x - 1, y + 1)) * 0.75f;
        num += (this.terrainData.GetHeight(x + 1, y - 1)) * 0.75f;
        num += (this.terrainData.GetHeight(x - 1, y - 1)) * 0.75f;
        num += this.terrainData.GetHeight(x, y + 1);
        num += this.terrainData.GetHeight(x, y - 1);
        return (num / 8f);
    }

#if UNITY_EDITOR
    public void PaintSplatsmap(LODTerrain terrain, Vector3 worldPos, Splat splat, int insertIndex = -1)
    {
        //if (Mathf.Abs(worldPos.x - terrain.transform.position.x) > 32)
           // return;
        if (terrain == null) return;
        this.terrain = terrain;
        int splatIndex = -1;


        for (int k = 0; k < this.terrain.terrainData.splats.Length; k++)
        {
            if (this.terrain.terrainData.splats[k] == null)
            {
                this.terrain.terrainData.splats[k] = splat;
                splatIndex = k;
                break;
            }
            if (this.terrain.terrainData.splats[k].key == splat.key)
            {
                this.terrain.terrainData.splats[k] = splat;
                splatIndex = k;
                break;
            }
        }

        if (splatIndex < 0)
        {
            if (insertIndex >= 0)
            {
                if (insertIndex > this.terrain.terrainData.splats.Length - 1)
                {
                    insertIndex = this.terrain.terrainData.splats.Length - 1;
                }
                this.terrain.terrainData.splats[insertIndex] = splat;
            }
        }

        if (splatIndex < 0)
        {
            // Debug.Log("地形纹理最多数量为4张.不能再进行编辑器,请删除后再编辑.");
            return;
        }



        if (splatIndex < this.terrain.terrainData.spaltsmapLayers)
        {

            // 获取笔刷中心点在地形中的位置
            int brushX = (int)(terrain.transform.position.x - worldPos.x) + 16;
            int brushY = (int)(terrain.transform.position.z - worldPos.z) + 16;

            int halfBrushSize = size / 2;

            float[, ,] alphamap = this.terrain.terrainData.splatsmap;

            for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    int ix = (brushX - (31 - j)) + halfBrushSize;          // (brushX - j) 获取地形顶点在笔刷中的位置
                    int iy = (brushY - i) + halfBrushSize;

                    if (ix < 0 || ix >= size)
                        continue;
                    if (iy < 0 || iy >= size)
                        continue;

                    float strengthInt = GetStrength(ix, iy);
                    // float strengthInt = this.strength[(iy * this.size) + ix];

                    // 应用笔刷修改
                    float alpha = ApplyBrush(alphamap[i, j, splatIndex], strengthInt * this.strengthen);
                    alphamap[i, j, splatIndex] = alpha;

                    Normalize(j, i, splatIndex, alphamap);
                }
            }

            // 修改溅斑数据
            this.terrain.terrainData.SetSplasmap(0, 0, alphamap);
        }
        else
        {
            // Debug.Log("超出纹理层级。");
        }
    }
#endif

    /** 应用笔刷修改 */
    private float ApplyBrush(float value, float brushStrength)
    {
        if (targetAlpha > value)
        {
            value += brushStrength;
            value = Mathf.Min(value, targetAlpha);
            return value;
        }
        value -= brushStrength;
        value = Mathf.Max(value, targetAlpha);
        return value;
    }

    /** 标准化溅斑纹理权值 */
    private void Normalize(int x, int y, int splatIndex, float[, ,] alphamap)
    {
        float num = alphamap[y, x, splatIndex];
        float num2 = 0f;
        int length = alphamap.GetLength(2);         // 获取混合纹理的数量
        for (int i = 0; i < length; i++)
        {
            // 获取其他混合纹理的权值和
            if (i != splatIndex)
            {
                num2 += alphamap[y, x, i];
            }
        }
        if (num2 > 0.01)
        {
            float num5 = (1f - num) / num2;
            for (int j = 0; j < length; j++)
            {
                if (j != splatIndex)
                {
                    float single1 = alphamap[y, x, j];
                    alphamap[y, x, j] = single1 * num5;
                }
            }
        }
        else
        {
            for (int k = 0; k < length; k++)
            {
                alphamap[y, x, k] = (k != splatIndex) ? 0f : 1f;
            }
        }
    }
}