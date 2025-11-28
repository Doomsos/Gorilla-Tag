using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

// Token: 0x020009C1 RID: 2497
public static class AESHMAC
{
	// Token: 0x06003FCA RID: 16330 RVA: 0x00155F70 File Offset: 0x00154170
	public static byte[] NewKey()
	{
		byte[] array = new byte[32];
		AESHMAC.gRNG.GetBytes(array);
		return array;
	}

	// Token: 0x06003FCB RID: 16331 RVA: 0x00155F91 File Offset: 0x00154191
	public static string SimpleEncrypt(string plaintext, byte[] key, byte[] auth, byte[] salt = null)
	{
		if (string.IsNullOrEmpty(plaintext))
		{
			throw new ArgumentNullException("plaintext");
		}
		return Convert.ToBase64String(AESHMAC.SimpleEncrypt(Encoding.UTF8.GetBytes(plaintext), key, auth, salt));
	}

	// Token: 0x06003FCC RID: 16332 RVA: 0x00155FC0 File Offset: 0x001541C0
	public static string SimpleDecrypt(string ciphertext, byte[] key, byte[] auth, int saltLength = 0)
	{
		if (string.IsNullOrWhiteSpace(ciphertext))
		{
			throw new ArgumentNullException("ciphertext");
		}
		byte[] array = AESHMAC.SimpleDecrypt(Convert.FromBase64String(ciphertext), key, auth, saltLength);
		if (array != null)
		{
			return Encoding.UTF8.GetString(array);
		}
		return null;
	}

	// Token: 0x06003FCD RID: 16333 RVA: 0x00155FFF File Offset: 0x001541FF
	public static string SimpleEncryptWithKey(string plaintext, string key, byte[] salt = null)
	{
		if (string.IsNullOrEmpty(plaintext))
		{
			throw new ArgumentNullException("plaintext");
		}
		return Convert.ToBase64String(AESHMAC.SimpleEncryptWithKey(Encoding.UTF8.GetBytes(plaintext), key, salt));
	}

	// Token: 0x06003FCE RID: 16334 RVA: 0x0015602C File Offset: 0x0015422C
	public static string SimpleDecryptWithKey(string ciphertext, string key, int saltLength = 0)
	{
		if (string.IsNullOrWhiteSpace(ciphertext))
		{
			throw new ArgumentNullException("ciphertext");
		}
		byte[] array = AESHMAC.SimpleDecryptWithKey(Convert.FromBase64String(ciphertext), key, saltLength);
		if (array != null)
		{
			return Encoding.UTF8.GetString(array);
		}
		return null;
	}

