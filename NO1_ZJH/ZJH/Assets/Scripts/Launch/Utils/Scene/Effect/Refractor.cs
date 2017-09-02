using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class Refractor : PEBase
{
    public Shader shader;
    private Texture2D heightMapDistortionMap;

    public float _DistortionPower = 0.1f;

    private float _DistortionScrollX = 0f;
    private float _DistortionScrollY = 0f;
	public float _DistortionScaleX = 1.0f;		
	public float _DistortionScaleY = 1.0f;		
	

    private float _DistortionScrollX2 = 0f;			
	private float _DistortionScrollY2 = 0f;				
	public float _DistortionScaleX2 = 1.0f;				
	public float _DistortionScaleY2 = 1.0f;

    public float speed = 1f;
    private float scroll = 0f;

    protected virtual void Start()
    {
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }

        if (heightMapDistortionMap == null)
        {
            heightMapDistortionMap = Resources.Load("Shaders/SnailGame/EffectShaders/HeightMapDistortionMap", typeof(Texture2D)) as Texture2D;
        }

        if (material == null || heightMapDistortionMap == null)
        {
            enabled = false;
            return;
        }

        if (!shader || !shader.isSupported)
            enabled = false;

    }

    override public Material material
    {
        get
        {
            if (m_Material == null)
            {
                if (shader == null)
                    shader = Shader.Find("Snail/Effect/Refractor");

                m_Material = new Material(shader);
                m_Material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_Material;
        }
    }

    void OnDestroy()
    {
        heightMapDistortionMap = null;
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

    // Called by camera to apply image effect
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader == null || heightMapDistortionMap == null)
        {
            Graphics.Blit(source, destination);
            enabled = false;
            return;
        }

        scroll += speed * 0.001f;

        _DistortionScrollX = scroll;
        _DistortionScrollY = scroll;

        _DistortionScrollX2 = -scroll * 0.2f;
        _DistortionScrollY2 = -scroll * 0.5f;

        material.SetFloat("_DistortionScrollX", _DistortionScrollX);
        material.SetFloat("_DistortionScrollY", _DistortionScrollY);
        material.SetFloat("_DistortionScaleX", _DistortionScaleX);
        material.SetFloat("_DistortionScaleY", _DistortionScaleY);
        material.SetFloat("_DistortionPower", _DistortionPower * 0.1f);

        material.SetFloat("_DistortionScrollX2", _DistortionScrollX2);
        material.SetFloat("_DistortionScrollY2", _DistortionScrollY2);
        material.SetFloat("_DistortionScaleX2", _DistortionScaleX2);
        material.SetFloat("_DistortionScaleY2", _DistortionScaleY2);
        material.SetFloat("_DistortionPower2", _DistortionPower * 0.1f);

        material.SetTexture("_DistortionMap", heightMapDistortionMap);

        Graphics.Blit(source, destination, material);
    }

  
}