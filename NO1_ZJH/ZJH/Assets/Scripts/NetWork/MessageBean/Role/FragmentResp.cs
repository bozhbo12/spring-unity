public class FragmentResp : MessageBody{
	private int fragmentId; //碎片ID 
	private int fragmentCount; //碎片数量 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("fragmentId",0);
		ps.add("fragmentCount",0);
	}

	public int getFragmentId(){
		return fragmentId;
	}

	public void setFragmentId(int fragmentId){
		this.fragmentId = fragmentId;
	}

	public int getFragmentCount(){
		return fragmentCount;
	}

	public void setFragmentCount(int fragmentCount){
		this.fragmentCount = fragmentCount;
	}

}