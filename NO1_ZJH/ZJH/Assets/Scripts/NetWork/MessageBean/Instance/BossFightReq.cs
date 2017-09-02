public class BossFightReq : MessageBody{
	private int bossType; // BOSS类型 1-世界BOSS 2-关底BOSS 3-精英BOSS;
	private int roleId; // 角色Id(世界BOSS时使用) duplicateId(精英BOSS时使用);

	public override void setSequnce(ProtocolSequence ps){
		ps.add("bossType",0);
		ps.add("roleId",0);
	}

	public int getBossType(){
		return bossType;
	}

	public void setBossType(int bossType){
		this.bossType = bossType;
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

}