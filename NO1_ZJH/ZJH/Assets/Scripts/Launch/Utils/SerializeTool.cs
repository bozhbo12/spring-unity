using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 二进制序列化工具
/// 20160218
/// zhangrj
/// </summary>
public class SerializeTool
{
    
    public static void SerializeMethod<T>(List<T> list, string path)
    {
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, list);
        }
    }

    public static List<T> DeSerializeMethod<T>(string path)
    {
        List<T> list = new List<T>();
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                BinaryFormatter bf = new BinaryFormatter();
                list = bf.Deserialize(fs) as List<T>;
            }
        }
        catch
        { 
            
        }
        return list;
    }
}