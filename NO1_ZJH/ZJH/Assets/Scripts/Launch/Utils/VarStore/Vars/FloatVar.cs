using UnityEngine;

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class FloatVar : IVar
{
    private float mValue = 0.0f;
    public FloatVar()
    {
    }
    public FloatVar(float value)
    {
        mValue = value;
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_FLOAT;
    }
    public override void SetBool(bool bValue)
    {
        mValue = bValue ? 1.0f : 0.0f;
    }
    public override void SetByte(byte bValue)
    {
        mValue = (float)bValue;
    }
    public override void SetShort(short sValue)
    {
        mValue = (float)sValue;
    }
    public override void SetInt(int iValue)
    {
        mValue = (float)iValue;
    }
    public override void SetInt64(long lValue)
    {
        mValue = (float)lValue;
    }
    public override void SetFloat(float fValue)
    {
        mValue = fValue;
    }
    public override void SetDouble(double dValue)
    {
        mValue = (float)dValue;
    }
    public override void SetString(string strValue)
    {
        float value;
        if (!string.IsNullOrEmpty(strValue) && float.TryParse(strValue, out value))
        {
            mValue = value;
        }
        else
        {
            mValue = 0.0f;
        }
    }
    public override void SetWideStr(string wstrValue)
    {
        float value;
        if (!string.IsNullOrEmpty(wstrValue) && float.TryParse(wstrValue, out value))
        {
            mValue = value;
        }
        else
        {
            mValue = 0.0f;
        }
    }
    public override void SetBytes(byte[] bsValue, bool bdump = false, int start = 0, int count = 0)
    {
        mValue = (bsValue != null && bsValue.Length > 0) ? 1.0f : 0.0f;
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
        return mValue > 0.00001f;
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
        return (long)mValue;
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
        mValue = 0.0f;
        CollectVar(this);
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static FloatVar CreateVar()
    {
        return mVarStores.CreateVar();
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static FloatVar CreateVar(float value)
    {
        FloatVar var = mVarStores.CreateVar();
        var.SetFloat(value);
        return var;
    }
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(FloatVar Var)
    {
        mVarStores.CollectVar(Var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<FloatVar> mVarStores = new VarHelper<FloatVar>(20);
}