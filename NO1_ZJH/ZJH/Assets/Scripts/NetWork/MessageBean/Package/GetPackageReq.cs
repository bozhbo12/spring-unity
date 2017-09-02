public class GetPackageReq : MessageBody{
	private int beginIndex; //开始位置;
	private int endIndex; //结束位置;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("beginIndex",0);
		ps.add("endIndex",0);
	}

	public int getBeginIndex(){
		return beginIndex;
	}

	public void setBeginIndex(int beginIndex){
		this.beginIndex = beginIndex;
	}

	public int getEndIndex(){
		return endIndex;
	}

	public void setEndIndex(int endIndex){
		this.endIndex = endIndex;
	}

}