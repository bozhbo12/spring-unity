using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// RadioButton 组
/// Author : YangDan
/// Date : 2014/10/28
/// </summary>
public class RadioButtonGroup : MonoBehaviour
{
    public List<UIButton> list = new List<UIButton>();

    public Vector3 NormalLabColor = Vector3.zero;

    public Vector3 PressLabColor = Vector3.zero;

    public Vector3 btnLabel_selected;

    public Vector3 btnLabel_Unselected;


    public delegate void RadioButtonDelagate(GameObject button);
    private RadioButtonDelagate m_GroupChangeEvent;

    public RadioButtonDelagate GroupChangeEvent
    {
        set { m_GroupChangeEvent = value; }
    }

    public delegate void RadioButtonTowDelagate(GameObject prebutton,GameObject button);
    private RadioButtonTowDelagate m_GroupTowChangeEvent;
    public RadioButtonTowDelagate GroupTowChangeEvent
    {
        set { m_GroupTowChangeEvent = value; }
    }

    private int m_selectedIndex = -1;

    private UIButton m_lastButton;
    private UIButton m_preButton;

    public int SelectedIndex
    {
        get
        {
            return m_selectedIndex;
        }
        set
        {
            m_selectedIndex = Mathf.Min(value, list.Count - 1);
            m_selectedIndex = Mathf.Max(m_selectedIndex, 0);
            UpdateGroup();
        }
    }

    public void Reset()
    {
        m_selectedIndex = -1;
        for (int i = 0; i < list.Count; i++)
        {
            UIButton button = list[i];
            button.isEnabled = (m_selectedIndex != i);
        }
    }

    /// <summary>
    /// 设置文字默认颜色
    /// </summary>
    /// <param name="obj"></param>
    private void SetNormalColor(GameObject obj)
    {
        if (NormalLabColor != Vector3.zero)
        {
            UILabel label = obj.GetComponentInChildren<UILabel>();
            if (label != null)
            {
                Color temp = Color.white;
                temp.a = NormalLabColor.x / 255f;
                temp.g = NormalLabColor.y / 255f;
                temp.b = NormalLabColor.z / 255f;
                label.color = temp;
            }
        }
    }

    /// <summary>
    /// 设置文字选中颜色
    /// </summary>
    /// <param name="obj"></param>
    private void SetPressColor(GameObject obj)
    {
        if (PressLabColor != Vector3.zero)
        {
            UILabel label = obj.GetComponentInChildren<UILabel>();
            if (label != null)
            {
                Color temp = Color.white;
                temp.a = PressLabColor.x / 255f;
                temp.g = PressLabColor.y / 255f;
                temp.b = PressLabColor.z / 255f;
                label.color = temp;
            }
        }
    }

    public void Init()
    {
        for (int i = 0; i < list.Count; i++)
        {
            UIButton button = list[i];
            button.isEnabled = (m_selectedIndex != i);
            UIEventListener.Get(button.gameObject).onClick = OnChangeEvent;
            SetNormalColor(button.gameObject);
        }
    }

    public GameObject SelectedItem
    {
        get
        {
            return list[m_selectedIndex].gameObject;
        }
        set
        {
            int index = list.IndexOf(value.GetComponent<UIButton>());
            if (index > -1)
                m_selectedIndex = index;

            UpdateGroup();
        }
    }

    /// <summary>
    /// 设置选中框显示
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="state"></param>
    private void SetBoardVisiable(GameObject obj, bool state)
    {
        if (obj == null)
            return;

        GameObject board = GetChild("Select", obj);

        if (board != null)
        {
            board.SetActive(state);
        }
    }

    /// <summary>
    /// 查找子节点
    /// </summary>
    /// <param name="name"></param>
    /// <param name="go"></param>
    /// <returns></returns>
    public GameObject GetChild(string name, GameObject go)
    {
        if (go == null)
            return null;

        Transform child = go.transform.Find(name);
        if (child == null)
        {
            return null;
        }

        return child.gameObject;
    }

    private void OnChangeEvent(GameObject go)
    {
        UIButton button = go.GetComponent<UIButton>();
        m_selectedIndex = list.IndexOf(button);

        UpdateGroup();
    }

    private void UpdateGroup()
    {
        if (m_lastButton != null)
        {
            m_lastButton.isEnabled = true;
			m_preButton = m_lastButton;
            SetBoardVisiable(m_lastButton.gameObject, false);
            SetNormalColor(m_lastButton.gameObject);
        }

        m_lastButton = list[m_selectedIndex];
        m_lastButton.isEnabled = false;
        SetBoardVisiable(m_lastButton.gameObject, true);
        SetPressColor(m_lastButton.gameObject);
        SetBtnLabelPos();
        if (m_GroupChangeEvent != null)
            m_GroupChangeEvent(m_lastButton.gameObject);

        if (m_GroupTowChangeEvent != null)
        {
            m_GroupTowChangeEvent(m_preButton != null ? m_preButton.gameObject : null, m_lastButton.gameObject);
        }
    }
    private void SetBtnLabelPos()
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].mBtnLabel == null) continue;
            if (i == m_selectedIndex)
            {
                list[i].mBtnLabel.transform.localPosition = btnLabel_selected;
            }
            else
            {
                list[i].mBtnLabel.transform.localPosition = btnLabel_Unselected;
            }
        }
    }
    #region 释放
    //add by chenfei 20150824
    void OnDestroy()
    {

        if (null != list)
        {
            list.Clear();
        }

        m_lastButton = null;
        m_preButton = null;
    }
    #endregion;
}
