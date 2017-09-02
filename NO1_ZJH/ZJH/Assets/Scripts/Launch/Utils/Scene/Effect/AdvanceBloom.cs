using System;
using UnityEngine;
using System.Collections.Generic;


public enum LensFlareStyle
{
    Ghosting = 0,
    Anamorphic = 1,
    Combined = 2,
}

public enum TweakMode
{
    Basic = 0,
    Complex = 1,
}

public enum HDRBloomMode
{
    Auto = 0,
    On = 1,
    Off = 2,
}

public enum BloomScreenBlendMode
{
    Screen = 0,
    Add = 1,
}

public enum BloomQuality
{
    Cheap = 0,
    High = 1,
}


[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AdvanceBloom : PEBase
{
    protected virtual void Start()
    {
        if (CheckResources() == false)
        {
            enabled = false;
            return;
        }
    }

    public TweakMode tweakMode = 0;
    public BloomScreenBlendMode screenBlendMode = BloomScreenBlendMode.Add;

    public HDRBloomMode hdr = HDRBloomMode.Auto;
    private bool doHdr = false;
    public float sepBlurSpread = 10f;

    public BloomQuality quality = BloomQuality.High;

    public float bloomIntensity = 1f;
    public float bloomThreshold = 0.4f;
    public Color bloomThresholdColor = Color.white;
    public int bloomBlurIterations = 4;

    public int hollywoodFlareBlurIterations = 2;
    public float flareRotation = 0.0f;
    public LensFlareStyle lensflareMode = (LensFlareStyle)1;
    public float hollyStretchWidth = 2.5f;
    public float lensflareIntensity = 0.0f;
    public float lensflareThreshold = 0.3f;
    public float lensFlareSaturation = 0.75f;
    public Color flareColorA = new Color(0.4f, 0.4f, 0.8f, 0.75f);
    public Color flareColorB = new Color(0.4f, 0.8f, 0.8f, 0.75f);
    public Color flareColorC = new Color(0.8f, 0.4f, 0.8f, 0.75f);
    public Color flareColorD = new Color(0.8f, 0.4f, 0.0f, 0.75f);
    public Texture2D lensFlareVignetteMask;

    public Shader lensFlareShader;
    public Material m_LensFlareMaterial;
    private Material LensFlareMaterial
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


    public Shader screenBlendShader;
    private Material m_ScreenBlend;
    private Material ScreenBlend
    {
        get
        {
            if (m_ScreenBlend == null)
            {
                if (screenBlendShader == null)
                    screenBlendShader = Shader.Find("Snail/Effect/BlendForBloom");

                m_ScreenBlend = new Material(screenBlendShader);
                m_ScreenBlend.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_ScreenBlend;
        }
    }

    public Shader blurAndFlaresShader;
    private Material m_BlurAndFlaresMaterial;

    private Material BlurAndFlaresMaterial
    {
        get
        {
            if (m_BlurAndFlaresMaterial == null)
            {
                if (blurAndFlaresShader == null)
                    blurAndFlaresShader = Shader.Find("Snail/Effect/BlurAndFlares");

                m_BlurAndFlaresMaterial = new Material(blurAndFlaresShader);
                m_BlurAndFlaresMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_BlurAndFlaresMaterial;
        }
    }

    public Shader brightPassFilterShader;
    private Material m_BrightPassFilterMaterial;

    private Material BrightPassFilterMaterial
    {
        get
        {
            if (m_BrightPassFilterMaterial == null)
            {
                if (brightPassFilterShader == null)
                    brightPassFilterShader = Shader.Find("Snail/Effect/BrightPassFilter2");

                m_BrightPassFilterMaterial = new Material(brightPassFilterShader);
                m_BrightPassFilterMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_BrightPassFilterMaterial;
        }
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
            _matParams.Add("_pScreenBlendMode", (int)MatParamType.Type_Int);
            _matParams.Add("_pBloomIntensity", (int)MatParamType.Type_Float);
            _matParams.Add("_pBloomThresholdColor", (int)MatParamType.Type_Color);
            _matParams.Add("_pBloomThreshold", (int)MatParamType.Type_Float);
            _matParams.Add("_pBloomBlurIterations", (int)MatParamType.Type_Int);
            _matParams.Add("_psSepBlurSpread", (int)MatParamType.Type_Float);
            return _matParams;
        }
    }

    override public void LoadParams()
    {
        screenBlendMode = (BloomScreenBlendMode)varData.GetInt("_pScreenBlendMode");
        bloomIntensity = varData.GetFloat("_pBloomIntensity");
        bloomThresholdColor = varData.GetColor("_pBloomThresholdColor");
        bloomThreshold = varData.GetFloat("_pBloomThreshold");
        bloomBlurIterations = varData.GetInt("_pBloomBlurIterations");
        sepBlurSpread = varData.GetFloat("_psSepBlurSpread");
    }


    public bool CheckResources()
    {
        CheckSupport(false);

        CheckMaterial(LensFlareMaterial);
        CheckMaterial(ScreenBlend);
        CheckMaterial(BlurAndFlaresMaterial);
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
        if (m_ScreenBlend)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_ScreenBlend);
#else
                Destroy(m_ScreenBlend);
#endif
        }
        if (m_BlurAndFlaresMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_BlurAndFlaresMaterial);
#else
                Destroy(m_BlurAndFlaresMaterial);
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
        if (m_ScreenBlend)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_ScreenBlend);
#else
                Destroy(m_ScreenBlend);
#endif
        }
        if (m_BlurAndFlaresMaterial)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_BlurAndFlaresMaterial);
