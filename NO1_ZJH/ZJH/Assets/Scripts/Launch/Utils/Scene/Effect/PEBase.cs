//Based on Unity's PostEffectsBase.js, just so I don't have to import their standard assets all the time.
//Also so I can customize it

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PEBase : MonoBehaviour
{
    protected bool supportHDRTextures = true;
    protected bool supportDX11 = false;
    protected bool isSupported = true;


#if UNITY_EDITOR
    public List<RenderTexture> tRenderTextures = new List<RenderTexture>();

    /// <summary>
    /// 是否展开
    /// </summary>
    public bool showPosition;

    public int tIndex = 0;

    /// <summary>
    /// 选中RT
    /// </summary>
    public int selectIndex = 0;


    public void AddRTs(int w, int h, RenderTexture source)
    {
        if (tIndex < tRenderTextures.Count)
        {
            if (tRenderTextures[tIndex] == null)
            {
                RenderTexture tBuffer = new RenderTexture(w, h, 0);
                Graphics.Blit(source, tBuffer);
                tRenderTextures[tIndex] = tBuffer;
            }
            else
            {
                tRenderTextures[tIndex].DiscardContents();
                Graphics.Blit(source, tRenderTextures[tIndex]);
            }
        }
        else
        {
            RenderTexture tBuffer = new RenderTexture(w, h, 0);
            Graphics.Blit(source, tBuffer);
            tRenderTextures.Add(tBuffer);
        }
        tIndex++;
    }

    public void RemoveUnUsedRTs()
    {
        for (int i = tIndex; i < tRenderTextures.Count; i++)
        {
            tRenderTextures[tIndex] = null;
            tRenderTextures.RemoveAt(tIndex);
        }
    }

#endif



    //private bool mStarted = false;
    // 材质
    protected Material m_Material = null;
    // 参数
    protected Dictionary<string, int> _matParams;

    virtual public Material material
    {
        get
        {
            return null;
        }
    }

    // 数据
    public VarData varData = new VarData();

    static public int useDepthTick = 0;

    virtual public Dictionary<string, int> matParams
    {
        get
        {
            return null;
        }
    }

    virtual public void LoadParams()
    { 
        
    }

    virtual public void SetVarData()
    {
        
    }

    /****************************************************************************************************
     * 功能 : 创建着色器和材质
     ****************************************************************************************************/

    public Material CheckShaderAndCreateMaterial(Shader s, Material m2Create)
    {
        if(s == null) {
            LogSystem.Log("Missing shader in " + name);
            enabled = false;
            return null;
        }

        if(s.isSupported && m2Create && m2Create.shader == s)
            return m2Create;

        if(!s.isSupported) {
            NotSupported();
            LogSystem.Log("The shader " + s.ToString() + " on effect " + this.ToString() + " is not supported on this platform!");
            return null;
        }
        else {
            m2Create = new Material(s);
            m2Create.hideFlags = HideFlags.DontSave;
            if(m2Create)
                return m2Create;
            else return null;
        }
    }

    public Material CreateMaterial(Shader s, Material m2Create) 
    {   
        if(s == null) {
            LogSystem.Log("Missing shader in " + this.ToString());
            return null;
        }

        if(m2Create != null && (m2Create.shader == s) && (s.isSupported))
            return m2Create;

        if(!s.isSupported) {
            return null;
        }
        else {
            m2Create = new Material(s);
            m2Create.hideFlags = HideFlags.DontSave;
            if(m2Create)
                return m2Create;
            else return null;
        }
    }

    public bool CheckSupport() {
        return CheckSupport(false);
    }

    public bool CheckSupport(bool needDepth) 
    {
        isSupported = true;
        supportHDRTextures = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf);
        supportDX11 = SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders;

        if(!SystemInfo.supportsImageEffects || !SystemInfo.supportsRenderTextures) {
            NotSupported();
            return false;
        }

        if(needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth)) {
            NotSupported();
            return false;
        }

        if(needDepth)
            GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;

        return true;
    }

    public bool CheckSupport(bool needDepth, bool needHdr) 
    {
        if(!CheckSupport(needDepth))
            return false;

        if(needHdr && !supportHDRTextures) 
        {
            NotSupported();
            return false;
        }
        return true;
    }

    public bool Dx11Support() 
    {
        return supportDX11;
    }

    public void ReportAutoDisable() 
    {
        LogSystem.LogWarning("The image effect " + this.ToString() + " has been disabled as it's not supported on the current platform.");
    }
        
    public bool CheckShader(Shader s) 
    {
        LogSystem.Log("The shader " + s.ToString() + " on effect " + this.ToString() + " is not part of the Unity 3.2+ effects suite anymore. For best performance and quality, please ensure you are using the latest Standard Assets Image Effects (Pro only) package.");
        if(!s.isSupported) {
            NotSupported();
            return false;
        }
        else {
            return false;
        }
    }


    public void CheckMaterial(Material m2Create)
    {
        if (m2Create == null)
        {
            LogSystem.Log("Missing material in " + name);
            enabled = false;
            return;
        }
        if (m2Create.shader == null)
        {
            LogSystem.Log("Missing shader in " + name);
            enabled = false;
            return;
        }

        if (!m2Create.shader.isSupported)
        {
            NotSupported();
            LogSystem.Log("The shader " + m2Create.shader.ToString() + " on effect " + this.ToString() + " is not supported on this platform!");
            return;
        }
    }


    public void NotSupported() 
    {
        enabled = false;
        isSupported = false;
        return;
    }

