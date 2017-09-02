using UnityEngine;
using System.Collections;
using System.Net;

public class SingletonDetectionNetWork{

	private static SingletonDetectionNetWork mDetetctonNetwork;
	
	private static object sRoot = new object();
	
	public static SingletonDetectionNetWork instance
	{
		get
		{
			if(mDetetctonNetwork == null)
			{
				lock(sRoot)
				{
					if(mDetetctonNetwork == null)
					{
						mDetetctonNetwork = new SingletonDetectionNetWork();
					}
				}
			}	
			return mDetetctonNetwork;
		}
	}
	
	private SingletonDetectionNetWork()
	{
		
	}
	
	
	//当前网络状态;
	public enum STATE_NETWORK
	{
		WIFT,
		GPS,
		NULL,
	}
	
	private STATE_NETWORK mState;
	public STATE_NETWORK GetStateNetwork()
	{
		return mState;
	}
	private bool SetStateNetwork(STATE_NETWORK mStates)
	{
		mState = mStates;
		if(mState == STATE_NETWORK.NULL)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	
	
	public delegate void NetWorkFunction();
	
	//回调方法;
	public NetWorkFunction mNetWorkFunction;
	
	public string GetIP(string ulr)
	{
		int index = ulr.Substring(7).IndexOf("/");
		string ip = ulr.Substring(7,index);
		return ip;
	}
	/// <summary>
	/// 检查当前网络连接情况;
	/// </summary>
	/// <param name='ip'>
	/// 需要连接的域名;
	/// </param>
	/// <param name='thisFunction'>
	/// 连接成功回调的方法;
	/// </param>
	public void DetetionFunction(string path ,NetWorkFunction thisFunction)
	{
		if (!SetStateNetwork(NetworkEnvironment())) {
			TestResult(false);
			return;
		}
		int index = path.Substring(7).IndexOf("/");
		string ip = path.Substring(7,index);
		string ulr;
		
		if(ip.Equals("192.168.6.227") || ip.Equals("172.19.61.74"))
		{
			ulr = ip;
		}
		else
		{
			IPAddress[] ips = Dns.GetHostAddresses(ip);
			ulr = ips[0].ToString();
		}
		if(ulr == null || ulr == "127.0.0.1")
		{
			TestResult(false);
			return;
		}
		else
		{
			mNetWorkFunction = thisFunction;
			ConnetNetwork(ulr);
		}
		
	}
    public void WifiNetwork()
    {
        if (!SetStateNetwork(NetworkEnvironment()))
        {
//            SingletonErrorMessage.instance.ShowClenOn(ErrorCodeManager.Instance().GetContext(30016));
        }
        else 
        {
            if (mState == STATE_NETWORK.GPS)
            {
//                SingletonErrorMessage.instance.ShowClenOn(ErrorCodeManager.Instance().GetContext(30017));
            }
        }
    }
	
	public bool IS_Connot;
	DetectionController mDetetctionController;
	private void ConnetNetwork(string ip)
	{
		if(mDetetctionController == null)
		{
			mDetetctionController = new GameObject().AddComponent<DetectionController>();
			mDetetctionController.name = "Network";
		}
		mDetetctionController.Function(ip);
	}
	
	private void PlayFunction()
	{
		if(mNetWorkFunction != null){
			mNetWorkFunction();
		}
		mNetWorkFunction = null;
	}
	
	//网络ip的结果;
	public void TestResult(bool isBool)
	{
        //PlayFunction();
        //return;
		
		if(isBool)
		{
			if(GetStateNetwork() == STATE_NETWORK.WIFT)
			{
				PlayFunction();
			}
			else if(GetStateNetwork() == STATE_NETWORK.GPS)
			{
				//资源更新需要提醒;
//				if(SingletonLoad.instance.mState == SingletonLoad.MSTATE.STATE_Down )
//				{
//					SingletonErrorMessage.instance.ShowClenOn(PlayFunction ,ErrorCodeManager.Instance().GetContext(30014)
//						,PropertyManager.Instance().getValue("SetCard_Sure"));
//				}
//				else
//				{
//					PlayFunction();
//				}
			}
		}
		else
		{
			//网络连接异常,返回登入界面;
//			if(SingletonLoad.instance.mState == SingletonLoad.MSTATE.STATE_START || SingletonLoad.instance.mState == SingletonLoad.MSTATE.STATE_Down)
//			{
//				SingletonErrorMessage.instance.ShowClenOn(ErrorCodeManager.Instance().GetContext(30013));
//			}
//			else{
//				SingletonErrorMessage.instance.ShowClenOn(SingletonMessage.instance.GotoLogin,ErrorCodeManager.Instance().GetContext(30013));
//			}
		}
	}
	
	//判断硬件网络连接的状态;
	private STATE_NETWORK NetworkEnvironment()
	{
		if(Application.internetReachability == NetworkReachability.NotReachable)
		{
			return STATE_NETWORK.NULL;
		}
		else if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
		{
			return STATE_NETWORK.GPS;
		}
		else if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
		{
			return STATE_NETWORK.WIFT;
		}
		else
		{
			return STATE_NETWORK.NULL;
		}
	}
	
	
}
