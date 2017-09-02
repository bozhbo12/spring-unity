using System;


public class GmCmdReq : MessageBody
{
	
	private string cmd;
	public override void setSequnce(ProtocolSequence ps)
    {
        ps.add("cmd");
    }
	
	
	public string getCMD(){return cmd;}
	public void   setCMD(string v){cmd = v;}
	
	public GmCmdReq ()
	{
	}

}
