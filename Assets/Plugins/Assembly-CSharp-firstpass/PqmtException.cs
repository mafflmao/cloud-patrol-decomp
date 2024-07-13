using System;
using PQ_SDK_MultiTouch;

public class PqmtException : Exception
{
	public PQMTClientImport.EnumPQErrorType ErrorCode { get; protected set; }

	public override string Message
	{
		get
		{
			if (ErrorCode != 0)
			{
				return string.Format("{0}/nPQMT Error {1:X}: {2}", base.Message, (int)ErrorCode, ErrorCode.ToString());
			}
			return base.Message;
		}
	}

	public PqmtException()
	{
	}

	public PqmtException(string aMessage)
		: base(aMessage)
	{
	}

	public PqmtException(string aMessage, PQMTClientImport.EnumPQErrorType aErrorCode)
		: base(aMessage)
	{
		ErrorCode = aErrorCode;
	}
}
