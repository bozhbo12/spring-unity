using UnityEngine; 
using System; 
using System.Net.Sockets; 
using System.Net; 
using System.Collections; 
using System.Text;
using System.IO;
using System.Threading;



/// <summary>
/// 需要重构成为异步版本
/// 心跳包可以用协逞实现
/// </summary>
public class UnitySocket 
{
    private string LocalIP;
	private int LocalPort;
	private Socket mSocket;
	private MessageCode messageCode = new MessageCode();

    private static UnitySocket s_Instance = new UnitySocket();

    private UnitySocket() 
    { 
    }
	
    public static UnitySocket Instance 
    {
        get {
            return s_Instance;
        } 
    }
	
	public MessageCode getSocketMessageCode()
	{
		return messageCode;
	}

    public Socket getSocket()
    {
        return mSocket;
    }
	//非连接状态;
	public void NotConnet()
	{
			this._isConn = false;
	}
	
    //public bool SocketInit()
    //{
    //    return SocketInit(false);
    //}
    public void SocketInit()
    {
	//	NGUIDebug.Log("GlobalConfig.GAME_SERVER_IP = "+GlobalConfig.GAME_SERVER_IP+" , GlobalConfig.GAME_SERVER_PORT[0] = "+GlobalConfig.GAME_SERVER_PORT[0]);
		if(this.mSocket != null && this.mSocket.Connected == true){DisconnectNow();}
        AsyncConnect(GlobalConfig.GAME_SERVER_IP, GlobalConfig.GAME_SERVER_PORT[0]);
	}	
	
	
	bool needLog = false;
	public bool sendData(Message message)
	{
        Stream stream = messageCode.setMessageSteam(GlobalConfig.GAME_SERVER_NAME, message);
		if(stream==null)
		{
			return false;
		}
		sendData( stream);
        
		needLog = (message.getHead().getProtocolId() != 65535);
		if (needLog)
			Debug.Log("sendData:" + message.getHead().getProtocolId().ToString());
		return true;
	}
	
	
	public bool  sendData(Stream stream)
	{

		byte[] b = new byte[stream.Length];
		stream.Position = 0;
		stream.Read(b,0,b.Length);
	    byte[] lenByte = BaseMessageHandle.getSendInt(b.Length);
		
 		Debug.Log("@@sendData1:" + b.Length + "|" + stream.Length);
		
 	    stream.Close();
		if(mSocket.Connected)
		{
		if (needLog)
			{
				Debug.Log("sendData1:" + lenByte.Length.ToString() + "|" + lenByte.ToString());
		Debug.Log("sendData2:" + b.Length.ToString() + "|" + b.ToString());
			}
			mSocket.Send(lenByte);
			mSocket.Send(b);
			return true;
		}
		else{
			return false ; 	
		}	
	}
 	

	/// <summary>
	/// Unity3d 策略验证(在开启socket前必须先完成验证,
	/// Security.PrefetchSocketPolicy默认发送:<policy-file-request>到服务器
	/// </summary>
	public bool validatePolicy(string LocalIP, int LocalPort) {
		Debug.Log("==============validatePolicy=========");
		if (Application.webSecurityEnabled) {
			return Security.PrefetchSocketPolicy(LocalIP, LocalPort);
        }
		Debug.Log("==========validatePolicy=========");
        return true;
	}
	
	
	//Thread m_ReceiveThread = null;
	Thread m_CheckThread = null;
	
    ///// <summary>
    ///// 读取线程
    ///// </summary>
    //private void _SocketReceiveThread()
    //{
    //    while(true)
    //    {
    //        if(mSocket!=null&&mSocket.Connected)
    //        {
    //             if(!_beginReadMsg())
    //            {
    //                Debug.LogWarning("Socket disconnected");
    //                break;
    //            }
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Socket disconnected");
    //            break;
    //        }
    //        Thread.Sleep(1);
    //    }
    //}
    //private  bool _beginReadMsg()
    //{				
    //    byte[] lengthByte = new byte[4];
    //    if(mSocket != null && !mSocket.Connected) 
    //    {
    //        ///<网络断开>
    //        return false;
    //    }
    //    if(mSocket != null && mSocket.Receive(lengthByte) != 4)
    //    {
    //        ///<网络出错>
    //        Debug.LogWarning("Net Receive error");
    //    }
    //    int msgLength = TypeConvert.bytesToInt(lengthByte);	
    //    return _readMessage(msgLength);
    //}					
    //private bool  _readMessage(int msgLength)
    //{
    //       byte[] msgByte = new byte[msgLength];
    //       if(mSocket != null && !mSocket.Connected) 
    //       {
    //            //<网络断开>
    //            return false;
    //       }
		
