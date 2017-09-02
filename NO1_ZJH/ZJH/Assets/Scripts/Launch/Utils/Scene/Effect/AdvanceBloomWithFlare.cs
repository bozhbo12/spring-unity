using System;
using UnityEngine;

public enum LensflareStyle34
{
    Ghosting = 0,
    Anamorphic = 1,
    Combined = 2,
}

public enum TweakMode34
{
    Basic = 0,
    Complex = 1,
}


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

/**************************************************************************************************
 * 类 : 高级泛光效果
 **************************************************************************************************/

public class AdvanceBloomWithFlare : PEBase
{

    protected virtual void Start()
    {
        if (CheckResources() == false)
        {
            enabled = false;
            return;
        }
    }

    public TweakMode34 tweakMode = 0;
    public BloomScreenBlendMode screenBlendMode = BloomScreenBlendMode.Add;

    public HDRBloomMode hdr = HDRBloomMode.Auto;
    private bool doHdr = false;
    public float sepBlurSpread = 1.5f;
    public float useSrcAlphaAsMask = 0.5f;

    public float bloomIntensity = 1.0f;
    [Range(0, 0.5f)]
    public float bloomThreshold = 0.5f;
    public int bloomBlurIterations = 2;

    public bool lensflares = false;
    public int hollywoodFlareBlurIterations = 2;
    public LensflareStyle34 lensflareMode = (LensflareStyle34)1;
    public float hollyStretchWidth = 3.5f;
    public float lensflareIntensity = 1.0f;
    public float lensflareThreshold = 0.3f;
    public Color flareColorA = new Color(0.4f, 0.4f, 0.8f, 0.75f);
    public Color flareColorB = new Color(0.4f, 0.8f, 0.8f, 0.75f);
    public Color flareColorC = new Color(0.8f, 0.4f, 0.8f, 0.75f);
    public Color flareColorD = new Color(0.8f, 0.4f, 0.0f, 0.75f);
    public Texture2D lensFlareVignetteMask;

    public Shader lensFlareShader;
    public Material m_LensFlareMaterial;
    public Material LensFlareMaterial
    {
        get
        {
            if (m_LensFlareMaterial == null)
            {
                if (lensFlareShader == null)
                    lensFlareShader = Shader.Find("Snail/Effect/LensFlareCreate");

                m_LensFlareMaterial = new Material(lensFlareShader);
                m_LensFlareMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_LensFlareMaterial;
        }
    }

    public Shader vignetteShader;
    public Material m_VignetteMaterial;
    public Material VignetteMaterial
    {
        get
        {
            if (m_VignetteMaterial == null)
            {
                if (vignetteShader == null)
                    vignetteShader = Shader.Find("Snail/Effect/VignetteShader");

                m_VignetteMaterial = new Material(vignetteShader);
                m_VignetteMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_VignetteMaterial;
        }
    }

    public Shader separableBlurShader;
    public Material m_SeparableBlurMaterial;
    public Material SeparableBlurMaterial
    {
        get
        {
            if (m_SeparableBlurMaterial == null)
            {
                if (separableBlurShader == null)
                    separableBlurShader = Shader.Find("Snail/Effect/SeparableBlurPlus");

                m_SeparableBlurMaterial = new Material(separableBlurShader);
                m_SeparableBlurMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_SeparableBlurMaterial;
        }
    }

    public Shader addBrightStuffOneOneShader;
    public Material m_AddBrightStuffBlendOneOneMaterial;
    public Material AddBrightStuffBlendOneOneMaterial
    {
        get
        {
            if (m_AddBrightStuffBlendOneOneMaterial == null)
            {
                if (addBrightStuffOneOneShader == null)
                    addBrightStuffOneOneShader = Shader.Find("Snail/Effect/BlendOneOne");

                m_AddBrightStuffBlendOneOneMaterial = new Material(addBrightStuffOneOneShader);
                m_AddBrightStuffBlendOneOneMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_AddBrightStuffBlendOneOneMaterial;
        }
    }

    public Shader screenBlendShader;
    public Material m_ScreenBlend;
    public Material ScreenBlend
    {
        get
        {
            if (m_ScreenBlend == null)
            {
                if (screenBlendShader == null)
                    screenBlendShader = Shader.Find("Snail/Effect/Blend");

                m_ScreenBlend = new Material(screenBlendShader);
                m_ScreenBlend.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_ScreenBlend;
        }
    }

    public Shader hollywoodFlaresShader;
    public Material m_HollywoodFlaresMaterial;
    public Material HollywoodFlaresMaterial
    {
        get
        {
            if (m_HollywoodFlaresMaterial == null)
            {
                if (hollywoodFlaresShader == null)
                    hollywoodFlaresShader = Shader.Find("Snail/Effect/MultipassHollywoodFlares");

                m_HollywoodFlaresMaterial = new Material(hollywoodFlaresShader);
                m_HollywoodFlaresMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_HollywoodFlaresMaterial;
        }
    }

    public Shader brightPassFilterShader;
    public Material m_BrightPassFilterMaterial;
    public Material BrightPassFilterMaterial
    {
        get
        {
            if (m_BrightPassFilterMaterial == null)
            {
                if (brightPassFilterShader == null)
                    brightPassFilterShader = Shader.Find("Snail/Effect/BrightPassFilterForBloom");

                m_BrightPassFilterMaterial = new Material(brightPassFilterShader);
                m_BrightPassFilterMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_BrightPassFilterMaterial;
        }
    }

    /**********************************************************************************************************
     * 功能 : 检测资源是否正确
     **********************************************************************************************************/

    public bool CheckResources()
    {
        CheckSupport(false);
        CheckMaterial(LensFlareMaterial);
        CheckMaterial(VignetteMaterial);
        CheckMaterial(SeparableBlurMaterial);
        CheckMaterial(AddBrightStuffBlendOneOneMaterial);
        CheckMaterial(ScreenBlend);
        CheckMaterial(HollywoodFlaresMaterial);
        CheckMaterial(BrightPassFilterMaterial);

        if (!isSupported)
            ReportAutoDisable();
        return isSupported;
    }

    void OnDestroy()
    {
        if (m_LensFlareMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_LensFlareMaterial);
#else
                Destroy(m_LensFlareMaterial);
#endif
        }
        if (m_VignetteMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_VignetteMaterial);
#else
                Destroy(m_VignetteMaterial);
#endif
        }
        if (m_SeparableBlurMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_SeparableBlurMaterial);
#else
                Destroy(m_SeparableBlurMaterial);
#endif
        }
        if (m_AddBrightStuffBlendOneOneMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_AddBrightStuffBlendOneOneMaterial);
#else
                Destroy(m_AddBrightStuffBlendOneOneMaterial);
#endif
        }
        if (m_ScreenBlend)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_ScreenBlend);
#else
                Destroy(m_ScreenBlend);
#endif
        }
        if (m_HollywoodFlaresMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_HollywoodFlaresMaterial);
#else
                Destroy(m_HollywoodFlaresMaterial);
#endif
        }
        if (m_BrightPassFilterMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_BrightPassFilterMaterial);
#else
                Destroy(m_BrightPassFilterMaterial);
#endif
        }
    }
    void OnDisable()
    {
        if (m_LensFlareMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_LensFlareMaterial);
#else
                Destroy(m_LensFlareMaterial);
#endif
        }
        if (m_VignetteMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_VignetteMaterial);
