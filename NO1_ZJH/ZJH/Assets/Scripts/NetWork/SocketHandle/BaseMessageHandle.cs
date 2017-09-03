	using UnityEngine; 
	using System; 
 	using System.IO;
	using System.Net.Sockets; 
	using System.Net; 
	using System.Collections;
	using System.Text;
public class BaseMessageHandle 
{
	
	public BaseMessageHandle()
	{
		
	}
	
	
	public static void setMsgDataStream(Stream stream,string type,object obj,bool flag)
	{
		
		switch(type)
		{
			case "BYTE":
				  if(flag)
				  {
				 	//stream.WriteByte(0);
				  }
				 stream.WriteByte((byte)obj);
				 break;
			
			case "INT16":
			
				  if(flag)
				  {
				 	//stream.WriteByte(1);
				  }
				 stream.Write(getSendShort((short)obj),0,2);
				 break;
			
			case "INT32":
			
				  if(flag)
				  {
				 	//stream.WriteByte(2);
				  }
				 stream.Write(getSendInt((int)obj),0,4);
				 break;
			case "INT64":
			
				  if(flag)
				  {
				 	//stream.WriteByte(9);
				  }
				 stream.Write(getSendLong((long)obj),0,8);			
				 break;
			case "FLOAT":
			
				  if(flag)
				  {
				 	//stream.WriteByte(3);
				  }
				 stream.Write(getSendFloat((float)obj),0,4);
				 break;
			case "SINGLE":
			
				  if(flag)
				  {
				 	//stream.WriteByte(3);
				  }
				 stream.Write(getSendFloat((float)obj),0,4);
				 break;
			case "DOUBLE":
				  if(flag)
				  {
				 	//stream.WriteByte(7);
				  }
				 stream.Write(getSendDouble((double)obj),0,8);
				 break;
			case "LONG":
				  if(flag)
				  {
				 	//stream.WriteByte(9);
				  }
				 stream.Write(getSendLong((long)obj),0,8);			
				 break;
			case "STRING":
				  if(flag)
				  {
				 	//stream.WriteByte(4);
				  }
				 byte [] g = getSendString((string)obj);	
				 stream.Write( g,0,g.Length);			
				 break;
		}
	}
	
	
	
	
   private static byte[] ReverseBytes(byte[] inArray)
   {
      byte temp;
      int highCtr = inArray.Length - 1;

      for (int ctr = 0; ctr < inArray.Length / 2; ctr++)
      {
         temp = inArray[ctr];
         inArray[ctr] = inArray[highCtr];
         inArray[highCtr] = temp;
         highCtr -= 1;
      }
      return inArray;
   }
	public static byte[] getSendShort(short data) 
	{ 
		byte [] bytes =BitConverter.GetBytes(data);
		if(BitConverter.IsLittleEndian)
		{
			bytes = ReverseBytes(bytes);
		}
	 
			return bytes;
	} 
		 
	public static byte[] getSendLong(long data) 
	{ 
		 
		byte [] bytes =BitConverter.GetBytes(data);
		if(BitConverter.IsLittleEndian)
		{
			bytes = ReverseBytes(bytes);
		}
	 
			return bytes;
			 
	} 
		 
	public static byte[] getSendInt(int data) 
	{ 
 
	    byte [] bytes =BitConverter.GetBytes(data);
		if(BitConverter.IsLittleEndian)
		{
			bytes = ReverseBytes(bytes);
		}
	 
		return bytes;
	} 
	
	
	public static byte[] getSendFloat(float data)
	{
		byte [] bytes =BitConverter.GetBytes(data);
		if(BitConverter.IsLittleEndian)
		{
			bytes = ReverseBytes(bytes);
		}
	 
			return bytes;
	}
		
	public static byte[] getSendDouble(double data)
	{
		byte [] bytes =BitConverter.GetBytes(data);
		if(BitConverter.IsLittleEndian)
		{
			bytes = ReverseBytes(bytes);
		}
	 
			return bytes;
	}	
	
	
	public static byte[] getSendString(string data) 
	{ 
	 
		return TypeConvert.stringToBytes(data);
	} 
		

		
	
	public static object getMsgDataStream(Stream stream,string type,bool flag)
	{
		
		byte [] b = null;
		
