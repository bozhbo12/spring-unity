/*
 *  版    本： 0.0.2
 *  作    者： Zhuwenqiang
 *  创建日期： 2016/7/14
 *  内    容： 去掉secondDistanceFogDensity后面的系数
 * *
 *  版    本： 0.0.1
 *  作    者： ChenGang
 *  创建日期： 2016/7/8
 */

using UnityEngine;
using System.Collections;

// Editor "GlobalFogInMatEditor.cs"
[ExecuteInEditMode]
public class SnailFogPro : MonoBehaviour
{
    public int fogLevel = 0;

	public bool useDistanceFog = true;
	private MatFogMode distantFogMode = MatFogMode.LINEAR;
	public Color distanceFogColor = Color.gray;
	private float distanceFogDensity = 0.01f;
	public float distanceFogStart = 0;
	public float distanceFogEnd = 100;

	public float skyFogHeight = 500.0f;
	public float skyFogRate = 1.0f;
    public float skyFogStart = 0.001f;

    public bool useSecondDistanceFog = false;
	public Color secondDistanceFogColor = Color.gray;
	public float secondDistanceFogDensity = 0.01f;

	public bool useHeightFog = false;
	private MatFogMode heightFogMode = MatFogMode.LINEAR;
	public Color heightFogColor = Color.gray;
	//[Range(0.001f, 3.0f)]
	private float heightFogDensity = 0.01f;
	[Tooltip ("Fog top Y coordinate")]
	public float height = 1.0f;
	//[Range(0.0001f, 0.1f)]
	private float heightDensity = 0.1f;
	//[Tooltip("Push fog away from the camera by this amount")]
	//public float startDistance = 0.0f;
	public float heightFogStart = 0.0f;
	public float heightFogEnd = 100.0f;

    //public bool tempUseDistanceFog;
    //public bool tempUseSecDistanceFog;
    //public bool tempUseHeightFog;

    static string DISTANCE_FOG_LINEAR_MACRO = "_DISTANCE_FOG_LINEAR";
	static string DISTANCE_FOG_EXP_MACRO = "_DISTANCE_FOG_EXP";
	static string SECOND_DISTANCE_FOG_MACRO = "_SECOND_DISTANCE_FOG";
	static string HEIGHT_FOG_LINEAR_MACRO = "_HEIGHT_FOG_LINEAR";
	static string HEIGHT_FOG_EXP_MACRO = "_HEIGHT_FOG_EXP";

    public enum MatFogMode
    {
        LINEAR = 0,
        EXP
    }

    private static SnailFogPro _Instance;
    public static SnailFogPro Instance
    {
        get
        {
            return _Instance;
        }
    }


    void Awake()
    {

    }

    void Start()
    {

	}


	// Update is called once per frame
	void Update ()
	{
        Shader.SetGlobalFloat("_Rotation", Time.deltaTime);	
	}

