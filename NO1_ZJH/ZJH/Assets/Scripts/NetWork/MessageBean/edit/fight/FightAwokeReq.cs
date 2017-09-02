using System;
using System.Collections;
using System.Collections.Generic;

public class FightAwokeReq : MessageBody{

 
	private int result;
	private int fightServerId;
	private long fightId;
	//private String mapId;
	
	public override void setSequnce(ProtocolSequence ps) {
	 
		ps.add("result", 0);
		ps.add("fightServerId", 0);
		ps.add("fightId", 0);
		//ps.addString("mapId", "flashCode", 0);
	}

	public int getResult() {
		return result;
	}

	public void setResult(int result) {
		this.result = result;
	}

	public long getFightId() {
		return fightId;
	}

	public void setFightId(long fightId) {
		this.fightId = fightId;
	}

	//public String getMapId() {
	//	return mapId;
	//}

	public int getFightServerId() {
		return fightServerId;
	}

	public void setFightServerId(int fightServerId) {
		this.fightServerId = fightServerId;
	}

	//public void setMapId(String mapId) {
	//	this.mapId = mapId;
	//}
	//public String getMapId()
	//{
	//	return this.mapId;
	//}
}
