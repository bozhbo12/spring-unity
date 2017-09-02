public class BossCardRes : MessageBody{
	private int remainHP; // 剩余血量 
	private int roleId; // 属于哪个角色 
	private int roleHeadCardId; // 属于哪个角色的头像 
	private int worldBossId; // bossId 
	private int deathCount; // BOSS死亡次数 
	private long appearTime; // 出现时间 
	private string roleName; // 角色名 
	private string attackMostRoleName; // 伤害最高的角色 
	private string killedRoleName; // 击杀的角色 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("remainHP",0);
		ps.add("roleId",0);
		ps.add("roleHeadCardId",0);
		ps.add("worldBossId",0);
		ps.add("deathCount",0);
		ps.add("appearTime",0);
		ps.addString("roleName","flashCode",0);
		ps.addString("attackMostRoleName","flashCode",0);
		ps.addString("killedRoleName","flashCode",0);
	}

	public int getRemainHP(){
		return remainHP;
	}

	public void setRemainHP(int remainHP){
		this.remainHP = remainHP;
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

	public int getRoleHeadCardId(){
		return roleHeadCardId;
	}

	public void setRoleHeadCardId(int roleHeadCardId){
		this.roleHeadCardId = roleHeadCardId;
	}

	public int getWorldBossId(){
		return worldBossId;
	}

	public void setWorldBossId(int worldBossId){
		this.worldBossId = worldBossId;
	}

	public int getDeathCount(){
		return deathCount;
	}

	public void setDeathCount(int deathCount){
		this.deathCount = deathCount;
	}

	public long getAppearTime(){
		return appearTime;
	}

	public void setAppearTime(long appearTime){
		this.appearTime = appearTime;
	}

	public string getRoleName(){
		return roleName;
	}

	public void setRoleName(string roleName){
		this.roleName = roleName;
	}

	public string getAttackMostRoleName(){
		return attackMostRoleName;
	}

	public void setAttackMostRoleName(string attackMostRoleName){
		this.attackMostRoleName = attackMostRoleName;
	}

	public string getKilledRoleName(){
		return killedRoleName;
	}

	public void setKilledRoleName(string killedRoleName){
		this.killedRoleName = killedRoleName;
	}

}