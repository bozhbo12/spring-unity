public class DuplicateStepReq : MessageBody{
	private int posX; // 到达点X坐标;
	private int posY; // 到达点Y坐标;
	private int beforePosX;// 到达点X坐标;
	private int beforePosY;// 到达点Y坐标;


	public override void setSequnce(ProtocolSequence ps){
		ps.add("posX",0);
		ps.add("posY",0);
		ps.add("beforePosX", 0);
		ps.add("beforePosY", 0);

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
	public int getBeforePosX() {
		return beforePosX;
	}

	public void setBeforePosX(int beforePosX) {
		this.beforePosX = beforePosX;
	}

	public int getBeforePosY() {
		return beforePosY;
	}

	public void setBeforePosY(int beforePosY) {
		this.beforePosY = beforePosY;
	}
}