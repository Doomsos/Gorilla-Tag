using System;
using System.IO;
using System.Security.Cryptography;

namespace PublicKeyConvert
{
	// Token: 0x02000D06 RID: 3334
	public class PEMKeyLoader
	{
		// Token: 0x060050EC RID: 20716 RVA: 0x001A1C50 File Offset: 0x0019FE50
		private static bool CompareBytearrays(byte[] a, byte[] b)
		{
			if (a.Length != b.Length)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < a.Length; i++)
			{
				if (a[i] != b[num])
				{
					return false;
				}
				num++;
			}
			return true;
		}

		// Token: 0x060050ED RID: 20717 RVA: 0x001A1C88 File Offset: 0x0019FE88
		public static RSACryptoServiceProvider CryptoServiceProviderFromPublicKeyInfo(byte[] x509key)
		{
			new byte[15];
			if (x509key == null || x509key.Length == 0)
			{
				return null;
			}
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(x509key));
			RSACryptoServiceProvider result;
			try
			{
				ushort num = binaryReader.ReadUInt16();
				if (num == 33072)
				{
					binaryReader.ReadByte();
				}
				else
				{
					if (num != 33328)
					{
						return null;
					}
					binaryReader.ReadInt16();
				}
				if (!PEMKeyLoader.CompareBytearrays(binaryReader.ReadBytes(15), PEMKeyLoader.SeqOID))
				{
					result = null;
				}
				else
				{
					num = binaryReader.ReadUInt16();
					if (num == 33027)
					{
						binaryReader.ReadByte();
					}
					else
					{
						if (num != 33283)
						{
							return null;
						}
						binaryReader.ReadInt16();
					}
					if (binaryReader.ReadByte() != 0)
					{
						result = null;
					}
					else
					{
						num = binaryReader.ReadUInt16();
						if (num == 33072)
						{
							binaryReader.ReadByte();
						}
						else
						{
							if (num != 33328)
							{
								return null;
							}
							binaryReader.ReadInt16();
						}
						num = binaryReader.ReadUInt16();
						byte b = 0;
						byte b2;
						if (num == 33026)
						{
							b2 = binaryReader.ReadByte();
						}
						else
						{
							if (num != 33282)
							{
								return null;
							}
							b = binaryReader.ReadByte();
							b2 = binaryReader.ReadByte();
						}
						byte[] array = new byte[4];
						array[0] = b2;
						array[1] = b;
						int num2 = BitConverter.ToInt32(array, 0);
						if (binaryReader.PeekChar() == 0)
						{
							binaryReader.ReadByte();
							num2--;
						}
						byte[] modulus = binaryReader.ReadBytes(num2);
						if (binaryReader.ReadByte() != 2)
						{
							result = null;
						}
						else
						{
							int num3 = (int)binaryReader.ReadByte();
							byte[] exponent = binaryReader.ReadBytes(num3);
							RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider();
							RSAParameters rsaparameters = default(RSAParameters);
							rsaparameters.Modulus = modulus;
							rsaparameters.Exponent = exponent;
							rsacryptoServiceProvider.ImportParameters(rsaparameters);
							result = rsacryptoServiceProvider;
						}
					}
				}
			}
			finally
			{
				binaryReader.Close();
			}
			return result;
		}

		// Token: 0x060050EE RID: 20718 RVA: 0x001A1E54 File Offset: 0x001A0054
		public static RSACryptoServiceProvider CryptoServiceProviderFromPublicKeyInfo(string base64EncodedKey)
		{
			try
			{
				return PEMKeyLoader.CryptoServiceProviderFromPublicKeyInfo(Convert.FromBase64String(base64EncodedKey));
			}
			catch (FormatException)
			{
			}
			return null;
		}

		// Token: 0x04006028 RID: 24616
		private static byte[] SeqOID = new byte[]
		{
			48,
			13,
			6,
			9,
			42,
			134,
			72,
			134,
			247,
			13,
			1,
			1,
			1,
			5,
			0
		};
	}
}
