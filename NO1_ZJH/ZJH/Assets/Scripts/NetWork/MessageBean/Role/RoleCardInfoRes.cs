// 0xA014
public class RoleCardInfoRes : MessageBody{
	private long cardSeqId; // 角色物品id 
	private int cardId; // 卡牌ID 
	private int dropType; //掉落类型 1-卡牌 2-碎片 
	private int cardLevel; // 卡牌等级 
	private byte position; // 卡牌阵型 0代表不在阵型中 1代表1号位，以此类推 
	private double curExp; // 卡牌当前经验 
	private int maxLevel; // 卡牌当前最大等级 
	private long createTime; // 获取时间 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("cardSeqId",0);
		ps.add("cardId",0);
		ps.add("dropType",0);
		ps.add("cardLevel",0);
		ps.add("position",0);
		ps.add("curExp",0);
		ps.add("maxLevel",0);
		ps.add("createTime",0);
	}

	public long getCardSeqId(){
		return cardSeqId;
	}

	public void setCardSeqId(long cardSeqId){
		this.cardSeqId = cardSeqId;
	}

	public int getCardId(){
		return cardId;
	}

	public void setCardId(int cardId){
		this.cardId = cardId;
	}

	public int getDropType(){
		return dropType;
	}

	public void setDropType(int dropType){
		this.dropType = dropType;
	}

	public int getCardLevel(){
		return cardLevel;
	}

	public void setCardLevel(int cardLevel){
		this.cardLevel = cardLevel;
	}

	public byte getPosition(){
		return position;
	}

	public void setPosition(byte position){
		this.position = position;
	}

	public double getCurExp(){
		return curExp;
	}

	public void setCurExp(double curExp){
		this.curExp = curExp;
	}

	public int getMaxLevel(){
		return maxLevel;
	}

	public void setMaxLevel(int maxLevel){
		this.maxLevel = maxLevel;
	}

	public long getCreateTime(){
		return createTime;
	}

	public void setCreateTime(long createTime){
		this.createTime = createTime;
	}

}