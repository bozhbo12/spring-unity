using System.Collections.Generic;


/// <summary>
/// �����洢��
/// </summary>
public class VarHelper<T>
{
    /// <summary>
    /// ��ǰ���������������
    /// </summary>
    private int miMaxVarStores = 20;
    /// <summary>
    /// �洢���б�
    /// </summary>
    private List<T> mVars = new List<T>();
   
    public VarHelper(int iMaxStores)
    {
        miMaxVarStores = iMaxStores;
    }
    /// <summary>
    /// ���������洢��
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
    /// ���ձ����洢��
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
    