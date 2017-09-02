/**
 * 刷新到客户端的添加好友请求
 * 
 * @author xiasd
 *
 */
public class SendAddFriendResp : MessageBody{
	private int count;// 好友请求数量

	public override void setSequnce(ProtocolSequence ps) {
		ps.add("count", 0);
	}

	public int getCount() {
		return count;
	}

	public void setCount(int count) {
		this.count = count;
	}

}
