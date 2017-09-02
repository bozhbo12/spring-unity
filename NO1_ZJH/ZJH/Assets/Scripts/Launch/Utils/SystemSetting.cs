using UnityEngine;

/// <summary>
/// 系统设置信息
/// </summary>
public class SystemSetting
{
    #region 同屏人数

    public const int iScreenCountHeight = 30;
    public const int iScreenCountMIDDLE = 20;
    public const int iScreenCountLow = 15;
    public const int iScreenCountSuperLow = 8;

    /// <summary>
    /// 最大同屏人数
    /// </summary>
    public static int iScreenCountMax
    {
        get
        {
            return 40 - iScreenCountMin;
        }
    }

    /// <summary>
    /// 同屏最小人数
    /// </summary>
    public static int iScreenCountMin
    {
        get
        {
            return 8;
        }
    }

    /// <summary>
    /// 获取数值根据比例
    /// </summary>
    /// <param name="fRate"></param>
    /// <returns></returns>
    public static int GetScreenCountByRate(float fRate)
    {
        return Mathf.RoundToInt(fRate * iScreenCountMax) + iScreenCountMin;
    }

    /// <summary>
    /// 获取比例依据人物数据
    /// </summary>
    /// <param name="iCount"></param>
    /// <returns></returns>
    public static float GetScreenRateByCount(int iCount)
    {
        return (iCount - iScreenCountMin) / (iScreenCountMax * 1.0f);
    }

    #endregion

    private static string StrTZfirstRunGame = "MLfirstRunGame";
    private static string StrImageQuality = "ImageQuality";
    private static string StrSpeakVoice = "SpeakVoice";
    private static string StrFuseConfig = "FuseConfig";
    private static string StrMusic = "Music";
    private static string StrMusicVoice = "MusicVoice";
    private static string StrSound = "Sound";
    private static string StrSoundVoice = "SoundVoice";
    private static string StrSpeak = "Speak";
    private static string StrScreenCount = "ScreenCount";

    private static string StrPingBatteryTime = "PingBatteryTime";//for保存ping 电量 的开关设置   

	private static string BossView = "BossView";

    private static string CommonView = "CommonView";

    private static string StrBlockWorld = "BlockWorld"; //屏蔽世界频道信息
    private static string StrBlockNation = "BlockNation"; //屏蔽阵营频道信息
    private static string StrBlockGuild = "BlockGuild"; //屏蔽公会频道信息
    private static string StrBlockNearby = "BlockNearby"; //屏蔽附近频道信息
    private static string StrBlockTeam = "BlockTeam"; //屏蔽附近频道信息

    private static string StrAutoBloodValue = "AutoBloodValue";//自动吃药血量设置
    private static string StrAutoDrug = "AutoDrug";//自动吃药开关
    private static string StrAutoDrugItemId = "AutoDrugItemId"; //自动选择的血药id

    /// <summary>
    /// 高配推荐人数
    /// </summary>
    const string HIGHCOUNT = "HighCount";
    const string MIDDLECOUNT = "MiddleCount";
    const string LOWCOUNT = "LowCount";
    const string SUPERLOWCOUNT = "SuperLowCount";

