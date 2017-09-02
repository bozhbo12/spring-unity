public class AgreeOrNotResp : MessageBody{
	private string friendName; //好友名字;
	private int isAgree; // 1--同意 otherwise --不同意;

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("friendName","flashCode",0);
		ps.add("isAgree",0);
	}

	public string getFriendName(){
		return friendName;
	}

	public void setFriendName(string friendName){
		this.friendName = friendName;
	}

	public int getIsAgree(){
		return isAgree;
	}

	public void setIsAgree(int isAgree){
		this.isAgree = isAgree;
	}

}