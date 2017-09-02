public class DuplicateBossListResp : MessageBody{
	private int buyCount; // 当天购买Boss次数;
	private string attackCount; // 攻击Boss次数(为空表示无修改) 格式:;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("buyCount",0);
		ps.addString("attackCount","flashCode",0);
	}

	public int getBuyCount(){
		return buyCount;
	}

	public void setBuyCount(int buyCount){
		this.buyCount = buyCount;
	}

	public string getAttackCount(){
		return attackCount;
	}

	public void setAttackCount(string attackCount){
		this.attackCount = attackCount;
	}

}