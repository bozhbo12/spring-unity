using System;
using System.Net.Sockets;
public static class NetUtils
{
	
	/// <summary>
	/// 发送网络消息 成功返回true
	/// </summary>
	public static bool Send(int messageId, MessageBody body)
	{
//		if(!SingletonMessage.instance.BoolSend(messageId))
//		{
//			Logger.Info("============================================Send=error=================2==========================" + messageId);
//			return false;
//		}
		
		UnitySocket unitySocket = UnitySocket.Instance;
        Socket socket = unitySocket.getSocket();

        if (socket != null && socket.Connected)
        {
            Message messgae = new Message();
			
			GameMessageHead messageHead = BaseMessageHandle.getMessageHead(1, 1, messageId, 1,string.Empty);
			
			if (messageId == ServerMessage.SERVER_KEY_REQ)
			{
				messageHead.setUserID2(SingletonMessage.instance.GetRoleKey());
			}
            messgae.setHead(messageHead);
			messgae.setBody(body);
            unitySocket.sendData(messgae);
			return true;
        }
		return false;
	}
}


