///<summary>
///实现目标：对象或者资源高速缓存器
///作者：wanglc
///编写日期： 2014-12-02
///</summary>
using System.Collections.Generic;
using UnityEngine;
/* 缓存池说明
 * 目的：减少资源内存消耗问题
 *      1，减少IO操作次数，避免加载资源；
 *      2，减少多次实例化资源的消耗，会造成内存分配消耗；
 *      3，实例化多次，在包系统时，如果不是同一个预制件分配，会有多份实例；
 *      4，系统会每隔一段时间，回收为使用的对象，以缩减缓存池对象数目,达到优化控制目标；
 * 使用方法要点:
 *      1,统一使用InstantiatePool,InstantiateClone()接口，不在使用系统的Instantiate接口，原因，统一管理资源实例化对象；
 *      2,统一使用DestroyPoolObject, DestroyClone() DestroyImmediatePool等删除接口，原因，统一管理资源实例化对象；
 *      3,monobehaiver的删除与实例，有对应的接口，不要使用系统的Instantiate接口，目前是实例化不出来的，所以UISPRIT UIlabel的实例化现在是不可以使用的；如需使用使用Clone或者pool
 *      4,new 自定义对象 ，常用的此类对象可以使用CacheObject基类使用对象的缓存与回收
 *      5,平时注意缓存池中的对象列表，开发时可按快捷键输出到日志系统，对同一数量实例多的对象要检查是否存在未释放问题；
 *      6,缓存对象的重置问题，会是后续开发中需要关注的问题，特效重置已经完成，但其他用户资源实现的类在重置是可以参照ModelLoadEffect的重置方法实现.
 *      
 * 
 * 决战缓存池
 * 1.PrefabPool	预制件缓存
 * 2.ClonePool	复制缓存
 * 3.CachePool	脚本对象缓存
 *
 * 魔龙与决战区别
 * 1.预制件缓存
 *  (界面不缓存,切换场景时会清空缓存池，导致缓存的界面全部删除,致无缝地图不连贯)
 *
 * 2.复制缓存
 *  (暂没有使用,改动太多,新功能可以使用,但要修改相关代码)
 *
 * 3.脚本对象缓存
 *  (暂没有使用,改动太多,新功能可以使用)
 */

/// <summary>
/// 对象高速缓存容器
/// </summary>
public class CacheObjects
{
    #region pool缓存池相关

    #region 创建与清除

    /// <summary>
    /// 模型缓存
    /// </summary>
    private static SpawnPool mSpawnPool = null;

    public static SpawnPool spawnPool
    {
        get
        {
            if (mSpawnPool == null)
            {
                InitSpawnPool();
            }
            return mSpawnPool;
        }
    }

    /// <summary>
    /// 初始化缓存池
    /// </summary>
    private static void InitSpawnPool()
    {
        if (mSpawnPool != null)
            return;

        //如果已有此节点，就先删掉
        GameObject obj = GameObject.Find("_CacheObjects");
        if (obj != null)
        {
            GameObject.Destroy(obj);
        }
        mSpawnPool = CachePoolManager.Pools.Create("_CacheObjects");

        if (Config.bAndroid)
        {
            //AndroidInterface.unityTrimMemoryListener(MemoryWarning);
        }
    }

    /// <summary>
    /// 内存警告
    /// </summary>
    /// <param name="args"></param>
    private static void MemoryWarning(string[] args)
    {
        //if (Config.bDebugModel)
        //{
        //    Debug.LogError("MemoryWarning:: Available:", SnailPluginsUtils.getInstance.GetAvailableMemorySize());
        //}
//        LogSystem.LogWarning("MemoryWarning:: Available:", AndroidInterface.getAvailableMemorySize());
    }

    /// <summary>
    /// 清空缓存池
    /// </summary>
    public static void ClearSpawnPool()
    {
        if (spawnPool != null)
        {
            spawnPool.DestroyPool();
        }
    }

