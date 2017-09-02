using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏质量
/// </summary>
public enum GameQuality
{
    /// <summary>
    /// 极简
    /// </summary>
    SUPER_LOW = 1,
    /// <summary>
    /// 低
    /// </summary>
    LOW = 2,
    /// <summary>
    /// 中
    /// </summary>
    MIDDLE = 3,
    /// <summary>
    /// 高
    /// </summary>
    HIGH = 4
};
/// <summary>
/// 高 中 低配 极简
/// </summary>
public class EffectLevel : MonoBehaviour
{
    /// <summary>
    /// 默认低配
    /// </summary>
    public static GameQuality iEffectLevel = GameQuality.LOW;
    public static void SetEffectLevel(GameQuality level)
    {
        if (iEffectLevel != level)
        {
            iEffectLevel = level;
            NoticeLevelChange(iEffectLevel);
        }
    }

    /// <summary>
    /// 变化
    /// </summary>
    /// <param name="ilevel"></param>
    static void NoticeLevelChange(GameQuality level)
    {
        for (int i = 0; i < mGoList.Count; i++)
        {
            EffectLevel eLevel = mGoList[i];
            if (eLevel != null)
            {
                eLevel.LevelChange(level);
            }
        }
    }
    static List<EffectLevel> mGoList = new List<EffectLevel>();
    public static void AddGoList(EffectLevel eLevel)
    {
        if (eLevel != null)
            mGoList.Add(eLevel);
    }

    public static void ClearList()
    {
        mGoList.Clear();
    }

    public static void RemoveGoList(EffectLevel eLevel)
    {
        if (eLevel != null && mGoList.Contains(eLevel))
        {
            mGoList.Remove(eLevel);
        }
    }

    /// <summary>
    /// 高
    /// </summary>
    public bool mbHigh = false;

    /// <summary>
    /// 中
    /// </summary>
    public bool mbMiddle = false;

    /// <summary>
    /// 低
    /// </summary>
    public bool mbLow = false;

    /// <summary>
    /// 级科
    /// </summary>
    public bool mbMinimalism = false;

    /// <summary>
    /// 自身的显示状态
    /// </summary>
    protected bool mbActive = false;

    /// <summary>
    /// 显示是添加到列表
    /// </summary>
    void OnEnable()
    {
        AddGoList(this);
        SetActive(GetActiveGameQuality(SystemSetting.ImageQuality), true);
    }

    /// <summary>
    /// 隐藏时删除列表
    /// </summary>
    void OnDisable()
    {
        RemoveGoList(this);
        //当前为显示时，设为隐藏
        SetActive(false, false);
    }

    /// <summary>
    /// 级别变化
    /// </summary>
    /// <param name="ieLevel"></param>
    private void LevelChange(GameQuality level)
    {
        SetActive(GetActiveGameQuality(level), true);
    }

    /// <summary>
    /// 依据高中低配置判断是否可显示
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private bool GetActiveGameQuality(GameQuality level)
    {
        bool bShow = false;
        switch (level)
        {
            case GameQuality.SUPER_LOW://极简
                if (mbMinimalism)
                    bShow = true;
                break;
            case GameQuality.LOW://低
                if (mbLow)
                    bShow = true;
                break;
            case GameQuality.MIDDLE://中
                if (mbMiddle)
                    bShow = true;
                break;
            case GameQuality.HIGH://高
                if (mbHigh)
                    bShow = true;
                break;
        }
        return bShow;
    }

    protected virtual void OnSetActive(bool bShow, bool bInitiative)
    {

    }

    /// <summary>
    /// 设置显示隐藏
    /// </summary>
    /// <param name="bShow"></param>
    /// <param name="bInitiative">是高中低设置隐藏，还是被隐藏</param>
    private void SetActive(bool bShow, bool bInitiative)
    {
        if (mbActive == bShow)
            return;

        mbActive = bShow;
        OnSetActive(mbActive, bInitiative);
    }
}