#else
                Destroy(m_BlurAndFlaresMaterial);
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

    override public void SetVarData()
    {
        // 存储太阳光位置
        varData.SetInt("_pScreenBlendMode", (int)screenBlendMode);
        varData.SetFloat("_pBloomIntensity", bloomIntensity);
        varData.SetFloat("_pBloomThreshold", bloomThreshold);
        varData.SetFloat("_psSepBlurSpread", sepBlurSpread);
        varData.SetColor("_pBloomThresholdColor", bloomThresholdColor);
        varData.SetInt("_pBloomBlurIterations", bloomBlurIterations);
        varData.SetFloat("_psSepBlurSpread", sepBlurSpread);
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (CheckResources() == false)
        {
            Graphics.Blit(source, destination);
            enabled = false;
            return;
        }

        SetVarData();
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

        // -------------------------------------------------------------------------------------------------------
        //  Set RenderTexture Size
        var rtW2 = source.width / 2;
        var rtH2 = source.height / 2;
        var rtW4 = source.width / 4;
        var rtH4 = source.height / 4;

        float widthOverHeight = (1.0f * source.width) / (1.0f * source.height);
        float oneOverBaseSize = 1.0f / 512.0f;

        // --------------------------------------------------------------------------------------------------------
        //  Downsample 
        RenderTexture quarterRezColor = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
        RenderTexture halfRezColorDown = RenderTexture.GetTemporary(rtW2, rtH2, 0, rtFormat);
        if (quality > BloomQuality.Cheap)
        {
            // Get Max Color Value
            Graphics.Blit(source, halfRezColorDown, ScreenBlend, 2);
            RenderTexture rtDown4 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
            Graphics.Blit(halfRezColorDown, rtDown4, ScreenBlend, 2);

            // Simple Blur
            Graphics.Blit(rtDown4, quarterRezColor, ScreenBlend, 6);
            RenderTexture.ReleaseTemporary(rtDown4);
        }
        else
        {
            // 
            Graphics.Blit(source, halfRezColorDown);
            Graphics.Blit(halfRezColorDown, quarterRezColor, ScreenBlend, 6);
        }
        RenderTexture.ReleaseTemporary(halfRezColorDown);

        // --------------------------------------------------------------------------------------------------------
        //  Cut colors (thresholding)
        RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
        BrightFilter(bloomThreshold * bloomThresholdColor, quarterRezColor, secondQuarterRezColor);
            



        // ---------------------------------------------------------------------------------------------------------
        //  Blurring
        if (bloomBlurIterations < 1) bloomBlurIterations = 1;
        else if (bloomBlurIterations > 10) bloomBlurIterations = 10;

        for (int iter = 0; iter < bloomBlurIterations; iter++)
        {
            float spreadForPass = (1.0f + (iter * 0.25f)) * sepBlurSpread;

            // Vertical blur
            RenderTexture blur4 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
            BlurAndFlaresMaterial.SetVector("_Offsets", new Vector4(0.0f, spreadForPass * oneOverBaseSize, 0.0f, 0.0f));
            Graphics.Blit(secondQuarterRezColor, blur4, BlurAndFlaresMaterial, 4);
            RenderTexture.ReleaseTemporary(secondQuarterRezColor);
            secondQuarterRezColor = blur4;

            // Horizontal blur
            blur4 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);
            BlurAndFlaresMaterial.SetVector("_Offsets", new Vector4((spreadForPass / widthOverHeight) * oneOverBaseSize, 0.0f, 0.0f, 0.0f));
            Graphics.Blit(secondQuarterRezColor, blur4, BlurAndFlaresMaterial, 4);
            RenderTexture.ReleaseTemporary(secondQuarterRezColor);
            secondQuarterRezColor = blur4;

 
            if (quality > BloomQuality.Cheap)
            {
                if (iter == 0)
                {
                    Graphics.SetRenderTarget(quarterRezColor);
                    GL.Clear(false, true, Color.black); // Clear to avoid RT restore
                    Graphics.Blit(secondQuarterRezColor, quarterRezColor);
                }
                else
                {
                    quarterRezColor.MarkRestoreExpected(); // using max blending, RT restore expected
                    Graphics.Blit(secondQuarterRezColor, quarterRezColor, ScreenBlend, 10);
                }
            }
        }

        // -------------------------------------------------------------------------------------------------------------------------
        //  

        if (quality > BloomQuality.Cheap)
        {
            Graphics.SetRenderTarget(secondQuarterRezColor);
            GL.Clear(false, true, Color.black); 
            // Clear to avoid RT restore
            Graphics.Blit(quarterRezColor, secondQuarterRezColor, ScreenBlend, 6);
        }

        // ----------------------------------------------------------------------------------------------------------------------------
        //  Lens flares: ghosting, anamorphic or both (ghosted anamorphic flares) 
        if (lensflareIntensity > Mathf.Epsilon)
        {
            RenderTexture rtFlares4 = RenderTexture.GetTemporary(rtW4, rtH4, 0, rtFormat);

            if (lensflareMode == 0)
            {
                // Ghosting only
                BrightFilter(lensflareThreshold, secondQuarterRezColor, rtFlares4);

                if (quality > BloomQuality.Cheap)
                {
                    // Smooth a little
                    BlurAndFlaresMaterial.SetVector("_Offsets", new Vector4(0.0f, (1.5f) / (1.0f * quarterRezColor.height), 0.0f, 0.0f));
                    Graphics.SetRenderTarget(quarterRezColor);
                    GL.Clear(false, true, Color.black); // Clear to avoid RT restore
                    Graphics.Blit(rtFlares4, quarterRezColor, BlurAndFlaresMaterial, 4);

                    BlurAndFlaresMaterial.SetVector("_Offsets", new Vector4((1.5f) / (1.0f * quarterRezColor.width), 0.0f, 0.0f, 0.0f));
                    Graphics.SetRenderTarget(rtFlares4);
                    GL.Clear(false, true, Color.black); // Clear to avoid RT restore
                    Graphics.Blit(quarterRezColor, rtFlares4, BlurAndFlaresMaterial, 4);
                }

                // No ugly edges!
                Vignette(0.975f, rtFlares4, rtFlares4);

                // Create Flares
                BlendFlares(rtFlares4, secondQuarterRezColor);
            }
            else
            {
                // Vignette (0.975ff, rtFlares4, rtFlares4);
                // DrawBorder(rtFlares4, screenBlend, 8);

                float flareXRot = 1.0f * Mathf.Cos(flareRotation);
                float flareyRot = 1.0f * Mathf.Sin(flareRotation);

                float stretchWidth = (hollyStretchWidth * 1.0f / widthOverHeight) * oneOverBaseSize;

                BlurAndFlaresMaterial.SetVector("_Offsets", new Vector4(flareXRot, flareyRot, 0.0f, 0.0f));
                BlurAndFlaresMaterial.SetVector("_Threshhold", new Vector4(lensflareThreshold, 1.0f, 0.0f, 0.0f));
                BlurAndFlaresMaterial.SetVector("_TintColor", new Vector4(flareColorA.r, flareColorA.g, flareColorA.b, flareColorA.a) * flareColorA.a * lensflareIntensity);
                BlurAndFlaresMaterial.SetFloat("_Saturation", lensFlareSaturation);

                // Pre And Cut"
                quarterRezColor.DiscardContents();
                Graphics.Blit(rtFlares4, quarterRezColor, BlurAndFlaresMaterial, 2);

                // Post
                rtFlares4.DiscardContents();
                Graphics.Blit(quarterRezColor, rtFlares4, BlurAndFlaresMaterial, 3);

                BlurAndFlaresMaterial.SetVector("_Offsets", new Vector4(flareXRot * stretchWidth, flareyRot * stretchWidth, 0.0f, 0.0f));

                // Stretch 1st
                BlurAndFlaresMaterial.SetFloat("_StretchWidth", hollyStretchWidth);
                quarterRezColor.DiscardContents();
                Graphics.Blit(rtFlares4, quarterRezColor, BlurAndFlaresMaterial, 1);
                // Stretch 2nd
                BlurAndFlaresMaterial.SetFloat("_StretchWidth", hollyStretchWidth * 2.0f);
                rtFlares4.DiscardContents();
                Graphics.Blit(quarterRezColor, rtFlares4, BlurAndFlaresMaterial, 1);
                // Stretch 3rd
                BlurAndFlaresMaterial.SetFloat("_StretchWidth", hollyStretchWidth * 4.0f);
                quarterRezColor.DiscardContents();
                Graphics.Blit(rtFlares4, quarterRezColor, BlurAndFlaresMaterial, 1);

                // Additional blur passes
                for (int iter = 0; iter < hollywoodFlareBlurIterations; iter++)
                {
                    stretchWidth = (hollyStretchWidth * 2.0f / widthOverHeight) * oneOverBaseSize;

                    BlurAndFlaresMaterial.SetVector("_Offsets", new Vector4(stretchWidth * flareXRot, stretchWidth * flareyRot, 0.0f, 0.0f));
                    rtFlares4.DiscardContents();
                    Graphics.Blit(quarterRezColor, rtFlares4, BlurAndFlaresMaterial, 4);

                    BlurAndFlaresMaterial.SetVector("_Offsets", new Vector4(stretchWidth * flareXRot, stretchWidth * flareyRot, 0.0f, 0.0f));
                    quarterRezColor.DiscardContents();
                    Graphics.Blit(rtFlares4, quarterRezColor, BlurAndFlaresMaterial, 4);
                }

                if (lensflareMode == (LensFlareStyle)1)
                    // anamorphic lens flares
                    AddTo(1.0f, quarterRezColor, secondQuarterRezColor);
                else
                {
                    // "combined" lens flares

                    Vignette(1.0f, quarterRezColor, rtFlares4);
                    BlendFlares(rtFlares4, quarterRezColor);
                    AddTo(1.0f, quarterRezColor, secondQuarterRezColor);
                }
            }
            RenderTexture.ReleaseTemporary(rtFlares4);
        }

        int blendPass = (int)realBlendMode;
        //if (Mathf.Abs(chromaticBloom) < Mathf.Epsilon)
        //	blendPass += 4;

        ScreenBlend.SetFloat("_Intensity", bloomIntensity);
        ScreenBlend.SetTexture("_ColorBuffer", source);

        if (quality > BloomQuality.Cheap)
        {
            RenderTexture halfRezColorUp = RenderTexture.GetTemporary(rtW2, rtH2, 0, rtFormat);
            Graphics.Blit(secondQuarterRezColor, halfRezColorUp);
            Graphics.Blit(halfRezColorUp, destination, ScreenBlend, blendPass);
            RenderTexture.ReleaseTemporary(halfRezColorUp);
        }
        else
            Graphics.Blit(secondQuarterRezColor, destination, ScreenBlend, blendPass);

        RenderTexture.ReleaseTemporary(quarterRezColor);
        RenderTexture.ReleaseTemporary(secondQuarterRezColor);
    }

    /***********************************************************************************************************************
     * Function : Blend Add To
     ***********************************************************************************************************************/

    private void AddTo(float intensity_, RenderTexture from, RenderTexture to)
    {
        ScreenBlend.SetFloat("_Intensity", intensity_);
        to.MarkRestoreExpected(); // additive blending, RT restore expected
        Graphics.Blit(from, to, ScreenBlend, 9);
    }

    /***********************************************************************************************************************
     * Function : Blend Flares
     ***********************************************************************************************************************/

    private void BlendFlares(RenderTexture from, RenderTexture to)
    {
        LensFlareMaterial.SetVector("colorA", new Vector4(flareColorA.r, flareColorA.g, flareColorA.b, flareColorA.a) * lensflareIntensity);
        LensFlareMaterial.SetVector("colorB", new Vector4(flareColorB.r, flareColorB.g, flareColorB.b, flareColorB.a) * lensflareIntensity);
        LensFlareMaterial.SetVector("colorC", new Vector4(flareColorC.r, flareColorC.g, flareColorC.b, flareColorC.a) * lensflareIntensity);
        LensFlareMaterial.SetVector("colorD", new Vector4(flareColorD.r, flareColorD.g, flareColorD.b, flareColorD.a) * lensflareIntensity);
        to.MarkRestoreExpected(); // additive blending, RT restore expected
        Graphics.Blit(from, to, LensFlareMaterial);
    }

    /***********************************************************************************************************************
     * Function : Bright Filter By Threshhold
     ***********************************************************************************************************************/

    private void BrightFilter(float thresh, RenderTexture from, RenderTexture to)
    {
        BrightPassFilterMaterial.SetVector("_Threshhold", new Vector4(thresh, thresh, thresh, thresh));
        Graphics.Blit(from, to, BrightPassFilterMaterial, 0);
    }

    /***********************************************************************************************************************
     * Function : Bright Filter By Color
     ***********************************************************************************************************************/

    private void BrightFilter(Color threshColor, RenderTexture from, RenderTexture to)
    {
        BrightPassFilterMaterial.SetVector("_Threshhold", threshColor);
        Graphics.Blit(from, to, BrightPassFilterMaterial, 1);
    }

    /***********************************************************************************************************************
     * Function : Vignette
     ***********************************************************************************************************************/

    private void Vignette(float amount, RenderTexture from, RenderTexture to)
    {
        if (lensFlareVignetteMask)
        {
            ScreenBlend.SetTexture("_ColorBuffer", lensFlareVignetteMask);
            to.MarkRestoreExpected(); // using blending, RT restore expected
            Graphics.Blit(from == to ? null : from, to, ScreenBlend, from == to ? 7 : 3);
        }
        else if (from != to)
        {
            Graphics.SetRenderTarget(to);
            GL.Clear(false, true, Color.black); // clear destination to avoid RT restore
            Graphics.Blit(from, to);
        }
    }
}