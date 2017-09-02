public class DuplicateStepCommonResp : MessageBody{
	private int nowPosX; // 当前位置X坐标;
	private int nowPosY; // 当前位置Y坐标;
	private int upActionValue; // 更新行动值;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("nowPosX",0);
		ps.add("nowPosY",0);
		ps.add("upActionValue",0);
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

}