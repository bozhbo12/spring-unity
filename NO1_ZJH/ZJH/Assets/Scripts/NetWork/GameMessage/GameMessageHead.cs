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
    private int UserID4;
    private int UserID5;
    private int UserID6;
    private int UserID7;
    private int MsgType;

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
    public int getUserID4()
    {
        return UserID4;
    }
    public void setUserID4(int userID4)
    {
        UserID4 = userID4;
    }
    public int getUserID5()
    {
        return UserID5;
    }
    public void setUserID5(int userID5)
    {
        UserID5 = userID5;
    }
    public int getUserID6()
    {
        return UserID6;
    }
    public void setUserID6(int userID6)
    {
        UserID6 = userID6;
    }
    public int getUserID7()
    {
        return UserID7;
    }
    public void setUserID7(int userID7)
    {
        UserID7 = userID7;
    }

    public override void setSequnce(ProtocolSequence ps)
    {
        ps.add("Version");
        ps.add("UserID0");
        ps.add("UserID1");
        ps.add("UserID2");
        ps.add("UserID3");
        ps.add("UserID4");
        ps.add("UserID5");
        ps.add("UserID6");
        ps.add("UserID7");
        ps.add("MsgType");
    }


}
