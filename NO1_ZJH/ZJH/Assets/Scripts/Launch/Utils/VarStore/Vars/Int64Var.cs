using UnityEngine;

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class Int64Var : IVar
{
    private long mValue = 0;
    public Int64Var()
    {
    }
    public Int64Var(long value)
    {
        mValue = value;
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_INT64;
    }
    public override void SetBool(bool bValue)
    {
        mValue = bValue ? 1 : 0;
    }
    public override void SetByte(byte bValue)
    {
        mValue = (short)bValue;
    }
    public override void SetShort(short sValue)
    {
        mValue = (long)sValue;
    }
    public override void SetInt(int iValue)
    {
        mValue = (long)iValue;
    }
    public override void SetInt64(long lValue)
    {
        mValue = lValue;
    }
    public override void SetFloat(float fValue)
    {
        mValue = (long)fValue;
    }
    public override void SetDouble(double dValue)
    {
        mValue = (long)dValue;
    }
    public override void SetString(string strValue)
    {
        long value;
        if (!string.IsNullOrEmpty(strValue) && long.TryParse(strValue, out value))
        {
            mValue = value;
        }
        else
        {
            mValue = 0;
        }
    }
    public override void SetWideStr(string wstrValue)
    {
        long value;
        if (!string.IsNullOrEmpty(wstrValue) && long.TryParse(wstrValue, out value))
        {
            mValue = value;
        }
        else
        {
            mValue = 0;
        }
    }
    public override void SetBytes(byte[] bsValue, bool bdump = false, int start = 0, int count = 0)
    {
        mValue = (bsValue != null && bsValue.Length > 0) ? 1 : 0;
    }

    public override void SetObject(object oValue)
    {
        mValue = (oValue != null) ? 1 : 0;
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
        mValue = (oValue != null) ? 1 : 0;
    }

    public override bool GetBool()
    {
        return mValue == 1;
    }
    public override byte GetByte()
    {
        return (byte)mValue;
    }
    public override short GetShort()
    {
        return (short)mValue;
    }
    public override int GetInt()
    {
        return (int)mValue;
    }
    public override long GetInt64()
    {
        return mValue;
    }
    public override float GetFloat()
    {
        return mValue;
    }
    public override double GetDouble()
    {
        return mValue;
    }
    public override string GetString()
    {
        return mValue.ToString();
    }
    public override string GetWideStr()
    {
        return mValue.ToString();
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
        mValue = 0;
        CollectVar(this);
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static Int64Var CreateVar()
    {
        return mVarStores.CreateVar();
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static Int64Var CreateVar(long value)
    {
        Int64Var var = mVarStores.CreateVar();
        var.SetInt64(value);
        return var;
    }
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(Int64Var Var)
    {
        mVarStores.CollectVar(Var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<Int64Var> mVarStores = new VarHelper<Int64Var>(20);
}