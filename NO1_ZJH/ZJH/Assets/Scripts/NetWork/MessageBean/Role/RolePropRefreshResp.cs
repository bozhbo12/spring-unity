public class RolePropRefreshResp : MessageBody{
	
	private int roleLevel; // 角色等级;
	private int currExper; // 当前角色经验值;
	private int gold; // 金币;
	private int rmbGold; // 点券;
//	private int cardListCount; // 卡牌集合数量
//	private List<CardRes> roleCardList;// 卡牌集合


	public override void setSequnce(ProtocolSequence ps) {
		ps.add("roleLevel", 0);
		ps.add("currExper", 0);
		ps.add("gold", 0);
		ps.add("rmbGold", 0);
//		ps.add("cardListCount", 0);
//		ps.addObjectArray("roleCardList", "com.snail.webgame.game.protocal.rolemgt.prop.CardRes", "cardListCount");
		
	}

	public int getRoleLevel() {
		return roleLevel;
	}

	public void setRoleLevel(int roleLevel) {
		this.roleLevel = roleLevel;
	}

	public int getCurrExper() {
		return currExper;
	}

	public void setCurrExper(int currExper) {
		this.currExper = currExper;
	}

	public int getGold() {
		return gold;
	}

	public void setGold(int gold) {
		this.gold = gold;
	}

	public int getRmbGold() {
		return rmbGold;
	}

	public void setRmbGold(int rmbGold) {
		this.rmbGold = rmbGold;
	}
}
