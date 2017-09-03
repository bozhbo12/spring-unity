using System;


public static class ServerMessage
{
	#region <>通用模块<>
	/// <summary>
	/// 通用接口类型;
	/// </summary>
	public enum COMMONTYPE
	{

	}
	//通用接口;
	public const int SERVER_COMMON_REQ = 0xA105;
	public const int SERVER_COMMON_RESP = 0xA106;

	/// <summary>
	/// 服务端心跳维护(请求)
	/// </summary>
	public const int  SERVER_ACTIVE_REQ = 0xffff;
	
	public const int  MESSAGE_TIME = 0x9999;
	
	//Socket 成功;
	public const int CLIENT_SOCKET_SUCCEED = 0xf998;
	//Soceket 失败;
	public const int CLENT_SOCKET_LOSS = 0Xf999;
	
	/// <summary>
	/// GM命令(请求)
	/// </summary>
    public const int SERVER_GM_CMD_REQ = 0xff11;
    public const int SERVER_GM_CMD_RESP = 0xff12;
	public const int  SERVER_GET_SYS_TIME_REQ = 0xff03;
	public const int  SERVER_GET_SYS_TIME_RESP = 0xff04;
	
	
	#endregion
	#region <>角色管理模块<>
	
//	//<!-- 验证角色名是否重复 -->
//	public const int SERVER_ROLE_CHECK_NAME_REQ = 0xA003;
//	public const int SERVER_ROLE_CHECK_NAME_RESP = 0xA004;
	
	//<!--验证不登入> /OnRespVerify
	public const int SERVER_VERIFY_REQ = 0xA001;
	public const int SERVER_VERIFY_RESP = 0xA002;
	
	//重新连接刷新数据;
	public const int SERVER_RELOAD_REQ = 0xA151;
	public const int SERVER_RELOAD_RESP = 0xA152;
	
	//重新连接;
	public const int SERVER_KEY_REQ = 0xfffA;
	public const int SERVER_KEY_RESP = 0xfffB;
	

	
	//<!-- 创建角色 -->
	#endregion
	
	#region <>登陆模块<>	
	/// <summary>
	/// 用户登录(返回0xA008创建角色)
	/// </summary>
	public const int SERVER_LOGIN_REQ = 0xA003;
	public const int SERVER_LOGIN_RESP = 0xA004;



	/// <summary>
	/// 登出消息号;
	/// </summary>
	public const int SERVER_LOGIN_OUT_RESP = 0xA010;
	
	/// <summary>
	/// 用户DAtay>
	public const int SERVER_ROLE_PLAY_REQ = 0xA013;
	public const int SERVER_ROLE_PLAY_RESP = 0xA014;
	
	public const int SERVER_CREAT_REQ = 0xA007;
	public const int SERVER_CREAT_RESP = 0xA008;
	#endregion
	
	public const int SERVER_MODULUS_REQ = 0xA015;
	public const int SERVER_MODULUS_RESP = 0xA016;
	
	#region <>GPS搜索模块<>
	public const int SERVER_GPS_REQ = 0xA019;
	public const int SERVER_GPS_RESP = 0xA020;
	#endregion
	
	public const int SERVER_FIND_FRIEND_REQ = 0xA031;
	public const int SERVER_FIND_FRIEND_RESP = 0xA032;
	
	public const int SERVER_SEND_ADD_FRIEND_REQ = 0xA033;
//	public const int SERVER_SEND_ADD_FRIEND_RESP = 0xA034;
	
	public const int SERVER_FIND_AddBlackList_REQ = 0xA035;
	public const int SERVER_FIND_AddBlackList_RESP = 0xA036;
	public const int SERVER_FIND_RemoveBlackList_REQ = 0xA037;
	public const int SERVER_FIND_RemoveBlackList_RESP = 0xA038;
	
	
	
//	public const int SERVER_FRIEND_HELP_REQ = 0xA041;
//	public const int SERVER_FRIEND_HELP_RESP = 0xA042;
	public const int SERVER_HELP_RCV_RESP = 0xA044;
	
	public const int SERVER_GET_PACKAGE_REQ = 0xA045;
	public const int SERVER_GET_PACKAGE_RESP = 0xA046;
	public const int SERVER_OPEN_PACKAGE_REQ = 0xA047;
	public const int SERVER_OPEN_PACKAGE_RESP = 0xA048;
	public const int SERVER_PACKAGE_COUNT_RESP = 0xA050; 
	
