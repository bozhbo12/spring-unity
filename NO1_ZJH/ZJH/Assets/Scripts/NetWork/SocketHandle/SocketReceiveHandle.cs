using UnityEngine;
using System; 
using System.Collections;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Reflection;
public class SocketReceiveHandle
{
	
	private static SocketReceiveHandle s_instance = new SocketReceiveHandle();
	private SocketReceiveHandle() {}
	
	public static SocketReceiveHandle Instance()
	{
		return s_instance;
	}
	public  void HandelMessage(byte[] data,MessageCode  messageCode)
	{
		
		if(data.Length==0)
		{
			return ;
		}
		Stream stream = new  MemoryStream();
	    stream.Write(data,0,data.Length);
		stream.Position = 0 ;
		GameMessageHead gameMessageHead = new GameMessageHead ();

        messageCode.getMessageHead(GlobalConfig.GAME_SERVER_NAME, stream, gameMessageHead);

        
		int protocolId = gameMessageHead.getMsgType();


        // 服务器返回心跳维护不处理;
	        if (protocolId == ServerMessage.SERVER_ACTIVE_REQ)
	        {
				Debug.LogError(" ========SERVER_ACTIVE_REQ");
	        }

		//获取服务器消息头;
			if (protocolId == ServerMessage.SERVER_LOGIN_RESP) 
			{
				 SingletonMessage.instance.SetRoleKey(gameMessageHead.getUserID2());
			}
		
			if(protocolId != ServerMessage.SERVER_PHALANX_STOP_RESP)
            	Debug.Log("the==Socket Receive MessageId:0x"+Convert.ToString(protocolId, 16));

            MessageBody messageBody = ClassMapping.buildBody("" + protocolId);
		
            if (messageBody != null)
            {
                //转换
                messageCode.getMessageBody(stream, "" + protocolId, messageBody);
			
				//分发消息
				if(SingletonMessage.instance.GetServerMessage(protocolId))
				{
					Debug.LogError("==========================--=-=-=");
//					Client.NetMessageDispather.Dispatch(0x9999,this,messageBody);
				}
				Debug.LogError("=----=-=-=-=-=-=-=-=-=-=-==-=-=-==-=-=-=-");
//				Client.NetMessageDispather.Dispatch(protocolId,this,messageBody);
				
            }
            else
            {
                Debug.LogError("--ClassMapping buildBody is null,protocolId=:0x" + Convert.ToString(protocolId, 16));
            }

		stream.Close();		
	}

}
