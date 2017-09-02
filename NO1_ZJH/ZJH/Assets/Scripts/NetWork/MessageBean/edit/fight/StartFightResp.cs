using System;
using System.Collections;
using System.Collections.Generic;

public class StartFightResp : MessageBody {

	private int result;
//	private long fightId;
//	private int landForm;
//	private String fightMapType;
//	private int fightType;
//	private long endTime1;
//	private long endTime2;
//	private int controlType;
//	private int count;
//	private List list;
	
	public override void setSequnce(ProtocolSequence ps) {
	 
		ps.add("result", 0);
//		ps.add("fightId", 0);
//		ps.add("landForm", 0);
//		ps.addString("fightMapType", "flashCode", 0);
//		ps.add("fightType", 0);
//		ps.add("endTime1", 0);
//		ps.add("endTime2", 0);
//		ps.add("controlType", 0);
//		ps.add("count", 0);
//		ps.addObjectArray("list", "com.snail.webgame.game.protocal.fight.in.RoleInFightArmy", "count");
	}

	public int getResult() {
		return result;
	}

	public void setResult(int result) {
		this.result = result;
	}

//	public long getFightId() {
//		return fightId;
//	}
//
//	public void setFightId(long fightId) {
//		this.fightId = fightId;
//	}
//
//	public int getLandForm() {
//		return landForm;
//	}
//
//	public void setLandForm(int landForm) {
//		this.landForm = landForm;
//	}
//
//	public String getFightMapType() {
//		return fightMapType;
//	}
//
//	public void setFightMapType(String fightMapType) {
//		this.fightMapType = fightMapType;
//	}
//
//	public int getFightType() {
//		return fightType;
//	}
//
//	public void setFightType(int fightType) {
//		this.fightType = fightType;
//	}
//
//	public long getEndTime1() {
//		return endTime1;
//	}
//
//	public void setEndTime1(long endTime1) {
//		this.endTime1 = endTime1;
//	}
//
//	public long getEndTime2() {
//		return endTime2;
//	}
//
//	public void setEndTime2(long endTime2) {
//		this.endTime2 = endTime2;
//	}
//
//	public int getControlType() {
//		return controlType;
//	}
//
//	public void setControlType(int controlType) {
//		this.controlType = controlType;
//	}
//
//	public int getCount() {
//		return count;
//	}
//
//	public void setCount(int count) {
//		this.count = count;
//	}
//
//	public List getList() {
//		return list;
//	}
//
//	public void setList(List list) {
//		this.list = list;
//	}

}
