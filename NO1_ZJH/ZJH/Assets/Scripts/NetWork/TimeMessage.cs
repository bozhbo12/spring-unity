using UnityEngine;
using System.Collections;

public class TimeMessage : MonoBehaviour {
	
	
	private float newTime = 0;
	public bool loading = false;

	//打开;
	public void OpenLoading()
	{
		Invoke("ShowLoading",1f);
	}
	//显示出;
	private void ShowLoading()
	{
		if(SingletonMessage.instance.GetConnetState != SingletonMessage.ConnetState.NetWork_Ing)
		{
			return;
		}
		newTime = Time.time;
		loading = true;
//		UI.Instance().OpenLoadingUI();
	}
	//关闭;
	public void CloseLoading()
	{
		loading = false;
//		UI.Instance().CloseLoadingUI();
	}
	//运行loading
	private void UpdataLoading()
	{
		if(loading)
		{
			if(SingletonMessage.instance.GetConnetState != SingletonMessage.ConnetState.NetWork_Ing)
			{
				CloseLoading();
			}
			if(Time.time > newTime+SingletonMessage.instance.mWaitTime)
			{
				newTime = Time.time;
				SingletonMessage.instance.DeduceNetWork(false);
			}
		}
	}
	void Update () {
		UpdataLoading();
	}
}
