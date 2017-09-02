// 0xA157
public class InviteCodeReq : MessageBody{
	private int roleId; //角色ID 
	private string inviteCode; //邀请码 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("roleId",0);
		ps.addString("inviteCode","flashCode",0);
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

	public string getInviteCode(){
		return inviteCode;
	}

	public void setInviteCode(string inviteCode){
		this.inviteCode = inviteCode;
	}

}