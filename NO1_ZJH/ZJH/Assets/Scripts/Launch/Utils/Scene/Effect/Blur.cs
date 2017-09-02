using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;

public class Blur : PEBase
{
	public int downsample = 1;

    /** 模糊类型 */
	public enum BlurType {
		StandardGauss = 0,
		SgxGauss = 1,
	}

	/** 模糊尺寸 */
	public float blurSize = 1.1f;
	
	/** 模糊次数 */
	public int blurIterations = 1;

	//public BlurType blurType = BlurType.StandardGauss;

    public Material material = new Material(Shader.Find("Snail/Effect/MobileBlur"));
	
	void OnDisable()
    {
       
    }

    void OnEnable()
    {
        if (varData == null)
            varData = new VarData();
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

    override public void LoadParams()
    {
        blurSize = varData.GetFloat("blurSize");
        blurIterations = varData.GetInt("blurIterations");
    }

    override public void SetVarData()
    {
        if (varData == null)
            varData = new VarData();

        varData.SetFloat("blurSize", blurSize);
        varData.SetFloat("blurIterations", blurIterations);
    }

	
	void OnRenderImage (RenderTexture source, RenderTexture destination)
    {	
		 if (GameScene.mainScene == null)
            return;

        SetVarData();

		float widthMod = 1.0f / (1.0f * (1<<downsample));

		material.SetVector ("_Parameter", new Vector4 (blurSize * widthMod, -blurSize * widthMod, 0.0f, 0.0f));
		source.filterMode = FilterMode.Bilinear;

		int rtW = source.width >> downsample;
		int rtH = source.height >> downsample;

		RenderTexture rt = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);

		rt.filterMode = FilterMode.Bilinear;
		Graphics.Blit (source, rt, material, 0);

		//var passOffs = blurType == BlurType.StandardGauss ? 0 : 2;
		
		for(int i = 0; i < blurIterations; i++) {
			float iterationOffs = (i*1.0f);
			material.SetVector ("_Parameter", new Vector4 (blurSize * widthMod + iterationOffs, -blurSize * widthMod - iterationOffs, 0.0f, 0.0f));

			// 垂直模糊
			RenderTexture rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, material, 1);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;

			// 水平模糊
			rt2 = RenderTexture.GetTemporary (rtW, rtH, 0, source.format);
			rt2.filterMode = FilterMode.Bilinear;
            Graphics.Blit(rt, rt2, material, 2);
			RenderTexture.ReleaseTemporary (rt);
			rt = rt2;
		}
		
		Graphics.Blit (rt, destination);

		RenderTexture.ReleaseTemporary (rt);
	}	
}