	public const int SERVER_RACE_RANK_LIST_REQ = 0xA049;
//	public const int SERVER_RACE_RANK_LIST_RESP = 0xA050;
	public const int SERVER_FIGHT_LIST_REQ = 0xA051;
	public const int SERVER_FIGHT_LIST_RESP = 0xA052;
	
	public const int SERVER_CARD_STRONG_REQ = 0xA053;
	public const int SERVER_CARD_STRONG_RESP = 0xA054;
	
	public const int SERVER_AUCTION_SELL_REQ = 0xA071;
	public const int SERVER_AUCTION_SELL_RESP = 0xA072;
	public const int SERVER_AUCTION_CANCEL_REQ = 0xA073;
	public const int SERVER_AUCTION_CANCEL_RESP = 0xA074;
	
	public const int SERVER_AUCTION_BUY_REQ = 0xA075;
	public const int SERVER_AUCTION_BUY_RESP = 0xA076;
	public const int SERVER_AUCTION_QUERY_REQ = 0xA077;
	public const int SERVER_AUCTION_QUERY_RESP = 0xA078;
	
	#region <>强化 售卖 进阶 卡模块<>
	public const int SERVER_ENHANCE_REQ = 0xA053;
//	public const int SERVER_ENHANCE_RESP = 0xA054;
	public const int SERVER_SELL_REQ = 0xA055;
	public const int SERVER_SELL_RESP = 0xA056;
	
	public const int SERVER_UPGRADE_REQ = 0xA057;
	public const int SERVER_UPGRADE_RESP = 0xA058;
	
	#endregion
	
	#region<抽卡>
	public const int SERVER_SHOP_REQ = 0xA061;
	public const int SERVER_SHOP_RESP = 0xA062;
	
	
	#endregion
	
	#region<副本>
	//进入副本请求;
	public const int SERVER_DUPLICATE_ENTER_REQ = 0xA081;
	public const int SERVER_DUPLICATE_ENTER_RESP = 0xA082;
	public const int SERVER_DUPLICATE_STEP_REQ = 0xA083;
	public const int SERVER_DUPLICATE_STEP_COMMON_RESP = 0xA084;
	public const int SERVER_DUPLICATE_STEP_BOSS_RESP = 0xA086;
	public const int SERVER_DUPLICATE_STEP_DROP_RESP = 0xA088;
	//public const int SERVER_DUPLICATE_STEP_FRIEND_RESP = 0xA090;
	public const int SERVER_BOSS_FIGHT_REQ = 0xA091;
	public const int SERVER_BOSS_FIGHT_RESP = 0xA092;
	public const int SERVER_BOSS_LIST_REQ = 0xA093;
    public const int SERVER_BOSS_LIST_RESP = 0xA094;
    //public const int SERVER_DUPLICATE_QUIT_REQ = 0xA095;
    public const int SERVER_DUPLICATE_QUIT_REQ = -100;
    public const int SERVER_WORLD_BOSS_BUY_REQ = 0xA095;
    public const int SERVER_WORLD_BOSS_BUY_RESP = 0xA096;
	
	public const int SERVER_DUPLICATE_BOSS_LIST_REQ = 0xA097;
	public const int SERVER_DUPLICATE_BOSS_LIST_RESP = 0xA098;
	public const int SERVER_DUPLICATE_BUY_REQ = 0xA099;
	public const int SERVER_DUPLICATE_BUY_RESP = 0xA100;
	#endregion
	
	public const int SERVER_FRIEND_LIST_REQ = 0xA021;
	public const int SERVER_FRIEND_LIST_RESP = 0xA022;
	
//	public const int SERVER_FRIEND_INFO_REQ = 0xA133;
//	public const int SERVER_FRIEND_INFO_RESP = 0xA134;
	
	public const int SERVER_FRIEND_DELETE_REQ = 0xA023;
	public const int SERVER_FRIEND_DELETE_RESP = 0xA024;
	
	public const int SERVER_CHECK_FRIEND_REQ = 0xA027;
	public const int SERVER_CHECK_FRIEND_RESP = 0xA028;
	
