public class UserVerifyResp : MessageBody{
	private int result; 
	private string account; 
	private int serverId; 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("result",0);
		ps.addString("account","flashCode",0);
		ps.add("serverId",0);
	}

	public int getResult(){
		return result;
	}

	public void setResult(int result){
		this.result = result;
	}

	public string getAccount(){
		return account;
	}

	public void setAccount(string account){
		this.account = account;
	}

	public int getServerId(){
		return serverId;
	}

	public void setServerId(int serverId){
		this.serverId = serverId;
	}

}