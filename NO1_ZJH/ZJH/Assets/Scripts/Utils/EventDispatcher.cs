using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 事件分发器
/// </summary>
public class EventDispatcher 
{
	class TMessage
	{
		public TMessage(int id, object sender,object args)
		{
			this.id = id; this.sender = sender; this.args = args;
		}
		public int id;
		public object sender;
		public object args;
	};
	
	public delegate void EventHandler(int id,object sender,object args);
	private Dictionary<int, EventHandler> m_handlers = new Dictionary<int, EventHandler>();
	private List<TMessage> m_messages_tmp = new List<TMessage>();
    private List<TMessage> m_messages = new List<TMessage>();
	
	/// <summary>
	/// 注册事件回调
	/// </summary>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	/// <param name='handler'>
	/// Handler.
	/// </param>
	public void Register(int id,EventHandler handler)
	{
		if(m_handlers.ContainsKey(id))
		{
			m_handlers.Remove(id);
		}
		m_handlers.Add(id,handler);
	}
	
	/// <summary>
	/// 删除事件回调
	/// </summary>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	public void Remove(int id)
	{
		if(m_handlers.ContainsKey(id))
		{
			m_handlers.Remove(id);
		}
	}
	
	/// <summary>
	/// 分发事件 在相应的Update时候调用
	/// </summary>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	public void Dispatch(int id,object sender, object args)
	{
		if(!m_handlers.ContainsKey(id))
		{
			return;
		}
		EventHandler handler = m_handlers[id];
		if(handler!=null)
		{
			m_messages.Add(new TMessage(id,sender,args));
		}
	}
	/// <summary>
	/// 分发事件，马上执行
	/// </summary>
	/// <param name='id'>
	/// Identifier.
	/// </param>
	/// <param name='sender'>
	/// Sender.
	/// </param>
	/// <param name='args'>
	/// Arguments.
	/// </param>
	public void DispatchNow(int id ,object sender, object args)
	{
		if(!m_handlers.ContainsKey(id))
		{
			return;
		}
		EventHandler handler = m_handlers[id];
		
		if(handler != null)
		{
			handler(id,sender,args);
		}
	}
	/// <summary>
	/// 调度器调度时
	/// </summary>
	public void OnUpdate()
	{
		//if(support mutil Thread lock this code block)
		foreach(TMessage msg in m_messages)
		{
			m_messages_tmp.Add(msg);
		}
		m_messages.Clear();
		
		
		//process events
		foreach(TMessage msg in m_messages_tmp)
		{
			EventHandler handler = m_handlers[msg.id];
			if(handler != null)
			{
				handler(msg.id,msg.sender,msg.args);
			}
		}
		m_messages_tmp.Clear();
		
	}
	
}
