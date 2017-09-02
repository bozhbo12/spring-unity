using System.Collections;
using System.Collections.Generic;
public class BossListResp : MessageBody{
	private long bossLastAttackTime; // 上次攻击时间;
	private int count; //BOSS数量;
	private List<object> bossCardList; //BOSS列表;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("bossLastAttackTime",0);
		ps.add("count",0);
		ps.addObjectArray("bossCardList","BossCardRes","count");
	}

	public long getBossLastAttackTime(){
		return bossLastAttackTime;
	}

	public void setBossLastAttackTime(long bossLastAttackTime){
		this.bossLastAttackTime = bossLastAttackTime;
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

	public List<object> getBossCardList(){
		return  bossCardList;
	}

	public void setBossCardList(List<object> list){
		this.bossCardList = list;
	}

}