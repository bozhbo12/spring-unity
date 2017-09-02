#if UNITY_EDITOR
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

namespace TinyBinaryXml
{
	public class TbXmlSerializer
	{
		private List<TbXmlNodeTemplate> nodeTemplates = new List<TbXmlNodeTemplate>();

		private List<TbXmlNode> nodes = new List<TbXmlNode>();

        private List<bool> boolPool = new List<bool>();

        private List<int> intPool = new List<int>();

        private List<long> longPool = new List<long>();

        private List<float> floatPool = new List<float>();

        private List<double> doublePool = new List<double>();

        private List<string> stringPool = new List<string>();

		private ushort nodeIdInc = 0;

		private ushort nodeTemplateIdInc = 0;

        private bool bNewXML = false;


        public byte[] SerializeXmlString(string xmlString)
		{
			if(string.IsNullOrEmpty(xmlString))
			{
				return null;
			}

            nodeTemplates.Clear();
            nodes.Clear();
            stringPool.Clear();
            boolPool.Clear();
            intPool.Clear();
            longPool.Clear();
            floatPool.Clear();
            doublePool.Clear();
            bNewXML = false;
            nodeIdInc = 0;
			nodeTemplateIdInc = 0;

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xmlString);

			TbXmlNode docNode = new TbXmlNode();
			docNode.childrenIds = new List<ushort>();

			XmlNodeList xmlNodeList = doc.ChildNodes;
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                XmlNode xmlNode = xmlNodeList[i];
                if (xmlNode.NodeType == XmlNodeType.Element)
                {
                    TbXmlNodeTemplate nodeTemplate = GetNodeTemplate(xmlNode);
                    if (nodeTemplate == null)
                    {
                        nodeTemplate = new TbXmlNodeTemplate();
                        nodeTemplates.Add(nodeTemplate);
                        nodeTemplate.attributeNames = new List<string>();
                        nodeTemplate.attributeTypes = new List<TB_XML_ATTRIBUTE_TYPE>();
                        nodeTemplate.id = nodeTemplateIdInc;
                        nodeTemplateIdInc++;
                        nodeTemplate.name = xmlNode.Name;
                        for (int j = 0; j < xmlNode.Attributes.Count; j++)
                        {
                            XmlAttribute xmlAttribute = xmlNode.Attributes[j];
                            string attributeString = xmlAttribute.Value;
                            int type = 0;
                            int.TryParse(attributeString, out type);
                            nodeTemplate.attributeTypes.Add((TB_XML_ATTRIBUTE_TYPE)type);
                            nodeTemplate.attributeNames.Add(xmlAttribute.Name);
                        }
                    }
                    if(!ProcessXmlNode(docNode, xmlNode, nodeTemplate))
                    {
                        return null;
                    }
                }
            }

