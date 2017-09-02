public class RoleGuideReq : MessageBody{
	private int guideId; // 引导Id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("guideId",0);
	}

	public int getGuideId(){
		return guideId;
	}

	public void setGuideId(int guideId){
		this.guideId = guideId;
	}

}