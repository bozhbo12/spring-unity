public class GmReq : MessageBody{
	private int reqType; // 请求类型;
	private string value; // 请求值;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("reqType",0);
		ps.addString("value","flashCode",0);
	}

	public int getReqType(){
		return reqType;
	}

	public void setReqType(int reqType){
		this.reqType = reqType;
	}

	public string getValue(){
		return value;
	}

	public void setValue(string value){
		this.value = value;
	}

}