public class CardReq : MessageBody{
	private long cardSeqId; // 卡牌唯一Id 
	private byte position; // 卡牌位置 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("cardSeqId",0);
		ps.add("position",0);
	}

	public long getCardSeqId(){
		return cardSeqId;
	}

	public void setCardSeqId(long cardSeqId){
		this.cardSeqId = cardSeqId;
	}

	public byte getPosition(){
		return position;
	}

	public void setPosition(byte position){
		this.position = position;
	}

}