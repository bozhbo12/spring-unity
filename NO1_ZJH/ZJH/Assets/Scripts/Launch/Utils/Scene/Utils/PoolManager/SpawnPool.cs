/// <Licensing>
/// ?2011 (Copyright) Path-o-logical Games, LLC
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <description>
/// Online Docs: 
///     http://docs.poolmanager2.path-o-logical.com/code-reference/spawnpool
///     
///	A special List class that manages object pools and keeps the scene 
///	organized.
///	
///  * Only active/spawned instances are iterable. Inactive/despawned
///    instances in the pool are kept internally only.
/// 
///	 * Instanciated objects can optionally be made a child of this GameObject
///	   (reffered to as a 'group') to keep the scene hierachy organized.
///		 
///	 * Instances will get a number appended to the end of their name. E.g. 
///	   an "Enemy" prefab will be called "Enemy(Clone)001", "Enemy(Clone)002", 
///	   "Enemy(Clone)003", etc. Unity names all clones the same which can be
///	   confusing to work with.
///		   
///	 * Objects aren't destroyed by the Despawn() method. Instead, they are
///	   deactivated and stored to an internal queue to be used again. This
///	   avoids the time it takes unity to destroy gameobjects and helps  
///	   performance by reusing GameObjects. 
///		   
///  * Two events are implimented to enable objects to handle their own reset needs. 
///    Both are optional.
///      1) When objects are despawned BroadcastMessage("OnDespawned()") is sent.
///		 2) When reactivated, a BroadcastMessage("OnRespawned()") is sent. 
///		    This 
/// </description>
[AddComponentMenu("Path-o-logical/PoolManager/SpawnPool")]
public sealed class SpawnPool : MonoBehaviour, IList<Transform>
{
    public const string strUntagged = "Untagged";

    #region Inspector Parameters
    /// <summary>
    /// Returns the name of this pool used by PoolManager. This will always be the
    /// same as the name in Unity, unless the name contains the work "Pool", which
    /// PoolManager will strip out. This is done so you can name a prefab or
    /// GameObject in a way that is development friendly. For example, "EnemiesPool" 
    /// is easier to understand than just "Enemies" when looking through a project.
    /// </summary>
    public string poolName = "";

    /// <summary>
    /// Matches new instances to the SpawnPool GameObject's scale.
    /// </summary>
    public bool matchPoolScale = false;

    /// <summary>
    /// Matches new instances to the SpawnPool GameObject's layer.
    /// </summary>
    public bool matchPoolLayer = false;

    /// <summary>
    /// If true, this GameObject will be set to Unity's Object.DontDestroyOnLoad()
    /// </summary>
    public bool dontDestroyOnLoad = false;

    /// <summary>
    /// Print information to the Unity Console
    /// </summary>
    public bool logMessages = false;

    /// <summary>
    /// A list of PreloadDef options objects used by the inspector for user input
    /// </summary>
    public List<PrefabPool> _perPrefabPoolOptions = new List<PrefabPool>(16);

    /// <summary>
    /// Used by the inspector to store this instances foldout states.
    /// </summary>
    public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();
    #endregion Inspector Parameters

    #region Public Code-only Parameters
    /// <summary>
    /// The time in seconds to stop waiting for particles to die.
    /// A warning will be logged if this is triggered.
    /// </summary>
    [HideInInspector]
    public float maxParticleDespawnTime = 60f;

    /// <summary>
    /// 实例化父Trans
    /// The group is an empty game object which will be the parent of all
    /// instances in the pool. This helps keep the scene easy to work with.
    /// </summary>
    public Transform group { get; private set; }

    /// <summary>
    /// 克隆父Trans
    /// </summary>
    public Transform cloneGroup { get; private set; }
    /// <summary>
    /// Returns the prefab of the given name (dictionary key)
    /// </summary>


    // Keeps the state of each individual foldout item during the editor session
    public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

    /// <summary>
    /// Readonly access to prefab pools via a dictionary<string, PrefabPool>.
    /// </summary>
    public Dictionary<string, PrefabPool> prefabPools
    {
        get
        {
            var dict = new Dictionary<string, PrefabPool>();
            for (int i = 0; i < _prefabPools.Count; i++)
            {
                PrefabPool pool = this._prefabPools[i];
                dict[pool.prefabGO.name] = pool;
            }

            return dict;
        }
    }
    #endregion Public Code-only Parameters

    #region Private Properties

    private List<PrefabPool> _prefabPools = new List<PrefabPool>(16);

    /// <summary>
    /// 存储缓存池的预制件
    /// </summary>
    private List<Transform> mPrefabAssets = new List<Transform>(16);

    /// <summary>
    /// 所有的显示列表　主要用于快速判断是否在显示列表
    /// </summary>
    internal List<Transform> spawned = new List<Transform>(16);

    /// <summary>
    /// 所有显示对象
    /// </summary>
    internal Dictionary<Transform, PrefabPool> mSpawnedObjectsToPools = new Dictionary<Transform, PrefabPool>();

    /// <summary>
    /// 所对缓存对象
    /// </summary>
    internal Dictionary<Transform, PrefabPool> mAllObjectsToPools = new Dictionary<Transform, PrefabPool>();

    #endregion Private Properties

    public void PrintLog()
    {
        LogSystem.LogWarning("=========================Spawn Pool Total Info Start===========================");
        LogSystem.LogWarning("Prefab Count :", _prefabPools.Count);
        for (int i = 0; i < _prefabPools.Count; i++)
        {
            PrefabPool prefabPool = _prefabPools[i];
            LogSystem.LogWarning("Prefab Index:", i, " Name:", prefabPool.prefab.name, " SpawnCount:", prefabPool.spawned.Count, " DespawnCount:", prefabPool.despawned.Count);
        }
        LogSystem.LogWarning("----------------------------------------------------");
        LogSystem.LogWarning("Clone Count :", mClonePoolList.Count);
        for (int i = 0; i < mClonePoolList.Count; i++)
        {
            ClonePool clonePool = mClonePoolList[i];
            if (clonePool.oClone == null)
                continue;

            LogSystem.LogWarning("Clone Index:", i, " Name:", clonePool.oClone.name, " SpawnCount:", clonePool.spawned.Count, " DespawnCount:", clonePool.despawned.Count);
        }
        LogSystem.LogWarning("----------------------------------------------------");
        LogSystem.LogWarning("Cache Count :", mCachePoolList.Count);
        for (int i = 0; i < mCachePoolList.Count; i++)
        {
            CachePool cachePool = mCachePoolList[i];
            LogSystem.LogWarning("cache Index:", i, " Name:", cachePool.oType.ToString(), " SpawnCount:", cachePool.spawned.Count, " DespawnCount:", cachePool.despawned.Count);
        }
        LogSystem.LogWarning("=========================Spawn Pool Total Info End===========================");
    }
        
    #region Constructor and Init

