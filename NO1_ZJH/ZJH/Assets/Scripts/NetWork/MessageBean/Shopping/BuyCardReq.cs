public class BuyCardReq : MessageBody{
	private int id; // 卡包id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("id",0);
	}

	public int getId(){
		return id;
	}

	public void setId(int id){
		this.id = id;
	}

}