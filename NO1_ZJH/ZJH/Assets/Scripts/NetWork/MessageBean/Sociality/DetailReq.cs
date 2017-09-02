public class DetailReq : MessageBody{
	private int roleId; //角色Id;

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