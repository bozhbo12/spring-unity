using System.Collections;
using System.Collections.Generic;
public class GmResp : MessageBody{
	private int gold; // 金钱;
	private int rmbGold; // 黄金;
	private int activeValue; // 行动值;
    private int exp; // 经验值;
    private int level; // 等级;
	private int cardListCount; // 卡牌集合数量;
	private List<object> roleCardList; // 卡牌集合;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("gold",0);
		ps.add("rmbGold",0);
		ps.add("activeValue",0);
        ps.add("exp", 0);
        ps.add("level", 0);
		ps.add("cardListCount",0);
		ps.addObjectArray("roleCardList","RoleCardInfoRes","cardListCount");
	}

    public int getExp()
    {
        return exp;
    }

    public void setExp(int exp)
    {
        this.exp = exp;
    }

    public int getLevel()
    {
        return level;
    }

    public void setLevel(int level)
    {
        this.level = level;
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

	public int getActiveValue(){
		return activeValue;
	}

	public void setActiveValue(int activeValue){
		this.activeValue = activeValue;
	}

	public int getCardListCount(){
		return cardListCount;
	}

	public void setCardListCount(int cardListCount){
		this.cardListCount = cardListCount;
	}

	public List<object> getRoleCardList(){
		return  roleCardList;
	}

	public void setRoleCardList(List<object> list){
		this.roleCardList = list;
	}

}