    /// <summary>
    /// 清除缓存池中没有显示的对象
    /// </summary>
    public static void DestroyUnDisplayObject()
    {
        if (spawnPool != null)
        {
            spawnPool.DestroyUnDisplayObject();
        }
    }

    #endregion

    #region 预制件缓存方法

    /// <summary>
    /// 是否在缓存中
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static bool IsSpawnObject(Transform trans)
    {
        return spawnPool.IsSpawnObject(trans);
    }

    /// <summary>
    /// 实例化(SpawnPool)
    /// </summary>
    /// <param name="o"></param>
    /// <param name="postion"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static UnityEngine.Object InstantiatePool(UnityEngine.Object oPrefab, Vector3 postion, Quaternion rotation)
    {
        if (oPrefab != null)
        {
            GameObject poolGo = oPrefab as GameObject;
            if (poolGo != null)
            {
                Transform poolTrans = null;
                if (!Application.isPlaying) // 当前游戏是否在运行
                {
                    poolTrans = Object.Instantiate(poolGo.transform, postion, rotation) as Transform;
                }
                else
                {
#if NOUSEPOOL
                    poolTrans = Object.Instantiate(poolGo.transform, postion, rotation) as Transform;
#else
                    poolTrans = spawnPool.Spawn(poolGo.transform, postion, rotation);
#endif
                }

                if (poolTrans != null)
                {
                    poolTrans.parent = null;
                    return poolTrans.gameObject;
                }
            }
        }
        return null;
    }

    public static bool ChechPrefabInPool (UnityEngine.Object oPrefab)
    {
        return spawnPool.checkGameObjectInPool((oPrefab as GameObject).transform);
    }

