public class DelFriendResp : MessageBody{
	private int roleId; // 被删除的好友ID;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("roleId",0);
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

}