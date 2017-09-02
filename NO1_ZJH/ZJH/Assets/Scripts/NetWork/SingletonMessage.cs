using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class SingletonMessage{

	private static SingletonMessage mMessage;
	
	private static object sRoot = new object();
	
	public static SingletonMessage instance
	{
		get
		{
			if(mMessage == null)
			{
				lock(sRoot)
				{
					if(mMessage == null)
					{
						mMessage = new SingletonMessage();
					}
				}
			}	
			return mMessage;
		}
	}
	
	private SingletonMessage()
	{
		
	}
	public delegate void SocketConnectBack(bool isBool);
	//发送消息等待时间;
	public float mWaitTime = 10f;
	//次数的范围;
	public const int SocketCount = 3;
	//唤醒的状态;
	public enum WhetherPause{
		Pause_yes,
		pause_No,
	}
	
	public WhetherPause m_StatePause;
	
	public void SetPause(bool isBool)
	{
		if(isBool)
		{
			m_StatePause = WhetherPause.pause_No; //休眠状态;
		}
		else
		{
			m_StatePause = WhetherPause.Pause_yes; //唤醒状态;
		}
	}
	
	
	//当前操作状态;
	public enum MackState
	{
		//心跳;
		Hearbeat,
		//连接状态;
		Connet,
		//刷新,
		Refresh,
		//正常
		Normal
	}
	public MackState mMackState;
	
	//当前网络的状态;
	public enum ConnetState
	{
		NetWork_Succ,
		NetWork_Ing,
		NetWork_UnSucc,
	}
	private ConnetState mConnetState;
	public ConnetState GetConnetState
	{
		get{return mConnetState;}
	}
	
	//网络连接成功;
	public void NetWorkSucc()
	{
		mConnetState = ConnetState.NetWork_Succ;
		GetTimeMessage().CloseLoading();
	}
	//正在等待响应或者创建连接中.......;
	public void NetWorkIng()
	{
		mConnetState = ConnetState.NetWork_Ing;
		GetTimeMessage().OpenLoading();
	}
	//网络连接失败;
	public void NetWorkUnSucc()
	{
		mConnetState = ConnetState.NetWork_UnSucc;
		GetTimeMessage().CloseLoading();
		UnitySocket.Instance.NotConnet();
	}
	
#region<检查网络状态>
	//检查Socket连接;
	public bool ShowSocketConnect()
	{
		if(UnitySocket.Instance.getSocket() == null)
		{
			return false;
		}
		if(!UnitySocket.Instance.getSocket().Connected)
		{
			return false;
		}
		return true;
	}
	//心跳检查网络连接状态;
	//请求;
	public void ReqHearbeat()
	{
		mMackState = MackState.Hearbeat;
//		Client.Instance().ReqServerActive();
		
	}
	//响应;
	public void OnRespHearbeat(bool isBool)
	{
//		NGUIDebug.Log("==============OnRespHearbeat================"+isBool);
	}
#endregion
	
#region<创建Socket>
//创建Socket请求;
	private void SocketConnect()
	{
//		NGUIDebug.Log(" Show Socket ----------------");
		UnitySocket.Instance.NotConnet();
		NetWorkIng();
		UnitySocket.Instance.SocketInit();
	}
//创建Socket响应;
	//检测Socket连接是否创建成功< flase-->失败 true -->Succ;
	public void DeduceScoketSucc(bool isbool)
	{
//		NGUIDebug.Log("=====DeduceScoketSucc==="+isbool);
		if(isbool)
		{
			NetWorkSucc();
			//成功;
//			if(Data.Instance.function_State == UI.FUNCTION_STATE.STATE_LOGIN 
//				|| Data.Instance.function_State == UI.FUNCTION_STATE.STATE_SERVER)
//			{
//				mMackState = MackState.Normal;
//				//登入服务器;
//#if UNITY_STANDALONE_WIN
//                if (Data.Instance.LoginString != null && Data.Instance.LoginString != "")
//                {
//                    Client.Instance().ReqRoleLogin("", "");
//                }
//                else
//#endif
//                {
////					NGUIDebug.Log("===================================="+isbool);
//                    NumberLogin();
//                }
//			}
//			else
//			{
//				//创建重新连接;
//				NumberLogin();
//			}
		}
		else
		{
			
			//网络创建失败返回登入界面;
			ShowUnSuccNetWork();
		}
	}
#endregion

#region<连接服务器>
//连接服务器请求;
	//登入请求;
	private void NumberLogin()
	{
	//	NGUIDebug.Log("+++++++++++++++NumberLoginNumberLogin++++++++++++++");
		if(GlobalConfig.IS_PLATFORM_LINK)
		{
			//串登入;
//			PlatformLink.ConnectServer();
		}
		else
		{
//			Client.Instance().ReqRoleVerify(UserName, Password);
		}
	} 
	//key值请求;
	public void KeyLogin()
	{
//		Client.Instance().ReqServerReload();
	}
////连接服务器响应;
//	//响应;
//	public void RespServer(bool isBool)
//	{
//		
//	}
#endregion
	
#region<正常通信检查>
	//判断是否能够发送消息;
	public bool BoolSend(int messageId)
	{
		Debug.LogError("GetConnetState = "+GetConnetState.ToString());
		if (GetConnetState == ConnetState.NetWork_Succ)
		{
			if(getShieldMassage(messageId))
			{
				Debug.LogError("=======1");
				return true;
			}
			else
			{
				//需要等待网络消息回复;
				NetWorkIng();
				Debug.LogError("=======2");
				return true;
			}
		}
		else
		{
			if(GetConnetState == ConnetState.NetWork_Ing)
			{
				if(!GetTimeMessage().loading)
				{
					GetTimeMessage().OpenLoading();
				}
			}
			Debug.LogError("=======3");
			return false;
		}
	}
	
	//接受消息号的 <-flase 断开  true 连接->;
	public void DeduceNetWork(bool isbool)
	{
		Debug.LogError("=============DeduceNetWork================="+isbool);
		if(isbool)
		{
			
			NetWorkSucc();
			NormalNetwork();
		}
		else
		{
			ErrorNetWork();
		}
	}
	
	/******************
	 * 屏蔽某些特定的消息号 请求
	 * *****************/
	private List<int> shieldMassage = new List<int>();
	private void SetShieldMassage()
	{
		if(!shieldMassage.Contains(0xA015))
			shieldMassage.Add(0xA015);
//		if(!shieldMassage.Contains(0xffff))
//			shieldMassage.Add(0xffff);
		if(!shieldMassage.Contains(0xff03))
			shieldMassage.Add(0xff03);
		if(!shieldMassage.Contains(0xA121))
			shieldMassage.Add(0xA121);
		if(!shieldMassage.Contains(0xa205))
			shieldMassage.Add(0xa205);
		if(!shieldMassage.Contains(0xA095))
			shieldMassage.Add(0xA095);
		if(!shieldMassage.Contains(0xA041))
			shieldMassage.Add(0xA041);
        if (!shieldMassage.Contains(0xAA00))
            shieldMassage.Add(0xAA00);
		if (!shieldMassage.Contains(0xA147))
            shieldMassage.Add(0xA147);
		if (!shieldMassage.Contains(0xA761))
            shieldMassage.Add(0xA761);
		if (!shieldMassage.Contains(0xA158))
            shieldMassage.Add(0xA158);
	}
	//判断消息号;
	private bool getShieldMassage(int massage)
	{
		if(shieldMassage.Count <= 0)
		{
			SetShieldMassage();
		}
		
		if(shieldMassage.Contains(massage))
		{
			return true;
		}
		return false;
	}
	
	/************************
	 * 屏蔽某些响应消息号
	 * ***********/
	private List<int> serverMessage = new List<int>();
	//判断消息号;
	public bool GetServerMessage(int message)
	{
		Debug.LogError("message = 0x"+ Convert.ToString(message, 16));
		if(serverMessage.Count <= 0)
		{
			AddServerMessage();
		}
		if(serverMessage.Contains(message))
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	private void AddServerMessage()
	{
//		if(!serverMessage.Contains(0xffff))
//			serverMessage.Add(0xffff);
		if(!serverMessage.Contains(0xA016))
			serverMessage.Add(0xA016);
		if(!serverMessage.Contains(0xff04))
			serverMessage.Add(0xff04);
		if(!serverMessage.Contains(0xA122))
			serverMessage.Add(0xA122);
		if(!serverMessage.Contains(0xA095))
			serverMessage.Add(0xA095);
		if(!serverMessage.Contains(0xA142))
			serverMessage.Add(0xA142);
		if(!serverMessage.Contains(0xA144))
            serverMessage.Add(0xA144);
        if (!serverMessage.Contains(0xA208))
            serverMessage.Add(0xA208);
        if (!serverMessage.Contains(0xA050))
            serverMessage.Add(0xA050);
        if (!serverMessage.Contains(0xA150))
            serverMessage.Add(0xA150);
        if (!serverMessage.Contains(0xAA01))
            serverMessage.Add(0xAA01);
		if (!serverMessage.Contains(0xA148))
            serverMessage.Add(0xA148);
		if (!serverMessage.Contains(0xA762))
            serverMessage.Add(0xA762);
	}
	
	//检测消息发送的时间;
	private TimeMessage m_TimeMessage;
	private TimeMessage GetTimeMessage()
	{
		if(m_TimeMessage == null)
		{
			if(GameObject.Find("TimeMessage") == null)
			{
				GameObject game = new GameObject();
				game.name = "TimeMessage";
				m_TimeMessage = game.AddComponent<TimeMessage>();
			}
			else
			{
				m_TimeMessage = GameObject.Find("TimeMessage").GetComponent<TimeMessage>();
			}
		}
		return m_TimeMessage;
	}
	
#endregion
	
#region<执行状态>
	//账号连接;
	
	//登入账号;
	private string UserName;
	
	//登入密码;
	private string Password;

	//登入Key值;
	private int RoleKey;
	
	public void SetRoleKey(int key)
	{
		RoleKey = key;
	}
	public int GetRoleKey()
	{
		return RoleKey;
	}
	//获取用户登入信息 --登入状态和选择服务器状态;
	public void SetRoleInfo(string username ,  string password)
	{
	//	NGUIDebug.Log("====================SetRoleInfo===========================");
		UserName = username;
		Password = password;
//		Data.Instance.Password = password;
//		Data.Instance.UserName = username;
		SocketConnect();
	}
	
	//重连成功后请求刷新数据;
	public void RefreshData()
	{
//		Client.Instance().ReqRolePlay();
	}
	//刷新数据;
	public void RefreshDataState()
	{
//		mMackState = MackState.Normal;
//		Data.Instance.function_Hearbeat = Data.Instance.function_State;
//		UI.Instance().CloseGameUI(UI.FUNCTION_STATE.STATE_Hearbeat);
//		
//		if (GuideControl.isShowGuide)
//		{
//			GuideControl.isShowGuide = false;
//			
//			if (GuideControl.currentGuideID == (int)GuideControl.GuideIndex.CardSettingGuide2
//				|| GuideControl.currentGuideID == (int)GuideControl.GuideIndex.CardSettingGuide3)
//			{
//				SingletonAttactkCardsControl.instance.AutoMatism();
//			}
//			
//			Data.Instance.Role.Gold += Data.Instance.Role.guidTempGold;
//			Data.Instance.Role.RMBGold += Data.Instance.Role.guidTempRMBGold;
//			
//			Data.Instance.Role.guidTempGold = 0;
//			Data.Instance.Role.guidTempRMBGold = 0;
//		}
	}
	
	
	//提示网络连接失败;
	private void ShowUnSuccNetWork()
	{
		NetWorkUnSucc();
//		SingletonErrorMessage.instance.ShowClenOn(GotoLogin,ErrorCodeManager.Instance().GetContext(387878));
	}
	//返回登入界面;
 	public void GotoLogin()
	{
//		switch(Data.Instance.function_State)
//		{
//			case UI.FUNCTION_STATE.STATE_ATTACK :
//				Time.timeScale = 1;
//			break;
//			case UI.FUNCTION_STATE.STATE_SETCARDS: 
//				SingletonAttactkCardsControl.instance.CloseFunctaion();
//			break;
//		default:
//			break;
//		}
//		
//		UI.Instance().CloseGameUI(UI.FUNCTION_STATE.STATE_SERVER);
//        NetWorkSucc();
	}
	
	#endregion

#region<连接逻辑判断>
	//网络连接失败;
	private void ErrorNetWork()
	{
//		if(Data.Instance.function_State == UI.FUNCTION_STATE.STATE_LOGIN ||
//			Data.Instance.function_State == UI.FUNCTION_STATE.STATE_SERVER)
//		{
//			ShowUnSuccNetWork();
//			return;
//		}
		
		if(mMackState == MackState.Normal)
		{
			//心心跳检测;
			ReqHearbeat();
		}
		else if(mMackState == MackState.Hearbeat)
		{
			if(GlobalConfig.IS_PLATFORM_LINK)
			{
				ShowUnSuccNetWork();
				return;
			}
//			NGUIDebug.Log("=-=-=-=-ErrorNetWork=-=-=-=");
			mMackState = MackState.Connet;
			SocketConnect();
		}
		else if(mMackState == MackState.Connet || mMackState == MackState.Refresh)
		{
			//提示网络错误;
			ShowUnSuccNetWork();
		}
	}
	//网络连接成功;
	private void NormalNetwork()
	{
		Debug.LogError("==============================================="+mMackState.ToString());
		if(mMackState == MackState.Normal)
		{
			return;
		}
		else if(mMackState == MackState.Hearbeat)
		{
//			NGUIDebug.Log("=-=-=-=-NormalNetwork=-=-=-=");
			mMackState = MackState.Normal;
			RefreshData();
		}
		else if(mMackState == MackState.Connet)
		{
			mMackState = MackState.Normal;
//			RefreshDataState();
		}
		else if(mMackState == MackState.Refresh)
		{
			//刷新界面;
			RefreshDataState();
		}
	}
#endregion
	
	
	#region<唤醒重连>
	public void SetPauseServerReload()
	{
		if(m_StatePause == WhetherPause.Pause_yes)
		{
			return;
		}
		//心跳请求;
		
	}
	//心跳响应;
	public void ServerReload(bool isBool)
	{
		SetPause(false);
		if(isBool)
		{
			//成功;
			
			//申请数据刷新界面;
		}
		else
		{
			//失败;
			//重新登入;
			
		}
	}
	
	
	#endregion
}
