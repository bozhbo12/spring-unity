public class HeadResp : MessageBody{
	private int headCardId; // 头像卡牌ID 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("headCardId",0);
	}

	public int getHeadCardId(){
		return headCardId;
	}

	public void setHeadCardId(int headCardId){
		this.headCardId = headCardId;
	}

}