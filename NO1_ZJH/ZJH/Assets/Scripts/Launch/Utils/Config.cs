/// 作者 wanglc
/// 日期 20140923
/// 实现目标  启动阶段相关参数配置

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 包含信息如下
/// 1，更新地址配置(固定配置)
/// 2，更新引导配置(取回后设置)
/// 3，资源列表信息(取回设置)
/// 4，服务器列表(取回设置)
/// </summary>

public class Config
{
    #region 结构

    /// <summary>
    /// 地址信息
    /// </summary>
    public class GameAddress
    {
        public string strID = string.Empty;
        public string strName = string.Empty;
        public string strDomainAddress = string.Empty;
        public string strIPAddress = string.Empty;
    }

    /// <summary>
    /// customInfo配置信息
    /// </summary>
    public class CustomInfo
    {
        public string strCustomInfoID = string.Empty;
        public string strCustomInfoVaule = string.Empty;
    }

    #region 区域

    /// <summary>
    /// 地区
    /// </summary>
    public enum EnumArea
    {
        None = 0,
        Area_China,
        /// <summary>
        /// 台湾
        /// </summary>
        Area_Taiwan,
        /// <summary>
        /// 北美
        /// </summary>
        Area_NorthAmerica,
        /// <summary>
        /// 韩国
        /// </summary>
        Area_Korea,
        /// <summary>
        /// 日本
        /// </summary>
        Area_Japan,
        /// <summary>
        /// 国际化
        /// </summary>
        Area_Internet,
    }
    public static bool bTaiWan
    {
        get
        {
            return Area == EnumArea.Area_Taiwan;
        }
    }

    public static bool bChina
    {
        get
        {
            return Area == EnumArea.Area_China;
        }
    }
    public static bool bInternational
    {
        get
        {
            return Area == EnumArea.Area_Internet;
        }
    }
    #endregion

	public delegate void CallLoadAsset(string strFileName, AssetCallback callback, VarStore varStore = null, bool bAsync = false);
    public static CallLoadAsset monLoadAsset = null;
    public static void SetLoadAssetCall(CallLoadAsset call)
    {
        monLoadAsset = call;
    }

    static Dictionary<string, UIAtlas> mAtlasTable = new Dictionary<string, UIAtlas>();
    public static UIAtlas GetAtlas(string strAtlasName)
    {
        UIAtlas uiAtals = null;
        mAtlasTable.TryGetValue(strAtlasName, out uiAtals);
        return uiAtals;
    }

    /// <summary>
    /// 是否包含图集
    /// </summary>
    /// <param name="strAtlasName"></param>
    /// <returns></returns>
    public static bool ContainsAtlas(UIAtlas uiAtlas)
    {
        return mAtlasTable.ContainsValue(uiAtlas);
    }

    /// <summary>
    /// 加载远程字体
    /// </summary>
    public static void LoadRemoteAtlas(OnActionCallback callback)
    {
        string strFileName = "Prefabs/UIAtlas/" + "/CommonUI";
        if (monLoadAsset != null)
        {
            monLoadAsset(strFileName, (oAsset, file, args) =>
            {
                if (oAsset != null)
                {
                    GameObject go = oAsset as GameObject;
                    UIAtlas atlas = go.GetComponent<UIAtlas>();
                    if (atlas != null)
                    {
                        mAtlasTable["CommonUI"] = atlas;
                    }
                }
                if (callback != null)
                    callback();
            }, null);
        }
    }

    public static void PreloadUIAtlas()
    {
        UIAtlas atlas = Config.LoadUIAtlas("CommonUI");
        if (!mAtlasTable.ContainsKey("CommonUI"))
        {
            mAtlasTable.Add("CommonUI", atlas);
        }
        else
        {
            mAtlasTable["CommonUI"] = atlas;
        }
    }

    static UIAtlas LoadUIAtlas(string strAtlasName)
    {
        if (Application.isPlaying)
        {
            string strFileName = "Prefabs/UIAtlas/" + strAtlasName;
            Object o = Resources.Load(strFileName);
            if (o != null)
            {
                GameObject go = o as GameObject;
                UIAtlas atlas = go.GetComponent<UIAtlas>();
                if (atlas != null)
                {
                    return atlas;
                }
            }
        }
        else
        {
            ///加载主字体
#if UNITY_EDITOR
            string strFileName = "Assets/Resources/Prefabs/UIAtlas/" + strAtlasName + ".prefab";
            Object o = UnityEditor.AssetDatabase.LoadMainAssetAtPath(strFileName);
            if (o != null)
            {
                GameObject go = o as GameObject;
                UIAtlas atlas = go.GetComponent<UIAtlas>();
                if (atlas != null)
                {
                    return atlas;
                }
            }
#endif
        }
        return null;
    }

    /// <summary>
    /// UI界面字体
    /// </summary>
    static UIFont snailUIFont = null;
    public static UIFont SnailUIFont
    {
        get
        {
            return snailUIFont;
        }
    }


    static Dictionary<string, UIFont> mFontsTable = new Dictionary<string, UIFont>();
    /// <summary>
    /// 获取当期字体
    /// </summary>
    /// <param name="strFontName"></param>
    /// <returns></returns>
    public static UIFont GetFont(string strFontName)
    {
        if (mFontsTable.ContainsKey(strFontName))
        {
            return mFontsTable[strFontName];
        }
        return null;
    }

    /// <summary>
    /// 加载远程字体
    /// </summary>
    public static void LoadRemoteFont(OnActionCallback callback)
    {
        string strFileName = "Fonts/" +  "/NGUIFont";
        if (monLoadAsset != null)
        {
            monLoadAsset(strFileName, (oAsset, file, args) =>
            {
                if (oAsset != null)
                {
                    GameObject go = oAsset as GameObject;
                    UIFont font = go.GetComponent<UIFont>();
                    if (font != null)
                    {
                        mFontsTable["NGUIFont"] = font;
                    }
                }
                if (callback != null)
                    callback();
            }, null);
        }
    }

    /// <summary>
    /// 加载UI字体
    /// </summary>
    public static void PreloadUIFont()
    {
        if (snailUIFont == null)
        {
            snailUIFont = Config.LoadUIFont("NGUIFont");
            if (!mFontsTable.ContainsKey("NGUIFont"))
            {
                mFontsTable.Add("NGUIFont", snailUIFont);
            }
            else
            {
                mFontsTable["NGUIFont"] = snailUIFont;
            }
        }
    }

    static UIFont LoadUIFont(string strFontName)
    {
        if (Application.isPlaying)
        {
            string strFileName = "Fonts/" + "/" + strFontName;
            Object o = Resources.Load(strFileName);
            if (o != null)
            {
                GameObject go = o as GameObject;
                UIFont font = go.GetComponent<UIFont>();
                if (font != null)
                {
                    return font;
                }
                else
                {
                    LogSystem.LogWarning("GetComponent UIFont Error!!");
                }
            }
        }
        else
        {
            ///加载主字体
#if UNITY_EDITOR
            string strFileName = "Assets/Resources/Fonts/" + "/" + strFontName + ".prefab";
            Object o = UnityEditor.AssetDatabase.LoadMainAssetAtPath(strFileName);
            if (o != null)
            {
                GameObject go = o as GameObject;
                UIFont font = go.GetComponent<UIFont>();
                if (font != null)
                {
                    return font;
                }
            }
            else
            {
                LogSystem.LogWarning(strFileName + " Load Error!!");
            }
#endif
        }
        return null;

    }

    /// <summary>
    /// 渠道type
    /// </summary>
    public enum EunmChannelType
    {
        NONE = 0,
        /// <summary>
        /// 苹果管方
        /// </summary>
        AppStore = 1,

        /// <summary>
        /// 酷派
        /// </summary>
        COOLPAD = 2,
    }


    /// <summary>
    /// 订单类型
    /// </summary>
    public enum EnumOrderType
    {
        /// <summary>
        /// 常用类型
        /// </summary>
        Order_General = 0,
        /// <summary>
        /// vivo
        /// </summary>
        Order_Vivo = 1,
        /// <summary>
        /// mz
        /// </summary>
        Order_Mz = 3,
    }
    #endregion

    #region 平台相关

