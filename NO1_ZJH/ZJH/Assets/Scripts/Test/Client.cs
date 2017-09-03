
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System;
//using System.Diagnostics;


/// <summary>
/// 客户端全局对象，用于网络方面的处理;
/// </summary>
public class Client {
	
	/// <summary>
	/// 网络消息分发器，非线程安全;
	/// </summary>
	public static EventDispatcher NetMessageDispather = new EventDispatcher();
	private int m_FriendRoleId = 0;
	private static string tempAccount = "";
			
	private static Client s_instance = new Client();
	
	public static Client Instance()
	{
		return s_instance;
	}
	
	/// <summary>
	/// 服务器返回结果定义;
	/// </summary>
	public class BoolConfig
	{
		public const int SUCCESS = 1;
		public const int TRUE = 1;
	}
	
	#region <基本功能>
	/// <summary>
	/// 初始化;
	/// </summary>
	public void init()
	{
		NetMessageDispather.Register(ServerMessage.MESSAGE_TIME, OnRespMessageTime);
		NetMessageDispather.Register(ServerMessage.SERVER_KEY_RESP, OnRespServerKeyLogin);
		NetMessageDispather.Register(ServerMessage.SERVER_ACTIVE_REQ,OnRespServerActive);
		NetMessageDispather.Register(ServerMessage.CLIENT_SOCKET_SUCCEED,OnSocketSucceed);
		NetMessageDispather.Register(ServerMessage.CLENT_SOCKET_LOSS,OnSocketLoss);

		NetMessageDispather.Register(ServerMessage.SERVER_LOGIN_RESP, OnRespLogin); 

    }
	
	public void Close()
	{
		

		NetMessageDispather.Remove(ServerMessage.SERVER_ACTIVE_REQ);
		
		NetMessageDispather.Remove(ServerMessage.CLIENT_SOCKET_SUCCEED);
		NetMessageDispather.Remove(ServerMessage.CLENT_SOCKET_LOSS);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_KEY_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_Mail_GetList_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_Mail_RefMailList_REQ);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_Friend_RefAddFreind_REQ);
		
		//注销消息注册;
		NetMessageDispather.Remove(ServerMessage.SERVER_GET_SYS_TIME_RESP);	
		NetMessageDispather.Remove(ServerMessage.SERVER_GET_GAME_ERROR_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_GET_GAME_SUCCESS_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_LOGIN_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_VERIFY_RESP); 
		NetMessageDispather.Remove(ServerMessage.SERVER_ROLE_PLAY_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_CREAT_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_FIGHT_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_BOSS_LIST_RESP); 
		NetMessageDispather.Remove(ServerMessage.SERVER_HELP_RCV_RESP);		
		NetMessageDispather.Remove(ServerMessage.SERVER_LOGIN_OUT_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_GPS_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_FIGHT_WORLD_BOSS_RESP); 
		NetMessageDispather.Remove(ServerMessage.SERVER_GET_PACKAGE_RESP); 
		
		NetMessageDispather.Remove(ServerMessage.SERVER_FRIEND_LIST_RESP);
		//NetMessageDispather.Remove(ServerMessage.SERVER_FRIEND_INFO_RESP);
		//NetMessageDispather.Remove(ServerMessage.SERVER_FRIEND_ADD_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_FIND_FRIEND_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_FRIEND_DELETE_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_CHECK_FRIEND_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_OPEN_PACKAGE_RESP); 
		NetMessageDispather.Remove(ServerMessage.SERVER_PACKAGE_COUNT_RESP);
		
//		NetMessageDispather.Remove(ServerMessage.SERVER_RACE_RANK_LIST_RESP); 
		NetMessageDispather.Remove(ServerMessage.SERVER_FIGHT_LIST_RESP); 
		NetMessageDispather.Remove(ServerMessage.SERVER_RACE_FIGHT_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_SHOP_RESP);
		
		//NetMessageDispather.Remove(ServerMessage.SERVER_EXTRACT_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_AUCTION_SELL_RESP); 
		NetMessageDispather.Remove(ServerMessage.SERVER_AUCTION_CANCEL_RESP); 
		NetMessageDispather.Remove(ServerMessage.SERVER_AUCTION_BUY_RESP); 
		NetMessageDispather.Remove(ServerMessage.SERVER_AUCTION_QUERY_RESP);
		//NetMessageDispather.Remove(ServerMessage.SERVER_SEND_ADD_FRIEND_RESP);
		
		#region <副本>
		NetMessageDispather.Remove(ServerMessage.SERVER_DUPLICATE_STEP_COMMON_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_DUPLICATE_STEP_BOSS_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_DUPLICATE_STEP_DROP_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_BOSS_FIGHT_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_BOSS_LIST_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_FRIEND_ACTION_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_SELL_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_DUPLICATE_BOSS_LIST_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_DUPLICATE_BUY_RESP);
		#endregion
		
		NetMessageDispather.Remove(ServerMessage.SERVER_MODULUS_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_ACTION_FIGHT_VAL_RESP);
		
		NetMessageDispather.Remove(ServerMessage.MESSAGE_TIME);
		
		
