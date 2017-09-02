using UnityEngine;

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class ShortVar : IVar
{
    private short mValue = 0;
    public ShortVar()
    {
    }
    public ShortVar(short value)
    {
        mValue = value;
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_SHORT;
    }
    public override void SetBool(bool bValue)
    {
        mValue = (short)(bValue ? 1 : 0);
    }
    public override void SetByte(byte bValue)
    {
        mValue = bValue;
    }
    public override void SetShort(short sValue)
    {
        mValue = sValue;
    }
    public override void SetInt(int iValue)
    {
        mValue = (short)iValue;
    }
    public override void SetInt64(long lValue)
    {
        mValue = (short)lValue;
    }
    public override void SetFloat(float fValue)
    {
        mValue = (short)fValue;
    }
    public override void SetDouble(double dValue)
    {
        mValue = (short)dValue;
    }
    public override void SetString(string strValue)
    {
        short value;
        if (!string.IsNullOrEmpty(strValue) && short.TryParse(strValue, out value))
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
        short value;
        if (!string.IsNullOrEmpty(wstrValue) && short.TryParse(wstrValue, out value))
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
        mValue = (short)((bsValue != null && bsValue.Length > 0) ? 1 : 0);
    }

    public override void SetObject(object oValue)
    {
        mValue = (short)((oValue != null) ? 1 : 0);
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
        mValue = (short)((oValue != null) ? 1 : 0);
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
        return mValue;
    }
    public override int GetInt()
    {
        return mValue;
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
    public static ShortVar CreateVar()
    {
        return mVarStores.CreateVar();
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static ShortVar CreateVar(short value)
    {
        ShortVar var = mVarStores.CreateVar();
        var.SetShort(value);
        return var;
    }
    
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(ShortVar Var)
    {
        mVarStores.CollectVar(Var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<ShortVar> mVarStores = new VarHelper<ShortVar>(20);
}