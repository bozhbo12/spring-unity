public class GameSuccessResponse : MessageBody{
	private int result; //正确码;
	private string param; //参数;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("result",0);
		ps.addString("param","flashCode",0);
	}

	public int getResult(){
		return result;
	}

	public void setResult(int result){
		this.result = result;
	}

	public string getParam(){
		return param;
	}

	public void setParam( string param){
		this.param = param;
	}

}