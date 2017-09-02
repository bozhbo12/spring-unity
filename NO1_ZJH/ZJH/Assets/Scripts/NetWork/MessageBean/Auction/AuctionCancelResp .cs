public class AuctionCancelResp  : MessageBody{
	private long cardSeqId; // 角色物品id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("cardSeqId",0);
	}

	public long getCardSeqId(){
		return cardSeqId;
	}

	public void setCardSeqId(long cardSeqId){
		this.cardSeqId = cardSeqId;
	}

}