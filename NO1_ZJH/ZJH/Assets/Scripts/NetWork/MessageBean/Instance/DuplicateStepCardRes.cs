public class DuplicateStepCardRes : MessageBody{
	private long cardSeqId; // 掉落卡牌唯一Id;
	private int cardId; // 掉落卡牌Id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("cardSeqId",0);
		ps.add("cardId",0);
	}

	public long getCardSeqId(){
		return cardSeqId;
	}

	public void setCardSeqId(long cardSeqId){
		this.cardSeqId = cardSeqId;
	}

	public int getCardId(){
		return cardId;
	}

	public void setCardId(int cardId){
		this.cardId = cardId;
	}

}