    //      if(mSocket.Receive(msgByte) != msgLength)
    //      {
    //            Debug.LogWarning("Net receive error");
    //      }
    //       SocketReceiveHandle.Instance().HandelMessage(msgByte,getSocketMessageCode());
    //       msgLength = 0 ;
    //       if(mSocket!=null&&mSocket.Connected)
    //       {
    //          return _beginReadMsg();
    //       }
    //    return false;
    //}
	/// <summary>
	/// 心跳线程
	/// </summary>
	private void _CheckServerThread()
	{
		while(true)
		{
			if(mSocket!=null&&mSocket.Connected)
			{
				Message message = new Message();
				GameMessageHead messageHead  = BaseMessageHandle.getMessageHead(1,1,(int)ServerMessage.SERVER_ACTIVE_REQ,1);
				message.setHead(messageHead);
				sendData(message);
                Debug.Log("====##CheckServerThread===");
			}
			else
			{
				Debug.LogWarning("Socket disconnected");
				break;
			}
			Thread.Sleep(1000);
		}
	}
	
    /// <summary>
    /// 全局断开socket连接
    /// </summary>
    //public void DisconnectNow()
    //{
    //    try
    //    {
    //        try
    //        {
    //            if(mSocket != null && mSocket.Connected)
    //            {
    //                mSocket.Close();//Shutdown(SocketShutdown.Both);
    //                //mSocket.Disconnect(false);
    //                mSocket = null;
    //            }
    //            if(mSocket != null)
    //            {
    //                mSocket = null;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Debug.LogError(ex.Message);
    //        }
    //        if(m_CheckThread != null)
    //        {
    //            m_CheckThread.Join();
    //            m_CheckThread = null;
    //        }
    //        if(m_ReceiveThread != null)
    //        {
    //            m_ReceiveThread.Join();
    //            m_CheckThread = null;
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        Debug.LogError("CheckServerThread and SocketReceiveThread colse error,e= " + e.Message);
    //    }
    //}


    #region ========异步通信开始========

    private bool _isConn;
    private object _lock = new object();
    private string _ip;
    private int _port;
    private int _offset;
    private uint _dataSize;
    private int _state = 1;
    private byte[] _dataBuffer;
    private int _postRecvre;
    private bool _hasError;
    private int _endRecvre;

    public void AsyncConnect(string ip, int port)
    {
		Debug.Log("===AsyncConnect===" + this._ip +"|" + this._port);
        if (this._isConn)
        {
            Debug.Log("connected!");
        }
        else
        {
            this._ip = ip;
            this._port = port;
			
			if(this.mSocket !=null && this.mSocket.Connected)
				DisconnectNow();
			
            try
            {
				Debug.Log("===AsyncConnect===" + this._ip +"|" + this._port);
                this.mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.mSocket.NoDelay = true;
				this.mSocket.BeginConnect(this._ip, this._port, new AsyncCallback(this.OnConnected), this.mSocket);

                Debug.Log(string.Concat(new object[] { "start to connect: ", ip, ":", port }));
            }
            catch (Exception exception)
            {
                Debug.LogError("NetWork Info On AsyncConnect:" + exception.ToString());
                this.OnError();
            }
        }
    }

    private void OnConnected(IAsyncResult ia)
    {
		Debug.LogError("----------------------------------1---------------------------------------");
        try
        {
            Socket asyncState = ia.AsyncState as Socket;
            if (!asyncState.Connected)
            {
                Debug.LogError("Network connected error.");
                this.OnError();
//				Data.Instance.PortIndex++;
//				if(Data.Instance.PortIndex < GlobalConfig.GAME_SERVER_PORT.Length)
//				{
//					SocketInit();
//				}
//				else{
//					Debug.Log("===================ServerMessageServerMessageServerMessageServerMessage=========2=======");
//					Client.NetMessageDispather.Dispatch(ServerMessage.CLENT_SOCKET_LOSS,null,null);
//				}
                return;
            }
			Debug.LogError("Network connected.");
            asyncState.EndConnect(ia);
//			Data.Instance.FristLongin = GlobalConfig.GAME_SERVER_PORT[Data.Instance.PortIndex]; 
			Debug.Log("===================ServerMessageServerMessageServerMessageServerMessage=============1===");
//			Client.NetMessageDispather.Dispatch(ServerMessage.CLIENT_SOCKET_SUCCEED,null,null);
        }
        catch (Exception exception)
        {
            Debug.LogError("NetWork Info On OnConnected:" + exception.ToString());
            this.OnError();
            return;
        }
        this.StartRead();
        this._isConn = true;
        this._hasError = false;

        if (m_CheckThread == null)
        {
            //m_CheckThread = new Thread(new ThreadStart(_CheckServerThread));
            //m_CheckThread.Start();
        }

        Debug.Log("socket connecton succ..");
    }

