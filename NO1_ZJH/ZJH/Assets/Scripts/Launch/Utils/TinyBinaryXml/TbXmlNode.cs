using System;
using System.Collections.Generic;

namespace TinyBinaryXml
{
	public class TbXmlNode
	{
        public ushort id = 0;

        public List<ushort> childrenIds = null;

        public ushort templateId = 0;

        public Dictionary<string,int> attributeValues = null;

        public TbXml tbXml = null;

        public int text = -1;

        public string GetText()
        {
            if (text == -1)
            {
                return string.Empty;
            }
            else
            {
                return tbXml.stringPool[text];
            }
        }

        public object GetValue(string name)
        {
            TbXmlNodeTemplate nodeTemplate = tbXml.nodeTemplates[templateId];
            int attributeIndex;
            if (nodeTemplate.attributeNameIndexMapping.TryGetValue(name, out attributeIndex))
            {
                if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_DOUBLE)
                {
                    if (!attributeValues.ContainsKey(name) || attributeValues[name] >= tbXml.doublePool.Count)
                    {
                        return null;
                    }
                    return tbXml.doublePool[attributeValues[name]];
                }
                else
                {
                    if (!attributeValues.ContainsKey(name) || attributeValues[name] >= tbXml.stringPool.Count)
                    {
                        return null;
                    }
                    return tbXml.stringPool[attributeValues[name]];
                }
            }
            else
            {
                return null;
            }
        }


