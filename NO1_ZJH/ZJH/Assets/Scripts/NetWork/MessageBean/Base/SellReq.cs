public class SellReq : MessageBody{
	private string cardSeqIds; //出售的卡牌，逗号拼接 

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("cardSeqIds","flashCode",0);
	}

	public string getCardSeqIds(){
		return cardSeqIds;
	}

	public void setCardSeqIds(string cardSeqIds){
		this.cardSeqIds = cardSeqIds;
	}

}