	void SetKeywordAndParameters()
	{
		if (!useDistanceFog && !useSecondDistanceFog && !useHeightFog) {
			Shader.DisableKeyword (DISTANCE_FOG_LINEAR_MACRO);
			Shader.DisableKeyword (DISTANCE_FOG_EXP_MACRO);
			Shader.DisableKeyword (SECOND_DISTANCE_FOG_MACRO);
			Shader.DisableKeyword (HEIGHT_FOG_LINEAR_MACRO);
			Shader.DisableKeyword (HEIGHT_FOG_EXP_MACRO);
			return;
		}

		if (useDistanceFog) {
			if (distantFogMode == MatFogMode.LINEAR) {
				Shader.DisableKeyword (DISTANCE_FOG_EXP_MACRO);
				Shader.EnableKeyword (DISTANCE_FOG_LINEAR_MACRO);
			} else {
				Shader.DisableKeyword (DISTANCE_FOG_LINEAR_MACRO);
				Shader.EnableKeyword (DISTANCE_FOG_EXP_MACRO);
			}
		} else {
			Shader.DisableKeyword (DISTANCE_FOG_LINEAR_MACRO);
			Shader.DisableKeyword (DISTANCE_FOG_EXP_MACRO);
		}

		if (useSecondDistanceFog) {
			Shader.EnableKeyword (SECOND_DISTANCE_FOG_MACRO);
		} else {
			Shader.DisableKeyword (SECOND_DISTANCE_FOG_MACRO);
		}

		if (useHeightFog) {
			if (heightFogMode == MatFogMode.LINEAR) {
				Shader.DisableKeyword (HEIGHT_FOG_EXP_MACRO);
				Shader.EnableKeyword (HEIGHT_FOG_LINEAR_MACRO);
			} else {
				Shader.DisableKeyword (HEIGHT_FOG_LINEAR_MACRO);
				Shader.EnableKeyword (HEIGHT_FOG_EXP_MACRO);
			}
		} else {
			Shader.DisableKeyword (HEIGHT_FOG_LINEAR_MACRO);
			Shader.DisableKeyword (HEIGHT_FOG_EXP_MACRO);
		}

		Vector4 sceneParams;
		float diff = distanceFogEnd - distanceFogStart;
		float invDiff = Mathf.Abs (diff) > 0.0001f ? 1.0f / diff : 0.0f;
		sceneParams.x = distanceFogDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
		//sceneParams.y = distanceFogDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
		sceneParams.y = distanceFogDensity * 1.4426950408f;
		//sceneParams.y = distanceFogDensity; // density / ln(2), used by Exp fog mode
		sceneParams.z = distanceFogStart;
		sceneParams.w = invDiff;
		Shader.SetGlobalColor ("_DistanceFogColor", distanceFogColor);
		Shader.SetGlobalVector ("_SceneFogParams", sceneParams);

		float secFogDensity = secondDistanceFogDensity;
		Shader.SetGlobalColor ("_SecDistanceFogColor", secondDistanceFogColor);
		Shader.SetGlobalFloat ("_SecDistanceFogDensity", secFogDensity);

		Shader.SetGlobalColor ("_HeightFogColor", heightFogColor);

		Vector4 heightSceneParams;
		float heightDiff = heightFogEnd - heightFogStart;
		float heightInvDiff = Mathf.Abs (heightDiff) > 0.0001f ? 1.0f / heightDiff : 0.0f;
		heightSceneParams.x = heightFogDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
		heightSceneParams.y = heightFogDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
		heightSceneParams.z = -heightInvDiff;
		heightSceneParams.w = heightFogEnd * heightInvDiff;
		Shader.SetGlobalVector ("_HeightParams", new Vector4 (heightFogStart, heightInvDiff, 0, 0));
		Shader.SetGlobalVector ("_HeightSceneFogParams", heightSceneParams);

		Shader.SetGlobalFloat ("_SkyFogHeight", skyFogHeight / 1000.0f);
		Shader.SetGlobalFloat ("_SkyFogRate", skyFogRate);
        Shader.SetGlobalFloat("_SkyStart", skyFogStart);
    }

    void OnEnable ()
	{
        _Instance = this;
        SetKeywordAndParameters();
    }

	void OnDisable ()
	{

    }

    void OnDestroy()
    {
        _Instance = null;
        //if (Shader.IsKeywordEnabled(DISTANCE_FOG_LINEAR_MACRO))
        //{
        //    Shader.DisableKeyword(DISTANCE_FOG_LINEAR_MACRO);
        //}
        //if (Shader.IsKeywordEnabled(DISTANCE_FOG_EXP_MACRO))
        //{
        //    Shader.DisableKeyword(DISTANCE_FOG_EXP_MACRO);
        //}
        //if (Shader.IsKeywordEnabled(SECOND_DISTANCE_FOG_MACRO))
        //{
        //    Shader.DisableKeyword(SECOND_DISTANCE_FOG_MACRO);
        //}
        //if (Shader.IsKeywordEnabled(HEIGHT_FOG_LINEAR_MACRO))
        //{
        //    Shader.DisableKeyword(HEIGHT_FOG_LINEAR_MACRO);
        //}
        //if (Shader.IsKeywordEnabled(HEIGHT_FOG_EXP_MACRO))
        //{
        //    Shader.DisableKeyword(HEIGHT_FOG_EXP_MACRO);
        //}
    }
}
