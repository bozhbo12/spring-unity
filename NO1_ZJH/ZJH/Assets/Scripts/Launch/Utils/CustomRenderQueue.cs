using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 自定义特效层级
/// </summary>
public class CustomRenderQueue : MonoBehaviour
{
    List<Renderer> rendererList = new List<Renderer>();

    GameObject mGameObject = null;

    public UIWidget mTarget = null;

    public int frontOrBack = -1;

    public int lastRendererQueue = 0;

    public bool needClipWithPanel;

    public UIPanel mClipPanel;

	public bool Open;
    private static List<CustomRenderQueue> mRenderQueues = new List<CustomRenderQueue>();
    void Awake()
    {
        mRenderQueues.Add(this);
    }

    /// <summary>
    /// 更新子节点Renderer
    /// </summary>
    /// <param name="oModel"></param>
    public static void UpdateRendererQueue(Transform oTrans)
    {
        if (oTrans == null)
            return;

        CustomRenderQueue cRender = null;
        for (int i = mRenderQueues.Count - 1; i >= 0; i--)
        {
            cRender = mRenderQueues[i];
            if (cRender == null)
            {
                mRenderQueues.RemoveAt(i);
                continue;
            }
            if (!cRender.gameObject.activeInHierarchy)
                continue;

            if (cRender.transform.IsChildOf(oTrans))
            {
                cRender.UpdateRendererQueue();
            }
        }
    }

    void Start()
    {
        OnApplicationFocus();

    }

	void OnEnable()
	{
		if(Open)
		{
			RefreshTargetRenderer (mTarget,frontOrBack,true);	
		}

	}
    void LateUpdate()
    {
        //UpdateRendererQueue();
        OnApplicationFocus();
    }

    void OnValidate()
    {
        OnApplicationFocus();
    }

    public void OnApplicationFocus()
    {
        if (null != mTarget && null != mTarget.panel && needClipWithPanel)
        {
            int mWorldToPanel = Shader.PropertyToID("_WorldToPanel");
            int _MinX = Shader.PropertyToID("_MinX");
            int _MaxX = Shader.PropertyToID("_MinY");
            int _MinY = Shader.PropertyToID("_MaxX");
            int _MaxY = Shader.PropertyToID("_MaxY");

            float minX = mTarget.panel.mMin.x;
            float minY = mTarget.panel.mMin.y;
            float maxX = mTarget.panel.mMax.x;
            float maxY = mTarget.panel.mMax.y;
            Matrix4x4 mMatrix = mTarget.panel.worldToLocal;

            for (int i = 0; i < rendererList.Count; i++)
            {
                if (rendererList[i] != null)
                {
                    string shaderName = rendererList[i].material.shader.name;
                    if (!shaderName.Contains(" ClipWithPanel"))
                    {
                        shaderName = rendererList[i].material.shader.name + " ClipWithPanel";
                        Shader shader = Shader.Find(shaderName);
                        if (shader != null)
                        {
                            rendererList[i].material.shader = shader;
                        }
                    }
                    rendererList[i].material.SetMatrix(mWorldToPanel, mMatrix);
                    rendererList[i].material.SetFloat(_MinX, minX);
                    rendererList[i].material.SetFloat(_MaxX , minY);
                    rendererList[i].material.SetFloat(_MinY , maxX);
                    rendererList[i].material.SetFloat(_MaxY , maxY);
                }
            }
        }
        else if (null != mClipPanel && needClipWithPanel)
        {
            int mWorldToPanel = Shader.PropertyToID("_WorldToPanel");
            float minX = mClipPanel.mMin.x;
            float minY = mClipPanel.mMin.y;
            float maxX = mClipPanel.mMax.x;
            float maxY = mClipPanel.mMax.y;
            Matrix4x4 mMatrix = mClipPanel.worldToLocal;

            for (int i = 0; i < rendererList.Count; i++)
            {
                if (rendererList[i] != null && rendererList[i].material != null)
                {
                    string shaderName = rendererList[i].material.shader.name;
                    if (!shaderName.Contains(" ClipWithPanel"))
                    {
                        shaderName = rendererList[i].material.shader.name + " ClipWithPanel";
                        Shader shader = Shader.Find(shaderName);
                        if (shader != null)
                        {
                            rendererList[i].material.shader = shader;
                        }
                    }
                    rendererList[i].material.SetMatrix(mWorldToPanel, mMatrix);
                    rendererList[i].material.SetFloat("_MinX", minX);
                    rendererList[i].material.SetFloat("_MinY", minY);
                    rendererList[i].material.SetFloat("_MaxX", maxX);
                    rendererList[i].material.SetFloat("_MaxY", maxY);
                }
            }
        }
    }

	public void RefreshTargetRenderer()
	{
		if(mTarget == null)
			return;
		RefreshTargetRenderer (mTarget,frontOrBack,true);
	}

