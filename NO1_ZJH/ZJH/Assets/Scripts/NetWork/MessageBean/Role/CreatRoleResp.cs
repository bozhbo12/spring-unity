public class CreatRoleResp : MessageBody{
	private int result; // 结果;
	private int loginFlag; // 登录标志;
	private int roleId; // 角色Id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("result",0);
		ps.add("loginFlag",0);
		ps.add("roleId",0);
	}

	public int getResult(){
		return result;
	}

	public void setResult(int result){
		this.result = result;
	}

	public int getLoginFlag(){
		return loginFlag;
	}

	public void setLoginFlag(int loginFlag){
		this.loginFlag = loginFlag;
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

}