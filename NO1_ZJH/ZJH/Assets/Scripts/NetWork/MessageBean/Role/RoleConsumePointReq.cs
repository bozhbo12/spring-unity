public class RoleConsumePointReq : MessageBody{
	private int pkgId; // 卡包Id 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("pkgId",0);
	}

	public int getPkgId(){
		return pkgId;
	}

	public void setPkgId(int pkgId){
		this.pkgId = pkgId;
	}

}