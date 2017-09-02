public class CountPos : MessageBody{
	private int posX; 
	private int posY; 

	public override void setSequnce(ProtocolSequence ps){
		ps.add("posX",0);
		ps.add("posY",0);
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

}