            byte[] buffer = null;
            using (BinaryWriter binaryWriter = new BinaryWriter(new MemoryStream(), Encoding.UTF8))
            {
                Serialize(binaryWriter);

                buffer = new byte[binaryWriter.BaseStream.Length];
                binaryWriter.BaseStream.Position = 0;
                binaryWriter.BaseStream.Read(buffer, 0, (int)binaryWriter.BaseStream.Length);
                binaryWriter.BaseStream.Close();
                binaryWriter.BaseStream.Dispose();
                binaryWriter.Close();
            }
			return buffer;
		}

		private bool ProcessXmlNode(TbXmlNode parentNode, XmlNode xmlNode, TbXmlNodeTemplate nodeTemplate)
		{
            TbXmlNode node = new TbXmlNode();
            nodes.Add(node);
            node.attributeValues = new Dictionary<string, int>();
			node.childrenIds = new List<ushort>();
			node.id = nodeIdInc++;
			node.templateId = nodeTemplate.id;
			parentNode.childrenIds.Add(node.id);
            for (int i = 0; i < xmlNode.Attributes.Count; i++)
            {
                XmlAttribute xmlAttribute = xmlNode.Attributes[i];
                string attributeString = xmlAttribute.Value;
                double attributeDouble;
                bool attributeBool;
                int attributeInt;
                long attributeLong;
                float attributeFloat;
                TB_XML_ATTRIBUTE_TYPE type = TB_XML_ATTRIBUTE_TYPE.VTYPE_UNKNOWN;
                if (nodeTemplate.attributeNames.Contains(xmlAttribute.Name))
                {
                    type = nodeTemplate.attributeTypes[nodeTemplate.attributeNames.IndexOf(xmlAttribute.Name)];
                }
                if (type == TB_XML_ATTRIBUTE_TYPE.VTYPE_BOOL)
                {
                    BoolTryParse(attributeString, out attributeBool);
                    int valueIndex = MatchBoolIndex(attributeBool);
                    if (valueIndex == -1)
                    {
                        boolPool.Add(attributeBool);
                        node.attributeValues.Add(xmlAttribute.Name, boolPool.Count - 1);
                    }
                    else
                    {
                        node.attributeValues.Add(xmlAttribute.Name, valueIndex);
                    }
                }
                else if (type == TB_XML_ATTRIBUTE_TYPE.VTYPE_INT)
                {
                    IntTryParse(attributeString, out attributeInt);
                    int valueIndex = MatchIntIndex(attributeInt);
                    if (valueIndex == -1)
                    {
                        intPool.Add(attributeInt);
                        node.attributeValues.Add(xmlAttribute.Name, intPool.Count - 1);
                    }
                    else
                    {
                        node.attributeValues.Add(xmlAttribute.Name, valueIndex);
                    }
                }
                else if (type == TB_XML_ATTRIBUTE_TYPE.VTYPE_INT64)
                {
                    LongTryParse(attributeString, out attributeLong);
                    int valueIndex = MatchLongIndex(attributeLong);
                    if (valueIndex == -1)
                    {
                        longPool.Add(attributeLong);
                        node.attributeValues.Add(xmlAttribute.Name, longPool.Count - 1);
                    }
                    else
                    {
                        node.attributeValues.Add(xmlAttribute.Name, valueIndex);
                    }
                }
                else if (type == TB_XML_ATTRIBUTE_TYPE.VTYPE_FLOAT)
                {
                    FloatTryParse(attributeString, out attributeFloat);
                    int valueIndex = MatchFloatIndex(attributeFloat);
                    if (valueIndex == -1)
                    {
                         floatPool.Add(attributeFloat);
                        node.attributeValues.Add(xmlAttribute.Name, floatPool.Count - 1);
                    }
                    else
                    {
                        node.attributeValues.Add(xmlAttribute.Name, valueIndex);
                    }
                }
                else if (type == TB_XML_ATTRIBUTE_TYPE.VTYPE_DOUBLE)
                {
                    DoubleTryParse(attributeString, out attributeDouble);
                    int valueIndex = MatchDoubleIndex(attributeDouble);
                    if (valueIndex == -1)
                    {
                        doublePool.Add(attributeDouble);
                        node.attributeValues.Add(xmlAttribute.Name, doublePool.Count - 1);
                    }
                    else
                    {
                        node.attributeValues.Add(xmlAttribute.Name, valueIndex);
                    }
                }
                else
                {
                    int stringIndex = MatchStringIndex(attributeString);
                    if (stringIndex == -1)
                    {
                        stringPool.Add(attributeString);
                        node.attributeValues.Add(xmlAttribute.Name, stringPool.Count - 1);
                    }
                    else
                    {
                        node.attributeValues.Add(xmlAttribute.Name, stringIndex);
                    }
                }
            }
            int index = 0;
            for (int i = 0; i < xmlNode.ChildNodes.Count; i++)
			{
                XmlNode subXmlNode = xmlNode.ChildNodes[i];

                if (subXmlNode.NodeType == XmlNodeType.Element)
                {
                    index++;
                    TbXmlNodeTemplate nodeTemplate1 = GetNodeTemplate(subXmlNode);
                    if (nodeTemplate1 == null)
                    {
                        nodeTemplate1 = new TbXmlNodeTemplate();
                        nodeTemplate1.attributeNames = new List<string>();
                        nodeTemplate1.attributeTypes = new List<TB_XML_ATTRIBUTE_TYPE>();
                        nodeTemplate1.id = nodeTemplateIdInc;
                        nodeTemplate1.name = subXmlNode.Name;
                        for (int j = 0; j < subXmlNode.Attributes.Count; j++)
                        {
                            XmlAttribute xmlAttribute = subXmlNode.Attributes[j];
                            string attributeString = xmlAttribute.Value;
                            if (j == 0 && attributeString.Equals("-1"))
                            {
                                bNewXML = true;
                            }
                            int type = 0;
                            int.TryParse(attributeString, out type);
                            if (!bNewXML)
                            {
                                double attributeDouble;
                                if (DoubleTryParse(attributeString, out attributeDouble))
                                {
                                    nodeTemplate1.attributeTypes.Add(TB_XML_ATTRIBUTE_TYPE.VTYPE_DOUBLE);
                                }
                                else
                                {
                                    nodeTemplate1.attributeTypes.Add(TB_XML_ATTRIBUTE_TYPE.VTYPE_STRING);
                                }
                            }
                            else
                            {
                                nodeTemplate1.attributeTypes.Add((TB_XML_ATTRIBUTE_TYPE)type);    
                            }
                            nodeTemplate1.attributeNames.Add(xmlAttribute.Name);
                        }

                        if (index == 2 || !bNewXML)
                        {
                            nodeTemplates.Add(nodeTemplate1);
                            nodeTemplateIdInc++;
                        }

                        if (index > 2 || !bNewXML)
                        {
                            if (!ProcessXmlNode(node, subXmlNode, nodeTemplate1))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        if(!ProcessXmlNode(node, subXmlNode, nodeTemplate1))
                        {
                            return false;
                        }
                    }
                }
				else if(subXmlNode.NodeType == XmlNodeType.Text || subXmlNode.NodeType == XmlNodeType.CDATA)
				{
					if(node.text == -1)
					{
                        int stringIndex = MatchStringIndex(subXmlNode.Value);
                        if (stringIndex == -1)
                        {
                            stringPool.Add(subXmlNode.Value);
                            node.text = stringPool.Count - 1;
                        }
                        else
                        {
                            node.text = stringIndex;
                        }
					}
					else
					{
                        //LogSystem.LogError("Ignore yyy of <nodeA>xxx<nodeB/>yyy<nodeA/>");
                        //LogSystem.LogError("Ignore:" + subXmlNode.InnerText);
					}
				}
			}
            return true;
		}

        private int MatchStringIndex(string str)
        {
            if (str == null)
            {
                str = "null";
            }

            int numStrings = stringPool.Count;
            for (int i = 0; i < numStrings; ++i)
            {
                if (stringPool[i] == str)
                {
                    return i;
                }
            }
            return -1;
        }
        private int MatchBoolIndex(bool value)
        {
            int numValues = boolPool.Count;
            for (int i = 0; i < numValues; ++i)
            {
                if (boolPool[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }
        private int MatchIntIndex(int value)
        {
            int numValues = intPool.Count;
            for (int i = 0; i < numValues; ++i)
            {
                if (intPool[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }
        private int MatchLongIndex(long value)
        {
            int numValues = longPool.Count;
            for (int i = 0; i < numValues; ++i)
            {
                if (longPool[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }
        private int MatchFloatIndex(float value)
        {
            int numValues = floatPool.Count;
            for (int i = 0; i < numValues; ++i)
            {
                if (floatPool[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }
        private int MatchDoubleIndex(double value)
        {
            int numValues = doublePool.Count;
            for (int i = 0; i < numValues; ++i)
            {
                if (doublePool[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }

        private TbXmlNodeTemplate GetNodeTemplate(ushort templateId)
        {
            int iCount = nodeTemplates.Count;
            for (int k = 0; k < iCount; k++)
            {
                TbXmlNodeTemplate nodeTemplate = nodeTemplates[k];
                if (nodeTemplate.id == templateId)
                {
                    return nodeTemplate;
                }
            }
            return null;
        }

        private TbXmlNodeTemplate GetNodeTemplate(XmlNode xmlNode)
        {
            int iCount = nodeTemplates.Count;
            for (int k = 0; k < iCount; k++)
            {
                TbXmlNodeTemplate nodeTemplate = nodeTemplates[k];
                if (XmlNodeMatchTemplate(xmlNode, nodeTemplate))
                {
                    return nodeTemplate;
                }
            }
            return null;
        }

        private bool XmlNodeMatchTemplate(XmlNode xmlNode, TbXmlNodeTemplate nodeTemplate)
		{
			if(nodeTemplate.name != xmlNode.Name)
			{
				return false;
			}

            if (bNewXML)
            {
                if (xmlNode.Attributes.Count > nodeTemplate.attributeNames.Count)
                {
                    return false;
                }
            }
            else
            {
                if (xmlNode.Attributes.Count != nodeTemplate.attributeNames.Count)
                {
                    return false;
                }
            }

            XmlAttributeCollection xmlAttributes = xmlNode.Attributes;
            int numAttributes = xmlAttributes == null ? 0 : xmlAttributes.Count;
            if (bNewXML)
            {
                for (int i = 0; i < numAttributes; ++i)
                {
                    XmlAttribute xmlAttribute = xmlAttributes[i];
                    if (nodeTemplate.attributeNames != null && !nodeTemplate.attributeNames.Contains(xmlAttribute.Name))
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < numAttributes; ++i)
                {
                    XmlAttribute xmlAttribute = xmlAttributes[i];
                    if (nodeTemplate.attributeNames != null && !nodeTemplate.attributeNames[i].Equals(xmlAttribute.Name))
                    {
                        return false;
                    }
                    double attributeDouble;
                    bool isAttributeDouble = DoubleTryParse(xmlAttribute.Value, out attributeDouble);
                    if ((isAttributeDouble && nodeTemplate.attributeTypes[i] != TB_XML_ATTRIBUTE_TYPE.VTYPE_DOUBLE) ||
                        (!isAttributeDouble && nodeTemplate.attributeTypes[i] == TB_XML_ATTRIBUTE_TYPE.VTYPE_DOUBLE))
                    {
                        return false;
                    }

                }
            }

			return true;
		}

		private void Serialize(BinaryWriter binaryWriter)
		{
            binaryWriter.Write(bNewXML);
            binaryWriter.Write((ushort)nodeTemplates.Count);
            int iCount = nodeTemplates.Count;
            for (int k = 0; k < iCount; k++)
            {
                TbXmlNodeTemplate nodeTemplate = nodeTemplates[k];
                SerializeNodeTemplate(binaryWriter, nodeTemplate);
            }

            binaryWriter.Write((ushort)(nodes.Count));

            for (int i = 0; i < nodes.Count; i++)
            {
                SerializeNode(binaryWriter, nodes[i]);
            }
            SerializeBoolPool(binaryWriter);
            SerializeIntPool(binaryWriter);
            SerializeLongPool(binaryWriter);
            SerializeFloatPool(binaryWriter);
            SerializeDoublePool(binaryWriter);
            SerializeStirngPool(binaryWriter);
        }

        private void SerializeStirngPool(BinaryWriter binaryWriter)
        {
            int numStrings = stringPool.Count;

            binaryWriter.Write(numStrings);

            for (int i = 0; i < numStrings; ++i)
            {
                binaryWriter.Write(stringPool[i]);
            }
        }

        private void SerializeDoublePool(BinaryWriter binaryWriter)
        {
            int numValues = doublePool.Count;

            binaryWriter.Write(numValues);

            for (int i = 0; i < numValues; ++i)
            {
                binaryWriter.Write(doublePool[i]);
            }
        }
        private void SerializeBoolPool(BinaryWriter binaryWriter)
        {
            int numValues = boolPool.Count;

            binaryWriter.Write(numValues);

            for (int i = 0; i < numValues; ++i)
            {
                binaryWriter.Write(boolPool[i]);
            }
        }
        private void SerializeIntPool(BinaryWriter binaryWriter)
        {
            int numValues = intPool.Count;

            binaryWriter.Write(numValues);

            for (int i = 0; i < numValues; ++i)
            {
                binaryWriter.Write(intPool[i]);
            }
        }
        private void SerializeLongPool(BinaryWriter binaryWriter)
        {
            int numValues = longPool.Count;

            binaryWriter.Write(numValues);

            for (int i = 0; i < numValues; ++i)
            {
                binaryWriter.Write(longPool[i]);
            }
        }
        private void SerializeFloatPool(BinaryWriter binaryWriter)
        {
            int numValues = floatPool.Count;

            binaryWriter.Write(numValues);

            for (int i = 0; i < numValues; ++i)
            {
                binaryWriter.Write(floatPool[i]);
            }
        }
        private void SerializeNodeTemplate(BinaryWriter binaryWriter, TbXmlNodeTemplate nodeTemplate)
		{
			binaryWriter.Write(nodeTemplate.id);

			binaryWriter.Write(nodeTemplate.name);

			binaryWriter.Write((ushort)nodeTemplate.attributeNames.Count);

            int iCount = nodeTemplate.attributeNames.Count;
            for (int i = 0; i < iCount;i++)
            {
                string attributeName = nodeTemplate.attributeNames[i];
                binaryWriter.Write(attributeName);
            }

            int iCount2 = nodeTemplate.attributeTypes.Count;
            for (int i = 0; i < iCount2; i++)
            {
                byte attributeType = (byte) nodeTemplate.attributeTypes[i];
                binaryWriter.Write(attributeType);
			}
		}

		private void SerializeNode(BinaryWriter binaryWriter, TbXmlNode node)
		{
			//TbXmlNodeTemplate nodeTemplate = GetNodeTemplate(node.templateId);

			binaryWriter.Write(node.id);

			binaryWriter.Write(node.templateId);

			binaryWriter.Write((ushort)node.childrenIds.Count);
            int iCount = node.childrenIds.Count;
            for ( int i = 0;i < iCount;i++)
            {
                ushort childId = node.childrenIds[i];
                binaryWriter.Write(childId);
            }
		
			int attributeIndex = 0;
            binaryWriter.Write((ushort)node.attributeValues.Count);
            foreach (var item in node.attributeValues)
            {
                binaryWriter.Write(item.Key);
                binaryWriter.Write(item.Value);
                ++attributeIndex;
            }

			if(node.text == -1)
			{
				binaryWriter.Write((byte)0);
			}
			else
			{
				binaryWriter.Write((byte)1);
				binaryWriter.Write(node.text);
			}
		}

        private bool DoubleTryParse(string input, out double value)
        {
            value = 0;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (input.Contains(",") || input.Contains("$") || (input.StartsWith("0") && !input.Equals("0") && !input.Contains(".")))
            {
                return false;
            }
            return double.TryParse(input, out value);
        }
        private bool IntTryParse(string input, out int value)
        {
            value = 0;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (input.Contains(",") || input.Contains("$") || (input.StartsWith("0") && !input.Equals("0")))
            {
                return false;
            }
            return int.TryParse(input, out value);
        }
        private bool BoolTryParse(string input, out bool value)
        {
            value = false;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (input.Contains(",") || input.Contains("$") || (input.StartsWith("0") && !input.Equals("0")))
            {
                return false;
            }
            if (bool.TryParse(input, out value))
            {
                return value;
            }
            int ivalue = 0;
            if (int.TryParse(input, out ivalue))
            {
                if (ivalue == 0)
                {
                    value = false;
                }
                else if (ivalue == 1)
                {
                    value = true;
                }
            }
            return value;
        }
        private bool LongTryParse(string input, out long value)
        {
            value = 0;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (input.Contains(",") || input.Contains("$") || (input.StartsWith("0") && !input.Equals("0")))
            {
                return false;
            }
            return long.TryParse(input, out value);
        }
        private bool FloatTryParse(string input, out float value)
        {
            value = 0.0f;
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            if (input.Contains(",") || input.Contains("$") || (input.StartsWith("0") && !input.Equals("0")&& !input.Contains(".")))
            {
                return false;
            }
            return float.TryParse(input, out value);
        }
    }
}
#endif

