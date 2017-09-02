public class UpgradeReq : MessageBody{
	private long cardSeqId; // 被进阶的卡牌ID;
	private long expCardSeqId; // 经验卡;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("cardSeqId",0);
		ps.add("expCardSeqId",0);
	}

	public long getCardSeqId(){
		return cardSeqId;
	}

	public void setCardSeqId(long cardSeqId){
		this.cardSeqId = cardSeqId;
	}

	public long getExpCardSeqId(){
		return expCardSeqId;
	}

	public void setExpCardSeqId(long expCardSeqId){
		this.expCardSeqId = expCardSeqId;
	}

}