using UnityEngine;
using System.Collections;

public class ButtonSendMessage : MonoBehaviour
{
    public int messageID;
    public int subMessagetID;

    void Start()
    {
        UIEventListener.Get(gameObject).onClick += OnClickEvent;
    }

    void OnClickEvent(GameObject go)
    {
        DelegateProxy.OnSendMessageCallback(messageID, subMessagetID);
    }
}
