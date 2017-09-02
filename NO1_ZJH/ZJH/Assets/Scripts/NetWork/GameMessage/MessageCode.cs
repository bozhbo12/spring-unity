using UnityEngine;
using System; 
using System.Collections;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Reflection;

public class MessageCode
{
 
 
	private  Hashtable msgHead = new Hashtable ();
	private  Hashtable msgBody = new Hashtable ();
	private  Hashtable msgHeadSeq = new Hashtable ();
	private  Hashtable msgBodySeq = new Hashtable ();
	
	public MessageCode()
	{
		
		
	}
	
	
	public Stream setMessageSteam(String serviceName,Message message)
	{
		
		Stream stream = new  MemoryStream();
		MessageHead messageHead = message.getHead();
		MessageBody messageBody = message.getBody();

		Hashtable headTable = getMessageHeadMethod(serviceName,messageHead);
	 
		if(headTable == null ) 
		{
			return null;
		}

		List<string> headSeqList = getMessageHeadSeq(serviceName,messageHead);
	 
		if(headSeqList == null ) 
		{
			
			return null;
		}
		for(int i=0;i<headSeqList.Count;i++)
		{
		
			if(headSeqList[i].StartsWith("GET"))
			{
				MethodInfo method = (MethodInfo)headTable[headSeqList[i]];
	 
				Type type = method.ReturnType;
			 
				BaseMessageHandle.setMsgDataStream(stream,type.Name.ToUpper(),method.Invoke(messageHead,null),false);
			}
		}
		if(messageBody!=null)
		{
			setMessageBody( stream,""+messageHead.getProtocolId(),messageBody);	
		}

		return stream;
		
	}
	
	public void setMessageBody(Stream stream,string protocolId,MessageBody messageBody)	
	{
		
		Hashtable bodyTable = getMessageBodyMethod(""+protocolId,messageBody);
	 	
		List<string> bodySeqList = getMessageBodySeq(""+protocolId,messageBody);
 
	 
	 	for(int i=0;i<bodySeqList.Count;i++)
		{
			if(bodySeqList[i].IndexOf("-")!=-1&&bodySeqList[i].StartsWith("GET"))
			{
				//对象数组
                string[] ArrayStr = bodySeqList[i].Split(new char[] { '-' });
                MethodInfo method = (MethodInfo)bodyTable[ArrayStr[0]];
                List<object> bodys = (List<object>)method.Invoke(messageBody, null);

                if (bodys != null && bodys.Count > 0)
                {
                    for (int j = 0; j < bodys.Count; j++)
                    {
                        if (!bodys[j].Equals(messageBody))
                        {
                            MessageBody body = (MessageBody)bodys[j];
                            setMessageBody(stream, protocolId + "-" + bodySeqList[i], body);
                        }
                    }
                }
			}
			else if(bodySeqList[i].StartsWith("GET"))
			{
		 
				MethodInfo method = (MethodInfo)bodyTable[bodySeqList[i]];

				Type type = method.ReturnType;
		
				BaseMessageHandle.setMsgDataStream(stream,type.Name.ToUpper(),method.Invoke(messageBody,null),true);
			}
		}
	}
	
	
	public void getMessageHead(String serviceName,Stream stream ,MessageHead messageHead )
	{
		
		
		Hashtable headTable = getMessageHeadMethod(serviceName,messageHead);
	 
		if(headTable == null ) 
		{
			return ;
		}

		List<string> headSeqList = getMessageHeadSeq(serviceName,messageHead);
	 
		if(headSeqList == null ) 
		{
			
			return ;
		}
		for(int i=0;i<headSeqList.Count;i++)
		{
		
			if(headSeqList[i].StartsWith("SET"))
			{
				MethodInfo method = (MethodInfo)headTable[headSeqList[i]];
				ParameterInfo[] parm = method.GetParameters();
				Type  type =  parm[0].ParameterType;
 
				object obj = BaseMessageHandle.getMsgDataStream(stream,type.Name.ToUpper(),false);
				method.Invoke(messageHead,new object[]{obj});
			}
		}
	}
	
	
	public void getMessageBody(Stream stream ,string protocolId,MessageBody messageBody )
	{
		
	 
		try{
			Hashtable bodyTable = getMessageBodyMethod(""+protocolId,messageBody);
			 	
			List<string> bodySeqList = getMessageBodySeq(""+protocolId,messageBody);
			

			
			for(int i=0;i<bodySeqList.Count;i++)
			{

				if(bodySeqList[i].IndexOf("-")!=-1&&bodySeqList[i].StartsWith("SET"))
				{
					//对象数组;
					string [] ArrayStr = bodySeqList[i].Split(new char[]{'-'});
					MethodInfo method = (MethodInfo)bodyTable[ArrayStr[0]];
					string className = ArrayStr[1];
				   	string countStr = "GET"+ArrayStr[2];
					
				    MethodInfo method1 = (MethodInfo)bodyTable[countStr];
					int count = (int)method1.Invoke(messageBody,null);
					
			 		Assembly asm = Assembly.GetExecutingAssembly();
					
					
					List<object> list  = new List<object>();
					for(int j=0;j<count;j++)
					{
						
						object obj = asm.CreateInstance(className,true);
						getMessageBody(stream ,protocolId+"-"+bodySeqList[i],(MessageBody)obj);
						
						list.Add(obj);
					}
					 
			 		method.Invoke(messageBody,new object[]{list});
 
				}
				else if(bodySeqList[i].StartsWith("SET"))
				{
					MethodInfo method = (MethodInfo)bodyTable[bodySeqList[i]];
					ParameterInfo[] parm = method.GetParameters();
					Type  type =  parm[0].ParameterType;
					object obj = BaseMessageHandle.getMsgDataStream(stream,type.Name.ToUpper(),true);
					method.Invoke(messageBody,new object[]{obj});
				}
			}
		} catch(Exception e) {
			string es = e.StackTrace;
			Debug.Log("Exception throw when parse:0x"+Convert.ToString(Convert.ToInt32(protocolId),16)+" MessageBody"+messageBody.GetType().Name+" Exception:"+e.Message);
		}

	}
	
	
	
	
	
	
	private Hashtable getMessageHeadMethod(String serviceName,	MessageHead messageHead)
	{
		Hashtable headTable = null;
	 	if(msgHead.ContainsKey(serviceName))
		{
			headTable = (Hashtable)msgHead[serviceName];
		}
		else
		{
			lock(this)
			{
			  if(msgHead.ContainsKey(serviceName))
		      {
			    headTable = (Hashtable)msgHead[serviceName];
		      }
			  else
			  {
				 //缓存消息头里面的方法名称
				 headTable = new Hashtable ();
				 Type type = messageHead.GetType();
			     MethodInfo [] methods = type.GetMethods();
			     for(int j=0;j<methods.Length;j++)
			     {
					string name = methods[j].Name.ToUpper();
				
				    headTable.Add(name,methods[j]);
		
			     }
				 msgHead.Add(serviceName,headTable);

			   }
			 }
		 }	
		
		
		return headTable;
	}
	
