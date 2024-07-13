using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

public static class DataTransform
{
	private static byte[] KEY = new byte[32]
	{
		25, 117, 135, 65, 2, 201, 206, 191, 243, 229,
		193, 34, 16, 225, 48, 202, 210, 198, 81, 228,
		142, 1, 112, 109, 30, 127, 249, 190, 57, 249,
		196, 152
	};

	private static byte[] IV = new byte[16]
	{
		19, 234, 116, 90, 15, 125, 88, 236, 165, 165,
		161, 2, 100, 159, 151, 197
	};

	private static readonly int NB_BLOCKS = 1024;

	public static Stream ToBase64(Stream i_InputStrm, Stream i_OutputStrm)
	{
		i_InputStrm.Seek(0L, SeekOrigin.Begin);
		i_OutputStrm.Seek(0L, SeekOrigin.Begin);
		byte[] array = new byte[3 * NB_BLOCKS];
		char[] array2 = new char[4 * NB_BLOCKS];
		StreamWriter streamWriter = new StreamWriter(i_OutputStrm);
		streamWriter.AutoFlush = false;
		int length;
		while ((length = i_InputStrm.Read(array, 0, array.Length)) != 0)
		{
			int count = Convert.ToBase64CharArray(array, 0, length, array2, 0);
			streamWriter.Write(array2, 0, count);
		}
		streamWriter.Flush();
		i_InputStrm.Seek(0L, SeekOrigin.Begin);
		i_OutputStrm.Flush();
		i_OutputStrm.Seek(0L, SeekOrigin.Begin);
		return i_OutputStrm;
	}

	public static Stream GZip(Stream i_InputStrm, Stream i_OutputStrm)
	{
		i_InputStrm.Seek(0L, SeekOrigin.Begin);
		i_OutputStrm.Seek(0L, SeekOrigin.Begin);
		using (GZipStream gZipStream = new GZipStream(i_OutputStrm, CompressionMode.Compress, true))
		{
			byte[] array = new byte[4096];
			int count;
			while ((count = i_InputStrm.Read(array, 0, array.Length)) != 0)
			{
				gZipStream.Write(array, 0, count);
			}
		}
		i_InputStrm.Seek(0L, SeekOrigin.Begin);
		i_OutputStrm.Seek(0L, SeekOrigin.Begin);
		return i_OutputStrm;
	}

	public static Stream Encrypt(Stream i_InputStrm, Stream i_OutputStrm)
	{
		byte[] array = EncryptStreamToBytes_Aes(i_InputStrm, KEY, IV);
		i_OutputStrm.Write(array, 0, array.Length);
		i_InputStrm.Seek(0L, SeekOrigin.Begin);
		i_OutputStrm.Seek(0L, SeekOrigin.Begin);
		return i_OutputStrm;
	}

	public static Stream Decrypt(Stream i_InputStrm, Stream i_OutputStrm)
	{
		byte[] array = DecryptStreamFromBytes_Aes(i_InputStrm, KEY, IV);
		i_OutputStrm.Write(array, 0, array.Length);
		i_InputStrm.Seek(0L, SeekOrigin.Begin);
		i_OutputStrm.Seek(0L, SeekOrigin.Begin);
		return i_OutputStrm;
	}

	public static Stream GUnzip(Stream i_InputStrm, Stream i_OutputStrm)
	{
		i_InputStrm.Seek(0L, SeekOrigin.Begin);
		i_OutputStrm.Seek(0L, SeekOrigin.Begin);
		using (GZipStream gZipStream = new GZipStream(i_InputStrm, CompressionMode.Decompress, true))
		{
			byte[] array = new byte[4096];
			int count;
			while ((count = gZipStream.Read(array, 0, array.Length)) != 0)
			{
				i_OutputStrm.Write(array, 0, count);
			}
		}
		i_InputStrm.Seek(0L, SeekOrigin.Begin);
		i_OutputStrm.Seek(0L, SeekOrigin.Begin);
		return i_OutputStrm;
	}

	public static Stream FromBase64(Stream i_InputStrm, Stream i_OutputStrm)
	{
		i_InputStrm.Seek(0L, SeekOrigin.Begin);
		i_OutputStrm.Seek(0L, SeekOrigin.Begin);
		char[] array = new char[4 * NB_BLOCKS];
		StreamReader streamReader = new StreamReader(i_InputStrm);
		int length;
		while ((length = streamReader.Read(array, 0, array.Length)) != 0)
		{
			byte[] array2 = Convert.FromBase64CharArray(array, 0, length);
			i_OutputStrm.Write(array2, 0, array2.Length);
		}
		i_InputStrm.Seek(0L, SeekOrigin.Begin);
		i_OutputStrm.Flush();
		i_OutputStrm.Seek(0L, SeekOrigin.Begin);
		return i_OutputStrm;
	}

	private static byte[] EncryptStreamToBytes_Aes(Stream i_PlainStrm, byte[] Key, byte[] IV)
	{
		if (i_PlainStrm == null)
		{
			throw new ArgumentNullException("i_PlainStrm");
		}
		if (Key == null || Key.Length <= 0)
		{
			throw new ArgumentNullException("Key");
		}
		if (IV == null || IV.Length <= 0)
		{
			throw new ArgumentNullException("IV");
		}
		if (i_PlainStrm.Length <= 0)
		{
			return new byte[0];
		}
		byte[] array = null;
		using (AesManaged aesManaged = new AesManaged())
		{
			aesManaged.Key = Key;
			aesManaged.IV = IV;
			aesManaged.Padding = PaddingMode.PKCS7;
			ICryptoTransform transform = aesManaged.CreateEncryptor(aesManaged.Key, aesManaged.IV);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
				{
					byte[] array2 = new byte[4096];
					i_PlainStrm.Seek(0L, SeekOrigin.Begin);
					int count;
					while ((count = i_PlainStrm.Read(array2, 0, array2.Length)) != 0)
					{
						cryptoStream.Write(array2, 0, count);
					}
					cryptoStream.FlushFinalBlock();
					cryptoStream.Close();
					return memoryStream.ToArray();
				}
			}
		}
	}

	private static byte[] DecryptStreamFromBytes_Aes(Stream i_CipherStrm, byte[] Key, byte[] IV)
	{
		if (i_CipherStrm == null)
		{
			throw new ArgumentNullException("i_CipherStrm");
		}
		if (Key == null || Key.Length <= 0)
		{
			throw new ArgumentNullException("Key");
		}
		if (IV == null || IV.Length <= 0)
		{
			throw new ArgumentNullException("IV");
		}
		if (i_CipherStrm.Length <= 0)
		{
			return new byte[0];
		}
		byte[] array = null;
		using (AesManaged aesManaged = new AesManaged())
		{
			aesManaged.Key = Key;
			aesManaged.IV = IV;
			aesManaged.Padding = PaddingMode.PKCS7;
			ICryptoTransform transform = aesManaged.CreateDecryptor(aesManaged.Key, aesManaged.IV);
			i_CipherStrm.Seek(0L, SeekOrigin.Begin);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
				{
					byte[] array2 = new byte[4096];
					int count;
					while ((count = i_CipherStrm.Read(array2, 0, array2.Length)) != 0)
					{
						cryptoStream.Write(array2, 0, count);
					}
					cryptoStream.FlushFinalBlock();
					cryptoStream.Close();
					return memoryStream.ToArray();
				}
			}
		}
	}
}
