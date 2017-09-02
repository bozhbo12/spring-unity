public class AuctionDisplayInfo : MessageBody{
	private long cardSeqId; // 卡牌唯一Id;
	private int roleId; // 角色Id;
	private int cardId; // 卡牌Id;
	private int level; // 卡牌等级;
	private int price; // 卡牌价格;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("cardSeqId",0);
		ps.add("roleId",0);
		ps.add("cardId",0);
		ps.add("level",0);
		ps.add("price",0);
	}

	public long getCardSeqId(){
		return cardSeqId;
	}

	public void setCardSeqId(long cardSeqId){
		this.cardSeqId = cardSeqId;
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

	public int getCardId(){
		return cardId;
	}

	public void setCardId(int cardId){
		this.cardId = cardId;
	}

	public int getLevel(){
		return level;
	}

	public void setLevel(int level){
		this.level = level;
	}

	public int getPrice(){
		return price;
	}

	public void setPrice(int price){
		this.price = price;
	}

}