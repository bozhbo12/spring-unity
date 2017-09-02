using System;
using System.Text;

public class BaseResp : MessageBody {

	private int result;

	public override void setSequnce(ProtocolSequence ps) {
		ps.add("result", 0);
	}

	public int getResult() {
		return result;
	}

	public void setResult(int result) {
		this.result = result;
	}

}
