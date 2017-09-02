using System;
using System.Text;
using System.Collections.Generic;

public class ReceiveFightMsgResp : MessageBody{
	private int bossId; //BOSSID;
	private string roleName; //角色名;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("bossId");
		ps.addString("roleName","flashCode");
	}

	public int getBossId(){
		return bossId;
	}

	public void setBossId(int bossId){
		this.bossId = bossId;
	}

	public string getRoleName(){
		return roleName;
	}

	public void setRoleName(string roleName){
		this.roleName = roleName;
	}

}