	private  List<string> getMessageHeadSeq(String serviceName,MessageHead messageHead)
	{
		
		List<string> headSeqList = null;
		if(msgHeadSeq.ContainsKey(serviceName))
		{
			headSeqList = (List<string>)msgHeadSeq[serviceName];
		}
		else
		{
			
			lock(this)
			{
				if(msgHeadSeq.ContainsKey(serviceName))
				{
					headSeqList = (List<string>)msgHeadSeq[serviceName];
				}
				else
				{
					headSeqList = new List<string>();
					ProtocolSequence headSeq = messageHead.getSequnce();
	 
					List<string> seqList = headSeq.getList();
			
					for(int i=0;i<seqList.Count;i++)
					{
						string name = seqList[i].ToUpper();
						
					 	headSeqList.Add("GET"+name);
						headSeqList.Add("SET"+name);
					}
					
					msgHeadSeq.Add(serviceName,headSeqList);
				}
			}
		}

		
		
		return  headSeqList;
	}
	
 	private Hashtable getMessageBodyMethod(string protocolId,	MessageBody messageBody)
	{
		Hashtable bodyTable = null;
	 	if(msgBody.ContainsKey(protocolId))
		{
			bodyTable = (Hashtable)msgBody[protocolId];
		}
		else
		{
			lock(this)
			{
			  if(msgBody.ContainsKey(protocolId))
		      {
			    bodyTable = (Hashtable)msgBody[protocolId];
		      }
			  else
			  {
				 //缓存消息体里面的方法名称
				 bodyTable = new Hashtable ();
				 Type type = messageBody.GetType();
			     MethodInfo [] methods = type.GetMethods();
			     for(int j=0;j<methods.Length;j++)
			     {
				   string name = methods[j].Name.ToUpper();
				   bodyTable.Add(name,methods[j]);
			 
			     }
				 msgBody.Add(protocolId,bodyTable);
			   }
			 }
		 }	
		
		
		return bodyTable;
	}
	
  	private  List<string> getMessageBodySeq(string protocolId,MessageBody messageBody)
	{
		
		List<string> bodySeqList = null;
		if(msgBodySeq.ContainsKey(protocolId))
		{
			bodySeqList = (List<string>)msgBodySeq[protocolId];
		}
		else
		{
			
			lock(this)
			{
				if(msgBodySeq.ContainsKey(protocolId))
				{
					bodySeqList = (List<string>)msgBodySeq[protocolId];
				}
				else
				{
					bodySeqList = new List<string>();
					ProtocolSequence bodySeq = messageBody.getSequnce();
	 
					List<string> seqList = bodySeq.getList();
			
					for(int i=0;i<seqList.Count;i++)
					{
						string name = seqList[i].ToUpper();
					 	bodySeqList.Add("GET"+name);
						bodySeqList.Add("SET"+name);
					}
					
					msgBodySeq.Add(protocolId,bodySeqList);
				}
			}
		}

		
		
		return  bodySeqList;
	}
}
