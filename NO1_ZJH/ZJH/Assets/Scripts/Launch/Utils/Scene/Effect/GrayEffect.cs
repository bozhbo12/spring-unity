using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class GrayEffect : PEBase {


    public Shader curShader;
    public float grayScaleAmount = 1.0f;
    public float grayScaleStr = 1.0f;
    private Material curMaterial;

    public Material material
    {
        get
        {
            if (curMaterial == null)
            {
                if (curShader == null)
                    curShader = Shader.Find("Snail/Effect/GrayEffect");
                curMaterial = new Material(curShader);
                curMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return curMaterial;
        }
    }

    // Use this for initialization  
    void Start()
    {
        if (SystemInfo.supportsImageEffects == false)
        {
            enabled = false;
            return;
        }

        if (material == null)
        {
            enabled = false;
            return;
        }

        if (curShader != null && curShader.isSupported == false)
        {
            enabled = false;
        }
    }

    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (curShader != null)
        {
            material.SetFloat("_LuminosityAmount", grayScaleAmount);
            material.SetFloat("_LuminosityStr", grayScaleStr);
            Graphics.Blit(sourceTexture, destTexture, material);
        }
        else
        {
            Graphics.Blit(sourceTexture, destTexture);
        }
    }


    void OnDestroy()
    {
        if (curMaterial != null)
        {
            DestroyImmediate(curMaterial);
        }
    }
}
