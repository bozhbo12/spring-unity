using UnityEngine;
using System.Collections.Generic;
/// <summary>
/// 显示or隐藏
/// </summary>
public class ObjectEnable : MonoBehaviour
{
    /// <summary>
    /// 隐藏的物体名称
    /// </summary>
    string mstrObj = string.Empty;

    /// <summary>
    /// 显示信息
    /// </summary>
    string mstrInfo = string.Empty;

    List<GameObject> moCurHide = new List<GameObject>();
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void OnGUI()
    {
        mstrObj = GUI.TextField(new Rect(0, 0, 300, 40), mstrObj, 15);//15为最大字符串长度  
        //信息  
        GUI.Label(new Rect(20, 100, 100, 20), mstrInfo);
        //登录按钮  
        if (GUI.Button(new Rect(0, 80, 70, 40), "隐藏"))
        {
            SetObjHide(mstrObj);
        }
        if (GUI.Button(new Rect(80, 80, 70, 40), "显示"))
        {
            SetObjShow(mstrObj);
        }
    }

    /// <summary>
    /// 显示隐藏物体
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    bool SetObjHide(string name)
    {
        GameObject go = GameObject.Find(name);
        if (go != null)
        {
            moCurHide.Add(go);
            go.SetActive(false);
            return true;
        }
        return false;
    }

    void SetObjShow(string name)
    {
        GameObject go = GetHideobjByName(name);
        if (go == null)
        {
            go = GameObject.Find(name);
        }
        if (go != null)
        {
            go.SetActive(true);
            RemoveHideObjByName(name);
        }
    }

    GameObject GetHideobjByName(string strName)
    {
        for (int i = 0; i < moCurHide.Count; i++)
        {
            if (strName == moCurHide[i].name)
                return moCurHide[i];
        }
        return null;
    }

    /// <summary>
    /// 移出列表
    /// </summary>
    /// <param name="strName"></param>
    /// <returns></returns>
    bool RemoveHideObjByName(string strName)
    {
        for (int i = 0; i < moCurHide.Count; i++)
        {
            if (strName == moCurHide[i].name)
            {
                moCurHide.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}