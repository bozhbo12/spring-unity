using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
/***********************************************************************************************
 * 功能 ： 地形数据
 ***********************************************************************************************/
public class TerrainData
{
    /** 高度图的分辨率 */
    public int heightmapResolution = -1;

    /** 地形在世界坐标系中的尺寸, 最大宽度、最大长度、最大高度 */
    public float maxTerrainHeight = 1000f;

    /** 灰度图 */
    public float[,] heightmap;

    public Texture2D _heightmapTex;

    private bool _heightmapDirty = true;

    public DetailDataBase detailDatabase = null;



    /*************************************************************
     * 功能 ： 根据地形配置创建新的地形数据
     **************************************************************/
    public TerrainData(int heightmapResolution = 480, int splatmapResolution = 480, int spaltsmapLayers = 4, float maxTerrainHeight = 1000f, float defaultTerrainHeight = 100f)
    {
        this.detailDatabase = new DetailDataBase();
        detailDatabase.SetDetailResolution(512, 64);

        this.heightmapResolution = heightmapResolution;
        this.maxTerrainHeight = maxTerrainHeight;

        heightmap = new float[this.heightmapResolution, this.heightmapResolution];

        int i = 0;
        int j = 0;

        // 填充初始高度图
        for (i = 0; i < this.heightmapResolution; i++)
        {
            for (j = 0; j < this.heightmapResolution; j++)
            {
                heightmap[i, j] = defaultTerrainHeight;
            }
        }
    }

  
    public bool heightmapDirty
    {
        get
        {
            return _heightmapDirty;
        }
        set
        {
            _heightmapDirty = value;
        }
    }


    /**********************************************************************************
     * 功能 ：采样指定像素点高度
     * 注 ： 高度在二维数组中排列顺序 heightmap【行号， 列号】
     * heightmap[0, 1] heightmap[0, 2]  heightmap[0, 3]
     * heightmap[1, 1] heightmap[1, 2]  heightmap[1, 3]
     * heightmap[3, 1] heightmap[3, 2]  heightmap[3, 3]
     **********************************************************************************/
    public float GetHeight(int x, int z)
    {
		float h = GameScene.mainScene.terrainConfig.defaultTerrainHeight;

        if ((z >= 0 && z < heightmapResolution) && (x >= 0 && x < heightmapResolution))
        {
            h = heightmap[z, x];
            return h;
        }

        if (z >= heightmapResolution)
            z = heightmapResolution - 1;
        if (x >= heightmapResolution)
            x = heightmapResolution - 1;

        h = heightmap[z, x];

        return h;
    }

    public void SetHeight(int x, int z, float value)
    {
        if (x < 0 || x >= heightmapResolution || z < 0 || z >= heightmapResolution)
            return;

        heightmap[z, x] = value;
    }

