public class ModulusResp : MessageBody{
	private int randomFriendMaxNum; // 随机搜索好友数量;
	private int worldBossMissTime; // 世界bossId(单位小时);
	private int firstDuplicateId; // 初始副本id;
	private double worldBossGrowthRate;// 世界boss成长率;
	private int pictureNum; // 图鉴数量单位（每10个又一次加成）;
	private double pictureModulus;// 图鉴加成系数;
	private int battleRegainTime; // 战斗力恢复时间(min);
	private int actionRegainTime; // 行动力恢复时间(min);
	private int heroBossChallengeBaseTime; // 精英副本boss挑战次数;
	private int heroBossChallengeExtraTime; // 精英副本boss额外挑战次数;
	private int heroBossChallengeExtraCost; // 精英副本boss额外次数购买费用;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("randomFriendMaxNum", 0);
		ps.add("worldBossMissTime", 0);
		ps.add("firstDuplicateId", 0);
		ps.add("worldBossGrowthRate", 0);
		ps.add("pictureNum", 0);
		ps.add("pictureModulus", 0);
		ps.add("battleRegainTime", 0);
		ps.add("actionRegainTime", 0);
		ps.add("heroBossChallengeBaseTime", 0);
		ps.add("heroBossChallengeExtraTime", 0);
		ps.add("heroBossChallengeExtraCost", 0);
	}

	public int getRandomFriendMaxNum(){
		return randomFriendMaxNum;
	}

	public void setRandomFriendMaxNum(int randomFriendMaxNum){
		this.randomFriendMaxNum = randomFriendMaxNum;
	}

	public int getWorldBossMissTime(){
		return worldBossMissTime;
	}

	public void setWorldBossMissTime(int worldBossMissTime){
		this.worldBossMissTime = worldBossMissTime;
	}

	public int getFirstDuplicateId(){
		return firstDuplicateId;
	}

	public double getWorldBossGrowthRate() {
		return worldBossGrowthRate;
	}

	public void setWorldBossGrowthRate(double worldBossGrowthRate) {
		this.worldBossGrowthRate = worldBossGrowthRate;
	}


	public void setFirstDuplicateId(int firstDuplicateId){
		this.firstDuplicateId = firstDuplicateId;
	}

	public int getPictureNum(){
		return pictureNum;
	}

	public void setPictureNum(int pictureNum){
		this.pictureNum = pictureNum;
	}

	public double getPictureModulus() {
		return pictureModulus;
	}

	public void setPictureModulus(double pictureModulus) {
		this.pictureModulus = pictureModulus;
	}


	public int getBattleRegainTime(){
		return battleRegainTime;
	}

	public void setBattleRegainTime(int battleRegainTime){
		this.battleRegainTime = battleRegainTime;
	}

	public int getActionRegainTime(){
		return actionRegainTime;
	}

	public void setActionRegainTime(int actionRegainTime){
		this.actionRegainTime = actionRegainTime;
	}

	public int getHeroBossChallengeBaseTime(){
		return heroBossChallengeBaseTime;
	}

	public void setHeroBossChallengeBaseTime(int heroBossChallengeBaseTime){
		this.heroBossChallengeBaseTime = heroBossChallengeBaseTime;
	}

	public int getHeroBossChallengeExtraTime(){
		return heroBossChallengeExtraTime;
	}

	public void setHeroBossChallengeExtraTime(int heroBossChallengeExtraTime){
		this.heroBossChallengeExtraTime = heroBossChallengeExtraTime;
	}

	public int getHeroBossChallengeExtraCost(){
		return heroBossChallengeExtraCost;
	}

	public void setHeroBossChallengeExtraCost(int heroBossChallengeExtraCost){
		this.heroBossChallengeExtraCost = heroBossChallengeExtraCost;
	}

}