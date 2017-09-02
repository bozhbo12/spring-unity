using System.Collections.Generic;
using SysUtils;

//发送时为了区分宽字符用
public class WideString
{
    public WideString(string strContent)
    {
        mstrContent = strContent;
    }
    public static WideString ToWideString(string strContent)
    {
        return new WideString(strContent);
    }
    public override string ToString()
    {
        return mstrContent;
    }
    public string mstrContent;
}

/// <summary>
/// 变量存储器
/// </summary>
public class VarStore
{
    /// <summary>
    /// 变量存储列表
    /// </summary>
    private List<IVar> mParams = new List<IVar>(8);
    public static VarStore operator +(VarStore lo, bool bValue)
    {
        BoolVar var = BoolVar.CreateVar();
        var.SetBool(bValue);
        lo.mParams.Add(var);
        return lo;
    }
    public static VarStore operator +(VarStore lo, byte byValue)
    {
        ByteVar var = ByteVar.CreateVar();
        var.SetByte(byValue);
        lo.mParams.Add(var);
        return lo;
    }
    public static VarStore operator +(VarStore lo, short sValue)
    {
        ShortVar var = ShortVar.CreateVar();
        var.SetShort(sValue);
        lo.mParams.Add(var);
        return lo;
    }
    public static VarStore operator +(VarStore lo, int iValue)
    {
        IntVar var = IntVar.CreateVar();
        var.SetInt(iValue);
        lo.mParams.Add(var);
        return lo;
    }
    public static VarStore operator +(VarStore lo, long lValue)
    {
        Int64Var var = Int64Var.CreateVar();
        var.SetInt64(lValue);
        lo.mParams.Add(var);
        return lo;
    }
    public static VarStore operator +(VarStore lo, float fValue)
    {
        FloatVar var = FloatVar.CreateVar();
        var.SetFloat(fValue);
        lo.mParams.Add(var);
        return lo;
    }
    public static VarStore operator +(VarStore lo, double dValue)
    {
        DoubleVar var = DoubleVar.CreateVar();
        var.SetDouble(dValue);
        lo.mParams.Add(var);
        return lo;
    }
    public static VarStore operator +(VarStore lo, string strValue)
    {
        StringVar var = StringVar.CreateVar();
        var.SetString(strValue);
        lo.mParams.Add(var);
        return lo;
    }

    public static VarStore operator +(VarStore lo, WideString chValue)
    {
        WideStrVar var = WideStrVar.CreateVar();
        var.SetWideStr(chValue.mstrContent);
        lo.mParams.Add(var);
        return lo;
    }

    public static VarStore operator +(VarStore lo, object oValue)
    {
        ObjectVar var = ObjectVar.CreateVar();
        var.SetObject(oValue);
        lo.mParams.Add(var);
        return lo;
    }

    public static VarStore operator +(VarStore lo, UnityEngine.Object oValue)
    {
        UObjectVar var = UObjectVar.CreateVar();
        var.SetUObject(oValue);
        lo.mParams.Add(var);
        return lo;
    }
    public static VarStore operator +(VarStore lo, ObjectID oValue)
    {
        ObjectIDVar var = ObjectIDVar.CreateVar();
        var.SetObjectID(oValue);
        lo.mParams.Add(var);
        return lo;
    }
    public static VarStore operator +(VarStore lo, IVar chValue)
    {
        lo.mParams.Add(chValue);
        return lo;
    }
    /// <summary>
    /// 获取变量个数
    /// </summary>
    /// <returns></returns>
    public int GetVarCount()
    {
        return mParams.Count;
    }
    /// <summary>
    /// 获取指定序号的变量，索引模式
    /// </summary>
    /// <param name="iIndex"></param>
    /// <returns></returns>
    public IVar GetVar(int iIndex)
    {
        if (iIndex >= 0 && iIndex < mParams.Count)
        {
            return mParams[iIndex];
        }
        return null;
    }
    /// <summary>
    /// 获取指定序号的变量，索引模式
    /// </summary>
    /// <param name="iIndex"></param>
    /// <returns></returns>
    public void SetVar(int iIndex,IVar var)
    {
        if (iIndex >= 0 && iIndex < mParams.Count)
        {
            IVar oldvar = mParams[iIndex];
            if (oldvar != null )
            {
                oldvar.Destroy();
            }
            mParams[iIndex] = var;
        }
    }
    /// <summary>
    /// 添加变量
    /// </summary>
    /// <param name="oData"></param>
    public void PushVar(IVar oData)
    {
        mParams.Add(oData);
    }
    /// <summary>
    /// 弹出变量模式
    /// </summary>
    /// <returns></returns>
    public IVar PopVar()
    {
        int iCount = mParams.Count;
        if (iCount > 0)
        {
            IVar param = mParams[iCount - 1];
            mParams.RemoveAt(iCount - 1);
            return param;
        }
        return null;
    }

    public void AddBool(bool bValue)
    {
        BoolVar value = BoolVar.CreateVar();
        value.SetBool(bValue);
        mParams.Add(value);
    }

    public void AddByte(byte byValue)
    {
        ByteVar value = ByteVar.CreateVar();
        value.SetByte(byValue);
        mParams.Add(value);
    }

    public void AddShort(short sValue)
    {
        ShortVar var = ShortVar.CreateVar();
        var.SetShort(sValue);
        mParams.Add(var);
    }

