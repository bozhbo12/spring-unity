
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// fps输出日志
/// </summary>
public static class LogFps
{

    private const string Name = "fps.log";

    private static System.Text.StringBuilder sb = null;

    /// <summary>
    /// 文件日志记录者
    /// </summary>
    private static StreamWriter mStreamFileWtiter = null;
    private static FileStream mfstream = null;

    private static bool bInit = false;
    /// <summary>
    /// 初始化
    /// </summary>
    public static bool Init(int iLogMaxLines = 256)
    {
        if (!Config.bAndroid && !Config.bIPhone)
            return false;

        if (!Config.bWriteFps)
            return false;

        if (bInit)
            return false;
       
        string strLogFpsFile = Config.GetAssetBundleRootPath() + "/" + Name;
        if (File.Exists(strLogFpsFile))
        {
            File.Delete(strLogFpsFile);
        }
        try
        {
            mfstream = new FileStream(strLogFpsFile, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
            mStreamFileWtiter = new StreamWriter(mfstream);
        }
        catch (System.Exception e)
        {
            LogSystem.LogWarning("FpsWrite:Error:", e.ToString());
            if (File.Exists(strLogFpsFile))
            {
                File.Delete(strLogFpsFile);
            }
            return false;
        }
        miLogCountMax = iLogMaxLines;
        sb = new System.Text.StringBuilder();
        bInit = true;
        return true;
    }
   
    /// <summary>
    /// 当前行数
    /// </summary>
    private static int miLogCount = 0;
    /// <summary>
    /// 设定最大输出行数
    /// </summary>
    private static int miLogCountMax = 256;
    private static string mstrLastFileLog = string.Empty;
    /// <summary>
    /// 输出一行日志到文件
    /// </summary>
    /// <param name="str">日志内容</param>
    public static void TraceFile(string str)
    {
        if (mStreamFileWtiter == null)
            return;

        try
        {
            mStreamFileWtiter.WriteLine(str);
            ///暂时每次日志输出，发布是调节为每帧flush一次，出异常捕捉后flush一次
            mStreamFileWtiter.Flush();
        }
        catch (IOException)
        {
            ///不处理，抓住即可，多数为磁盘满，坏道等
        }
        catch (System.Exception ex)
        {
            LogSystem.LogError(ex.ToString());
        }

        //try
        //{
        //    if (miLogCount++ > miLogCountMax)
        //    {
        //        ///从头开始写
        //        if (mfstream != null)
        //        {
        //            mfstream.Seek(0, SeekOrigin.Begin);
        //        }
        //        miLogCount = 0;
        //    }
        //}
        //catch (System.Exception ex2)
        //{
        //    LogSystem.LogError(ex2.ToString());
        //}
    }

    static string StringBuilder(int iFps)
    {
        if (sb != null)
        {
            if (sb.Length > 0)
                sb.Remove(0, sb.Length);

            
            sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append(" ");
            sb.Append(iFps);

            return sb.ToString();
        }

        return string.Empty;
    }

    /// <summary>
    /// log
    /// </summary>
    /// <param name="ifps"></param>
    public static void LOG(int ifps)
    {
        if (!bInit)
            return;

        TraceFile(StringBuilder(ifps));
    }
}