namespace MATRIX_ENCRYPT
{
	public class MxCrypt
	{
		public static void MxApp_Encrypt(uint[] Data, uint[] Key)
		{
			uint num = 0u;
			uint num2 = 2654435769u;
			uint num3 = 32u;
			while (num3-- != 0)
			{
				Data[0] += (((Data[1] << 4) ^ (Data[1] >> 5)) + Data[1]) ^ (num + Key[num & 3]);
				num += num2;
				Data[1] += (((Data[0] << 4) ^ (Data[0] >> 5)) + Data[0]) ^ (num + Key[(num >> 11) & 3]);
			}
		}

		public static void MxApp_Decrypt(uint[] Data, uint[] Key)
		{
			uint num = 3337565984u;
			uint num2 = 2654435769u;
			uint num3 = 32u;
			while (num3-- != 0)
			{
				Data[1] -= (((Data[0] << 4) ^ (Data[0] >> 5)) + Data[0]) ^ (num + Key[(num >> 11) & 3]);
				num -= num2;
				Data[0] -= (((Data[1] << 4) ^ (Data[1] >> 5)) + Data[1]) ^ (num + Key[num & 3]);
			}
		}
	}
}