    public void AddInt(int iValue)
    {
        IntVar var = IntVar.CreateVar();
        var.SetInt(iValue);
        mParams.Add(var);
    }

    public void AddInt64(long lValue)
    {
        Int64Var var = Int64Var.CreateVar();
        var.SetInt64(lValue);
        mParams.Add(var);
    }

    public void AddFloat(float fValue)
    {
        FloatVar var = FloatVar.CreateVar();
        var.SetFloat(fValue);
        mParams.Add(var);
    }

    public void AddDouble(double dValue)
    {
        DoubleVar var = DoubleVar.CreateVar();
        var.SetDouble(dValue);
        mParams.Add(var);
    }

    public void AddString(string strValue)
    {
        StringVar var = StringVar.CreateVar();
        var.SetString(strValue);
        mParams.Add(var);
    }

    public void AddWideStr(string chValue)
    {
        WideStrVar var = WideStrVar.CreateVar();
        var.SetWideStr(chValue);
        mParams.Add(var);
    }

    public void AddObject(object oValue)
    {
        ObjectVar var = ObjectVar.CreateVar();
        var.SetObject(oValue);
        mParams.Add(var);
    }

    public void AddUnityObject(UnityEngine.Object oValue)
    {
        UObjectVar var = UObjectVar.CreateVar();
        var.SetObject(oValue);
        mParams.Add(var);
    }
    public void AddObjectID(ObjectID oValue)
    {
        ObjectIDVar var = ObjectIDVar.CreateVar(oValue);
        var.SetObject(oValue);
        mParams.Add(var);
    }
    public void AddBytes(byte[] byValue,bool bDump=false,int start = 0, int count= 0)
    {
        BytesVar value = BytesVar.CreateVar();
        value.SetBytes(byValue, bDump, start,count);
        mParams.Add(value);
    }
    public void AddEvent<T>(T onEvent)
    {
        EventVar<T> var = EventVar<T>.CreateVar(onEvent);
        mParams.Add(var);
    }
    public void SetBool(int index, bool value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetBool(value);
    }
    public void SetByte(int index, byte value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetByte(value);
    }
    public void SetShort(int index,short value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetShort(value);
    }
    public void SetInt(int index,int value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetInt(value);
    }
    public void SetInt64(int index,long value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetInt64(value);
    }
    public void SetFloat(int index,float value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetFloat(value);
    }
    public void SetDouble(int index,double value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetDouble(value);
    }
    public void SetString(int index,string value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetString(value);
    }
    public void SetWideStr(int index,string value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetWideStr(value);
    }
    public void SetBytes(int index,byte[] value,bool bDump=false,int start=0,int count=0)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetBytes(value, bDump, start, count);
    }
    public void SetObject(int index,object value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetObject(value);
    }
    public void SetUObject(int index, UnityEngine.Object value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetUObject(value);
    }
    public void SetObjectID(int index ,ObjectID value)
    {
        IVar var = GetVar(index);
        if (var == null)
            return;

        var.SetObjectID(value);
    }
    public void SetEvent<T>(int index, T onEvent)
    {
        EventVar<T> var = GetVar(index) as EventVar<T>;
        if (var == null)
            return;

        var.SetEvent(onEvent);
    }
   
    public byte GetVarType(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return VarStoreType.VT_NONE;

        return var.GetVarType();
    }
    public bool GetBool(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return false;

        return var.GetBool();
    }
    public byte GetByte(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return 0;

        return var.GetByte();
    }
    public short GetShort(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return 0;

        return var.GetShort();
    }
    public int GetInt(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return 0;

        return var.GetInt();
    }
    public long GetInt64(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return 0;

        return var.GetInt64();
    }
    public float GetFloat(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return 0.0f;

        return var.GetFloat();
    }
    public double GetDouble(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return 0.0f;

        return var.GetDouble();
    }
    public string GetString(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return string.Empty;

        return var.GetString();
    }
    public string GetWideStr(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return string.Empty;

        return var.GetWideStr();
    }
    public byte[] GetBytes(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return null;

        return var.GetBytes();
    }
    public object GetObject(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return null;

        return var.GetObject();
    }
    public UnityEngine.Object GetUnityObject(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return null;

        return var.GetUObject();
    }
    public ObjectID GetObjectID(int index)
    {
        IVar var = GetVar(index);
        if (var == null)
            return ObjectID.zero;

        return var.GetObjectID();
    }

    public T GetEvent<T>(int index)
    {
        EventVar<T> var = GetVar(index) as EventVar<T>;
        if (var == null)
            return default(T);

       return var.GetEvent();
    }
    /// <summary>
    /// 清空
    /// </summary>
    public void Clear()
    {
        ///回收内部成员
        for (int i = 0; i < mParams.Count; i++)
        {
            IVar var = mParams[i];
            if (var != null)
            {
                var.Destroy();
            }
        }
        ///清空并回收自己
        mParams.Clear();
    }
    //添加对象销毁
    public void Destroy()
    {
        ///回收内部成员
        Clear();
        CollectVarStore(this);
    }

    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public static VarStore CreateVarStore()
    {
        return mVarStores.CreateVar();
    }

    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    private static void CollectVarStore(VarStore varStore)
    {
        mVarStores.CollectVar(varStore);
    }
    /// <summary>
    /// VarStore缓存池，最大缓冲数量20个
    /// </summary>
    private static VarHelper<VarStore> mVarStores = new VarHelper<VarStore>(20);
}
