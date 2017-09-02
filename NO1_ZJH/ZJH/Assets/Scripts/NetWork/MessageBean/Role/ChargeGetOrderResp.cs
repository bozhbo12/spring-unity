public class ChargeGetOrderResp : MessageBody{
	private string info; // 订单信息 
	private int itemId; // 商品ID 

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("info","flashCode",0);
		ps.add("itemId",0);
	}

	public string getInfo(){
		return info;
	}

	public void setInfo(string info){
		this.info = info;
	}

	public int getItemId(){
		return itemId;
	}

	public void setItemId(int itemId){
		this.itemId = itemId;
	}

}