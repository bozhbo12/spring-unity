using System.Collections;
using System.Collections.Generic;
public class SetCardPositionReq : MessageBody{
	private int positionType; // 阵型类型;
	private int count; // 阵型里卡牌数量;
	private List<object> list; // 阵型里卡牌列表;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("positionType",0);
		ps.add("count",0);
		ps.addObjectArray("list","CardReq","count");
	}

	public int getPositionType(){
		return positionType;
	}

	public void setPositionType(int positionType){
		this.positionType = positionType;
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