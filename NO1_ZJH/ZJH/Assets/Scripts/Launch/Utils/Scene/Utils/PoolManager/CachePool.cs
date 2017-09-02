using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 脚本对象缓存
/// </summary>
public class CachePool
{
    public System.Type oType;
    public List<CacheObject> spawned = new List<CacheObject>(16);
    public List<long> despawnedTime = new List<long>(16);
    public List<CacheObject> despawned = new List<CacheObject>(16);
    public T SpawnCache<T>() where T : CacheObject
    {
        ///从当前列表中退出一个
        if (despawned.Count > 0)
        {
            CacheObject oCache = despawned[0];
            despawned.RemoveAt(0);
            despawnedTime.RemoveAt(0);
            spawned.Add(oCache);
            oCache.OnSpawn();

            return oCache as T;
        }

        ///新加一个
        CacheObject oNewCache = System.Activator.CreateInstance<T>() as CacheObject;
        spawned.Add(oNewCache);
        oNewCache.OnSpawn();

        return oNewCache as T;
    }

    public void DespawnCache(CacheObject oCache)
    {
        spawned.Remove(oCache);
        oCache.OnDespawn();
        despawned.Add(oCache);
        long lTick = System.DateTime.Now.Ticks;
        despawnedTime.Add(lTick);
    }

    public void SelfDestruct()
    {
        spawned.Clear();
        despawned.Clear();
        despawnedTime.Clear();
        oType = null;
    }

    /// <summary>
    /// 优化控制克隆池的对象
    /// </summary>
    /// <param name="lTickNow"></param>
    /// <param name="lFreeTime"></param>
    public void OptimizeCachePool(long lTickNow, long lFreeTime)
    {
        for (int i = 0; i < despawnedTime.Count; i++)
        {
            long lTick = despawnedTime[i];
            if (lTickNow - lTick > lFreeTime)
            {
                CacheObject oCache = despawned[i];
                if (oCache != null)
                {
                    oCache.OnDespawn();
                }
                despawnedTime.RemoveAt(i);
                despawned.RemoveAt(i);
                i--;
                continue;
            }
        }
    }
}

public class CacheObject
{
    public virtual void OnSpawn()
    {

    }
    public virtual void OnDespawn()
    {

    }
}