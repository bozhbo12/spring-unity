using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*****************************************************************************************************
 * 类 : TopGradualColor
 *****************************************************************************************************/
public class Vignetting : PEBase
{
    public Shader shader;

    private int iIntensity;
    private int iBlur;

    protected virtual void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
        }

        if (material == null)
        {
            enabled = false;
            return;
        }

        if (!shader || !shader.isSupported)
            enabled = false;

        iIntensity = Shader.PropertyToID("_Intensity");
        iBlur = Shader.PropertyToID("_Blur");
    }

    override public Material material
    {
        get
        {
            if (m_Material == null)
            {
                if (shader == null)
                    shader = Shader.Find("Snail/Effect/Vignetting");

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
                _matParams = new Dictionary<string, int>();
        
            _matParams.Clear();
            _matParams.Add("_Intensity", (int)MatParamType.Type_Float);
            _matParams.Add("_Blur", (int)MatParamType.Type_Float);
            return _matParams;
        }
    }

    override public void LoadParams()
    {
        //_Intensity = varData.GetFloat("_Intensity");
        //_Blur = varData.GetFloat("_Blur");
        _Intensity = 2f;
        _Blur = 0f;
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

    public float _Intensity = 3f;
    public float _Blur = 0.2f;

    override public void SetVarData()
    {
        varData.SetFloat("_Intensity", _Intensity); 		    // 强度值 
        varData.SetFloat("_Blur", _Blur); 					// 模糊值
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

#if UNITY_EDITOR
        SetVarData();
#endif

        material.SetFloat(iIntensity, _Intensity); 		    // 强度值 
        material.SetFloat(iBlur, _Blur); 					// 模糊值

        Graphics.Blit(source, destination, material);

    }
}
