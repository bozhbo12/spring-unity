using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 得到绑定点
/// </summary>
public class AttachmentPoint : MonoBehaviour
{

    public enum PitPosition
    {
        //槽位一
        Pit_One = 0,
		//槽位二 （左手武器）
        Pit_two = 1,
		//槽位三 （背饰）
		Pit_three =2,
    }

    public enum Slot
    {
        None = 0,			// First element MUST be 'None'
        SelfBody, //自身节点
        Head,
        LeftHand, //左手
        RightHand, //右手
        Shoulders, //背部（坐骑）
        LeftFoot,//左脚
        RightFoot,////右脚
        FBX,
        /// <summary>
        /// 翅膀
        /// </summary>
        Wing,
        Hair,
        CreateRoleHand,//创建角色右手
    }

    //当前点类型
    public Slot slot;

    /// <summary>
    /// 槽位
    /// </summary>
    public List<GameObject> mChildList = null;

    public List<GameObject> ChildList
    {
        get
        {
            return mChildList;
        }
    }

    /// <summary>
    /// 加入绑点
    /// </summary>
    /// <param name="go"></param>
    /// <param name="index"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public bool Attach(GameObject go, float scale = 1, int index = 0)
    {
        if (go == null)
        {
            LogSystem.LogWarning("[Attach]::go is null");
            return false;
        }

        if (bOccupied(index))
        {
#if BUILD
            LogSystem.Log("坑位已被占用:" + gameObject.name + " " + go.name + "  " + slot + " " + index + " 占用人:" + mChildList[index].name);
#else
            LogSystem.LogWarning("坑位已被占用:" + gameObject.name + " " + go.name + "  " + slot + " " + index + " 占用人:" + mChildList[index].name);
#endif
        }

        AddGameObject(go, index);
        Vector3 localScale = go.transform.localScale / scale;
        go.transform.parent = transform;
        go.transform.localScale = localScale;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        return true;
    }

    /// <summary>
    /// 是否含用子节点
    /// </summary>
    public bool bHaveChild
    {
        get
        {
            if(mChildList == null || mChildList.Count == 0)
                return false;

            for (int i = 0; i < mChildList.Count; i++)
            {
                if(mChildList[i] != null)
                    return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 移除
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public bool Remove(GameObject go)
    {
        if (go == null)
            return false;

        if (mChildList == null)
            return false;

        if (!mChildList.Contains(go))
            return false;

        Vector3 vScale = go.transform.localScale;

        go.transform.parent = null;

        go.transform.localScale = vScale;

        return mChildList.Remove(go);
    }

    /// <summary>
    /// 移除
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool Remove(int index)
    {
        if (mChildList == null)
            return false;

        if (index >= mChildList.Count || index < 0)
            return false;

        GameObject go = mChildList[index];
        if (go == null)
            return false;

        Vector3 vScale = go.transform.localScale;
        
        go.transform.parent = null;

        go.transform.localScale = vScale;

        mChildList[index] = null;

#if BUILD
        LogSystem.Log("释放坑位:" + gameObject.name + " " + go.name + "  " + slot + " " + index);
#endif
        return true;
    }

    /// <summary>
    /// 删除
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool DestroyChild(int index)
    {
        if (mChildList == null)
            return false;

        if (index >= mChildList.Count || index < 0)
            return false;

        CacheObjects.DestoryPoolObject(mChildList[index]);
        mChildList[index] = null;
        return true;
    }

    /// <summary>
    /// 清除
    /// </summary>
    /// <param name="bDestroyChild"></param>
    /// <returns></returns>
    public bool Clear(bool bDestroyChild)
    {
        if (mChildList == null || mChildList.Count == 0)
            return false;

        for (int i = mChildList.Count - 1; i>=0; i--)
        {
            if (bDestroyChild)
            {
                CacheObjects.DestoryPoolObject(mChildList[i]);
                mChildList[i] = null;
            }
            else
            {
                Remove(i);
            }
        }
        return true;
    }

    /// <summary>
    /// 获取object
    /// </summary>
    /// <param name="index"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool GetObject(int index, ref GameObject obj)
    {
        if (mChildList == null)
            return false;

        if (index >= mChildList.Count || index < 0)
            return false;

        obj = mChildList[index];
        return obj != null;
    }

    /// <summary>
    /// 是否被占用
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool bOccupied(int index)
    {
        if (mChildList == null)
            return false;

        if (index >= mChildList.Count || index < 0)
            return false;

        return mChildList[index] != null;
    }

    /// <summary>
    /// 将物体占用坑位
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private bool AddGameObject(GameObject obj, int index)
    {
        if (mChildList == null)
            mChildList = new List<GameObject>();

        if (index >= mChildList.Count)
        {
            //撑大坑位，暂没好方法
            for (int i = mChildList.Count; i <= index; i++)
            {
                mChildList.Add(null);
            }
        }

#if BUILD
        LogSystem.Log("占用坑位:"+gameObject.name+ " " +obj.name + " "+ slot +" " + index);
#endif
        mChildList[index] = obj;
        return true;
    }

    /// <summary>
    /// 添加特效
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public bool AttachEffect(GameObject go)
    {
        if (go == null)
        {
            LogSystem.LogWarning("[AttachEffect]::go is null");
            return false;
        }
        go.transform.parent = transform;
        return true;
    }

    void OnDestroy()
    {
        if (mChildList != null)
            mChildList.Clear();
    }
}
