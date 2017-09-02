using System.Collections;
using System.Collections.Generic;
public class FindFriendResp : MessageBody{
	private int friendCount; //好友列表数量;
	private List<object> friendList; //好友列表;

	public override void setSequnce(ProtocolSequence ps){
		ps.add("friendCount",0);
		ps.addObjectArray("friendList","FriendRes","friendCount");
	}

	public int getFriendCount(){
		return friendCount;
	}

	public void setFriendCount(int friendCount){
		this.friendCount = friendCount;
	}

	public List<object> getFriendList(){
		return  friendList;
	}

	public void setFriendList(List<object> list){
		this.friendList = list;
	}

}