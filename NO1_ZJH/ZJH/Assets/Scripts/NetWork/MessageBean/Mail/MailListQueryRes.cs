public class MailListQueryRes : MessageBody{
	private string mailTitle; // 邮件头;
	private string mailContext; // 邮件内容;
	private long mailTime; // 邮件时间;
	private int readFlag; // 读取标识位 0-未读取 1-已读取;
	//private string removeCard;// 移除卡牌唯一Id(1,2...);


	public override void setSequnce(ProtocolSequence ps){
		ps.addString("mailTitle","flashCode",0);
		ps.addString("mailContext","flashCode",0);
		ps.add("mailTime",0);
		ps.add("readFlag",0);
		//ps.addString("removeCard","flashCode",0);
	}

	public string getMailTitle(){
		return mailTitle;
	}

	public void setMailTitle(string mailTitle){
		this.mailTitle = mailTitle;
	}

	public string getMailContext(){
		return mailContext;
	}

	public void setMailContext(string mailContext){
		this.mailContext = mailContext;
	}

	public long getMailTime(){
		return mailTime;
	}

	public void setMailTime(long mailTime){
		this.mailTime = mailTime;
	}

	public int getReadFlag(){
		return readFlag;
	}

	public void setReadFlag(int readFlag){
		this.readFlag = readFlag;
	}
	
//	public string getRemoveCard(){
//		return removeCard;
//	}
//
//	public void setRemoveCard(string removeCard){
//		this.removeCard = removeCard;
//	}

}