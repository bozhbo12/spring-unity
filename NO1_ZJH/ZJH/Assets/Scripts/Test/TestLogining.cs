using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestLogining : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GlobalConfig.GAME_SERVER_PORT = new int[1];
		GlobalConfig.GAME_SERVER_PORT[0]=8088;
		bool isLoad = NetMessageConfig.getInstance().loadConfig();
		Debug.LogError("isLoad == "+isLoad);
		Client.Instance().init();
		//NetMessageDispather.Register(ServerMessage.SERVER_LOGIN_RESP, OnRespLogin);
	}
	
	// Update is called once per frame
	void Update () {
		Client.NetMessageDispather.OnUpdate();
	}
	void OnGUI()
	{
		if(GUILayout.Button("init"))
		{
			Client.Instance().Connect();
		}

		if (GUILayout.Button("Login")) 
		{
			LoginIng();
		}

		if(GUILayout.Button("OUT"))
		{
			Client.Instance().Close();
			UnitySocket.Instance.DisconnectNow();
			Destroy(this);
		}
	}

	bool LoginIng()
	{
		UserLoginReq req = new UserLoginReq();
		req.setIP(IPtoInt("192.168.0.11"));
		req.setAccount("accountNO1");
		req.setMd5Pass("");
		req.setValidate("");
		req.setCilentType(1);
		req.setMac("1212121212");
		req.setPackageName("2323232323232");

		return NetUtils.Send(ServerMessage.SERVER_LOGIN_REQ, req);
	}

	public static int IPtoInt(string ip) {
		int res = 0;
		if (ip != null && ip.Length > 0) {
			string[] aip = ip.Split('.');
			if (aip != null && aip.Length == 4) {
				for (int i = 0; i < 4; i++) {
					res = res + int.Parse(aip[i]);
					if (i < 3)
						res = res << 8;
				}
			}
		}
		return res;
	}

	void OnRespLogin()
	{
		
	}
}
