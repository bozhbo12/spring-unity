using System.Collections.Generic;


/// <summary>
/// 变量存储器
/// </summary>
public class VarHelper<T>
{
    /// <summary>
    /// 当前变量允许的最大个数
    /// </summary>
    private int miMaxVarStores = 20;
    /// <summary>
    /// 存储器列表
    /// </summary>
    private List<T> mVars = new List<T>();
   
    public VarHelper(int iMaxStores)
    {
        miMaxVarStores = iMaxStores;
    }
    /// <summary>
    /// 创建变量存储器
    /// </summary>
    /// <returns></returns>
    public T CreateVar()
    {
        if(mVars.Count > 0 )
        {
            T var = mVars[0];
            mVars.RemoveAt(0);
            return var;
        }

        return System.Activator.CreateInstance<T>();
    }

    /// <summary>
    /// 回收变量存储器
    /// </summary>
    /// <param name="varStore"></param>
    public void CollectVar( T var )
    {
        if (var == null)
            return;

        if(mVars.Count > miMaxVarStores)
        {
            mVars.RemoveAt(0);
        }

        mVars.Add(var);
    }
}
    