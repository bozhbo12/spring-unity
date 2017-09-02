using System.Collections;
using System.Collections.Generic;
public class DetailResp : MessageBody{
	private string name; // 角色姓名;
	private int level; // 角色等级;
	private int currExp; // 经验;
	private int headCardId; // 头像卡牌ID;
	private long lastLoginTime; // 上次登陆时间;
	private string fightCapacity; // 战斗力;
	private int raceWinCount; // 胜场;
	private int raceLoseCount; // 负场;
	private int score; // 竞技场积分;
	private int count; // 战斗卡组数量;
	private List<object> list; //战斗卡组;

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("name","flashCode",0);
		ps.add("level",0);
		ps.add("currExp",0);
		ps.add("headCardId",0);
		ps.add("lastLoginTime",0);
		ps.addString("fightCapacity","flashCode",0);
		ps.add("raceWinCount",0);
		ps.add("raceLoseCount",0);
		ps.add("score",0);
		ps.add("count",0);
		ps.addObjectArray("list","RoleCardInfoRes","count");
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

	public int getCurrExp(){
		return currExp;
	}

	public void setCurrExp(int currExp){
		this.currExp = currExp;
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

	public string getFightCapacity(){
		return fightCapacity;
	}

	public void setFightCapacity(string fightCapacity){
		this.fightCapacity = fightCapacity;
	}

	public int getRaceWinCount(){
		return raceWinCount;
	}

	public void setRaceWinCount(int raceWinCount){
		this.raceWinCount = raceWinCount;
	}

	public int getRaceLoseCount(){
		return raceLoseCount;
	}

	public void setRaceLoseCount(int raceLoseCount){
		this.raceLoseCount = raceLoseCount;
	}

	public int getScore(){
		return score;
	}

	public void setScore(int score){
		this.score = score;
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

	public List<object> getList(){
		return  list;
	}

	public void setList(List<object> list){
		this.list = list;
	}

}