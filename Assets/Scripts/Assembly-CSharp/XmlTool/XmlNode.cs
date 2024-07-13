using System.Collections.Generic;

namespace XmlTool
{
	public class XmlNode
	{
		private string mName;

		private string mElement;

		private Dictionary<string, string> mAttributes;

		private List<XmlNode> mChilds;

		public XmlNode(string iXmlData, out string oXmlData)
		{
			int num = iXmlData.IndexOf('<');
			int num2 = num + 1;
			char[] anyOf = new char[3] { ' ', '/', '>' };
			int num3 = iXmlData.IndexOfAny(anyOf, num);
			mName = iXmlData.Substring(num2, num3 - num2);
			mElement = string.Empty;
			mAttributes = new Dictionary<string, string>();
			mChilds = new List<XmlNode>();
			char c = iXmlData.ToCharArray()[num3];
			iXmlData = iXmlData.Substring(num3 + 1);
			if (c == ' ')
			{
				iXmlData = iXmlData.TrimStart();
				c = iXmlData.ToCharArray()[0];
				if (c != '>' && c != '/')
				{
					iXmlData = ParseAttributes(iXmlData);
					c = iXmlData.ToCharArray()[0];
				}
			}
			if (c == '>')
			{
				iXmlData = ParseChilds(iXmlData);
			}
			int num4 = iXmlData.IndexOf('>');
			oXmlData = iXmlData.Substring(num4 + 1);
		}

		private string ParseAttributes(string iXmlData)
		{
			do
			{
				int length = iXmlData.IndexOf('=');
				string key = iXmlData.Substring(0, length);
				int num = iXmlData.IndexOf('"');
				iXmlData = iXmlData.Substring(num + 1);
				int num2 = iXmlData.IndexOf('"');
				string value = ParseValue(iXmlData, num2);
				mAttributes.Add(key, value);
				iXmlData = iXmlData.Substring(num2 + 1);
				iXmlData = iXmlData.TrimStart();
			}
			while (iXmlData.ToCharArray()[0] != '>' && iXmlData.ToCharArray()[0] != '/');
			return iXmlData;
		}

		private string ParseChilds(string iXmlData)
		{
			char[] trimChars = new char[5] { '>', '\r', '\n', ' ', '\t' };
			iXmlData = iXmlData.TrimStart(trimChars);
			if (iXmlData.ToCharArray()[0] == '<')
			{
				while (iXmlData.ToCharArray()[1] != '/')
				{
					XmlNode item = new XmlNode(iXmlData, out iXmlData);
					mChilds.Add(item);
					iXmlData = iXmlData.TrimStart(trimChars);
				}
			}
			else
			{
				int num = iXmlData.IndexOf('<');
				mElement = ParseValue(iXmlData, num);
				iXmlData = iXmlData.Substring(num);
			}
			return iXmlData;
		}

		private static string ParseValue(string iXmlData, int iSize)
		{
			string text = string.Empty;
			int num = 0;
			int num2;
			while ((num2 = iXmlData.IndexOf('&', num, iSize - num)) != -1)
			{
				text += iXmlData.Substring(num, num2 - num);
				num = num2 + 1;
				int num3 = iXmlData.IndexOf(';', num, 5);
				switch (iXmlData.Substring(num, num3 - num))
				{
				case "lt":
					text += "<";
					break;
				case "gt":
					text += ">";
					break;
				case "amp":
					text += "&";
					break;
				case "apos":
					text += "'";
					break;
				case "quot":
					text += "\"";
					break;
				}
				num = num3 + 1;
			}
			return text + iXmlData.Substring(num, iSize - num);
		}

		public string GetName()
		{
			return mName;
		}

		public string GetElement()
		{
			return mElement;
		}

		public int GetElementAsInt()
		{
			return int.Parse(mElement);
		}

		public float GetElementAsFloat()
		{
			return float.Parse(mElement);
		}

		public bool GetElementAsBool()
		{
			return bool.Parse(mElement);
		}

		public string GetAttribute(string iName)
		{
			string value;
			if (mAttributes.TryGetValue(iName, out value))
			{
				return value;
			}
			return string.Empty;
		}

		public int GetAttributeAsInt(string iName)
		{
			string value;
			if (mAttributes.TryGetValue(iName, out value))
			{
				return int.Parse(value);
			}
			return 0;
		}

		public float GetAttributeAsFloat(string iName)
		{
			string value;
			if (mAttributes.TryGetValue(iName, out value))
			{
				return float.Parse(value);
			}
			return 0f;
		}

		public bool GetAttributeAsBool(string iName)
		{
			string value;
			if (mAttributes.TryGetValue(iName, out value))
			{
				return bool.Parse(value);
			}
			return false;
		}

		public XmlNode GetChild(string iName)
		{
			return mChilds.Find((XmlNode lNode) => lNode.mName == iName);
		}

		public List<XmlNode> GetChildList()
		{
			return mChilds;
		}

		public XmlNode GetNode(string iPath)
		{
			int num = iPath.IndexOf('/');
			string text;
			string lName;
			if (num == -1)
			{
				text = string.Empty;
				lName = iPath;
			}
			else
			{
				text = iPath.Substring(num + 1);
				lName = iPath.Substring(0, num);
			}
			XmlNode xmlNode = mChilds.Find((XmlNode lNode) => lNode.mName == lName);
			if (text != string.Empty && xmlNode != null)
			{
				return xmlNode.GetNode(text);
			}
			return xmlNode;
		}
	}
}
