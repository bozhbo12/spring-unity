using System.Collections;
using System.Collections.Generic;
public class RaceFightResp : MessageBody{
	private int gold; //获得的金币;
	private int exp; //获得的经验;
	private int score; //获得的竞技场积分;
	private int isWin; //2-win 3-lose;
	private int count; //攻击步骤数量;
	private List<object> fightStepList; // 攻击步骤;
	private int leftPos; // 左方阵型;
	private int count1; //左方阵营卡牌位置数量;
	private List<object> leftCardPosList; //左方阵营位置;
	private int rightPos; // 右方阵型;
	private int count2; //右方阵营卡牌位置数量;
	private List<object> rightCardPosList; //右方阵营位置;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("gold",0);
		ps.add("exp",0);
		ps.add("score",0);
		ps.add("isWin",0);
		ps.add("count",0);
		ps.addObjectArray("fightStepList","FightStep","count");
		ps.add("leftPos",0);
		ps.add("count1",0);
		ps.addObjectArray("leftCardPosList","FightReq","count1");
		ps.add("rightPos",0);
		ps.add("count2",0);
		ps.addObjectArray("rightCardPosList","FightReq","count2");
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

	public int getScore(){
		return score;
	}

	public void setScore(int score){
		this.score = score;
	}

	public int getIsWin(){
		return isWin;
	}

	public void setIsWin(int isWin){
		this.isWin = isWin;
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

	public List<object> getFightStepList(){
		return  fightStepList;
	}

	public void setFightStepList(List<object> list){
		this.fightStepList = list;
	}

	public int getLeftPos(){
		return leftPos;
	}

	public void setLeftPos(int leftPos){
		this.leftPos = leftPos;
	}

	public int getCount1(){
		return count1;
	}

	public void setCount1(int count1){
		this.count1 = count1;
	}

	public List<object> getLeftCardPosList(){
		return  leftCardPosList;
	}

	public void setLeftCardPosList(List<object> list){
		this.leftCardPosList = list;
	}

	public int getRightPos(){
		return rightPos;
	}

	public void setRightPos(int rightPos){
		this.rightPos = rightPos;
	}

	public int getCount2(){
		return count2;
	}

	public void setCount2(int count2){
		this.count2 = count2;
	}

	public List<object> getRightCardPosList(){
		return  rightCardPosList;
	}

	public void setRightCardPosList(List<object> list){
		this.rightCardPosList = list;
	}

}