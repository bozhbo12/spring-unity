using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector3Var : IVar
{
    private Vector3 mValue = Vector3.zero;
    public Vector3Var()
    {
    }
    public Vector3Var(Vector3 value)
    {
        mValue = value;
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_VECTOR3;
    }

    public override void SetBool(bool bValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetByte(byte bValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetShort(short sValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetInt(int iValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetInt64(long lValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetFloat(float fValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetDouble(double dValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetString(string strValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetWideStr(string wstrValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetBytes(byte[] bsValue, bool bdump = false, int start = 0, int count = 0)
    {
        mValue = Vector3.zero;
    }

    public override void SetObject(object oValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
        mValue = Vector3.zero;
    }
    public override void SetVector3(Vector3 vValue)
    {
        mValue = vValue;
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
        return 0;
    }
    public override double GetDouble()
    {
        return 0;
    }
    public override string GetString()
    {
        return string.Empty;
    }
    public override string GetWideStr()
    {
        return string.Empty;
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
    public override Vector3 GetVector3()
    {
        return mValue;
    }

    public override void Destroy()
    {
        mValue = Vector3.zero;
        CollectVar(this);
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static Vector3Var CreateVar(Vector3 value)
    {
        Vector3Var var = mVarStores.CreateVar();
        var.SetVector3(value);
        return var;
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static Vector3Var CreateVar()
    {
        return mVarStores.CreateVar();
    }
    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(Vector3Var var)
    {
        mVarStores.CollectVar(var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<Vector3Var> mVarStores = new VarHelper<Vector3Var>(20);
}