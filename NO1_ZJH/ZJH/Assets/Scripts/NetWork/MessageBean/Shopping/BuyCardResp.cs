using System.Collections;
using System.Collections.Generic;
public class BuyCardResp : MessageBody{
	private int id; // 购买卡包id;
	private int costTyp; // 消耗类型 1-金币 2-点券;
	private int gold; // 消耗值;
	private int count; 
	private List<object> list; 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("id",0);
		ps.add("costTyp",0);
		ps.add("gold",0);
		ps.add("count",0);
		ps.addObjectArray("list","RoleCardInfoRes","count");
	}

	public int getId(){
		return id;
	}

	public void setId(int id){
		this.id = id;
	}

	public int getCostTyp(){
		return costTyp;
	}

	public void setCostTyp(int costTyp){
		this.costTyp = costTyp;
	}

	public int getGold(){
		return gold;
	}

	public void setGold(int gold){
		this.gold = gold;
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