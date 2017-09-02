public class UserLoginResp : MessageBody{
	private int result; // 登录结果 1-成功 0-无角色;
	private int roleId; // 角色Id;
	private string roleName; // 角色名称;
	private int allianceId; //;
	private int chatStatus; // 聊天状态;
	private long chatTime; // 聊天时间;
	private int vendorId; //;
	private int loginFlag; // 登录标志;
	private int gateServerId; // 接入服务器Id;
	private int fightServerId; // 战斗服务器Id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("result",0);
		ps.add("roleId",0);
		ps.addString("roleName","flashCode",0);
		ps.add("allianceId",0);
		ps.add("chatStatus",0);
		ps.add("chatTime",0);
		ps.add("vendorId",0);
		ps.add("loginFlag",0);
		ps.add("gateServerId",0);
		ps.add("fightServerId",0);
	}

	public int getResult(){
		return result;
	}

	public void setResult(int result){
		this.result = result;
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

	public int getAllianceId(){
		return allianceId;
	}

	public void setAllianceId(int allianceId){
		this.allianceId = allianceId;
	}

	public int getChatStatus(){
		return chatStatus;
	}

	public void setChatStatus(int chatStatus){
		this.chatStatus = chatStatus;
	}

	public long getChatTime(){
		return chatTime;
	}

	public void setChatTime(long chatTime){
		this.chatTime = chatTime;
	}

	public int getVendorId(){
		return vendorId;
	}

	public void setVendorId(int vendorId){
		this.vendorId = vendorId;
	}

	public int getLoginFlag(){
		return loginFlag;
	}

	public void setLoginFlag(int loginFlag){
		this.loginFlag = loginFlag;
	}

	public int getGateServerId(){
		return gateServerId;
	}

	public void setGateServerId(int gateServerId){
		this.gateServerId = gateServerId;
	}

	public int getFightServerId(){
		return fightServerId;
	}

	public void setFightServerId(int fightServerId){
		this.fightServerId = fightServerId;
	}

}