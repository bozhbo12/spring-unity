public class CheckFriendReq : MessageBody{
	private int friendRoleId; //好友角色ID;
	private int checkType; //1-同意 otherwise-拒绝;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("friendRoleId",0);
		ps.add("checkType",0);
	}

	public int getFriendRoleId(){
		return friendRoleId;
	}

	public void setFriendRoleId(int friendRoleId){
		this.friendRoleId = friendRoleId;
	}

	public int getCheckType(){
		return checkType;
	}

	public void setCheckType(int checkType){
		this.checkType = checkType;
	}

}