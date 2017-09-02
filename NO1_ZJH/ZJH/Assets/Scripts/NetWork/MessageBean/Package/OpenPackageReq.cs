public class OpenPackageReq : MessageBody{
	private long seqId; //宝箱的ID;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("seqId",0);
	}

	public long getSeqId(){
		return seqId;
	}

	public void setSeqId(long seqId){
		this.seqId = seqId;
	}

}