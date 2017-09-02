using UnityEngine;
using System;
using System.IO;
using System.Collections;



public class SocketDataEvent : EventArgs
{

    private MessageBody messageBody;

    public SocketDataEvent(MessageBody messageBody)
    {
        this.messageBody = messageBody;
    }


    public MessageBody MessageBody
    {
        get { return messageBody; }
        set { messageBody = value; }
    }
}
