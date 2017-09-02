public class UserVerifyReq : MessageBody{
	private int IP; 
	private string account; 
	private string md5Pass; 
	private int verifyType; 
	private string extendPass; 
	private string validate; 
	private int clientType; 
	private string Reserved; 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("IP",0);
		ps.addString("account","flashCode",0);
		ps.addString("md5Pass","flashCode",0);
		ps.add("verifyType",0);
		ps.addString("extendPass","flashCode",0);
		ps.addString("validate","flashCode",0);
		ps.add("clientType",0);
		ps.addString("Reserved","flashCode",0);
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

	public int getClientType(){
		return clientType;
	}

	public void setClientType(int clientType){
		this.clientType = clientType;
	}

	public string getReserved(){
		return Reserved;
	}

	public void setReserved(string Reserved){
		this.Reserved = Reserved;
	}

}