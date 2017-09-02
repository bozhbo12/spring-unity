using System;
using UnityEngine;

/// <summary>
/// 可变变量存储单元基类
/// </summary>
public class BytesVar :IVar
{
    private byte[] mValue = null;
    public BytesVar()
    {
    }
    public BytesVar(byte[] value,bool bdump, int start, int count)
    {
        SetBytes(value, bdump,start,count);
    }
    public override byte GetVarType()
    {
        return VarStoreType.VT_BYTES;
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
        mValue = !string.IsNullOrEmpty(strValue) ? System.Text.Encoding.Default.GetBytes(strValue): null;
    }
    public override void SetWideStr(string wstrValue)
    {
        mValue = !string.IsNullOrEmpty(wstrValue) ?  System.Text.Encoding.Default.GetBytes(wstrValue) : null;
    }
    public override void SetBytes(byte[] bsValue,bool bDump, int start, int count)
    {
        if (bsValue == null)
        {
            mValue = null;
            return;
        }

        if ( bDump)
        {
            int iLength = bsValue.Length;
            ///如果为零，只生成数组对象
            mValue = new byte[iLength];
            if (iLength > 0)
            {
                Array.Copy(bsValue, mValue, bsValue.Length);
            }
            
        }
        else
        {
            mValue = bsValue;
        }
    }

    public override void SetObject(object oValue)
    {
        mValue = (byte[])oValue;
    }
    public override void SetUObject(UnityEngine.Object oValue)
    {
        mValue = null;
    }
   
    public override bool GetBool()
    {
        return mValue == null?false:true;
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
        return 0.0;
    }
    public override string GetString()
    {
        return mValue!=null?System.Text.Encoding.Default.GetString(mValue):string.Empty;
    }
    public override string GetWideStr()
    {
        return mValue != null ? System.Text.Encoding.Default.GetString(mValue) : string.Empty;
    }
    public override byte[] GetBytes()
    {
        return mValue;
    }

    public override object GetObject()
    {
        return mValue;
    }
    public override UnityEngine.Object GetUObject()
    {
        return null;
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
    public static BytesVar CreateVar()
    {
        return mVarStores.CreateVar();
    }

    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static BytesVar CreateVar(Byte[] value,bool bDump,int start, int count)
    {
        BytesVar var = mVarStores.CreateVar();
        var.SetBytes(value, bDump, start, count);
        return var;
    }

    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVar(BytesVar var)
    {
        mVarStores.CollectVar(var);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<BytesVar> mVarStores = new VarHelper<BytesVar>(5);
}