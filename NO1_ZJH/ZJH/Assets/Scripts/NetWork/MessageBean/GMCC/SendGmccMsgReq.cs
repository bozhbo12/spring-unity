public class SendGmccMsgReq : MessageBody{
	private byte flag; 
	private string content; 
	private string addContent; 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("flag",0);
		ps.addString("content","flashCode",0);
		ps.addString("addContent","flashCode",0);
	}

	public byte getFlag(){
		return flag;
	}

	public void setFlag(byte flag){
		this.flag = flag;
	}

	public string getContent(){
		return content;
	}

	public void setContent(string content){
		this.content = content;
	}

	public string getAddContent(){
		return addContent;
	}

	public void setAddContent(string addContent){
		this.addContent = addContent;
	}

}