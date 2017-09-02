using UnityEngine;
using System.Collections;


public class FightTestReq: MessageBody
{
	private string fightMapType;
	
	public override void setSequnce(ProtocolSequence ps)
    {
        ps.add("fightMapType");
    }
	
	public string getFightMapType() {return fightMapType;}
	public void   setFightMapType(string v){fightMapType = v;}
	
	public FightTestReq ()
	{
		
	}
}

