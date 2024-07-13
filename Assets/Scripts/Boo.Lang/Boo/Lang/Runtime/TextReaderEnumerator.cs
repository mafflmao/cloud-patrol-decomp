using System.Collections.Generic;
using System.IO;

namespace Boo.Lang.Runtime
{
	public class TextReaderEnumerator
	{
		public static IEnumerable<string> lines(TextReader reader)
		{
			using (reader)
			{
				while (true)
				{
					string text;
					string line = (text = reader.ReadLine());
					if (text != null)
					{
						yield return line;
						continue;
					}
					break;
				}
			}
		}
	}
}
