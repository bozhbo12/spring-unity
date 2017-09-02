using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*****************************************************************************************
 * 类 : 溅斑法线图生成器
 ******************************************************************************************/
public class SplatsNormalmap
{
    private Camera rttCamera;
    public RenderTexture rtt;
    private int size = 2048;

    /** 四边形世界坐标范围(-0.5, -0.5) - (0.5, 0.5) */
    private GameObject quad;
    private Material sntMat;            

    static private Dictionary<string, GameObject> quads = new Dictionary<string, GameObject>();

    private string key = "";

    static private float tick = 0;

    private Texture2D tex;

    public SplatsNormalmap(string key)
    { 
        this.key = key;
    }

    public void Real()
    {
        //RenderTexture.active = null;
        //rttCamera.targetTexture = null;
        rttCamera.enabled = false;
        rttCamera.gameObject.SetActive(false);
    }

    private void UpdateCameraModes()
    {
        // rttCamera.clearFlags = src.clearFlags;
        rttCamera.backgroundColor = new Color(1f, 1f, 1f);

        rttCamera.farClipPlane = 1f;
        rttCamera.nearClipPlane = 0.1f;
        rttCamera.orthographic = true;                              // 正交摄像机
        rttCamera.aspect = 1f;
        rttCamera.orthographicSize = 1f;
    }

    private void CreateObjects()
    {
        // 创建反射纹理
        if (rtt == null)
        {
            rtt = new RenderTexture(size, size, 24);
            rtt.name = "SplatsNormalmapRTT";
            rtt.isPowerOfTwo = true;
            rtt.hideFlags = HideFlags.DontSave;
            // rtt.format = RenderTextureFormat.ARGB32;
        }

        // 创建反射摄像机
        if (rttCamera == null)
        {
            GameObject go = new GameObject("SplatsNormalmapCamera", typeof(Camera), typeof(Skybox));
            go.transform.position = new Vector3(0f, 0f, tick++);
            rttCamera = go.GetComponent<Camera>();
            rttCamera.enabled = false;
            rttCamera.gameObject.AddComponent<FlareLayer>();
            // go.hideFlags = HideFlags.HideAndDontSave;
        }

        // 创建四边形
        if (quad == null)
        {
            quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.transform.parent = rttCamera.transform;
            quad.transform.localScale = new Vector3(2f, 2f, 0f);
            quad.transform.localPosition = new Vector3(0f, 0f, 1f);
            sntMat = new Material(Shader.Find("Snail/Terrain-Splats-Bump-Texture"));
            quad.GetComponent<Renderer>().material = sntMat;
            quad.SetActive(false);

            quads.Add(key, quad);
        }

        if (tex == null)
        {
            tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        }
    }


}