public class AuctionBuyResp : MessageBody{
	private int remainRmbGold; // 剩余点券;
	private long cardSeqId; // 角色物品id;
	private int cardId; // 卡牌Id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("remainRmbGold",0);
		ps.add("cardSeqId",0);
		ps.add("cardId",0);
	}

	public int getRemainRmbGold(){
		return remainRmbGold;
	}

	public void setRemainRmbGold(int remainRmbGold){
		this.remainRmbGold = remainRmbGold;
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