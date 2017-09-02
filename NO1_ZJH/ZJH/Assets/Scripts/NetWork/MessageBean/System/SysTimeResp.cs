public class SysTimeResp : MessageBody{
	private long currTime; //当前时间

	public override void setSequnce(ProtocolSequence ps){
		ps.add("currTime",0);
	}

	public long getCurrTime(){
		return currTime;
	}

	public void setCurrTime(long currTime){
		this.currTime = currTime;
	}

}