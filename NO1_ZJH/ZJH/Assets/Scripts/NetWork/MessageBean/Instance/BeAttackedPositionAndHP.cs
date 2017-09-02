public class BeAttackedPositionAndHP : MessageBody{
	private int position; // 卡牌位置;
	private int HP; //负数表示伤害，正数表示加血;
	private int baseHp; // 卡牌基础HP;
	private int skillId; //技能ID 0-普通攻击 -1-消除所有BUFFER,疾病-29 瘟疫-30 裂伤-35 除外;
	private string beSkillId;
	private string beSkillIdEffect; // 被动技能影响;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("position",0);
		ps.add("HP",0);
		ps.add("baseHp",0);
		ps.add("skillId",0);
		ps.addString("beSkillId","flashCode",0);
		ps.addString("beSkillIdEffect","flashCode",0);
	}

	public int getPosition(){
		return position;
	}

	public void setPosition(int position){
		this.position = position;
	}

	public int getHP(){
		return HP;
	}

	public void setHP(int HP){
		this.HP = HP;
	}

	public int getBaseHp(){
		return baseHp;
	}

	public void setBaseHp(int baseHp){
		this.baseHp = baseHp;
	}

	public int getSkillId(){
		return skillId;
	}

	public void setSkillId(int skillId){
		this.skillId = skillId;
	}

	public string getBeSkillId(){
		return beSkillId;
	}

	public void setBeSkillId(string beSkillId){
		this.beSkillId = beSkillId;
	}
	
	public string getBeSkillIdEffect(){
		return beSkillIdEffect;
	}
	
	public void setBeSkillIdEffect(string beSkillIdEffect)
	{
		this.beSkillIdEffect = beSkillIdEffect;
	}
	
}