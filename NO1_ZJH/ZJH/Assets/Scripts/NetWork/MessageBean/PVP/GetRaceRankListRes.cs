public class GetRaceRankListRes : MessageBody{
	private int headId; //头像ID;
	private int level; //角色等级;
	private int score; //竞技场积分;
	private string name; //角色姓名;
	private int roleId; //角色ID;
	private int winCount; // 竞技场胜利场数;
	private int loseCount; // 竞技场失败场数;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("headId",0);
		ps.add("level",0);
		ps.add("score",0);
		ps.addString("name","flashCode",0);
		ps.add("roleId",0);
		ps.add("winCount",0);
		ps.add("loseCount",0);
	}

	public int getHeadId(){
		return headId;
	}

	public void setHeadId(int headId){
		this.headId = headId;
	}

	public int getLevel(){
		return level;
	}

	public void setLevel(int level){
		this.level = level;
	}

	public int getScore(){
		return score;
	}

	public void setScore(int score){
		this.score = score;
	}

	public string getName(){
		return name;
	}

	public void setName(string name){
		this.name = name;
	}

	public int getRoleId(){
		return roleId;
	}

	public void setRoleId(int roleId){
		this.roleId = roleId;
	}

	public int getWinCount(){
		return winCount;
	}

	public void setWinCount(int winCount){
		this.winCount = winCount;
	}

	public int getLoseCount(){
		return loseCount;
	}

	public void setLoseCount(int loseCount){
		this.loseCount = loseCount;
	}

}