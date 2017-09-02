public class DuplicateStepBossResp : MessageBody{
	private int nowPosX; // 当前位置X坐标;
	private int nowPosY; // 当前位置Y坐标;
	private int upActionValue; // 更新行动值;
	private int bossType; // BOSS类型  1-关卡一般BOSS 2-世界BOSS 3-套卡BOSS 4-关底BOSS 5-精英BOSS;
	private int bossId; // BOSSId;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("nowPosX",0);
		ps.add("nowPosY",0);
		ps.add("upActionValue",0);
		ps.add("bossType",0);
		ps.add("bossId",0);
	}

	public int getNowPosX(){
		return nowPosX;
	}

	public void setNowPosX(int nowPosX){
		this.nowPosX = nowPosX;
	}

	public int getNowPosY(){
		return nowPosY;
	}

	public void setNowPosY(int nowPosY){
		this.nowPosY = nowPosY;
	}

	public int getUpActionValue(){
		return upActionValue;
	}

	public void setUpActionValue(int upActionValue){
		this.upActionValue = upActionValue;
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

}