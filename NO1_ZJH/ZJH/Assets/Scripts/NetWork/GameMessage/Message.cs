  


 public class Message {
	
	private MessageHead header;
	private MessageBody body;
	
	public MessageBody getBody() {
		return body;
	}
	public void setBody(MessageBody body) {
		this.body = body;
	}
	public MessageHead getHead() {
		return header;
	}
	public void setHead(MessageHead head) {
		this.header = head;
	}
}