    public enum SettingBtnsType
    {
        ROLE = 0,    //更换角色
        STUCK = 1,   //脱离卡死
        HELP = 2,   //游戏帮助
        NOTICE = 3,  //公告
        GIFT = 4,  //礼包兑换
        AUTODRUG = 5,  //自动吃药
        PLAYERINFO = 6, //用户信息
        REPEATKIT = 7,  //视频录制
        WECHAT = 8,   //微信分享
        CONTACT = 9,  //联系客服
        EXIT = 10,    //退出登陆
        LANGUAGE = 11,    //语言设置
        QRCODE=12, //扫码登陆
        KEYSET=13, //按键设置
    }
    public static void Clear()
    {
        imageQualityCallback.ClearCalls();
        musicCallback.ClearCalls();
        musicVoiceCallback.ClearCalls();
        soundCallback.ClearCalls();
        soundVoiceCallback.ClearCalls();
        speakCallback = null;
        screenCountCallback.ClearCalls();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="completeCallback"></param>
    public static void Init()
    {
        Clear();
        LoadDefaultConfig();
        EffectLevel.SetEffectLevel(SystemSetting.ImageQuality);
        GameInitQuality();
    }

    /// <summary>
    /// 第一次运行该游戏
    /// </summary>
    public static bool bFirstRun
    {
        get
        {
            return PlayerPrefs.GetInt(StrTZfirstRunGame, 1) == 1;
        }
        set
        {
            PlayerPrefs.SetInt(StrTZfirstRunGame, value ? 1 : 0);
        }
    }

    /// <summary>
    /// 一键熔炼设置选项;
    /// </summary>
    private static int fuseConfig;
    public static int FuseConfig
    {
        get { return fuseConfig; }
        set
        {
            fuseConfig = value;
            PlayerPrefs.SetInt(StrFuseConfig, fuseConfig);
        }
    }


    public static UserDelegate musicCallback = new UserDelegate();
    /// <summary>
    /// 游戏音乐
    /// </summary>
    private static bool music;
    public static bool Music
    {
        get { return music; }
        set
        {
            if (music == value)
                return;
            music = value;
            PlayerPrefs.SetInt(StrMusic, music ? 1 : 0);
            if (musicCallback != null)
            {
                VarStore varStore = VarStore.CreateVarStore();
                varStore += value;
                musicCallback.ExecuteCalls(varStore);
                varStore.Destroy();
            }
        }
    }

    public static UserDelegate musicVoiceCallback = new UserDelegate();
    /// <summary>
    /// 音乐大小
    /// </summary>
    private static float musicVoice;
    public static float MusicVoice
    {
        get { return musicVoice; }
        set
        {
            //LogSystem.Log("MusicVoice: ", value);
            if (value > 1 || value < 0)
                return;
            float offest = musicVoice - KeepFloat(value);
            if (offest > 0.09f || offest < -0.09f)
            {
                musicVoice = KeepFloat(value);
                PlayerPrefs.SetFloat(StrMusicVoice, musicVoice);
                if (musicVoiceCallback != null)
                {
                    VarStore varStore = VarStore.CreateVarStore();
                    varStore += musicVoice;
                    musicVoiceCallback.ExecuteCalls(varStore);
                    varStore.Destroy();
                }
            }
        }
    }

    public static UserDelegate soundCallback = new UserDelegate();
    /// <summary>
    /// 游戏音效
    /// </summary>
    private static bool sound;
    public static bool Sound
    {
        get { return sound; }
        set
        {
            if (sound != value)
            {
                sound = value;
                PlayerPrefs.SetInt(StrSound, sound ? 1 : 0);
                if (soundCallback != null)
                {
                    VarStore varStore = VarStore.CreateVarStore();
                    varStore += value;
                    soundCallback.ExecuteCalls(varStore);
                    varStore.Destroy();
                }
            }
        }
    }

    public static UserDelegate soundVoiceCallback = new UserDelegate();
    /// <summary>
    /// 音效大小
    /// </summary>
    private static float soundVoice;
    public static float SoundVoice
    {
        get { return soundVoice; }
        set
        {
            //LogSystem.Log("SoundVoice: ", value);
            if (value > 1 || value < 0)
                return;
            float offest = soundVoice - KeepFloat(value);
            if (offest > 0.09f || offest < -0.09f)
            {
                soundVoice = KeepFloat(value);
                PlayerPrefs.SetFloat(StrSoundVoice, soundVoice);
                if (soundVoiceCallback != null)
                {
                    VarStore varStore = VarStore.CreateVarStore();
                    varStore += soundVoice;
                    soundVoiceCallback.ExecuteCalls(varStore);
                    varStore.Destroy();
                }
            }
        }
    }
    /// <summary>
    /// 自动吃药血量设置
    /// </summary>
    private static float autoBloodValue;
    public static float AutoBloodValue
    {
        get { return autoBloodValue; }
        set
        {
            if (value > 1 || value < 0)
                return;
            //autoBloodValue = KeepFloat(value);
            autoBloodValue = value;
            PlayerPrefs.SetFloat(StrAutoBloodValue, autoBloodValue);

        }
    }
    public static System.Action<bool> autoDrugCallback = null;
    private static bool autoDrug;
    public static bool AutoDrug
    {
        get { return autoDrug; }
        set
        {
            if (autoDrug != value)
            {
                autoDrug = value;
                PlayerPrefs.SetInt(StrAutoDrug, autoDrug ? 1 : 0);
                if (autoDrugCallback != null)
                    autoDrugCallback(value);
            }
        }
    }
    private static string autoDrugItemId;
    public static string AutoDrugItemId
    {
        get { return autoDrugItemId; }
        set
        {
            if (!autoDrugItemId.Equals(value))
            {
                autoDrugItemId = value;
                PlayerPrefs.SetString(StrAutoDrugItemId, autoDrugItemId);
            }
        }

    }
    public static System.Action<bool> speakCallback = null;
    /// <summary>
    /// 语音
    /// </summary>
    private static bool speak;
    public static bool Speak
    {
        get { return speak; }
        set
        {
            if (speak != value)
            {
                speak = value;
                PlayerPrefs.SetInt(StrSpeak, speak ? 1 : 0);
                if (speakCallback != null)
                    speakCallback(value);
            }
        }
    }

    public static System.Action<bool> showPingCallback = null;//响应开关事件，是否显示左下角ping、电池、时间
    /// <summary>
    /// 左下角ping值显示
    /// </summary>
    private static bool bping_battery_time;
    public static bool BPing_battery_time
    {
        get { return bping_battery_time; }
        set
        {
            if (bping_battery_time != value)
            {
                bping_battery_time = value;
                PlayerPrefs.SetInt(StrPingBatteryTime, bping_battery_time ? 1 : 0);
                if (showPingCallback != null)
                    showPingCallback(value);
            }
        }
    }
    /// <summary>
    /// 屏蔽世界频道
    /// </summary>
    private static int iBlockWorld;
    public static bool BBlockWorld
    {
        get
        {
			iBlockWorld = PlayerPrefs.GetInt(StrBlockWorld, 0);
            return iBlockWorld == 1 ? true : false;
        }
        set
        {
            bool bblock = iBlockWorld == 1 ? true : false;
            if (bblock != value)
            {
				PlayerPrefs.SetInt(StrBlockWorld, value ? 1 : 0);
            }
        }
    }
    /// <summary>
    /// 屏蔽阵营频道
    /// </summary>
    private static int iBlockNation;
    public static bool BBlockNation
    {
        get
        {
			iBlockNation = PlayerPrefs.GetInt(StrBlockNation, 0);
            return iBlockNation == 1 ? true : false;
        }
        set
        {
            bool bblock = iBlockNation == 1 ? true : false;
            if (bblock != value)
            {
				PlayerPrefs.SetInt(StrBlockNation, value ? 1 : 0);
            }
        }
    }
    /// <summary>
    /// 屏蔽公会频道
    /// </summary>
    private static int iBlockGuild;
    public static bool BBlockGuild
    {
        get
        {
			iBlockGuild = PlayerPrefs.GetInt(StrBlockGuild, 0);
            return iBlockGuild == 1 ? true : false;
        }
        set
        {
            bool bblock = iBlockGuild == 1 ? true : false;
            if (bblock != value)
            {
				PlayerPrefs.SetInt(StrBlockGuild, value ? 1 : 0);
            }
        }
    }
    /// <summary>
    /// 屏蔽附近频道
    /// </summary>
    private static int iBlockNearby;
    public static bool BBlockNearby
    {
        get
        {
			iBlockNearby = PlayerPrefs.GetInt(StrBlockNearby, 0);
            return iBlockNearby == 1 ? true : false;
        }
        set
        {
            bool bblock = iBlockNearby == 1 ? true : false;
            if (bblock != value)
            {
				PlayerPrefs.SetInt(StrBlockNearby, value ? 1 : 0);
            }
        }
    }

    /// <summary>
    /// 屏蔽附近频道
    /// </summary>
    private static int iBlockTeam;
    public static bool BBlockTeam
    {
        get
        {
            iBlockTeam = PlayerPrefs.GetInt(StrBlockTeam, 1);
            return iBlockTeam == 1 ? true : false;
        }
        set
        {
            bool bblock = iBlockTeam == 1 ? true : false;
            if (bblock != value)
            {
                PlayerPrefs.SetInt(StrBlockTeam, value ? 1 : 0);
            }
        }
    }


    public static System.Action<float> speakVoiceCallback = null;
    /// <summary>
    /// 语音大小
    /// </summary>
    private static float speakVoice;
    public static float SpeakVoice
    {
        get { return speakVoice; }
        set
        {
            if (value > 1 || value < 0)
                return;
            float offest = speakVoice - KeepFloat(value);
            if (offest > 0.09f || offest < -0.09f)
            {
                speakVoice = KeepFloat(value);
                PlayerPrefs.SetFloat(StrSpeakVoice, speakVoice);
                if (speakVoiceCallback != null)
                    speakVoiceCallback(musicVoice);
            }
        }
    }
    public static UserDelegate imageQualityCallback = new UserDelegate();
    /// <summary>
    /// 游戏画质 高是3 中是2 低是1 极简0
    /// </summary>
    private static GameQuality mImageQuality;
    public static GameQuality ImageQuality
    {
        get { return mImageQuality; }
        set
        {
            if (mImageQuality != value)
            {
                mImageQuality = value;

                //手动设置也记录到临时变化里
                mOriginalQuality = value;

                PlayerPrefs.SetInt(StrImageQuality, (int)mImageQuality);
                VarStore varStore = VarStore.CreateVarStore();
                varStore += (int)mImageQuality;
                imageQualityCallback.ExecuteCalls(varStore);
                varStore.Destroy();
                UpdateQuality();
                //LogSystem.LogWarning(StrImageQuality, imageQuality);
            }
        }
    }

    /// <summary>
    /// 暂时用于边境事件开启记录原始的效果设置
    /// </summary>
    private static GameQuality mOriginalQuality;

    /// <summary>
    /// 多人游戏质量设置，不做持久化存储。只做临时改变
    /// </summary>
    public static GameQuality MultiplayerImageQuality
    {
        get
        {
            return mImageQuality;
        }
        set
        {
            if (mImageQuality != value)
            {
                mImageQuality = value;
                VarStore varStore = VarStore.CreateVarStore();
                varStore += (int)mImageQuality;
                imageQualityCallback.ExecuteCalls(varStore);
                varStore.Destroy();
                UpdateQuality();
                //LogSystem.LogWarning(StrImageQuality, imageQuality);
            }
        }
    }

    /// <summary>
    /// 多人降低游戏质量
    /// </summary>
    public static void DownMultiplayerImageQuality()
    {
        mOriginalQuality = MultiplayerImageQuality;

        int temp = (int)MultiplayerImageQuality;
        int temp2 = temp;
        temp = Mathf.Min(temp2 - 1, (int)GameQuality.HIGH);
        temp = Mathf.Max(temp2 - 1, (int)GameQuality.SUPER_LOW);



        
        MultiplayerImageQuality = (GameQuality)temp;
    }

    /// <summary>
    /// 多人还原游戏质量
    /// </summary>
    public static void ResetMultiplayerImageQuality()
    {
        int temp = (int)mOriginalQuality;
        int temp2 = temp;
        temp = Mathf.Min(temp2, (int)GameQuality.HIGH);
        temp = Mathf.Max(temp2, (int)GameQuality.SUPER_LOW);

        MultiplayerImageQuality = (GameQuality)temp;
    }

    public static UserDelegate screenCountCallback = new UserDelegate();
    /// <summary>
    /// 同屏数量
    /// </summary>
    private static int miScreenCount;
    public static int ScreenCount
    {
        get { return miScreenCount; }
        set
        {
            if (miScreenCount != value)
            {
                miScreenCount = value;
                PlayerPrefs.SetInt(StrScreenCount, miScreenCount);
                if (screenCountCallback != null)
                {
                    VarStore varStore = VarStore.CreateVarStore();
                    varStore += value;
                    screenCountCallback.ExecuteCalls(varStore);
                    varStore.Destroy();
                }
            }
        }
    }
    
    public static UserDelegate BossViewCountCallback = new UserDelegate();
    /// <summary>
    /// Boss 视角
    /// </summary>
	private static int openBossView = -1;
	public static int OpenBossView
	{
		get{
			if(openBossView == -1)
			{
				openBossView = PlayerPrefs.GetInt(BossView, 1);
			}
			return openBossView;
		}
		set
		{
			if (openBossView != value)
			{
				openBossView = value;
				PlayerPrefs.SetInt(BossView, openBossView);
                if (BossViewCountCallback != null) 
                {
                    VarStore varStore = VarStore.CreateVarStore();
                    varStore += openBossView;
                    BossViewCountCallback.ExecuteCalls(varStore);
                    varStore.Destroy();
                }
			}
		}
	}
	/// <summary>
	/// 返回Boss视角；
	/// </summary>
	/// <value><c>true</c> if get boss view; otherwise, <c>false</c>.</value>
	public static bool getBossView
	{
		get{
			return OpenBossView==1?true:false;
		}
	}

    /// <summary>
    /// 普通视角设定
    /// 1 = 3D
    /// 2=2.5D
    /// </summary>
    private static int commonView = -1;

    public static UserDelegate CommonViewCallback = new UserDelegate();

    public static int OpenCommonView
    {
        get
        {
            if (commonView == -1)
            {
                commonView = PlayerPrefs.GetInt(CommonView, 1);
            }
            return commonView;
        }
        set
        {
            if (commonView != value)
            {
                commonView = value;
                PlayerPrefs.SetInt(CommonView, commonView);
                if (CommonViewCallback != null)
                {
                    VarStore varStore = VarStore.CreateVarStore();
                    varStore += commonView;
                    CommonViewCallback.ExecuteCalls(varStore);
                    varStore.Destroy();
                }
            }
        }
    }
    /// <summary>
    /// 是否3D视角
    /// </summary>
    public static bool getCommonView 
    {
        get
        {
            return OpenCommonView == 1 ? true : false;
        }
    }

    /// <summary>
    /// 是否2.5D视角
    /// </summary>
    public static bool getCommonView2D
    {
        get
        {
            return OpenCommonView == 2 ? true : false;
        }
    }


    /// <summary>
    /// 更新游戏品质 
    /// </summary>
    static void UpdateQuality()
    {
        EffectLevel.SetEffectLevel(SystemSetting.ImageQuality);
        SystemSetting.ScreenCount = GetScreenCount(SystemSetting.ImageQuality);
        GameInitQuality();
    }

    /// <summary>
    /// 依据高中低获取人数数量
    /// </summary>
    /// <param name="quality"></param>
    /// <returns></returns>
    public static int GetScreenCount(GameQuality quality)
    {
        switch (quality)
        {
            case GameQuality.HIGH:
                return SystemSetting.iScreenCountHeight;
            case GameQuality.MIDDLE:
                return SystemSetting.iScreenCountMIDDLE;
            case GameQuality.LOW:
                return SystemSetting.iScreenCountLow;
            case GameQuality.SUPER_LOW:
                return SystemSetting.iScreenCountSuperLow;
        }
        return SystemSetting.iScreenCountMIDDLE;
    }

    public static void GameInitQuality()
    {
        if (SystemSetting.ImageQuality == GameQuality.HIGH)
        {
            QualitySettings.SetQualityLevel(3);
        }
        else if (SystemSetting.ImageQuality == GameQuality.MIDDLE)
        {
            QualitySettings.SetQualityLevel(2);
        }
        else if (SystemSetting.ImageQuality == GameQuality.LOW)
        {
            QualitySettings.SetQualityLevel(1);
        }
        else
        {
            QualitySettings.SetQualityLevel(0);
        }
    }

    /// <summary>
    /// 加载默认配置
    /// </summary>
    static void LoadDefaultConfig()
    {
        music = PlayerPrefs.GetInt(StrMusic, 1) == 1;
        musicVoice = PlayerPrefs.GetFloat(StrMusicVoice, 0.5f);
        sound = PlayerPrefs.GetInt(StrSound, 1) == 1;
        soundVoice = PlayerPrefs.GetFloat(StrSoundVoice, 0.5f);
        speak = PlayerPrefs.GetInt(StrSpeak, 1) == 1;
        speakVoice = PlayerPrefs.GetFloat(StrSpeakVoice, 0.5f);
        autoDrug = PlayerPrefs.GetInt(StrAutoDrug, 1) == 1;
        autoBloodValue = PlayerPrefs.GetFloat(StrAutoBloodValue, 0.3f);
        autoDrugItemId = PlayerPrefs.GetString(StrAutoDrugItemId, "Item6021001");
        mImageQuality = (GameQuality)PlayerPrefs.GetInt(StrImageQuality, (int)GameQuality.MIDDLE);
        mOriginalQuality = (GameQuality)PlayerPrefs.GetInt(StrImageQuality, (int)GameQuality.MIDDLE);
        miScreenCount = PlayerPrefs.GetInt(StrScreenCount, -1);
        if (miScreenCount == -1)
        {
            miScreenCount = GetScreenCount(ImageQuality);
        }
        fuseConfig = PlayerPrefs.GetInt(StrFuseConfig, 0);
        bping_battery_time = PlayerPrefs.GetInt(StrPingBatteryTime,1)==1;
    }
 
    static float KeepFloat(float f, int count = 1)
    {
        double b = System.Math.Round(f, count);
        return (float)b;
    }

}