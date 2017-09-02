public class FightReq : MessageBody{
	private int cardId; // 卡牌Id 
	private int hp; // 卡牌HP 
	private int baseHp; // 卡牌基础HP 
	private int attack; // 卡牌攻击力 
	private int position; // 卡牌位置 
	private double exp; // 卡牌当前等级经验 
	private int level; // 卡牌等级 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("cardId",0);
		ps.add("hp",0);
		ps.add("baseHp",0);
		ps.add("attack",0);
		ps.add("position",0);
		ps.add("exp",0);
		ps.add("level",0);
	}

	public int getCardId(){
		return cardId;
	}

	public void setCardId(int cardId){
		this.cardId = cardId;
	}

	public int getHp(){
		return hp;
	}

	public void setHp(int hp){
		this.hp = hp;
	}

	public int getBaseHp(){
		return baseHp;
	}

	public void setBaseHp(int baseHp){
		this.baseHp = baseHp;
	}

	public int getAttack(){
		return attack;
	}

	public void setAttack(int attack){
		this.attack = attack;
	}

	public int getPosition(){
		return position;
	}

	public void setPosition(int position){
		this.position = position;
	}

	public double getExp(){
		return exp;
	}

	public void setExp(double exp){
		this.exp = exp;
	}

	public int getLevel(){
		return level;
	}

	public void setLevel(int level){
		this.level = level;
	}

}