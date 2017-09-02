using System.Collections.Generic;

namespace TinyBinaryXml
{
	public class TbXml
	{
        public List<TbXmlNodeTemplate> nodeTemplates = null;

        public List<TbXmlNode> nodes = null;

        public List<string> stringPool = null;

        public List<bool> boolPool = null;

        public List<int> intPool = null;

        public List<long> longPool = null;

        public List<float> floatPool = null;

        public List<double> doublePool = null;

		public TbXmlNode docNode = null;

        public bool bNewXML = false;

		public static TbXml Load(byte[] xmlBytes)
		{
			TbXmlDeserializer deserializer = new TbXmlDeserializer();
			return deserializer.DeserializeXmlBytes(xmlBytes);
		}
	}
}

