public class RolePlayReq : MessageBody{
	private string deviceId; // 设备号 
	private int deviceType; // 1-IOS,2-ANDRIOD 
	private string version; // 版本号 

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("deviceId","flashCode",0);
		ps.add("deviceType",0);
		ps.addString("version","flashCode",0);
	}

	public string getDeviceId(){
		return deviceId;
	}

	public void setDeviceId(string deviceId){
		this.deviceId = deviceId;
	}

	public int getDeviceType(){
		return deviceType;
	}

	public void setDeviceType(int deviceType){
		this.deviceType = deviceType;
	}

	public string getVersion(){
		return version;
	}

	public void setVersion(string version){
		this.version = version;
	}

}