using System.IO;

namespace XmlTool
{
	public class XmlParser
	{
		private XmlNode mRootNode;

		public XmlParser(string iFilePath)
		{
			StreamReader streamReader = new StreamReader(iFilePath);
			string oXmlData = streamReader.ReadToEnd();
			mRootNode = new XmlNode(oXmlData, out oXmlData);
			streamReader.Close();
		}

		public XmlParser()
		{
			mRootNode = null;
		}

		public XmlNode Parse(string iXmlData)
		{
			mRootNode = new XmlNode(iXmlData, out iXmlData);
			return mRootNode;
		}

		public XmlNode GetRoot()
		{
			return mRootNode;
		}

		public XmlNode GetNode(string iPath)
		{
			int num = iPath.IndexOf('/');
			string text;
			string text2;
			if (num == -1)
			{
				text = string.Empty;
				text2 = iPath;
			}
			else
			{
				text = iPath.Substring(num + 1);
				text2 = iPath.Substring(0, num);
			}
			if (mRootNode.GetName() == text2)
			{
				if (text != string.Empty)
				{
					return mRootNode.GetNode(text);
				}
				return mRootNode;
			}
			return null;
		}
	}
}
