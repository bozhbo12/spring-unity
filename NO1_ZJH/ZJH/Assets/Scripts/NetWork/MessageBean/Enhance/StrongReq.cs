public class StrongReq : MessageBody{
	private long cardSeqId; //被强化的卡牌ID
	private string cardSeqIds; //经验卡

	public override void setSequnce(ProtocolSequence ps){
		ps.add("cardSeqId",0);
		ps.addString("cardSeqIds","flashCode",0);
	}

	public long getCardSeqId(){
		return cardSeqId;
	}

	public void setCardSeqId(long cardSeqId){
		this.cardSeqId = cardSeqId;
	}

	public string getCardSeqIds(){
		return cardSeqIds;
	}

	public void setCardSeqIds( string cardSeqIds){
		this.cardSeqIds = cardSeqIds;
	}

}