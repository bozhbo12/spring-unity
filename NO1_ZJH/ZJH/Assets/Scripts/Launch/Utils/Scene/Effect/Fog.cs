using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class Fog : PEBase
{
    public Vector4 unity_FogParams = new Vector4();
    public Vector4 unity_HeightFogParams = new Vector4();

    public bool enableLinearFog = true;
    public bool enableExpFog = false;
    public bool enableExp2Fog = false;
    public bool enableHeightFog = false;

    public Color linearFogColor = new Color(116f / 255f, 129f / 255f, 178f / 255f, 1f);
    public Color expFogColor = new Color(116f / 255f, 129f / 255f, 178f / 255f, 1f);
    public Color heightFogColor = new Color(116f / 255f, 129f / 255f, 178f / 255f, 1f);

    public float fogStart = -50f;
    public float fogEnd = 1000f;

    public float fogHeightStart = 50f;
    public float fogHeightEnd = 50f;

    public float linearFogHeightFactor = 0f;

    [Range(0.0001f, 1.0f)]
    public float fogDensity = 0f;


	override public Dictionary<string, int> matParams
	{
		get
		{
			if (_matParams == null)
			{
				_matParams = new Dictionary<string, int>();
			}
			_matParams.Clear();
            _matParams.Add("_pLinearFogEnable", (int)MatParamType.Type_Int);
            _matParams.Add("_pExpFogEnable", (int)MatParamType.Type_Int);
            _matParams.Add("_pHeightFogEnable", (int)MatParamType.Type_Int);

            _matParams.Add("_pLinearFogColor", (int)MatParamType.Type_Color);
            _matParams.Add("_pExpFogColor", (int)MatParamType.Type_Color);
            _matParams.Add("_pHeightFogColor", (int)MatParamType.Type_Color);
            _matParams.Add("_pFogHeightStart", (int)MatParamType.Type_Float);
            _matParams.Add("_pFogHeightEnd", (int)MatParamType.Type_Float);
                
            _matParams.Add("_pFogStart", (int)MatParamType.Type_Float);
            _matParams.Add("_pFogEnd", (int)MatParamType.Type_Float);
            _matParams.Add("_pFogDensity", (int)MatParamType.Type_Float);

            _matParams.Add("_pFogMaxHeight", (int)MatParamType.Type_Float);
			return _matParams;
		}
	}

	override public void LoadParams()
	{
        enableLinearFog = varData.GetInt("_pLinearFogEnable") == 1 ? true : false;
        enableExpFog = varData.GetInt("_pExpFogEnable") == 1 ? true : false;
        enableHeightFog = varData.GetInt("_pHeightFogEnable") == 1 ? true : false;

        linearFogColor = varData.GetColor("_pLinearFogColor");
        expFogColor = varData.GetColor("_pExpFogColor");
        heightFogColor = varData.GetColor("_pHeightFogColor");
        fogHeightStart = varData.GetFloat("_pFogHeightStart");
        fogHeightEnd = varData.GetFloat("_pFogHeightEnd");

        fogStart = varData.GetFloat("_pFogStart");
        fogEnd = varData.GetFloat("_pFogEnd");
        fogDensity = varData.GetFloat("_pFogDensity");

        linearFogHeightFactor = varData.GetFloat("_pLinearFogHeightFactor");
	}

    void Update()
    {
        ForceUpdate();
    }

    override public void SetVarData()
    {
        varData.SetInt("_pLinearFogEnable", enableLinearFog ? 1 : 0);
        varData.SetInt("_pExpFogEnable", enableExpFog ? 1 : 0);
        varData.SetInt("_pHeightFogEnable", enableHeightFog ? 1 : 0);

        varData.SetColor("_pLinearFogColor", linearFogColor);
        varData.SetColor("_pExpFogColor", expFogColor);
        varData.SetColor("_pHeightFogColor", heightFogColor);
        varData.SetFloat("_pFogHeightStart", fogHeightStart);
        varData.SetFloat("_pFogHeightEnd", fogHeightEnd);

        varData.SetFloat("_pFogStart", fogStart);
        varData.SetFloat("_pFogEnd", fogEnd);
        varData.SetFloat("_pFogDensity", fogDensity);

        varData.SetFloat("_pLinearFogHeightFactor", linearFogHeightFactor);
    }

	public void ForceUpdate()
	{
        if (varData == null)
			return;

        SetVarData();

        RenderSettings.fogStartDistance = fogStart;
        RenderSettings.fogEndDistance = fogEnd;
        RenderSettings.fogDensity = fogDensity;

        // factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
        unity_FogParams.z = -1f / (RenderSettings.fogEndDistance - RenderSettings.fogStartDistance);
        unity_FogParams.w = RenderSettings.fogEndDistance / (RenderSettings.fogEndDistance - RenderSettings.fogStartDistance);

        // factor = exp(-density*z)
        unity_FogParams.y = RenderSettings.fogDensity;
        unity_FogParams.x = RenderSettings.fogDensity;

        unity_HeightFogParams.z = -1f / (fogHeightEnd - fogHeightStart);
        unity_HeightFogParams.w = fogHeightEnd / (fogHeightEnd - fogHeightStart);


        Shader.SetGlobalColor("linearFogColor", linearFogColor);
        Shader.SetGlobalColor("expFogColor", expFogColor);
        Shader.SetGlobalColor("heightFogColor", heightFogColor);
        // Shader.SetGlobalFloat("linearFogHeightFactor", linearFogHeightFactor);

        if (enableLinearFog == true)
            Shader.EnableKeyword("CUSTOM_FOG_LINEAR");
        else
            Shader.DisableKeyword("CUSTOM_FOG_LINEAR");

        if (enableExpFog == true)
            Shader.EnableKeyword("CUSTOM_FOG_EXP");
        else
            Shader.DisableKeyword("CUSTOM_FOG_EXP");

        if (enableExp2Fog == true)
            Shader.EnableKeyword("CUSTOM_FOG_EXP2");
        else
            Shader.DisableKeyword("CUSTOM_FOG_EXP2");

        if (enableHeightFog == true)
            Shader.EnableKeyword("CUSTOM_FOG_HEIGHT");
        else
            Shader.DisableKeyword("CUSTOM_FOG_HEIGHT");

        Shader.SetGlobalVector("custom_FogParams", unity_FogParams);

        Shader.SetGlobalVector("custom_HeightFogParams", unity_HeightFogParams);
	}

   
}