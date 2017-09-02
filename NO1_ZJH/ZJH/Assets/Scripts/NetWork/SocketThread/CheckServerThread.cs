//    using UnityEngine; 
//	using System; 
//	using System.Net.Sockets; 
//	using System.Net; 
//	using System.Collections; 
//	using System.Text;
//    using System .IO;
//  using  System.Threading;
////using DateTime ;
// public class CheckServerThread 
// {
//	
//	private  UnitySocket unitySocket;
//	public static bool flag = true;
//	public CheckServerThread(UnitySocket unitySocket)
//	{
//		this.unitySocket = unitySocket;
//		Monitor.Enter(flag);
//        flag = true;
//		Monitor.Exit(flag);
//	}
//	
//	public static void close() {
//		
//		Monitor.Enter(flag);
//		flag = false;
//		Monitor.Exit(flag);
//	}
//	
//	public void Run()
//	{
//		while(true)
//		{
//			Monitor.Enter(flag);
//			if(flag)
//			{
//	            Socket socket = UnitySocket.Instance.getSocket();
//		 
//				if(socket!=null&&socket.Connected)
//				{
//		 
//				   Message message = new Message();
//				   GameMessageHead messageHead  = BaseMessageHandle.getMessageHead(1,1,(int)ServerMessage.SERVER_ACTIVE_REQ,1);
//	   			   DateTime ti = DateTime.Now;
//				   TimeSpan t22 = new TimeSpan(ti.Ticks);
//				   //GameValue.sendTime = t22.TotalMilliseconds;
//				   message.setHead(messageHead);
//			       unitySocket.sendData(message);
//				}
//				else
//				{
//					Logger.Warning("Socket disconnected");
//	 				//服务器断开连接
//				}
//			}
//			else {
//				break;
//			}
//	 		Monitor.Exit(flag);
//			
//			Thread.Sleep(1000);
//		   
//		}
//
//		
//
//	}
//	
//	
// }
