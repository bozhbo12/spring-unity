using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


/*******************************************************************************************
 * 功能 : 文件路径处理工具
 ********************************************************************************************/
public class FilePathUtils
{

    static public string GetMobileFilePath(string path)
    {
        return Application.persistentDataPath + path;
    }
    
}