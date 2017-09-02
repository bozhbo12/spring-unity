public class AuctionQueryReq : MessageBody{
	private string cardName; // 卡牌名称 可为空;
	private int cardStar; // 卡牌星级 可为0;
	private int index; // 页数 第一页为0;
	private int queryType; // 查询类型 1-查询自己拍卖卡牌;

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("cardName","flashCode",0);
		ps.add("cardStar",0);
		ps.add("index",0);
		ps.add("queryType",0);
	}

	public string getCardName(){
		return cardName;
	}

	public void setCardName(string cardName){
		this.cardName = cardName;
	}

	public int getCardStar(){
		return cardStar;
	}

	public void setCardStar(int cardStar){
		this.cardStar = cardStar;
	}

	public int getIndex(){
		return index;
	}

	public void setIndex(int index){
		this.index = index;
	}

	public int getQueryType(){
		return queryType;
	}

	public void setQueryType(int queryType){
		this.queryType = queryType;
	}

}