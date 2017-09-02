public class MailReadReq : MessageBody{
	private long mailTime; // 邮件时间;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("mailTime",0);
	}

	public long getMailTime(){
		return mailTime;
	}

	public void setMailTime(long mailTime){
		this.mailTime = mailTime;
	}

}