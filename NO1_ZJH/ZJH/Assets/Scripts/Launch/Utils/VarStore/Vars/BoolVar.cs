using UnityEngine;

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class BoolVar :IVar
{
    private bool mValue = false;
    public BoolVar()
    {
    }
    public BoolVar(bool value)
    {
        mValue = value;
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_BOOL;
    }
    public override void SetBool(bool bValue)
    {
        mValue = bValue;
    }
    public override void SetByte(byte bValue)
    {
        mValue = bValue == 1;
    }
    public override void SetShort(short sValue)
    {
        mValue = sValue == 1;
    }
    public override void SetInt(int iValue)
    {
        mValue = iValue == 1;
    }
    public override void SetInt64(long lValue)
    {
        mValue = lValue == 1;
    }
    public override void SetFloat(float fValue)
    {
        mValue = fValue>0.01f;
    }
    public override void SetDouble(double dValue)
    {
        mValue = dValue>0.01f ;
    }
    public override void SetString(string strValue)
    {
        bool value;
        if (!string.IsNullOrEmpty(strValue) && bool.TryParse(strValue, out value))
        {
            mValue = value;
        }
        else
        {
            mValue = false;
        }
    }
    public override void SetWideStr(string wstrValue)
    {
        bool value;
        if (!string.IsNullOrEmpty(wstrValue) && bool.TryParse(wstrValue, out value))
        {
            mValue = value;
        }
        else
        {
            mValue = false;
        }
    }
    public override void SetBytes(byte[] bsValue , bool bdump = false, int start=0, int count = 0)
    {
        mValue = (bsValue != null &&bsValue.Length>0);
    }

    public override void SetObject(object oValue)
    {
        mValue = (oValue != null);
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
        mValue = (oValue != null);
    }
 
    public override bool GetBool()
    {
        return mValue;
    }
    public override byte GetByte()
    {
        return (byte)(mValue ? 1 : 0);
    }
    public override short GetShort()
    {
        return (short)(mValue ? 1 : 0);
    }
    public override int GetInt()
    {
        return mValue ? 1 : 0;
    }
    public override long GetInt64()
    {
        return mValue ? 1 : 0;
    }
    public override float GetFloat()
    {
        return mValue ? 1.0f : 0.0f;
    }
    public override double GetDouble()
    {
        return mValue ? 1.0f : 0.0f;
    }
    public override string GetString()
    {
        return mValue ? "true" : "false";
    }
    public override string GetWideStr()
    {
        return mValue ? "true" : "false";
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
        mValue = false;
        CollectVar(this);
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static BoolVar CreateVar(bool value)
    {
        BoolVar var = mVarStores.CreateVar();
        var.SetBool(value);
        return var;
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static BoolVar CreateVar()
    {
        return mVarStores.CreateVar();
    }
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(BoolVar var)
    {
        mVarStores.CollectVar(var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<BoolVar> mVarStores = new VarHelper<BoolVar>(20);
}