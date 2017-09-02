public class SellResp : MessageBody{
	private string beSelledSeqId; // 被售卖的卡 

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("beSelledSeqId","flashCode",0);
	}

	public string getBeSelledSeqId(){
		return beSelledSeqId;
	}

	public void setBeSelledSeqId(string beSelledSeqId){
		this.beSelledSeqId = beSelledSeqId;
	}

}