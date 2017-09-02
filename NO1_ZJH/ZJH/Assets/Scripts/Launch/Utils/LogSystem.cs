
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Log控制台 打印输出调试信息
/// </summary>
public static class LogSystem
{
    public const string Name = "System.Log";
    /// <summary>
    /// 是否打印输出Log文本
    /// </summary>
    private static bool mbFileLog = false;
    /// <summary>
    /// 是否在控制台打印Log
    /// </summary>
    private static bool mbDebugLog = false;

    /// <summary>
    /// 文件日志记录者
    /// </summary>
    private static StreamWriter mStreamFileWtiter = null;
    private static FileStream mfstream = null;
	private static string strLogPath = string.Empty;

    /// <summary>
    /// 日志系统初始化
    /// </summary>
    public static bool Init(string strLogFile, bool bFileLog = true, bool bDebugLog = false, int iLogMaxLines = 256)
    {
        mbFileLog = bFileLog;
        mbDebugLog = bDebugLog;
		strLogPath = strLogFile;
        if (mbFileLog)
        {
            try
            {
                if (File.Exists(strLogFile))
                {
                    File.Delete(strLogFile);
                }
                mfstream = new FileStream(strLogFile, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                mStreamFileWtiter = new StreamWriter(mfstream);
            }
            catch (System.Exception)
            {
                mfstream = new FileStream(strLogFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                mStreamFileWtiter = new StreamWriter(mfstream);
            }
            miLogCountMax = iLogMaxLines;
        }

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
    public static void TraceFile(int iType, string str)
    {
        if (mStreamFileWtiter == null)
            return;

        try
        {
            string strCurTime = StringBuilderCurrTime(iType);
            mStreamFileWtiter.Write(strCurTime);
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

        try
        {
            if (miLogCount++ > miLogCountMax)
            {
                ///从头开始写
                if (mfstream != null)
                {
                    mfstream.Seek(0, SeekOrigin.Begin);
                }
                miLogCount = 0;
            }
        }
        catch (System.Exception ex2)
        {
            LogSystem.LogError(ex2.ToString());
        }
    }
    private static System.Text.StringBuilder sb1 = new System.Text.StringBuilder();

    static string strInfo1 = "[Info ";
    static string strInfo2 = "[Warn ";
    static string strInfo3 = "[Error ";
    static string strInfo4 = "]";
    static string strTimeFlag = "MM-dd HH:mm:ss";
  
    static string StringBuilder(int iType, object[] args)
    {
        if (sb1 != null)
        {
            if( sb1.Length > 0)
                sb1.Remove(0, sb1.Length);

            if (iType == 0)
            {
                sb1.Append(strInfo1);
            }
            else if (iType == 1)
            {
                sb1.Append(strInfo2);
            }
            else
            {
                sb1.Append(strInfo3);
            }
            sb1.Append(DateTime.Now.ToString(strTimeFlag));
            sb1.Append(strInfo4);

            int len = args.Length;
            for (int i = 0; i < len; ++i)
            {
                sb1.Append(args[i]);
            }
            return sb1.ToString(0, sb1.Length);
        }

        return string.Empty;
    }
    private static System.Text.StringBuilder sb2 = new System.Text.StringBuilder();
    static string StringBuilderCurrTime(int iType)
    {
        if (sb2 != null)
        {
            if (sb2.Length > 0)
                sb2.Remove(0, sb2.Length);

            if (iType == 0)
            {
                sb2.Append(strInfo1);
            }
            else if (iType == 1)
            {
                sb2.Append(strInfo2);
            }
            else
            {
                sb2.Append(strInfo3);
            }
            sb2.Append(DateTime.Now.ToString(strTimeFlag));
            sb2.Append(strInfo4);
            return sb2.ToString(0,sb2.Length);
        }
        return string.Empty;
    }
    private static System.Text.StringBuilder sb3 = new System.Text.StringBuilder();
    static string strSplitChar = "|";
    static string StringBuilderContent(object[] args)
    {
        if (sb3 != null)
        {
            if (sb3.Length > 0)
                sb3.Remove(0, sb3.Length);

            int len = args.Length;
            for (int i = 0; i < len; ++i)
            {
                sb3.Append(args[i]);
                sb3.Append(strSplitChar);
            }
            return sb3.ToString(0, sb3.Length);
        }

        return string.Empty;
    }
    /// <summary>
    /// 打印一条信息日志
    /// </summary>
    /// <param name="msgText">文本记录</param>
    /// <param name="context">错误对象</param>
    public static void Log(params object[] args)
    {
#if LOG
        string msgText = StringBuilderContent(args);
        if (mbDebugLog)
        {
            if (Config.bEditor)
            {
                UnityEngine.Debug.Log(msgText, null);
            }
        }

        if (mbFileLog)
        {
            TraceFile(0, msgText);
        }
#endif
    }

    /// <summary>
    /// 打印一条警告日志
    /// </summary>
    /// <param name="msgObj"></param>
    /// <param name="context"></param>
    public static void LogWarning(params object[] args)
    {
        string msgText = StringBuilderContent(args);
        ///目的是为了避免相同的日志输出
        if (string.IsNullOrEmpty(mstrLastFileLog))
        {
            mstrLastFileLog = msgText;
        }
        else
        {
            if (mstrLastFileLog.Equals(msgText))
            {
                return;
            }
            mstrLastFileLog = msgText;
        }
        
        if (mbDebugLog)
        {
            if (Config.bEditor)
            {
                UnityEngine.Debug.LogError(msgText, null);
            }
        }

        if (mbFileLog)
        {
            TraceFile(1, msgText);
        }
    }

    /// <summary>
    /// 打印一条错误日志
    /// </summary>
    /// <param name="msgObj"></param>
    /// <param name="context"></param>
    public static void LogError(params object[] args)
    {
        string msgText = StringBuilderContent(args);

        ///目的是为了避免相同的日志输出
        if (string.IsNullOrEmpty(mstrLastFileLog))
        {
            mstrLastFileLog = msgText;
        }
        else
        {
            if (mstrLastFileLog.Equals(msgText))
            {
                return;
            }
            mstrLastFileLog = msgText;
        }
        UnityEngine.Debug.LogError(msgText, null);
        if (mbFileLog)
        {
            TraceFile(2, msgText);
        }
    }

	/// <summary>
	/// 得到日志地址
	/// </summary>
	/// <returns>The string log path.</returns>
	public static string GetStrLogPath()
	{
		return  strLogPath;
	}
}