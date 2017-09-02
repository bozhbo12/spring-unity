using System.Collections;
using System.Collections.Generic;
public class QueryPublishResp : MessageBody{
	private int count; // 公告数量 
	private List<object> list; // 公告集合 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("count",0);
		ps.addObjectArray("list","QueryPublishRes","count");
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

	public List<object> getList(){
		return list;
	}

	public void setList(List<object> list){
		this.list = list;
	}

}