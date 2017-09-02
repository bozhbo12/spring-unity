public class MyListReq : MessageBody{
	private int queryType; // 查询类型 1-好友列表 2-黑名单列表 3-申请列表;
	private int beginIndex; // 起始位;
	private int endIndex; // 结束位;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("queryType",0);
		ps.add("beginIndex",0);
		ps.add("endIndex",0);
	}

	public int getQueryType(){
		return queryType;
	}

	public void setQueryType(int queryType){
		this.queryType = queryType;
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