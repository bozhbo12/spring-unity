// 0xA158
public class InviteCodeResp : MessageBody{
	private int isInvited; //是否被邀请过  0-未被邀请 1-已邀请   其他错误码 
	private int inviteResult; //被邀请人邀请结果 0-失败  1-成功 
	private int invitedCout; //当前角色邀请数量 
	private string inviteCode; //当前角色的邀请码 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("isInvited",0);
		ps.add("inviteResult",0);
		ps.add("invitedCout",0);
		ps.addString("inviteCode","flashCode",0);
	}

	public int getIsInvited(){
		return isInvited;
	}

	public void setIsInvited(int isInvited){
		this.isInvited = isInvited;
	}

	public int getInviteResult(){
		return inviteResult;
	}

	public void setInviteResult(int inviteResult){
		this.inviteResult = inviteResult;
	}

	public int getInvitedCout(){
		return invitedCout;
	}

	public void setInvitedCout(int invitedCout){
		this.invitedCout = invitedCout;
	}

	public string getInviteCode(){
		return inviteCode;
	}

	public void setInviteCode(string inviteCode){
		this.inviteCode = inviteCode;
	}

}