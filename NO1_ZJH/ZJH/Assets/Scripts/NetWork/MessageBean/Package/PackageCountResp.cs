public class PackageCountResp : MessageBody{
	private int count; 
	private int loginTimes; 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("count",0);
		ps.add("loginTimes",0);
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

	public int getLoginTimes(){
		return loginTimes;
	}

	public void setLoginTimes(int loginTimes){
		this.loginTimes = loginTimes;
	}

}