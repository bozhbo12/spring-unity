using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClonePool
{
    public Transform oClone;
    public SpawnPool mSpawnPool;

    public List<Transform> despawned = new List<Transform>(16);
    private List<long> despawnedTime = new List<long>(16);
    public List<Transform> spawned = new List<Transform>(16);

    public Transform SpawnInstance()
    {
        if (oClone == null)
            return null;

        TrimEmpty();

        Transform trans;
        if (despawned.Count > 0 )
        {
            trans = despawned[0];
            
            despawned.RemoveAt(0);
            despawnedTime.RemoveAt(0);
            spawned.Add(trans);
            trans.localScale = oClone.localScale;
            //SpawnPool.ResetEffect(trans.gameObject);
            SpawnPool.CheckEffectWatcher(trans.gameObject);
            //CopyTransform(oClone, oClone, trans);
        }
        else
        {
            trans = SpawnNew(oClone);
            spawned.Add(trans);
        }

        ///添加到克隆列表
        mSpawnPool.mSpawnClones.Add(trans);
        //PoolManagerUtils.SetActive(trans.gameObject, true);

        return trans;
    }

    void CopyTransform(Transform Prefab, Transform transPrefab, Transform transCopy)
    {
        if (transCopy == null || transPrefab == null)
            return;

        if (!transPrefab.localPosition.Equals(transCopy.localPosition))
        {
            transCopy.localPosition = transPrefab.localPosition;
        }
#if DEBUGPOOL
        else
        {
            LogSystem.LogWarning("CopyTransform localPosition Diff", Prefab.name, " ->", transCopy.name);
        }
#endif
        if (!transPrefab.localRotation.Equals(transCopy.localRotation))
        {
            transCopy.localRotation = transPrefab.localRotation;
        }
#if DEBUGPOOL
        else
        {
            LogSystem.LogWarning("CopyTransform localRotation Diff", Prefab.name, " ->", transCopy.name);
        }
#endif
        if (!transPrefab.localScale.Equals(transCopy.localScale))
        {
            transCopy.localScale = transPrefab.localScale;
        }
#if DEBUGPOOL
        else
        {
            LogSystem.LogWarning("CopyTransform localScale Diff", Prefab.name, " ->", transCopy.name);
        }
#endif
        for (int i = 0; i < transPrefab.childCount; i++)
        {
            Transform prefabchild = transPrefab.GetChild(i);
            Transform copychild = transCopy.GetChild(i);
            if (prefabchild != null && copychild != null)
            {
                if (prefabchild.childCount == copychild.childCount)
                {
                    CopyTransform(Prefab, prefabchild, copychild);
                }
#if DEBUGPOOL
                else
                {
                    LogSystem.LogWarning("Check Object Trans Diff , Prefab: ", Prefab.name);
                }
#endif
            }
        }
    }
    void TrimEmpty()
    {
        for (int i = 0; i < despawned.Count; i++)
        {
            Transform trans = despawned[i];
            if (trans == null)
            {
                despawned.RemoveAt(i);
                despawnedTime.RemoveAt(i);
                i--;
                continue;
            }
        }
    }
    private Transform SpawnNew(Transform trans)
    {
        GameObject go = Object.Instantiate(trans.gameObject) as GameObject;
        if (go == null)
            return null;

        return go.transform; 
    }

    public void DespawnInstance(Transform trans)
    {
        if( spawned.Contains(trans) )
        {
            DetachObjectTree(trans);

            spawned.Remove(trans);
            mSpawnPool.mSpawnClones.Remove(trans);
            despawned.Add(trans);

            long lTickNow = System.DateTime.Now.Ticks;
            despawnedTime.Add(lTickNow);
            // Notify instance of event OnDespawned for custom code additions.
            //   This is done before handling the deactivate and enqueue incase 
            //   there the user introduces an unforseen issue.
            trans.gameObject.BroadcastMessage("OnDespawned",SendMessageOptions.DontRequireReceiver);

            // Deactivate the instance and all children
            //PoolManagerUtils.SetActive(trans.gameObject, false);
            trans.parent = mSpawnPool.cloneGroup;
        }
    }
    public void SelfDestruct()
    {
        for (int i = 0; i < despawned.Count; i++)
        {
            Transform trans = despawned[i];
            if (trans != null)
            {
                //DetachObjectTree(trans);
                //CacheObjects.PopWidget(trans);
                GameObject.Destroy(trans.gameObject);
            }
        }

        for( int i = 0; i< spawned.Count;i++)
        {
            Transform trans = spawned[i];
            if (trans != null)
            {
                mSpawnPool.mSpawnClones.Remove(trans);
                DetachObjectTree(trans);
                //CacheObjects.PopWidget(trans);
                GameObject.Destroy(trans.gameObject);
            }
        }
       
        spawned.Clear();
        despawned.Clear();
        despawnedTime.Clear();
        oClone = null;
    }
    public void DespawnObjectTree(Transform trans)
    {
        if (trans == null)
            return;

        int iChildCount = trans.childCount;
        if( iChildCount > 0 )
        {
            for (int i = iChildCount - 1; i >= 0; i--)
            {
                Transform childTrans = trans.GetChild(i);
                if (childTrans != null)
                {
                    DespawnObjectChildTree(childTrans);
                }
            }
        }
    }
    public void DespawnObjectChildTree(Transform trans)
    {
        ///如果是预制件对象，添加到预制件回收列表
        if (mSpawnPool.IsSpawnObject(trans))
        {
            mSpawnPool.Despawn(trans);
            return;
        }
        ///如何是克隆的对象，回收到克隆列表中
        if (mSpawnPool.IsSpawnClone(trans))
        {
            mSpawnPool.DespawnClone(trans);
            return;
        }

        int iChildCount = trans.childCount;
        if( iChildCount > 0 )
        {
            for (int i = iChildCount - 1; i >= 0; i--)
            {
                Transform childTrans = trans.GetChild(i);
                if (childTrans)
                {
                    DespawnObjectChildTree(childTrans);
                }
            }
        }
        
    }
    public void DetachObjectTree(Transform trans)
    {
        if (trans == null)
            return;

        int iChildCount = trans.childCount;
        if( iChildCount > 0 )
        {
            for (int i = iChildCount - 1; i >= 0; i--)
            {
                Transform childTrans = trans.GetChild(i);
                if (childTrans != null)
                {
                    DetachObjectChildTree(childTrans);
                }
            }
        }
       
    }

    public void DetachObjectChildTree(Transform trans)
    {
        int iChildCount = trans.childCount;
        if (iChildCount > 0)
        {
            for (int i = iChildCount - 1; i >= 0; i--)
            {
                Transform childTrans = trans.GetChild(i);
                if (childTrans)
                {
                    DetachObjectChildTree(childTrans);
                }
            }
        }

        if (trans.CompareTag(SpawnPool.strUntagged) == false)
        {
            ///如果是预制件对象，添加到预制件回收列表,这里只修改父亲结点，因为很快要回收了
            if (mSpawnPool.IsSpawnObject(trans))
            {
                trans.parent = mSpawnPool.group.transform;
                // spawnPool.Despawn(trans);
                return;
            }
            ///如何是克隆的对象，回收到克隆列表中,这里只修改父亲结点，因为很快要回收了
            if (mSpawnPool.IsSpawnClone(trans))
            {
                trans.parent = mSpawnPool.cloneGroup.transform;
                //spawnPool.DespawnClone(trans);
                return;
            }
        }
    }
    /// <summary>
    /// 优化控制克隆池的对象
    /// </summary>
    /// <param name="lTickNow"></param>
    /// <param name="lFreeTime"></param>
    public void OptimizeClonePool(long lTickNow ,long lFreeTime)
    {
        for (int i = 0; i < despawnedTime.Count; i++)
        {
            long lTick = despawnedTime[i];
            if (lTickNow - lTick > lFreeTime)
            {
                Transform trans = despawned[i];
                if( trans != null )
                {
                    DespawnObjectTree(trans);
                    //CacheObjects.PopWidget(trans);
                    GameObject.Destroy(trans.gameObject);
                }
                despawned.RemoveAt(i);
                despawnedTime.RemoveAt(i);
                i--;
                continue;
            }
        }
    }
}