	// Token: 0x06003FCF RID: 16335 RVA: 0x0015606C File Offset: 0x0015426C
	public static byte[] SimpleEncrypt(byte[] plaintext, byte[] key, byte[] auth, byte[] salt = null)
	{
		if (key == null || key.Length != 32)
		{
			throw new ArgumentException(string.Format("Key must be {0} bits", 256), "key");
		}
		if (auth == null || auth.Length != 32)
		{
			throw new ArgumentException(string.Format("Auth must be {0} bits", 256), "auth");
		}
		if (plaintext == null || plaintext.Length < 1)
		{
			throw new ArgumentNullException("plaintext");
		}
		if (salt == null)
		{
			salt = new byte[0];
		}
		byte[] iv;
		byte[] array;
		using (AesManaged aesManaged = new AesManaged
		{
			KeySize = 256,
			BlockSize = 128,
			Mode = 1,
			Padding = 2
		})
		{
			aesManaged.GenerateIV();
			iv = aesManaged.IV;
			using (ICryptoTransform cryptoTransform = aesManaged.CreateEncryptor(key, iv))
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, 1))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
						{
							binaryWriter.Write(plaintext);
						}
					}
					array = memoryStream.ToArray();
				}
			}
		}
		byte[] result;
		using (HMACSHA256 hmacsha = new HMACSHA256(auth))
		{
			using (MemoryStream memoryStream2 = new MemoryStream())
			{
				using (BinaryWriter binaryWriter2 = new BinaryWriter(memoryStream2))
				{
					binaryWriter2.Write(salt);
					binaryWriter2.Write(iv);
					binaryWriter2.Write(array);
					binaryWriter2.Flush();
					byte[] array2 = hmacsha.ComputeHash(memoryStream2.ToArray());
					binaryWriter2.Write(array2);
				}
				result = memoryStream2.ToArray();
			}
		}
		return result;
	}

	// Token: 0x06003FD0 RID: 16336 RVA: 0x00156278 File Offset: 0x00154478
	public static byte[] SimpleDecrypt(byte[] ciphertext, byte[] key, byte[] auth, int saltLength = 0)
	{
		if (key == null || key.Length != 32)
		{
			throw new ArgumentException(string.Format("Key must be {0} bits", 256), "key");
		}
		if (auth == null || auth.Length != 32)
		{
			throw new ArgumentException(string.Format("Auth must be {0} bits", 256), "auth");
		}
		if (ciphertext == null || ciphertext.Length == 0)
		{
			throw new ArgumentNullException("ciphertext");
		}
		byte[] result;
		using (HMACSHA256 hmacsha = new HMACSHA256(auth))
		{
			byte[] array = new byte[hmacsha.HashSize / 8];
			byte[] array2 = hmacsha.ComputeHash(ciphertext, 0, ciphertext.Length - array.Length);
			int num = 16;
			if (ciphertext.Length < array.Length + saltLength + num)
			{
				result = null;
			}
			else
			{
				Array.Copy(ciphertext, ciphertext.Length - array.Length, array, 0, array.Length);
				int num2 = 0;
				for (int i = 0; i < array.Length; i++)
				{
					num2 |= (int)(array[i] ^ array2[i]);
				}
				if (num2 != 0)
				{
					result = null;
				}
				else
				{
					using (AesManaged aesManaged = new AesManaged
					{
						KeySize = 256,
						BlockSize = 128,
						Mode = 1,
						Padding = 2
					})
					{
						byte[] array3 = new byte[num];
						Array.Copy(ciphertext, saltLength, array3, 0, array3.Length);
						using (ICryptoTransform cryptoTransform = aesManaged.CreateDecryptor(key, array3))
						{
							using (MemoryStream memoryStream = new MemoryStream())
							{
								using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, 1))
								{
									using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
									{
										binaryWriter.Write(ciphertext, saltLength + array3.Length, ciphertext.Length - saltLength - array3.Length - array.Length);
									}
								}
								result = memoryStream.ToArray();
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06003FD1 RID: 16337 RVA: 0x001564D4 File Offset: 0x001546D4
	public static byte[] SimpleEncryptWithKey(byte[] plaintext, string key, byte[] salt = null)
	{
		if (salt == null)
		{
			salt = new byte[0];
		}
		if (string.IsNullOrWhiteSpace(key) || key.Length < 12)
		{
			throw new ArgumentException(string.Format("Key must be at least {0} characters", 12), "key");
		}
		if (plaintext == null || plaintext.Length == 0)
		{
			throw new ArgumentNullException("plaintext");
		}
		byte[] array = new byte[16 + salt.Length];
		Array.Copy(salt, array, salt.Length);
		int num = salt.Length;
		byte[] bytes;
		using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(key, 8, 10000))
		{
			byte[] salt2 = rfc2898DeriveBytes.Salt;
			bytes = rfc2898DeriveBytes.GetBytes(32);
			Array.Copy(salt2, 0, array, num, salt2.Length);
			num += salt2.Length;
		}
		byte[] bytes2;
		using (Rfc2898DeriveBytes rfc2898DeriveBytes2 = new Rfc2898DeriveBytes(key, 8, 10000))
		{
			byte[] salt3 = rfc2898DeriveBytes2.Salt;
			bytes2 = rfc2898DeriveBytes2.GetBytes(32);
			Array.Copy(salt3, 0, array, num, salt3.Length);
		}
		return AESHMAC.SimpleEncrypt(plaintext, bytes, bytes2, array);
	}

	// Token: 0x06003FD2 RID: 16338 RVA: 0x001565EC File Offset: 0x001547EC
	public static byte[] SimpleDecryptWithKey(byte[] ciphertext, string key, int saltLength = 0)
	{
		if (string.IsNullOrWhiteSpace(key) || key.Length < 12)
		{
			throw new ArgumentException(string.Format("Key must be at least {0} characters", 12), "key");
		}
		if (ciphertext == null || ciphertext.Length == 0)
		{
			throw new ArgumentNullException("ciphertext");
		}
		byte[] array = new byte[8];
		byte[] array2 = new byte[8];
		Array.Copy(ciphertext, saltLength, array, 0, array.Length);
		Array.Copy(ciphertext, saltLength + array.Length, array2, 0, array2.Length);
		byte[] key2 = AESHMAC.Rfc2898DeriveBytes(key, array, 10000, 32);
		byte[] auth = AESHMAC.Rfc2898DeriveBytes(key, array2, 10000, 32);
		return AESHMAC.SimpleDecrypt(ciphertext, key2, auth, array.Length + array2.Length + saltLength);
	}

	// Token: 0x06003FD3 RID: 16339 RVA: 0x00156694 File Offset: 0x00154894
	private static byte[] Rfc2898DeriveBytes(string password, byte[] salt, int iterations, int numBytes)
	{
		byte[] bytes;
		using (Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
		{
			bytes = rfc2898DeriveBytes.GetBytes(numBytes);
		}
		return bytes;
	}

	// Token: 0x04005103 RID: 20739
	private static readonly RandomNumberGenerator gRNG = RandomNumberGenerator.Create();

	// Token: 0x04005104 RID: 20740
	public const int BlockBitSize = 128;

	// Token: 0x04005105 RID: 20741
	public const int KeyBitSize = 256;

	// Token: 0x04005106 RID: 20742
	public const int SaltBitSize = 64;

	// Token: 0x04005107 RID: 20743
	public const int Iterations = 10000;

	// Token: 0x04005108 RID: 20744
	public const int MinPasswordLength = 12;
}