//		NetMessageDispather.Remove(ServerMessage.SERVER_Mail_GETMAILATTACHMENT_RESP);
		
		//set card
		//SetCard
		NetMessageDispather.Remove(ServerMessage.SERVER_SET_CARD_POSITION_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_CHAT_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_FIND_AddBlackList_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_FIND_RemoveBlackList_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_CARD_STRONG_RESP);
	
		NetMessageDispather.Remove(ServerMessage.SERVER_ACTION_BUY_RESP);

		NetMessageDispather.Remove(ServerMessage.SERVER_UPGRADE_RESP);
        NetMessageDispather.Remove(ServerMessage.SERVER_GM_CMD_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_GET_ROLE_HEAD_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_RELOAD_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_GET_ROLE_RolePoint_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_GET_ROLE_ConsumePoint_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_GET_QueryPublish_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_FriendFight_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_ChargerGetOrder_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_InviteInfo_RESP);
		NetMessageDispather.Remove(ServerMessage.SERVER_InviteCodeValidate_RESP);
		
		NetMessageDispather.Remove(ServerMessage.SERVER_ExchangeInfo_RESP);
		

    }
	
	/// <summary>
	/// 连接服务器;
	/// </summary>
	public void Connect()
	{
		UnitySocket.Instance.SocketInit();
	}
	/// <summary>
	/// 断开游戏服务器;
	/// </summary>
	public void Disconnect()
	{
		UnitySocket.Instance.DisconnectNow();
	}
	
	/// <summary>
	/// 连接成功回调;
	/// </summary>
	public void OnConnected()
	{
		//if(DebugConfig.Debug_Net_Protocal)
		{
		}
	}
	/// <summary>
	/// 网络错误回调;
	/// </summary>
	public void OnNetError()
	{
	}
	
	/*
	 * 返回Socket成功;
	 * */
	protected void OnSocketSucceed(int messageId, object sender, object args)
	{
		SingletonMessage.instance.DeduceScoketSucc(true);
	}
	/*
	 *返回Socket失败;
	 * */
	protected void OnSocketLoss(int messageId, object sender, object args)
	{
		SingletonMessage.instance.DeduceScoketSucc(false);
	}
	/*请求重新连接*/
	public void ReqServerKeyLogin()
	{
		int messageId = ServerMessage.SERVER_KEY_REQ;
			
		UnitySocket unitySocket = UnitySocket.Instance;
     
		Socket socket = unitySocket.getSocket();

		if (socket != null && socket.Connected)
        {
            Message messgae = new Message();
			

			GameMessageHead messageHead = BaseMessageHandle.getMessageHead(1, 1, messageId, 1,string.Empty);
			
			messageHead.setUserID2(SingletonMessage.instance.GetRoleKey());
			
            messgae.setHead(messageHead);
			messgae.setBody(null);
            unitySocket.sendData(messgae);
        }
	}
	/*响应重新连接*/
	protected void OnRespServerKeyLogin(int messageId, object sender, object args)
	{
		SingletonMessage.instance.DeduceNetWork(true);
	}
	/*
	 * 请求刷新服务器数据;
	 * */
	public void ReqServerReload()
	{
		 NetUtils.Send(ServerMessage.SERVER_RELOAD_REQ ,null);
	}
	/*
	 * 响应服务器服务器数据;
	 * */
	protected void OnRespServerReload(int messageId, object sender, object args)
	{
		RoleReloadResp resp = (RoleReloadResp)args;
	}
	
	/*
	 * 请求心跳;
	 * */
	public void ReqServerActive()
	{
		//Debug.Log("=========================ReqServerActiveReqServerActive=====");
		 NetUtils.Send(ServerMessage.SERVER_ACTIVE_REQ ,null);
	}
	/*
	 * 响应心跳;
	 * */
	protected void OnRespServerActive(int messageId, object sender, object args)
	{
		SingletonMessage.instance.OnRespHearbeat(true);
	}
	
	
	#endregion
	
	#region<网络连接测试>
	/*收集消息成功回调*/
	protected void OnRespMessageTime(int messageId, object sender, object args)
	{
		SingletonMessage.instance.DeduceNetWork(true);
//		MessageDetection.instance.SetServerInfo(messageId);
	}
	#endregion
	
	

	
	//串登入;
	public bool ReqRoleLogin(string loginbunch)
	{
		UserLoginReq req = new UserLoginReq();


		
        return NetUtils.Send(ServerMessage.SERVER_LOGIN_REQ, req);
	}
	
	
	/// <summary>
	/// 登入;
	/// </summary>
	protected void OnRespLogin(int messageId, object sender, object args)
	{
		Debug.LogError("====OnRespLogin===");
		if (args != null)
		{
			UserLoginResp resp = (UserLoginResp) args;
            			
			Debug.LogError("resp.getResult() = "+resp.getResult());
			if(resp.getResult() == BoolConfig.SUCCESS )
            {
               
			}
			else if(resp.getResult() == 0)
            {
				
			}
			else
			{
			}
		}
	}
	

}