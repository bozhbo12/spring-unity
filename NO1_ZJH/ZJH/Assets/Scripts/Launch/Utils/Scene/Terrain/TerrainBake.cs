#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*****************************************************************************************
 * 类 : 地形烘焙
 ******************************************************************************************/
public class TerrainBake
{
    static private Camera rttCamera;
    static public int size = 135;

    static private GameObject quad;
    static private Material sntMat;            

    static public List<RenderTexture> maps = new List<RenderTexture>();
    static private List<int> mapIndex = new List<int>();

    public TerrainBake()
    { 
        
    }

    static public void Init()
    {
        while (maps.Count > 0)
        {
            RenderTexture rtt = maps[0];
            rtt.Release();

            maps.RemoveAt(0);
            mapIndex.RemoveAt(0);
        }
    }

    static public int mapsCount
    {
        set
        {
            if (value > maps.Count)
            {
                for (int i = 0; i < value; i++)
                {
                    RenderTexture rtt = new RenderTexture(size, size, 0);
                    rtt.name = "Terrainmapping" + i;
                    rtt.isPowerOfTwo = true;
                    rtt.hideFlags = HideFlags.DontSave;
                    rtt.format = RenderTextureFormat.ARGB32;
                    rtt.depth = 0;
                    maps.Add(rtt);
                    mapIndex.Add(-1);
                }
            }
        }
    }

    static public void Cancel(int index)
    {
        if (index >= 0)
            mapIndex[index] = -1;
    }

    static public int Bake(LODTerrain terrain, int size = 512)
    {
        if (terrain == null) return -1;
        RenderTexture rtt = null;
        int index = -1;
        for (int i = 0; i < mapIndex.Count; i++)
        {
            if (mapIndex[i] < 0)
            {
                index = i;
                rtt = maps[i];
                mapIndex[i] = index;
                break;
            }
        }
        if (rtt != null)
        {
            CreateObjects();
            UpdateCameraModes();
            rttCamera.gameObject.SetActive(true);
            rttCamera.enabled = true;
            
            sntMat = terrain.matrial;

            quad.GetComponent<Renderer>().lightmapIndex = terrain.GetComponent<Renderer>().lightmapIndex;
            quad.GetComponent<Renderer>().lightmapScaleOffset = terrain.GetComponent<Renderer>().lightmapScaleOffset;

            quad.GetComponent<Renderer>().material = sntMat;

            // 拷贝渲染结果
            rttCamera.targetTexture = rtt;
            RenderTexture.active = rtt;


            rttCamera.transform.position = new Vector3(terrain.transform.position.x, 1000f, terrain.transform.position.z);

            // 拷贝到2D纹理中
            rttCamera.Render();
            rttCamera.gameObject.SetActive(false);

            // RenderTexture.active = null;
            // quad.GetComponent<Renderer>().material = null;
            sntMat = null;
        }
        return index;
    }


    static private void UpdateCameraModes()
    {
        // rttCamera.clearFlags = src.clearFlags;
        rttCamera.backgroundColor = new Color(1f, 1f, 1f);

        rttCamera.farClipPlane = 2000f;
        rttCamera.nearClipPlane = 0.1f;
        rttCamera.orthographic = true;                              // 正交摄像机
        rttCamera.aspect = 1f;
        rttCamera.orthographicSize = GameScene.mainScene.terrainConfig.tileSize * 0.5f;
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
            go.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            go.transform.position = new Vector3(0f, 0f, 100f);
        }

        // 创建四边形
        if (quad == null)
        {
            quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.layer = GameLayer.Layer_Ground;
            quad.transform.parent = rttCamera.transform;
            quad.transform.localScale = new Vector3(2f, 2f, 0f);
            quad.transform.localPosition = new Vector3(0f, 0f, 1f);
        }

    }


}
#endif