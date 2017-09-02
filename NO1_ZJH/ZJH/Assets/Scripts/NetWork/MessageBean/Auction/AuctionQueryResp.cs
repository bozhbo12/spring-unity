using System.Collections;
using System.Collections.Generic;
public class AuctionQueryResp : MessageBody{
	private int queryType; // 查询类型 1-查询自己拍卖卡牌;
	private int count; // 集合数量;
	private List<object> list; // 卡牌集合;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("queryType",0);
		ps.add("count",0);
		ps.addObjectArray("list","AuctionDisplayInfo","count");
	}

	public int getQueryType(){
		return queryType;
	}

	public void setQueryType(int queryType){
		this.queryType = queryType;
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

	public List<object> getList(){
		return  list;
	}

	public void setList(List<object> list){
		this.list = list;
	}

}