    private void Awake()
    {
        miEffectLayer =  LayerMask.NameToLayer("Effect");
        miUIEffectLayer =  LayerMask.NameToLayer("UIEffect");
        miCreateRoleLayer =  LayerMask.NameToLayer("CreateRole");

        // Make this GameObject immortal if the user requests it.
        if (this.dontDestroyOnLoad) Object.DontDestroyOnLoad(this.gameObject);
        CreateRoot();
        // Default name behavior will use the GameObject's name without "Pool" (if found)
        if (this.poolName == "")
        {
            // Automatically Remove "Pool" from names to allow users to name prefabs in a 
            //   more development-friendly way. E.g. "EnemiesPool" becomes just "Enemies".
            //   Notes: This will return the original string if "Pool" isn't found.
            //          Do this once here, rather than a getter, to avoide string work
            this.poolName = this.group.name.Replace("Pool", "");
            this.poolName = this.poolName.Replace("(Clone)", "");
        }


        if (this.logMessages)
            LogSystem.Log(string.Format("SpawnPool {0}: Initializing..", this.poolName));

        // Only used on items defined in the Inspector
        for (int i = 0; i < _perPrefabPoolOptions.Count; i++)
        {
            PrefabPool prefabPool = this._perPrefabPoolOptions[i];
            if (prefabPool.prefab == null)
            {
                if (this.logMessages)
                    LogSystem.LogWarning(string.Format("Initialization Warning: Pool '{0}' " +
                          "contains a PrefabPool with no prefab reference. Skipping.",
                           this.poolName));
                continue;
            }

            // Init the PrefabPool's GameObject cache because it can't do it.
            //   This is only needed when created by the inspector because the constructor
            //   won't be run.
            prefabPool.inspectorInstanceConstructor();
            this.CreatePrefabPool(prefabPool);
        }

        // Add this SpawnPool to PoolManager for use. This is done last to lower the 
        //   possibility of adding a badly init pool.
        CachePoolManager.Pools.Add(this);
    }

    private bool mbInit = false;
    public void CreateRoot()
    {
        if (!mbInit)
        {
            mbInit = true;
            GameObject oPrefabRoot = new GameObject("ObjectsPool");
            oPrefabRoot.transform.parent = transform;
            this.group = oPrefabRoot.transform;
            oPrefabRoot.SetActive(false);

            GameObject oCloneRoot = new GameObject("ClonesPool");
            oCloneRoot.transform.parent = transform;
            this.cloneGroup = oCloneRoot.transform;
            oCloneRoot.SetActive(false);
        }
    }

    /// <summary>
    /// Runs when this group GameObject is destroyed and executes clean-up
    /// </summary>
    private void OnDestroy()
    {
        DestroyPool();
    }

    public void DestroyPool()
    {
        if (this.logMessages)
            LogSystem.Log(string.Format("SpawnPool {0}: Destroying...", this.poolName));

        CachePoolManager.Pools.Remove(this);

        this.StopAllCoroutines();

        // We don't need the references to spawns which are about to be destroyed
        this.spawned.Clear();
        mSpawnedObjectsToPools.Clear();
        mAllObjectsToPools.Clear();
        this.mSpawnClones.Clear();
        // Clean-up
        for (int i = 0; i < _prefabPools.Count; i++)
        {
            PrefabPool pool = this._prefabPools[i];
            pool.SelfDestruct();
        }

        for (int i = 0; i < mClonePoolList.Count; i++)
        {
            ClonePool pool = this.mClonePoolList[i];
            pool.SelfDestruct();
        }
        for (int i = 0; i < mCachePoolList.Count; i++)
        {
            CachePool pool = this.mCachePoolList[i];
            pool.SelfDestruct();
        }

        this.mClonePoolList.Clear();

        // Probably overkill, and may not do anything at all, but...
        this._prefabPools.Clear();
        this.mPrefabAssets.Clear();
        this.mCachePoolList.Clear();

        Resources.UnloadUnusedAssets();
        System.GC.Collect();
        //this.prefabs._Clear();
    }


    /// <summary>
    /// Creates a new PrefabPool in this Pool and instances the requested 
    /// number of instances (set by PrefabPool.preloadAmount). If preload 
    /// amount is 0, nothing will be spawned and the return list will be empty.
    /// 
    /// It is rare this function is needed during regular usage.
    /// This function should only be used if you need to set the preferences
    /// of a new PrefabPool, such as culling or pre-loading, before use. Otherwise, 
    /// just use Spawn() and if the prefab is used for the first time a PrefabPool 
    /// will automatically be created with defaults anyway.
    /// 
    /// Note: Instances with ParticleEmitters can be preloaded too because 
    ///       it won't trigger the emmiter or the coroutine which waits for 
    ///       particles to die, which Spawn() does.
    ///       
    /// Usage Example:
    ///     // Creates a prefab pool and sets culling options but doesn't
    ///     //   need to spawn any instances (this is fine)
    ///     PrefabPool prefabPool = new PrefabPool()
    ///     prefabPool.prefab = myPrefabReference;
    ///     prefabPool.preloadAmount = 0;
    ///     prefabPool.cullDespawned = True;
    ///     prefabPool.cullAbove = 50;
    ///     prefabPool.cullDelay = 30;
    ///     
    ///     // Enemies is just an example. Any pool is fine.
    ///     PoolManager.Pools["Enemies"].CreatePrefabPool(prefabPool);
    ///     
    ///     // Then, just use as normal...
    ///     PoolManager.Pools["Enemies"].Spawn(myPrefabReference);
    /// </summary>
    /// <param name="prefabPool">A PrefabPool object</param>
    /// <returns>A List of instances spawned or an empty List</returns>
    public void CreatePrefabPool(PrefabPool prefabPool)
    {
        // Only add a PrefabPool once. Uses a GameObject comparison on the prefabs
        //   This will rarely be needed and will almost Always run at game start, 
        //   even if user-executed. This really only fails If a user tries to create 
        //   a PrefabPool using a prefab which already has a PrefabPool in the same
        //   SpawnPool. Either user created twice or PoolManager went first or even 
        //   second in cases where a user-script beats out PoolManager's init during 
        //   Awake();
        bool isAlreadyPool = this.GetPrefab(prefabPool.prefab) == null ? false : true;
        if (!isAlreadyPool)
        {
            // Used internally to reference back to this spawnPool for things 
            //   like anchoring co-routines.
            prefabPool.spawnPool = this;

            this._prefabPools.Add(prefabPool);
            mPrefabAssets.Add(prefabPool.prefab);
            // Add to the prefabs dict for convenience
            //this.prefabs._Add(prefabPool.prefab.name, prefabPool.prefab);
        }

        // Preloading (uses a singleton bool to be sure this is only done once)
        //if (prefabPool.preloaded != true)
        //{
        //    if (this.logMessages)
        //        LogSystem.Log(string.Format("SpawnPool {0}: Preloading {1} {2}",
        //                                   this.poolName,
        //                                   prefabPool.preloadAmount,
        //                                   prefabPool.prefab.name));

        //    prefabPool.PreloadInstances();
        //}

        if (Application.isPlaying)
        {
            DontDestroyOnLoad(this);
        }
    }

