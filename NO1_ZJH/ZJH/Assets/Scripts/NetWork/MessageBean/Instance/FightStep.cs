using System.Collections;
using System.Collections.Generic;
public class FightStep : MessageBody{
	private int myPosition; // 
	private int targetPosition; // 受击者位置 普通攻击时使用;
	private int beAttackedListCount; //被影响的数量;
	private List<object> beAttackedList; // 被攻击者;
	private int skillId; // 卡牌技能;
    private int roundNum; // 回合数;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("myPosition",0);
		ps.add("targetPosition",0);
		ps.add("beAttackedListCount",0);
		ps.addObjectArray("beAttackedList","BeAttackedPositionAndHP","beAttackedListCount");
		ps.add("skillId",0);
        ps.add("roundNum", 0);
	}

	public int getMyPosition(){
		return myPosition;
	}

	public void setMyPosition(int myPosition){
		this.myPosition = myPosition;
	}

	public int getTargetPosition(){
		return targetPosition;
	}

	public void setTargetPosition(int targetPosition){
		this.targetPosition = targetPosition;
	}

	public int getBeAttackedListCount(){
		return beAttackedListCount;
	}

	public void setBeAttackedListCount(int beAttackedListCount){
		this.beAttackedListCount = beAttackedListCount;
	}

	public List<object> getBeAttackedList(){
		return  beAttackedList;
	}

	public void setBeAttackedList(List<object> list){
		this.beAttackedList = list;
	}

	public int getSkillId(){
		return skillId;
	}

	public void setSkillId(int skillId){
		this.skillId = skillId;
	}

    public int getRoundNum()
    {
        return roundNum;
    }

    public void setRoundNum(int roundNum)
    {
        this.roundNum = roundNum;
    }

}