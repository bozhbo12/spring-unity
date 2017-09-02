public class GetRaceRankListReq : MessageBody{
	private int beginIndex; //查询起始位;
	private int endIndex; //查询结束位;
	private int rankType; //1-查看自己排名;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("beginIndex",0);
		ps.add("endIndex",0);
		ps.add("rankType",0);
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

	public void setEndIndex( int endIndex){
		this.endIndex = endIndex;
	}

	public int getRankType(){
		return rankType;
	}

	public void setRankType( int rankType){
		this.rankType = rankType;
	}

}