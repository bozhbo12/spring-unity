using UnityEngine;

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class UObjectVar :IVar
{
    private UnityEngine.Object mValue = null;
    public UObjectVar()
    {
    }
    public UObjectVar(UnityEngine.Object value)
    {
        mValue = value;
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_UOBJECT;
    }
    public override void SetBool(bool bValue)
    {
        mValue = null;
    }
    public override void SetByte(byte bValue)
    {
        mValue = null;
    }
    public override void SetShort(short sValue)
    {
        mValue = null;
    }
    public override void SetInt(int iValue)
    {
        mValue = null;
    }
    public override void SetInt64(long lValue)
    {
        mValue = null;
    }
    public override void SetFloat(float fValue)
    {
        mValue = null;
    }
    public override void SetDouble(double dValue)
    {
        mValue = null;
    }
    public override void SetString(string strValue)
    {
        mValue = null;
    }
    public override void SetWideStr(string wstrValue)
    {
        mValue = null;
    }
    public override void SetBytes(byte[] bsValue, bool bdump, int start = 0, int count = 0)
    {
        mValue = null;
    }

    public override void SetObject(object oValue)
    {
        mValue = null;
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
        mValue = oValue;
    }

    public override bool GetBool()
    {
        return mValue!=null;
    }
    public override byte GetByte()
    {
        return (byte)(mValue != null?1:0);
    }
    public override short GetShort()
    {
        return (byte)(mValue != null ? 1 : 0);
    }
    public override int GetInt()
    {
        return mValue != null ? 1 : 0;
    }
    public override long GetInt64()
    {
        return mValue != null ? 1 : 0;
    }
    public override float GetFloat()
    {
        return mValue != null ? 1.0f : 0.0f;
    }
    public override double GetDouble()
    {
        return mValue != null ? 1.0f : 0.0;
    }
    public override string GetString()
    {
        return mValue != null ? mValue.name : string.Empty;
    }
    public override string GetWideStr()
    {
        return mValue != null ? mValue.name : string.Empty;
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
        return mValue;
    }
    public override void Destroy()
    {
        mValue = null;
        CollectVar(this);
    }

    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static UObjectVar CreateVar()
    {
        return mVarStores.CreateVar();
    }

    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static UObjectVar CreateVar(UnityEngine.Object value)
    {
        UObjectVar var = mVarStores.CreateVar();
        var.SetUObject(value);
        return var;
    }
 
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(UObjectVar Var)
    {
        mVarStores.CollectVar(Var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<UObjectVar> mVarStores = new VarHelper<UObjectVar>(20);
}