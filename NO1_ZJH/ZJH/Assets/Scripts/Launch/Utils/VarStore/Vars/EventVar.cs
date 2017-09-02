using UnityEngine;

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class EventVar<T> :IVar
{
    private T mValue = default(T);
    public EventVar()
    {
    }

    public EventVar(T tvalue)
    {
        mValue = tvalue;
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_EVENT;
    }
    public override void SetBool(bool bValue)
    {
        mValue = default(T);
    }
    public override void SetByte(byte bValue)
    {
        mValue = default(T);
    }
    public override void SetShort(short sValue)
    {
        mValue = default(T);
    }
    public override void SetInt(int iValue)
    {
        mValue = default(T);
    }
    public override void SetInt64(long lValue)
    {
        mValue = default(T);
    }
    public override void SetFloat(float fValue)
    {
        mValue = default(T);
    }
    public override void SetDouble(double dValue)
    {
        mValue = default(T);
    }
    public override void SetString(string strValue)
    {
        mValue = default(T);
    }
    public override void SetWideStr(string wstrValue)
    {
        mValue = default(T);
    }
    public override void SetBytes(byte[] bsValue, bool bdump = false, int start = 0, int count = 0)
    {
        mValue = default(T);
    }

    public override void SetObject(object oValue)
    {
        mValue = default(T);
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
        mValue = default(T);
    }
    public void SetEvent(T t)
    {
        mValue = t;
    }

    public T GetEvent()
    {
        return mValue;
    }
    public override bool GetBool()
    {
        return false;
    }
    public override byte GetByte()
    {
        return 0;
    }
    public override short GetShort()
    {
        return 0;
    }
    public override int GetInt()
    {
        return 0;
    }
    public override long GetInt64()
    {
        return 0;
    }
    public override float GetFloat()
    {
        return 0.0f;
    }
    public override double GetDouble()
    {
        return 0.0f;
    }
    public override string GetString()
    {
        return string.Empty;
    }
    public override string GetWideStr()
    {
        return string.Empty;
    }
    public override byte[] GetBytes()
    {
        return null;
    }
    public override object GetObject()
    {
        return null;
    }
    public override UnityEngine.Object GetUObject()
    {
        return null;
    }

    public override void Destroy()
    {
        mValue = default(T);

        CollectVar(this);
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static EventVar<T> CreateVar(T tValue)
    {
        return new EventVar<T>(tValue);
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static EventVar<T> CreateVar()
    {
        return new EventVar<T>();
    }
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(EventVar<T> Var)
    {
        
    }
}