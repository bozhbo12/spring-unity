// 0xA014
using System.Collections;
using System.Collections.Generic;
public class RolePlayResp : MessageBody{
	private int roleId; // 角色ID 
	private string roleName; // 角色名字 
	private int roleLevel; // 角色等级 
	private int currExper; // 当前角色经验值 
	private int gold; // 金币 
	private int rmbGold; // 点券 
	private string buyCardCount; // 购买卡包次数(id:num,id:num...) 
	private int score; // 竞技场积分 
	private int duplicateId; 
	private int stepId; 
	private int actionValue; // 行动力 
	private int cardListCount; 
	private List<object> roleCardList; 
	private string picture; // 图鉴 
	private int dayLoginFlag; // 0- 第一次登陆 1-非第一次登陆 
	private int giftHasGet; // 已领到第几个礼包 
	private string roundGifts; // 一个周期礼包的内容 以type:value,type:value格式拼接 //type: 
	private int winCount; // 竞技场战斗胜利场数 
	private int loseCount; // 竞技场战斗失败场数 
	private int captainCardId; // 队长头像 
	private int positionType; // 阵型类型 
	private long actionRefreshTime; // 上次行动力刷新时间 
	private int friendCount; // 好友数量 
	private string talks; // 当前关卡出现过的剧情 
	private int rolePackageCount; // 角色宝箱数量 
	private string roleGuide; // 角色未完成引导Id(id1,id2,...) 
	private long bossLastAttackTime; // 上次世界BOSS攻击时间 
	private string fightedCardSetBoss; // 打过的套卡BOSS 
	private int loginDays; // 登录天数 
	private int chargeTotal; // 充值总额 
	private int isShowMovie; // 1-副本开启动画 
	private string inviteCode; //当前角色的邀请码 
	private int isInvited; //当前角色是否使用过邀请码 0-未使用 1-已使用 
	private int invitedCount; //当前角色邀请数量 
	private int fragmentListCount; //碎片数量 
	private List<object> fragmentList; 
	private string accesstoken; // 360计费token 
	private string qihooUserId; // 360UserId 
	private string loginInfo; //登陆信息 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("roleId",0);
		ps.addString("roleName","flashCode",0);
		ps.add("roleLevel",0);
		ps.add("currExper",0);
		ps.add("gold",0);
		ps.add("rmbGold",0);
		ps.addString("buyCardCount","flashCode",0);
		ps.add("score",0);
		ps.add("duplicateId",0);
		ps.add("stepId",0);
		ps.add("actionValue",0);
		ps.add("cardListCount",0);
		ps.addObjectArray("roleCardList","RoleCardInfoRes","cardListCount");
		ps.addString("picture","flashCode",0);
		ps.add("dayLoginFlag",0);
		ps.add("giftHasGet",0);
		ps.addString("roundGifts","flashCode",0);
		ps.add("winCount",0);
		ps.add("loseCount",0);
		ps.add("captainCardId",0);
		ps.add("positionType",0);
		ps.add("actionRefreshTime",0);
		ps.add("friendCount",0);
		ps.addString("talks","flashCode",0);
		ps.add("rolePackageCount",0);
		ps.addString("roleGuide","flashCode",0);
		ps.add("bossLastAttackTime",0);
		ps.addString("fightedCardSetBoss","flashCode",0);
		ps.add("loginDays",0);
		ps.add("chargeTotal",0);
		ps.add("isShowMovie",0);
		ps.addString("inviteCode","flashCode",0);
		ps.add("isInvited",0);
		ps.add("invitedCount",0);
		ps.add("fragmentListCount",0);
		ps.addObjectArray("fragmentList","FragmentResp","fragmentListCount");
		ps.addString("accesstoken","flashCode",0);
		ps.addString("qihooUserId","flashCode",0);
		ps.addString("loginInfo","flashCode",0);
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

	public int getScore(){
		return score;
	}

	public void setScore(int score){
		this.score = score;
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

	public string getPicture(){
		return picture;
	}

	public void setPicture(string picture){
		this.picture = picture;
	}

	public int getDayLoginFlag(){
		return dayLoginFlag;
	}

	public void setDayLoginFlag(int dayLoginFlag){
		this.dayLoginFlag = dayLoginFlag;
	}

	public int getGiftHasGet(){
		return giftHasGet;
	}

	public void setGiftHasGet(int giftHasGet){
		this.giftHasGet = giftHasGet;
	}

	public string getRoundGifts(){
		return roundGifts;
	}

	public void setRoundGifts(string roundGifts){
		this.roundGifts = roundGifts;
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

	public int getCaptainCardId(){
		return captainCardId;
	}

	public void setCaptainCardId(int captainCardId){
		this.captainCardId = captainCardId;
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

	public string getTalks(){
		return talks;
	}

	public void setTalks(string talks){
		this.talks = talks;
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

	public int getLoginDays(){
		return loginDays;
	}

	public void setLoginDays(int loginDays){
		this.loginDays = loginDays;
	}

	public int getChargeTotal(){
		return chargeTotal;
	}

	public void setChargeTotal(int chargeTotal){
		this.chargeTotal = chargeTotal;
	}

	public int getIsShowMovie(){
		return isShowMovie;
	}

	public void setIsShowMovie(int isShowMovie){
		this.isShowMovie = isShowMovie;
	}

	public string getInviteCode(){
		return inviteCode;
	}

	public void setInviteCode(string inviteCode){
		this.inviteCode = inviteCode;
	}

	public int getIsInvited(){
		return isInvited;
	}

	public void setIsInvited(int isInvited){
		this.isInvited = isInvited;
	}

	public int getInvitedCount(){
		return invitedCount;
	}

	public void setInvitedCount(int invitedCount){
		this.invitedCount = invitedCount;
	}

	public int getFragmentListCount(){
		return fragmentListCount;
	}

	public void setFragmentListCount(int fragmentListCount){
		this.fragmentListCount = fragmentListCount;
	}

	public List<object> getFragmentList(){
		return fragmentList;
	}

	public void setFragmentList(List<object> list){
		this.fragmentList = list;
	}

	public string getAccesstoken(){
		return accesstoken;
	}

	public void setAccesstoken(string accesstoken){
		this.accesstoken = accesstoken;
	}

	public string getQihooUserId(){
		return qihooUserId;
	}

	public void setQihooUserId(string qihooUserId){
		this.qihooUserId = qihooUserId;
	}

	public string getLoginInfo(){
		return loginInfo;
	}

	public void setLoginInfo(string loginInfo){
		this.loginInfo = loginInfo;
	}

}