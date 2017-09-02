using System.Collections;
using System.Collections.Generic;
public class OpenPackageResp : MessageBody{
	private int gold; // 获取的金币;
	private int rmbGold; // 获取的黄金;
	private int count; 
	private List<object> list; // 卡牌列表;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("gold",0);
		ps.add("rmbGold",0);
		ps.add("count",0);
		ps.addObjectArray("list","RoleCardInfoRes","count");
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