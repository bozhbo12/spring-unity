using System.IO;
using System.Text;
using System.Collections;
#if !UNITY_WP8 && WEI_GZip
using ICSharpCode.SharpZipLib.GZip;
#endif

public class ByteUtils
{
    /// <summary>
    /// GZIP压缩UTF8字符串
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static byte[] compressString(string input)
    {
        if (input == null)
        {
            return null;
        }

        byte[] encodeBytes = System.Text.Encoding.UTF8.GetBytes(input);
        return ByteUtils.compress(encodeBytes);
    }

    /// <summary>
    /// GZIP解压缩UTF8字符串
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string decompressString(byte[] input)
    {
        if (input == null)
        {
            return null;
        }

        byte[] encodeBytes = ByteUtils.decompress(input);
        return ByteUtils.byteConverString(encodeBytes);
    }

    /// <summary>
    /// GZIP压缩
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] compress(byte[] data)
    {
		#if !UNITY_WP8 && WEI_GZip
        MemoryStream ms = new MemoryStream();
        GZipOutputStream stream = new GZipOutputStream(ms);
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Finish();
        stream.Close();
        return ms.ToArray();
#else
		return data;
#endif
    }

    /// <summary>
    /// GZIP解压缩
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static byte[] decompress(byte[] data)
    {
		#if !UNITY_WP8 && WEI_GZip
        MemoryStream ms = new MemoryStream(data);
        GZipInputStream stream = new GZipInputStream(ms);
        MemoryStream buff = new MemoryStream();
        byte[] b = new byte[1024];
        while (true)
        {
            int i = stream.Read(b, 0, b.Length);
            if (i <= 0)
            {
                break;
            }
            else
            {
                buff.Write(b, 0, i);
            }
        }
        stream.Close();
        return buff.ToArray();
#else
		return data;
#endif
    }

    /// <summary>
    /// wp8 byte转换为string
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string byteConverString(byte[] data)
    {
        Decoder d = Encoding.UTF8.GetDecoder();
        int arrSize = d.GetCharCount(data, 0, data.Length);
        char[] chars = new char[arrSize];
        int charSize = d.GetChars(data, 0, data.Length, chars, 0);
        string str = new string(chars, 0, charSize);

        return str;
    }

    /// <summary>
    /// wp8 byte转换为string
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static string byteConverString(byte[] data, int index, int count)
    {
        Decoder d = Encoding.UTF8.GetDecoder();
        int arrSize = d.GetCharCount(data, index, count);
        char[] chars = new char[arrSize];
        int charSize = d.GetChars(data, index, count, chars, 0);
        string str = new string(chars, 0, charSize);

        return str;
    }
    /// <summary>
    /// float to Short
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static short floatConverShort(float data)
    {
        data = data * 100f;
        if (data > 32767f)
        {
            data = 32767f;
        }
        if (data < -32768f)
        {
            data = -32768f;
        }

        return (short)data;
    }

    public static float shortConverFloat(short data)
    {
        float newdata = (float)data;
        if (data != 0)
        {
            newdata = newdata / 100f;
        }
        return newdata;
    }
    public static byte intConverByte(int data)
    {
        if (data > 255)
        {
            data = 255;
        }
        if (data < 0)
        {
            data = 0;
        }

        return (byte)data;
    }
}
