using System.Collections;
using System.Collections.Generic;
public class RoleReloadResp : MessageBody{
	private int roleLevel; // 角色等级 
	private int currExper; // 当前角色经验值 
	private int gold; // 金币 
	private int rmbGold; // 点券 
	private string buyCardCount; // 购买卡包次数(id:num,id:num...) 
	private int duplicateId; // 最新副本 
	private int stepId; // 最新副本关卡 
	private int actionValue; // 行动力 
	private int positionType; // 阵型类型 
	private long actionRefreshTime; // 上次行动力刷新时间 
	private int friendCount; // 好友数量 
	private int rolePackageCount; // 角色宝箱数量 
	private string roleGuide; // 角色未完成引导Id(id1,id2,...) 
	private long bossLastAttackTime; // 上次世界BOSS攻击时间 
	private string fightedCardSetBoss; // 打过的套卡BOSS 
	private int cardListCount; // 卡牌集合数量 
	private List<object> roleCardList; // 卡牌集合 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("roleLevel",0);
		ps.add("currExper",0);
		ps.add("gold",0);
		ps.add("rmbGold",0);
		ps.addString("buyCardCount","flashCode",0);
		ps.add("duplicateId",0);
		ps.add("stepId",0);
		ps.add("actionValue",0);
		ps.add("positionType",0);
		ps.add("actionRefreshTime",0);
		ps.add("friendCount",0);
		ps.add("rolePackageCount",0);
		ps.addString("roleGuide","flashCode",0);
		ps.add("bossLastAttackTime",0);
		ps.addString("fightedCardSetBoss","flashCode",0);
		ps.add("cardListCount",0);
		ps.addObjectArray("roleCardList","RoleCardInfoRes","cardListCount");
	}

	public int getRoleLevel(){
		return roleLevel;
	}

	public void setRoleLevel(int roleLevel){
		this.roleLevel = roleLevel;
	}

	public int getCurrExper(){
		return currExper;
	}

	public void setCurrExper(int currExper){
		this.currExper = currExper;
	}

	public int getGold(){
		return gold;
	}

	public void setGold(int gold){
		this.gold = gold;
	}

	public int getRmbGold(){
		return rmbGold;
	}

	public void setRmbGold(int rmbGold){
		this.rmbGold = rmbGold;
	}

	public string getBuyCardCount(){
		return buyCardCount;
	}

	public void setBuyCardCount(string buyCardCount){
		this.buyCardCount = buyCardCount;
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

	public int getActionValue(){
		return actionValue;
	}

	public void setActionValue(int actionValue){
		this.actionValue = actionValue;
	}

	public int getPositionType(){
		return positionType;
	}

	public void setPositionType(int positionType){
		this.positionType = positionType;
	}

	public long getActionRefreshTime(){
		return actionRefreshTime;
	}

	public void setActionRefreshTime(long actionRefreshTime){
		this.actionRefreshTime = actionRefreshTime;
	}

	public int getFriendCount(){
		return friendCount;
	}

	public void setFriendCount(int friendCount){
		this.friendCount = friendCount;
	}

	public int getRolePackageCount(){
		return rolePackageCount;
	}

	public void setRolePackageCount(int rolePackageCount){
		this.rolePackageCount = rolePackageCount;
	}

	public string getRoleGuide(){
		return roleGuide;
	}

	public void setRoleGuide(string roleGuide){
		this.roleGuide = roleGuide;
	}

	public long getBossLastAttackTime(){
		return bossLastAttackTime;
	}

	public void setBossLastAttackTime(long bossLastAttackTime){
		this.bossLastAttackTime = bossLastAttackTime;
	}

	public string getFightedCardSetBoss(){
		return fightedCardSetBoss;
	}

	public void setFightedCardSetBoss(string fightedCardSetBoss){
		this.fightedCardSetBoss = fightedCardSetBoss;
	}

	public int getCardListCount(){
		return cardListCount;
	}

	public void setCardListCount(int cardListCount){
		this.cardListCount = cardListCount;
	}

	public List<object> getRoleCardList(){
		return roleCardList;
	}

	public void setRoleCardList(List<object> list){
		this.roleCardList = list;
	}

}