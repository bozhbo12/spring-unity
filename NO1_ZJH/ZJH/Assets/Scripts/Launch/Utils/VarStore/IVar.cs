using SysUtils;
using UnityEngine;

/// <summary>
/// 变量类型及顺序，顺序切勿修改
/// </summary>
public class VarStoreType
{
    public const byte VT_NONE      = 0;
    public const byte VT_BOOL      = 1;
    public const byte VT_INT       = 2;
    public const byte VT_INT64     = 3;
    public const byte VT_FLOAT     = 4;
    public const byte VT_DOUBLE    = 5;
    public const byte VT_STRING    = 6;
    public const byte VT_WIDESTR   = 7;
    public const byte VT_OBJECTID  = 8;
    public const byte VT_OBJECT    = 9;
    public const byte VT_BYTES     =10;
    public const byte VT_BYTE      =11;
    public const byte VT_SHORT     =12;
    public const byte VT_UOBJECT   =13;
    public const byte VT_EVENT     = 14;
    public const byte VT_VECTOR3 = 15;
}

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class IVar
{

    public virtual byte GetVarType()
    {
        return VarStoreType.VT_NONE;
    }
    public virtual void Destroy() { }
    public virtual void SetBool(bool bValue) { }
    public virtual void SetByte(byte bValue) { }
    public virtual void SetShort(short sValue) { }
    public virtual void SetInt(int iValue) { }
    public virtual void SetInt64(long lValue) { }
    public virtual void SetFloat(float fValue) { }
    public virtual void SetDouble(double dValue) { }
    public virtual void SetString(string strValue) { }
    public virtual void SetWideStr(string wstrValue) { }
    public virtual void SetBytes(byte[] bsValue , bool bdump, int start, int count) { }
    public virtual void SetObject(object oValue) { }
    public virtual void SetUObject(UnityEngine.Object oValue) { }
    public virtual void SetVector3(Vector3 vValue) { }
    public virtual void SetObjectID(ObjectID oValue) { }
    public virtual bool GetBool() { return false; }
    public virtual byte GetByte() { return 0; }
    public virtual short GetShort() { return 0; }
    public virtual int GetInt() { return 0; }
    public virtual long GetInt64() { return 0; }
    public virtual float GetFloat() { return 0.0f; }
    public virtual double GetDouble() { return 0.0f; }
    public virtual string GetString() { return string.Empty; }
    public virtual string GetWideStr() { return string.Empty; }
    public virtual byte[] GetBytes() { return null; }
    public virtual object GetObject() { return null; }
    public virtual UnityEngine.Object GetUObject() { return null; }
    public virtual ObjectID GetObjectID() { return ObjectID.zero; }
    public virtual Vector3 GetVector3() { return Vector3.zero; }
}