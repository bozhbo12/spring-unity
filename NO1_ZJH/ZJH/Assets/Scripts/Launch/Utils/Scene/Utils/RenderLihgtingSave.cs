using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class RenderLihgtingSave : MonoBehaviour
{
    public bool synData = false;

    public LightmapItem[] datas;

    private int tick = 0;

    public SaveLightmapData[] savelms;


    public LightmapsMode lgMode;

    void Start()
    {
        if (savelms != null && savelms.Length > 0)
        {
            LightmapData[] lmds = new LightmapData[savelms.Length];
            for (int i = 0; i < savelms.Length; i++)
            {
                LightmapData lmd = new LightmapData();
                lmd.lightmapDir = savelms[i].lightmapNear.data;
                lmd.lightmapColor = savelms[i].lightmapFar.data;
                lmds[i] = lmd;
            }
            LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;

            LightmapSettings.lightmaps = lmds;

            LightmapSettings.lightmapsMode = lgMode;

            MeshRenderer[] rds = this.GetComponentsInChildren<MeshRenderer>();
            for (int i = 0; i < rds.Length; i++)
            {
                LightmapItem ld = datas[i];

                rds[i].lightmapIndex = ld.lightmapIndex.data;
                rds[i].lightmapScaleOffset = ld.tilingOffset.data;
            }
        }
    }

    void Update()
    {
        if (synData == true)
        {
            if (tick % 1 == 0)
            {
                LightmapData[] lms  = LightmapSettings.lightmaps ;
                if (lms != null && lms.Length > 0)
                {
                    MeshRenderer[] rds = this.GetComponentsInChildren<MeshRenderer>();
                    datas = new LightmapItem[rds.Length];
                    for (int i = 0; i < rds.Length; i++)
                    {
                        LightmapItem ld = new LightmapItem();
                        ld.lightmapIndex.Fill(rds[i].lightmapIndex);
                        ld.tilingOffset.Fill(rds[i].lightmapScaleOffset);

                        datas[i] = ld;
                    }
#if UNITY_EDITOR
                    // savelms = lms;
                    lgMode = LightmapSettings.lightmapsMode;
                    savelms = new SaveLightmapData[lms.Length];
                    for (int j = 0; j < lms.Length; j++)
                    {
                        if (savelms[j] == null)
                            savelms[j] = new SaveLightmapData();
                        savelms[j].lightmapFar.Fill(lms[j].lightmapColor);
                        savelms[j].lightmapNear.Fill(lms[j].lightmapDir);
                    }
#endif
                }

            }
            tick++; 
        }
    }


}
[Serializable]
public class SaveLightmapData
{

    public LightmapSerializer lightmapFar;

    public LightmapSerializer lightmapNear;
}

[Serializable]
public class LightmapItem
{

    public IntSerializer lightmapIndex;
    public Vector4Serializer tilingOffset;
}

[Serializable]
public struct LightmapSerializer
{
    public Texture2D value;

    public void Fill(Texture2D v)
    {
        value = v;
    }

    public Texture2D data
    {
        get { return value; }
    }
}


[Serializable]
public struct IntSerializer
{
    public int value;

    public void Fill(int v)
    {
        value = v;
    }

    public int data
    {
        get { return value; }
    }
}

[Serializable]
public struct Vector4Serializer
{
    public float x;
    public float y;
    public float z;
    public float w;

    public void Fill(Vector4 v4)
    {
        x = v4.x;
        y = v4.y;
        z = v4.z;
        w = v4.w;
    }

    public Vector4 data
    { 
        get { return new Vector4(x, y, z, w); } 
    }
}