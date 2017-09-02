using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GPSRoleListRe : MessageBody {
	
	private string roleName;
	private string rolePic;
	private int roleLevel;
	private double distance;
	private int count;
	private List<object> roleCardPositionList = new List<object>();

	public override void setSequnce(ProtocolSequence ps) {
		ps.addString("roleName", "flashCode", 0);
		//ps.addString("rolePic", "flashCode", 0);
		ps.add("roleLevel", 0);
		ps.add("count", 0);
		ps.addObjectArray("roleCardPositionList", "RoleCardInfoRes", "count");
	}

	public string getRoleName() {
		return roleName;
	}

	public void setRoleName(string roleName) {
		this.roleName = roleName;
	}

	public string getRolePic() {
		return rolePic;
	}

	public void setRolePic(string rolePic) {
		this.rolePic = rolePic;
	}

	public int getRoleLevel() {
		return roleLevel;
	}

	public void setRoleLevel(int roleLevel) {
		this.roleLevel = roleLevel;
	}

	public int getCount() {
		return count;
	}

	public void setCount(int count) {
		this.count = count;
	}

	public List<object> getRoleCardPositionList() {
		return roleCardPositionList;
	}

	public void setRoleCardPositionList(List<object> roleCardPositionList) {
		this.roleCardPositionList = roleCardPositionList;
	}

	public double getDistance() {
		return distance;
	}

	public void setDistance(double distance) {
		this.distance = distance;
	}

}

