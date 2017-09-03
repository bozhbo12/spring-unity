public class UserLoginReq : MessageBody{
	private int IP; //客户端IP;
	private string account; //玩家登陆帐号;
	private string md5Pass; //MD5密码;
	private string validate; // 角色验证串
	private int cilentType;//// 客户端类型 1:android 2:ios 3:pc
	private string mac;// 客户端mac地址
	private string packageName;// 客户端包名

//	private int verifyType; //验证类型;
//	private string extendPass; 
//	private string loginFlag; 
//	 
//	private string Reserved; 
//	private string accounts;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("IP",0);
		ps.addString("account","flashCode",0);
		ps.addString("md5Pass","flashCode",0);
		//ps.add("verifyType",0);
		//ps.addString("extendPass","flashCode",0);
		//ps.addString("loginFlag","flashCode",0);
		ps.addString("validate","flashCode",0);
		ps.add("cilentType",0);
		ps.addString("mac","flashCode",0);
		ps.addString("packageName","flashCode",0);
		//ps.addString("Reserved","flashCode",0);
		//ps.addString("accounts","flashCode",0);
	}

	public int getIP(){
		return IP;
	}

	public void setIP(int IP){
		this.IP = IP;
	}

	public string getAccount(){
		return account;
	}

	public void setAccount(string account){
		this.account = account;
	}

	public string getMd5Pass(){
		return md5Pass;
	}

	public void setMd5Pass(string md5Pass){
		this.md5Pass = md5Pass;
	}
//
//	public int getVerifyType(){
//		return verifyType;
//	}
//
//	public void setVerifyType(int verifyType){
//		this.verifyType = verifyType;
//	}
//
//	public string getExtendPass(){
//		return extendPass;
//	}
//
//	public void setExtendPass(string extendPass){
//		this.extendPass = extendPass;
//	}
//
//	public string getLoginFlag(){
//		return loginFlag;
//	}
//
//	public void setLoginFlag(string loginFlag){
//		this.loginFlag = loginFlag;
//	}

	public string getValidate(){
		return validate;
	}

	public void setValidate(string validate){
		this.validate = validate;
	}

	public int getCilentType(){
		return cilentType;
	}

	public void setCilentType(int cilentType){
		this.cilentType = cilentType;
	}

//	public string getReserved(){
//		return Reserved;
//	}
//
//	public void setReserved(string Reserved){
//		this.Reserved = Reserved;
//	}
//	
//	public string getAccounts()
//	{
//		return accounts;
//	}
//	
//	public void setAccounts(string accounts)
//	{
//		this.accounts = accounts;
//	}

	public string getMac(){
		return mac;
	}

	public void setMac(string mac){
		this.mac = mac;
	}

	public string getPackageName(){
		return packageName;
	}

	public void setPackageName(string packageName){
		this.packageName = packageName;
	}

}