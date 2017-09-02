public class RaceFightReq : MessageBody{
	private int playerId; //敌方角色ID;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("playerId",0);
	}

	public int getPlayerId(){
		return playerId;
	}

	public void setPlayerId(int playerId){
		this.playerId = playerId;
	}

}