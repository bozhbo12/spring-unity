using System;
using System.Collections.Generic;
using UnityEngine;
//using XLua;//WEIBOBUG

/// <summary>
/// 对象加载完成回调
/// </summary>
/// <param name="odata">已加载对象</param>
public delegate void LoadAssetCallback(params object[] args);

/// <summary>
/// 对象加载完成回调
/// </summary>
/// <param name="odata">已加载对象</param>
public delegate void AssetCallback(UnityEngine.Object oAsset, string strFileName, VarStore varStore);

/// <summary>
/// 动画播放时回调
/// </summary>
/// <param name="strAnimationName"></param>
/// <param name="aProxy"></param>
public delegate void OnAnimationPlayCallBack(string strAnimationName, AnimationProxy aProxy, params object[] args);

/// <summary>
/// 动画播放后回调
/// </summary>
/// <param name="aProxy"></param>
/// <param name="strAnimationName"></param>
/// <param name="strNextAnimation"></param>
public delegate void OnWatchAnimationPlayed(AnimationProxy aProxy, string strAnimationName, string strNextAnimation);

/// <summary>
/// 图集所有加载完毕回调
/// </summary>
/// <param name="oGo"></param>
/// <param name="args"></param>
public delegate void OnUIWidgetAtlasAllLoaded(GameObject oGo, params object[] args);

/// <summary>
/// 当个控件加载完毕回调，提供最终回调参数
/// </summary>
/// <param name="spList"></param>
/// <param name="onLoaded"></param>
/// <param name="oGo"></param>
/// <param name="args"></param>
public delegate void OnUIWidgetAtlasLoaded(UIWidget[] spList, OnUIWidgetAtlasAllLoaded onLoaded, GameObject oGo, params object[] args);

/// <summary>
/// 动作中触发播放效果组(用于卡帧)
/// </summary>
/// <param name="strEffectGroupID"></param>
public delegate void AniamtionEventDelegate(string strEffectGroupID);

/// <summary>
/// 事件回调原型
/// </summary>
public delegate void OnActionCallback();

/// <summary>
/// 路点寻路
/// </summary>
/// <param name="paths"></param>
public delegate void PathFindCallBack(List<Vector3> paths);

/// <summary>
/// 加载ab包
/// </summary>
/// <param name="strAbPath"></param>
/// <param name="callback"></param>
/// <param name="varStore"></param>
public delegate void LoadAssetBundleDelegateProxy(string strAbPath, AssetBundleCallBackDelegateProxy callback, VarStore varStore);


/// <summary>
/// ab加载完成回调
/// </summary>
/// <param name="ab"></param>
/// <param name="varStore"></param>
public delegate void AssetBundleCallBackDelegateProxy(AssetBundle assetBundle, VarStore varStore);

/// <summary>
/// ab资源加载回调
/// </summary>
/// <param name="o"></param>
/// <param name="strOrignFileName"></param>
/// <param name="callback"></param>
/// <param name="varStore"></param>
public delegate void OnLoadCallBack(UnityEngine.Object o, string strOrignFileName, AssetCallback callback, VarStore varStore);

/*//WEIBOBUG
/// <summary>
/// 
/// </summary>
/// <param name="strPath"></param>
/// <param name="oAsset"></param>
#if LUA
[CSharpCallLua]
#endif
public delegate void IViewTextAssetDelegateProxy(string strPath, TextAsset oAsset);


/// <summary>
/// 模型加载
/// </summary>
/// <param name="strPath"></param>
/// <param name="oAsset"></param>
#if LUA
[CSharpCallLua]
#endif
public delegate void IViewModelDelegateProxy(string strPath, GameObject oAsset);

/// <summary>
/// 加载材质
/// </summary>
/// <param name="strPath"></param>
/// <param name="oAsset"></param>
#if LUA
[CSharpCallLua]
#endif
public delegate void IViewMatDelegateProxy(string strPath, Material oAsset);

/// <summary>
/// 界面鼠标事件
/// </summary>
/// <param name="o"></param>
/// <param name="obj"></param>
#if LUA
[CSharpCallLua]
#endif
public delegate void IViewMouseEventDelegateProxy(GameObject o, object obj);

/// <summary>
/// 获取场景lightmap数量
/// </summary>
/// <param name="strScene"></param>
/// <returns></returns>
#if LUA
[CSharpCallLua]
#endif
public delegate int GetLightMapCountDelegateProxy(string strSceneID);
*/