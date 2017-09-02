using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class AtmosphericScattering : PEBase
{

    [Range(0.0001f, 1.0f)]
    public float sunSize = 0.5f;

    public float atmosphereThickness = 1f;

    public Color skyTint = new Color(1f, 1f, 1f);

    public Color groundColor = new Color(1f, 1f, 1f);

    public float exposure = 1.3f;

    static private Material skyMat;

    void OnEnable()
    {
        if (skyMat == null)
            skyMat = new Material(Shader.Find("Snail/Skybox/Procedural"));

        if (RenderSettings.skybox != skyMat)
            RenderSettings.skybox = skyMat;
    }

    void OnDisable()
    {
        if (RenderSettings.skybox == skyMat && GameScene.mainScene != null)
            RenderSettings.skybox = GameScene.mainScene.skybox;
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
            _matParams.Add("_SunSize", (int)MatParamType.Type_Float);
            _matParams.Add("_AtmosphereThickness", (int)MatParamType.Type_Float);
            _matParams.Add("_Exposure", (int)MatParamType.Type_Float);

            _matParams.Add("_SkyTint", (int)MatParamType.Type_Color);
            _matParams.Add("_GroundColor", (int)MatParamType.Type_Color);

            return _matParams;
        }
    }

    override public void LoadParams()
    {
        sunSize = varData.GetFloat("_SunSize");
        atmosphereThickness = varData.GetFloat("_AtmosphereThickness");
        exposure = varData.GetFloat("_Exposure");

        skyTint = varData.GetColor("_SkyTint");
        groundColor = varData.GetColor("_GroundColor");
    }

    private LensFlare lensFlare;

    void Update()
    {
        Shader.SetGlobalFloat("_SunSize", sunSize);
        Shader.SetGlobalFloat("_AtmosphereThickness", atmosphereThickness);
        Shader.SetGlobalFloat("_Exposure", exposure);

        Shader.SetGlobalColor("_SkyTint", skyTint);
        Shader.SetGlobalColor("_GroundColor", groundColor);

        if (lensFlare == null)
        {
            if (GameScene.mainScene != null && GameScene.mainScene.sunLight != null)
            {
                lensFlare = GameScene.mainScene.sunLight.GetComponent<LensFlare>();
            }
        }
        if (lensFlare != null)
        	lensFlare.brightness = sunSize * 10f;
    }

    override public void SetVarData()
    {
        varData.SetFloat("_SunSize", sunSize);
        varData.SetFloat("_AtmosphereThickness", atmosphereThickness);
        varData.SetFloat("_Exposure", exposure);

        varData.SetColor("_SkyTint", skyTint);
        varData.SetColor("_GroundColor", groundColor);
    }

   

}