    #endregion Constructor and Init

    #region 预制件缓存
    /// <description>
    ///	Spawns an instance or creates a new instance if none are available.
    ///	Either way, an instance will be set to the passed position and 
    ///	rotation.
    /// 
    /// Detailed Information:
    /// Checks the Data structure for an instance that was already created
    /// using the prefab. If the prefab has been used before, such as by
    /// setting it in the Unity Editor to preload instances, or just used
    /// before via this function, one of its instances will be used if one
    /// is available, or a new one will be created.
    /// 
    /// If the prefab has never been used a new PrefabPool will be started 
    /// with default options. 
    /// 
    /// To alter the options on a prefab pool, use the Unity Editor or see
    /// the documentation for the PrefabPool class and 
    /// SpawnPool.SpawnPrefabPool()
    ///		
    /// Broadcasts "OnSpawned" to the instance. Use this instead of Awake()
    ///		
    /// This function has the same initial signature as Unity's InstantiatePrefab() 
    /// that takes position and rotation. The return Type is different though.
    /// </description>
    /// <param name="prefab">
    /// The prefab used to spawn an instance. Only used for reference if an 
    /// instance is already in the pool and available for respawn. 
    /// NOTE: Type = Transform
    /// </param>
    /// <param name="pos">The position to set the instance to</param>
    /// <param name="rot">The rotation to set the instance to</param>
    /// <returns>
    /// The instance's Transform. 
    /// 
    /// If the Limit option was used for the PrefabPool associated with the
    /// passed prefab, then this method will return null if the limit is
    /// reached. You DO NOT need to test for null return values unless you 
    /// used the limit option.
    /// </returns>
    ///
#if DEBUGPOOL
    public void CheckPrefab(Transform prefab)
    {
        ///检查预制件是不是Object对象
        for (int i = 0; i < _prefabPools.Count; i++)
        {
            PrefabPool prefabPool = this._prefabPools[i];
            if (prefabPool.spawned.Contains(prefab) || prefabPool.despawned.Contains(prefab))
            {
                LogSystem.LogWarning("Spawn an object as Prefab,please Check!", prefab.name);
            }
            else
            {
                ///检查是不是这些激活对象的子对象
                for (int k = 0; k < prefabPool.spawned.Count; k++)
                {
                    Transform trans = prefabPool.spawned[k];
                    if (trans == null)
                        continue;

                    if (prefab.IsChildOf(trans))
                    {
                        LogSystem.LogWarning("Spawn an object as Prefab Spawn child,please Check!", prefab.name);
                        break;
                    }
                }
                ///检查是不是这些非激活对象的子对象
                for (int k = 0; k < prefabPool.despawned.Count; k++)
                {
                    Transform trans = prefabPool.despawned[k];
                    if (trans == null)
                        continue;

                    if (prefab.IsChildOf(trans))
                    {
                        LogSystem.LogWarning("Spawn an object as Prefab Despawn child,please Check!", prefab.name);
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < mClonePoolList.Count; i++)
        {
            ClonePool clonePool = this.mClonePoolList[i];
            if (clonePool.spawned.Contains(prefab) || clonePool.despawned.Contains(prefab))
            {
                LogSystem.LogWarning("Spawn an object In Clone,please Check!", prefab.name);
            }
            else
            {
                ///检查是不是这些激活对象的子对象
                for (int k = 0; k < clonePool.spawned.Count; k++)
                {
                    Transform trans = clonePool.spawned[k];
                    if (trans == null)
                        continue;

                    if (prefab.IsChildOf(trans))
                    {
                        LogSystem.LogWarning("Spawn an object as Clone Spawn child,please Check!", prefab.name);
                        break;
                    }
                }
                ///检查是不是这些非激活对象的子对象
                for (int k = 0; k < clonePool.despawned.Count; k++)
                {
                    Transform trans = clonePool.despawned[k];
                    if (trans == null)
                        continue;

                    if (prefab.IsChildOf(trans))
                    {
                        LogSystem.LogWarning("Spawn an object as Clone Despawn child,please Check!", prefab.name);
                        break;
                    }
                }
            }
        }
    }
#endif

    public bool checkGameObjectInPool(Transform prefab)
    {
        bool beExist = false;
        for (int i = 0; i < _prefabPools.Count; i++)
        {
            PrefabPool prefabPool = this._prefabPools[i];
            if (prefabPool.prefabGO == prefab.gameObject)
            {
                beExist = true;
                break;
            }
        }

        return beExist;
    }

    public Transform Spawn(Transform prefab, Vector3 pos, Quaternion rot)
    {
        Transform inst;
        #region Use from Pool
        for (int i = 0; i < _prefabPools.Count; i++)
        {
            PrefabPool prefabPool = this._prefabPools[i];
            // Determine if the prefab was ever used as explained in the docs
            //   I believe a comparison of two references is processor-cheap.
            if (prefabPool.prefabGO == prefab.gameObject)
            {
                // Now we know the prefabPool for this prefab exists. 
                // Ask the prefab pool to setup and activate an instance.
                // If there is no instance to spawn, a new one is instanced
                inst = prefabPool.SpawnInstance(pos, rot);

                // This only happens if the limit option was used for this
                //   Prefab Pool.
                if (inst == null)
                    return null;

                //修改:新创建出的GameObject的Transform不设置
                //如果设置会被调用OnDisabled
                // If a new instance was created, it won't be grouped
                //if (inst.parent != this.group)
                //    inst.parent = this.group;

                // Add to internal list - holds only active instances in the pool
                // 	 This isn't needed for Pool functionality. It is just done 
                //	 as a user-friendly feature which has been needed before.
                this.spawned.Add(inst);
                if (!mSpawnedObjectsToPools.ContainsKey(inst))
                {
                    mSpawnedObjectsToPools.Add(inst, prefabPool);
                }

                if (!mAllObjectsToPools.ContainsKey(inst))
                {
                    mAllObjectsToPools.Add(inst, prefabPool);
                }
#if DEBUGPOOL
                Transform[] trans = inst.GetComponentsInChildren<Transform>(true);
                for (int k = 0; k< trans.Length;k++)
                {
                    CheckObject(trans[k]);
                }
#endif
                inst.localScale = prefab.localScale;
                // Done!
                return inst;
            }
        }
        #endregion Use from Pool
#if DEBUGPOOL
        CheckPrefab(prefab);
#endif
        #region New PrefabPool
        // The prefab wasn't found in any PrefabPools above. Make a new one
        PrefabPool newPrefabPool = new PrefabPool(prefab);
        this.CreatePrefabPool(newPrefabPool);

        // Spawn the new instance (Note: prefab already set in PrefabPool)
        inst = newPrefabPool.SpawnInstance(pos, rot);
        //inst.parent = this.group;  // Add to this parent group

        // New instances are active and must be added to the internal list 
        this.spawned.Add(inst);
        if (!mSpawnedObjectsToPools.ContainsKey(inst))
        {
            mSpawnedObjectsToPools.Add(inst, newPrefabPool);
        }

        if (!mAllObjectsToPools.ContainsKey(inst))
        {
            mAllObjectsToPools.Add(inst, newPrefabPool);
        }
        #endregion New PrefabPool


        return inst;
    }



    /// <summary>
    /// See primary Spawn method for documentation.
    /// 
    /// Overload to take only a prefab and instance using an 'empty' 
    /// position and rotation.
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns>
    /// The instance's Transform. 
    /// 
    /// If the Limit option was used for the PrefabPool associated with the
    /// passed prefab, then this method will return null if the limit is
    /// reached. You DO NOT need to test for null return values unless you 
    /// used the limit option.
    /// </returns>
    public Transform Spawn(Transform prefab)
    {
        return this.Spawn(prefab, Vector3.zero, Quaternion.identity);
    }



    /// <summary>
    /// 检查此预制件是否真正使用
    /// </summary>
    /// <param name="oAsset"></param>
    /// <returns></returns>
    public bool IsPrefabUsing(Transform oAsset)
    {
        if (mPrefabAssets.Contains(oAsset))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 是否在显示状态
    /// </summary>
    /// <param name="xform"></param>
    public bool IsSpawnObject(Transform xform)
    {
        if (mSpawnedObjectsToPools.ContainsKey(xform))
            return true;

        return false;
    }

    public bool IsPoolObject(Transform xform)
    {
        if (xform == null)
            return false;

        if (mAllObjectsToPools.ContainsKey(xform))
        {
            return true;
        }

        return false;
    }

    /// <description>
    ///	If the passed object is managed by the SpawnPool, it will be 
    ///	deactivated and made available to be spawned again.
    ///		
    /// despawned instances are removed from the primary list.
    /// 删除预制件物体
    /// </description>
    /// <param name="item">The transform of the gameobject to process</param>
    public void Despawn(Transform xform, bool bOnlyClear = false)
    {
        if (xform == null)
            return;

        PrefabPool prefabPool = null;
        if (mSpawnedObjectsToPools.TryGetValue(xform, out prefabPool) && prefabPool != null)
        {
            //xform.parent = null;
            bool despawned = prefabPool.DespawnInstance(xform, bOnlyClear);
            // If still false, then the instance wasn't found anywhere in the pool
            if (!despawned)
            {
                if (this.logMessages)
                    LogSystem.LogWarning(string.Format("SpawnPool {0}: {1} not found in SpawnPool",
                               this.poolName,
                               xform.name));
            }
        }

        // Remove from the internal list. Only active instances are kept. 
        // 	 This isn't needed for Pool functionality. It is just done 
        //	 as a user-friendly feature which has been needed before.
        this.spawned.Remove(xform);
        mSpawnedObjectsToPools.Remove(xform);
    }



    /// <description>
    ///	Returns true if the passed transform is currently spawned.
    /// </description>
    /// <param name="item">The transform of the gameobject to test</param>
    public bool IsSpawned(Transform instance)
    {
        return this.spawned.Contains(instance);
    }

    #endregion 预制件缓存

    #region 克隆缓存
    /// <summary>
    /// 克隆对象池列表
    /// </summary>
    private List<ClonePool> mClonePoolList = new List<ClonePool>(16);
    public List<Transform> mSpawnClones = new List<Transform>(16);
    public bool IsSpawnClone(Transform xform)
    {
        if (mSpawnClones.Contains(xform))
        {
            return true;
        }
        return false;
    }

#if DEBUGPOOL 
    void CheckObject( Transform oTrans)
    {
        ///检查预制件是不是Object对象
        for (int i = 0; i < _prefabPools.Count; i++)
        {
            PrefabPool prefabPool = this._prefabPools[i];
            if (prefabPool.spawned.Contains(oTrans) || prefabPool.despawned.Contains(oTrans))
            {
                LogSystem.LogWarning("has a Prefab object,please Check!", oTrans.name);
            }
            else
            {
                for (int k = 0; k < prefabPool.spawned.Count; k++)
                {
                    Transform transParent = prefabPool.spawned[k];
                    if (transParent == null)
                        continue;
                    if (oTrans.IsChildOf(transParent))
                    {
                        LogSystem.LogWarning("has an child in spawned object of prefab,please Check!", oTrans.name);
                        break;
                    }
                }
                for (int k = 0; k < prefabPool.despawned.Count; k++)
                {
                    Transform transParent = prefabPool.despawned[k];
                    if (transParent == null)
                        continue;
                    if (oTrans.IsChildOf(transParent))
                    {
                        LogSystem.LogWarning("has an child in despawned object of prefab ,please Check!", oTrans.name);
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < mClonePoolList.Count; i++)
        {
            ClonePool clonePool2 = this.mClonePoolList[i];
            if (clonePool2.spawned.Contains(oTrans) || clonePool2.despawned.Contains(oTrans))
            {
                LogSystem.LogWarning("Clone an object In CloneList,please Check!", oTrans.name);
            }
            else
            {
                for (int k = 0; k < clonePool2.spawned.Count; k++)
                {
                    Transform transParent = clonePool2.spawned[k];
                    if (transParent == null)
                        continue;
                    if (oTrans.IsChildOf(transParent))
                    {
                        LogSystem.LogWarning("Clone an object child of spawned,please Check!", oTrans.name);
                        break;
                    }
                }
                for (int k = 0; k < clonePool2.despawned.Count; k++)
                {
                    Transform transParent = clonePool2.despawned[k];
                    if (transParent == null)
                        continue;
                    if (oTrans.IsChildOf(transParent))
                    {
                        LogSystem.LogWarning("Clone an object  child of Clone despawned,please Check!", oTrans.name);
                        break;
                    }
                }
            }
        }
    }
#endif

    /// <summary>
    /// 从克隆表中实例化对象
    /// </summary>
    /// <param name="strObject"></param>
    public Transform Clone(Transform oTrans)
    {
        if (oTrans == null)
            return null;

        for (int i = 0; i < mClonePoolList.Count; i++)
        {
            ClonePool clonepool = mClonePoolList[i];

            if (clonepool.oClone == oTrans)
            {
                return clonepool.SpawnInstance();
            }
        }

#if DEBUGPOOL
        Transform[] trans = oTrans.GetComponentsInChildren<Transform>();
        for (int k = 0;k< trans.Length;k++)
        {
            CheckObject(trans[k]);
        }
#endif
        ClonePool clonePool = new ClonePool();
        clonePool.oClone = oTrans;
        clonePool.mSpawnPool = this;
        mClonePoolList.Add(clonePool);

        return clonePool.SpawnInstance();
    }

    /// <summary>
    /// 从克隆表中实例化对象
    /// </summary>
    /// <param name="strObject"></param>
    public void DespawnClone(Transform oTrans)
    {
        if (oTrans == null)
            return;

        for (int i = 0; i < mClonePoolList.Count; i++)
        {
            ClonePool clonepool = mClonePoolList[i];
            if (clonepool.spawned.Contains(oTrans))
            {
                clonepool.DespawnInstance(oTrans);
                return;
            }
            else if (clonepool.despawned.Contains(oTrans))
            {
                if (this.logMessages)
                    LogSystem.LogWarning(string.Format("ClonePool {0}: {1} has already been despawned. " +
                                   "You cannot despawn something more than once!",
                                    this.poolName,
                                    oTrans.name));
                return;
            }
        }

        mSpawnClones.Remove(oTrans);
#if DEBUGPOOL
        ///没有删除到资源的时候走到这里，提示删除失败
        LogSystem.LogWarning("Missing Destroy Clone Object", oTrans.name);
#endif
    }

    #endregion 克隆缓存

    #region 脚本(对象)缓存池

    /// <summary>
    /// 对象缓存池
    /// </summary>
    private List<CachePool> mCachePoolList = new List<CachePool>(16);

    /// <summary>
    /// 获得对象池中的数据
    /// </summary>
    /// <returns></returns>
    public T SpawnCache<T>() where T : CacheObject
    {
        System.Type type = typeof(T);
        for (int i = 0; i < mCachePoolList.Count; i++)
        {
            CachePool cache = mCachePoolList[i];
            if (cache.oType == type)
            {
                return cache.SpawnCache<T>();
            }
        }

        CachePool cachePool = new CachePool();
        cachePool.oType = type;
        mCachePoolList.Add(cachePool);

        return cachePool.SpawnCache<T>();
    }

    /// <summary>
    /// 获得对象池中的数据
    /// </summary>
    /// <returns></returns>
    public void DespawnCache(CacheObject oCache)
    {
        for (int i = 0; i < mCachePoolList.Count; i++)
        {
            CachePool cache = mCachePoolList[i];
            if (cache.spawned.Contains(oCache))
            {
                cache.DespawnCache(oCache);
                break;
            }
            else if (cache.despawned.Contains(oCache))
            {
                LogSystem.LogWarning("SpawnCache in despawned list", oCache.ToString());
            }
        }
    }

    #endregion

    #region 获取对象的预制件

    /// <summary>
    /// 获取当前对象的预制件
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public Transform GetObjectPrefab(Transform trans)
    {
        if (trans == null)
            return null;

        PrefabPool prefabPool = null;
        if (mSpawnedObjectsToPools.TryGetValue(trans, out prefabPool) && prefabPool != null)
        {
            return prefabPool.prefab;
        }

        return null;
    }

    /// <summary>
    /// 获取当前对象的预制件
    /// </summary>
    /// <param name="trans"></param>
    /// <returns></returns>
    public Transform GetObjectClone(Transform trans)
    {
        if (trans == null)
            return null;

        for (int i = 0; i < mClonePoolList.Count; i++)
        {
            ClonePool clonePool = mClonePoolList[i];
            if (clonePool == null)
                continue;
            if (clonePool.spawned.Contains(trans))
            {
                return clonePool.oClone;
            }
        }

        return null;
    }

    #endregion 获取对象的预制件

    #region 特效重置


    public static int miEffectLayer = -1; 
    public static int miUIEffectLayer = -1; 
    public static int miCreateRoleLayer = -1; 
    public static void CheckEffectWatcher(GameObject oEffect)
    {
        if (oEffect == null)
            return;

        if (oEffect.layer == miEffectLayer || oEffect.layer == miUIEffectLayer)
        {
            EffectWatcher watcher = oEffect.GetComponent<EffectWatcher>();
            if (watcher == null)
            {
                watcher = oEffect.AddComponent<EffectWatcher>();
            }
            watcher.ResetEffect();
        }
    }

    /// <summary>
    /// 特效重置
    /// </summary>
    /// <param name="oEffect"></param>
    /// <param name="bForce">不判断层级</param>
    public static void ResetEffect(GameObject oEffect, bool bForce = false)
    {
        if (oEffect == null)
            return;

        if ( bForce || (oEffect.layer == miEffectLayer || oEffect.layer == miUIEffectLayer || oEffect.layer == miCreateRoleLayer))
        {
            {
                NcDuplicator item = oEffect.GetComponentInChildren<NcDuplicator>();
                if (item != null)
                {
                    LogSystem.Log(oEffect.name, " NcDuplicator cannot be replayed.");
                    return;
                }
            }

            {
                //优生设置延迟脚本
                //父节点使用ncDelay延迟,子节点也做了延迟时,统一调用重置接口会发生ncDelay延迟失效问题
                //原因是在统一调用接口中会重置子节点，这时子节点已经开始做计时，导致ncdelay失效
                //先调用ncdelay将它的子节点设为false，在ncdelay计时完成后再调用ResetAnimation;
                NcDelayActive[] list = oEffect.GetComponentsInChildren<NcDelayActive>(true);
                for (int i = 0; i < list.Length; i++)
                {
                    if (list[i] != null)
                        list[i].ResetAnimation();
                }
            }

            {
                IResetAnimation[] list = oEffect.GetComponentsInChildren<IResetAnimation>(true);
                for (int i = 0; i < list.Length; i++)
                {
                    //当父对象不是ncDelay调用重置
                    if (list[i] != null && !list[i].GetParentIsNcDelay())
                        list[i].ResetAnimation();
                }
            }

            {
                ParticleSystem[] list = oEffect.GetComponentsInChildren<ParticleSystem>(true);
                for (int i = 0; i < list.Length; i++)
                {
                    ParticleSystem pSystem = list[i];
                    if (pSystem != null)
                    {
                        pSystem.Stop();
                        pSystem.Clear();
                        pSystem.time = 0;
                        pSystem.Play();
                    }
                }
            }

            {
                Animation[] list = oEffect.GetComponentsInChildren<Animation>(true);
                for (int i = 0; i < list.Length; i++)
                {
                    Animation animation = list[i];
                    if (animation == null)
                        continue;
                    foreach (AnimationState anim in animation)
                    {
                        anim.time = 0;
                    }
                    animation.Play();
                }
            }
        }
    }

    #endregion 特效重置

    #region 删除超出时间回收资源


    void FixedUpdate()
    {
#if DEBUGPOOL
        if (Input.GetKeyDown(KeyCode.F4))
        {
            PrintLog();
        }
#endif
        long lTicks = System.DateTime.Now.Ticks;
        if (lTicks - mlLastTick < mlCheckTime)
            return;

        mlLastTick = lTicks;
        OptimizePool(lTicks, mlFreeTime);
    }

    long mlLastTick = 0;
    public long mlCheckTime = 50000000;
    public long mlFreeTime = 300000000;
    public long mlCloneFreeTime = 300000000;
    private int miCollectCount = 0;
    private int miCollectLast = 0;
    private float mfLastCollectTime = 0;
    private float mfMaxCollectTime = 60.0f;
    /// <summary>
    /// 优化池对象
    /// </summary>
    /// <param name="lTickNow"></param>
    public void OptimizePool(long lTickNow, long lFreeTime)
    {
        int iCollectCount = 0;
        int iCollectCount2 = 0;
        // 优化实例化池
        for (int i = 0; i < _prefabPools.Count; i++)
        {
            PrefabPool pool = this._prefabPools[i];
            if (pool == null)
                continue;

            pool.OptimizePool(lTickNow, lFreeTime);
            //当没有显示对象时删除
            if (pool.mbOnlyClear && pool.spawned.Count == 0)
            {
                pool.SelfDestruct();
                iCollectCount2++;
                _prefabPools.RemoveAt(i);
                mPrefabAssets.RemoveAt(i);
                i--;
                continue;
            }//显示\隐藏列表都为空时删除
            else if (pool.despawned.Count == 0 && pool.spawned.Count == 0)
            {
                pool.SelfDestruct();
                iCollectCount++;
                _prefabPools.RemoveAt(i);
                mPrefabAssets.RemoveAt(i);
                i--;
                continue;
            }

        }

        ///优化克隆池
        for (int i = 0; i < mClonePoolList.Count; i++)
        {
            ClonePool clonepool = mClonePoolList[i];
            clonepool.OptimizeClonePool(lTickNow, mlCloneFreeTime);
            if (clonepool.despawned.Count == 0 && clonepool.spawned.Count == 0)
            {
                clonepool.SelfDestruct();
                mClonePoolList.RemoveAt(i);
                i--;
                continue;
            }
        }
        ///优化克隆池
        for (int i = 0; i < mCachePoolList.Count; i++)
        {
            CachePool cachePool = mCachePoolList[i];
            cachePool.OptimizeCachePool(lTickNow, mlCloneFreeTime);
            if (cachePool.despawned.Count == 0 && cachePool.spawned.Count == 0)
            {
                cachePool.SelfDestruct();
                mCachePoolList.RemoveAt(i);
                i--;
                continue;
            }
        }

        ///如果有对象回收
        if (iCollectCount > 0 || iCollectCount2 > 0)
        {
            miCollectLast += (iCollectCount + iCollectCount2);
            float fTime = Time.realtimeSinceStartup;
            if (miCollectLast >= 10 && fTime - mfLastCollectTime > mfMaxCollectTime)
            {
                miCollectCount = miCollectCount + miCollectLast;
                miCollectLast = 0;
                mfLastCollectTime = fTime;
                ///预制件回收次数大于10次，清空一次未使用资源

                if (miCollectCount > 200)
                {
                    miCollectCount = 0;
                    System.GC.Collect();
                }

                Resources.UnloadUnusedAssets();
            }
            else if (iCollectCount2 > 0)
            {
                ///有立即释放资源的，手动释放
                Resources.UnloadUnusedAssets();
            }
        }
        //else
        //{
        //    float fTime = Time.realtimeSinceStartup;
        //    if (miCollectLast > 0 && fTime - mfLastCollectTime > mfMaxCollectTime*2)
        //    {
        //        Resources.UnloadUnusedAssets();
        //    }
        //}
    }

    /// <summary>
    /// 删除没有显示对象的缓存
    /// </summary>
    /// <returns></returns>
    public bool DestroyUnDisplayObject()
    {
        for (int i = _prefabPools.Count - 1; i >=0; i--)
        {
            PrefabPool pool = this._prefabPools[i];
            if (pool == null)
                continue;

            //当没有显示对象时删除
            if (pool.spawned.Count == 0)
            {
                pool.SelfDestruct();
                _prefabPools.RemoveAt(i);
                mPrefabAssets.RemoveAt(i);
            }
        }

        for (int i = mClonePoolList.Count - 1; i >= 0; i--)
        {
            ClonePool clonepool = mClonePoolList[i];
            if (clonepool == null)
                continue;

            if (clonepool.spawned.Count == 0)
            {
                clonepool.SelfDestruct();
                mClonePoolList.RemoveAt(i);
            }
        }

        for (int i = mCachePoolList.Count - 1; i >= 0; i--)
        {
            CachePool cachePool = mCachePoolList[i];
            if (cachePool == null)
                continue;

            if (cachePool.spawned.Count == 0)
            {
                cachePool.SelfDestruct();
                mCachePoolList.RemoveAt(i);
            }
        }
        return true;
    }

    #endregion 删除超出时间回收资源

    #region Utility Functions
    /// <summary>
    /// 获取
    /// Returns the prefab used to create the passed instance. 
    /// This is provided for convienince as Unity doesn't offer this feature.
    /// </summary>
    /// <param name="prefab">The Transform of an instance</param>
    /// <returns>Transform</returns>
    public Transform GetPrefab(Transform prefab)
    {
        for (int i = 0; i < _prefabPools.Count; i++)
        {
            PrefabPool prefabPool = this._prefabPools[i];
            if (prefabPool.prefabGO == null)
            {
                if (this.logMessages)
                    LogSystem.LogWarning(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null", this.poolName));
            }

            if (prefabPool.prefabGO == prefab.gameObject)
                return prefabPool.prefab;
        }

        // Nothing found
        return null;
    }


    /// <summary>
    /// Returns the prefab used to create the passed instance. 
    /// This is provided for convienince as Unity doesn't offer this feature.
    /// </summary>
    /// <param name="prefab">The GameObject of an instance</param>
    /// <returns>GameObject</returns>
    public GameObject GetPrefab(GameObject prefab)
    {
        for (int i = 0; i < _prefabPools.Count; i++)
        {
            PrefabPool prefabPool = this._prefabPools[i];
            if (prefabPool.prefabGO == null)
            {
                if (this.logMessages)
                    LogSystem.LogWarning(string.Format("SpawnPool {0}: PrefabPool.prefabGO is null",
                                             this.poolName));
            }


            if (prefabPool.prefabGO == prefab)
                return prefabPool.prefabGO;
        }

        // Nothing found
        return null;
    }


    #endregion Utility Functions

    #region Overrides
    /// <summary>
    /// Not Implimented. Use Spawn() to properly add items to the pool.
    /// This is required because the prefab needs to be stored in the internal
    /// data structure in order for the pool to function properly. Items can
    /// only be added by instencing them using SpawnInstance().
    /// </summary>
    /// <param name="item"></param>
    public void Add(Transform item)
    {
        string msg = "Use SpawnPool.Spawn() to properly add items to the pool.";
        throw new System.NotImplementedException(msg);
    }


    /// <summary>
    /// Not Implimented. Use Despawn() to properly manage items that should remain 
    /// in the Queue but be deactivated. There is currently no way to safetly
    /// remove items from the pool permentantly. Destroying Objects would
    /// defeat the purpose of the pool.
    /// </summary>
    /// <param name="item"></param>
    public void Remove(Transform item)
    {
        string msg = "Use Despawn() to properly manage items that should " +
                     "remain in the pool but be deactivated.";
        throw new System.NotImplementedException(msg);
    }

    /// <summary>
    /// Returns a formatted string showing all the spawned member names
    /// </summary>
    public override string ToString()
    {
        // Get a string[] array of the keys for formatting with join()
        List<string> name_list = new List<string>();
        for (int i = 0; i < name_list.Count; i++)
        {
            Transform item = this.spawned[i];
            name_list.Add(item.name);
        }

        // Return a comma-sperated list inside square brackets (Pythonesque)
        return System.String.Join(", ", name_list.ToArray());
    }


    /// <summary>
    /// Read-only index access. You can still modify the instance at the given index.
    /// Read-only reffers to setting an index to a new instance reference, which would
    /// change the list. Setting via index is never needed to work with index access.
    /// </summary>
    /// <param name="index">int address of the item to get</param>
    /// <returns></returns>
    public Transform this[int index]
    {
        get { return this.spawned[index]; }
        set { throw new System.NotImplementedException("Read-only."); }
    }

    /// <summary>
    /// The name "Contains" is misleading so IsSpawned was implimented instead.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(Transform item)
    {
        string message = "Use IsSpawned(Transform instance) instead.";
        throw new System.NotImplementedException(message);
    }


    /// <summary>
    /// Used by OTHERList.AddRange()
    /// This adds this list to the passed list
    /// </summary>
    /// <param name="array">The list AddRange is being called on</param>
    /// <param name="arrayIndex">
    /// The starting index for the copy operation. AddRange seems to pass the last index.
    /// </param>
    public void CopyTo(Transform[] array, int arrayIndex)
    {
        this.spawned.CopyTo(array, arrayIndex);
    }


    /// <summary>
    /// Returns the number of items in this (the collection). Readonly.
    /// </summary>
    public int Count
    {
        get { return this.spawned.Count; }
    }


    /// <summary>
    /// Impliments the ability to use this list in a foreach loop
    /// </summary>
    /// <returns></returns>
    public IEnumerator<Transform> GetEnumerator()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            Transform instance = this.spawned[i];
            yield return instance;
        }
    }

    /// <summary>
    /// Impliments the ability to use this list in a foreach loop
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            Transform instance = this.spawned[i];
            yield return instance;
        }
    }

    // Not implemented
    public int IndexOf(Transform item) { throw new System.NotImplementedException(); }
    public void Insert(int index, Transform item) { throw new System.NotImplementedException(); }
    public void RemoveAt(int index) { throw new System.NotImplementedException(); }
    public void Clear() { throw new System.NotImplementedException(); }
    public bool IsReadOnly { get { throw new System.NotImplementedException(); } }
    bool ICollection<Transform>.Remove(Transform item) { throw new System.NotImplementedException(); }

    #endregion Overrides

    #region 未用到接口

    /// <summary>
    /// Add an existing instance to this pool. This is used during game start 
    /// to pool objects which are not instanciated at runtime
    /// </summary>
    /// <param name="instance">The instance to add</param>
    /// <param name="prefabName">
    /// The name of the prefab used to create this instance
    /// </param>
    /// <param name="despawn">True to depawn on start</param>
    /// <param name="parent">True to make a child of the pool's group</param>
    public void Add(Transform instance, string prefabName, bool despawn, bool parent)
    {
        for (int i = 0; i < _prefabPools.Count; i++)
        {
            PrefabPool prefabPool = this._prefabPools[i];
            if (prefabPool.prefabGO == null)
            {
                if (this.logMessages)
                    LogSystem.LogWarning("Unexpected Error: PrefabPool.prefabGO is null");
                return;
            }

            if (prefabPool.prefabGO.name == prefabName)
            {
                prefabPool.AddUnpooled(instance, despawn);

                if (this.logMessages)
                    LogSystem.LogWarning(string.Format(
                            "SpawnPool {0}: Adding previously unpooled instance {1}",
                                            this.poolName,
                                            instance.name));

                if (parent) instance.parent = this.group;

                // New instances are active and must be added to the internal list 
                if (!despawn)
                {
                    if (!mSpawnedObjectsToPools.ContainsKey(instance))
                    {
                        mSpawnedObjectsToPools.Add(instance, prefabPool);
                    }

                    if (!mAllObjectsToPools.ContainsKey(instance))
                    {
                        mAllObjectsToPools.Add(instance, prefabPool);
                    }

                    this.spawned.Add(instance);
                }

                return;
            }
        }

        // Log an error if a PrefabPool with the given name was not found
        if (this.logMessages)
            LogSystem.LogWarning(string.Format("SpawnPool {0}: PrefabPool {1} not found.",
                                     this.poolName,
                                     prefabName));

    }

    /// <description>
    ///	See docs for SpawnInstance(Transform prefab, Vector3 pos, Quaternion rot)
    ///	for basic functionalty information.
    ///		
    /// Pass a ParticleEmitter component of a prefab to instantiate, trigger 
    /// emit, then listen for when all particles have died to "auto-destruct", 
    /// but instead of destroying the game object it will be deactivated and 
    /// added to the pool to be reused.
    /// 
    /// IMPORTANT: 
    ///     * This function turns off Unity's ParticleAnimator autodestruct if
    ///       one is found.
    ///     * You must pass a ParticleEmitter next time as well, or the emitter
    ///       will be treated as a regular prefab and simply activate, but emit
    ///       will not be triggered!
    ///     * The listner that waits for the death of all particles will 
    ///       time-out after a set number of seconds and log a warning. 
    ///       This is done to keep the developer aware of any unexpected 
    ///       usage cases. Change the public property "maxParticleDespawnTime"
    ///       to adjust this length of time.
    /// 
    /// Broadcasts "OnSpawned" to the instance. Use this instead of Awake()
    ///		
    /// This function has the same initial signature as Unity's InstantiatePrefab() 
    /// that takes position and rotation. The return Type is different though.
    /// </description>
    /// <param name="prefab">
    /// The prefab to instance. Not used if an instance already exists in 
    /// the scene that is queued for reuse. Type = ParticleEmitter
    /// </param>
    /// <param name="pos">The position to set the instance to</param>
    /// <param name="quat">The rotation to set the instance to</param>
    /// <returns>
    /// The instance's ParticleEmitter. 
    /// 
    /// If the Limit option was used for the PrefabPool associated with the
    /// passed prefab, then this method will return null if the limit is
    /// reached. You DO NOT need to test for null return values unless you 
    /// used the limit option.
    /// </returns>
    public ParticleEmitter Spawn(ParticleEmitter prefab,
                                 Vector3 pos, Quaternion quat)
    {
        // Instance using the standard method before doing particle stuff
        Transform inst = Spawn(prefab.transform, pos, quat);

        // Can happen if limit was used
        if (inst == null) return null;

        // Make sure autodestrouct is OFF as it will cause null references
        var animator = inst.GetComponent<ParticleAnimator>();
        if (animator != null) animator.autodestruct = false;

        // Get the emitter
        var emitter = inst.GetComponent<ParticleEmitter>();
        emitter.emit = true;

        // Coroutines MUST be run on a MonoBehaviour. Use PoolManager.
        //   This will not affect PoolManager in any way. It is just used
        //   to host the coroutine
        this.StartCoroutine(this.ListenForEmitDespawn(emitter));

        return emitter;
    }

    // ParticleSystem (Shuriken) Version...
    public ParticleSystem Spawn(ParticleSystem prefab,
                                 Vector3 pos, Quaternion quat)
    {
        // Instance using the standard method before doing particle stuff
        Transform inst = Spawn(prefab.transform, pos, quat);

        // Can happen if limit was used
        if (inst == null) return null;

        // Get the emitter and start it
        var emitter = inst.GetComponent<ParticleSystem>();
        //emitter.Play(true);  // Seems to auto-play on activation so this may not be needed

        // Coroutines MUST be run on a MonoBehaviour. Use PoolManager.
        //   This will not affect PoolManager in any way. It is just used
        //   to host the coroutine
        this.StartCoroutine(this.ListenForEmitDespawn(emitter));

        return emitter;
    }

    /// <description>
    /// This will not be supported for Shuriken particles. This will eventually 
    /// be depricated.
    /// 
    ///	See docs for SpawnInstance(Transform prefab, Vector3 pos, Quaternion rot)
    ///	for basic functionalty information.
    ///		
    /// Pass a ParticleEmitter component of a prefab to instantiate, trigger 
    /// emit, then listen for when all particles have died to "auto-destruct", 
    /// but instead of destroying the game object it will be deactivated and 
    /// added to the pool to be reused.
    /// 
    /// IMPORTANT: 
    ///     * This function turns off Unity's ParticleAnimator autodestruct if
    ///       one is found.
    ///     * You must pass a ParticleEmitter next time as well, or the emitter
    ///       will be treated as a regular prefab and simply activate, but emit
    ///       will not be triggered!
    ///     * The listner that waits for the death of all particles will 
    ///       time-out after a set number of seconds and log a warning. 
    ///       This is done to keep the developer aware of any unexpected 
    ///       usage cases. Change the public property "maxParticleDespawnTime"
    ///       to adjust this length of time.
    /// 
    /// Broadcasts "OnSpawned" to the instance. Use this instead of Awake()
    ///		
    /// This function has the same initial signature as Unity's InstantiatePrefab() 
    /// that takes position and rotation. The return Type is different though.
    /// </description>
    /// <param name="prefab">
    /// The prefab to instance. Not used if an instance already exists in 
    /// the scene that is queued for reuse. Type = ParticleEmitter
    /// </param>
    /// <param name="pos">The position to set the instance to</param>
    /// <param name="quat">The rotation to set the instance to</param>
    /// <param name="colorPropertyName">Same as Material.SetColor()</param>
    /// <param name="color">a Color object. Same as Material.SetColor()</param>
    /// <returns>The instance's ParticleEmitter</returns>
    public ParticleEmitter Spawn(ParticleEmitter prefab,
                                 Vector3 pos, Quaternion quat,
                                 string colorPropertyName, Color color)
    {
        // Instance using the standard method before doing particle stuff
        Transform inst = Spawn(prefab.transform, pos, quat);

        // Can happen if limit was used
        if (inst == null) return null;

        // Make sure autodestrouct is OFF as it will cause null references
        var animator = inst.GetComponent<ParticleAnimator>();
        if (animator != null) animator.autodestruct = false;

        // Get the emitter
        var emitter = inst.GetComponent<ParticleEmitter>();

        // Set the color of the particles, then emit
        emitter.GetComponent<Renderer>().material.SetColor(colorPropertyName, color);
        emitter.emit = true;

        // Coroutines MUST be run on a MonoBehaviour. Use PoolManager.
        //   This will not affect PoolManager in any way. It is just used
        //   to host the coroutine
        this.StartCoroutine(ListenForEmitDespawn(emitter));

        return emitter;
    }

    /// <description>
    /// Despawns all active instances in this SpawnPool
    /// </description>
    public void DespawnAll()
    {
        var spawned = new List<Transform>(this.spawned);
        for (int i = 0; i < spawned.Count; i++)
        {
            Transform instance = spawned[i];
            this.Despawn(instance);
        }
    }


    /// <description>
    /// See docs for Despawn(Transform instance). This expands that functionality.
    ///   If the passed object is managed by this SpawnPool, it will be 
    ///   deactivated and made available to be spawned again.
    /// </description>
    /// <param name="item">The transform of the instance to process</param>
    /// <param name="seconds">The time in seconds to wait before despawning</param>
    public void Despawn(Transform instance, float seconds)
    {
        this.StartCoroutine(this.DoDespawnAfterSeconds(instance, seconds));
    }

    /// <summary>
    /// Waits X seconds before despawning. See the docs for DespawnAfterSeconds()
    /// </summary>
    private IEnumerator DoDespawnAfterSeconds(Transform instance, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        this.Despawn(instance);
    }

    /// <summary>
    /// Used to determine when a particle emiter should be despawned
    /// </summary>
    /// <param name="emitter">ParticleEmitter to process</param>
    /// <returns></returns>
    private IEnumerator ListenForEmitDespawn(ParticleEmitter emitter)
    {
        // This will wait for the particles to emit. Without this, there will
        //   be no particles in the while test below. I don't know why the extra 
        //   frame is required but should never be noticable. No particles can
        //   fade out that fast and still be seen to change over time.
        yield return null;
        yield return new WaitForEndOfFrame();

        // Do nothing until all particles die or the safecount hits a max value
        float safetimer = 0;   // Just in case! See Spawn() for more info
        while (emitter.particleCount > 0)
        {
            safetimer += Time.deltaTime;
            if (safetimer > this.maxParticleDespawnTime)
            {
                if (this.logMessages)
                    LogSystem.LogWarning(string.Format("SpawnPool {0}: " + "Timed out while listening for all particles to die. " +
                                  "Waited for {1}sec.", this.poolName, this.maxParticleDespawnTime));
            }

            yield return null;
        }

        // Turn off emit before despawning
        emitter.emit = false;
        this.Despawn(emitter.transform);
    }

    // ParticleSystem (Shuriken) Version...
    private IEnumerator ListenForEmitDespawn(ParticleSystem emitter)
    {
        // Wait for the delay time to complete
        // Waiting the extra frame seems to be more stable and means at least one 
        //  frame will always pass
        yield return new WaitForSeconds(emitter.startDelay + 0.25f);

        // Do nothing until all particles die or the safecount hits a max value
        float safetimer = 0;   // Just in case! See Spawn() for more info
        while (emitter.IsAlive(true))
        {
            if (!PoolManagerUtils.activeInHierarchy(emitter.gameObject))
            {
                emitter.Clear(true);
                yield break;  // Do nothing, already despawned. Quit.
            }
            safetimer += Time.deltaTime;
            if (safetimer > this.maxParticleDespawnTime)
            {
                if (this.logMessages)
                    LogSystem.LogWarning(string.Format("SpawnPool {0}: " + "Timed out while listening for all particles to die. " +
                                  "Waited for {1}sec.", this.poolName, this.maxParticleDespawnTime));
            }

            yield return null;
        }

        // Turn off emit before despawning
        //emitter.Clear(true);
        this.Despawn(emitter.transform);
    }

    #endregion 未用到接口
}
