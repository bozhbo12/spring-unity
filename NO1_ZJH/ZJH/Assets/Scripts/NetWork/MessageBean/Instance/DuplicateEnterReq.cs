public class DuplicateEnterReq : MessageBody{
	private int duplicateId; // 副本Id;
	private int stepId; // 关卡Id;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("duplicateId",0);
		ps.add("stepId",0);
	}

	public int getDuplicateId(){
		return duplicateId;
	}

	public void setDuplicateId(int duplicateId){
		this.duplicateId = duplicateId;
	}

	public int getStepId(){
		return stepId;
	}

	public void setStepId(int stepId){
		this.stepId = stepId;
	}

}