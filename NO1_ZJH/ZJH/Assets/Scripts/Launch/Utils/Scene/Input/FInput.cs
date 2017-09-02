using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;

/***********************************************************************
 * 功能 ： 用户输入监听器
 ***********************************************************************/

public class FInput : MonoBehaviour
{
    static public FInput ins;

    private Hashtable _uiMap = new Hashtable();
    private Hashtable _onDownKeys = new Hashtable();
    private int _onDownKeysCount = 0;

    void Awake()
    {
        ins = this;
    }

    void Update()
    {
        
    }

    public void LateUpdate()
    {
        Reset();
    }

    /***********************************************************************************
     * 功能 : 监听按键按下操作, 按键绑定器发送
     ***********************************************************************************/
    void OnKeyDown(UIKeyCode keyCode)
    {
        _onDownKeys.Add(keyCode, true);
        _onDownKeysCount++;
    }

    void OnKeyBinding(KeyBinding binder)
    {
        if (_uiMap.ContainsKey(binder.keyCode))
            _uiMap[binder] = binder;
        else
            _uiMap.Add(binder.keyCode, binder);
    }

    void OnKeyRemove(KeyBinding binder)
    {
        if (_uiMap.ContainsKey(binder.keyCode))
            _uiMap.Remove(binder.keyCode);
        else
            return;
    }

    void Reset()
    {
        // 清理按键记录
        _onDownKeys.Clear();
        _onDownKeysCount = 0;
    }

    /** 监听按钮是否被点击 */
    public bool GetKeyDown(UIKeyCode keyCode)
    {
        return _onDownKeys.ContainsKey(keyCode);
    }

}