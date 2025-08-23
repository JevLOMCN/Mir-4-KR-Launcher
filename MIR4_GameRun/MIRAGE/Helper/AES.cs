using System;
using System.Security.Cryptography;
using System.Text;

namespace MIRAGE.Helper
{
  internal class AES
  {
    private byte[] Key2 = new byte[16]
    {
      (byte) 4,
      (byte) 0,
      (byte) 36,
      (byte) 141,
      (byte) 82,
      (byte) 73,
      (byte) 82,
      (byte) 244,
      (byte) 240,
      (byte) 117,
      (byte) 253,
      (byte) 183,
      (byte) 123,
      (byte) 221,
      (byte) 216,
      (byte) 186
    };

    public byte[] AES_Encrypt(byte[] origin, byte[] Key)
    {
      Aes aes = (Aes) null;
      try
      {
        aes = Aes.Create();
        aes.Key = Key;
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.ECB;
        return aes.CreateEncryptor(aes.Key, aes.IV).TransformFinalBlock(origin, 0, origin.Length);
      }
      finally
      {
        aes?.Clear();
      }
    }

    public byte[] AES_Decrypt(byte[] _encrypted)
    {
      if (_encrypted == null || _encrypted.Length == 0)
        throw new ArgumentNullException(nameof (_encrypted));
      Aes aes = (Aes) null;
      try
      {
        aes = Aes.Create();
        aes.Key = this.Key2;
        aes.Padding = PaddingMode.PKCS7;
        aes.Mode = CipherMode.ECB;
        return aes.CreateDecryptor(aes.Key, aes.IV).TransformFinalBlock(_encrypted, 0, _encrypted.Length);
      }
      finally
      {
        aes?.Clear();
      }
    }

    public void PrintByteArr(byte[] b)
    {
      for (int index = 0; index < b.Length; ++index)
      {
        Console.Write("{0:X2}", (object) b[index]);
        string.Format("{0:X2}", (object) b[index]);
        if (index != 0 && (index + 1) % 16 == 0)
          Console.WriteLine();
      }
      Console.WriteLine();
    }

    public string ToHexString(byte[] hex)
    {
      if (hex == null)
        return (string) null;
      if (hex.Length == 0)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (byte num in hex)
        stringBuilder.Append(num.ToString("x2"));
      return stringBuilder.ToString();
    }
  }
}
