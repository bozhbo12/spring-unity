using System.Text;
using UnityEngine;

/*****************************************************************************
 * 功能 ： 按键绑定器，将按键绑定在指定的按钮上
 *****************************************************************************/
public class KeyBinding : MonoBehaviour
{
    /** 定义交互动作 */
    public enum UIAction
    {
        PressAndClick,                  // 点击
        Select,                         // 选择
    }

    public UIKeyCode keyCode = UIKeyCode.None;
    public UIAction action = UIAction.PressAndClick;


    void Start()
    {
        FInput.ins.gameObject.SendMessage("OnKeyBinding", this);
    }

    void OnClick()
    {
        if (action == UIAction.PressAndClick)
        {
            FInput.ins.gameObject.SendMessage("OnKeyDown", keyCode);
        }
    }

}