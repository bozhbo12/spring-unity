
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class Bloom : PEBase
{
    public Shader shader;

    private int iParameter;
    private int iBloom;


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

        iParameter = Shader.PropertyToID("_Parameter");
        iBloom = Shader.PropertyToID("_Bloom");

    }

    public enum Resolution
    {
        Low = 0,
        High = 1,
    }

    public enum BlurType
    {
        Standard = 0,
        Sgx = 1,
    }

    override public Material material
    {
        get
        {
            if (m_Material == null)
            {
                if (shader == null)
                    shader = Shader.Find("Snail/Effect/Bloom");

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
            // _matParams.Add("_Parameter", (int)MatParamType.Type_Vector4);
            _matParams.Add("threshhold", (int)MatParamType.Type_Float);
            _matParams.Add("intensity", (int)MatParamType.Type_Float);
            _matParams.Add("blurSize", (int)MatParamType.Type_Float);
            return _matParams;
        }
    }

    public float threshhold = 0.25f;
    public float intensity = 0.32f;
    public float blurSize = 1.0f;
    public int blurIterations = 1;

    // public Vector4 _Parameter = new Vector4(0.25f, 0.32f, 1.0f, 1f);

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

    override public void LoadParams()
    {
        threshhold = varData.GetFloat("threshhold");
        intensity = varData.GetFloat("intensity");
        blurSize = varData.GetFloat("blurSize");
    }

    override public void SetVarData()
    {
        float widthMod = 0.5f;  

        varData.SetFloat("threshhold", threshhold);
        varData.SetFloat("intensity", intensity);
        varData.SetFloat("blurSize", blurSize);
        varData.SetVector("_Parameter", new Vector4(blurSize * widthMod, 0.0f, threshhold, intensity));
    }


    /***************************************************************************************************************
     * Function : 
     ***************************************************************************************************************/

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader == null)
        {
            Graphics.Blit(source, destination);
            enabled = false;
            return;
        }

        float widthMod = 0.5f;

#if UNITY_EDITOR
        tIndex = 0;

        SetVarData();
#endif

        source.filterMode = FilterMode.Bilinear;

        var rtW = source.width / 4;
        var rtH = source.height / 4;

        // 降低采样
        RenderTexture rt = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
        rt.filterMode = FilterMode.Bilinear;
        Graphics.Blit(source, rt, material, 1);

#if UNITY_EDITOR
        AddRTs(rtW, rtH, rt);
#endif

        for (int i = 0; i < 1; i++)
        {
            material.SetVector(iParameter, new Vector4(blurSize * widthMod + (i * 1.0f), 0.0f, threshhold, intensity));

            // 垂直模糊
            RenderTexture rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, material, 2);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;

#if UNITY_EDITOR
            AddRTs(rtW, rtH, rt);
#endif

            // 水平模糊
            rt2 = RenderTexture.GetTemporary(rtW, rtH, 0, source.format);
            rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, material, 3);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;

#if UNITY_EDITOR
            AddRTs(rtW, rtH, rt);
#endif
        }

        material.SetTexture(iBloom, rt);

        Graphics.Blit(source, destination, material, 0);

#if UNITY_EDITOR

        AddRTs(rtW, rtH, destination);
        RemoveUnUsedRTs();
#endif

        RenderTexture.ReleaseTemporary(rt);
    }
}

#if UNITY_EDITOR

[ExecuteInEditMode]
[CustomEditor(typeof(Bloom))]
public class BloomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Bloom pe = target as Bloom;
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