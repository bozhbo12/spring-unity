using System.Collections;
using System.Collections.Generic;
public class DuplicateEnterResp : MessageBody{
	private int posX; // 当前副本X坐标 ;
	private int posY; // 当前副本Y坐标 ;
	private int count; // 点数量; 
    private List<object> coutPosList; // 亮的点（无战争迷雾） ;
    private byte isPass; 
	private int count1; // 事件数量; 
	private List<object> countEventList; // 事件集合;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("posX",0);
		ps.add("posY",0);
		ps.add("count",0);
        ps.addObjectArray("coutPosList", "CountPos", "count");
        ps.add("isPass", 0);
        ps.add("count1", 0);
		ps.addObjectArray("countEventList","CountEvent","count1");
	}

	public int getPosX(){
		return posX;
	}

	public void setPosX(int posX){
		this.posX = posX;
	}

	public int getPosY(){
		return posY;
	}

	public void setPosY(int posY){
		this.posY = posY;
	}

	public int getCount(){
		return count;
	}

	public void setCount(int count){
		this.count = count;
	}

	public List<object> getCoutPosList(){
		return coutPosList;
	}

	public void setCoutPosList(List<object> list){
		this.coutPosList = list;
	}

    public int getisPass()
    {
        return isPass;
	}

    public void setisPass(byte isPass)
    {
        this.isPass = isPass;
	}

	public int getCount1(){
		return count1;
	}

	public void setCount1(int count1){
		this.count1 = count1;
	}

	public List<object> getCountEventList(){
		return countEventList;
	}

	public void setCountEventList(List<object> list){
		this.countEventList = list;
	}

}