        public string GetStringValue(string name)
		{
            if (tbXml.bNewXML)
            {
                if (tbXml.nodeTemplates.Count > templateId)
                {
                    TbXmlNodeTemplate nodeTemplate = tbXml.nodeTemplates[templateId];
                    int attributeIndex;
                    if (nodeTemplate.attributeNameIndexMapping.TryGetValue(name, out attributeIndex))
                    {
                        if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_UNKNOWN)
                        {
                            object value = GetValue(name);
                            if( value != null )
                            {
                                if (value is double)
                                {
                                    return value.ToString();
                                }
                                else
                                {
                                    return value as string;
                                }
                            }
                            return string.Empty;
                            
                        }
                        else if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_STRING && attributeValues.ContainsKey(name) && attributeValues[name] < tbXml.stringPool.Count)
                        {
                            return tbXml.stringPool[attributeValues[name]];
                        }
                    }
                }
                return string.Empty;
            }
            else
            {
                object value = GetValue(name);
                if (value != null)
                {
                    if (value is double)
                    {
                        return value.ToString();
                    }
                    else
                    {
                        return value as string;
                    }
                }
                return string.Empty;
            }

        }

        public double GetDoubleValue(string name, double dDefualt = 0.0f)
        {
            if (tbXml.bNewXML)
            {
                if (tbXml.nodeTemplates.Count > templateId)
                {
                    TbXmlNodeTemplate nodeTemplate = tbXml.nodeTemplates[templateId];
                    int attributeIndex;
                    if (nodeTemplate.attributeNameIndexMapping.TryGetValue(name, out attributeIndex))
                    {
                        if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_UNKNOWN)
                        {
                            object value = GetValue(name);
                            if( value != null )
                            {
                                if (value is double)
                                {
                                    return (double)value;
                                }
                                else if (value is string)
                                {
                                    double fValue = dDefualt;
                                    string strValue = value as string;
                                    if (double.TryParse(strValue, out fValue))
                                    {
                                        return fValue;
                                    }
                                }
                            }

                            return dDefualt;
                        }
                        else if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_DOUBLE && attributeValues.ContainsKey(name) && attributeValues[name] < tbXml.doublePool.Count)
                        {
                            return tbXml.doublePool[attributeValues[name]];
                        }
                    }
                }

                return 0.0f;
            }
            else
            {
                object value = GetValue(name);
                if (value != null)
                {
                    if (value is double)
                    {
                        return (double)value;
                    }
                    else if (value is string)
                    {
                        double fValue = dDefualt;
                        string strValue = value as string;
                        if (double.TryParse(strValue, out fValue))
                        {
                            return fValue;
                        }
                    }
                }

                return dDefualt;
            }

        }

		public float GetFloatValue(string name, float iDefualt = 0.0f)
		{
            if (tbXml.bNewXML)
            {
                if (tbXml.nodeTemplates.Count > templateId)
                {
                    TbXmlNodeTemplate nodeTemplate = tbXml.nodeTemplates[templateId];
                    int attributeIndex;
                    if (nodeTemplate.attributeNameIndexMapping.TryGetValue(name, out attributeIndex))
                    {
                        if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_UNKNOWN)
                        {
                            object value = GetValue(name);
                            if( value != null )
                            {
                                if( value is double )
                                {
                                    return (float)(double)value;
                                }
                                else if( value is string)
                                {
                                    string strValue = value as string;
                                    float result = iDefualt;
                                    if (float.TryParse(strValue, out result))
                                    {
                                        return result;
                                    }
                                    return iDefualt;
                                }
                            }
                            return iDefualt;
                        }
                        else if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_FLOAT && attributeValues.ContainsKey(name) && attributeValues[name] < tbXml.floatPool.Count)
                        {
                            return tbXml.floatPool[attributeValues[name]];
                        }
                    }
                }

                return 0;
            }
            else
            {
                /*  object value = GetValue(name);
                  if (value is double)
                  {
                      return (float)(double)value;
                  }
                  else if (value is string)
                  {
                      float fValue = 0.0f;
                      string strValue = value as string;
                      if (float.TryParse(strValue, out fValue))
                      {
                          return fValue;
                      }
                  }

                  return 0.0f;*/
                object value = GetValue(name);
                if (value != null)
                {
                    if (value is double)
                    {
                        return (float)(double)value;
                    }
                    else if (value is string)
                    {
                        string strValue = value as string;
                        float result = iDefualt;
                        if (float.TryParse(strValue, out result))
                        {
                            return result;
                        }
                        return iDefualt;
                    }
                }
                return iDefualt;
            }
        }

		public int GetIntValue(string name,int iDefualt = 0 )
		{
            if (tbXml.bNewXML)
            {
                if (tbXml.nodeTemplates.Count > templateId)
                {
                    TbXmlNodeTemplate nodeTemplate = tbXml.nodeTemplates[templateId];
                    int attributeIndex;
                    if (nodeTemplate.attributeNameIndexMapping.TryGetValue(name, out attributeIndex))
                    {
                        if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_UNKNOWN)
                        {
                            object oValue = GetValue(name);
                            if( oValue != null )
                            {
                                if (oValue is double)
                                {
                                    int result = (int)(double)oValue;
                                    return result;
                                }
                                else if ( oValue is string)
                                {
                                    string strValue = oValue as string;
                                    int result = iDefualt;
                                    if (int.TryParse(strValue, out result))
                                    {
                                        return result;
                                    }
                                    return iDefualt;
                                }
                            }
                            return iDefualt;
                        }
                        else if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_INT && attributeValues.ContainsKey(name) && attributeValues[name] < tbXml.intPool.Count)
                        {
                            return tbXml.intPool[attributeValues[name]];
                        }
                    }
                }

                return iDefualt;
            }
            else
            {
                object oValue = GetValue(name);
                if (oValue != null)
                {
                    if (oValue is double)
                    {
                        int result = (int)(double)oValue;
                        return result;
                    }
                    else if (oValue is string)
                    {
                        string strValue = oValue as string;
                        int result = iDefualt;
                        if (int.TryParse(strValue, out result))
                        {
                            return result;
                        }
                        return iDefualt;
                    }
                }
                return iDefualt;
            }
        }
        public long GetLongValue(string name, long lDefualt = 0)
        {
            if (tbXml.bNewXML)
            {
                if (tbXml.nodeTemplates.Count > templateId)
                {
                    TbXmlNodeTemplate nodeTemplate = tbXml.nodeTemplates[templateId];
                    int attributeIndex;
                    if (nodeTemplate.attributeNameIndexMapping.TryGetValue(name, out attributeIndex))
                    {
                        if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_UNKNOWN)
                        {
                            object value = GetValue(name);
                            if( value != null )
                            {
                                if (value is double)
                                {
                                    return (long)(double)value;
                                }
                                else if (value is string)
                                {
                                    long iValue = lDefualt;
                                    string strValue = value as string;
                                    if (long.TryParse(strValue, out iValue))
                                    {
                                        return iValue;
                                    }
                                }
                            }
                           
                            return lDefualt;
                        }
                        else if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_INT64 && attributeValues.ContainsKey(name) && attributeValues[name] < tbXml.longPool.Count)
                        {
                            return tbXml.longPool[attributeValues[name]];
                        }
                    }
                }

                return lDefualt;
            }
            else
            {
                object value = GetValue(name);
                if (value != null)
                {
                    if (value is double)
                    {
                        return (long)(double)value;
                    }
                    else if (value is string)
                    {
                        long iValue = lDefualt;
                        string strValue = value as string;
                        if (long.TryParse(strValue, out iValue))
                        {
                            return iValue;
                        }
                    }
                }

                return lDefualt;
            }
 
        }
        public bool GetBooleanValue(string name, bool bDefualt = false)
        {
            if (tbXml.bNewXML)
            {
                if (tbXml.nodeTemplates.Count > templateId)
                {
                    TbXmlNodeTemplate nodeTemplate = tbXml.nodeTemplates[templateId];
                    int attributeIndex;
                    if (nodeTemplate.attributeNameIndexMapping.TryGetValue(name, out attributeIndex))
                    {
                        if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_UNKNOWN)
                        {
                            object value = GetValue(name);
                            if (value != null)
                            {
                                if (value is double)
                                {
                                    int iValue = (int)(double)value;
                                    return iValue == 1;
                                }
                                else if (value is string)
                                {
                                    string strValue = value as string;
                                    Boolean result;
                                    int iResult;
                                    if (Boolean.TryParse(strValue, out result))
                                    {
                                        return result;
                                    }
                                    else if (int.TryParse(strValue, out iResult))
                                    {
                                        return iResult == 1;
                                    }
                                    return bDefualt;
                                }
                            }

                            return bDefualt;
                            
                        }
                        else if (nodeTemplate.attributeTypes[attributeIndex] == TB_XML_ATTRIBUTE_TYPE.VTYPE_BOOL && attributeValues.ContainsKey(name) && attributeValues[name] < tbXml.boolPool.Count)
                        {
                            return tbXml.boolPool[attributeValues[name]];
                        }
                    }
                }
                return bDefualt;
            }
            else
            {
                object value = GetValue(name);
                if (value != null)
                {
                    if (value is double)
                    {
                        int iValue = (int)(double)value;
                        return iValue == 1;
                    }
                    else if (value is string)
                    {
                        string strValue = value as string;
                        Boolean result;
                        int iResult;
                        if (Boolean.TryParse(strValue, out result))
                        {
                            return result;
                        }
                        else if (int.TryParse(strValue, out iResult))
                        {
                            return iResult == 1;
                        }
                        return bDefualt;
                    }
                }

                return bDefualt;
            }
        }

		public List<TbXmlNode> GetNodes(string path)
		{
			if(string.IsNullOrEmpty(path))
			{
				return null;
			}

			List<TbXmlNode> resultNodes = null;
			int numChildren = childrenIds == null ? 0 : childrenIds.Count;
			string[] pathBlocks = path.Split('/');
			for(int childIndex = 0; childIndex < numChildren; ++childIndex)
			{
				TbXmlNode childNode = tbXml.nodes[childrenIds[childIndex]];
				GetNodesRecursive(pathBlocks, 0, ref pathBlocks[0], childNode, ref resultNodes);
			}
			
			return resultNodes;
		}

		private void GetNodesRecursive(string[] pathBlocks, int pathBlockIndex, ref string pathBlock, TbXmlNode currentNode, ref List<TbXmlNode> resultNodes)
		{
            if (currentNode.templateId >= tbXml.nodeTemplates.Count)
                return;
			if(tbXml.nodeTemplates[currentNode.templateId].name.Equals(pathBlock))
			{
				if(pathBlockIndex == pathBlocks.Length - 1)
				{
					if(resultNodes == null)
					{
						resultNodes = new List<TbXmlNode>();
					}
					resultNodes.Add(currentNode);
				}
				else
				{
					List<ushort> childrenIds = currentNode.childrenIds;
					int numChildren = childrenIds == null ? 0 : childrenIds.Count;
					for(int childIndex = 0; childIndex < numChildren; ++childIndex)
					{
						GetNodesRecursive(pathBlocks, pathBlockIndex + 1, ref pathBlocks[pathBlockIndex + 1], tbXml.nodes[childrenIds[childIndex]], ref resultNodes);
					}
				}
			}
		}
	}
}