    public void StartRead()
    {
        int state = 1;
        uint dataSize = 4;
        this.StartRead(state, dataSize);
    }


    public void StartRead(int state, uint dataSize)
    {
        object obj2 = this._lock;
        Monitor.Enter(obj2);
        try
        {
            this._offset = 0;
            this._dataSize = dataSize;
            this._state = state;
            this._dataBuffer = new byte[this._dataSize];
            this.mSocket.BeginReceive(this._dataBuffer, this._offset, ((int)this._dataSize) - this._offset, SocketFlags.None, new AsyncCallback(this.OnRead), null);
            this._postRecvre++;
        }
        catch (Exception exception)
        {
            Debug.Log(exception.ToString());
            this.OnError();
        }
        finally
        {
            Monitor.Exit(obj2);
        }
    }

    private void OnRead(IAsyncResult ar)
    {

        if (this._hasError)
        {
            Debug.Log("ClientRpc.OnRead, has error, return.");
            this._hasError = true;
            this.DisconnectNow();
        }
        else
        {
            object obj2 = this._lock;
            Monitor.Enter(obj2);
            try
            {
                int num = this.mSocket.EndReceive(ar);
                this._endRecvre++;
                this._offset += num;
                if (num <= 0)
                {
                    this._hasError = true;
//					Client.NetMessageDispather.Dispatch(ServerMessage.SERVER_LOGIN_OUT_RESP,null,null);
                    Debug.LogError("ClientRpc.OnRead, call EndReceive() error!, close!");
                }
                else
                {
                    switch (this._state)
                    {
                        case 1:
                            BeginReadMsg();
                            break;
                        case 2:
                            ReadMessage();
                            break;
                    }
                }
                return;
            }
            catch (Exception exception)
            {
                #region exception info
                string str = string.Empty;
                if (exception.InnerException != null)
                {
                    string[] textArray2 = new string[] { exception.InnerException.Message, "\n", exception.StackTrace, "\n", exception.Source };
                    str = string.Concat(textArray2);
                }
                else
                {
                    string[] textArray3 = new string[] { exception.Message, "\n", exception.StackTrace, "\n", exception.Source };
                    str = string.Concat(textArray3);
                }
                Debug.LogError("Socket disconnected.." + str);
                #endregion
            }
            finally
            {
                Monitor.Exit(obj2);
            }
        }
    }

    private void BeginReadMsg()
    {
        if (this._offset == this._dataSize)
        {
            uint readLength = Convert.ToUInt32(TypeConvert.bytesToInt(this._dataBuffer));
            this.StartRead(2, readLength);
        }
        else
        {
            this.ContinueRead();
        }
    }

    private void ReadMessage()
    {
        if (this._offset == this._dataSize) //消息接收完毕
        {
            SocketReceiveHandle.Instance().HandelMessage(this._dataBuffer, getSocketMessageCode());
            this.StartRead();
        }
        else
        {
            this.ContinueRead();
        }
    }

    private void ContinueRead()
    {
        try
        {
            this.mSocket.BeginReceive(this._dataBuffer, this._offset, ((int)this._dataSize) - this._offset, SocketFlags.None, new AsyncCallback(this.OnRead), null);
            this._postRecvre++;
        }
        catch (Exception exception)
        {
            Debug.LogError(exception.ToString());
            this.OnError();
        }
    }

    public void OnError()
    {
        this._hasError = true;
        Debug.Log("socket err");

        if (this._isConn)
        {
            try
            {
                this.DisconnectNow();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception.ToString());
            }
            this._isConn = false;
        }
    }

    /// <summary>
    /// 全局断开socket连接
    /// </summary>
    public void DisconnectNow()
    {
        Debug.Log("connection close");
        try
        {
            if (mSocket != null && mSocket.Connected)
            {
                this.mSocket.Shutdown(SocketShutdown.Both);
                this.mSocket.Close();
            }
            if (mSocket != null)
            {
                mSocket = null;
            }
			this._isConn = false;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    
        if (m_CheckThread != null)
        {
            m_CheckThread.Join();
            m_CheckThread = null;
        }
    }
    #endregion ========异步通信结束========
}