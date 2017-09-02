public class AuctionBuyReq : MessageBody{
	private long cardSeqId; // 角色物品id;
	private int roleId; // 卖方角色Id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("cardSeqId",0);
		ps.add("roleId",0);
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

} 