public class RoleConsumePointResp : MessageBody{
	private int rmbGold; // 点券 
	private int remainCount; // 剩余点数 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("rmbGold",0);
		ps.add("remainCount",0);
	}

	public int getRmbGold(){
		return rmbGold;
	}

	public void setRmbGold(int rmbGold){
		this.rmbGold = rmbGold;
	}

	public int getRemainCount(){
		return remainCount;
	}

	public void setRemainCount(int remainCount){
		this.remainCount = remainCount;
	}

}