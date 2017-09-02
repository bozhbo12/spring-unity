public class Friend : MessageBody{
	private int roleId; //角色ID;
	private string name; //角色姓名;
	private int level; //角色等级;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("roleId",0);
		ps.addString("name","flashCode",0);
		ps.add("level",0);
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

	public string getName(){
		return name;
	}

	public void setName(string name){
		this.name = name;
	}

	public int getLevel(){
		return level;
	}

	public void setLevel( int level){
		this.level = level;
	}

}