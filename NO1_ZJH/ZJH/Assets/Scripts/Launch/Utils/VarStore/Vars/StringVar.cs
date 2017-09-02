using UnityEngine;

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class StringVar : IVar
{
    private string mValue = string.Empty;
    public StringVar()
    {
    }
    public StringVar(string value)
    {
        mValue = value;
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_STRING;
    }
    public override void SetBool(bool bValue)
    {
        mValue = bValue ? "true" :"false";
    }
    public override void SetByte(byte bValue)
    {
        mValue = bValue.ToString();
    }
    public override void SetShort(short sValue)
    {
        mValue = sValue.ToString();
    }
    public override void SetInt(int iValue)
    {
        mValue = iValue.ToString();
    }
    public override void SetInt64(long lValue)
    {
        mValue = lValue.ToString();
    }
    public override void SetFloat(float fValue)
    {
        mValue = fValue.ToString();
    }
    public override void SetDouble(double dValue)
    {
        mValue = dValue.ToString();
    }
    public override void SetString(string strValue)
    {
        mValue = strValue;
    }
    public override void SetWideStr(string wstrValue)
    {
        mValue = wstrValue;
    }
    public override void SetBytes(byte[] bsValue, bool bdump, int start = 0, int count = 0)
    {
        mValue = (bsValue != null && bsValue.Length > 0) ? System.Text.Encoding.Default.GetString(bsValue) : string.Empty;
    }

    public override void SetObject(object oValue)
    {
        mValue = (oValue != null) ? oValue.ToString() : string.Empty;
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
        mValue = (oValue != null) ? oValue.ToString() : string.Empty;
    }

    public override bool GetBool()
    {
        return string.IsNullOrEmpty(mValue)?false:true;
    }
    public override byte GetByte()
    {
        byte value;
        if(byte.TryParse(mValue, out value))
        {
            return value;
        }
        return 0;
    }
    public override short GetShort()
    {
        short value;
        if (short.TryParse(mValue, out value))
        {
            return value;
        }
        return 0;
    }
    public override int GetInt()
    {
        int value;
        if (int.TryParse(mValue, out value))
        {
            return value;
        }
        return 0;
    }
    public override long GetInt64()
    {
        long value;
        if (long.TryParse(mValue, out value))
        {
            return value;
        }
        return 0;
    }
    public override float GetFloat()
    {
        float value;
        if (float.TryParse(mValue, out value))
        {
            return value;
        }
        return 0;
    }
    public override double GetDouble()
    {
        double value;
        if (double.TryParse(mValue, out value))
        {
            return value;
        }
        return 0;
    }
    public override string GetString()
    {
        return mValue;
    }
    public override string GetWideStr()
    {
        return mValue;
    }
    public override byte[] GetBytes()
    {
        return string.IsNullOrEmpty(mValue)?null:System.Text.Encoding.Default.GetBytes(mValue);
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
        mValue = string.Empty;
        CollectVar(this);
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static StringVar CreateVar()
    {
        return mVarStores.CreateVar();
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static StringVar CreateVar(string value)
    {
        StringVar var = mVarStores.CreateVar();
        var.SetString(value);
        return var;
    }
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(StringVar Var)
    {
        mVarStores.CollectVar(Var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<StringVar> mVarStores = new VarHelper<StringVar>(20);
}