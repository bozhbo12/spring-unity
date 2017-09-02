using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*****************************************************************************************
 * 类 : 地形烘焙
 ******************************************************************************************/
public class Terrainmapping
{
    public enum RT_Type
    {
        NONE = 0,
        MIN = 1,        //512
        MAX = 2,        //1024
    }

    static private Camera rttCamera;

    static private GameObject quad;
    static private Material sntMat;

    static public List<RenderTexture> maps = new List<RenderTexture>(9);

    private const int MaxSize = 1024;

    private const int MinSize = 512;

    /// <summary>
    /// RT数量
    /// </summary>
    private static int iCountRT
    {
        get
        {
            //低配只有8RT
            if (SystemSetting.ImageQuality == GameQuality.LOW)
            {
                return 8;
            }
            else if (SystemSetting.ImageQuality == GameQuality.SUPER_LOW)
            {
                return 9;
            }
            return 0;
        }
    }

    //public static void Clear()
    //{
    //    ClearRender();
    //}


    private static List<RenderTexture> UnUseRT = new List<RenderTexture>(9);

    /// <summary>
    /// 512 RenderTexture缓存
    /// </summary>
    private static List<RenderTexture> UseRT = new List<RenderTexture>(9);

    /// <summary>
    /// 1024 RenderTexture
    /// </summary>
    private static RenderTexture mMaxRenders;

    /// <summary>
    /// 获取tempRenderer
    /// </summary>
    /// <returns></returns>
    private static RenderTexture GetTempRenderer(int rtType)
    {
        RenderTextureFormat format = RenderTextureFormat.RGB565;
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_STANDALONE_OSX || UNITY_EDITOR
        format = RenderTextureFormat.Default;
#endif

        RenderTexture RT = null;
        if (rtType == (int)RT_Type.MAX)
        {
            if (mMaxRenders == null)
            {
                mMaxRenders = new RenderTexture(MaxSize, MaxSize, 0, format);
            }
            RT = mMaxRenders;
        }
        else
        {
            if (UnUseRT.Count > 0)
            {
                RT = UnUseRT[0];
                UnUseRT.RemoveAt(0);
            }
            else
            {
                RT = new RenderTexture(MinSize, MinSize, 0, format);
            }
            UseRT.Add(RT);
        }
        return RT;
    }

    public static void Print()
    {
        LogSystem.LogWarning("====================Terrainmapping========================");
        for (int i = 0; i < UseRT.Count; i++)
        {
            LogSystem.LogWarning("UseRT:", i, UseRT[i].width,"*", UseRT[i].height);
        }
        for (int i = 0; i < UnUseRT.Count; i++)
        {
            LogSystem.LogWarning("UnUseRT:", i, UnUseRT[i].width, "*", UnUseRT[i].height);
        }

        if(mMaxRenders!=null)
            LogSystem.LogWarning("mMaxRenders:", mMaxRenders.width, "*", mMaxRenders.height);
    }

    /// <summary>
    /// 释放Renderer
    /// </summary>
    /// <param name="renderer"></param>
    /// <returns></returns>
    private static bool ReleaseTempRenderer(RenderTexture RT)
    {
        if (RT == null)
        {
            LogSystem.LogWarning("ReleaseTempRenderer is null!!");
            return false;
        }

        //高中配没有RT
        if (SystemSetting.ImageQuality == GameQuality.HIGH || SystemSetting.ImageQuality == GameQuality.MIDDLE)
        {
            RT.Release();
            UnityEngine.Object.DestroyImmediate(RT, true);
            UseRT.Remove(RT);
            //中断
            return true;
        }
        else if (SystemSetting.ImageQuality == GameQuality.SUPER_LOW)
        {
            if (RT.width == MaxSize)
            {
                RT.Release();
                UnityEngine.Object.DestroyImmediate(RT, true);
                mMaxRenders = null;
                //中断
                return true;
            }
        }

        if (RT.width == MaxSize)
        {
            mMaxRenders = RT;
        }
        else
        {
            UseRT.Remove(RT);
            //小于缓存数
            if (UnUseRT.Count + UseRT.Count < iCountRT)
            {
                UnUseRT.Add(RT);
            }
            else
            {
                RT.Release();
                UnityEngine.Object.DestroyImmediate(RT, true);
            }
        }
        return true;
    }


    /// <summary>
    /// 释放
    /// </summary>
    /// <param name="index"></param>
    static public void Cancel(RenderTexture RT)
    {
        ReleaseTempRenderer(RT);
    }

    static public RenderTexture Bake(LODTerrain terrain, int rtType)
    {
        RenderTexture RT = GetTempRenderer(rtType);
        if (RT != null)
        {
            CreateObjects();
            UpdateCameraModes();
            rttCamera.gameObject.SetActive(true);
            rttCamera.enabled = true;

            sntMat = terrain.matrial;
            Renderer quadRenderer = quad.GetComponent<Renderer>();
            Renderer terRenderer = terrain.GetComponent<Renderer>();
            quadRenderer.lightmapIndex = terRenderer.lightmapIndex;
            quadRenderer.lightmapScaleOffset = terRenderer.lightmapScaleOffset;
            quadRenderer.material = sntMat;

            // 拷贝渲染结果
            rttCamera.targetTexture = RT;
            RenderTexture.active = RT;

            // 拷贝到2D纹理中
            rttCamera.Render();
            rttCamera.gameObject.SetActive(false);
            rttCamera.targetTexture = null;
            RenderTexture.active = null;
            quadRenderer.material = null;
            sntMat = null;
        }
        return RT;
    }


    static private void UpdateCameraModes()
    {
        // rttCamera.clearFlags = src.clearFlags;
        rttCamera.backgroundColor = new Color(1f, 1f, 1f);

        rttCamera.farClipPlane = 1000f;
        rttCamera.nearClipPlane = 0.1f;
        rttCamera.orthographic = true;                              // 正交摄像机
        rttCamera.aspect = 1f;
        rttCamera.orthographicSize = 1f;
    }


    static private void CreateObjects()
    {
        // 创建反射摄像机
        if (rttCamera == null)
        {
            GameObject go = new GameObject("SplatsNormalmapCamera", typeof(Camera));
            rttCamera = go.GetComponent<Camera>();
            rttCamera.enabled = false;
            rttCamera.cullingMask = GameLayer.Mask_AmbienceSphere;
            rttCamera.clearFlags = CameraClearFlags.SolidColor;
            go.transform.position = new Vector3(0f, 0f, 1000f);
        }

        // 创建四边形
        if (quad == null)
        {
            quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.layer = GameLayer.Layer_AmbienceSphere;
            quad.transform.parent = rttCamera.transform;
            quad.transform.localScale = new Vector3(2f, 2f, 0f);
            quad.transform.localPosition = new Vector3(0f, 0f, 1f);
        }

    }
}