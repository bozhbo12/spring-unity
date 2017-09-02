public class ChatResp : MessageBody{
	private int result; // 处理结果;
	private int msgType; // 消息类型 私聊-1 世界-3 系统频道-6 系统消息-9;
	private int sendRoleId; // 发送者Id;
	private string sendRoleName; // 发送者名称;
	private int recRoleId; // 接收者Id;
	private string recRoleName; // 接收者名称;
	private string msgContent; // 消息内容;
    private long sendTime; // 发送时间;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("result",0);
		ps.add("msgType",0);
		ps.add("sendRoleId",0);
		ps.addString("sendRoleName","flashCode",0);
		ps.add("recRoleId",0);
		ps.addString("recRoleName","flashCode",0);
		ps.addString("msgContent","flashCode",0);
		ps.add("sendTime",0);
	}

	public int getResult(){
		return result;
	}

	public void setResult(int result){
		this.result = result;
	}

	public int getMsgType(){
		return msgType;
	}

	public void setMsgType(int msgType){
		this.msgType = msgType;
	}

	public int getSendRoleId(){
		return sendRoleId;
	}

	public void setSendRoleId(int sendRoleId){
		this.sendRoleId = sendRoleId;
	}

	public string getSendRoleName(){
		return sendRoleName;
	}

	public void setSendRoleName(string sendRoleName){
		this.sendRoleName = sendRoleName;
	}

	public int getRecRoleId(){
		return recRoleId;
	}

	public void setRecRoleId(int recRoleId){
		this.recRoleId = recRoleId;
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

	public long getSendTime(){
		return sendTime;
	}

	public void setSendTime(long sendTime){
		this.sendTime = sendTime;
	}

}