#if UNITY_EDITOR

    public void DrawBorder(RenderTexture dest, Material material) 
    {
        float x1, x2, y1, y2;

        RenderTexture.active = dest;
        bool invertY = true; // source.texelSize.y < 0.0f;
        // Set up the simple Matrix
        GL.PushMatrix();
        GL.LoadOrtho();

        for(int i = 0; i < material.passCount; i++) 
        {
            material.SetPass(i);

            float y1_, y2_;

            if(invertY) {
                y1_ = 1.0f; y2_ = 0.0f;
            }
            else {
                    y1_ = 0.0f; y2_ = 1.0f;
            }

            // left	        
            x1 = 0.0f;
            x2 = 0.0f + 1.0f / (dest.width * 1.0f);
            y1 = 0.0f;
            y2 = 1.0f;
            GL.Begin(GL.QUADS);

            GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
            GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
            GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
            GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

            // right
            x1 = 1.0f - 1.0f / (dest.width * 1.0f);
            x2 = 1.0f;
            y1 = 0.0f;
            y2 = 1.0f;

            GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
            GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
            GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
            GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

            // top
            x1 = 0.0f;
            x2 = 1.0f;
            y1 = 0.0f;
            y2 = 0.0f + 1.0f / (dest.height * 1.0f);

            GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
            GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
            GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
            GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

            // bottom
            x1 = 0.0f;
            x2 = 1.0f;
            y1 = 1.0f - 1.0f / (dest.height * 1.0f);
            y2 = 1.0f;

            GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
            GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
            GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
            GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

            GL.End();
        }

        GL.PopMatrix();
    }

    void OnDisable()
    {
        if (tRenderTextures != null)
        {
            tRenderTextures.Clear();
            tRenderTextures = null;
        }
    }

    void OnDestroy()
    {
        if (tRenderTextures != null)
        {
            tRenderTextures.Clear();
            tRenderTextures = null;
        }
    }

#endif
}

#if UNITY_EDITOR

public class PEEditWindow : EditorWindow
{
    private Material mMat = null;

    public RenderTexture rtTexture;

    void OnGUI()
    {
        GUILayout.Label("当前渲染图", EditorStyles.boldLabel);

        if (!mMat)
        {
            mMat = new Material(Shader.Find("Unlit/Texture"));
        }
        if (rtTexture != null)
        {
            mMat.mainTexture = rtTexture;
            EditorGUI.DrawPreviewTexture(new Rect(0, 0, Screen.width, Screen.height), mMat.mainTexture, mMat);
        }

    }


    public void Init(RenderTexture rt)
    {
        //rtTexture = RenderTexture.GetTemporary(rt.width, rt.height, 0);
        //Graphics.Blit(rt, rtTexture);
        rtTexture = rt;
    }



    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnDestroy()
    {
        rtTexture = null;
        mMat = null;
    }

}

#endif