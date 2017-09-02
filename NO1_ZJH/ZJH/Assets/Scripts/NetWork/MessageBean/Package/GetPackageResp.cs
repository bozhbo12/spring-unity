using System.Collections;
using System.Collections.Generic;
public class GetPackageResp : MessageBody{
	private int count; //储物箱数量;
	private List<object> list; //储物箱列表;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("count",0);
		ps.addObjectArray("list","RolePackageRes","count");
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