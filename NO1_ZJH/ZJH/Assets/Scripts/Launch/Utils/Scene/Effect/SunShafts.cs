using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SunShafts : PEBase
{
    public Shader shader;

    private int iBlurRadius4;
    private int iSunPosition;
    private int iSunThreshold;
    private int iSkybox;
    private int iSunColor;
    private int iColorBuffer;

    protected virtual void Start()
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

        if (!shader || !shader.isSupported)
            enabled = false;

        iBlurRadius4 = Shader.PropertyToID("_BlurRadius4");
        iSunPosition = Shader.PropertyToID("_SunPosition");
        iSunThreshold = Shader.PropertyToID("_SunThreshold");
        iSkybox = Shader.PropertyToID("_Skybox");
        iSunColor = Shader.PropertyToID("_SunColor");
        iColorBuffer = Shader.PropertyToID("_ColorBuffer");

    }


    public enum SunShaftsResolution
    {
        Low = 0,
        Normal = 1,
        High = 2,
    }

    public enum ShaftsScreenBlendMode
    {
        Screen = 0,
        Add = 1,
    }

    public SunShaftsResolution resolution = SunShaftsResolution.Normal;
    public ShaftsScreenBlendMode screenBlendMode = ShaftsScreenBlendMode.Screen;

    public Transform sunTransform;
    public int radialBlurIterations = 2;
    public Color sunColor = new Color(1f, 1f, 1f, 1f);
    public Color sunThreshold = new Color(0f, 0f, 0f); 
    public float sunShaftBlurRadius = 10f;
    public float sunShaftIntensity = 1.15f;

    public float maxRadius = 0.75f;

    public bool useDepthTexture = false;

    override public Material material
    {
        get
        {
            if (m_Material == null)
            {
                if (shader == null)
                    shader = Shader.Find("Snail/Effect/SunShaftsComposite");

                m_Material = new Material(shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
        }
    }


    public GameObject sun;

    public Vector4 sunPositon;

    /// <summary>
    /// 主像机
    /// </summary>
    private Camera mCamera = null;

    private Vector4 vBlurRadius4;

    private Vector4 vsunPosition;

    private Vector4 vSunColor;

    override public Dictionary<string, int> matParams
    {
        get
        {
            if (_matParams == null)
            {
                _matParams = new Dictionary<string, int>();
            }
            _matParams.Clear();
            _matParams.Add("_pSunColor", (int)MatParamType.Type_Color);
            _matParams.Add("_pSunThreshold", (int)MatParamType.Type_Color);
            _matParams.Add("_SunPostion", (int)MatParamType.Type_Vector4);
            _matParams.Add("_pRadialBlurIterations", (int)MatParamType.Type_Int);         // 径向模糊次数
            _matParams.Add("_pSunShaftBlurRadius", (int)MatParamType.Type_Float);         // 模糊半径
            _matParams.Add("_pSunShaftIntensity", (int)MatParamType.Type_Float);          // 体积光强度

            return _matParams;
        }
    }

    private void CheckSun()
    {
        if (sun == null)
        {
            sun = GameObject.Find("sunLight");
            if (sun != null)
                sunTransform = sun.transform;
        }
    }


    override public void LoadParams()
    {
        sunColor = varData.GetColor("_pSunColor");
        sunThreshold = varData.GetColor("_pSunThreshold");
        sunPositon = varData.GetVector("_SunPostion");
        radialBlurIterations = varData.GetInt("_pRadialBlurIterations");
        sunShaftBlurRadius = varData.GetFloat("_pSunShaftBlurRadius");
        sunShaftIntensity = varData.GetFloat("_pSunShaftIntensity");

        CheckSun();

        if (sun != null)
            sun.transform.position = new Vector3(sunPositon.x, sunPositon.y, sunPositon.z);
    }


    override public void SetVarData()
    {
        // 存储太阳光位置
        varData.SetColor("_pSunColor", sunColor);
        varData.SetColor("_pSunThreshold", sunThreshold);
        varData.SetVector("_SunPostion", sunPositon);
        varData.SetFloat("_pRadialBlurIterations", radialBlurIterations);
        varData.SetFloat("_pSunShaftBlurRadius", sunShaftBlurRadius);
        varData.SetFloat("_pSunShaftIntensity", sunShaftIntensity);
    }

    void Awake()
    {
        mCamera = GetComponent<Camera>();
        vBlurRadius4 = new Vector4(1.0f, 1.0f, 0.0f, 0.0f);
    }


    void OnDestroy()
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


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mCamera == null)
        {
            Graphics.Blit(source, destination);
            enabled = false;
            return;
        }

        CheckSun();

        if (material == null || sun == null)
        {
            Graphics.Blit(source, destination);
            enabled = false;
            return;
        }

        int divider = 4;
        if (resolution == SunShaftsResolution.Normal)
            divider = 2;
        else if (resolution == SunShaftsResolution.High)
            divider = 1;

        Vector3 v = Vector3.one * 0.5f;
        if (sunTransform != null)
        {
            v = -sunTransform.forward * 1000;
            v = mCamera.WorldToViewportPoint(v);
        }
        else
            v = new Vector3(0.5f, 0.5f, 0.0f);

        int rtW = source.width / divider;
        int rtH = source.height / divider;

        RenderTexture lrColorB; 
        RenderTexture lrDepthBuffer = RenderTexture.GetTemporary(rtW, rtH, 0);

#if UNITY_EDITOR
        tIndex = 0;

        SetVarData();
#endif
        vsunPosition.x = v.x;
        vsunPosition.y = v.y;
        vsunPosition.z = v.z;
        vsunPosition.w = maxRadius;
        // 设置模糊值及太阳位置和太阳光强度
        material.SetVector(iBlurRadius4, vBlurRadius4 * sunShaftBlurRadius);
        material.SetVector(iSunPosition, vsunPosition);
        material.SetVector(iSunThreshold, sunThreshold);

        // 渲染深度缓存
        if (!useDepthTexture)
        {
            var format = mCamera.hdr ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
            RenderTexture tmpBuffer = RenderTexture.GetTemporary(source.width, source.height, 0, format);
            RenderTexture.active = tmpBuffer;
            GL.ClearWithSkybox(false, mCamera);

            material.SetTexture(iSkybox, tmpBuffer);
            Graphics.Blit(source, lrDepthBuffer, material, 3);
            RenderTexture.ReleaseTemporary(tmpBuffer);
        }
        else
        {
            Graphics.Blit(source, lrDepthBuffer, material, 2);  
        }

#if UNITY_EDITOR
        AddRTs(rtW, rtH, lrDepthBuffer);
#endif

        // 径向模糊
        radialBlurIterations = Mathf.Clamp(radialBlurIterations, 1, 4);

        float ofs = sunShaftBlurRadius * (1.0f / 768.0f);
        material.SetVector(iBlurRadius4, vBlurRadius4 * ofs);
        material.SetVector(iSunPosition, vsunPosition);

        // 径向模糊深度
        for (int it2 = 0; it2 < radialBlurIterations; it2++)
        {
            lrColorB = RenderTexture.GetTemporary(rtW, rtH, 0);
            Graphics.Blit(lrDepthBuffer, lrColorB, material, 1);
            RenderTexture.ReleaseTemporary(lrDepthBuffer);

#if UNITY_EDITOR
            AddRTs(rtW, rtH, lrColorB);
#endif

            ofs = sunShaftBlurRadius * (((it2 * 2.0f + 1.0f) * 6.0f)) / 768.0f;
            material.SetVector(iBlurRadius4, vBlurRadius4 * ofs);

            lrDepthBuffer = RenderTexture.GetTemporary(rtW, rtH, 0);
            Graphics.Blit(lrColorB, lrDepthBuffer, material, 1);
            RenderTexture.ReleaseTemporary(lrColorB);

#if UNITY_EDITOR
            AddRTs(rtW, rtH, lrDepthBuffer);
#endif

            ofs = sunShaftBlurRadius * (((it2 * 2.0f + 2.0f) * 6.0f)) / 768.0f;
            material.SetVector(iBlurRadius4, vBlurRadius4 * ofs);
        }

        // 设置太阳颜色
        if (v.z >= 0.0f)
        {
            vSunColor.x = sunColor.r;
            vSunColor.y = sunColor.g;
            vSunColor.z = sunColor.b;
            vSunColor.w = sunColor.a;
            material.SetVector(iSunColor, vSunColor * sunShaftIntensity);
        }
        else
            material.SetVector(iSunColor, Vector4.zero);

        material.SetTexture(iColorBuffer, lrDepthBuffer);

        Graphics.Blit(source, destination, material, (screenBlendMode == ShaftsScreenBlendMode.Screen) ? 0 : 4);

#if UNITY_EDITOR
        AddRTs(rtW, rtH, destination);
        RemoveUnUsedRTs();
#endif

        RenderTexture.ReleaseTemporary(lrDepthBuffer);
    }
}

#if UNITY_EDITOR

[ExecuteInEditMode]
[CustomEditor(typeof(SunShafts))]
public class SunShaftsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SunShafts pe = target as SunShafts;
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