using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GPSResp : MessageBody{

	private int result;
	private int count;
	private List<object> roleList = new List<object>();
	
	public override void setSequnce(ProtocolSequence ps) {
		ps.add("result", 0);
		ps.add("count", 0);
		ps.addObjectArray("roleList", "GPSRoleListRe", "count");
		
	}

	public int getCount() {
		return count;
	}

	public void setCount(int count) {
		this.count = count;
	}

	public List<object> getRoleList() {
		return roleList;
	}

	public void setRoleList(List<object> roleList) {
		this.roleList = roleList;
	}

	public int getResult() {
		return result;
	}

	public void setResult(int result) {
		this.result = result;
	}

}
