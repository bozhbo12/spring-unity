public class RolePackageRes : MessageBody{
	private long seqId; // 储物箱唯一ID;
	private int packageType; //1-世界BOSS奖励 2-礼包;
	private long obtainTime; // 获取时间;
	private int contentType; // 储物箱类型 1-卡牌 2-金币 3-黄金;
	private int content; // 数量;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("seqId",0);
		ps.add("packageType",0);
		ps.add("obtainTime",0);
		ps.add("contentType",0);
		ps.add("content",0);
	}

	public long getSeqId(){
		return seqId;
	}

	public void setSeqId(long seqId){
		this.seqId = seqId;
	}

	public int getPackageType(){
		return packageType;
	}

	public void setPackageType(int packageType){
		this.packageType = packageType;
	}

	public long getObtainTime(){
		return obtainTime;
	}

	public void setObtainTime(long obtainTime){
		this.obtainTime = obtainTime;
	}

	public int getContentType(){
		return contentType;
	}

	public void setContentType(int contentType){
		this.contentType = contentType;
	}

	public int getContent(){
		return content;
	}

	public void setContent(int content){
		this.content = content;
	}

}