    /// <summary>
    /// 是否是编辑器模式
    /// </summary>
    public static bool bEditor
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }
    /// <summary>
    /// 是否是安卓平台模式
    /// </summary>
    public static bool bAndroid
    {
        get
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }
    /// <summary>
    /// 是否是IOS平台模式
    /// </summary>
    public static bool bIPhone
    {
        get
        {
#if UNITY_IPHONE && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }
    public static bool bWin 
    {
        get
        {

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    public static bool bWinPhone
    {
        get
        {
#if UNITY_WP8 && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    public static bool bMac
    {
        get
        {
#if UNITY_STANDALONE_OSX
            return true;
#else
            return false;
#endif
        }
    }

    #region 区域相关设置
   
    
    /// <summary>
    /// 区域
    /// </summary>
    public static EnumArea Area
    {
        get
        {
#if Taiwan
        return EnumArea.Area_Taiwan;
#elif Europe
        return EnumArea.Area_Europe;
#elif Japan
        return EnumArea.Area_Japan;
#elif Korea
        return EnumArea.Area_Korea;
#elif SoutheastAsia
        return EnumArea.Area_SoutheastAsia;
#elif Internet
        return EnumArea.Area_Internet;
#else
        return EnumArea.Area_China;
#endif
        }
    }
    #endregion

    public static EnumVersion CurVersion
    {
        get
        {
#if GTDEV
            return EnumVersion.Dev;
#elif GTDES
            return EnumVersion.Des;
#elif GTQA
            return EnumVersion.Qa;
#elif GTOUT
            return EnumVersion.Out;
#else
            return EnumVersion.Dev;
#endif
        }
    }

    #endregion

    #region Google平台
    /// <summary>
    /// 是否是谷歌帐号
    /// </summary>
    public static bool bGoogleAccount = false;
    /// <summary>
    /// 谷歌默认账号标识
    /// </summary>
    private static string strGoogleDefaultAccountIdent = "#google_gmid";
    /// <summary>
    /// 谷歌帐号串标识
    /// </summary>
    public static string strGoogleAccountIdent = "#google_gmid";

    /// <summary>
    /// 设置谷歌账号
    /// </summary>
    /// <param name="validateString"></param>
    public static void SetGoogleAccountIdent(string validateString)
    {
        if (string.IsNullOrEmpty(validateString))
        {
            bGoogleAccount = false;
            return;
        }
        string ident = strGoogleDefaultAccountIdent;
        if (!string.IsNullOrEmpty(strGoogleAccountIdent))
        {
            ident = strGoogleAccountIdent;
        }
        bGoogleAccount = false;
        bGoogleAccount = validateString.Contains(ident);
    }

    /// <summary>
    /// 是否显示google成就
    /// </summary>
    public static bool bGoogleAchievement
    {
        get
        {
            if (bGoogleAccount && bAndroid)
                return true;
            else
                return false;
        }
    }

    #endregion

    #region 国际版本 国家特许区分



    #endregion



    #region FaceBook和VK相关,GameCenter,分享，邀请的相关配置



   



    /// <summary>
    /// 国际版GooglePlay应用下载链接
    /// </summary>
    public static string strGooglePlayDownLoadUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(GooglePlayDownLoadUrl, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// 国际版GooglePlay应用下载链接
    /// </summary>
    public static string strAppStoreDownLoadUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(AppStoreDownLoadUrl, ref strResult);
            LogSystem.LogWarning("strAppStoreDownLoadUrl========" + strResult);
            return strResult;
        }
    }


    /// <summary>
    /// 国际版点赞跳转地址
    /// </summary>
    public static string strLanguageClickPraise
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(LanguageClickPraise, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// 点赞跳转字典
    /// </summary>
    public static DictionaryEx<int, string> clickPraiseDic;

    /// <summary>
    /// 根据语言获取点赞跳转URL
    /// </summary>
    /// <returns></returns>
    public static string GetPraiseUrlByLanguage()
    {
        DictionaryEx<int, string> temp = GetClickPraiseDic();
        if (temp == null || temp.Count <= 0)
            return string.Empty;



        return string.Empty;
    }

    /// <summary>
    /// 获取点赞跳转字典
    /// </summary>
    /// <returns></returns>
    private static DictionaryEx<int, string> GetClickPraiseDic()
    {
        if (clickPraiseDic != null && clickPraiseDic.Count > 0)
            return clickPraiseDic;

        if (string.IsNullOrEmpty(strLanguageClickPraise))
            return null;
        
        string[] strSplit = strLanguageClickPraise.Split(SplitChars.chAnd);
        if (strSplit == null || strSplit.Length < 2)
            return null;
        
        int length = strSplit.Length;
        if (length % 2 == 1 && length > 0)
        {
            length = length - 1;
        }

        if (clickPraiseDic != null)
        {
            clickPraiseDic.Clear();
        }
        else
        {
            clickPraiseDic = new DictionaryEx<int, string>();
        }

        for (int k = 0; k < length / 2; k++)
        {
            if (clickPraiseDic.ContainsKey(IntParse(strSplit[k * 2])))
            {
                clickPraiseDic[IntParse(strSplit[k * 2])] = strSplit[k * 2 + 1];
            }
            else
            {
                clickPraiseDic.Add(IntParse(strSplit[k * 2]), strSplit[k * 2 + 1]);
            }
        }

        return clickPraiseDic;
    }
    /// <summary>
    /// String 强转 Int 时调用 默认返回 0
    /// 避免转换过程中包含空字符、以及非数字字符
    /// 导致程序报错
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int IntParse(string value, int defaultValue = 0)
    {
        if (string.IsNullOrEmpty(value))
        {
            return defaultValue;
        }
        value = value.Trim();
        int result;
        if (int.TryParse(value, out result))
        {
            return result;
        }
        return defaultValue;
    }
    #endregion


    public enum EnumVersion
    {
        None = 0,
        /// <summary>
        /// 开发
        /// </summary>
        Dev = 1,
        /// <summary>
        /// 策划
        /// </summary>
        Des = 2,
        /// <summary>
        /// 质检
        /// </summary>
        Qa = 3,
        /// <summary>
        /// 发布
        /// </summary>
        Out = 4,
    }


    #region CustomInfo配置Key


    public const string GAMEID_KEY = "gameID";

    public const string APPKEY_KEY = "appkey";

    //ios 充值用
    public const string APPID_KEY = "appid";
    public const string APPID_DEBUG_KEY = "appid_debug";

    //崩溃时上传手机信息的
    public const string ERRORLOGURL_KEY = "errorLogUrl";
    public const string ERRORLOGURL_DEBUG_KEY = "errorLogUrl_debug";

    //上传崩溃日志文件的
    public const string ERRORLOGURLID_KEY = "errorLogUrlID";
    public const string ERRORLOGURLID_DEBUG_KEY = "errorLogUrlID_debug";

    //dump采集上传崩溃日志数据用的
    public const string NATIVEERRORURL_KEY = "NativeErrorUrl";
    public const string NATIVEERRORURL_DEBUG_KEY = "NativeErrorUrl_debug";

    //新dump采集地址 安卓
    public const string NEWDUMPURL_KEY_ANDROID = "NewDumpUrl_Android";
    public const string NEWDUMPURL_DEBUG_KEY_ANDROID = "NewDumpUrl_debug_Android";
    //新dump采集地址 IOS
    public const string NEWDUMPURL_KEY_IOS = "NewDumpUrl_IOS";
    public const string NEWDUMPURL_DEBUG_KEY_IOS = "NewDumpUrl_debug_IOS";
    
    //客户端版本
    public const string CLIENTVERSION_KEY = "ClientVersion";
    //项目名称
    public const string PROJECTNAME_KEY = "ProjectName";

    public const string APPSTOREID_KEY = "AppStoreID";

    public const string ACCESSID_KEY = "accessID";

    public const string ACCESSPWD_KEY = "accessPwd";

    public const string SEED_KEY = "seed";


    public const string WEIXIN_LINK_URL_KEY = "Weixin_Link_Url";

    public const string ACT_URL_KEY = "ACT_Url";


    public const string WXANDROIDID_KEY = "wxAndroidId";

    public const string WXIOSID_KEY = "wxiOSId";

    //数据采集的服务器内外网地址
    public const string DATACOLLECTIONURL_KEY = "dataCollectionUrl";
    public const string DATACOLLECTIONURL_DEBUG_KEY = "dataCollectionUrl_debug";
    public const string DATACOLLECTIONURL_KEY_ABROAD = "dataCollectionUrl_abroad";

    //更新日志上传回调
    public const string UPDATELOGCALLBACK_KEY = "updateLogCallbackUrl";
    public const string UPDATELOGCALLBACK_DEBUG_KEY = "updateLogCallbackUrl_debug";
    /// <summary>
    /// 错误日志手机号开关
    /// </summary>
    public const string ERRORLOGPHONEOPEN_KEY = "ErrorLogPhoneOpen";

    /// <summary>
    /// 广告json地址
    /// </summary>
    public const string SHOWAPPADURL_KEY = "AppStoreAdUrl";

    /// <summary>
    /// 游戏广告json地址
    /// </summary>
    public const string GAMEADSURL_KEY = "GameAdsUrl_Key";

    /// <summary>
    /// 游戏分享开关，没配值默认或者0不开启分享，有值并且是1开启微信分享，2开启微博分享，3开启微信和微博分享，4开启Facebook或者VK只针对国际版
    /// </summary>
    public const string ShareSwitch_KEY = "ShareSwitch_KEY";

    /// <summary>
    /// 截屏分享开关，微信微博分享属于其他开关,1是开启0或者不配关闭
    /// </summary>
    public const string ShotSwitch_KEY = "ShotSwitch_KEY";

    public const string GAMEADSUPDATETAG = "GameAdsUpdateTag";

    public const string REGURL_KEY = "regUrl";

    public const string ACCESSTYPE_KEY = "accessType";


    public const string REGISTER_SERVER_URL_KEY = "Register_Server_Url";


    public const string CHANNELTYPE_KEY = "channeltype";

    public const string PUSHSERVICEIP_KEY = "pushServiceIP";

    public const string PUSHSERVICEPOST_KEY = "pushServicePOST";

    public const string CHANNELAPPSECRET_KEY = "channelAppSecret";

    /// <summary>
    /// 详情地址
    /// </summary>
    public const string DOWNLOADURL = "downLoadUrl";

    /// <summary>
    /// 统一开fps
    /// </summary>
    public const string FPSOPEN_KEY = "FPSOpen";

    /// <summary>
    /// 省电模式开关  关键字
    /// </summary>
    public const string BEOPENWINBRIGHTNESS = "BeOpenWinBrightness";

    /// <summary>
    /// 显示方式0全显示 1 显示角色 2显示场景
    /// </summary>
    public const string DISPLAYTYPE_KEY = "DisplayType";

    public const string QUALITY_SUPERLOW_KEY = "Quality_SuperLow";
    public const string QUALITY_LOW_KEY = "Quality_Low";
    public const string QUALITY_MIDDLE_KEY = "Quality_Middle";
    public const string QUALITY_HIGH_KEY = "Quality_High";

    /// <summary>
    /// 是否开启服务器性能采集
    /// </summary>
    public const string NETDUMPER_KEY = "NetDumper";

    /// <summary>
    /// 海外网页公告地址
    /// </summary>
    public const string OVERSEANOTICEURL_KEY = "OverSeaNoticeUrl";
    /// <summary>
    /// 悬浮窗开关
    /// </summary>
    public const string FloatMenuKey = "FloatMenuKey";
    /// <summary>
    /// 官网地址
    /// </summary>
    public const string OfficialUrlKey = "OfficialUrlKey";
    /// <summary>
    /// 论坛地址
    /// </summary>
    public const string FormUrlKey = "FormUrlKey";
    /// <summary>
    /// 联系客服邮箱地址
    /// </summary>
    public const string EmailUrlKey = "EmailUrlKey";
    /// <summary>
    /// 用户协议地址
    /// </summary>
    public const string AgreementUrlKey = "AgreementUrlKey";
    /// <summary>
    /// 会员规约地址
    /// </summary>
    public const string TreatyUrlKey = "TreatyUrlKey";
    /// <summary>
    /// 海外版点赞跳转地址，结构是1|url|2|url
    /// </summary>
    public const string LanguageClickPraise = "LanguageClickPraise";
    /// <summary>
    /// 海外版GooglePlay商店下载链接
    /// </summary>
    public const string GooglePlayDownLoadUrl = "GooglePlayDownLoadUrl";
    /// <summary>
    /// 海外版AppStore商店下载链接
    /// </summary>
    public const string AppStoreDownLoadUrl = "GooglePlayDownLoadUrl";
    #endregion


    #region ios充值－补单相关
    /// <summary>
    /// 玩家帐号
    /// </summary>
    public const string USER_ACCOUNT = "useraccount";
    /// <summary>
    /// 玩家名
    /// </summary>
    public const string USER_NAME = "user_name";
    /// <summary>
    /// 服务器id
    /// </summary>
    public const string SERVERID = "serverid";
    /// <summary>
    /// 游戏id
    /// </summary>
    public const string GAME_ID = "game_id";
    /// <summary>
    /// 回调地址
    /// </summary>
    public const string APPSTORE_URL = "appstore_url";

    #endregion

    #region ADS广告配置ID
    /// <summary>
    /// 回调地址
    /// </summary>
    public const string ADSKEY_URL = "appstore_url";
    #endregion

    #region 读取服务器CustomInfo信息
    public static string gameId
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.GAMEID_KEY, ref strResult);
            return strResult;
        }
    }
    public static string accessId
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.ACCESSID_KEY, ref strResult);
            return strResult;
        }
    }
    public static string accessPassword
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.ACCESSPWD_KEY, ref strResult);
            return strResult;
        }
    }
    public static string accessType
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.ACCESSTYPE_KEY, ref strResult);
            return strResult;
        }
    }
    public static string seed
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.SEED_KEY, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// 媒体id
    /// </summary>
    public static string mediaId = string.Empty;

    /// <summary>
    /// 崩溃时上传手机信息的地址
    /// </summary>
    public static string errorLogUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(bDebugModel ? Config.ERRORLOGURL_DEBUG_KEY : Config.ERRORLOGURL_KEY, ref strResult);
            return strResult;
        }
    }
    /// <summary>
    /// 数据采集初始化的服务器地址
    /// </summary>
    public static string dataCollectionUrl
    {
        get
        {
            string strResult = string.Empty;
            if (Config.bChina)
            {
                GetCustomInfoValue(bDebugModel ? Config.DATACOLLECTIONURL_DEBUG_KEY : Config.DATACOLLECTIONURL_KEY, ref strResult);
            }
            else
            {
                GetCustomInfoValue(Config.DATACOLLECTIONURL_KEY_ABROAD, ref strResult);
            }
            return strResult;
        }
    }
    
    /// <summary>
    /// 更新日志上传回调服务器地址
    /// </summary>
    public static string updateLogCallbackUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(bDebugModel ? Config.UPDATELOGCALLBACK_DEBUG_KEY : Config.UPDATELOGCALLBACK_KEY, ref strResult);
            return strResult;
        }
    }
    /// <summary>
    /// 上传崩溃日志文件的
    /// </summary>
    public static string errorLogUrlId
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(bDebugModel ? Config.ERRORLOGURLID_DEBUG_KEY : Config.ERRORLOGURLID_KEY, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// dump采集上传崩溃日志数据用的
    /// </summary>
    public static string NativeErrorUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(bDebugModel ? Config.NATIVEERRORURL_DEBUG_KEY : Config.NATIVEERRORURL_KEY, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// 新的dump采集地址 安卓
    /// </summary>
    public static string NewDumpUrl_Android
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(bDebugModel ? Config.NEWDUMPURL_DEBUG_KEY_ANDROID : Config.NEWDUMPURL_KEY_ANDROID, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// 新的dump采集地址 IOS
    /// </summary>
    public static string NewDumpUrl_IOS
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(bDebugModel ? Config.NEWDUMPURL_DEBUG_KEY_IOS : Config.NEWDUMPURL_KEY_IOS, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// U3D错误日志采集地址
    /// </summary>
    public static string U3DErrorUrl
    {
        get
        {
            string key = "U3DErrorUrl";
            if (bDebugModel)
            {
                key = "U3DErrorUrl_debug";
            }
            string strResult = string.Empty;
            GetCustomInfoValue(key, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// 当前客户端版本
    /// </summary>
    public static string ClientVersion
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.CLIENTVERSION_KEY, ref strResult);
            return strResult;
        }
    }
    /// <summary>
    /// 项目名称
    /// </summary>
    public static string ProjectName
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.PROJECTNAME_KEY, ref strResult);
            return strResult;
        }
    }

    public static string appid
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(bDebugModel ? Config.APPID_DEBUG_KEY : Config.APPID_KEY, ref strResult);
            return strResult;
        }
    }

    public static string appkey
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.APPKEY_KEY, ref strResult);
            return strResult;
        }
    }

    public static string registerServerUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.REGURL_KEY, ref strResult);
            return strResult;
        }
    }

    //错误日志手机号开关
    public static bool ErrorLogPhoneOpen
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.ERRORLOGPHONEOPEN_KEY, ref strResult);
            return strResult == "1" || strResult == "true";
        }
    }

    /// <summary>
    /// 是否开启服务器性能采集
    /// </summary>
    private static bool mbNetDumper
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.NETDUMPER_KEY, ref strResult);
            return strResult == "1";
        }
    }

    /// <summary>
    /// 广告功能未显示出来	游戏本地需设置广告地址，不能从服务器上读取，第二次运行才正常显示。(本地需要配个地址？？？？？)
    /// </summary>
    public static string strShowAppAdUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.SHOWAPPADURL_KEY, ref strResult);
            return GetResServerPath(true) + strResult;
        }
    }

    /// <summary>
    /// 广告开关
    /// </summary>
    public static bool bShowAppAdUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.SHOWAPPADURL_KEY, ref strResult);
            return !string.IsNullOrEmpty(strResult);
        }
    }

    /// <summary>
    /// 游戏中广告
    /// </summary>
    public static string strGameAdsUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(GAMEADSURL_KEY, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// 没配值默认或者0不开启分享，有值并且是1开启微信分享，2开启微博分享，3开启微信和微博分享，4开启Facebook或者VK只针对国际版
    /// </summary>
    public static string strShareOpen
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(ShareSwitch_KEY, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// 分享功能是否开启
    /// </summary>
    public static bool bShareOpen
    {
        get
        {
            if (strShareOpen == "1" || strShareOpen == "2" || strShareOpen == "3" || strShareOpen == "4")
                return true;
            else
                return false;
        }
    }

    /// <summary>
    /// 截屏分享开关，微信微博分享属于其他开关
    /// </summary>
    public static bool strShotOpen
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(ShotSwitch_KEY, ref strResult);
            if (string.IsNullOrEmpty(strResult))
                return false;
            else
            {
                if ("1".Equals(strResult))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    /// <summary>
    /// 游戏中广告升级标示
    /// </summary>
    public static string strGameAdsUpdate
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.GAMEADSUPDATETAG, ref strResult);
            return strResult;
        }
    }

    public static string wxAndroidId
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.WXANDROIDID_KEY, ref strResult);
            return strResult;
        }
    }

    public static string wxiOSId
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.WXIOSID_KEY, ref strResult);
            return strResult;
        }
    }

    public static string pushServiceIP
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.PUSHSERVICEIP_KEY, ref strResult);
            return strResult;
        }
    }
    public static string pushServicePOST
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.PUSHSERVICEPOST_KEY, ref strResult);
            return strResult;
        }
    }

    public static string channelAppSecret
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.CHANNELAPPSECRET_KEY, ref strResult);
            return strResult;
        }
    }
    public static string Weixin_Link_Url
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.WEIXIN_LINK_URL_KEY, ref strResult);
            return strResult;
        }
    }

    public static string act_Url
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.ACT_URL_KEY, ref strResult);
            return strResult;
        }
    }

    public static string AppStoreID
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.APPSTOREID_KEY, ref strResult);
            return strResult;
        }
    }

    public static string downLoadUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.DOWNLOADURL, ref strResult);
            return strResult;
        }
    }

    public static string overseaNoticeUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.OVERSEANOTICEURL_KEY, ref strResult);
            return strResult;
        }
    }

    public static bool bFloatMenuOpen
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(FloatMenuKey, ref strResult);
            return "1".Equals(strResult);
        }
    }

    //官网地址
    public static string officialUrl
    {
        get
        {
            string strResult = string.Empty;
            string strKey = string.Empty;
            GetCustomInfoValue(strKey, ref strResult);
            return strResult;
        }
    }

    //论坛地址
    public static string formUrl
    {
        get
        {
            string strResult = string.Empty;
            string strKey = string.Empty;
      		strKey = FormUrlKey;
            GetCustomInfoValue(strKey, ref strResult);
            return strResult;
        }
    }

    //联系客服邮箱地址
    public static string emailUrl
    {
        get
        {
            string strResult = string.Empty;
            string strKey = string.Empty;

            GetCustomInfoValue(strKey, ref strResult);
            return strResult;
        }
    }

    //用户协议地址
    public static string agreementUrl
    {
        get
        {
            string strResult = string.Empty;
            string strKey = string.Empty;
     
            GetCustomInfoValue(strKey, ref strResult);
            return strResult;
        }
    }

    //会员规约地址(日本)
    public static string treatyUrl
    {
        get
        {
            string strResult = string.Empty;
            string strKey = string.Empty;
   
            GetCustomInfoValue(strKey, ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// 是不是appStore
    /// </summary>
    public static bool bAppStore
    {
        get
        {
#if UNITY_STANDALONE_OSX
		    return true;
#endif
            return ChannelType == EunmChannelType.AppStore;
        }
    }

    /// <summary>
    /// 是否打开省电模式
    /// </summary>
    public static bool bOpenScreenBright
    {
        get
        {
            string strResult = string.Empty;
            if (!GetCustomInfoValue(Config.BEOPENWINBRIGHTNESS, ref strResult))
                return false;

            int iResult = 0;
            if (!int.TryParse(strResult, out iResult))
                return false;

            return (iResult > 0);
        }
    }

    /// <summary>
    /// 是否显示FPS
    /// </summary>
    public static bool bDisplayFps
    {
        get
        {
            string strResult = string.Empty;
            if (!GetCustomInfoValue(Config.FPSOPEN_KEY, ref strResult))
                return false;
            
            int iResult = 0;
            if(!int.TryParse(strResult, out iResult))
                return false;

            return (iResult&1) > 0;
        }
    }

    /// <summary>
    /// 是否写日志
    /// </summary>
    public static bool bWriteFps
    {
        get
        {
            string strResult = string.Empty;
            if (!GetCustomInfoValue(Config.FPSOPEN_KEY, ref strResult))
                return false;

            int iResult = 0;
            if (!int.TryParse(strResult, out iResult))
                return false;

            return (iResult & 2) > 0;
        }
    }

    private static int miDisplayType = -1;
    /// <summary>
    /// 显示方式0全显示 1 显示角色 2显示场景
    /// </summary>
    public static int iDisplayType
    {
        get
        {
            if (miDisplayType != -1)
            {
                return miDisplayType;
            }
            string strResult = string.Empty;
            if (GetCustomInfoValue(Config.DISPLAYTYPE_KEY, ref strResult))
            {
                miDisplayType = int.Parse(strResult);
            }
            else
            {
                miDisplayType = 0;
            }
            return miDisplayType;
        }
    }

    #endregion

    public const string ANYSDK = "ANYSDK";

    public const string LaunchName = "Launch";

    public const string MessageName = "MessageName";

    public const string GameName = "魔龙世界";

    public static int mSnailVoiceAppId = 2001;

    /// <summary>
    /// 订单类型
    /// </summary>
    public static EnumOrderType OrderType { get; private set; }

    /// <summary>
    /// 渠道类型
    /// </summary>
    public static EunmChannelType ChannelType { get; private set; }

    /// <summary>
    /// ios渠道ID
    /// </summary>
    public static string ChannelID { get; private set; }

    /// <summary>
    /// 服务器ID(以前采集用)
    /// </summary>
    public static string serverId = string.Empty;

    /// <summary>
    /// 服务器名称(以前采集用)
    /// </summary>
    public static string serverName = string.Empty;

    /// <summary>
    /// 断线重线判断用
    /// </summary>
    public static bool isFirstLogin = false;

    /// <summary>
    /// 以前天子采集用
    /// </summary>
    public static bool mbEnableCatcher = false;

    /// <summary>
    /// 是否领取奖励
    /// </summary>
    public static bool isPickGuildReward = false;

    /// <summary>
    /// 内外网模式
    /// </summary>
    public static bool bDebugModel = false;

    /// <summary>
    /// 是否开启分辨率适配
    /// </summary>
    public static bool bResolutionAdjuster = false;

    public static int bRenderSize = 1024;
    public static int bSuperLowerRenderSize = 512;
    /// <summary>
    /// 是否是anysdk
    /// </summary>
    public static bool bANYSDK = false;

    /// <summary>
    /// 是否开启服务器性能采集
    /// </summary>
    public static bool bNetDumper = false;

    /// <summary>
    /// 是否初始化失败（必要资源加载失败）（貌似现在无用）
    /// </summary>
    public static bool bGameInitFailed = false;

    /// <summary>
    /// 是否使用包系统加载
    /// </summary>
    public static bool mbDynamicRes = true;

    /// <summary>
    /// 资源包是否使用md5
    /// </summary>
    public static bool mbMd5 = false;

    /// <summary>
    /// 是否需要采集
    /// </summary>
    public static bool bNeedDataCollect = false;

    /// <summary>
    /// 设置是否开启苹果内置评论
    /// </summary>
    public static bool bisOpenCommentValue = true;

    /// <summary>
    /// 是否设置过平台配置信息 Config_Android,Config_Apple
    /// </summary>
    public static bool bSettedChannelConfig = false;

    /// <summary>
    /// 是否播放过CG(由于播放CG会触发断线重连，又会进入场景添加角色,会重新再播放视频,所以不能与LoginStage.isNewCreateRole共用)
    /// </summary>
    public static bool bCreateRolePlayedCG = false;

    /// <summary>
    /// 是否显示过广告
    /// </summary>
    public static bool bIsShowedAD = false;

    /// <summary>
    /// 渠道的唯一id, 混服中CustomInfo配置是同一份，但要区分不同的渠道，比如小米 android_mi
    /// </summary>
    public static string strChannelUniqueName = string.Empty;

    /// <summary>
    /// 渠道的唯一二维码串号
    /// </summary>
    public static string strChannelQRIMEI = string.Empty;

    /// <summary>
    /// 子包中配置 游戏的配置路径
    /// </summary>
    public static string strAdressId = string.Empty;

    /// <summary>
    /// 完整包名
    /// </summary>
    public static string BundleIdentifier = string.Empty;

    /// <summary>
    /// 手机型号
    /// </summary>
    public static string strPhoneType = string.Empty;

    /// <summary>
    /// 安装包更新地址(json文件 安卓与苹果共用)
    /// </summary>
    public static string ClientInstallUrl
    {
        get;
        private set;
    }

    /// <summary>
    /// 充值回调地址
    /// </summary>
    public static string payCallBackURL
    {
        get;
        private set;
    }

    /// <summary>
    /// 公告文件
    /// </summary>
    public static string strNoticeName
    {
        get;
        private set;
    }

    /// <summary>
    /// 本地所有服务器文件名（当状态服获取不到，取该服务器列表）
    /// </summary>
    public static string strServerName
    {
        get;
        private set;
    }

    /// <summary>
    /// 商品文件配置名称
    /// </summary>
    public static string strShopItemFullName
    {
        get;
        private set;
    }

    /// <summary>
    /// 采集用的渠道Name
    /// </summary>
    public static string strChannelName
    {
        get;
        private set;
    }

    

    /// <summary>
    /// 商店数据
    /// </summary>
    public static string mstrShopData = string.Empty;

    /// <summary>
    /// 精彩活动与公告
    /// </summary>
    public static string mstrServerNotice = string.Empty;


    /// <summary>
    /// 反外挂信息
    /// </summary>
    public static Dictionary<string, List<string>> accInfo = new Dictionary<string, List<string>>();

    /// <summary>
    /// 服务器地址信息
    /// </summary>
    private static Dictionary<string, GameAddress> mUpdaterAddress = new Dictionary<string, GameAddress>();

    /// <summary>
    /// 更新引导参数设置(平台相关配置Config_Android||Config_Apple)
    /// </summary>
    private static Dictionary<string, Dictionary<string, string>> mDictUpdaterGuide = new Dictionary<string, Dictionary<string, string>>();

    /// <summary>
    /// 服务器列表
    /// </summary>
    public static List<Dictionary<string, string>> mDictServerList = new List<Dictionary<string, string>>();

    /// <summary>
    /// 所有的服务器列表(用于跨服战显示服务器名称)
    /// </summary>
    public static List<Dictionary<string, string>> mDictAllServerList = new List<Dictionary<string, string>>();

    /// <summary>
    /// CustomInfo.xml　Resource信息
    /// </summary>
    private static Dictionary<string, CustomInfo> mDictCustomInfoList = new Dictionary<string, CustomInfo>();

    /// <summary>
    /// 文言表
    /// </summary>
    private static Dictionary<string, string> mWordsDict = new Dictionary<string, string>();

    // 是否是 市场PC包 并且用.bat文件启动 携带相关参数
    public static bool isSCLimitEffect = false;

    public static string mstrPreSuffix = "File://";
    public static string mstrStreamSuffix = "File://";
    /// <summary>
    /// 本地资源根目录
    /// </summary>
    public static string mstrAssetBundleRootPath = string.Empty;

    /// <summary>
    /// 拷贝资源根目录
    /// </summary>
    //public static string mstrSourceResRootPath = string.Empty;

    /// <summary>
    /// 本地资源StreamingAsset根目录
    /// </summary>
    public static string mstrStreamResRootPath = string.Empty;

    ///是否是评审版本客户端
    public static bool mbVerifyVersion
    {
        get
        {
            if (string.IsNullOrEmpty(mstrLocalVersion))
                return false;

            string remoteVersion = GetUpdaterConfig("VerifyVerison", "Value");
            if (string.IsNullOrEmpty(remoteVersion))
                return false;

            return mstrLocalVersion == remoteVersion;
        }
    }

    /// <summary>
    /// 当前设备的mac地址
    /// </summary>
    private static string mstrDeviceCid = string.Empty;

    /// <summary>
    /// 获取cid
    /// 目的避免重复从sdk取值
    /// </summary>
    public static string strDeviceCid
    {
        get
        {
            if (string.IsNullOrEmpty(mstrDeviceCid))
                LogSystem.LogWarning("Config::mstrDeviceCid is empty!!");

            return mstrDeviceCid;
        }
    }

    public static void SetDeviceCid(string strCid)
    {
        LogSystem.LogWarning("Config::strCid:" + strCid);
        mstrDeviceCid = strCid;
    }
    //当前设备的idfa
    private static string mstrDeviceIDFA = string.Empty;
    public static string strDeviceIDFA
    {
        get
        {
            //if (string.IsNullOrEmpty(mstrDeviceIDFA))
            //{
            //    LogSystem.LogWarning("Config::mstrDeviceIDFA is empty!!");
            //}
            return mstrDeviceIDFA;
        }
    }
    public static void SetDeviceIDFA(string strIDFA)
    {
        LogSystem.LogWarning("Config::strIDFA:" + strIDFA);
        mstrDeviceIDFA = strIDFA;
    }
    /// <summary>
    /// 设置平台流加载前缀
    /// </summary>
    /// <returns></returns>
    public static string GetPreSuffix()
    {
        return mstrPreSuffix;
    }
    public static void SetPreSuffix(string strSuffix)
    {
        mstrPreSuffix = strSuffix;
    }
    /// <summary>
    /// 设置平台流加载前缀
    /// </summary>
    /// <returns></returns>
    public static string GetStreamSuffix()
    {
        return mstrStreamSuffix;
    }
    public static void SetStreamSuffix(string strSuffix)
    {
        mstrStreamSuffix = strSuffix;
    }

    #region 获取服务器地址

    public static string GetPlatform
    {
        get
        {
            if(Config.bIPhone)
            {
                return "_Ios";
            }
            else
            {
                return "_Android";
            }
        }
    }

    public static string GetVersion
    {
        get
        {
            switch (CurVersion)
            {
                case EnumVersion.Dev:
                    return "Dev";
                case EnumVersion.Des:
                    return "Des";
                case EnumVersion.Qa:
                    return "Qa";
                case EnumVersion.Out:
                    return "Out";
            }
            return string.Empty;
        }
    }

    public static string GetArea
    {
        get
        {
            switch (Area)
            {
                case EnumArea.Area_China:
                    return "_China";
                case EnumArea.Area_Taiwan:
                    return "_Taiwan";
                case EnumArea.Area_Japan:
                    return "_Japan";
                case EnumArea.Area_Korea:
                    return "_Korea";
                case EnumArea.Area_Internet:
                    return "_Internet";
            }
            return string.Empty;
        }
    }

	

    public static string GetHttps
    {
        get
        {
            return string.Empty;
        }
    }

	//得到pc端类型（pc外网包分为安卓和ios两种）
	public static string GetPcStyle
	{
		get
		{
			#if AndroidPC
			return "_AndroidPC";
			#elif IosPC
			return "_IosPC";
			#endif
			return string.Empty;
		}
	}

    /// <summary>
    /// 地址
    /// </summary>
    private static GameAddress mGameAddress = null;

    /// <summary>
    /// 返回更新服务器地址，如果有多个，随机一个，无为空
    /// </summary>
    /// <returns>更新服务器地址</returns>
    public static GameAddress GetUpdaterAddress()
    {
        if (mGameAddress != null)
            return mGameAddress;

        if (mUpdaterAddress == null || mUpdaterAddress.Count == 0)
        {
            return null;
        }

        string strAddressID = strAdressId;
        if (string.IsNullOrEmpty(strAddressID))
        {
            if (Config.bWin && GetAdressId(System.Environment.GetCommandLineArgs(), ref strAdressId))
            {
                strAddressID = strAdressId;
            }
            else
            {
				strAddressID = GetVersion + GetArea + GetHttps + GetPcStyle;
            }
        }

        if (mUpdaterAddress.ContainsKey(strAddressID))
        {
            mGameAddress = mUpdaterAddress[strAddressID];
            //找到地址后，清除字典
            mUpdaterAddress.Clear();
            mUpdaterAddress = null;
            return mGameAddress;
        }
        return mGameAddress;
    }

    /// <summary>
    /// 获取命令行输入的地址
    /// </summary>
    /// <param name="strAdressId"></param>
    /// <returns></returns>
    private static bool GetAdressId(string[] args, ref string strAdressId)
    {
        LogSystem.LogWarning("GetAdressId");
        if (args == null || args.Length == 0)
            return false;

        string RexExp_Validate = @"EffectLimitBegin.+EffectLimitEnd";
        getMatchCollectionString(args, RexExp_Validate);
        System.Text.RegularExpressions.MatchCollection replaceArray = System.Text.RegularExpressions.Regex.Matches(string.Join(";", args), RexExp_Validate);

        if (null != replaceArray && replaceArray.Count > 0)
        {
            isSCLimitEffect = true;
        }

        RexExp_Validate = @"AdressBegin.+AdressEnd";
        getMatchCollectionString(args, RexExp_Validate);
        replaceArray = System.Text.RegularExpressions.Regex.Matches(string.Join(";", args), RexExp_Validate);

        if (null != replaceArray && replaceArray.Count > 0)
        {
            LogSystem.LogWarning("Adress:", replaceArray[0].Value);
            string[] param = replaceArray[0].Value.Split(';');
            if (param.Length < 3)
                return false;

            strAdressId = param[1]; //获取地址
            return true;
        }

        return false;
    }

    private static System.Text.RegularExpressions.MatchCollection getMatchCollectionString(string[] args, string markString)
    {
        return System.Text.RegularExpressions.Regex.Matches(string.Join(";", args), markString);
    }

    /// <summary>
    /// 获取平台Config地址如Config_Android
    /// </summary>
    /// <param name="bDomainAddress"></param>
    /// <returns></returns>
    public static string GetPlatformConfig(bool bDomainAddress)
    {
        string platformConfig = GetResServerPath(bDomainAddress);

#if UNITY_ANDROID
        platformConfig += @"Config/Config_Android.xml";
#elif UNITY_IPHONE
        platformConfig += @"Config/Config_Apple.xml";
#else
        platformConfig += @"Config/Config_UnityEditor.xml";
#endif
        return platformConfig;
    }

    /// <summary>
    /// 获取平台文件路径
    /// </summary>
    /// <param name="bDomainAddress"></param>
    /// <returns></returns>
    public static string GetPlatformFilePath(bool bDomainAddress)
    {
        string platformFilePath = GetResServerPath(bDomainAddress);

#if UNITY_ANDROID
        platformFilePath += @"Android/StreamingAssets";
#elif UNITY_IPHONE
        platformFilePath += @"Apple/StreamingAssets";
#else
        platformFilePath += @"UnityEditor/StreamingAssets";
#endif
        return platformFilePath;
    }

    /// <summary>
    /// 获取配置文件更新
    /// </summary>
    /// <param name="bDomainAddress"></param>
    /// <returns></returns>
    public static string GetResServerPath(bool bDomainAddress)
    {
        Config.GameAddress gAddress = Config.GetUpdaterAddress();
        if (gAddress == null)
            return string.Empty;

        if (bDomainAddress)
        {
            return gAddress.strDomainAddress;
        }
        else
        {
            return gAddress.strIPAddress;
        }
    }

    /// <summary>
    /// 获取资源包下载cdn地址
    /// </summary>
    /// <param name="bDomainAddress"></param>
    /// <returns></returns>
    public static string GetABCdnPath(bool bDomainAddress)
    {
        string key = "AbsPath";
        if (!bDomainAddress)
            key = "AbsPath2";

        string strCdn = Config.GetUpdaterConfig(key, "Value");
        if (string.IsNullOrEmpty(strCdn))
            strCdn = GetPlatformFilePath(bDomainAddress);

        return strCdn;
    }

    #endregion

    /// <summary>
    /// 是否打开语音消息
    /// </summary>
    public const string OPENVOICE = "OpenVoice";

    /// <summary>
    /// 是否需要打开语音聊天;
    /// </summary>
    public static bool getOpenSnailVoice 
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.OPENVOICE, ref strResult);
            return "1".Equals(strResult);
        }
    }

    /// <summary>
    /// 是否支持语音转文字;
    /// </summary>
    public const string AudioToText = "AudioToText";

    public static bool getAudioToText 
    {
        get 
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.AudioToText, ref strResult);
            return "1".Equals(strResult); 
        }
    }

    public const string AudioOpenLog = "AudioOpenLog";

    public static int getAudioOpenLog
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue(Config.AudioOpenLog, ref strResult);
            if (string.IsNullOrEmpty(strResult)) 
            {
                strResult = "0";
            }
            return int.Parse(strResult);
        }
    }

    /// <summary>
    /// 拦截工具是否开启
    /// </summary>
    public static bool bInterceptOpen
    {
        get
        {
            string strResult = string.Empty;
            strResult = GetUpdaterConfig("Intercept", "Value");
            return "1".Equals(strResult);
        }
    }

    /// <summary>
    /// 二维码登陆开关
    /// </summary>
    public static bool bQrcodeLogin
    {
        get
        {
            string strResult = string.Empty;
            strResult = GetUpdaterConfig("QrcodeLogin", "Value");
            return "1".Equals(strResult) && !Config.bEditor && Config.bWin && !Config.bNormalLogin;
        }
    }

    /// <summary>
    /// 端类型-1没有配置 0-GM 1-IOS 2-Android
    /// </summary>
    public static int iPlatfromType
    {
        get
        {
            string strResult = string.Empty;
            strResult = GetUpdaterConfig("RoleBindPlatfrom", "PlatfromType");
            if (string.IsNullOrEmpty(strResult))
                return -1;

            return int.Parse(strResult);
        }
    }

    /// <summary>
    /// 是否限制显示帐号绑定的端与当前端平台不同的角色
    /// </summary>
    public static bool bLimitRoleType
    {
        get
        {
            string strResult = string.Empty;
            strResult = GetUpdaterConfig("RoleBindPlatfrom", "LimitRoleType");
            return "1".Equals(strResult) || "true".Equals(strResult);
        }
    }


    public static bool bNormalLogin
    {
        get
        {
            string[] args = System.Environment.GetCommandLineArgs();
            LogSystem.LogWarning("bNormalLogin");
            if (args == null || args.Length == 0)
                return false;

            string RexExp_Validate = @"NormalLoginBegin.+NormalLoginEnd";
            getMatchCollectionString(args, RexExp_Validate);
            System.Text.RegularExpressions.MatchCollection replaceArray = System.Text.RegularExpressions.Regex.Matches(string.Join(";", args), RexExp_Validate);

            if (null != replaceArray && replaceArray.Count > 0)
            {
                return true;
            }
            return false;
        }
    }

    public static bool bQrcodeScan
    {
        get
        {
            string strResult = string.Empty;
            strResult = GetUpdaterConfig("QrcodeLogin", "Value");
            return "1".Equals(strResult) && !Config.bEditor && (Config.bAndroid || Config.bIPhone);
        }
    }

    public static bool bPcSDKLog
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue("PcSDKLog", ref strResult);
            return "1".Equals(strResult) && !Config.bEditor;
        }
    }

    public static string PcDataCollectUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue("PcCollectUrl", ref strResult);
            return strResult;
        }
    }

    public static bool PcSDK
    {
        get
        {
            string strResult = string.Empty;
            strResult = GetUpdaterConfig("PcSDK", "Value");
            return "1".Equals(strResult);
        }
    }

    public static bool bCleanSnailAccount
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue("CleanSnailAccount", ref strResult);
            return "1".Equals(strResult);
        }
    }

    public static string MolongHelpUrl
    {
        get
        {
            string strResult = string.Empty;
            GetCustomInfoValue("MolongHelpUrl", ref strResult);
            return strResult;
        }
    }

    /// <summary>
    /// 获取更新配置信息
    /// </summary>
    /// <param name="strKey">主键名</param>
    /// <param name="strName">次键名</param>
    /// <returns>键值</returns>
    public static string GetUpdaterConfig(string strKey, string strName)
    {
        if (mDictUpdaterGuide.ContainsKey(strKey))
        {
            if (mDictUpdaterGuide[strKey].ContainsKey(strName))
            {
                return mDictUpdaterGuide[strKey][strName];
            }
        }
        return string.Empty;
    }

    /// <summary>
    /// 获取远程配置值
    /// </summary>
    /// <param name="strKey"></param>
    /// <param name="strResult"></param>
    /// <returns></returns>
    public static bool GetCustomInfoValue(string strKey, ref string strResult)
    {
        CustomInfo customInfo;
        if (!mDictCustomInfoList.TryGetValue(strKey, out customInfo))
        {
            LogSystem.LogWarning("Config::not found key ", strKey);
            return false;
        }
        strResult = customInfo.strCustomInfoVaule;
        return true;
    }

    /// <summary>
    /// 取本地资源根目录
    /// </summary>
    /// <returns>目录</returns>
    public static string GetAssetBundleRootPath()
    {
        return mstrAssetBundleRootPath;
    }
    /// <summary>
    /// 设置本地资源根目录
    /// </summary>
    public static void SetAssetBundleRootPath(string strRootPath)
    {
        mstrAssetBundleRootPath = strRootPath;
    }

    /// <summary>
    /// 评审服
    /// </summary>
    public static string mstrVersionUseage = string.Empty;
    public static string GetVersionUseage()
    {
        return mstrVersionUseage;
    }
    /// <summary>
    /// 设置评审服号
    /// </summary>
    /// <param name="strLocalVersion"></param>
    public static void SetVersionUseage(string strValue)
    {
        mstrVersionUseage = strValue;
    }


    /// <summary>
    /// 安装包版本号(用于更新地址获取)
    /// </summary>
    public static string mstrInstallationVersion = string.Empty;

    public static string GetInstallationVersion()
    {
        return mstrInstallationVersion;
    }
    /// <summary>
    /// 设置安装包版本号
    /// </summary>
    /// <param name="strValue"></param>
    /// <returns></returns>
    public static void SetInstallationVersion(string strValue)
    {
        mstrInstallationVersion = strValue;
    }

    /// <summary>
    /// 本地安装包的版本号(用于评审时，走评审流程使用)
    /// </summary>
    public static string mstrLocalVersion = string.Empty;
    public static string GetLocalVersion()
    {
        return mstrLocalVersion;
    }
    /// <summary>
    /// 设置本地安装包版本号
    /// </summary>
    /// <param name="strLocalVersion"></param>
    public static void SetLocalVersion(string strLocalVersion)
    {
        mstrLocalVersion = strLocalVersion;
    }

    /// <summary>
    /// 通信版本号(与服务器通信时,值必须大于等于服务器值)
    /// </summary>
    public static int miLocalNumberVersion = 1;
    public static int GetLocalNumberVersion()
    {
        return miLocalNumberVersion;
    }
    /// <summary>
    /// 设置本地安装包版本号
    /// </summary>
    /// <param name="strLocalVersion"></param>
    public static void SetLocalNumberVersion(string strLocalNumberVersion)
    {
        if (string.IsNullOrEmpty(strLocalNumberVersion))
        {
            miLocalNumberVersion = 0;
        }
        else
        {
            try
            {
                miLocalNumberVersion = int.Parse(strLocalNumberVersion);
            }
            catch (System.Exception ex)
            {
                miLocalNumberVersion = 0;
                LogSystem.LogError(ex.ToString());
            }
        }
    }

    /// <summary>
    /// 取streamingasset资源根目录
    /// </summary>
    /// <returns>目录</returns>
    public static string GetStreamRootPath()
    {
        return mstrStreamResRootPath;
    }
    /// <summary>
    /// 设置streamingasset资源根目录
    /// </summary>
    public static void SetStreamRootPath(string strRootPath)
    {
        mstrStreamResRootPath = strRootPath;
    }
    /// <summary>
    /// 设置本地资源更新服务器配置信息
    /// </summary>
    /// <param name="text">更新服务器地址文件信息</param>
    public static void SetUpdaterAddress(TextAsset text)
    {
        if (text == null)
            return;

        XMLParser parse = new XMLParser();
        XMLNode node = parse.Parse(text.text);
        XMLNodeList nodelist = node.GetNodeList("Addresses>0>Address");
        if (nodelist != null)
        {
            foreach (XMLNode n in nodelist)
            {
                GameAddress gAddr = new GameAddress();
                gAddr.strID = n.GetValue("@ID");
                gAddr.strName = n.GetValue("@Name");
                gAddr.strDomainAddress = n.GetValue("@DomainAddr") + "/" + Config.GetInstallationVersion() + "/";
                gAddr.strIPAddress = n.GetValue("@IPAddr") + "/" + Config.GetInstallationVersion()+"/";
                if (!mUpdaterAddress.ContainsKey(gAddr.strID))
                {
                    mUpdaterAddress.Add(gAddr.strID, gAddr);
                }
            }
        }
    }

    /// <summary>
    /// 是否是串登陆
    /// </summary>
    public static bool bValidateLogin = false;


    /// <summary>
    /// 登陆日志信息
    /// NetState        :是否没有网络
    /// ResConfig       :资源配置是否下载失败
    /// StateServer     :状态服是否获取失败
    /// ResAesstBundle  :资源包下载是否失败
    /// </summary>
    private static string mstrLoginLog = string.Empty;

    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="strLogType"></param>
    /// <param name="strLogInfo"></param>
    public static void LoginLog(string strLogType, string strLogInfo)
    {
        LogSystem.LogWarning("LoginLog:", strLogType, strLogInfo);
        string strFormat = LoginInfoFormat(strLogType, strLogInfo);
        mstrLoginLog = DelegateProxy.StringBuilder(mstrLoginLog, strFormat, "\t");
    }

    /// <summary>
    /// 清除数据
    /// </summary>
    public static void ClearLoginLog()
    {
        mstrLoginLog = string.Empty;
    }

    /// <summary>
    /// 是否有Login日志
    /// </summary>
    public static bool bHaveLoginLog
    {
        get
        {
            return !string.IsNullOrEmpty(mstrLoginLog);
        }
    }

    /// <summary>
    /// 登陆连接日志
    /// </summary>
    public static string strLoginLog
    {
        get
        {
            return mstrLoginLog;
        }
    }

    /// <summary>
    /// 格式设置
    /// </summary>
    /// <param name="strLogType"></param>
    /// <param name="strLogInfo"></param>
    /// <returns></returns>
    public static string LoginInfoFormat(string strLogType, string strLogInfo)
    {
        return DelegateProxy.StringBuilder(strLogType, "(", strLogInfo, ")");
    }

    /// <summary>
    /// 设置更新引导参数
    /// </summary>
    /// <param name="text">引导文件信息</param>
    public static void SetUpdateGuideInfo(string xmlString)
    {
        int index = xmlString.IndexOf('<');
        xmlString = xmlString.Substring(index);
        xmlString.Trim();

        XMLParser parse = new XMLParser();
        XMLNode rootnode = parse.Parse(xmlString);
        XMLNodeList xmlNodeList = (XMLNodeList)rootnode["Resources"];
        if (xmlNodeList == null)
        {
            return;
        }
        ///解析首段中的类型定义
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            XMLNode xmlnode = xmlNodeList[i] as XMLNode;
            XMLNodeList childNodeList1 = xmlnode.GetNodeList("Resource");
            if (childNodeList1 != null)
            {
                for (int j = 0; j < childNodeList1.Count; j++)
                {
                    XMLNode childnode = childNodeList1[j] as XMLNode;
                    Dictionary<string, string> resource = new Dictionary<string, string>();
                    string strID = string.Empty;
                    foreach (System.Collections.DictionaryEntry objDE in childnode)
                    {
                        if (objDE.Value == null)
                            continue;

                        string strKey = objDE.Key as string;
                        if (strKey[0] != '@')
                            continue;

                        strKey = strKey.Substring(1);
                        if (strKey == "ID")
                        {
                            strID = (string)objDE.Value;
                        }
                        else
                        {
                            if (resource.ContainsKey(strKey))
                            {
                                resource[strKey] = (string)objDE.Value;
                            }
                            else
                            {
                                resource.Add(strKey, (string)objDE.Value);
                            }
                        }
                    }
                    if (mDictUpdaterGuide.ContainsKey(strID))
                    {
                        mDictUpdaterGuide[strID] = resource;
                    }
                    else
                    {
                        mDictUpdaterGuide.Add(strID, resource);
                    }
                }
            }
        }

        bSettedChannelConfig = true;
    }

    /// <summary>
    /// 设置服务器公告信息(精彩活动)
    /// </summary>
    /// <param name="xmlString"></param>
    public static void SetServerNotice(string xmlString)
    {
        mstrServerNotice = xmlString;
    }

    /// <summary>
    /// 设置服务器信息
    /// </summary>
    /// <param name="text">服务器列表文本</param>
    public static void SetCustomInfoList(string xmlString)
    {
        int index = xmlString.IndexOf('<');
        xmlString = xmlString.Substring(index);
        xmlString.Trim();
        XMLParser parse = new XMLParser();
        XMLNode rootnode = parse.Parse(xmlString);

        SetCustomInfo((XMLNodeList)rootnode["Resources"]);
        InitResouceInfo();
        SetChannelInfo(rootnode);

        bNetDumper = mbNetDumper;

#if UNITY_ANDROID && !UNITY_EDITOR
        SetAccInfo(rootnode);
#endif
    }

    /// <summary>
    /// 设置渠道信息
    /// </summary>
    /// <param name="xmlNodeList"></param>
    static void SetChannelInfo(XMLNode rootnode)
    {
        if (rootnode == null)
        {
            return;
        }

        XMLNodeList xmlNodeList = rootnode.GetNodeList("Resources>0>Channel>0>Property");
        ChannelType = EunmChannelType.NONE;
        OrderType = EnumOrderType.Order_General;

        if (string.IsNullOrEmpty(strChannelUniqueName))
        {
            if(Config.bIPhone)
            {
                strChannelUniqueName = "ios_apple";
            }
            else
            {
                strChannelUniqueName = "android_snail";
            }
        }
        string strResServerPath = Config.GetResServerPath(true);
        ///解析首段中的类型定义
        foreach (XMLNode n in xmlNodeList)
        {
            string strID = n.GetValue("@ID");
            if (strID == strChannelUniqueName)
            {
                payCallBackURL = n.GetValue("@PayCallBackUrl");
                ClientInstallUrl = strResServerPath + n.GetValue("@ClientInstallUrl");
                strNoticeName = n.GetValue("@NoticeName");
                strChannelName = n.GetValue("@ChannelName");
                strServerName = n.GetValue("@ServerName");
                strShopItemFullName = n.GetValue("@ShopItemFullName");
                ChannelID = n.GetValue("@ChannelID");
                strChannelQRIMEI = n.GetValue("@QRIMEI");
                //采集
                bNeedDataCollect = "1".Equals(n.GetValue("@DataCollect"));
#if UNITY_STANDALONE_OSX && !UNITY_EDITOR
			    bNeedDataCollect = true;
#endif

                //订单类型
                string strOrderType = n.GetValue("@OrderType");
                if (!string.IsNullOrEmpty(strOrderType))
                {
                    strOrderType = strOrderType.Trim();
                    if (!string.IsNullOrEmpty(strOrderType))
                    {
                        int iOrderType = 0;
                        if (int.TryParse(strOrderType, out iOrderType))
                        {
                            OrderType = (EnumOrderType)iOrderType;
                        }
                        else
                        {
                            LogSystem.LogWarning("Error::OrderType Invalid value ", strOrderType);
                        }
                    }
                }

                //渠道类型
                string strChannelType = n.GetValue("@ChannelType");
                if (!string.IsNullOrEmpty(strChannelType))
                {
                    strChannelType = strChannelType.Trim();
                    if (!string.IsNullOrEmpty(strChannelType))
                    {
                        int iChannelType = 0;
                        if (int.TryParse(strChannelType, out iChannelType))
                        {
                            ChannelType = (EunmChannelType)iChannelType;
                        }
                        else
                        {
                            LogSystem.LogWarning("Error::OrderType Invalid value ", strChannelType);
                        }
                    }
                }

                break;
            }
        }
        LogSystem.LogWarning("Cofing::SetChannelInfo sdk ", strChannelUniqueName, " OrderType:", OrderType, " bNeedDataCollect:", bNeedDataCollect);
    }

    /// <summary>
    /// 初使化游戏信息
    /// </summary>
    public static void InitResouceInfo()
    {
        if (mDictCustomInfoList.ContainsKey("GoogleAccountIdent"))           //谷歌帐号串标识
        {
            strGoogleAccountIdent = mDictCustomInfoList["GoogleAccountIdent"].strCustomInfoVaule;
        }
        
    }

    /// <summary>
    /// 设置反外挂信息
    /// </summary>
    /// <param name="rootnode"></param>
    static void SetAccInfo(XMLNode rootnode)
    {
        if (rootnode == null)
        {
            LogSystem.LogWarning("SetAccInfo is null");
            return;
        }

        XMLNodeList xmlNodeList = rootnode.GetNodeList("Resources>0>Acc>0>Property");
        if (xmlNodeList == null)
        {
            LogSystem.LogWarning("SetAccInfo is null");
            return;
        }
        foreach (XMLNode n in xmlNodeList)
        {
            string strID = n.GetValue("@No");
            if (accInfo.ContainsKey(strID))
            {
                accInfo[strID].Add(n.GetValue("@Name"));
            }
            else
            {
                List<string> info = new List<string>();
                info.Add(n.GetValue("@Name"));
                accInfo.Add(strID, info);
            }
        }
    }

    /// <summary>
    /// 游戏配置信息
    /// </summary>
    /// <param name="xmlNodeList"></param>
    static void SetCustomInfo(XMLNodeList xmlNodeList)
    {
        if (xmlNodeList == null)
        {
            LogSystem.LogWarning("SetCustomInfo is null");
            return;
        }

        ///解析首段中的类型定义
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            XMLNode xmlnode = xmlNodeList[i] as XMLNode;
            XMLNodeList childNodeList1 = xmlnode.GetNodeList("Resource");
            if (childNodeList1 != null)
            {
                for (int j = 0; j < childNodeList1.Count; j++)
                {
                    XMLNode childnode = childNodeList1[j] as XMLNode;
                    CustomInfo psInfo = new CustomInfo();
                    foreach (System.Collections.DictionaryEntry objDE in childnode)
                    {
                        if (objDE.Value == null)
                            continue;

                        string strKey = objDE.Key as string;
                        if (strKey[0] != '@')
                            continue;

                        strKey = strKey.Substring(1);
                        if (strKey == "ID")
                        {
                            psInfo.strCustomInfoID = objDE.Value as string;
                        }
                        else if (strKey == "Value")
                        {
                            psInfo.strCustomInfoVaule = objDE.Value as string;
                        }
                    }
                    if (mDictCustomInfoList.ContainsKey(psInfo.strCustomInfoID))
                    {
                        mDictCustomInfoList[psInfo.strCustomInfoID] = psInfo;
                    }
                    else
                    {
                        mDictCustomInfoList.Add(psInfo.strCustomInfoID, psInfo);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 设置服务器信息
    /// </summary>
    /// <param name="text">服务器列表文本</param>
    public static void SetServerList(string xmlString)
    {
        int index = xmlString.IndexOf('<');
        xmlString = xmlString.Substring(index);
        xmlString.Trim();
        XMLParser parse = new XMLParser();
        XMLNode rootnode = parse.Parse(xmlString);
        XMLNodeList xmlNodeList = (XMLNodeList)rootnode["Servers"];
        if (xmlNodeList == null)
            return;

        ///解析首段中的类型定义
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            XMLNode xmlnode = xmlNodeList[i] as XMLNode;
            XMLNodeList childNodeList = xmlnode.GetNodeList("Server");
            if (childNodeList != null)
            {
                for (int j = 0; j < childNodeList.Count; j++)
                {
                    XMLNode childnode = childNodeList[j] as XMLNode;
                    Dictionary<string, string> server = new Dictionary<string, string>();
                    foreach (System.Collections.DictionaryEntry objDE in childnode)
                    {
                        if (objDE.Value == null)
                            continue;

                        string strKey = objDE.Key as string;
                        if (strKey[0] != '@')
                            continue;

                        strKey = strKey.Substring(1);
                        if (!server.ContainsKey(strKey))
                        {
                            server.Add(strKey, (string)objDE.Value);
                        }
                        else
                        {
                            server[strKey] = (string)objDE.Value;
                        }
                    }
                    mDictServerList.Add(server);
                }
            }
        }
    }

    /// <summary>
    /// 设置所有服务器信息
    /// </summary>
    /// <param name="text">服务器列表文本</param>
    public static void SetAllServerList(string xmlString)
    {
        int index = xmlString.IndexOf('<');
        xmlString = xmlString.Substring(index);
        xmlString.Trim();
        XMLParser parse = new XMLParser();
        XMLNode rootnode = parse.Parse(xmlString);
        XMLNodeList xmlNodeList = (XMLNodeList)rootnode["Servers"];
        if (xmlNodeList == null)
            return;

        ///解析首段中的类型定义
        for (int i = 0; i < xmlNodeList.Count; i++)
        {
            XMLNode xmlnode = xmlNodeList[i] as XMLNode;
            XMLNodeList childNodeList = xmlnode.GetNodeList("Server");
            if (childNodeList != null)
            {
                for (int j = 0; j < childNodeList.Count; j++)
                {
                    XMLNode childnode = childNodeList[j] as XMLNode;
                    Dictionary<string, string> server = new Dictionary<string, string>();
                    foreach (System.Collections.DictionaryEntry objDE in childnode)
                    {
                        if (objDE.Value == null)
                            continue;

                        string strKey = objDE.Key as string;
                        if (strKey[0] != '@')
                            continue;

                        strKey = strKey.Substring(1);
                        if (!server.ContainsKey(strKey))
                        {
                            server.Add(strKey, (string)objDE.Value);
                        }
                        else
                        {
                            server[strKey] = (string)objDE.Value;
                        }
                    }
                    mDictAllServerList.Add(server);
                }
            }
        }
    }

    //------------------------------------------------充值列表相关------------------------------------------------

    /// <summary>
    /// 设置VIP包信息
    /// </summary>
    /// <param name="text">服务器列表文本</param>
    public static void SetShopList(string xmlString)
    {
        mstrShopData = xmlString;
    }

    //------------------------------------------------充值列表相关------------------------------------------------

    /// <summary>
    /// 设置文言信息
    /// </summary>
    /// <param name="strWords"></param>
    public static void SetWordsInfo(string strWords)
    {
        if (string.IsNullOrEmpty(strWords))
            return;

        string[] strLines = strWords.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries); ;
        for (int i = 0; i < strLines.Length; i++)
        {
            string[] split = strLines[i].Split(new string[] { "=" }, 2, System.StringSplitOptions.RemoveEmptyEntries);
            if (split.Length == 2)
            {
                if (mWordsDict.ContainsKey(split[0]))
                    LogSystem.Log("the key is echo in local file!!! please check the key = ", split[0]);
                else
                {
                    split[1] = split[1].Replace("[n]", "\n");
                    mWordsDict[split[0]] = split[1];
                }
            }
        }
    }
   

    /// <summary>
    /// 是否有文言
    /// </summary>
    /// <param name="strKey"></param>
    /// <returns></returns>
    public static bool bUdpateLangage(string strKey)
    {
        return mWordsDict.ContainsKey(strKey);
    }

    /// <summary>
    /// 缓存缓存中
    /// </summary>
    /// <param name="useraccount"></param>
    /// <param name="username"></param>
    /// <param name="serverid"></param>
    /// <param name="gameid"></param>
    /// <param name="appstoreUrl"></param>
    public static void SavePlayerPrefs(string useraccount, string username, string serverid, string gameid, string appstoreUrl)
    {
        PlayerPrefs.SetString(USER_ACCOUNT, useraccount);
        string strTemp = WWW.EscapeURL(username);
        PlayerPrefs.SetString(USER_NAME, strTemp);
        PlayerPrefs.SetString(SERVERID, serverid);
        PlayerPrefs.SetString(GAME_ID, gameid);
        PlayerPrefs.SetString(APPSTORE_URL, appstoreUrl);
    }

    /// <summary>
    /// 获取缓存
    /// </summary>
    /// <param name="useraccount"></param>
    /// <param name="username"></param>
    /// <param name="serverid"></param>
    /// <param name="gameid"></param>
    /// <param name="appstoreUrl"></param>
    public static void GetPlayerPrefs(ref string useraccount, ref string username, ref string serverid, ref string gameid, ref string appstoreUrl)
    {
        useraccount = PlayerPrefs.GetString(USER_ACCOUNT, string.Empty);
        string strTemp = PlayerPrefs.GetString(USER_NAME, string.Empty);
        username = WWW.UnEscapeURL(strTemp);
        serverid = PlayerPrefs.GetString(SERVERID, string.Empty);
        gameid = PlayerPrefs.GetString(GAME_ID, string.Empty);
        appstoreUrl = PlayerPrefs.GetString(APPSTORE_URL, string.Empty);
    }

    /// <summary>
    /// 获取提示串
    /// </summary>
    /// <param name="strKeywords"></param>
    /// <returns></returns>
    public static string GetNetErrorPromp(string strKeywords)
    {
        string strPrompInfo = "CustomInfoError";
        //网络不通
        if (strKeywords.Contains("connect to host"))//网关错误
        {
            strPrompInfo = "CantConnectToHost";
        }
        else if (strKeywords.Contains("404"))//没有找到目标文件
        {
            strPrompInfo = "HTTPError_404";
        }
        else if (strKeywords.Contains("403"))//禁止访问
        {
            strPrompInfo = "HTTPError_403";
        }
        else if (strKeywords.Contains("405"))//资源被禁止
        {
            strPrompInfo = "HTTPError_405";
        }
        else if (strKeywords.Contains("406"))//无法接受
        {
            strPrompInfo = "HTTPError_406";
        }
        else if (strKeywords.Contains("407"))//要求代理身份验证
        {
            strPrompInfo = "HTTPError_407";
        }
        else if (strKeywords.Contains("410"))//永远不可用
        {
            strPrompInfo = "HTTPError_410";
        }
        else if (strKeywords.Contains("412"))//先决条件失败
        {
            strPrompInfo = "HTTPError_412";
        }
        else if (strKeywords.Contains("414"))//请求URI太长
        {
            strPrompInfo = "HTTPError_414";
        }
        else if (strKeywords.Contains("500"))//内部服务器错误
        {
            strPrompInfo = "HTTPError_500";
        }
        else if (strKeywords.Contains("501"))//未实现
        {
            strPrompInfo = "HTTPError_501";
        }
        else if (strKeywords.Contains("502"))//网关错误
        {
            strPrompInfo = "HTTPError_502";
        }

		return strPrompInfo;//Config.GetUdpateLangage(strPrompInfo);
    }
}