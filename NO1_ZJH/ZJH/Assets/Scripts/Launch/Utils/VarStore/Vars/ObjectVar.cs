using UnityEngine;

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class ObjectVar : IVar
{
    private object mValue = null;
    public ObjectVar()
    {
    }
    public ObjectVar(object value)
    {
        mValue = value;
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_OBJECT;
    }
    public override void SetBool(bool bValue)
    {
        mValue = bValue;
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
        mValue = iValue;
    }
    public override void SetInt64(long lValue)
    {
        mValue = lValue;
    }
    public override void SetFloat(float fValue)
    {
        mValue = fValue;
    }
    public override void SetDouble(double dValue)
    {
        mValue = dValue;
    }
    public override void SetString(string strValue)
    {
        mValue = strValue;
    }
    public override void SetWideStr(string wstrValue)
    {
        mValue = wstrValue;
    }
    public override void SetBytes(byte[] bsValue, bool bdump = false, int start = 0, int count = 0)
    {
        mValue = bsValue;
    }

    public override void SetObject(object oValue)
    {
        mValue = oValue;
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
        mValue = oValue;
    }

    public override bool GetBool()
    {
        if (mValue == null)
            return false;
        return (bool)mValue;
    }
    public override byte GetByte()
    {
        if (mValue == null)
            return 0;
        return (byte)mValue;
    }
    public override short GetShort()
    {
        if (mValue == null)
            return 0;
        return (short)mValue;
    }
    public override int GetInt()
    {
        if (mValue == null)
            return 0;
        return (int)mValue;
    }
    public override long GetInt64()
    {
        if (mValue == null)
            return 0;
        return (long)mValue;
    }
    public override float GetFloat()
    {
        if (mValue == null)
            return 0;
        return (float)mValue;
    }
    public override double GetDouble()
    {
        if (mValue == null)
            return 0;
        return (double)mValue;
    }
    public override string GetString()
    {
        if (mValue == null)
            return string.Empty;
        return (string)mValue;
    }
    public override string GetWideStr()
    {
        if (mValue == null)
            return string.Empty;
        return (string)mValue;
    }
    public override byte[] GetBytes()
    {
        return (byte[])mValue;
    }
    public override object GetObject()
    {
        if (mValue == null)
            return null;
        return mValue;
    }
    public override UnityEngine.Object GetUObject()
    {
        if (mValue == null)
            return null;
        return (UnityEngine.Object)mValue;
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
    public static ObjectVar CreateVar()
    {
        return mVarStores.CreateVar();
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static ObjectVar CreateVar(object oValue)
    {
        ObjectVar oVar = mVarStores.CreateVar();
        oVar.SetObject(oValue);
        return oVar;
    }
   
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(ObjectVar Var)
    {
        mVarStores.CollectVar(Var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<ObjectVar> mVarStores = new VarHelper<ObjectVar>(20);
}