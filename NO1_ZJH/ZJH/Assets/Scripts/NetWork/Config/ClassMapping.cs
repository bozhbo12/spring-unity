using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using UnityEngine;

/// <summary>
/// 类构建器
/// author:tianyt
/// </summary>
public class ClassMapping
{
    private static Dictionary<string, MessageBody> classMapBody = new Dictionary<string, MessageBody>();
    private static object BodyObject = new object();

    /// <summary>
    /// 构建消息体
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static MessageBody buildBody(string key)
    {
        MessageBody body = null;

        if (classMapBody.ContainsKey(key))
        {
			//if(DebugConfig.Debug_Net_Protocal)Debug.Log("classMapBody.ContainsKey key=" + key );
            Type type = classMapBody[key].GetType();
            body = (MessageBody)System.Activator.CreateInstance(type);
        }
        else
        {
            lock (BodyObject)
            {
                if (!classMapBody.ContainsKey(key))
                {
                    ProtocolsProcessor pm = null;
                    string name = null;
                    pm = NetMessageConfig.getInstance().getProcessorName(key);
					
                    if (pm != null)
                    {
                        name = pm.MessageBody;
                    }
					
                    if (name == null || name.Length == 0)
                    {
                        return null;
                    }
                    try
                    {
                        Assembly asm = Assembly.GetExecutingAssembly();
                        body = asm.CreateInstance(name, true) as MessageBody;

                        classMapBody.Add(key, body);
                        body = System.Activator.CreateInstance(body.GetType()) as MessageBody;
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.LogError("MessageBody init error , ProtocolId：" + key + ",Exception:" + e);
                    }
                }
                else
                {
                    Type t = classMapBody[key].GetType();
                    body = System.Activator.CreateInstance(t) as MessageBody;
                }
            }
        }
        return body;
    }
}

