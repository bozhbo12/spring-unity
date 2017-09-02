using System.Collections;
using System.Collections.Generic;
public class GetRaceRankListResp : MessageBody{
	private int myPosition; //角色当前位置;
	private int count; //列表数量;
	private List<object> list; //列表;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("myPosition",0);
		ps.add("count",0);
		ps.addObjectArray("list","GetRaceRankListRes","count");
	}

	public int getMyPosition(){
		return myPosition;
	}

	public void setMyPosition(int myPosition){
		this.myPosition = myPosition;
	}

	public int getCount(){
		return count;
	}

	public void setCount( int count){
		this.count = count;
	}

	public List<object> getList(){
		return  list;
	}

	public void setList(List<object> list){
		this.list = list;
	}

}