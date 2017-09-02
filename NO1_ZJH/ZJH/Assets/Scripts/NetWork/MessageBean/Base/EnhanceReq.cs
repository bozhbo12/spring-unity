using UnityEngine;
using System.Collections;

public class EnhanceReq : MessageBody {
	private long cardSeqId;//Enhance card id
	private string cardSeqIds;//expend card ids
	
	public override void setSequnce(ProtocolSequence ps) {
		ps.add("cardSeqId", 0);
		ps.addString("cardSeqIds", "falshCode", 0);
	}
	public long getCardSeqId() {
		return cardSeqId;
	}

	public void setCardSeqId(long cardSeqId) {
		this.cardSeqId = cardSeqId;
	}

	public string getCardSeqIds() {
		return cardSeqIds;
	}

	public void setCardSeqIds(string cardSeqIds) {
		this.cardSeqIds = cardSeqIds;
	}
}
