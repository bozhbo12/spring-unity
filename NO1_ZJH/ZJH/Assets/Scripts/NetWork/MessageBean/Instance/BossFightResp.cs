// 0xA092
using System.Collections;
using System.Collections.Generic;
public class BossFightResp : MessageBody{
	private int isWin; // 2-胜利 3-失败 
	private int bossType; // BOSS类型 1-关卡一般BOSS 2-世界BOSS 3-套卡BOSS 4-关底BOSS 5-精英BOSS 
	private int bossId; // BOSSId 精英BOSS时为副本Id 
	private int gold; // 掉落金币 
	private int exp; //掉落经验 
	private int rmbGold; // 打精英boss扣除的点券 
	private string battleAssessment; // 战斗收益 
	private int dropCount; // 掉落物品数量 
	private List<object> dropList; // 掉落物品集合 
	private int count; //攻击步骤数量 
	private List<object> fightStepList; // 攻击步骤 
	private int leftPositionType; // 左方阵型 
	private int count1; //左方阵营卡牌位置数量 
	private List<object> leftCardPosList; //左方阵营位置 
	private int rightPositionType; // 右方阵型 
	private int count2; //右方阵营卡牌位置数量 
	private List<object> rightCardPosList; //右方阵营位置 
	private int upActionValue; // 更新行动值 
	private int posX; // boss所站位置X 
	private int posY; // boss所站位置Y 
	private int stepStar; // 通关星级 
	private int stepExp; // 通关经验 
	private int stepGold; // 通关金币 
	private int stepDropCount; // 掉落物品数量 
	private List<object> stepDropList; // 掉落物品集合 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("isWin",0);
		ps.add("bossType",0);
		ps.add("bossId",0);
		ps.add("gold",0);
		ps.add("exp",0);
		ps.add("rmbGold",0);
		ps.addString("battleAssessment","flashCode",0);
		ps.add("dropCount",0);
		ps.addObjectArray("dropList","RoleCardInfoRes","dropCount");
		ps.add("count",0);
		ps.addObjectArray("fightStepList","FightStep","count");
		ps.add("leftPositionType",0);
		ps.add("count1",0);
		ps.addObjectArray("leftCardPosList","FightReq","count1");
		ps.add("rightPositionType",0);
		ps.add("count2",0);
		ps.addObjectArray("rightCardPosList","FightReq","count2");
		ps.add("upActionValue",0);
		ps.add("posX",0);
		ps.add("posY",0);
		ps.add("stepStar",0);
		ps.add("stepExp",0);
		ps.add("stepGold",0);
		ps.add("stepDropCount",0);
		ps.addObjectArray("stepDropList","RoleCardInfoRes","stepDropCount");
	}

	public int getIsWin(){
		return isWin;
	}

	public void setIsWin(int isWin){
		this.isWin = isWin;
	}

	public int getBossType(){
		return bossType;
	}

	public void setBossType(int bossType){
		this.bossType = bossType;
	}

	public int getBossId(){
		return bossId;
	}

	public void setBossId(int bossId){
		this.bossId = bossId;
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

	public int getRmbGold(){
		return rmbGold;
	}

	public void setRmbGold(int rmbGold){
		this.rmbGold = rmbGold;
	}

	public string getBattleAssessment(){
		return battleAssessment;
	}

	public void setBattleAssessment(string battleAssessment){
		this.battleAssessment = battleAssessment;
	}

	public int getDropCount(){
		return dropCount;
	}

	public void setDropCount(int dropCount){
		this.dropCount = dropCount;
	}

	public List<object> getDropList(){
		return dropList;
	}

	public void setDropList(List<object> list){
		this.dropList = list;
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

	public int getUpActionValue(){
		return upActionValue;
	}

	public void setUpActionValue(int upActionValue){
		this.upActionValue = upActionValue;
	}

	public int getPosX(){
		return posX;
	}

	public void setPosX(int posX){
		this.posX = posX;
	}

	public int getPosY(){
		return posY;
	}

	public void setPosY(int posY){
		this.posY = posY;
	}

	public int getStepStar(){
		return stepStar;
	}

	public void setStepStar(int stepStar){
		this.stepStar = stepStar;
	}

	public int getStepExp(){
		return stepExp;
	}

	public void setStepExp(int stepExp){
		this.stepExp = stepExp;
	}

	public int getStepGold(){
		return stepGold;
	}

	public void setStepGold(int stepGold){
		this.stepGold = stepGold;
	}

	public int getStepDropCount(){
		return stepDropCount;
	}

	public void setStepDropCount(int stepDropCount){
		this.stepDropCount = stepDropCount;
	}

	public List<object> getStepDropList(){
		return stepDropList;
	}

	public void setStepDropList(List<object> list){
		this.stepDropList = list;
	}

}