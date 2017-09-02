public class GameErrorResponse : MessageBody{
	private int result; //错误码

	public override void setSequnce(ProtocolSequence ps){
		ps.add("result",0);
	}

	public int getResult(){
		return result;
	}

	public void setResult(int result){
		this.result = result;
	}

}