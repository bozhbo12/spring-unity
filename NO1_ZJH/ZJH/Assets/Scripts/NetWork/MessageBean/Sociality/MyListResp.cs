using System.Collections;
using System.Collections.Generic;
public class MyListResp : MessageBody{
	private int queryType; // 查询类型 1-好友列表 2-黑名单列表 3-申请列表;
	private int count; //好友数量;
	private List<object> list; //好友集合;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("queryType",0);
		ps.add("count",0);
		ps.addObjectArray("list","FriendRes","count");
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