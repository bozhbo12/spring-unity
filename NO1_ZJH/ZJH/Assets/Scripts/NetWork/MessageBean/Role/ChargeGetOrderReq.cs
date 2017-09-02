public class ChargeGetOrderReq : MessageBody{
	private int itemId; //商品ID 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("itemId",0);
	}

	public int getItemId(){
		return itemId;
	}

	public void setItemId(int itemId){
		this.itemId = itemId;
	}

}