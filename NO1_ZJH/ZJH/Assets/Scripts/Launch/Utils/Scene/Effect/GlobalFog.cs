using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class GlobalFog : PEBase
{
	public bool  distanceFog = true;
	public bool  excludeFarPixels = true;
	public bool  useRadialDistance = false;
	public bool  heightFog = true;
	public float height = 1.0f;
	public float heightDensity = 2.0f;
	public float startDistance = 0.0f;

    public Color fogColor = new Color(1f, 1f, 1f, 1f);

    public bool syncSysFogColor = false;

	public Shader fogShader = null;

    public Material material = new Material(Shader.Find("Snail/Effect/GlobalFog"));


	override public Dictionary<string, int> matParams
	{
		get
		{
			if (_matParams == null)
			{
				_matParams = new Dictionary<string, int>();
			}
			_matParams.Clear();
            _matParams.Add("_pFogColor", (int)MatParamType.Type_Color);
			_matParams.Add("_pHeight", (int)MatParamType.Type_Float);
			_matParams.Add("_pHeightDensity", (int)MatParamType.Type_Float);
			_matParams.Add("_pStartDistance", (int)MatParamType.Type_Float);
			return _matParams;
		}
	}

	override public void LoadParams()
	{
        fogColor = varData.GetColor("_pFogColor");
        height = varData.GetFloat("_pHeight");
        heightDensity = varData.GetFloat("_pHeightDensity");
        startDistance = varData.GetFloat("_pStartDistance");
	}

    override public void SetVarData()
    {   
        varData.SetColor("_pFogColor", fogColor);
        varData.SetFloat("_pHeight", height);
        varData.SetFloat("_pHeightDensity", heightDensity);
        varData.SetFloat("_pStartDistance", startDistance);
    }

	[ImageEffectOpaque]
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		if (material == null)
			return;

        if (syncSysFogColor == true)
        {
            fogColor = RenderSettings.fogColor;
        }

        SetVarData();

		Camera cam = GetComponent<Camera>();
		Transform camtr = cam.transform;                // 坐标系信息
		float camNear = cam.nearClipPlane;              // 近裁面
		float camFar = cam.farClipPlane;                // 远裁面
		float camFov = cam.fieldOfView;                 // 视野
		float camAspect = cam.aspect;                   // 长宽比

		Matrix4x4 frustumCorners = Matrix4x4.identity;

		float fovWHalf = camFov * 0.5f;

		Vector3 toRight = camtr.right * camNear * Mathf.Tan (fovWHalf * Mathf.Deg2Rad) * camAspect;
		Vector3 toTop = camtr.up * camNear * Mathf.Tan (fovWHalf * Mathf.Deg2Rad);


		Vector3 topLeft = (camtr.forward * camNear - toRight + toTop);
		float camScale = topLeft.magnitude * camFar/camNear;
		topLeft.Normalize();
		topLeft *= camScale;

		Vector3 topRight = (camtr.forward * camNear + toRight + toTop);
		topRight.Normalize();
		topRight *= camScale;


		Vector3 bottomRight = (camtr.forward * camNear + toRight - toTop);
		bottomRight.Normalize();
		bottomRight *= camScale;

		Vector3 bottomLeft = (camtr.forward * camNear - toRight - toTop);
		bottomLeft.Normalize();
		bottomLeft *= camScale;

		frustumCorners.SetRow (0, topLeft);
		frustumCorners.SetRow (1, topRight);
		frustumCorners.SetRow (2, bottomRight);
		frustumCorners.SetRow (3, bottomLeft);

		var camPos= camtr.position;
		float FdotC = camPos.y-height;
		float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
		float excludeDepth = (excludeFarPixels ? 1.0f : 2.0f);

		material.SetMatrix ("_FrustumCornersWS", frustumCorners);
		material.SetVector ("_CameraWS", camPos);
		material.SetVector ("_HeightParams", new Vector4 (height, FdotC, paramK, heightDensity*0.5f));
		material.SetVector ("_DistanceParams", new Vector4 (-Mathf.Max(startDistance,0.0f), excludeDepth, 0, 0));
        material.SetColor("_pFogColor", fogColor);
        
        // 雾效模式
		var sceneMode = RenderSettings.fogMode;
		var sceneDensity = RenderSettings.fogDensity;
		var sceneStart = RenderSettings.fogStartDistance;
		var sceneEnd = RenderSettings.fogEndDistance;

		Vector4 sceneParams;
		bool  linear = (sceneMode == FogMode.Linear);
		float diff = linear ? sceneEnd - sceneStart : 0.0f;
		float invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;
		sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
		sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
		sceneParams.z = linear ? -invDiff : 0.0f;
		sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;

		material.SetVector ("_SceneFogParams", sceneParams);

        // 设置雾效模式
		material.SetVector ("_SceneFogMode", new Vector4((int)sceneMode, useRadialDistance ? 1 : 0, 0, 0));

		int pass = 0;
		if (distanceFog && heightFog)
			pass = 0; // distance + height
		else if (distanceFog)
			pass = 1; // distance only
		else
			pass = 2; // height only

		CustomGraphicsBlit (source, destination, material, pass);
	}

    /******************************************************************************************
	 * 功能 : 自定义绘制
 	 ******************************************************************************************/

	static void CustomGraphicsBlit (RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
	{
		RenderTexture.active = dest;

		fxMaterial.SetTexture ("_MainTex", source);

		GL.PushMatrix ();
		GL.LoadOrtho ();

		fxMaterial.SetPass (passNr);

		GL.Begin (GL.QUADS);

		GL.MultiTexCoord2 (0, 0.0f, 0.0f);
		GL.Vertex3 (0.0f, 0.0f, 3.0f); // BL

		GL.MultiTexCoord2 (0, 1.0f, 0.0f);
		GL.Vertex3 (1.0f, 0.0f, 2.0f); // BR

		GL.MultiTexCoord2 (0, 1.0f, 1.0f);
		GL.Vertex3 (1.0f, 1.0f, 1.0f); // TR

		GL.MultiTexCoord2 (0, 0.0f, 1.0f);
		GL.Vertex3 (0.0f, 1.0f, 0.0f); // TL

		GL.End ();
		GL.PopMatrix ();
	}
}