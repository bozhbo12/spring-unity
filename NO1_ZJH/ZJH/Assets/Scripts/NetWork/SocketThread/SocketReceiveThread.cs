//using UnityEngine; 
//using System; 
//using System.Net.Sockets; 
//using System.Net; 
//using System.Collections; 
//using System.Text;
//using System.IO;
//using System.Threading;
//
//
//public class SocketReceiveThread 
//{
//	public static int sceneUI = 0;
//	
//	private  UnitySocket unitySocket;
//
//	private  int msgLength = 0;
//	private  Socket socket  = null ;
//	public static bool flag = true;
//	public SocketReceiveThread(UnitySocket unitySocket)
//	{
//	    this.unitySocket = unitySocket;
//		Monitor.Enter(flag);
//        flag = true;
//		Monitor.Exit(flag);
//	}
//	
//	public static void close() {
//		Monitor.Enter(flag);
//		flag = false;
//		Monitor.Exit(flag);
//	}
//	
//    public void Run()
//	{
//		socket = unitySocket.getSocket();
//		while(true)
//		{
//			
//			Monitor.Enter(flag);
//			if(flag)
//			{
//				try{
//					if(socket!=null&&socket.Connected)
//				    {
//				       beginReadMsg();
//				    }
//					else
//					{
//						sceneUI = 1;
//						Debug.LogError("socket 断开连接");
//						break;
//					}
//				}
//				catch(Exception e)
//				{
//					sceneUI = 1;
//					Debug.LogError(e.Message);
//					if(socket!=null)
//					{
//					  socket.Close();	
//					}
//				}
//			}
//			else {
//				Debug.LogError("socket 断开连接");
//				break;
//			}
//			Monitor.Exit(flag);
//			Thread.Sleep(1);
//		}
//		
//	 
//	}
//	
//	private  void beginReadMsg()
//    {				
//		byte[] lengthByte = new byte[4];
//		socket.Receive(lengthByte);
//		msgLength = TypeConvert.bytesToInt(lengthByte);	
//		readMessage();
//	}
//							
//	private void  readMessage()
//	{
//	
//		   byte[] msgByte = new byte[msgLength];
//		   socket.Receive(msgByte);
//	 	   SocketReceiveHandle.Instance().HandelMessage(msgByte,unitySocket.getSocketMessageCode());
//		   msgLength = 0 ;
//		   if(socket!=null&&socket.Connected)
//		   {
//			  beginReadMsg();
//		   }
//	   
//	
//	}
//}