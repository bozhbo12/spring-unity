using UnityEngine;

public class SZUIRenderQueue : MonoBehaviour
{

    public int renderQueue = 3000;
    public bool runOnlyOnce = false;

    void Start()
    {
        SetRenderQueue();
    }

    void SetRenderQueue()
    {
        Renderer[] rens = GetComponentsInChildren<Renderer>(true);
        if (rens != null)
        {
            for (int i = 0; i < rens.Length; i++)
            {
                rens[i].material.renderQueue = renderQueue;//将特效显示在最上层
            }
        }
        if (runOnlyOnce && Application.isPlaying)
        {
            this.enabled = false;
        }
    }
}
