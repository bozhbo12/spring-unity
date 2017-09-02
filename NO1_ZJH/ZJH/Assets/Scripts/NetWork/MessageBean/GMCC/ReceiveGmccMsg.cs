public class ReceiveGmccMsg : MessageBody{
	private byte flag; 
	private int gmId; 
	private string gmNickname; 
	private string content; 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("flag",0);
		ps.add("gmId",0);
		ps.addString("gmNickname","flashCode",0);
		ps.addString("content","flashCode",0);
	}

	public byte getFlag(){
		return flag;
	}

	public void setFlag(byte flag){
		this.flag = flag;
	}

	public int getGmId(){
		return gmId;
	}

	public void setGmId(int gmId){
		this.gmId = gmId;
	}

	public string getGmNickname(){
		return gmNickname;
	}

	public void setGmNickname(string gmNickname){
		this.gmNickname = gmNickname;
	}

	public string getContent(){
		return content;
	}

	public void setContent(string content){
		this.content = content;
	}

}