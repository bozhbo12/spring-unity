public class CreatRoleReq : MessageBody{
	private int IP; // 客户端Ip;
	private string account; // 账号;
	private string MD5Pass; //加密密码;
	private int cilentType; // 客户端类型;
	private string roleName; // 角色名称;
	private int verifyType; // 验证类型;
	private string extendPass; // 扩张密码;
	private string validate; // 验证串;
	private string loginFlag; // 登录标志;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("IP",0);
		ps.addString("account","flashCode",0);
		ps.addString("MD5Pass","flashCode",0);
		ps.add("cilentType",0);
		ps.addString("roleName","flashCode",0);
		ps.add("verifyType",0);
		ps.addString("extendPass","flashCode",0);
		ps.addString("validate","flashCode",0);
		ps.addString("loginFlag","flashCode",0);
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

	public string getMD5Pass(){
		return MD5Pass;
	}

	public void setMD5Pass(string MD5Pass){
		this.MD5Pass = MD5Pass;
	}

	public int getCilentType(){
		return cilentType;
	}

	public void setCilentType(int cilentType){
		this.cilentType = cilentType;
	}

	public string getRoleName(){
		return roleName;
	}

	public void setRoleName(string roleName){
		this.roleName = roleName;
	}

//	public int getCardId(){
//		return cardId;
//	}
//
//	public void setCardId(int cardId){
//		this.cardId = cardId;
//	}

	public int getVerifyType(){
		return verifyType;
	}

	public void setVerifyType(int verifyType){
		this.verifyType = verifyType;
	}

	public string getExtendPass(){
		return extendPass;
	}

	public void setExtendPass(string extendPass){
		this.extendPass = extendPass;
	}

	public string getValidate(){
		return validate;
	}

	public void setValidate(string validate){
		this.validate = validate;
	}

	public string getLoginFlag(){
		return loginFlag;
	}

	public void setLoginFlag(string loginFlag){
		this.loginFlag = loginFlag;
	}

}