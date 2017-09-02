/// 作者 zhangrj
/// 日期 20141016
/// 实现目标  显示艺术数字
/// 用途 暴击伤害及面板数字显示
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;

/// <summary>
/// 数字Label
/// 用法 单纯数字直接调用Text 
/// flag标签提供三种艺术字
/// 直接调用WriteConstomNumText(string)
/// </summary>
[AddComponentMenu("NGUI/Custom/NumLabel")]
public class UINumLabel : UILabel
{
    /// <summary>
    /// Label支持的格式
    /// </summary>
    const string SupportPattern = @"^[0-9|+|-|=|*|/|%|.|:|{|}|<|>|~|^|`|#| ]+$";
    const string checkNum = "\\d\\d*";


    private MatchCollection matchs;

    /// <summary>
    /// 判断该内容是否符合要求
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    bool CanWrite(string str)
    {
        if (Regex.IsMatch(str, SupportPattern))
            return true;
        return false;
    }

    /// <summary>
    /// 特殊字体
    /// </summary>
    /// <param name="content"></param>
    public void SpecialText(string content)
    {
        base.text = content;
    }

    public void WriteConstomNumText(string str, string flag, string postfix = "")
    {
        for (int i = 0; i < str.Length; i++)
        {
            char ch = str[i];
            if (char.IsNumber(ch) || ch == '+' || ch == '-' || ch == '=')
            {
                str = str.Insert(i, flag);
                i += flag.Length;
            }
        }
        this.text = str;
    }
}
