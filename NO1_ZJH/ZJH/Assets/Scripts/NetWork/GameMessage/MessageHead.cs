 



abstract public class MessageHead   {

	public abstract int getLength();
	public abstract void setLength(int length);
	public abstract long getProtocolId();
	public abstract void setProtocolId(long Id);
 
	/**
	 * 设置协议字段顺序
	 * @return
	 */
	public abstract void setSequnce(ProtocolSequence ps);
	
 
	/**
	 * 获得协议字段顺序
	 * @return
	 */
	public  ProtocolSequence getSequnce()
	{	
		ProtocolSequence ps =new ProtocolSequence();
		setSequnce(ps);
		return ps;
	}
 

}
