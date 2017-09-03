public class CommonResp : MessageBody{

	private int optionType;
	private string optionStr;


	public override void setSequnce(ProtocolSequence ps){
		ps.add("optionType",0);
		ps.addString("optionStr","flashCode",0);
	}

	public int getOptionType(){
		return optionType;
	}

	public void setOptionType(int optionType){
		this.optionType = optionType;
	}

	public string getOptionStr(){
		return optionStr;
	}

	public void setOptionStr(string optionStr){
		this.optionStr = optionStr;
	}

}