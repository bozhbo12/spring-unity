// 0xA162
using System.Collections;
using System.Collections.Generic;
public class FragmentExchangeResp : MessageBody{
	private int fragmentId; //碎片ID 
	private int result; //兑换结果  1-成功  其他错误码-失败 
	private int dropCount; // 掉落物品数量 
	private List<object> dropList; // 掉落物品集合 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("fragmentId",0);
		ps.add("result",0);
		ps.add("dropCount",0);
		ps.addObjectArray("dropList","RoleCardInfoRes","dropCount");
	}

	public int getFragmentId(){
		return fragmentId;
	}

	public void setFragmentId(int fragmentId){
		this.fragmentId = fragmentId;
	}

	public int getResult(){
		return result;
	}

	public void setResult(int result){
		this.result = result;
	}

	public int getDropCount(){
		return dropCount;
	}

	public void setDropCount(int dropCount){
		this.dropCount = dropCount;
	}

	public List<object> getDropList(){
		return dropList;
	}

	public void setDropList(List<object> list){
		this.dropList = list;
	}

}