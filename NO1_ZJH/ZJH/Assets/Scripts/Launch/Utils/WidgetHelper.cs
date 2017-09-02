using UnityEngine;
using System.Collections.Generic;

public class WidgetHelper : MonoBehaviour
{

    void Awake()
    {
        Instance.Set<WidgetHelper>(this);
    }

    /// <summary>
    /// 等待加载的界面
    /// </summary>
    private DictionaryEx<GameObject, OnUIWidgetAtlasAllLoaded> waitLoadPanel = new DictionaryEx<GameObject, OnUIWidgetAtlasAllLoaded>();

    public void LoadPrefabUISprite(GameObject oGo, OnUIWidgetAtlasAllLoaded onLoaded, params object[] args)
    {
        if (oGo == null)
        {
            if (onLoaded != null)
                onLoaded(oGo);
            return;
        }
       
        UIWidget[] spList = oGo.GetComponentsInChildren<UIWidget>(true);
        if (spList == null)
        {
            if (onLoaded != null)
                onLoaded(oGo);
            return;
        }

        if (waitLoadPanel == null)
            waitLoadPanel = new DictionaryEx<GameObject, OnUIWidgetAtlasAllLoaded>();

        for (int index = 0; index < spList.Length; index++)
        {
            UIWidget sp = spList[index];
            if (sp == null)
                continue;
            sp.CheckLoadAtlas();
        }
        waitLoadPanel[oGo] = onLoaded;
    }

    bool OnWidgetLoad(GameObject oGo)
    {
        if (oGo == null) return true;
        UIWidget[] spList = oGo.GetComponentsInChildren<UIWidget>(true);
        if (spList == null) return true;
        for (int i = 0; i < spList.Length; i++)
        {
            UIWidget sp = spList[i];
            if (sp == null)
                continue;

            if (sp.CheckWaitLoadingAtlas())
            {
                return false;
            }
        }
        return true;
    }

    void Update()
    {
        if (waitLoadPanel == null || waitLoadPanel.Count <= 0) return;
        for (int i = waitLoadPanel.mList.Count - 1; i >= 0; --i)
        {
            GameObject oGo = waitLoadPanel.mList[i];
            if (oGo == null) continue;
            OnUIWidgetAtlasAllLoaded onLoaded = waitLoadPanel[oGo];
            if (OnWidgetLoad(oGo))
            {
                waitLoadPanel.Remove(oGo);
                if (onLoaded != null)
                {
                    onLoaded(oGo);
                }
            }
        }
    }
}
