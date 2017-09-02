using UnityEngine;

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class ByteVar :IVar
{
    private byte mValue = 0;
    public ByteVar()
    {
    }
    public ByteVar(byte value)
    {
        mValue = value;
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_BYTE;
    }
    public override void SetBool(bool bValue)
    {
        mValue = (byte)(bValue?1:0);
    }
    public override void SetByte(byte bValue)
    {
        mValue = bValue ;
    }
    public override void SetShort(short sValue)
    {
        mValue = (byte)sValue ;
    }
    public override void SetInt(int iValue)
    {
        mValue = (byte)iValue;
    }
    public override void SetInt64(long lValue)
    {
        mValue = (byte)lValue;
    }
    public override void SetFloat(float fValue)
    {
        mValue = (byte)fValue;
    }
    public override void SetDouble(double dValue)
    {
        mValue = (byte)dValue;
    }
    public override void SetString(string strValue)
    {
        byte value;
        if (!string.IsNullOrEmpty(strValue) && byte.TryParse(strValue, out value))
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
        byte value;
        if (!string.IsNullOrEmpty(wstrValue) && byte.TryParse(wstrValue, out value))
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
        mValue = (byte)((bsValue != null && bsValue.Length > 0)?1:0);
    }

    public override void SetObject(object oValue)
    {
        mValue = (byte)((oValue != null) ? 1 : 0);
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
        mValue = (byte)((oValue != null) ? 1 : 0);
    }
  
    public override bool GetBool()
    {
        return mValue==1;
    }
    public override byte GetByte()
    {
        return mValue ;
    }
    public override short GetShort()
    {
        return mValue;
    }
    public override int GetInt()
    {
        return mValue ;
    }
    public override long GetInt64()
    {
        return mValue ;
    }
    public override float GetFloat()
    {
        return mValue;
    }
    public override double GetDouble()
    {
        return mValue ;
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
    public static ByteVar CreateVar()
    {
        return mVarStores.CreateVar();
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static ByteVar CreateVar(byte value)
    {
        ByteVar var = mVarStores.CreateVar();
        var.SetByte(value);
        return var;
    }
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(ByteVar Var)
    {
        mVarStores.CollectVar(Var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<ByteVar> mVarStores = new VarHelper<ByteVar>(20);
}