public class CountEvent : MessageBody{
	private string countId; // 坐标点x,y ;
	private byte eventTyp; // 事件类型 1：普通军团 2：掉落 ;
	private int contentId; // 事件内容; 

	public override void setSequnce(ProtocolSequence ps){
		ps.addString("countId","flashCode",0);
		ps.add("eventTyp",0);
		ps.add("contentId",0);
	}

	public string getCountId(){
		return countId;
	}

	public void setCountId(string countId){
		this.countId = countId;
	}

	public byte getEventTyp(){
		return eventTyp;
	}

	public void setEventTyp(byte eventTyp){
		this.eventTyp = eventTyp;
	}

	public int getContentId(){
		return contentId;
	}

	public void setContentId(int contentId){
		this.contentId = contentId;
	}

}