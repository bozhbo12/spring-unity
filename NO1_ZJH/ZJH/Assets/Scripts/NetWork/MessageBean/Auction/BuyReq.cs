public class BuyReq : MessageBody{
	private int buyType; // 购买类型 1-战斗值 2-行动值 3-cardContainLimit(角色卡组上限);;
	private int num; // 购买数量;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("buyType",0);
		ps.add("num",0);
	}

	public int getBuyType(){
		return buyType;
	}

	public void setBuyType(int buyType){
		this.buyType = buyType;
	}

	public int getNum(){
		return num;
	}

	public void setNum(int num){
		this.num = num;
	}

}