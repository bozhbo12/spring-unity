public class QueryPublishRes : MessageBody{
	private string publishContent; // 公告内容 
	private long publishTime; // 公告时间 

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("publishContent","flashCode",0);
		ps.add("publishTime",0);
	}

	public string getPublishContent(){
		return publishContent;
	}

	public void setPublishContent(string publishContent){
		this.publishContent = publishContent;
	}

	public long getPublishTime(){
		return publishTime;
	}

	public void setPublishTime(long publishTime){
		this.publishTime = publishTime;
	}

}