    public static UnityEngine.Object InstantiatePool(UnityEngine.Object oPrefab)
    {
        return InstantiatePool(oPrefab, Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// 删除对象(Pool)
    /// </summary>
    /// <param name="o"></param>
    /// <param name="bOnlyClear">没有显示对象时，清除这个对象的缓存池</param>
    public static void DestoryPoolObject(UnityEngine.Object o, bool bOnlyClear = false)
    {
        if (o == null)
            return;

        if (o is GameObject)
        {
            GameObject go = o as GameObject;

            if (!Application.isPlaying)
            {
                Object.Destroy(o);
            }
            else
            {
#if NOUSEPOOL
                Object.Destroy(o);
#else
                //是否是缓存池分配出来的
                if (spawnPool.IsPoolObject(go.transform))
                {
                    spawnPool.Despawn(go.transform, bOnlyClear);
                    return;
                }
                else
                {
                    Object.Destroy(o);
#if UNITY_EDITOR
                    CheckObjectIsPool(go.transform);
#endif
                }
#endif
            }
        }
        else
        {
            //去除资源引用
            CacheObjects.PopCache(o);
        }
    }

    /// <summary>
    /// 删除对象子节点中的缓存池对象
    /// 1.递归清除子节点缓存池对象
    /// 2.删除自身对象
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="bOnlyClear">没有显示对象时，清除这个对象的缓存池</param>
    public static void DestroyChildPoolObject(Transform trans, bool bOnlyClear = false)
    {
        if (trans == null)
            return;

        DestroyChildTree(trans, bOnlyClear);
        GameObject.Destroy(trans.gameObject);
    }

    /// <summary>
    /// 删除对象子节点中的缓存池对象
    /// </summary>
    /// <param name="trans"></param>
    private static void DestroyChildTree(Transform trans, bool bOnlyClear = false)
    {
        if (spawnPool.IsPoolObject(trans))
        {
            spawnPool.Despawn(trans, bOnlyClear);
        }
        else
        {
            if (trans.childCount == 0)
                return;

            for (int i = trans.childCount - 1; i >= 0; i--)
            {
                DestroyChildTree(trans.GetChild(i), bOnlyClear);
            }
        }
    }

    /// <summary>
    /// 重点检查对象:根节点不是缓存池分配出来，但子节点时
    /// </summary>
    /// <param name="trans"></param>
    private static void CheckObjectIsPool(Transform trans)
    {
        if (spawnPool.IsPoolObject(trans))
        {
            LogSystem.LogWarning("CacheObject::", "资源泄露:", trans.gameObject.name);
            return;
        }
        for(int i = 0; i<trans.childCount; i++)
        {
            CheckObjectIsPool(trans.GetChild(i));
        }
    }

    /// <summary>
    /// 删除对象(Pool)
    /// 当没有缓存列表中没有可显示对象时，删除该缓存池
    /// </summary>
    /// <param name="go"></param>
    public static void DestoryPoolObjectImmediate(UnityEngine.Object o)
    {
        if (o == null)
            return;

        if (o is GameObject)
        {
            GameObject go = o as GameObject;

            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(o);
            }
            else
            {
#if NOUSEPOOL
                Object.DestroyImmediate(o);
#else
                //是否是缓存池分配出来的
                if (spawnPool.IsPoolObject(go.transform))
                {
                    spawnPool.Despawn(go.transform, true);
                    return;
                }
                else
                {
                    Object.DestroyImmediate(o);
#if UNITY_EDITOR
                    CheckObjectIsPool(go.transform);
#endif
                }
#endif
            }
        }
        else
        {
            //去除资源引用
            CacheObjects.PopCache(o);
        }
    }

    /// <summary>
    /// 获取当前对象的预制件
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public static Transform GetObjectPrefab(Transform trans)
    {
        return spawnPool.GetObjectPrefab(trans);
    }

    #endregion

    #region 克隆对象方法(暂没用缓存)

    /// <summary>
    /// 克隆对象
    /// </summary>
    /// <param name="o"></param>
    /// <param name="postion"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static UnityEngine.GameObject InstantiateClone(UnityEngine.GameObject o)
    {
        if (o == null)
            return null;

        return GameObject.Instantiate(o);

        //        GameObject poolGo = o as GameObject;
        //        if (poolGo == null)
        //            return null;

        //        Transform poolTrans = null;

        //        if (!Application.isPlaying)
        //        {
        //            poolTrans = Object.Instantiate(poolGo.transform) as Transform;
        //        }
        //        else
        //        {
        //#if NOUSEPOOL
        //                poolTrans = Object.Instantiate(poolGo.transform) as Transform;
        //#else
        //            poolTrans = spawnPool.Clone(poolGo.transform);
        //#endif
        //        }
        //        if (poolTrans == null)
        //            return null;

        //        return poolTrans.gameObject;
    }

    /// <summary>
    /// 逻辑删除Clone对象
    /// </summary>
    /// <param name="oAsset"></param>
    public static void DestroyClone(UnityEngine.GameObject o)
    {
        if (o == null)
            return;

        GameObject.Destroy(o);
        //        GameObject poolGo = null;
        //        if (oAsset is Behaviour)
        //        {
        //            poolGo = (oAsset as Behaviour).gameObject;
        //        }
        //        else if (oAsset is Component)
        //        {
        //            poolGo = (oAsset as Component).gameObject;
        //        }
        //        else
        //        {
        //            poolGo = oAsset as GameObject;
        //        }

        //        if (poolGo == null)
        //        {
        //            return;
        //        }

        //        if (!Application.isPlaying)
        //        {
        //            Object.Destroy(oAsset);
        //        }
        //        else
        //        {
        //#if NOUSEPOOL
        //                    Object.Destroy(oAsset);
        //#else
        //            spawnPool.DespawnClone(poolGo.transform);
        //#endif
        //        }
    }

    #endregion

    #region 脚本缓存池

    /// <summary>
    /// 获得对象池中的数据
    /// </summary>
    /// <returns></returns>
    public static T SpawnCache<T>() where T : CacheObject
    {
        return spawnPool.SpawnCache<T>();
    }

    /// <summary>
    /// 获得对象池中的数据
    /// </summary>
    /// <returns></returns>
    public static void DespawnCache(CacheObject oCache)
    {
        spawnPool.DespawnCache(oCache);
    }

    #endregion

    #region Component克隆与删除

    /// <summary>
    /// 实例化组件
    /// </summary>
    /// <param name="o"></param>
    /// <param name="postion"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static UnityEngine.Component InstantiateComponent(UnityEngine.Component component)
    {
        if (component != null)
        {
            return Object.Instantiate(component);
        }
        return null;
    }

    /// <summary>
    /// 删除组件
    /// </summary>
    /// <param name="o"></param>
    /// <param name="postion"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static void DestroyComponent(UnityEngine.Component component)
    {
        if (component != null)
        {
            Object.Destroy(component);
        }
    }

    /// <summary>
    /// 立即删除组件
    /// </summary>
    /// <param name="o"></param>
    /// <param name="postion"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static void DestroyComponentImmediate(UnityEngine.Component component)
    {
        if (component != null)
        {
            Object.DestroyImmediate(component);
        }
    }

    #endregion

    #endregion

    /// <summary>
    /// 缓存信息单元
    /// </summary>
    public class CacheInfo
    {
        /// <summary>
        /// 最新句柄
        /// </summary>
        public UnityEngine.Object oAsset = null;
        /// <summary>
        /// 使用频率(不除了，使用单位时间次数代替频率)
        /// </summary>
        public int iUseTimes = 0;
#if DEV
        public float fCreateTime = 0.0f;
#endif
        /// <summary>
        /// 路径
        /// </summary>
        public string strFileName = string.Empty;

        /// <summary>
        /// 是否GameObject类型
        /// </summary>
        public bool bGameObject
        {
            get
            {
                return oAsset is UnityEngine.GameObject;
            }
        }

        public override string ToString()
        {
            return strFileName+"/"+oAsset.name;
        }
#if DEV
        public float fUsedTime
        {
            get
            {
                return Time.realtimeSinceStartup - fCreateTime;
            }
        }
#endif
    }

    /// <summary>
    /// 缓存列表
    /// </summary>
    private static Dictionary<string, CacheInfo> mForeverCacheMaps = new Dictionary<string, CacheInfo>();

#if DEV
    private static List<string> mAssetLoaded = new List<string>();
#endif

    /// <summary>
    /// 路径到缓存对象的映射关系
    /// </summary>
    private static Dictionary<string, CacheInfo> mNameToCacheMaps = new Dictionary<string, CacheInfo>();

    /// <summary>
    /// 对象到缓存映射信息
    /// </summary>
    private static Dictionary<UnityEngine.Object, CacheInfo> mObjectToCacheMaps = new Dictionary<UnityEngine.Object, CacheInfo>();

    /// <summary>
    /// 包系统中相关回调
    /// </summary>
    /// <param name="oAsset"></param>
    public delegate void OnRemoveObjectsDepends(Object oAsset);
    public static OnRemoveObjectsDepends onremoveObject = null;
    public delegate void OnClearAllDepends();
    public static OnClearAllDepends onclearObjects = null;

    /// <summary>
    /// 添加全局缓存对象
    /// </summary>
    /// <param name="strName"></param>
    public static void PushForeverCache(string strName, UnityEngine.Object o)
    {
        if (!mForeverCacheMaps.ContainsKey(strName))
        {
            CacheInfo cInfo = new CacheInfo();
            cInfo.iUseTimes = 1;
#if DEV
            cInfo.fCreateTime = Time.realtimeSinceStartup;
#endif
            cInfo.oAsset = o;
            mForeverCacheMaps.Add(strName, cInfo);
        }
    }

    /// <summary>
    /// 取缓存单元信息
    /// </summary>
    /// <param name="o">缓存对象</param>
    /// <returns>缓存对象单元</returns>
    public static CacheInfo GetCacheObject(string strName)
    {
        CacheInfo oCacheInfo = null;
        if (mNameToCacheMaps.TryGetValue(strName, out oCacheInfo))
        {
            return oCacheInfo;
        }

        return null;
    }

    /// <summary>
    /// 缓存对象添加到高度缓存
    /// </summary>
    /// <param name="o">混成对象</param>
    public static bool PushCache(string strName, UnityEngine.Object oAsset)
    {
        //textAsset文件不缓存Assets文件
        if (oAsset == null)
            return false;

        ///如果是预制件对象，要检查是否在缓存池中使用，使用的不需要多次添加
        if (oAsset is GameObject)
        {
            GameObject go = oAsset as GameObject;
            //缓存池中已经存在不处理
            if (spawnPool.IsPrefabUsing(go.transform))
            {
                return false;
            }
        }

        //第一次加入到资源缓存池中
        if (!mNameToCacheMaps.ContainsKey(strName))
        {
            FirstAddCacheInfo(strName, oAsset);
        }
        else
        {
            CacheInfo cacheInfo = mNameToCacheMaps[strName];
            if (cacheInfo != null)
            {
                cacheInfo.iUseTimes++;
            }
            else
            {
                LogSystem.LogWarning("PushCache::oAsset is null:", strName);
            }
        }
        return true;
    }

    /// <summary>
    /// 是否在缓存中
    /// </summary>
    /// <param name="oAsset"></param>
    /// <returns></returns>
    public static bool InCacheAsset(UnityEngine.Object oAsset)
    {
        return mObjectToCacheMaps.ContainsKey(oAsset);
    }


    /// <summary>
    /// 清除镜像资源的引用
    /// </summary>
    /// <param name="oAsset"></param>
    /// <returns></returns>
    public static bool PopCache(UnityEngine.Object oAsset)
    {
        if (oAsset == null)
            return false;

#if UNITY_EDITOR
        if (oAsset is AnimationClip)
        {
            LogSystem.LogWarning("PopCache:接口调用错误!!!");
        }
#endif

        //1.判断是否在缓存中，不在跳出
        CacheInfo cacheInfo = null;
        if (!mObjectToCacheMaps.TryGetValue(oAsset, out cacheInfo))
        {
#if UNITY_EDITOR
            //LogSystem.LogWarning("PopCache: oAsset not be found!!  path:", oAsset.name, "  ", oAsset.GetType());
#endif
            return false;
        }

        //2.检查接口是否正确调用
        //if (cacheInfo.bGameObject)
        //{
        //    GameObject oPrefab = oAsset as GameObject;
        //    if (spawnPool.IsPrefabUsing(oPrefab.transform))
        //    {
        //        LogSystem.LogWarning("CacheObject::PopCache Error 接口调用错误 " + cacheInfo.strFileName);
        //        return false;
        //    }
        //}
        if (cacheInfo.iUseTimes > 1)
        {
            cacheInfo.iUseTimes--;
        }
        else
        {
            RemoveCacheInfo(cacheInfo);
        }
        return true;
    }

    /// <summary>
    /// 移除动作
    /// </summary>
    /// <param name="aClip"></param>
    /// <returns></returns>
    public static bool PopCacheAnimation(AnimationClip aClip)
    {
        CacheInfo cacheInfo = null;
        if (aClip == null || !mObjectToCacheMaps.TryGetValue(aClip, out cacheInfo))
        {
            string name = "clip is null";
            if (aClip != null)
            {
                name = aClip.name;
            }
            LogSystem.LogWarning("CacheObjects::PopAnimation Error,找不到动作或为null clipName:", name);
            return false;
        }
        //Debug.Log("动作 删除!!!");
        if (cacheInfo.iUseTimes > 1)
        {
            cacheInfo.iUseTimes--;
        }
        else
        {
            RemoveCacheInfo(cacheInfo);
            Resources.UnloadAsset(aClip);
        }
        return true;
    }

    /// <summary>
    /// 移除缓存,不参于计算引用次数
    /// 目前，提供给字体移除
    /// </summary>
    /// <param name="strName"></param>
    public static void RemoveCacheByPrefab(string strName)
    {
        CacheInfo cInfo = GetCacheObject(strName);
        RemoveCacheInfo(cInfo);
    }

    /// <summary>
    /// 移除镜像文件，不参于计算引用次数
    /// 只能缓存池(CacheObjects)内部调用,逻辑层不能调用
    /// </summary>
    /// <param name="oAsset"></param>
    public static void RemoveCacheByPrefab(GameObject oAsset)
    {
        if (oAsset == null) 
            return;

        CacheInfo cacheInfo = null;
        if (!mObjectToCacheMaps.TryGetValue(oAsset, out cacheInfo))
            return;

        if (cacheInfo == null)
            return;

        RemoveCacheInfo(cacheInfo);
    }

    /// <summary>
    /// 首次加入缓存
    /// </summary>
    /// <param name="strName"></param>
    /// <param name="oAsset"></param>
    /// <returns></returns>
    private static bool FirstAddCacheInfo(string strName, UnityEngine.Object oAsset)
    {
#if DEV
        string strNameLower = strName.ToLower();
        if (mAssetLoaded.Contains(strNameLower))
        {
            LogSystem.LogWarning("FirstAddCacheInfo:", strName);
        }
        else
        {
            mAssetLoaded.Add(strNameLower);
        }
#endif
        CacheInfo cacheInfo = new CacheInfo();
        cacheInfo.iUseTimes = 1;
#if DEV
        cacheInfo.fCreateTime = Time.realtimeSinceStartup;
#endif
        cacheInfo.strFileName = strName;
        cacheInfo.oAsset = oAsset;
        if (!mNameToCacheMaps.ContainsKey(strName))
        {
            mNameToCacheMaps.Add(strName, cacheInfo);
        }
        if (!mObjectToCacheMaps.ContainsKey(oAsset))
        {
            mObjectToCacheMaps.Add(oAsset, cacheInfo);
        }
        return true;
    }

    /// <summary>
    /// 移除cacheInfo
    /// </summary>
    /// <param name="cacheInfo"></param>
    /// <returns></returns>
    private static bool RemoveCacheInfo(CacheInfo cacheInfo)
    {
        mNameToCacheMaps.Remove(cacheInfo.strFileName);
#if DEV
        mAssetLoaded.Remove(cacheInfo.strFileName.ToLower());
#endif
        mObjectToCacheMaps.Remove(cacheInfo.oAsset);

        if (onremoveObject != null)
        {
            onremoveObject(cacheInfo.oAsset);
        }
        return true;
    }

    /// <summary>
    /// 清空所有缓存
    /// </summary>
    public static void ClearAllCache()
    {
        mForeverCacheMaps.Clear();
#if DEV
        mAssetLoaded.Clear();
#endif
        mNameToCacheMaps.Clear();
        mObjectToCacheMaps.Clear();
    }

    /// <summary>
    /// 打印(debug用)
    /// </summary>
    public static void Print()
    {
        LogSystem.LogWarning("=========================Cacheobjects Total Info Start===========================");
        LogSystem.LogWarning("mNameToCacheMaps Count :", mNameToCacheMaps.Count);
        List<CacheInfo> list = new List<CacheInfo>(mNameToCacheMaps.Values);
        list.Sort(Comparison);
        CacheInfo cInfo = null;
        for (int i = 0; i < list.Count; i++ )
        {
            cInfo = list[i];
#if DEV
            LogSystem.LogWarning("Asset -> ", cInfo.strFileName, " Name:", cInfo.oAsset.name, "  UseCount:", cInfo.iUseTimes, "  CreateTime:", cInfo.fCreateTime, "  UsedTime:", cInfo.fUsedTime);
#else
            LogSystem.LogWarning("Asset -> ",cInfo.strFileName, "  Name:",cInfo.oAsset.name, "  UseCount:",cInfo.iUseTimes);
#endif

        }
        spawnPool.PrintLog();
    }

    private static int Comparison(CacheInfo cInfo1, CacheInfo cInfo2)
    {
#if DEV
        return (int)(cInfo1.fCreateTime - cInfo2.fCreateTime);
#endif
        return 0;
    }
}
