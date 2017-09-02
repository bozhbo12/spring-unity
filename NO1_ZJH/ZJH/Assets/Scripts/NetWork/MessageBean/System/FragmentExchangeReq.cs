// 0xA161
public class FragmentExchangeReq : MessageBody{
	private int roleId; // 角色ID 
	private int fragmentId; // 碎片ID 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("roleId",0);
		ps.add("fragmentId",0);
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

	public int getFragmentId(){
		return fragmentId;
	}

	public void setFragmentId(int fragmentId){
		this.fragmentId = fragmentId;
	}

}