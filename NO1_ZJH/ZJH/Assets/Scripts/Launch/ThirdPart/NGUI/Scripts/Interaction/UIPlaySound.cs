//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;

/// <summary>
/// Plays the specified sound.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Play Sound")]
public class UIPlaySound : MonoBehaviour
{
    public enum Trigger
    {
        OnClick,
        OnMouseOver,
        OnMouseOut,
        OnPress,
        OnRelease,
        Custom,
    }

    public enum SoundMode : int
    {
        NULL = 0,
        NormalClick = 1,    //普通点击声音
        ViewClosed = 2,         //界面关闭声音
        Login = 3,              //进游戏音效
        Award = 4,              //领取奖励按钮
        RANDOM = 5,             //随机
        SKILL = 6,              //技能点击
        Button = 7, //通用按下按键音效
        GameButton = 8,//进入游戏按钮
        ViewEject = 9,//通用界面弹出音效
        TipsEject = 10,//tips弹窗
        Dropdown = 11,//下拉菜单弹出
        Putaway = 12,//下拉菜单收起
        FunctionEject = 13,//右上角功能弹出
        FunctionIncome = 14,//右上角功能收进
        FunctionSwitch = 15,//功能页签切换
        ForGold = 16,//获得金币
        DressUp = 17,//换装
        Strengthen = 18,//强化
        JewelSet = 19,//宝石镶嵌
        JewelSynthesis = 20,//宝石合成
        EquipRemove = 21,//装备卸下
        Smelting = 22,//熔炼
        SkillUpgrade = 23,//技能
        SoulUpgrade = 24,//灵魂
        ScreenOPen = 25,//选择角色那边 开幕声音
        ScreenClose = 26,//选择角色那边 闭幕声音
        createshow_yijian = 27,//角色登场剑男
        AddMoney = 28,//角色增加金钱
        system_button_all_8 = 29,//人物选择
        system_button_all_9 = 30,//男女切换 
        createshow_yijian_1 = 31,////角色登场剑女
        createshow_bahuang = 32,////角色登场八男
        createshow_bahuang_1 = 33,////角色登场八女
        createshow_taiqing = 34,////角色登场杖男
        createshow_taiqing_1 = 35,////角色登场杖女
        Button_Arrange = 36,//背包整理
        Button_Bottle_of_HP = 37,//使用血瓶
        Button_Draw = 38,//拜会（抽奖）
        Button_Mix = 39,//熔炼
        Button_Strengthen = 40,//强化
        Button_Horse = 41,//坐骑
        Button_Country = 42,//国家
        Button_Skill = 43,//技能
        Button_TenDraw = 44,//拜会十次
        taskcomplete = 50,//任务完成
        scene_Fail = 51,//任务完成
        Arena_01 = 52,//倒计时
        system_remind_all_18 = 55,	//成就	
        system_remind_all_19 = 56,	//捐献	
        system_remind_all_20 = 57,	//钱箱开启	
        system_remind_all_21 = 58,	//新功能开启	
        system_remind_all_22 = 59,	//钻石箱子开启
        system_huoban_001 = 61,     //喝酒
        system_remind_shen = 62, //神装激活与升阶
        system_remind_001 = 89, //宠物激活
        system_remind_002 = 90, //宠物技能提升
        system_remind_003 = 91, //宠物进阶
        system_remind_004 = 92, //宠物升级
        system_remind_005 = 93, //宠物突破
        system_remind_006 = 94, //上下拖动宠物栏
        Playertalk_01_m = 101,//男杖出场
        Playertalk_01_f = 104,//女杖出场
        Playertalk_02_m = 102,
        Playertalk_02_f = 105,
        Playertalk_03_m = 103,
        Playertalk_03_f = 106,
		system_remind_all_30 = 107,//返回按钮
        system_remind_all_31 = 108,//活动进入按钮
		system_remind_all_32 = 109,//商店购买
        system_remind_all_33 = 110,//左切换
        system_remind_all_34 = 111,//点开头像
		system_remind_all_35 = 112,//军阶升级
        system_remind_all_36 = 113,//洗筋伐髓
        system_remind_all_37 = 114,//	熔炼
        system_remind_all_38 = 115,//	签到
        system_remind_all_39 = 116,//	获得新技能
        system_remind_all_40 = 117,//	铸造
		system_remind_all_41 = 118,//	整理
		Npctalk0007 = 124,
		Npctalk0008 = 125,
		Npctalk0009 = 126,
		Npctalk0010 = 127,
		Npctalk0011 = 128,
		Npctalk0012 = 129,
		Npctalk0013 = 130,
		Npctalk0014 = 131,
		Npctalk0015 = 132,
        system_remind_item_18 = 137,//限时仙灵 莲花的播放
		system_remind_all_43 =139,//猴1
		system_remind_all_44 =140,//猴2
		system_remind_all_45 =141,//猴3
		system_remind_all_50=146,//帮战buff特效
		system_remind_all_51=147,//帮战被击杀提醒
        GuildTrainingClick=333,//团练点击
        GuildTrainingBGM=334,//团练背景音乐
    }

    public int soundMode = 0;
    public AudioClip audioClip;
    public Trigger trigger = Trigger.OnClick;

    bool mIsOver = false;

#if UNITY_3_5
	public float volume = 1f;
	public float pitch = 1f;
#else
	[Range(0f, 1f)] public float volume = 1f;
	[Range(0f, 2f)] public float pitch = 1f;
#endif

    void OnHover(bool isOver)
    {
        if (trigger == Trigger.OnMouseOver)
        {
            if (mIsOver == isOver)
                return;
            mIsOver = isOver;
        }

        if (enabled && ((isOver && trigger == Trigger.OnMouseOver) || (!isOver && trigger == Trigger.OnMouseOut)))
        {
            if (soundMode != 0)
                NGUITools.PlaySound(soundMode);
            else
                NGUITools.PlaySound(audioClip, volume, pitch);
        }
    }

    void OnPress(bool isPressed)
    {
        if (trigger == Trigger.OnPress)
        {
            if (mIsOver == isPressed)
                return;
            mIsOver = isPressed;
        }

        if (enabled && ((isPressed && trigger == Trigger.OnPress) || (!isPressed && trigger == Trigger.OnRelease)))
        {
            if (soundMode != 0)
                NGUITools.PlaySound(soundMode);
            else
                NGUITools.PlaySound(audioClip, volume, pitch);
        }
    }

    void OnClick()
    {
        if (enabled && trigger == Trigger.OnClick)
        {
            if (soundMode != 0)
                NGUITools.PlaySound(soundMode);
            else
                NGUITools.PlaySound(audioClip, volume, pitch);
        }
    }

    void OnSelect(bool isSelected)
    {
        if (enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
            OnHover(isSelected);
    }

    public void Play()
    {
        if (soundMode != 0)
            NGUITools.PlaySound(soundMode);
        else
            NGUITools.PlaySound(audioClip, volume, pitch);
    }
}
