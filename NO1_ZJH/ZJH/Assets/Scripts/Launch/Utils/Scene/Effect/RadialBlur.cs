using System;
using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class RadialBlur : PEBase
{
    public Shader shader;

    //public Vector3 playerPos = new Vector3(0.5f,0.5f,1);
    /// <summary>
    /// 模糊半径
    /// </summary>
    public float blurRadius = 0.9f;
    /// <summary>
    /// 模糊强度
    /// </summary>
    public float blurIntensity = 6f;
    /// <summary>
    /// 将采样
    /// </summary>
    public int divider = 1;


    override public Material material
    {
        get
        {
            if (m_Material == null)
            {
                if (shader == null)
                    shader = Shader.Find("Snail/Effect/RadialBlur");

                m_Material = new Material(shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
        }
    }

    /// <summary>
    /// 主像机
    /// </summary>
    private Camera mCamera = null;


    void Awake()
    {
        mCamera = GetComponent<Camera>();

    }

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
        if (mCamera == null || material == null)
            return;

        //if (ObjectManager.mRole != null)
        //{
        //    if (ObjectManager.mRole.mvHeadPoint != null)
        //    {
        //        playerPos = ObjectManager.mRole.mvHeadPoint.position;
        //        playerPos = mCamera.WorldToViewportPoint(playerPos);
        //    }
        //}
        
        //material.SetVector("_PlayerPos", new Vector4(playerPos.x, playerPos.y, playerPos.z,1));
        material.SetFloat("_BlurRadius", blurRadius);
        material.SetFloat("_BlurStrength", blurIntensity);

        int rtW = source.width / divider;
        int rtH = source.height / divider;
        RenderTexture BlurBuffer = RenderTexture.GetTemporary(rtW, rtH, 0);

        Graphics.Blit(source, BlurBuffer, material, 0);

        material.SetTexture("_BlurTex", BlurBuffer);

        Graphics.Blit(source, destination, material, 1);

        RenderTexture.ReleaseTemporary(BlurBuffer);
    }
}