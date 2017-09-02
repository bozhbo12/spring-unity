public class FriendActionResp : MessageBody{
	private int roleId; 
	private int actionType; 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("roleId",0);
		ps.add("actionType",0);
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

	public int getActionType(){
		return actionType;
	}

	public void setActionType(int actionType){
		this.actionType = actionType;
	}

}