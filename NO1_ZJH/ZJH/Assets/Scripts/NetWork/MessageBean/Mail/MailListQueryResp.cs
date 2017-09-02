using System.Collections;
using System.Collections.Generic;
public class MailListQueryResp : MessageBody{
	private int count; 
	private List<object> list; 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("count",0);
		ps.addObjectArray("list","MailListQueryRes","count");
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