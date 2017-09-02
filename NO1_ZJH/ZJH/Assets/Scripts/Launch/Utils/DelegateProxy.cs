using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 委托代理类
/// </summary>
public class DelegateProxy {

    public delegate string StringBuilderDelegateProxy(params object[] args);
    public static StringBuilderDelegateProxy StringBuilderPorxy;
    /// <summary>
    /// 组装字串
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string StringBuilder(params object[] args)
    {
        if (StringBuilderPorxy != null)
            return StringBuilderPorxy(args);

        string sTemp = string.Empty;
        for (int i = 0; i < args.Length; i++)
        {
            sTemp += (args[i].ToString());
        }
        return sTemp;
    }

    public delegate void SetObjRenderQDelegateProxy(GameObject oModel, int iLayer, int iRenderQueue);
    /*public static SetObjRenderQDelegateProxy SetObjRenderQProxy;
    /// <summary>
    /// 设置渲染层
    /// </summary>
    /// <param name="oModel"></param>
    /// <param name="iLayer"></param>
    /// <param name="iRenderQueue"></param>
    public static void SetObjRenderQ(GameObject oModel, int iLayer, int iRenderQueue)
    {
        if (SetObjRenderQProxy != null)
            SetObjRenderQProxy(oModel, iLayer, iRenderQueue);
    }*/

    public delegate void GameDestoryPoolDelegateProxy(GameObject obj);
    public static GameDestoryPoolDelegateProxy GameDestoryPoolProxy;
    /// <summary>
    /// 删除对象Pool
    /// </summary>
    /// <param name="obj"></param>
    public static void GamePoolDestory(GameObject obj)
    {
        if (GameDestoryPoolProxy != null)
            GameDestoryPoolProxy(obj);
    }

    public delegate void ShowViewDelegateProxy(string name, object arg = null,bool hideother=true);
    public static ShowViewDelegateProxy ShowViewProxy;
    /// <summary>
    /// 显示界面
    /// </summary>
    /// <param name="name"></param>
    /// <param name="arg"></param>
    public static void ShowView(string name, object arg = null, bool hideother = true)
    {
        if (ShowViewProxy != null)
            ShowViewProxy(name, arg);
    }

    public delegate void DestroyViewDelegateProxy(string name);
    public static DestroyViewDelegateProxy DestroyViewProxy;
    /// <summary>
    /// 删除界面
    /// </summary>
    /// <param name="name"></param>
    public static void DestroyView(string name)
    {
        if (DestroyViewProxy != null)
            DestroyViewProxy(name);
    }


    public delegate void LoadAssetDelegateProxy(string strFileName, AssetCallback back, VarStore varStore = null, bool bAsync = false);
    static public LoadAssetDelegateProxy LoadAssetProxy;
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="strFileName"></param>
    /// <param name="callback"></param>
    public static void LoadAsset(string strFileName, AssetCallback callback, VarStore varStore = null, bool bAsync = false)
    {
        if (LoadAssetProxy != null)
        {
            LoadAssetProxy(strFileName, callback, varStore, bAsync);
        }
        else
        {
            UnityEngine.Object oAsset = UnityEngine.Resources.Load(strFileName);
            callback(oAsset, strFileName, varStore);
            LogSystem.LogWarning("ResourceManager.LoadAsset is null");
        }
    }

    public delegate void UnloadAssetDelegateProxy(object[] args);
    static public UnloadAssetDelegateProxy UnloadAssetProxy;
    /// <summary>
    /// 释放内存
    /// </summary>
    /// <param name="args"></param>
    public static void UnloadAsset(object[] args)
    {
        if (UnloadAssetProxy != null)
            UnloadAssetProxy(args);
    }

    //utiltools
    public delegate void DestroyEffectDelegateProxy(GameObject obj, int layer);
    static public DestroyEffectDelegateProxy DestroyEffectProxy;
    /// <summary>
    /// 删除指定层
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layer"></param>
    public static void DestroyEffect(GameObject obj, int layer)
    {
        if (DestroyEffectProxy != null)
            DestroyEffectProxy(obj, layer);
    }

    public delegate bool HasViewDelegateProxy(string name);
    static public HasViewDelegateProxy HasViewProxy;
    /// <summary>
    /// 是否有界面
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool HasView(string name)
    {
        if (HasViewProxy != null)
            return HasViewProxy(name);

        return false;
    }

    public delegate void HideViewDelegateProxy(string name);
    static public HideViewDelegateProxy HideViewProxy;
    /// <summary>
    /// 隐藏界面
    /// </summary>
    /// <param name="name"></param>
    public static void HideView(string name)
    {
        if (HideViewProxy != null)
            HideViewProxy(name);
    }

