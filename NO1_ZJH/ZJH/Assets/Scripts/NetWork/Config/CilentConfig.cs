using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Xml;

/// <summary>
/// 消息处理器配置类
/// </summary>
public class CilentConfig
{
    private Dictionary<string, ProtocolsProcessor> protocolsProcessor;
	private static CilentConfig me = new CilentConfig();

	private CilentConfig()
	{

	}
	public  static CilentConfig getInstance()
	{	
		return me ;
	}

    /// <summary>
    /// 读配置文件
    /// </summary>
    public void loadConfig()
    {
		TextAsset textAsset =null;// CardResourcesload.instance.LoadXml(GlobalConfig.FUNCTION_CONFIG_PATH) as TextAsset;//weiboBUG
        if (textAsset != null)
        {
            ReadConfig1(textAsset.text); 
        }
    }

    private void ReadConfig1(String path)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(path);

        //获取所有protocol-processors节点
        XmlNode configXN = xmlDoc.SelectSingleNode("config");
        //获取所有protocols节点
        XmlNodeList protocolsXN = configXN.ChildNodes; 
        foreach (XmlNode proXN in protocolsXN)
        {
            //获取所有protocol-processors节点
            XmlNodeList protocolProcessors = proXN.ChildNodes;
            if (protocolProcessors != null && protocolProcessors.Count > 0)
            {
                protocolsProcessor = new Dictionary<string, ProtocolsProcessor>();

                foreach (XmlNode processXN in protocolProcessors)
                {
                    //获取所有processor-mapping节点
                    XmlNodeList processorMapping = processXN.ChildNodes;
                    foreach (XmlNode mappingXN in processorMapping)
                    {
                        if (mappingXN.Name == "processor-mapping")
                        {
                            XmlNodeList xnList = mappingXN.ChildNodes;

                            string protocolId = xnList.Item(0).InnerText.ToString();
                            string messageBody = xnList.Item(1).InnerText.ToString();

                            if (protocolId != null && !"".Equals(protocolId) && messageBody != null && !"".Equals(messageBody))
                            {
                                ProtocolsProcessor proProcessor = new ProtocolsProcessor();
                                proProcessor.ProtocolId = protocolId;
                                proProcessor.MessageBody = messageBody;

                                if (protocolId.StartsWith("0x"))
                                {
                                    long l = Convert.ToInt64(protocolId.Substring(2), 16);
                                    protocolId = l.ToString();
                                }

                                protocolsProcessor.Add(protocolId, proProcessor);
                            }
                            else
                            { 
                                Debug.LogError("protocolId is config error!");
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获得协议处理类
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ProtocolsProcessor getProcessorName(String key)
    {
        if (protocolsProcessor != null && protocolsProcessor.ContainsKey(key))
        {
            return protocolsProcessor[key];
        }
        else
        {
            return null;
        }
    }
}