	public const int SERVER_FRIEND_ACTION_REQ = 0xA039;
	public const int SERVER_FRIEND_ACTION_RESP = 0xA040;
	
//	public const int SERVER_FRIEND_ADD_REQ = 0xA025;
//	public const int SERVER_FRIEND_ADD_RESP = 0xA026;
	
	public const int SERVER_ACTION_FIGHT_VAL_REQ = 0xA121;
	public const int SERVER_ACTION_FIGHT_VAL_RESP = 0xA122;
	
	public const int SERVER_GUIDE_REQ = 0xA123;
	public const int SERVER_GUIDE_RESP = 0xA124;
	
//	public const int SERVER_FIGHT_REQ = 0xB001;
	public const int SERVER_FIGHT_RESP = 0xB002;
	
	public const int SERVER_SET_CARD_POSITION_REQ = 0xB003;
	public const int SERVER_SET_CARD_POSITION_RESP = 0xB004;
	
//	public const int SERVER_BOSS_LIST_REQ = 0xB005;
//	public const int SERVER_BOSS_LIST_RESP = 0xB006;
//	public const int SERVER_FIGHT_WORLD_BOSS_REQ = 0xB007;
	public const int SERVER_FIGHT_WORLD_BOSS_RESP = 0xB008;
	public const int SERVER_RACE_FIGHT_REQ = 0xB009;
	public const int SERVER_RACE_FIGHT_RESP = 0xB010;
	
	public const int SERVER_PHALANX_STOP_RESP = 0xB067;
	
	public const int SERVER_CHAT_REQ = 0xA761;
	public const int SERVER_CHAT_RESP = 0xA762;
	
	public const int SERVER_Mail_GetList_REQ = 0xA201;
	public const int SERVER_Mail_GetList_RESP = 0xA202;	
	
	public const int SERVER_Mail_ReadMail_REQ = 0xA205;
	public const int SERVER_Mail_RefMailList_REQ = 0xA208;	
	
	public const int SERVER_Friend_RefAddFreind_REQ = 0xA034;	
	
	#region<行动力>
	public const int SERVER_ACTION_BUY_REQ = 0xA145;
	public const int SERVER_ACTION_BUY_RESP = 0xA146;
	#endregion

	
	#region <>正确 错误码<>
	public const int  SERVER_GET_GAME_ERROR_RESP = 0xff08;
	public const int  SERVER_GET_GAME_SUCCESS_RESP = 0xff10;
	#endregion
	
	#region<更换角色头像>
	public const int  SERVER_GET_ROLE_HEAD_REQ = 0xA147;
    public const int SERVER_GET_ROLE_HEAD_RESP = 0xA148;

    public const int SERVER_GMCC_REQ = 0xAA00;
    public const int SERVER_GMCC_RESP = 0xAA01;
	#endregion
	
	#region<购买黄金>
	public const int SERVER_GET_ROLE_RolePoint_REQ = 0xA951;
    public const int SERVER_GET_ROLE_RolePoint_RESP = 0xA952;
	
    public const int SERVER_GET_ROLE_ConsumePoint_REQ = 0xA953;
    public const int SERVER_GET_ROLE_ConsumePoint_RESP = 0xA954;
	#endregion
	
	#region<公告>
	public const int SERVER_GET_QueryPublish_REQ = 0xA153;
    public const int SERVER_GET_QueryPublish_RESP = 0xA154;
	#endregion
	
	#region<竞技场>
	public const int SERVER_FriendFight_REQ = 0xB005;
    public const int SERVER_FriendFight_RESP = 0xB006;
	#endregion
	
	#region<请求商品id>
	public const int SERVER_ChargerGetOrder_REQ = 0xA963;
    public const int SERVER_ChargerGetOrder_RESP = 0xA964;
	#endregion
	
	#region<请求好友邀请>
	public const int SERVER_InviteInfo_REQ = 0xA159;
	public const int SERVER_InviteInfo_RESP = 0xA160;
	
	public const int SERVER_InviteCodeValidate_REQ = 0xA157;
	public const int SERVER_InviteCodeValidate_RESP = 0xA158;
	#endregion
	
	#region<碎片兑换>
	public const int SERVER_ExchangeInfo_REQ=0xA161;
	public const int SERVER_ExchangeInfo_RESP=0xA162;
	#endregion
}