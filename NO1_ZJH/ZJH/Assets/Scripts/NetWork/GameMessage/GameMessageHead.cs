using UnityEngine;
using System.Collections;

public class GameMessageHead : MessageHead
{
    private int Length;
    private int Version;
    private int UserID0; // 客户端角色ID
    private int UserID1; // 游戏通讯服务器ID
    private int UserID2; // 服务器端使用序列号
    private int UserID3; // 战斗运算服务器ID
    private int MsgType;
	private string userState; // 


    public override long getProtocolId()
    {

        return MsgType;
    }
    public override void setProtocolId(long protocolId)
    {

        this.MsgType = (int)protocolId;
    }

    public override int getLength()
    {
        return Length;
    }
    public override void setLength(int Length)
    {
        this.Length = Length;
    }

    public int getMsgType()
    {
        return MsgType;
    }

    public void setMsgType(int msgType) {
        this.MsgType = msgType;
    }

    public int getUserID0()
    {
        return UserID0;
    }
    public void setUserID0(int userID0)
    {
        UserID0 = userID0;
    }
    public int getUserID1()
    {
        return UserID1;
    }
    public void setUserID1(int userID1)
    {
        UserID1 = userID1;
    }
    public int getUserID2()
    {
        return UserID2;
    }
    public void setUserID2(int userID2)
    {
        UserID2 = userID2;
    }
    public int getUserID3()
    {
        return UserID3;
    }
    public void setUserID3(int userID3)
    {
        this.UserID3 = userID3;
    }
    public int getVersion()
    {
        return Version;
    }
    public void setVersion(int version)
    {
        Version = version;
    }
	public string getUserState()
	{
		return userState;
	}
	public void setUserState(string userState)
	{
		userState = userState;
	}

    public override void setSequnce(ProtocolSequence ps)
    {
        ps.add("Version");
        ps.add("UserID0");
        ps.add("UserID1");
        ps.add("UserID2");
        ps.add("UserID3");
        ps.add("MsgType");
		ps.add("userState");
    }
}
