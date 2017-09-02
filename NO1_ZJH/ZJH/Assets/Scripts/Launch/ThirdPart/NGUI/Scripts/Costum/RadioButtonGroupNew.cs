using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// RadioButtonNew 组
/// Author : cyj
/// Date : 2016/03/22
/// </summary>
public class RadioButtonGroupNew : MonoBehaviour
{
    //按钮组
    public List<UISprite> list = new List<UISprite>();

    private struct LabelStyle
    {
        public int iFontSize;
        public Color cFontColor;
        public int iWidth;
        public int iHeight;
    }
    protected struct SpriteStyle
    {
        public int iSpriteWidth;
        public int iSpriteHeight;
        public string strSpriteName;
    }
    private LabelStyle labelChoose;
    private LabelStyle labelNotChoose;

    private SpriteStyle spriteSelect;
    private SpriteStyle spriteUnselect;

    /// <summary>
    /// 没被选中字体的位置
    /// </summary>
    private Vector3 chooseVec;
    /// <summary>
    /// 被选中字体的位置
    /// </summary>
    private Vector3 notChooseVec; 
   
    //按钮位置
    public int miPianyi = 5;

    public string mstrBottonUp = string.Empty;
    public string mstrBottonDown =  string.Empty;

    public delegate void RadioButtonDelagate(GameObject button);
    private RadioButtonDelagate m_GroupChangeEvent;

    public RadioButtonDelagate GroupChangeEvent
    {
        set { m_GroupChangeEvent = value; }
    }

    public int m_selectedIndex = 0;

    private UISprite m_lastButton;

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
            UpdateGroup(m_selectedIndex);
        }
    }

    public void Reset()
    {
        m_selectedIndex = -1;
        for (int i = 0; i < list.Count; i++)
        {
            UISprite button = list[i];
            //button.isEnabled = (m_selectedIndex != i);
        }
    }

    public void Init()
    {  
        for (int i = 0; i < list.Count; i++)
        {
            UISprite button = list[i];
            if (button == null)
            {
                continue;
            } 
            UILabel temp = button.GetComponentInChildren<UILabel>();
            if (i == 0)
            { 
                spriteSelect.strSpriteName = button.spriteName;
                spriteSelect.iSpriteHeight = button.height;
                spriteSelect.iSpriteWidth = button.width;
                if (null != temp)
                {
                    chooseVec = temp.transform.localPosition;
                    labelChoose.iFontSize = temp.fontSize;
                    labelChoose.cFontColor = temp.color;
                    labelChoose.iWidth = temp.width;
                    labelChoose.iHeight = temp.height;
                }
                m_lastButton = list[0];
            }
            else if (i == 1)
            {
                spriteUnselect.strSpriteName = button.spriteName;
                spriteUnselect.iSpriteHeight = button.height;
                spriteUnselect.iSpriteWidth = button.width;
                labelNotChoose.iFontSize = temp.fontSize;
                notChooseVec = temp.transform.localPosition;
                labelNotChoose.cFontColor = temp.color;
                labelNotChoose.iWidth = temp.width;
                labelNotChoose.iHeight = temp.height;
            }
            UIEventListener.Get(button.gameObject).onClick = OnChangeEvent;
        }
        ResetButtonPostion(0);
        ResetWordScale();
        ResetWordColor();
    }

    private void ResetWordColor()
    {
         
    }

    private void ResetWordScale()
    {
         
    }

    public GameObject SelectedItem
    {
        get
        {
            return list[m_selectedIndex].gameObject;
        }
        set
        {
            int index = list.IndexOf(value.GetComponent<UISprite>());
            if (index > -1)
                m_selectedIndex = index;

            UpdateGroup(m_selectedIndex);
        }
    }

    private void OnChangeEvent(GameObject go)
    {
        UISprite button = go.GetComponent<UISprite>(); 

        UpdateGroup(list.IndexOf(button));
    }

    private void UpdateGroup(int index)
    {
        m_selectedIndex = index;
        int iLastIndex = 0;
        if (m_lastButton != null)
        { 
            SetSprite(m_lastButton, spriteUnselect); 
            iLastIndex = list.IndexOf(m_lastButton);
            UILabel labelOld = m_lastButton.GetComponentInChildren<UILabel>();
            if (null != labelOld)
            {
                labelOld.color = labelNotChoose.cFontColor;
                labelOld.fontSize = labelNotChoose.iFontSize;
                labelOld.transform.localPosition = notChooseVec;
                labelOld.width = labelNotChoose.iWidth;
                labelOld.height = labelNotChoose.iHeight;
            }
        }

        UILabel labelNow = list[m_selectedIndex].GetComponentInChildren<UILabel>();
        if (null != labelNow)
        {
            labelNow.color = labelChoose.cFontColor;
            labelNow.fontSize = labelChoose.iFontSize;
            labelNow.transform.localPosition = chooseVec;
            labelNow.width = labelChoose.iWidth;
            labelNow.height = labelChoose.iHeight;
        }
        m_lastButton = list[m_selectedIndex];
         
        SetSprite(m_lastButton, spriteSelect);

        ResetButtonPostion(index);

        if (m_GroupChangeEvent != null && m_lastButton != null)
            m_GroupChangeEvent(m_lastButton.gameObject);
    }


    protected void SetSprite(UISprite sprite, SpriteStyle sp)
    {
        if (sprite != null)
        {
            sprite.spriteName = sp.strSpriteName;
            sprite.width = sp.iSpriteWidth;
            sprite.height = sp.iSpriteHeight;
            //sprite.MakePixelPerfect();
        }
    }

    /// <summary>
    /// 重置按钮位置
    /// </summary>
    /// <param name="iIndex"></param>
    public void ResetButtonPostion(int index)
    {
        float fTemp = 0;
        for (int i = 0; i < list.Count; ++i )
        {
            if (list[i] == null)
            {
                continue;
            }
            list[i].depth = 2;
            if (i == 0)
            {
                fTemp = list[i].transform.localPosition.y - list[i].height;
            }
            else
            {
                fTemp -= miPianyi;
                list[i].transform.localPosition = new Vector3(list[i].transform.localPosition.x, fTemp, 0);
                fTemp = fTemp - list[i].height;
            }
        }
        if (index > -1 && index < list.Count)
        {
            list[index].depth = 3;
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
    }
    #endregion;
}
