public class ChatReq : MessageBody{
	private int msgType; // 消息类型 私聊-1 世界-3;
	private string sendRoleName; // 发送者名称;
	private string recRoleName; // 接受者名称;
	private string msgContent; // 消息内容;
	private string vendorId = "-1"; // 忽略;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("msgType",0);
		ps.addString("sendRoleName","flashCode",0);
		ps.addString("recRoleName","flashCode",0);
		ps.addString("msgContent","flashCode",0);
		ps.addString("vendorId","flashCode",0);
	}

	public int getMsgType(){
		return msgType;
	}

	public void setMsgType(int msgType){
		this.msgType = msgType;
	}

	public string getSendRoleName(){
		return sendRoleName;
	}

	public void setSendRoleName(string sendRoleName){
		this.sendRoleName = sendRoleName;
	}

	public string getRecRoleName(){
		return recRoleName;
	}

	public void setRecRoleName(string recRoleName){
		this.recRoleName = recRoleName;
	}

	public string getMsgContent(){
		return msgContent;
	}

	public void setMsgContent(string msgContent){
		this.msgContent = msgContent;
	}

	public string getVendorId(){
		return vendorId;
	}

	public void setVendorId(string vendorId){
		this.vendorId = vendorId ;
	}

}