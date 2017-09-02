public class FriendActionReq : MessageBody{
	private int friendRoleId; // 好友ID;
	private int actionType; // 行动力类型  1-赠送 2-接收;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("friendRoleId",0);
		ps.add("actionType",0);
	}

	public int getFriendRoleId(){
		return friendRoleId;
	}

	public void setFriendRoleId(int friendRoleId){
		this.friendRoleId = friendRoleId;
	}

	public int getActionType(){
		return actionType;
	}

	public void setActionType(int actionType){
		this.actionType = actionType;
	}

}