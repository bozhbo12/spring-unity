using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonReq : MessageBody{


	private int optionType;
	private string optionStr;


	public override void setSequnce(ProtocolSequence ps){
		ps.add("optionType",0);
		ps.addString("optionStr","flashCode",0);
	}

	public int getOptionType(){
		return optionType;
	}

	public void setOptionType(int optionType){
		this.optionType = optionType;
	}

	public string getOptionStr(){
		return optionStr;
	}

	public void setOptionStr(string optionStr){
		this.optionStr = optionStr;
	}
}