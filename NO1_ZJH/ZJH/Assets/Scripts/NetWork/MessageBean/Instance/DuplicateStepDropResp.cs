using System.Collections;
using System.Collections.Generic;
public class DuplicateStepDropResp : MessageBody{
	private int nowPosX; // 当前位置X坐标;
	private int nowPosY; // 当前位置Y坐标;
	private int upActionValue; // 更新行动值;
	private int gold; // 掉落金币;
	private int exp; // 掉落经验;
	private int count; 
	private List<object> cards; // 掉落卡牌;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("nowPosX",0);
		ps.add("nowPosY",0);
		ps.add("upActionValue",0);
		ps.add("gold",0);
		ps.add("exp",0);
		ps.add("count",0);
		ps.addObjectArray("cards","RoleCardInfoRes","count");
	}

	public int getNowPosX(){
		return nowPosX;
	}

	public void setNowPosX(int nowPosX){
		this.nowPosX = nowPosX;
	}

	public int getNowPosY(){
		return nowPosY;
	}

	public void setNowPosY(int nowPosY){
		this.nowPosY = nowPosY;
	}

	public int getUpActionValue(){
		return upActionValue;
	}

	public void setUpActionValue(int upActionValue){
		this.upActionValue = upActionValue;
	}

	public int getGold(){
		return gold;
	}

	public void setGold(int gold){
		this.gold = gold;
	}

	public int getExp(){
		return exp;
	}

	public void setExp(int exp){
		this.exp = exp;
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

	public List<object> getCards(){
		return  cards;
	}

	public void setCards(List<object> list){
		this.cards = list;
	}

}