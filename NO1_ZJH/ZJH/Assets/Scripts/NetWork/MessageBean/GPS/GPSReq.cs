/// <summary>
/// GPS request.
/// </summary>

using UnityEngine;
using System.Collections;

public class GPSReq : MessageBody{

	private double jingdu;
	private double weidu;
	private string country;
	private string firstLocal;
	private string secondLocal;
	private string thirdLocal;
	
	public override void setSequnce(ProtocolSequence ps) {
		ps.add("jingdu", 0);
		ps.add("weidu", 0);
		ps.addString("country", "flashCode", 0);
		ps.addString("firstLocal", "flashCode", 0);
		ps.addString("secondLocal", "flashCode", 0);
		ps.addString("thirdLocal", "flashCode", 0);
	}

	public double getJingdu() {
		return jingdu;
	}

	public void setJingdu(double jingdu) {
		this.jingdu = jingdu;
	}

	public double getWeidu() {
		return weidu;
	}

	public void setWeidu(double weidu) {
		this.weidu = weidu;
	}

	public string getCountry() {
		return country;
	}

	public void setCountry(string country) {
		this.country = country;
	}

	public string getFirstLocal() {
		return firstLocal;
	}

	public void setFirstLocal(string firstLocal) {
		this.firstLocal = firstLocal;
	}

	public string getSecondLocal() {
		return secondLocal;
	}

	public void setSecondLocal(string secondLocal) {
		this.secondLocal = secondLocal;
	}

	public string getThirdLocal() {
		return thirdLocal;
	}

	public void setThirdLocal(string thirdLocal) {
		this.thirdLocal = thirdLocal;
	}

}
