public class DuplicateBuyResp : MessageBody{
	private int duplicateId; // 副本Id;
	private int buyCount; // 购买次数;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("duplicateId",0);
		ps.add("buyCount",0);
	}

	public int getDuplicateId(){
		return duplicateId;
	}

	public void setDuplicateId(int duplicateId){
		this.duplicateId = duplicateId;
	}

	public int getBuyCount(){
		return buyCount;
	}

	public void setBuyCount(int buyCount){
		this.buyCount = buyCount;
	}

}