using UnityEngine;
using System.Collections;

public class DetectionController : MonoBehaviour {
	public void Function(string ip)
	{
		StartCoroutine(DetetionNetwork(ip));
	}
	
	IEnumerator DetetionNetwork(string ip)
	{
		Ping p = new Ping(ip);
		
		yield return new  WaitForSeconds(3);
		
		SingletonDetectionNetWork.instance.TestResult(p.isDone);
		
		yield return null;
	}
}