    /*******************************************************************************
     * 属性 : 获取高度纹理
     *******************************************************************************/
    public Texture2D heightmapTex
    {
        get
        {
            if (_heightmapDirty == true)
                GenerateHeightMapTex();
            return _heightmapTex;
        }
        set
        {
            _heightmapTex = value;
            int width = heightmap.GetLength(1);
            int height = heightmap.GetLength(0);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    heightmap[j, i] =_heightmapTex.GetPixel(j, i).r * maxTerrainHeight;
                }
            }
            _heightmapDirty = true;
        }
    }

    
    /*****************************************************************************************
       * 功能 ： 生成高度图纹理
       *****************************************************************************************/
    public void GenerateHeightMapTex()
    {
        _heightmapTex = new Texture2D(heightmapResolution, heightmapResolution, TextureFormat.ARGB32, false);
        _heightmapTex.wrapMode = TextureWrapMode.Clamp;
        for (int i = 0; i < heightmapResolution; i++)
        {
            for (int j = 0; j < heightmapResolution; j++)
            {
                float val = heightmap[j, i] / maxTerrainHeight;
                _heightmapTex.SetPixel(j, i, new Color(val, val, val, 1f));
            }
        }
        _heightmapTex.Apply();
    }


    /******************************************************************************
     * 功能 ： 释放地形数据
     ******************************************************************************/
    public void Release()
    {
        this.heightmap = null;
        if (_heightmapTex != null)
        {
            UnityEngine.Object.Destroy(_heightmapTex);
            _heightmapTex = null;
        }
    }


    /**********************************************************************************
     * 功能 ：获取地形的高度
     ***********************************************************************************/
    public float[,] GetHeights(int xBase, int yBase, int width, int height)
    {
        float[,] result = new float[height, width];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                result[j, i] = heightmap[yBase + j, xBase + i];
            }
        }
        return result;
    }

    /*********************************************************************************
    * 功能 ：应用地形高度
    *********************************************************************************/
    public void SetHeights(int xBase, int yBase, float[,] heights)
    {
        if (heights == null) return;
        int width = heights.GetLength(1);
        int height = heights.GetLength(0);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                heightmap[yBase + j, xBase + i] = heights[j, i];
            }
        }
        _heightmapDirty = true;
    }

    public void SetHeights(int xBase, int yBase, int width, int height, float value)
    {
        // i < width, 在线段采样高度的时候永远采样不到边缘的高度 i <= width 才可以
        // i 列号, j行号
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {  
                heightmap[yBase + j, xBase + i] = value;
            }
        }
        _heightmapDirty = true;
    }

    public void Flat()
    {
        int i = 0;
        int j = 0;

        // 填充初始高度图
        for (i = 0; i < this.heightmapResolution; i++)
        {
            for (j = 0; j < this.heightmapResolution; j++)
            {
                heightmap[i, j] = GameScene.mainScene.terrainConfig.defaultTerrainHeight;
            }
        }
    }

    public Vector3 GetInterpolatedNormal (float x, float y) 
    {
        float fx = x * (heightmapResolution - 1);
        float fy = y * (heightmapResolution - 1);
	    int lx = (int)fx;
	    int ly = (int)fy;
	
	    Vector3 n00 = CalculateNormalSobel (lx+0, ly+0);
        Vector3 n10 = CalculateNormalSobel(lx + 1, ly + 0);
        Vector3 n01 = CalculateNormalSobel(lx + 0, ly + 1);
        Vector3 n11 = CalculateNormalSobel(lx + 1, ly + 1);
	
	    float u = fx - lx;
	    float v = fy - ly;

        Vector3 s = Vector3.Lerp(n00, n10, u);
        Vector3 t = Vector3.Lerp(n01, n11, u);
        Vector3 value = Vector3.Lerp(s, t, v);
        value = value.normalized;
	
	    return value;
    }

    public Vector3 CalculateNormalSobel (int x, int y)
    {
	    Vector3 normal;
	    float dY, dX;
	    // Do X sobel filter
	    dX  = GetHeight (x-1, y-1) * -1.0f;
	    dX += GetHeight (x-1, y  ) * -2.0f;
	    dX += GetHeight (x-1, y+1) * -1.0f;
	    dX += GetHeight (x+1, y-1) *  1.0f;
	    dX += GetHeight (x+1, y  ) *  2.0f;
	    dX += GetHeight (x+1, y+1) *  1.0f;
	
	    //dX /= m_Scale.x;
	
	    // Do Y sobel filter
	    dY  = GetHeight (x-1, y-1) * -1.0f;
	    dY += GetHeight (x  , y-1) * -2.0f;
	    dY += GetHeight (x+1, y-1) * -1.0f;
	    dY += GetHeight (x-1, y+1) *  1.0f;
	    dY += GetHeight (x  , y+1) *  2.0f;
	    dY += GetHeight (x+1, y+1) *  1.0f;
	    //dY /= m_Scale.z;
	
	    // Cross Product of components of gradient reduces to
	    normal.x = -dX;
        normal.y = 8;
	    normal.z = -dY;
        normal = normal.normalized;	  
		
	    return normal;
    }
}

#endif