    public delegate void PlayUIAudioDelegateProxy(int index);
    static public PlayUIAudioDelegateProxy PlayUIAudioProxy;
    /// <summary>
    /// 播放界面音效
    /// </summary>
    /// <param name="index"></param>
    public static void PlayUIAudio(int index)
    {
        if (PlayUIAudioProxy != null)
            PlayUIAudioProxy(index);
    }

    public delegate void GetGuiCompentDelegateProxy(Dictionary<int, GameObject> lastgameObject, ref List<Component> newPanels, ref List<Component> tempPanels);
    static public GetGuiCompentDelegateProxy GetGuiCompentProxy;
    /// <summary>
    /// 不知什么功能
    /// </summary>
    /// <param name="lastgameObject"></param>
    /// <param name="newPanels"></param>
    /// <param name="tempPanels"></param>
    public static void GetGuiCompent(Dictionary<int, GameObject> lastgameObject, ref List<Component> newPanels, ref List<Component> tempPanels)
    {
        if (GetGuiCompentProxy != null)
            GetGuiCompentProxy(lastgameObject, ref newPanels, ref tempPanels);
    }

    public delegate void OnShareCallbackDelegateProxy(string result);
    static public OnShareCallbackDelegateProxy OnShareCallbackProxy;
    /// <summary>
    /// 分享回调
    /// </summary>
    /// <param name="result">0:成功 -2:取消 -3:发送失败 -4:授权失败 -5:微信不支持</param>
    public static void OnShareCallback(string result)
    {
        if (OnShareCallbackProxy != null)
            OnShareCallbackProxy(result);
    }

//    public delegate void OnSetPayCallbackDelegateProxy(List<IAPProductInfo> productInfos);//WEIBOBUG
	//    static public OnSetPayCallbackDelegateProxy OnSetPayCallbackProxy;//WEIBOBUG
    /// <summary>
    /// 苹果商品回调
    /// </summary>
    /// <param name="result"></param>
//    public static void OnSetPayCallback(List<IAPProductInfo> productInfos)//WEIBOBUG
//    {
//        if (OnSetPayCallbackProxy != null)
//            OnSetPayCallbackProxy(productInfos);
//    }

    public delegate void OnProductOrderDelegateProxy(string orderid, string pid);
    static public OnProductOrderDelegateProxy OnProductOrder;
    public static void OnProductOrderCallback(string orderid, string pid)
    {
        if (OnProductOrder != null)
            OnProductOrder(orderid, pid);
    }

    public delegate void OnAppStoreTransactionDelegateProxy(string orderid);
    static public OnAppStoreTransactionDelegateProxy OnAppStoreTransaction;
    public static void OnAppStoreTransactionCallback(string orderid)
    {
        if (OnAppStoreTransaction != null)
            OnAppStoreTransaction(orderid);
    }

    public delegate bool OnSendMessageCallbackDelegateProxy(int iMSG, params object[] objects);
    static public OnSendMessageCallbackDelegateProxy onSendMessageCallbackDelegateProxy;

    /// <summary>
    /// 点击按钮发送消息回调
    /// </summary>
    /// <param name="iMSG"></param>
    /// <param name="objects"></param>
    public static void OnSendMessageCallback(int iMSG, params object[] objects)
    {
        if (onSendMessageCallbackDelegateProxy != null)
            onSendMessageCallbackDelegateProxy(iMSG, objects);
    }


	public delegate void GetEffectObjDelegateProxy(string strEffectUrl, AssetCallback loadComplete, VarStore varStore = null, bool bAsync = false);
    static public GetEffectObjDelegateProxy GetEffectObjProxy;
    /// <summary>
    /// 获取特效对象
    /// </summary>
    /// <param name="strEffectUrl"></param>
    /// <param name="loadComplete"></param>
	public static void GetEffectObj(string strEffectUrl, AssetCallback loadComplete, VarStore varStore = null, bool bAsync = false)
    {
        if (GetEffectObjProxy != null)
			GetEffectObjProxy(strEffectUrl, loadComplete , varStore, bAsync);
    }

    public delegate void CollectObjectDelegateProxy(string strEffectUrl, GameObject oEffect);
    static public CollectObjectDelegateProxy CollectObjectProxy;
    /// <summary>
    /// 回调特效
    /// </summary>
    /// <param name="strEffectUrl"></param>
    /// <param name="loadComplete"></param>
    public static void CollectObject(string strEffectUrl, GameObject oEffect)
    {
        if (CollectObjectProxy != null)
            CollectObjectProxy(strEffectUrl, oEffect);
    }

