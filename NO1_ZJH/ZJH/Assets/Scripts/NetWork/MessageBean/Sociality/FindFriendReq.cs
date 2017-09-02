public class FindFriendReq : MessageBody{
	private string roleName; //角色名;
	private int findType; //查找类型  1--随机  2--根据角色名;

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("roleName","flashCode",0);
		ps.add("findType",0);
	}

	public string getRoleName(){
		return roleName;
	}

	public void setRoleName(string roleName){
		this.roleName = roleName;
	}

	public int getFindType(){
		return findType;
	}

	public void setFindType(int findType){
		this.findType = findType;
	}

}