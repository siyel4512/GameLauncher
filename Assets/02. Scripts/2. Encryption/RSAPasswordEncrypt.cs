using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Windows.Forms;

internal class RSAPasswordEncrypt
{
    public string GetRSAPassword(string pubKeyStr, string id, string password)
    {
        // 0 : Modulus
        // 1 : Exponent

        ShiftCryptography shift = new ShiftCryptography();
        string[] temp = shift.GetNumber(pubKeyStr).Split("||");
        
        return RSAEncrypt(temp[0], temp[1], id, password);
    }

    private string RSAEncrypt(string modulus, string exponent, string id, string plainText)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();        
        RSAParameters pubKey = new RSAParameters();

        pubKey.Modulus = Convert.FromBase64String(modulus);
        pubKey.Exponent = Convert.FromBase64String(exponent);

        rsa.ImportParameters(pubKey);

        //DirectoryInfo di = new DirectoryInfo(Environment.CurrentDirectory + "\\KEY");
        //if (!di.Exists) di.Create();
        //string pemPath = Environment.CurrentDirectory + "\\KEY\\" + id + ".pem";
        //File.Create(pemPath).Close();

        DirectoryInfo di;
        string pemPath;
#if UNITY_EDITOR
        di = new DirectoryInfo(Environment.CurrentDirectory + "\\KEY");
        if (!di.Exists) di.Create();
        pemPath = Environment.CurrentDirectory + "\\KEY\\" + id + ".pem";
        File.Create(pemPath).Close();
#else
        string tempPath = Application.ExecutablePath;

        // path setting
        string[] splitPaths = tempPath.Split('\\');
        string assemblePath = "";

        for (int i = 0; i < splitPaths.Length - 1; i++)
        {
            assemblePath += (splitPaths[i] + "\\");
        }

        di = new DirectoryInfo(assemblePath + "KEY");
        if (!di.Exists) di.Create();
        pemPath = assemblePath + "KEY\\" + id + ".pem";
        File.Create(pemPath).Close();
#endif





        File.WriteAllText(pemPath, ExportPublicKey(rsa));

        byte[] enbuf = rsa.Encrypt((new UTF8Encoding()).GetBytes(plainText), false);

        return Convert.ToBase64String(enbuf);
    }

    public static RSACryptoServiceProvider ImportPublicKey(string pem)
    {
        PemReader pr = new PemReader(new StringReader(pem));
        AsymmetricKeyParameter publicKey = (AsymmetricKeyParameter)pr.ReadObject();
        RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);

        RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
        csp.ImportParameters(rsaParams);
        return csp;
    }

    public static string ExportPublicKey(RSACryptoServiceProvider csp)
    {
        StringWriter outputStream = new StringWriter();
        var parameters = csp.ExportParameters(false);
        using (var stream = new MemoryStream())
        {
            var writer = new BinaryWriter(stream);
            writer.Write((byte)0x30); // SEQUENCE
            using (var innerStream = new MemoryStream())
            {
                var innerWriter = new BinaryWriter(innerStream);
                innerWriter.Write((byte)0x30); // SEQUENCE
                EncodeLength(innerWriter, 13);
                innerWriter.Write((byte)0x06); // OBJECT IDENTIFIER
                var rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
                EncodeLength(innerWriter, rsaEncryptionOid.Length);
                innerWriter.Write(rsaEncryptionOid);
                innerWriter.Write((byte)0x05); // NULL
                EncodeLength(innerWriter, 0);
                innerWriter.Write((byte)0x03); // BIT STRING
                using (var bitStringStream = new MemoryStream())
                {
                    var bitStringWriter = new BinaryWriter(bitStringStream);
                    bitStringWriter.Write((byte)0x00); // # of unused bits
                    bitStringWriter.Write((byte)0x30); // SEQUENCE
                    using (var paramsStream = new MemoryStream())
                    {
                        var paramsWriter = new BinaryWriter(paramsStream);
                        EncodeIntegerBigEndian(paramsWriter, parameters.Modulus); // Modulus
                        EncodeIntegerBigEndian(paramsWriter, parameters.Exponent); // Exponent
                        var paramsLength = (int)paramsStream.Length;
                        EncodeLength(bitStringWriter, paramsLength);
                        bitStringWriter.Write(paramsStream.GetBuffer(), 0, paramsLength);
                    }
                    var bitStringLength = (int)bitStringStream.Length;
                    EncodeLength(innerWriter, bitStringLength);
                    innerWriter.Write(bitStringStream.GetBuffer(), 0, bitStringLength);
                }
                var length = (int)innerStream.Length;
                EncodeLength(writer, length);
                writer.Write(innerStream.GetBuffer(), 0, length);
            }

            var base64 = Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length).ToCharArray();

            outputStream.Write("-----BEGIN PUBLIC KEY-----\n");
            for (var i = 0; i < base64.Length; i += 64)
            {
                outputStream.Write(base64, i, Math.Min(64, base64.Length - i));
                outputStream.Write("\n");
            }
            outputStream.Write("-----END PUBLIC KEY-----");
        }

        return outputStream.ToString();
    }

    private static void EncodeLength(BinaryWriter stream, int length)
    {
        if (length < 0) throw new ArgumentOutOfRangeException("length", "Length must be non-negative");
        if (length < 0x80)
        {
            // Short form
            stream.Write((byte)length);
        }
        else
        {
            // Long form
            var temp = length;
            var bytesRequired = 0;
            while (temp > 0)
            {
                temp >>= 8;
                bytesRequired++;
            }
            stream.Write((byte)(bytesRequired | 0x80));
            for (var i = bytesRequired - 1; i >= 0; i--)
            {
                stream.Write((byte)(length >> (8 * i) & 0xff));
            }
        }
    }

    private static void EncodeIntegerBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true)
    {
        stream.Write((byte)0x02); // INTEGER
        var prefixZeros = 0;
        for (var i = 0; i < value.Length; i++)
        {
            if (value[i] != 0) break;
            prefixZeros++;
        }
        if (value.Length - prefixZeros == 0)
        {
            EncodeLength(stream, 1);
            stream.Write((byte)0);
        }
        else
        {
            if (forceUnsigned && value[prefixZeros] > 0x7f)
            {
                // Add a prefix zero to force unsigned if the MSB is 1
                EncodeLength(stream, value.Length - prefixZeros + 1);
                stream.Write((byte)0);
            }
            else
            {
                EncodeLength(stream, value.Length - prefixZeros);
            }
            for (var i = prefixZeros; i < value.Length; i++)
            {
                stream.Write(value[i]);
            }
        }
    }
}
