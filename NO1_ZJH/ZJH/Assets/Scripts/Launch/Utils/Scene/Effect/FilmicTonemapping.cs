using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FilmicTonemapping : PEBase
{
    public Shader shader;

    private int iTexColor;
    private int iTexColor1;
    private int iTexelOffset;
    private int iHDRClampAndExposure;
    private int iHDRFilmCurve;
    private int iLumScale;
    private int iCutOffAndPower;

    // Use this for initialization
    void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        if (material == null)
        {
            enabled = false;
            return;
        }

        if (!shader || !shader.isSupported || !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGB32))
            enabled = false;

        FrameHDRReduce = new RenderTexture[TEX_HDR_REDUCE_NUM];
        FrameHDRLuminance = new RenderTexture[TEX_LUM_NUM];

        iTexColor = Shader.PropertyToID("texColor");
        iTexColor1 = Shader.PropertyToID("texColor1");
        iTexelOffset = Shader.PropertyToID("c_vTexelOffset");
        iHDRClampAndExposure = Shader.PropertyToID("c_vHDRClampAndExposure");
        iHDRFilmCurve = Shader.PropertyToID("c_HDRFilmCurve");
        iLumScale = Shader.PropertyToID("c_fLumScale");
        iCutOffAndPower = Shader.PropertyToID("iCutOffAndPower");
    }

    override public Material material
    {
        get
        {
            if (m_Material == null)
            {
                if (shader == null)
                    shader = Shader.Find("Snail/Effect/Filmic Tonemapping");

                m_Material = new Material(shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
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
            _matParams.Add("c_HDRClampMax", (int)MatParamType.Type_Float);
            _matParams.Add("c_HDRClampMin", (int)MatParamType.Type_Float);
            _matParams.Add("c_vExposure", (int)MatParamType.Type_Float);
            _matParams.Add("c_HDRCurveShoulder", (int)MatParamType.Type_Float);
            _matParams.Add("c_HDRCurveMidtone", (int)MatParamType.Type_Float);
            _matParams.Add("c_HDRCurveToe", (int)MatParamType.Type_Float);
            _matParams.Add("c_HDRWhitePoint", (int)MatParamType.Type_Float);
            return _matParams;
        }
    }

    override public void LoadParams()
    {
        m_fHDRClampMax = varData.GetFloat("c_HDRClampMax");
        m_fHDRClampMin = varData.GetFloat("c_HDRClampMin");
        m_fExposure = varData.GetFloat("c_vExposure");
        m_fHDRCurveShoulder = varData.GetFloat("c_HDRCurveShoulder");
        m_fHDRCurveMidtone = varData.GetFloat("c_HDRCurveMidtone");
        m_fHDRCurveToe = varData.GetFloat("c_HDRCurveToe");
        m_fHDRWhitePoint = varData.GetFloat("c_HDRWhitePoint");
    }


    override public void SetVarData()
    {
        varData.SetFloat("c_HDRClampMax", m_fHDRClampMax);
        varData.SetFloat("c_HDRClampMin", m_fHDRClampMin);
        varData.SetFloat("c_vExposure", m_fExposure);
        varData.SetFloat("c_HDRCurveShoulder", m_fHDRCurveShoulder);
        varData.SetFloat("c_HDRCurveMidtone", m_fHDRCurveMidtone);
        varData.SetFloat("c_HDRCurveToe", m_fHDRCurveToe);
        varData.SetFloat("c_HDRWhitePoint", m_fHDRWhitePoint);
    }

    static int TEX_HDR_REDUCE_NUM = 3;
    static int TEX_LUM_NUM = 2;
    int m_nCurLumIndex = 0;

    private float m_fEyeResponse = 1.0f;
    private float m_fDeltaTime = 0.0f;
    public float m_fHDRClampMax = 2.1f;
    public float m_fHDRClampMin = 0.2f;
    public float m_fExposure = 1.0f;

    public float m_fHDRCurveShoulder = 1.0f;
    public float m_fHDRCurveMidtone = 1.0f;
    public float m_fHDRCurveToe = 1.5f;
    public float m_fHDRWhitePoint = 1.0f;

    private float m_fCutoff = 0.9f;
    private float m_fPower = 0.25f;
    private float lum_scale = 4f;

    private static float RT_DOWNSAMPLE_SCALE = 0.3f;
    private RenderTexture[] FrameHDRReduce;
    RenderTexture[] FrameHDRLuminance;

	
	// Update is called once per frame
	void Update ()
	{
	    //m_fDeltaTime = Time.deltaTime;

	}

    void OnDestroy()
    {

        if (m_Material)
        {
            for (int i = 0; FrameHDRLuminance != null && i < FrameHDRLuminance.Length; ++i)
            {
                RenderTexture rt = FrameHDRLuminance[i];

                if (rt)
                {
                    rt.Release();
                    rt = null;
                }
            }

#if UNITY_EDITOR
            DestroyImmediate(m_Material);
#else
                Destroy(m_Material);
#endif
        }
    }

    void OnDisable()
    {
        if (m_Material)
        {
#if UNITY_EDITOR
            DestroyImmediate(m_Material);
#else
                Destroy(m_Material);
#endif
        }
    }

    // attribute indicates that the image filter chain will continue in LDR
    //[ImageEffectTransformsToLDR]
    void OnRenderImage(RenderTexture source, RenderTexture dest)
    {
        if (shader == null)
        {
            Graphics.Blit(source, dest);
            enabled = false;
            return;
        }

        float pixelSizeX;
        float pixelSizeY;
        RenderTexture rt = null;

        // 降采样 产出为 FrameSingleHDRPixel
        FrameHDRReduce[0] = RenderTexture.GetTemporary((int)(source.width * RT_DOWNSAMPLE_SCALE), (int)(source.height * RT_DOWNSAMPLE_SCALE), 0, RenderTextureFormat.ARGB32);
        material.SetTexture(iTexColor, source);
        Graphics.Blit(rt, FrameHDRReduce[0], material, 0);

#if UNITY_EDITOR
        tIndex = 0;
        AddRTs((int)(source.width * RT_DOWNSAMPLE_SCALE), (int)(source.height * RT_DOWNSAMPLE_SCALE), FrameHDRReduce[0]);
#endif

        for (int i = 1; i < TEX_HDR_REDUCE_NUM; i++)
        {
            FrameHDRReduce[i] = RenderTexture.GetTemporary((int)(FrameHDRReduce[i - 1].width * RT_DOWNSAMPLE_SCALE), (int)(FrameHDRReduce[i - 1].height * RT_DOWNSAMPLE_SCALE), 0, RenderTextureFormat.ARGB32);

            material.SetTexture(iTexColor, FrameHDRReduce[i - 1]);
            pixelSizeX = 1.0f / FrameHDRReduce[i - 1].width;
            pixelSizeY = 1.0f / FrameHDRReduce[i - 1].height;
            material.SetVector(iTexelOffset, new Vector4(pixelSizeX, pixelSizeY, 0, 0));
            Graphics.Blit(rt, FrameHDRReduce[i], material, 1);
            RenderTexture.ReleaseTemporary(FrameHDRReduce[i - 1]);

#if UNITY_EDITOR
            AddRTs((int)(FrameHDRReduce[i - 1].width * RT_DOWNSAMPLE_SCALE), (int)(FrameHDRReduce[i - 1].height * RT_DOWNSAMPLE_SCALE), FrameHDRReduce[i]);
#endif
        }

//        RenderTexture FrameSingleHDRPixel = RenderTexture.GetTemporary((int)(FrameHDRReduce[TEX_HDR_REDUCE_NUM - 1].width * RT_DOWNSAMPLE_SCALE), (int)(FrameHDRReduce[TEX_HDR_REDUCE_NUM - 1].height * RT_DOWNSAMPLE_SCALE), 0, RenderTextureFormat.ARGB32);
//        material.SetTexture(iTexColor, FrameHDRReduce[TEX_HDR_REDUCE_NUM - 1]);
//        pixelSizeX = 1.0f / FrameHDRReduce[TEX_HDR_REDUCE_NUM - 1].width;
//        pixelSizeY = 1.0f / FrameHDRReduce[TEX_HDR_REDUCE_NUM - 1].height;
//        Graphics.Blit(rt, FrameSingleHDRPixel, material, 1);
//        RenderTexture.ReleaseTemporary(FrameHDRReduce[TEX_HDR_REDUCE_NUM - 1]);

//#if UNITY_EDITOR
//        AddRTs((int)(FrameHDRReduce[TEX_HDR_REDUCE_NUM - 1].width * RT_DOWNSAMPLE_SCALE), (int)(FrameHDRReduce[TEX_HDR_REDUCE_NUM - 1].height * RT_DOWNSAMPLE_SCALE), FrameSingleHDRPixel);
//#endif

        //adapt lum
        //if (FrameHDRLuminance[m_nCurLumIndex] == null)
        //{
        //    FrameHDRLuminance[m_nCurLumIndex] = new RenderTexture(FrameSingleHDRPixel.width, FrameSingleHDRPixel.height, 0, RenderTextureFormat.ARGBHalf);   
        //}
        //int preFrameLumIndex = m_nCurLumIndex > 0 ? m_nCurLumIndex - 1 : (TEX_LUM_NUM - 1);

        //material.SetTexture("texColor", FrameHDRLuminance[preFrameLumIndex]);
        //material.SetTexture("texColor1", FrameSingleHDRPixel);
        //material.SetFloat("c_fLumDelay", m_fEyeResponse * m_fDeltaTime);
        //Graphics.Blit(rt, FrameHDRLuminance[m_nCurLumIndex], material, 2);

#if UNITY_EDITOR
        SetVarData();
#endif

        //tone mapping
        //material.SetTexture("texColor2", FrameQuratRT1); // 已经在bloom的阶段叠加
        material.SetTexture(iTexColor, source);
        material.SetTexture(iTexColor1, FrameHDRReduce[TEX_HDR_REDUCE_NUM - 1]);

        material.SetVector(iHDRClampAndExposure, new Vector4(m_fHDRClampMin, m_fHDRClampMax, m_fExposure));
        material.SetVector(iHDRFilmCurve, new Vector4(m_fHDRCurveShoulder, m_fHDRCurveMidtone, m_fHDRCurveToe, m_fHDRWhitePoint));
        material.SetFloat(iLumScale, lum_scale);
        material.SetVector(iCutOffAndPower, new Vector4(m_fCutoff, m_fPower));
        RenderTexture.ReleaseTemporary(FrameHDRReduce[TEX_HDR_REDUCE_NUM - 1]);
        Graphics.Blit(rt, dest, material, 3);

#if UNITY_EDITOR

        //AddRTs(rt.width, rt.height, dest);
        RemoveUnUsedRTs();
#endif

        m_nCurLumIndex = (m_nCurLumIndex + 1) % TEX_LUM_NUM;    
    }


}

#if UNITY_EDITOR

[ExecuteInEditMode]
[CustomEditor(typeof(FilmicTonemapping))]
public class FilmicTonemappingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        FilmicTonemapping pe = target as FilmicTonemapping;
        if (pe.tRenderTextures != null && pe.tRenderTextures.Count != 0)
        {
            pe.showPosition = EditorGUILayout.Foldout(pe.showPosition, "预览面板");
            if (pe.showPosition)
            {
                GUILayout.BeginVertical();
                bool doubleClick = false;

                int columns = (int)Mathf.Ceil((Screen.width - 20) / 128);
                int rows = pe.tRenderTextures.Count / columns;
                if (pe.tRenderTextures.Count % columns != 0)
                    rows++;
                Rect r = GUILayoutUtility.GetAspectRect(columns / (float)rows);
                Rect r2 = GUILayoutUtility.GetRect(10, 48 * rows);
                r.height -= r2.height;
                Event evt = Event.current;
                if (evt.type == EventType.MouseDown && evt.clickCount == 2 && r.Contains(evt.mousePosition))
                {
                    doubleClick = true;
                    evt.Use();
                }

                GUIContent[] rtContents = new GUIContent[pe.tRenderTextures.Count];

                for (int i = 0; i < rtContents.Length; ++i)
                {
                    rtContents[i] = new GUIContent();

                    rtContents[i].text = pe.tRenderTextures[i].name;
                    rtContents[i].image = pe.tRenderTextures[i];
                }

                pe.selectIndex = GUI.SelectionGrid(r, pe.selectIndex, rtContents, columns, "GridListText");

                GUILayout.EndVertical();

                if (doubleClick)
                {
                    PEEditWindow peWindow
                        = EditorWindow.GetWindow(typeof(PEEditWindow)) as PEEditWindow;
                    peWindow.Init(pe.tRenderTextures[pe.selectIndex]);
                    peWindow.Show();

                    GUIUtility.ExitGUI();
                }
            }
        }

        Repaint();
    }

}

#endif