    public delegate bool bWuFengMapIngDelegateProxy();
    static public bWuFengMapIngDelegateProxy bWuFengMapIngProxy;
    /// <summary>
    /// 无缝地图
    /// </summary>
    /// <param name="strEffectUrl"></param>
    /// <param name="loadComplete"></param>
    public static bool bWuFengMapIng()
    {
        if (bWuFengMapIngProxy != null)
            return bWuFengMapIngProxy();
        return false;
    }

    public delegate void EffectResetDelegateProxy(GameObject oEffect);
    static public EffectResetDelegateProxy EffectResetProxy;
    /// <summary>
    /// 重置特效
    /// </summary>
    /// <param name="strEffectUrl"></param>
    /// <param name="loadComplete"></param>
    public static void EffectReset(GameObject oEffect)
    {
        if (EffectResetProxy != null)
            EffectResetProxy(oEffect);
    }
    /// <summary>
    /// 手动启动引导
    /// </summary>
    /// <param name="step"></param>
    /// <param name="index"></param>
    public delegate void PlayGuideDelegateProxy(int step, int index);
    static public PlayGuideDelegateProxy PlayGuideProxy;
    public static void PlayGuide(int step, int index)
    {
        if (PlayGuideProxy != null)
        {
            PlayGuideProxy(step, index);
        }
    }

    /// <summary>
    /// 开关摇杆
    /// </summary>
    public delegate void SetJoyStickEnable (bool _b);
    static public SetJoyStickEnable _joyStickEnable;
    public static void setJoyStickEnable(bool _b)
    {
        if (_joyStickEnable != null)
        {
            _joyStickEnable(_b);
        }
    }

    public delegate bool GetSceneResIDDelegateProxy(ref string strSceneResID);
    static public GetSceneResIDDelegateProxy GetSceneResIDProxy;
    
    /// <summary>
    /// 获取场景资源id
    /// </summary>
    /// <returns></returns>
    public static bool GetSceneResID(ref string strSceneResID)
    {
        if (GetSceneResIDProxy != null)
            return GetSceneResIDProxy(ref strSceneResID);
        return false;
    }


    public delegate bool RequestPathsDelegateProxy(Vector3 startPos, Vector3 endPos, PathFindCallBack callBack);
    static public RequestPathsDelegateProxy RequestPathsProxy;

    /// <summary>
    /// 路点寻路
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="callBack"></param>
    /// <returns></returns>
    public static bool RequestPaths(Vector3 startPos, Vector3 endPos, PathFindCallBack callBack)
    {
        if (RequestPathsProxy != null)
        {
            return RequestPathsProxy(startPos, endPos, callBack);
        }
        return false;
    }

    public delegate bool bIsWayInitOkDelegateProxy();
    static public bIsWayInitOkDelegateProxy bIsWayInitOkProxy;
    /// <summary>
    /// 路点寻路是否初使化
    /// </summary>
    /// <returns></returns>
    public static bool bIsWayInitOk()
    {
        if (bIsWayInitOkProxy != null)
        {
            return bIsWayInitOkProxy();
        }
        return false;
    }


    static public LoadAssetBundleDelegateProxy LoadAssetBundleProxy;
    
    /// <summary>
    /// 加载ab包
    /// </summary>
    /// <param name="strAbPath"></param>
    /// <param name="callback"></param>
    /// <param name="varStore"></param>
    public static void LoadAssetBundle(string strAbPath, AssetBundleCallBackDelegateProxy callback, VarStore varStore)
    {
        if (LoadAssetBundleProxy != null)
        {
            LoadAssetBundleProxy(strAbPath, callback, varStore);
        }
        else
        {
            LogSystem.LogWarning("LoadAssetBundleProxy is null!!");
        }
    }

//    static public GetLightMapCountDelegateProxy GetLightMapCountProxy;

    /// <summary>
    /// 获取lightmap_Count
    /// </summary>
    /// <param name="strScene"></param>
    /// <returns></returns>
    public static int GetLightMapCount(string strSceneID)
    {
//        if (GetLightMapCountProxy != null)//WEIBOBUG
//        {
//            return GetLightMapCountProxy(strSceneID);
//        }
        
            return 1;
    }


    /// <summary>
    /// cg初始化  关闭主相机
    /// </summary>
    /*public delegate void SetMainCameraCullingMask(bool _b , float _tiem);
    static public SetMainCameraCullingMask _setMainCameraCullingMask;
    public static void setMainCameraCullingMask(bool _b, float _tiem)
    {
        if (_setMainCameraCullingMask != null)
        {
            _setMainCameraCullingMask(_b, _tiem);
        }
    }*/
}