    /// <summary>
    /// 刷新
    /// </summary>
    public void RefreshTargetRenderer(UIWidget tagertWidget, int customQueue,bool clipWithPanel)
    {
        mGameObject = this.gameObject;

        if (mGameObject != null)
        {
            rendererList.Clear();
            Renderer[] rens = mGameObject.GetComponentsInChildren<Renderer>(true);
            int len = rens.Length;
            for (int i = 0; i < len; ++i)
            {
                if (rens[i] != null && rens[i].material != null && !rendererList.Contains(rens[i]))
                {
                    rendererList.Add(rens[i]);
                }
            }

            ParticleSystem[] ps = mGameObject.GetComponentsInChildren<ParticleSystem>(true);
            len = ps.Length;
            Renderer parRenderer = null;
            for (int i = 0; i < len; ++i)
            {
                parRenderer = ps[i].GetComponent<Renderer>();
                if (parRenderer != null && parRenderer.material != null && !rendererList.Contains(parRenderer))
                {
                    rendererList.Add(parRenderer);
                }
            }
        }
        mTarget = tagertWidget;
        frontOrBack = customQueue;
        needClipWithPanel = clipWithPanel;

    }


    /// <summary>
    /// 换裁剪模型shader
    /// </summary>
    /// <param name="role"></param>
    /// <param name="shader"></param>
    public void ChangeClipWithPanelShader(GameObject role, UIPanel clipPanel, bool clipWithPanel = true)
    {
        if (role == null || clipPanel == null)
            return;

        mClipPanel = clipPanel;
        needClipWithPanel = clipWithPanel;

        Shader shader = Shader.Find("Effect/ModelClipWithPanel");
        if (shader == null)
        {
            LogSystem.LogWarning("shader not found");
            return;
        }
        rendererList.Clear();
        Renderer[] rens = role.GetComponentsInChildren<Renderer>(true);
        int len = rens.Length;
        for (int i = 0; i < len; ++i)
        {
            if (rens[i] != null && rens[i].material != null && !rendererList.Contains(rens[i]))
            {
                rendererList.Add(rens[i]);
            }
        }

        int mWorldToPanel = Shader.PropertyToID("_WorldToPanel");
        float minX = clipPanel.mMin.x;
        float minY = clipPanel.mMin.y;
        float maxX = clipPanel.mMax.x;
        float maxY = clipPanel.mMax.y;
        Matrix4x4 mMatrix = clipPanel.worldToLocal;

        for (int i = 0; i < rendererList.Count; i++)
        {
            if (rendererList[i] != null && rendererList[i].material != null)
            {
                rendererList[i].material.shader = shader;
                rendererList[i].material.SetMatrix(mWorldToPanel, mMatrix);
                rendererList[i].material.SetFloat("_MinX", minX);
                rendererList[i].material.SetFloat("_MinY", minY);
                rendererList[i].material.SetFloat("_MaxX", maxX);
                rendererList[i].material.SetFloat("_MaxY", maxY);
            }
        }
    }

    /// <summary>
    /// 更新渲染队列
    /// </summary>
    public void UpdateRendererQueue()
    {
        if (mTarget == null || mTarget.drawCall == null)
            return;

        int queue = mTarget.drawCall.renderQueue;
        queue += frontOrBack;

        if (lastRendererQueue != queue)
        {
            lastRendererQueue = queue;

            if (rendererList == null || rendererList.Count == 0)
                return;

            if (mTarget.panel != null && needClipWithPanel)
            {
                int mWorldToPanel = Shader.PropertyToID("_WorldToPanel");

                float minX = mTarget.panel.mMin.x;
                float minY = mTarget.panel.mMin.y;
                float maxX = mTarget.panel.mMax.x;
                float maxY = mTarget.panel.mMax.y;
                Matrix4x4 mMatrix = mTarget.panel.worldToLocal;

                for (int i = 0; i < rendererList.Count; i++)
                {
                    if (rendererList[i] != null)
                    {
                        rendererList[i].material.renderQueue = lastRendererQueue;

                        rendererList[i].material.SetMatrix(mWorldToPanel, mMatrix);
                        rendererList[i].material.SetFloat("_MinX", minX);
                        rendererList[i].material.SetFloat("_MinY", minY);
                        rendererList[i].material.SetFloat("_MaxX", maxX);
                        rendererList[i].material.SetFloat("_MaxY", maxY);
                    }
                }
            }
            else
            {
                for (int i = 0; i < rendererList.Count; i++)
                {
                    if (rendererList[i] != null)
                        rendererList[i].material.renderQueue = lastRendererQueue;
                }
            }
        }
        
    }

    void OnDestroy()
    {
        mRenderQueues.Remove(this);
        mGameObject = null;
        mTarget = null;
        mClipPanel = null;
        rendererList.Clear();
    }

}
