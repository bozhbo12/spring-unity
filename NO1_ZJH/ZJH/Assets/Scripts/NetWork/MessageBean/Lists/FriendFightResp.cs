using System.Collections;
using System.Collections.Generic;
public class FriendFightResp : MessageBody{
	private int isWin; // 2-胜利 3-失败 
	private string battleAssessment; // 战斗收益等级 
	private int count; // 攻击步骤数量 
	private List<object> fightStepList; // 攻击步骤 
	private int leftPositionType; // 左方阵型 
	private int count1; // 左方阵营卡牌位置数量 
	private List<object> leftCardPosList; // 左方阵营位置 
	private int rightPositionType; // 右方阵型 
	private int rightHeadId; // 右方头像 
	private int count2; // 右方阵营卡牌位置数量 
	private List<object> rightCardPosList; // 右方阵营位置 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("isWin",0);
		ps.addString("battleAssessment","flashCode",0);
		ps.add("count",0);
		ps.addObjectArray("fightStepList","FightStep","count");
		ps.add("leftPositionType",0);
		ps.add("count1",0);
		ps.addObjectArray("leftCardPosList","FightReq","count1");
		ps.add("rightPositionType",0);
		ps.add("rightHeadId",0);
		ps.add("count2",0);
		ps.addObjectArray("rightCardPosList","FightReq","count2");
	}

	public int getIsWin(){
		return isWin;
	}

	public void setIsWin(int isWin){
		this.isWin = isWin;
	}

	public string getBattleAssessment(){
		return battleAssessment;
	}

	public void setBattleAssessment(string battleAssessment){
		this.battleAssessment = battleAssessment;
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

	public List<object> getFightStepList(){
		return fightStepList;
	}

	public void setFightStepList(List<object> list){
		this.fightStepList = list;
	}

	public int getLeftPositionType(){
		return leftPositionType;
	}

	public void setLeftPositionType(int leftPositionType){
		this.leftPositionType = leftPositionType;
	}

	public int getCount1(){
		return count1;
	}

	public void setCount1(int count1){
		this.count1 = count1;
	}

	public List<object> getLeftCardPosList(){
		return leftCardPosList;
	}

	public void setLeftCardPosList(List<object> list){
		this.leftCardPosList = list;
	}

	public int getRightPositionType(){
		return rightPositionType;
	}

	public void setRightPositionType(int rightPositionType){
		this.rightPositionType = rightPositionType;
	}

	public int getRightHeadId(){
		return rightHeadId;
	}

	public void setRightHeadId(int rightHeadId){
		this.rightHeadId = rightHeadId;
	}

	public int getCount2(){
		return count2;
	}

	public void setCount2(int count2){
		this.count2 = count2;
	}

	public List<object> getRightCardPosList(){
		return rightCardPosList;
	}

	public void setRightCardPosList(List<object> list){
		this.rightCardPosList = list;
	}

}