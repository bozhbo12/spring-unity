using System.Collections;
using System.Collections.Generic;
public class RoleGuideResp : MessageBody{
	private int count; // 卡牌数量 
	private List<object> list; // 卡牌集合 
	private int gold; // 金币 
	private int rmbGold; // 五彩石 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("count",0);
		ps.addObjectArray("list","RoleCardInfoRes","count");
		ps.add("gold",0);
		ps.add("rmbGold",0);
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

	public int getGold(){
		return gold;
	}

	public void setGold(int gold){
		this.gold = gold;
	}

	public int getRmbGold(){
		return rmbGold;
	}

	public void setRmbGold(int rmbGold){
		this.rmbGold = rmbGold;
	}

}