		switch(type)
		{
			case "BYTE":
				  if(flag)
				  {
				 	//stream.ReadByte();
				  }

				  return (byte)stream.ReadByte();

			case "INT16":
			
				  if(flag)
				  {
				  	//stream.ReadByte();
				  }
				  b = new byte[2];
				  stream.Read(b,0,2);
				  return getRecShort(b);
			case "INT32":
			
				  if(flag)
				  {
				  	//stream.ReadByte();
				  }
				 b = new byte[4];
				 stream.Read(b,0,4);
				 return getRecInt(b);
			case "INT64":
			
				  if(flag)
				  {
				  	//stream.ReadByte();
				  }
				 b = new byte[8];
				 stream.Read(b,0,8);
				 return getRecLong(b);
			case "FLOAT":
			
				  if(flag)
				  {
				 	 	//stream.ReadByte();
				  }
				  b = new byte[4];
				  stream.Read(b,0,4);
				  return getRecFloat(b);
			case "SINGLE":
			
				  if(flag)
				  {
				 	 	//stream.ReadByte();
				  }
				  b = new byte[4];
				  stream.Read(b,0,4);
				  return getRecFloat(b);
			case "DOUBLE":
				  if(flag)
				  {
				  	//stream.ReadByte();
				  }
				  b = new byte[8];
				  stream.Read(b,0,8);
				  return getRecDouble(b);
			
			case "LONG":
				  if(flag)
				  {
				 	 	//stream.ReadByte();
				  }
				  b = new byte[8];
				  stream.Read(b,0,8);
				  return getRecLong(b);
 	
				
			case "STRING":
				  if(flag)
				  {
				 	// stream.ReadByte();
				  }
				  byte [] c = new byte[2];
				  stream.Read(c,0,2);
				  short length = getRecShort(c);
			
				  b = new byte[length];
				  stream.Read(b,0,length);
				  return getRecString(b);
		}
		
		return null;
		
	}
	
	
		
		
		
	public static short getRecShort(byte[] b)
	{
		
			
		if(BitConverter.IsLittleEndian)
		{
			byte [] bytes = ReverseBytes(b);
			return BitConverter.ToInt16(bytes,0);
		}
		
		return BitConverter.ToInt16(b,0);
	}
	public static int getRecInt(byte[] b)
	{
		if(BitConverter.IsLittleEndian)
		{
			byte [] bytes = ReverseBytes(b);
			return BitConverter.ToInt32(bytes,0);
		}
		
		return BitConverter.ToInt32(b,0);
	}
	
	public static float  getRecFloat(byte[] b)
	{
		if(BitConverter.IsLittleEndian)
		{
			byte [] bytes = ReverseBytes(b);
			return BitConverter.ToSingle(bytes,0);
		}
		
		return BitConverter.ToSingle(b,0);;
	}
	
	public static long  getRecLong(byte[] b)
	{
		if(BitConverter.IsLittleEndian)
		{
			byte [] bytes = ReverseBytes(b);
			return BitConverter.ToInt64(bytes,0);
		}
		
		return BitConverter.ToInt64(b,0);;
	}
	
	public static double  getRecDouble(byte[] b)
	{
		if(BitConverter.IsLittleEndian)
		{
			byte [] bytes = ReverseBytes(b);
			return BitConverter.ToDouble(bytes,0);
		}
		
		return BitConverter.ToDouble(b,0);;
	}
	
	
	
	public static string getRecString(byte[] b)
	{
		
	 	return Encoding.UTF8.GetString(b);
		
	}


	public static GameMessageHead getMessageHead(int roleId, int gateServerId, int msgType, int sceneServerId , string userState)
    {
        int sceneId = sceneServerId;
//        if (SceneInfoMap.RoleInfo.SceneId != 0)
//        {
//            sceneId = SceneInfoMap.RoleInfo.SceneId;
//        }
		return getMessageHead(roleId, gateServerId, 0, sceneId, msgType,userState);
    }
	
	public static GameMessageHead getMessageHead(int UserID0,int UserID1,int UserID2,int UserID3,int msgType,string userState)
	{
 
		GameMessageHead messageHead = new GameMessageHead ();
		messageHead.setVersion(0x00000200);
		messageHead.setUserID0(UserID0);
		messageHead.setUserID1(UserID1);
		messageHead.setUserID2(UserID2);
		messageHead.setUserID3(UserID3); //sceneServerId  ///<>战斗ID<><
		messageHead.setMsgType(msgType);
		messageHead.setUserState(userState);
		return messageHead;
	}
	
	
	
}