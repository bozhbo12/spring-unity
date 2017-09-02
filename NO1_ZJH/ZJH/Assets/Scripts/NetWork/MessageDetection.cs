using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MessageDetection {

	private static MessageDetection mMessage;
	
	private static object sRoot = new object();
	
	public static MessageDetection instance
	{
		get
		{
			if(mMessage == null)
			{
				lock(sRoot)
				{
					if(mMessage == null)
					{
						mMessage = new MessageDetection();
					}
				}
			}	
			return mMessage;
		}
	}
	
	private MessageDetection()
	{
		shieldMassage = new List<int>();
		
		serverMessage = new List<int>();
	}
	/******************
	 * 屏蔽某些特定的消息号 请求
	 * *****************/
	private List<int> shieldMassage;
	private void SetShieldMassage()
	{
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
	}
	
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
	

	
	/*****************
	 * 用户账号和密码
	 * *******************/
	public void SetNamePass(string userName,string password)
	{
//		Data.Instance.UserName = userName;
//		Data.Instance.Password = password;
	}
//	public string GetUserName()
//	{
//		return userName;
//	}
//	public string GetPassword()
//	{
//		return Password;
//	}
	
	/*******
	 * 当前的所发的消息号
	 * *********/
//	private int SEND_SERVER_MESSAGE;
//	public void SetSendMessage(int message)
//	{
//		SEND_SERVER_MESSAGE = message;
//	}
//	public int GetSendMessage()
//	{
//		return SEND_SERVER_MESSAGE;
//	}
	
	/******************
	 * 判断处理是否可以继续发送消息
	 * *****************/
//	private bool isSend;
//	public void SetSend(bool isBool)
//	{
//		isSend = isBool;
//	}
//	
	
//	private bool isUpDate;
//	public bool GetUPdate()
//	{
//		return isUpDate;
//	}
//	public void SetUPdate(bool isbool)
//	{
//		Debug.Log("isbool = "+isbool);
//		isUpDate = isbool;
//	}
//	
//	private void TimeInfo()
//	{
//		Debug.LogError("==================TimeInfo================1============");
//		GetTimeMessage().DataInfo();
//		isSend = false;
//	}
	/***************
	 * 发送消息后等待的时间
	 * ********************/
////	public const float WaitTime = 10f;
//	public float GetWaitTime()
//	{
//		return WaitTime;
//	}
	
//	/****************
//	 * 判断Socket是否连接
//	 * ************/
//	public bool GetSocket()
//	{
//		if(UnitySocket.Instance.getSocket() == null)
//		{
//			Debug.Log("GetSocket ==aaa");
//			return false;
//		}
//		if(!UnitySocket.Instance.getSocket().Connected)
//		{
//			Debug.Log("GetSocket ==ccc");
//			return false;
//		}
//		return true;
//	}
	//判断是否能够发送新消息;
	public bool GetSendServer(int message)
	{
		Debug.LogError("message = 0x"+ Convert.ToString(message, 16));
		if(getShieldMassage(message))
		{
			Debug.Log("1");
			return true;
		}
		
		return true;
	}
	//发送的消息是否有相应;
	public void SetServerInfo(int message)
	{
		if(!GetServerMessage(message))
		{
			return;
		}
	}
	
	private List<int> serverMessage;
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
	}
}
