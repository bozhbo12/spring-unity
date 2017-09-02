// 0xA022
public class FriendRes : MessageBody{
	private int roleId; // 角色ID 
	private string name; // 角色姓名 
	private int level; // 角色等级 
	private int headCardId; // 头像卡牌ID 
	private long lastLoginTime; // 上次登陆时间 
	private int friendNum; // 好友数量 
	private int hasSended; // 1-已发送 else - 未发送 
	private int hasReceived; // 1-未收取 else -已收取 
	private int positionType; // 阵型 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("roleId",0);
		ps.addString("name","flashCode",0);
		ps.add("level",0);
		ps.add("headCardId",0);
		ps.add("lastLoginTime",0);
		ps.add("friendNum",0);
		ps.add("hasSended",0);
		ps.add("hasReceived",0);
		ps.add("positionType",0);
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

	public void setLevel(int level){
		this.level = level;
	}

	public int getHeadCardId(){
		return headCardId;
	}

	public void setHeadCardId(int headCardId){
		this.headCardId = headCardId;
	}

	public long getLastLoginTime(){
		return lastLoginTime;
	}

	public void setLastLoginTime(long lastLoginTime){
		this.lastLoginTime = lastLoginTime;
	}

	public int getFriendNum(){
		return friendNum;
	}

	public void setFriendNum(int friendNum){
		this.friendNum = friendNum;
	}

	public int getHasSended(){
		return hasSended;
	}

	public void setHasSended(int hasSended){
		this.hasSended = hasSended;
	}

	public int getHasReceived(){
		return hasReceived;
	}

	public void setHasReceived(int hasReceived){
		this.hasReceived = hasReceived;
	}

	public int getPositionType(){
		return positionType;
	}

	public void setPositionType(int positionType){
		this.positionType = positionType;
	}

}