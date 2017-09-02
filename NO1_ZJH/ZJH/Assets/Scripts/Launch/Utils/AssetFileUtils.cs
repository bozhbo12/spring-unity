/// 作者 wanglc
/// 日期 20140923
/// 实现目标  资源包文件操作辅助

using System.IO;
using UnityEngine;
//using ZXing.QrCode;//WEIBOBUG
//using ZXing;//WEIBOBUG
using System.Collections.Generic;
/// <summary>
/// 资源包文件操作辅助
/// </summary>
public class AssetFileUtils
{
    /// <summary>
    /// 删除资源
    /// </summary>
    /// <param name="strFilePath"></param>
    /// <returns></returns>
    public static bool DeleteAsset(string strFilePath)
    {
        try
        {
            if (File.Exists(strFilePath))
            {
                File.Delete(strFilePath);
                return true;
            }
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError(ex.ToString());

        }
        return false;
    }
    /// <summary>
    /// 写入资源包操作
    /// </summary>
    /// <param name="strPath">路径</param>
    /// <param name="bytes">包数据</param>
    /// <returns>成败</returns>
    public static bool WriteLocalAsset(string strPath, byte[] bytes)
    {
        FileInfo t = new FileInfo(strPath);
        if (t.Exists)
        {
            if (!DeleteAsset(strPath))
                return false;
        }

        try
        {
            if (!t.Exists)
            {
                Directory.CreateDirectory(t.DirectoryName);
            }
            WriteFile(strPath, bytes);
            return true;
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError("WriteLocalAsset", ex.ToString());
        }

        return false;
    }
    /// <summary>
    /// 真正的写入文件
    /// </summary>
    /// <param name="filePath">路径</param>
    /// <param name="data">数据</param>
    public static void WriteFile(string filePath, object data)
    {
        FileStream fileStream = File.OpenWrite(filePath);
        fileStream.Position = 0;
        fileStream.SetLength(0);
        if (data is string)
        {
            byte[] d = System.Text.Encoding.UTF8.GetBytes(data as string);
            fileStream.Write(d, 0, d.Length);
        }
        else
        {
            fileStream.Write(data as byte[], 0, (data as byte[]).Length);
        }
        fileStream.Flush();
        fileStream.Close();
    }

    /// <summary>
    /// 获取二维码
    /// </summary>
    /// <param name="textForEncoding"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public static Texture GetQrCodeTexture(string textForEncoding, int width, int height)
    {
        Texture2D encoded = new Texture2D(width, height);

//        if (!string.IsNullOrEmpty(textForEncoding)) //WEIBOBUG
//        {
//            QrCodeEncodingOptions options = new QrCodeEncodingOptions();
//            options.Height = encoded.height;
//            options.Width = encoded.width;
//
//            BarcodeWriter writer = new BarcodeWriter();
//            writer.Format = BarcodeFormat.QR_CODE;
//            writer.Options = options;
//
//            Color32[] color32 = writer.Write(textForEncoding);
//            encoded.SetPixels32(color32);
//            encoded.Apply();
//        }

        return encoded;
    }

	public static void ReadString(string strText, ref Dictionary<string, string> DictMap)
	{
		if (string.IsNullOrEmpty(strText))
		{
			LogSystem.LogWarning("Config error , not found TaskString text!");
			return;
		}

		string[] strLines = strText.Split(new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries); ;
		for (int i = 0; i < strLines.Length; i++)
		{
			string[] split = strLines[i].Split(new string[] { "=" }, 2, System.StringSplitOptions.RemoveEmptyEntries);
			if (split.Length == 2)
			{
				if (DictMap.ContainsKey(split[0]))
					LogSystem.LogWarning("the key is echo in local file!!! please check the key = ", split[0]);
				else
				{
					split[1] = split[1].Replace("[n]", "\n");
					DictMap[split[0]] = split[1];
				}
			}
		}
	}

	/// <summary>
	/// String 强转 Int 时调用 默认返回 0
	/// 避免转换过程中包含空字符、以及非数字字符
	/// 导致程序报错
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static int IntParse(string value, int defaultValue = 0)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		value = value.Trim();
		int result;
		if (int.TryParse(value, out result))
		{
			return result;
		}
		return defaultValue;
	}
	// <summary>
	/// String 强转 bool 时调用 默认返回 false
	/// 避免转换过程中包含空字符、以及非数字字符
	/// 导致程序报错
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static bool BoolParse(string value, bool defaultValue = false)
	{
		if (string.IsNullOrEmpty(value))
		{
			return defaultValue;
		}
		value = value.Trim();
		bool result;
		int iResult;
		if (bool.TryParse(value, out result))
		{
			return result;
		}
		else if (int.TryParse(value, out iResult))
		{
			return iResult == 1;
		}
		return defaultValue;
	}
}
