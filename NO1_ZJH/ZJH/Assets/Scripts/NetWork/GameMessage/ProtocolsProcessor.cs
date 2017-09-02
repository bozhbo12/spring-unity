/// <summary>
/// 消息处理类
/// author:tianyt
/// </summary>
public class ProtocolsProcessor
{
    private string protocolId;

    public string ProtocolId
    {
      get { return protocolId; }
      set { protocolId = value; }
    }
    private string messageBody;

    public string MessageBody
    {
        get { return messageBody; }
        set { messageBody = value; }
    }
}
