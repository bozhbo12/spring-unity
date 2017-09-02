using UnityEngine;
using SysUtils;
/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class ObjectIDVar : IVar
{
    private ObjectID mValue = ObjectID.zero;
    public ObjectIDVar()
    {
        
    }
    public ObjectIDVar(ObjectID value)
    {
        mValue = value;
    }

    public override byte GetVarType()
    {
        return VarStoreType.VT_OBJECTID;
    }

    public override void SetBool(bool bValue)
    {

    }
    public override void SetByte(byte bValue)
    {
    }
    public override void SetShort(short sValue)
    {
    }
    public override void SetInt(int iValue)
    {
    }
    public override void SetInt64(long lValue)
    {
    }
    public override void SetFloat(float fValue)
    {
    }
    public override void SetDouble(double dValue)
    {
    }
    public override void SetString(string strValue)
    {
        mValue = ObjectID.FromString(strValue);
    }
    public override void SetWideStr(string wstrValue)
    {
    }
    public override void SetBytes(byte[] bsValue, bool bdump = false, int start = 0, int count = 0)
    {
    }

    public override void SetObject(object oValue)
    {
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
    }
    public override void SetObjectID(ObjectID oValue)
    {
        mValue = oValue;
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
        return mValue;
    }
    public override UnityEngine.Object GetUObject()
    {
        return null;
    }
    public override ObjectID GetObjectID()
    {
       return mValue;
    }
    public override void Destroy()
    {
        mValue = ObjectID.zero;
        CollectVar(this);
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static ObjectIDVar CreateVar()
    {
        return mVarStores.CreateVar();
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static ObjectIDVar CreateVar(ObjectID oValue)
    {
        ObjectIDVar oVar = mVarStores.CreateVar();
        oVar.SetObjectID(oValue);
        return oVar;
    }
   
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(ObjectIDVar Var)
    {
        mVarStores.CollectVar(Var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<ObjectIDVar> mVarStores = new VarHelper<ObjectIDVar>(20);
}