
/**
 * 
 * 消息实体抽象类
 */
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


abstract public class  MessageBody{
	
	
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
