using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SSAO : PEBase
{
    public enum SSAOSamples
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }

    /** 遮挡半径 */
    [Range(0.05f, 1.0f)]
    public float m_Radius = 0.4f;

    public SSAOSamples m_SampleCount = SSAOSamples.High;

    // 遮挡强度
    [Range(0.5f, 10.0f)]
    public float m_OcclusionIntensity = 1.5f;

    // 模糊次数
    [Range(0, 4)]
    public int m_Blur = 2;

    // 采样分辨率
    [Range(1, 6)]
    public int m_Downsampling = 1;

    // 遮挡衰减
    [Range(0.2f, 10.0f)]
    public float m_OcclusionAttenuation = 1.0f;


    [Range(0.00001f, 0.5f)]
    public float m_MinZ = 0.01f;

    public Texture2D m_RandomTexture;

    private Material material = new Material(Shader.Find("Snail/Effect/SSAO"));


    void OnEnable()
    {
        // 设置深度图模式
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
        useDepthTick++;
    }


    void OnDisable()
    {
        useDepthTick--;
    }

    override public Dictionary<string, int> matParams
    {
        get
        {
            if (_matParams == null)
            {
                _matParams = new Dictionary<string, int>();
            }
            _matParams.Clear();

            _matParams.Add("_pRadius", (int)MatParamType.Type_Float);
            _matParams.Add("_pOcclusionIntensity", (int)MatParamType.Type_Float);
            _matParams.Add("_pBlur", (int)MatParamType.Type_Int);
            _matParams.Add("_pDownsampling", (int)MatParamType.Type_Int);
            _matParams.Add("_pOcclusionAttenuation", (int)MatParamType.Type_Float);

            return _matParams;
        }
    }

    override public void LoadParams()
    {
        m_Radius = varData.GetFloat("_pRadius");
        m_OcclusionIntensity = varData.GetFloat("_pOcclusionIntensity");
        m_Blur = varData.GetInt("_pBlur");
        m_Downsampling = varData.GetInt("_pDownsampling");
        m_OcclusionAttenuation = varData.GetFloat("_pOcclusionAttenuation");

        m_RandomTexture = Resources.Load("Textures/Shader/RandomVectors") as Texture2D;
    }

    override public void SetVarData()
    {
        varData.SetFloat("_pRadius", m_Radius);
        varData.SetFloat("_pOcclusionIntensity", m_OcclusionIntensity);
        varData.SetInt("_pBlur", m_Blur);
        varData.SetInt("_pDownsampling", m_Downsampling);
        varData.SetFloat("_pOcclusionAttenuation", m_OcclusionAttenuation);
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material == null)
            return;

        SetVarData();

        // 采样分辨率
        m_Downsampling = Mathf.Clamp(m_Downsampling, 1, 6);
        // 采样半径
        m_Radius = Mathf.Clamp(m_Radius, 0.05f, 1.0f);
        m_MinZ = Mathf.Clamp(m_MinZ, 0.00001f, 0.5f);
        // AO强度
        // m_OcclusionIntensity = Mathf.Clamp(m_OcclusionIntensity, 0.5f, 4.0f);
        // 遮挡衰减
        //m_OcclusionAttenuation = Mathf.Clamp(m_OcclusionAttenuation, 0.2f, 2.0f);
        // 模糊
        m_Blur = Mathf.Clamp(m_Blur, 0, 4);

        RenderTexture rtAO = RenderTexture.GetTemporary(source.width / m_Downsampling, source.height / m_Downsampling, 0);
        
        float fovY = GetComponent<Camera>().fieldOfView;
        float far = GetComponent<Camera>().farClipPlane;
        float y = Mathf.Tan(fovY * Mathf.Deg2Rad * 0.5f) * far;
        float x = y * GetComponent<Camera>().aspect;
        material.SetVector("_FarCorner", new Vector3(x, y, far));

        int noiseWidth, noiseHeight;
        if (m_RandomTexture)
        {
            noiseWidth = m_RandomTexture.width;
            noiseHeight = m_RandomTexture.height;
        }
        else
        {
            noiseWidth = 1; noiseHeight = 1;
        }

        // 设置噪图偏移
        material.SetVector("_NoiseScale", new Vector3((float)rtAO.width / noiseWidth, (float)rtAO.height / noiseHeight, 0.0f));

        // 遮挡半径/计算距离/遮挡衰减/遮挡强度
        material.SetVector("_Params", new Vector4( m_Radius, m_MinZ, 1.0f / m_OcclusionAttenuation, m_OcclusionIntensity));

        bool doBlur = m_Blur > 0;
        Graphics.Blit(doBlur ? null : source, rtAO, material, (int)m_SampleCount);

        //Graphics.Blit(rtAO, destination);
        //RenderTexture.ReleaseTemporary(rtAO);
        //return;

        if (doBlur)
        {
            // SSAO 水平模糊
            RenderTexture rtBlurX = RenderTexture.GetTemporary(source.width, source.height, 0);
            material.SetVector("_TexelOffsetScale", new Vector4((float)m_Blur / source.width, 0, 0, 0));
            material.SetTexture("_SSAO", rtAO);
            Graphics.Blit(null, rtBlurX, material, 3);
            RenderTexture.ReleaseTemporary(rtAO); // original rtAO not needed anymore

            // SSAO 垂直模糊
            RenderTexture rtBlurY = RenderTexture.GetTemporary(source.width, source.height, 0);
            material.SetVector("_TexelOffsetScale", new Vector4(0, (float)m_Blur / source.height, 0, 0));
            material.SetTexture("_SSAO", rtBlurX);
            Graphics.Blit(source, rtBlurY, material, 3);
            RenderTexture.ReleaseTemporary(rtBlurX); // blurX RT not needed anymore

            rtAO = rtBlurY; // AO is the blurred one now
        }
     
        material.SetTexture("_SSAO", rtAO);

        Graphics.Blit(source, destination, material, 4);
        
        RenderTexture.ReleaseTemporary(rtAO);
    }

}