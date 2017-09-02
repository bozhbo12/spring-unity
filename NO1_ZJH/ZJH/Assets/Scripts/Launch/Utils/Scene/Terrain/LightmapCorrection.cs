using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*****************************************************************************************
 * 类 : 地形烘焙
 ******************************************************************************************/
public class LightmapCorrection
{
    static private Camera rttCamera;
    static private int size = 1024;

    static private GameObject quad;
    static private Material sntMat;

    static public List<Texture2D> maps = new List<Texture2D>();
    static private List<int> mapIndex = new List<int>();
    static private RenderTexture tagetRTT;

    public LightmapCorrection()
    {

    }

    static public int mapsCount
    {
        set
        {
            if (value > maps.Count)
            {
                int count = value - maps.Count;
                for (int i = 0; i < count; i++)
                {
                    Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
                    maps.Add(tex);
                    mapIndex.Add(-1);
                }
            }
        }
        get
        {
            return maps.Count;
        }
    }

    static public void Clear()
    {
        if (tagetRTT != null)
            tagetRTT.Release();
        tagetRTT = null;

        // for (int i = 0; i < maps.Count; i++)
            // Resources.UnloadAsset(maps[i]);
        maps.Clear();
        mapIndex.Clear();
    }

    static public Texture2D Bake(Texture2D lightmapTex, int size = 1024)
    {
        Texture2D tex = null;
        int index = -1;
        for (int i = 0; i < mapIndex.Count; i++)
        {
            if (mapIndex[i] < 0)
            {
                index = i;
                tex = maps[i];
                mapIndex[i] = index;
                break;
            }
        }

        if (tex != null)
        {
            if (sntMat == null)
                sntMat = new Material(Shader.Find("Snail/LightmapCorrection"));
            sntMat.mainTexture = lightmapTex;

            if (tagetRTT == null)
            {
                tagetRTT = new RenderTexture(size, size, 0, RenderTextureFormat.ARGB32);
                tagetRTT.isPowerOfTwo = true;
                tagetRTT.hideFlags = HideFlags.DontSave;
                tagetRTT.format = RenderTextureFormat.ARGB32;
                tagetRTT.depth = 0;
                tagetRTT.useMipMap = false;
            }
            RenderTexture.active = tagetRTT;
            Graphics.Blit(lightmapTex, tagetRTT, sntMat);
        }
        // 引用置空,使纹理内存可以回收
        if (sntMat != null)
            sntMat.mainTexture = null;
        tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        tex.Compress(true);
        tex.Apply();

        RenderTexture.active = null;
        return tex;
    }


}