#else
                Destroy(m_VignetteMaterial);
#endif
        }
        if (m_SeparableBlurMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_SeparableBlurMaterial);
#else
                Destroy(m_SeparableBlurMaterial);
#endif
        }
        if (m_AddBrightStuffBlendOneOneMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_AddBrightStuffBlendOneOneMaterial);
#else
                Destroy(m_AddBrightStuffBlendOneOneMaterial);
#endif
        }
        if (m_ScreenBlend)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_ScreenBlend);
#else
                Destroy(m_ScreenBlend);
#endif
        }
        if (m_HollywoodFlaresMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_HollywoodFlaresMaterial);
#else
                Destroy(m_HollywoodFlaresMaterial);
#endif
        }
        if (m_BrightPassFilterMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_BrightPassFilterMaterial);
#else
                Destroy(m_BrightPassFilterMaterial);
#endif
        }
    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CheckResources() == false)
        {
            Graphics.Blit(source, destination);
            return;
        }

        // screen blend is not supported when HDR is enabled (will cap values)

        doHdr = false;
        if (hdr == HDRBloomMode.Auto)
            doHdr = source.format == RenderTextureFormat.ARGBHalf && GetComponent<Camera>().hdr;
        else
        {
            doHdr = hdr == HDRBloomMode.On;
        }

        doHdr = doHdr && supportHDRTextures;

        BloomScreenBlendMode realBlendMode = screenBlendMode;
        if (doHdr)
            realBlendMode = BloomScreenBlendMode.Add;

        var rtFormat = (doHdr) ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.Default;
        RenderTexture halfRezColor = RenderTexture.GetTemporary(source.width / 2, source.height / 2, 0, rtFormat);
        RenderTexture quarterRezColor = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, rtFormat);
        RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, rtFormat);
        RenderTexture thirdQuarterRezColor = RenderTexture.GetTemporary(source.width / 4, source.height / 4, 0, rtFormat);

        float widthOverHeight = (1.0f * source.width) / (1.0f * source.height);
        float oneOverBaseSize = 1.0f / 512.0f;

        // 两次均匀采样
        Graphics.Blit(source, halfRezColor, ScreenBlend, 2);
        Graphics.Blit(halfRezColor, quarterRezColor, ScreenBlend, 2);
        RenderTexture.ReleaseTemporary(halfRezColor);

        // 颜色Cut增量
        BrightFilter(bloomThreshold, useSrcAlphaAsMask, quarterRezColor, secondQuarterRezColor);
        quarterRezColor.DiscardContents();      // 丢弃渲染纹理内容( XBOX平台 )

        // 模糊处理
        if (bloomBlurIterations < 1) bloomBlurIterations = 1;

        for (int iter = 0; iter < bloomBlurIterations; iter++)
        {
            float spreadForPass = (1.0f + (iter * 0.5f)) * sepBlurSpread;

            // 水平方向模糊
            SeparableBlurMaterial.SetVector("offsets", new Vector4(0.0f, spreadForPass * oneOverBaseSize, 0.0f, 0.0f));
            RenderTexture src = iter == 0 ? secondQuarterRezColor : quarterRezColor;
            Graphics.Blit(src, thirdQuarterRezColor, SeparableBlurMaterial);
            src.DiscardContents();

            // 垂直方向模糊
            SeparableBlurMaterial.SetVector("offsets", new Vector4((spreadForPass / widthOverHeight) * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
            Graphics.Blit(thirdQuarterRezColor, quarterRezColor, SeparableBlurMaterial);
            thirdQuarterRezColor.DiscardContents();
        }

        // 处理镜头光晕 ghosting anamorphic combination ( lens flares )
        if (lensflares)
        {
            if (lensflareMode == 0)
            {
                // 增亮处理
                BrightFilter(lensflareThreshold, 0.0f, quarterRezColor, thirdQuarterRezColor);
                quarterRezColor.DiscardContents();

                // smooth a little, this needs to be resolution dependent
                /*
                separableBlurMaterial.SetVector ("offsets", Vector4 (0.0ff, (2.0ff) / (1.0ff * quarterRezColor.height), 0.0ff, 0.0ff));
                Graphics.Blit (thirdQuarterRezColor, secondQuarterRezColor, separableBlurMaterial);
                separableBlurMaterial.SetVector ("offsets", Vector4 ((2.0ff) / (1.0ff * quarterRezColor.width), 0.0ff, 0.0ff, 0.0ff));
                Graphics.Blit (secondQuarterRezColor, thirdQuarterRezColor, separableBlurMaterial);
                */
                // no ugly edges!

                // 暗角处理
                Vignette(0.975f, thirdQuarterRezColor, secondQuarterRezColor);
                thirdQuarterRezColor.DiscardContents();

                // 光晕颜色混合处理
                BlendFlares(secondQuarterRezColor, quarterRezColor);
                secondQuarterRezColor.DiscardContents();
            }

            // 光晕B处理 ( hollywood / anamorphic flares? )
            else
            {

                // thirdQuarter has the brightcut unblurred colors
                // quarterRezColor is the blurred, brightcut buffer that will end up as bloom

                // 好莱坞镜头光晕效果处理

                HollywoodFlaresMaterial.SetVector("_threshold", new Vector4(lensflareThreshold, 1.0f / (1.0f - lensflareThreshold), 0.0f, 0.0f));
                HollywoodFlaresMaterial.SetVector("tintColor", new Vector4(flareColorA.r, flareColorA.g, flareColorA.b, flareColorA.a) * flareColorA.a * lensflareIntensity);
                Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, HollywoodFlaresMaterial, 2);
                thirdQuarterRezColor.DiscardContents();

                Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, HollywoodFlaresMaterial, 3);
                secondQuarterRezColor.DiscardContents();

                HollywoodFlaresMaterial.SetVector("offsets", new Vector4((sepBlurSpread * 1.0f / widthOverHeight) * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
                HollywoodFlaresMaterial.SetFloat("stretchWidth", hollyStretchWidth);
                Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, HollywoodFlaresMaterial, 1);
                thirdQuarterRezColor.DiscardContents();

                HollywoodFlaresMaterial.SetFloat("stretchWidth", hollyStretchWidth * 2.0f);
                Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, HollywoodFlaresMaterial, 1);
                secondQuarterRezColor.DiscardContents();

                HollywoodFlaresMaterial.SetFloat("stretchWidth", hollyStretchWidth * 4.0f);
                Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, HollywoodFlaresMaterial, 1);
                thirdQuarterRezColor.DiscardContents();

                if (lensflareMode == (LensflareStyle34)1)
                {
                    // 好莱坞模糊
                    for (int itera = 0; itera < hollywoodFlareBlurIterations; itera++)
                    {
                        SeparableBlurMaterial.SetVector("offsets", new Vector4((hollyStretchWidth * 2.0f / widthOverHeight) * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
                        Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, SeparableBlurMaterial);
                        secondQuarterRezColor.DiscardContents();

                        SeparableBlurMaterial.SetVector("offsets", new Vector4((hollyStretchWidth * 2.0f / widthOverHeight) * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
                        Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, SeparableBlurMaterial);
                        thirdQuarterRezColor.DiscardContents();
                    }

                    AddTo(1.0f, secondQuarterRezColor, quarterRezColor);
                    secondQuarterRezColor.DiscardContents();
                }
                else
                {

                    // (c) combined

                    for (int ix = 0; ix < hollywoodFlareBlurIterations; ix++)
                    {
                        SeparableBlurMaterial.SetVector("offsets", new Vector4((hollyStretchWidth * 2.0f / widthOverHeight) * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
                        Graphics.Blit(secondQuarterRezColor, thirdQuarterRezColor, SeparableBlurMaterial);
                        secondQuarterRezColor.DiscardContents();

                        SeparableBlurMaterial.SetVector("offsets", new Vector4((hollyStretchWidth * 2.0f / widthOverHeight) * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
                        Graphics.Blit(thirdQuarterRezColor, secondQuarterRezColor, SeparableBlurMaterial);
                        thirdQuarterRezColor.DiscardContents();
                    }

                    // 暗角处理
                    Vignette(1.0f, secondQuarterRezColor, thirdQuarterRezColor);
                    secondQuarterRezColor.DiscardContents();

                    // 光晕混合
                    BlendFlares(thirdQuarterRezColor, secondQuarterRezColor);
                    thirdQuarterRezColor.DiscardContents();

                    AddTo(1.0f, secondQuarterRezColor, quarterRezColor);
                    secondQuarterRezColor.DiscardContents();
                }
            }
        }

        // screen blend bloom results to color buffer

        ScreenBlend.SetFloat("_Intensity", bloomIntensity);
        ScreenBlend.SetTexture("_ColorBuffer", source);
        Graphics.Blit(quarterRezColor, destination, ScreenBlend, (int)realBlendMode);

        RenderTexture.ReleaseTemporary(quarterRezColor);
        RenderTexture.ReleaseTemporary(secondQuarterRezColor);
        RenderTexture.ReleaseTemporary(thirdQuarterRezColor);
    }

    private void AddTo(float intensity_, RenderTexture from, RenderTexture to)
    {
        AddBrightStuffBlendOneOneMaterial.SetFloat("_Intensity", intensity_);
        Graphics.Blit(from, to, AddBrightStuffBlendOneOneMaterial);
    }

    /**********************************************************************************************************
     * 功能 : 增量滤镜
     **********************************************************************************************************/

    private void BlendFlares(RenderTexture from, RenderTexture to)
    {
        LensFlareMaterial.SetVector("colorA", new Vector4(flareColorA.r, flareColorA.g, flareColorA.b, flareColorA.a) * lensflareIntensity);
        LensFlareMaterial.SetVector("colorB", new Vector4(flareColorB.r, flareColorB.g, flareColorB.b, flareColorB.a) * lensflareIntensity);
        LensFlareMaterial.SetVector("colorC", new Vector4(flareColorC.r, flareColorC.g, flareColorC.b, flareColorC.a) * lensflareIntensity);
        LensFlareMaterial.SetVector("colorD", new Vector4(flareColorD.r, flareColorD.g, flareColorD.b, flareColorD.a) * lensflareIntensity);
        Graphics.Blit(from, to, LensFlareMaterial);
    }

    /**********************************************************************************************************
     * 功能 : 增量滤镜
     **********************************************************************************************************/

    private void BrightFilter(float thresh, float useAlphaAsMask, RenderTexture from, RenderTexture to)
    {
        if (doHdr)
            BrightPassFilterMaterial.SetVector("threshold", new Vector4(thresh, 1.0f, 0.0f, 0.0f));
        else
            BrightPassFilterMaterial.SetVector("threshold", new Vector4(thresh, 1.0f / (1.0f - thresh), 0.0f, 0.0f));

        BrightPassFilterMaterial.SetFloat("useSrcAlphaAsMask", useAlphaAsMask);

        Graphics.Blit(from, to, BrightPassFilterMaterial);
    }

    /**********************************************************************************************************
     * 功能 : 
     **********************************************************************************************************/

    private void Vignette(float amount, RenderTexture from, RenderTexture to)
    {
        // 纹理暗角处理
        if (lensFlareVignetteMask)
        {
            ScreenBlend.SetTexture("_ColorBuffer", lensFlareVignetteMask);
            Graphics.Blit(from, to, ScreenBlend, 3);
        }
        // 程序暗角处理
        else
        {
            VignetteMaterial.SetFloat("vignetteIntensity", amount);
            Graphics.Blit(from, to, VignetteMaterial);
        }
    }

}