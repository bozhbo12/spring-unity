//#define PLATFORM_UC
//#define PLATFORM_PP
//#define PLATFORM_360
//#define PLATFORM_SS
public static class GlobalConfig
{
    public const string FUNCTION_CONFIG_PATH = "configures/function-config";
    public const string SYSTEM_CONFIG_PATH = "configures/system-config";


    /*
     * 行动值上限;
     * */
    public static int MAXACTION = 50;
    /*
     * 购买行动值上限;
     * */
    public static int MAXBUYACTION = 70;

	/*
	 * 购买添加的行动值
	 * */
	public static int AddAction = 20;
	
	/*
	 *  购买行动值的ID
	 * */
	public static int ActionId = 3;
	
	/*
	 * 是否是平台对接;
	 * */
    public static bool IS_PLATFORM_LINK = false;
	/*
	 * 判断是否是首包;
	 * */
	public static bool IS_First_Bag = true;
	
	
	/// <summary>
	/// 游戏服务器配置 可更改
	/// </summary>
	public static string GAME_SERVER_NAME = "GameServer";

	public static string GAME_SERVER_IP = "192.168.0.101"; 

	public static int[] GAME_SERVER_PORT = null;
     
    public static string LOGIN_ID = "998y7E1";//

	//注册服务器地址;
    public static string RegServerIP = "";
    public static bool RegDebugMode = false;

    public static int GameID = 31;//

    public static string TEMP = "111";
	
	//获取服务器信息地址;
	public static string ServerAddess =" ";
	
	//游戏资源更新地址;
	public static string url = "";
	
	//客户端更新地址;
    public static string UpdateAddress = "";

    //客户端更新地址;
    public static string UpdateAndroidAddress = "";
	
	//资源路径文件;
    public static string ResourceAddress
	{
		get{
#if PLATFORM_UC
			return "http://192.168.6.227/Card/ResourceInfo.xml";
#elif  PLATFORM_PP
         	return "http://192.168.6.227/Card/ResourceInfo.xml";
#elif  PLATFORM_360
			return "http://192.168.6.227/Card/360/ResourceInfo.xml";
#endif
			return "http://192.168.6.227/Card/Snail/ResourceInfo.xml";
		
		}
	}
		
		
		
//	public static string ResourceAddress = "http://updateps.xzh.woniu.com/Card/ResourceInfo.xml";

	
	//360网址文件配置;
	public static string Url360Adre = "http://222.92.116.36/ws/union/billing.do?method=imprestAccount360PAY";
	
    //公告显示地址;
    public static string PublishAddress = "";

    public static string PRODUCTION_ACCESSID = null;
    public static string PRODUCTION_ACCESSPASSWORD = null;
    public static string PRODUCTION_ACCESSTYPE = null;
    public static string PRODUCTION_SEED = null;
    public static string CHANNEL_ID = null;
	public static string WX_URL = null;

    // 当前接入平台;
    // 1-UC;    2-PP;
    public static int PLATFORM_TYPE = 1;
	
    // UC平台;
    public static int PLATFORM_TYPE_UC = 1;

    // PP平台;
    public static int PLATFORM_TYPE_PP = 2;
	
	//是否需要显示分享微信邀请好友功能;
	public static bool isNeedInviteFriend = true;
}

