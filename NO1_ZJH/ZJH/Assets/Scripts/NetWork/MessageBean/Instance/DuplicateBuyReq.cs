public class DuplicateBuyReq : MessageBody{
	private int duplicateId; // 副本Id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("duplicateId",0);
	}

	public int getDuplicateId(){
		return duplicateId;
	}

	public void setDuplicateId(int duplicateId){
		this.duplicateId = duplicateId;
	}

}