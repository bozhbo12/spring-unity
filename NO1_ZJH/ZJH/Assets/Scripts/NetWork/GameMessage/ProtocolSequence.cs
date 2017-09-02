 
using System;
using System.Collections.Generic;


/**
 * 
 * 设置协议字段顺序
 *
 */
public  class ProtocolSequence {
	 
	
	List<string> list = new  List<string>();
//	List<string> listType = new  List<string>();
	
	
	/**
	 * 添加一个协议字段
	 * @param name 字段名称 
	 * @param resource 处理字符串编码解码的资源类
	 */
	public void addString(string name,string resource)
	{
		list.Add(name);
//		listType.Add(resource);

	}
	/**
	 * 添加一个协议字段
	 * @param name 字段名称 
	 * @param endian 字节类型 0--bigEndian 1--littleEndian
	 * 
	 */
	public void add(string name)
	{
		list.Add(name);
//		listType.Add("Default");
	}
	public void add(string name, int number)
	{
		list.Add(name);
	}
	public void addString(string name,string resource, int number)
	{
		list.Add(name);
	}
	/**
	 * 添加一个对象字段
	 * @param name 字段名称 
	 */
//	public void addObject(string name,string num)
//	{
//		list.Add(name+","+num);
//	}
	/**
	 * 添加一个对象数组字段
	 * @param name 字段名称 
	 * @param className list中的对象类型
	 * @param List 大小对应字段
	 */
	public void addObjectArray(string name,string className,string num)
	{
		list.Add(name+"-"+className+"-"+num);
//		listType.Add("classArray-"+num);
//		list.Add(obj);
//		list.Add(num);
	}

 
	public List<string> getList()
	{
		return list;	
	}

	public int size()
	{
		return list.Count;
	}
}
