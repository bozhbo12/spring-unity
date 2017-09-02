public class FriendReceiveResp : MessageBody{
	private string roleName; //角色名;
	private int roleLevel; //角色等级;

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("roleName","flashCode",0);
		ps.add("roleLevel",0);
	}

	public string getRoleName(){
		return roleName;
	}

	public void setRoleName(string roleName){
		this.roleName = roleName;
	}

	public int getRoleLevel(){
		return roleLevel;
	}

	public void setRoleLevel( int roleLevel){
		this.roleLevel = roleLevel;
	}

}