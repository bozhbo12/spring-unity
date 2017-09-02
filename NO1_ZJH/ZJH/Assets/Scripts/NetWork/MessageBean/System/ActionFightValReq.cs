public class ActionFightValReq : MessageBody{
	private int regainType; //1-actionValue 2-fightValue;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("regainType",0);
	}

	public int getRegainType(){
		return regainType;
	}

	public void setRegainType(int regainType){
		this.regainType = regainType;
	}

}