public class ActionFightValResp : MessageBody{
	private long time; // 上次刷新时间;
	private int regainType; // 1-actionValue 2-fightValue;
	private int value; // 值;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("time",0);
		ps.add("regainType",0);
		ps.add("value",0);
	}

	public long getTime(){
		return time;
	}

	public void setTime(long time){
		this.time = time;
	}

	public int getRegainType(){
		return regainType;
	}

	public void setRegainType(int regainType){
		this.regainType = regainType;
	}

	public int getValue(){
		return value;
	}

	public void setValue(int value){
		this.value = value;
	}

}