using System;


public class GmCmdResp : MessageBody
{
	
	private int  result;
	private string cmd;
	public override void setSequnce(ProtocolSequence ps)
    {
		ps.add("result");
        ps.addString("cmd","flashCode");
    }
	
	public int  getResult(){return result;}
	public void setResult(int v) {result = v;}
	
	public string getCMD(){return cmd;}
	public void   setCMD(string v){cmd = v;}
	
	public GmCmdResp ()
	{
	}

	
}
