public class BossCoolDownResp : MessageBody{
	private int costRmbGold; // 消耗的点券;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("costRmbGold",0);
	}

	public int getCostRmbGold(){
		return costRmbGold;
	}

	public void setCostRmbGold(int costRmbGold){
		this.costRmbGold = costRmbGold;
	}

}