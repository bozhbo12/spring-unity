/*
 * @(#) UserLogoutResp.java 1.00 2011-9-8
 *
 *   FILENAME     :  UserLogoutResp.java
 *   PACKAGE      :  com.snail.webgame.game.protocal.rolemgt.logout
 *   CREATE DATE  :  2011-9-8
 *   AUTHOR       :  zhoubo
 *   MODIFIED BY  :  
 *   DESCRIPTION  :  User Logout Response
 */

using System;
using System.Text;

/**
 * 角色登出Response
 *
 * @author zhoubo
 * @version 1.00
 * @since 2011-9-8
 */
public class UserLogoutResp : MessageBody {
	private int result;
	private int roleId;

	public override void setSequnce(ProtocolSequence ps) {

		ps.add("result", 0);
		ps.add("roleId", 0);
	}

	public int getRoleId() {
		return roleId;
	}

	public void setRoleId(int roleId) {
		this.roleId = roleId;
	}

	public int getResult() {
		return result;
	}

	public void setResult(int result) {
		this.result = result;
	}

}
