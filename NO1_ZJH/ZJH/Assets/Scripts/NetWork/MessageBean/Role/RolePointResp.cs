public class RolePointResp : MessageBody{
	private int count; // 角色点数 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("count",0);
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

}