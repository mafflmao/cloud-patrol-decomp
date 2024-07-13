using System;
using System.Text;

namespace Boo.Lang
{
	public class QuackFuMember : IQuackFuMember
	{
		protected string name;

		protected QuackFuMemberKind kind;

		protected Type returnType;

		protected string[] argumentNames;

		protected Type[] argumentTypes;

		protected string info;

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentNullException("value");
				}
				name = value;
			}
		}

		public QuackFuMemberKind Kind
		{
			get
			{
				return kind;
			}
			set
			{
				kind = value;
			}
		}

		public Type ReturnType
		{
			get
			{
				return returnType;
			}
			set
			{
				returnType = value;
			}
		}

		public string[] ArgumentNames
		{
			get
			{
				return argumentNames;
			}
			set
			{
				argumentNames = value;
			}
		}

		public Type[] ArgumentTypes
		{
			get
			{
				return argumentTypes;
			}
			set
			{
				argumentTypes = value;
			}
		}

		public string Info
		{
			get
			{
				return info;
			}
			set
			{
				info = value;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(Name);
			if (Kind == QuackFuMemberKind.Method)
			{
				stringBuilder.Append("(");
				if (ArgumentNames != null)
				{
					for (int i = 0; i < ArgumentNames.Length; i++)
					{
						string value = ArgumentNames[i];
						if (i > 0)
						{
							stringBuilder.Append(", ");
						}
						stringBuilder.Append(value);
						if (ArgumentTypes != null && ArgumentTypes.Length > i)
						{
							Type type = ArgumentTypes[i];
							AppendTypeInformation(stringBuilder, type);
						}
					}
				}
				stringBuilder.Append(")");
			}
			AppendTypeInformation(stringBuilder, ReturnType);
			return stringBuilder.ToString();
		}

		private static void AppendTypeInformation(StringBuilder sb, Type type)
		{
			if (type != null)
			{
				sb.Append(" as ");
				sb.Append(GetBooTypeName(type));
			}
		}

		private static string GetBooTypeName(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (type == typeof(int))
			{
				return "int";
			}
			if (type == typeof(string))
			{
				return "string";
			}
			return type.FullName;
		}
	}
}
