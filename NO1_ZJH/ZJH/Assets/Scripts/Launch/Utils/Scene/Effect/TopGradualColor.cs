using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*****************************************************************************************************
 * 类 : TopGradualColor
 *****************************************************************************************************/
public class TopGradualColor : PEBase
{
    public Shader shader;

    private int iGradualColor;
    private int iGradualStart;
    private int iGradualEnd;
    private int iGradualExp;
    private int iColorAdjustParam;
    private int iGradualBaseColor;
    private int iVecViewDirection;

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

        iGradualColor = Shader.PropertyToID("_GradualColor");
        iGradualStart = Shader.PropertyToID("_fGradualStart");
        iGradualEnd = Shader.PropertyToID("_fGradualEnd");
        iGradualExp = Shader.PropertyToID("_fGradualExp");
        iColorAdjustParam = Shader.PropertyToID("_vColorAdjustParam");
        iGradualBaseColor = Shader.PropertyToID("_GradualBaseColor");
        iVecViewDirection = Shader.PropertyToID("_VecViewDirection");
    }

    override public Material material
    {
        get
        {
            if (m_Material == null)
            {
                if (shader == null)
                    shader = Shader.Find("Snail/Effect/TopGradualColor");

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
            _matParams.Add("_GradualColor", (int)MatParamType.Type_Color);
            _matParams.Add("_fGradualStart", (int)MatParamType.Type_Float);
            _matParams.Add("_fGradualEnd", (int)MatParamType.Type_Float);
            _matParams.Add("_fGradualExp", (int)MatParamType.Type_Float);

            _matParams.Add("_vColorAdjustParam", (int)MatParamType.Type_Vector4);
            _matParams.Add("_GradualBaseColor", (int)MatParamType.Type_Color);
            return _matParams;
        }
    }

    override public void LoadParams()
    {
        _GradualColor = varData.GetColor("_GradualColor");

        _fGradualStart = varData.GetFloat("_fGradualStart");
        _fGradualEnd = varData.GetFloat("_fGradualEnd");
        _fGradualExp = varData.GetFloat("_fGradualExp");

        _vColorAdjustParam = varData.GetVector("_vColorAdjustParam");
        _GradualBaseColor = varData.GetColor("_GradualBaseColor");
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

    public Color _GradualColor = new Color(0f, 55f/255f, 1f);
    public float _fGradualStart = 2f;
    public float _fGradualEnd = 1f;
    public float _fGradualExp = 3f;

    public Vector3 _VecViewDirection;

    /**
    public float brightness = 1f;                    // 亮度
    public float contrast = 0f;                      // 对比度
    public float saturation = 1f;                    // 饱和度
    **/
    public Vector4 _vColorAdjustParam = new Vector4(1f, 0f, 3f, 1f);

    public Color _GradualBaseColor = new Color(1f, 1f, 1f);             // 基色

    private Vector3 mvForWard = new Vector3(0f, 0f, 1f);
    override public void SetVarData()
    {
        varData.SetColor("_GradualColor", _GradualColor);
        varData.SetFloat("_fGradualStart", _fGradualStart);
        varData.SetFloat("_fGradualEnd", _fGradualEnd);
        varData.SetFloat("_fGradualExp", _fGradualExp);

        varData.SetVector("_vColorAdjustParam", _vColorAdjustParam);
        varData.SetColor("_GradualBaseColor", _GradualBaseColor);
    }


    /******************************************************************************************
     * 功能 : 后期效果处理
     ******************************************************************************************/
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader == null)
        {
            Graphics.Blit(source, destination);
            enabled = false;
            return;
        }

        Camera camera = Camera.current;
        Vector3 viewDir = camera.transform.localToWorldMatrix.MultiplyVector(mvForWard);

        _VecViewDirection = viewDir;

#if UNITY_EDITOR
        SetVarData();
#endif

        material.SetColor(iGradualColor, _GradualColor);
        material.SetFloat(iGradualStart, _fGradualStart);
        material.SetFloat(iGradualEnd, _fGradualEnd);
        material.SetFloat(iGradualExp, _fGradualExp);

        material.SetVector(iColorAdjustParam, _vColorAdjustParam);
        material.SetColor(iGradualBaseColor, _GradualBaseColor);

        material.SetVector(iVecViewDirection, _VecViewDirection);

        Graphics.Blit(source, destination, material);

    }
}
