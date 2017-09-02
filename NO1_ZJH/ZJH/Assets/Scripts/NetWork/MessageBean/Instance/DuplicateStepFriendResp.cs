public class DuplicateStepFriendResp : MessageBody{
	private int duplicateId; // 下一副本Id;
	private int stepId; // 下一关卡Id;
	private int count; // 下一当前关卡步数;
	private int isNew; // 是否为最新副本 1-最新副本 0-之前副本;
	private int gold; // 掉落的金币;
	private int exp; // 掉落的经验;
	private int upActionValue; // 更新行动值;
	private int roleId; // 角色Id;
	private string roleName; // 角色名称;
	private int roleHeadId; // 角色头像Id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("duplicateId",0);
		ps.add("stepId",0);
		ps.add("count",0);
		ps.add("isNew",0);
		ps.add("gold",0);
		ps.add("exp",0);
		ps.add("upActionValue",0);
		ps.add("roleId",0);
		ps.addString("roleName","flashCode",0);
		ps.add("roleHeadId",0);
	}

	public int getDuplicateId(){
		return duplicateId;
	}

	public void setDuplicateId(int duplicateId){
		this.duplicateId = duplicateId;
	}

	public int getStepId(){
		return stepId;
	}

	public void setStepId(int stepId){
		this.stepId = stepId;
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

	public int getIsNew(){
		return isNew;
	}

	public void setIsNew(int isNew){
		this.isNew = isNew;
	}

	public int getGold(){
		return gold;
	}

	public void setGold(int gold){
		this.gold = gold;
	}

	public int getExp(){
		return exp;
	}

	public void setExp(int exp){
		this.exp = exp;
	}

	public int getUpActionValue(){
		return upActionValue;
	}

	public void setUpActionValue(int upActionValue){
		this.upActionValue = upActionValue;
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

	public string getRoleName(){
		return roleName;
	}

	public void setRoleName(string roleName){
		this.roleName = roleName;
	}

	public int getRoleHeadId(){
		return roleHeadId;
	}

	public void setRoleHeadId(int roleHeadId){
		this.roleHeadId = roleHeadId;
	}

}