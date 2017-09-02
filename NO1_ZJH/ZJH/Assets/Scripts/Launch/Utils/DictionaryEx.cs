using System.Collections.Generic;
/// <summary>
/// 字典替代类
/// Author：chengang1
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TValue"></typeparam>
public class DictionaryEx<TKey, TValue> : Dictionary<TKey, TValue>
{
    public List<TKey> mList = new List<TKey>();
    public DictionaryEx()
    { }

    public DictionaryEx(Dictionary<TKey, TValue> value):base(value)
    {
        mList.AddRange(value.Keys);
    }

    public DictionaryEx(IEqualityComparer<TKey> comparer)
        : base(comparer)
    {

    }

    /// <summary>
    /// 增加
    /// </summary>
    /// <param name="tkey"></param>
    /// <param name="tvalue"></param>
    public new void Add(TKey tkey, TValue tvalue)
    {
        mList.Add(tkey);
        base.Add(tkey, tvalue);
    }
    /// <summary>
    /// 移除
    /// </summary>
    /// <param name="tkey"></param>
    /// <returns></returns>
    public new bool Remove(TKey tkey)
    {
        mList.Remove(tkey);
        return base.Remove(tkey);
    }
    /// <summary>
    /// 方便取存值
    /// </summary>
    /// <param name="tkey"></param>
    /// <returns></returns>
    public new TValue this[TKey tkey]
    {
        get
        {
            return base[tkey];
        }
        set
        {
            if (ContainsKey(tkey))
            {
                base[tkey] = value;
            }
            else
            {
                Add(tkey, value);
            }
        }
    }
    public bool GetTryValue(int index, out TValue value)
    {
        if (index >= 0 && index < mList.Count)
        {
            TKey key;
            if (GetTryKey(index, out key))
            {
                if (SafeHelper.IsObjectNotNull(key))
                {
                    TryGetValue(key, out value);
                    return true;
                }
            }
        }
        value = default(TValue);
        return false;
    }

    public bool GetTryKey(int index, out TKey value)
    {
        if (index >= 0 && index < mList.Count)
        {
            value = mList[index];
            return true;
        }
        value = default(TKey);
        return false;
    }

    /// <summary>
    /// 通过索引获取Value
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TValue GetValue(int index)
    {
        TKey tkey = mList[index];
        if(tkey != null && !tkey.Equals(null) )
        {
            TValue value;
            if (TryGetValue(tkey, out value))
            {
                return value;
            }
        }

        return default(TValue);
    }

    /// <summary>
    /// 通过索引获取键值
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public TKey GetKey(int index)
    {
        return mList[index];
    }
    /// <summary>
    /// 查询是否有键名
    /// </summary>
    /// <param name="tkey"></param>
    /// <returns></returns>
    public new bool ContainsKey(TKey tkey)
    {
        return mList.Contains(tkey);
    }
    /// <summary>
    /// 清除数据
    /// </summary>
    public new void Clear()
    {
        mList.Clear();
        base.Clear();
    }
}
