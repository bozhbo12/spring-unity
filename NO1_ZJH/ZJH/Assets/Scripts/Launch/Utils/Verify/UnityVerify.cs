using System;
using System.Runtime.InteropServices;
#if UNITY_IPHONE || UNITY_XBOX360
using UnityEngine;
using System.Linq;
#endif //UNITY_IPHONE || UNITY_XBOX360

public class UnityVerify
{
#if !UNITY_EDITOR &&( UNITY_IPHONE || UNITY_XBOX360 )
    
    [DllImport("__Internal")]
    public static extern IntPtr FxVerify_GetType();

    [DllImport("__Internal")]
    public static extern IntPtr FxVerify_GetInterface();

    [DllImport("__Internal")]
    public static extern int FxVerify_GetVersion(IntPtr pInstance);

    // 收到服务端 SERVER_MSG_SET_ENCODE 把 nCondition 设置进来
    [DllImport("__Internal")]
    public static extern void FxVerify_SetClientCond(IntPtr pInstance, Byte[] cond);

    [DllImport("__Internal")]
    public static extern void FxVerify_SetClientCode(IntPtr pInstance, Byte[] cond,int nLen);

    // 发送CLIENT_RET_ENCODE 时参数 nVerify 这边获得
    [DllImport("__Internal")]
    public static extern void FxVerify_GetRetEncodeVerify(IntPtr pInstance, string strIp, int nDynamicKey, int nPort,
        Byte[] strInfo, int nStrInfoLen, string strDecID, int nAddress,IntPtr output);

    // 发送CLIENT_LOGIN 时参数 nVerify 这边获得
    // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
    [DllImport("__Internal")]
    public static extern void FxVerify_GetLoginVerify(IntPtr pInstance, Byte[] strInfo, int nStrInfoLen, int nPort,
		                                                string strIp, string strDecID, int nAddress, int nDynamicKey, IntPtr output);
  
    // 发送CLIENT_CHOOSE_ROLE 时参数 nVerify 这边获得
    // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
    [DllImport("__Internal")]
    public static extern void FxVerify_GetChooseRoleVerify(IntPtr pInstance, Byte[] strInfo, int nStrInfoLen,
		                                                    string strIp, int nPort, string strDecID, int nDynamicKey, byte[] szRoleName, int nRoleNameLen, int nAddress, IntPtr output);
  
    // 发送CLIENT_SELECT 时参数 nVerify 这边获得
    // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
    [DllImport("__Internal")]
    public static extern void FxVerify_GetSelectVerify(IntPtr pInstance, int nDynamicKey, int nFunctionId,
		                                                string strIp, int nPort, string strDecID, int nPersistidSerial, int nSerial, int nPersistidIdent, IntPtr output);

    // 发送CLIENT_CUSTOM 时参数 nVerify 这边获得
    // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
    [DllImport("__Internal")]
    public static extern void FxVerify_GetCustomVerify(IntPtr pInstance, string strIp, int nDynamicKey, int nPort, int nSerial,
		                                                int nArgNum, string strDecID, Byte[] pArgData, int nArgLen, IntPtr output);

    // 发送CLIENT_LOGIN 时参数 账户加密
    [DllImport("__Internal")]
	public static extern bool FxVerify_EncodeAccount(IntPtr pInstance, string strAccount, IntPtr OutBuffer, ref int nSize);

    // 发送CLIENT_LOGIN 时参数 密码加密
    [DllImport("__Internal")]
	public static extern bool FxVerify_EncodePassword(IntPtr pInstance, string strPassword, IntPtr OutBuffer, ref int nSize);

    [DllImport("__Internal")]
    public static extern IntPtr FxVerify_Release(IntPtr pInstance);

    [DllImport("__Internal")]
    public static extern IntPtr FxVerify_AntiDebuggerCheck(IntPtr pInstance);

    [DllImport("__Internal")]
    public static extern IntPtr FxVerify_AntiDebuggerStop(IntPtr pInstance);
#else
    [DllImport("ClientVerify", EntryPoint = "FxVerify_GetType", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr FxVerify_GetType();

    [DllImport("ClientVerify", EntryPoint = "FxVerify_GetInterface", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr FxVerify_GetInterface();

    [DllImport("ClientVerify", EntryPoint = "FxVerify_GetVersion", CallingConvention = CallingConvention.Cdecl)]
    public static extern int FxVerify_GetVersion(IntPtr pInstance);

    // 收到服务端 SERVER_MSG_SET_ENCODE 把 nCondition 设置进来
    [DllImport("ClientVerify", EntryPoint = "FxVerify_SetClientCond", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FxVerify_SetClientCond(IntPtr pInstance, Byte[] cond);

    // 收到服务端 SERVER_MSG_SET_ENCODE 把 pData 设置进来
    [DllImport("ClientVerify", EntryPoint = "FxVerify_SetClientCode", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FxVerify_SetClientCode(IntPtr pInstance, Byte[] cond, int nLen);

    // 发送CLIENT_RET_ENCODE 时参数 nVerify 这边获得
    // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
    [DllImport("ClientVerify", EntryPoint = "FxVerify_GetRetEncodeVerify", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FxVerify_GetRetEncodeVerify(IntPtr pInstance, string strIp, int nDynamicKey, int nPort, Byte[] strInfo, int nStrInfoLen, string strDecID, int nAddress, IntPtr output);

    // 发送CLIENT_LOGIN 时参数 nVerify 这边获得
    // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
    [DllImport("ClientVerify", EntryPoint = "FxVerify_GetLoginVerify", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FxVerify_GetLoginVerify(IntPtr pInstance, Byte[] strInfo, int nStrInfoLen, int nPort, string strIp, string strDecID, int nAddress, int nDynamicKey, IntPtr output);

    // 发送CLIENT_CHOOSE_ROLE 时参数 nVerify 这边获得
    // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
    [DllImport("ClientVerify", EntryPoint = "FxVerify_GetChooseRoleVerify", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FxVerify_GetChooseRoleVerify(IntPtr pInstance, Byte[] strInfo, int nStrInfoLen, string strIp, int nPort, string strDecID, int nDynamicKey, byte[] szRoleName, int nRoleNameLen, int nAddress, IntPtr output);

    // 发送CLIENT_SELECT 时参数 nVerify 这边获得
    // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
    [DllImport("ClientVerify", EntryPoint = "FxVerify_GetSelectVerify", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FxVerify_GetSelectVerify(IntPtr pInstance, int nDynamicKey, int nFunctionId, string strIp, int nPort, string strDecID, int nPersistidSerial, int nSerial, int nPersistidIdent, IntPtr output);

    // 发送CLIENT_CUSTOM 时参数 nVerify 这边获得
    // nDynamicKey ，strInfo 是 SERVER_SET_ENCODE 里的参数
    [DllImport("ClientVerify", EntryPoint = "FxVerify_GetCustomVerify", CallingConvention = CallingConvention.Cdecl)]
    public static extern void FxVerify_GetCustomVerify(IntPtr pInstance, string strIp, int nDynamicKey, int nPort, int nSerial, int nArgNum, string strDecID, Byte[] pArgData, int nArgLen, IntPtr output);

    // 发送CLIENT_LOGIN 时参数 账户加密
    [DllImport("ClientVerify", EntryPoint = "FxVerify_EncodeAccount", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool FxVerify_EncodeAccount(IntPtr pInstance, string strAccount, IntPtr OutBuffer, ref int nSize);

    // 发送CLIENT_LOGIN 时参数 密码加密
    [DllImport("ClientVerify", EntryPoint = "FxVerify_EncodePassword", CallingConvention = CallingConvention.Cdecl)]
    public static extern bool FxVerify_EncodePassword(IntPtr pInstance, string strPassword, IntPtr OutBuffer, ref int nSize);

    [DllImport("ClientVerify", EntryPoint = "FxVerify_Release", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr FxVerify_Release(IntPtr pInstance);

    [DllImport("ClientVerify", EntryPoint = "FxVerify_AntiDebuggerCheck", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr FxVerify_AntiDebuggerCheck(IntPtr pInstance);

    [DllImport("ClientVerify", EntryPoint = "FxVerify_AntiDebuggerStop", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr FxVerify_AntiDebuggerStop(IntPtr pInstance);
#endif
    public static IntPtr fxVerifyinstance = IntPtr.Zero;
    public static IntPtr fxVerifyType = IntPtr.Zero;

    public static void UVFxVerify_GetType()
    {
        if (fxVerifyType == IntPtr.Zero)
        {
            fxVerifyType = FxVerify_GetType();
        }
    }


    public static void UVFxVerify_GetInterface()
    {
        try
        {
            fxVerifyinstance = FxVerify_GetInterface();
        }
        catch (Exception ex)
        {
            fxVerifyinstance = IntPtr.Zero;
            LogSystem.LogError(ex.ToString());

        }
    }


    public static int UVFxVerify_GetVersion()
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            return FxVerify_GetVersion(fxVerifyinstance);
        }
        return 0x300;
    }

    // 收到服务端 SERVER_MSG_SET_ENCODE 把 nCondition 设置进来

    public static void UVFxVerify_fnSetClientCond(Byte[] cond)
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            FxVerify_SetClientCond(fxVerifyinstance, cond);
        }
    }


    public static void UVFxVerify_fnSetClientCode(Byte[] cond, int nLen)
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            FxVerify_SetClientCode(fxVerifyinstance, cond, nLen);
        }
    }

    public static void UVFxVerify_GetRetEncodeVerify(string strIp, int nDynamicKey, int nPort,
        Byte[] strInfo, int nStrInfoLen, string strDecID, int nAddress, ref Byte[] output)
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            IntPtr p = Marshal.AllocHGlobal(16);
            FxVerify_GetRetEncodeVerify(fxVerifyinstance, strIp, nDynamicKey, nPort, strInfo, nStrInfoLen, strDecID, nAddress, p);
            Marshal.Copy(p, output, 0, 16);
            Marshal.FreeHGlobal(p);
        }
    }

    public static void UVFxVerify_GetLoginVerify(Byte[] strInfo, int nStrInfoLen, int nPort,
                                                       string strIp, string strDecID, int nAddress, int nDynamicKey, ref Byte[] output)
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            try
            {
                IntPtr p = Marshal.AllocHGlobal(16);
                FxVerify_GetLoginVerify(fxVerifyinstance, strInfo, nStrInfoLen, nPort, strIp, strDecID, nAddress, nDynamicKey, p);
                Marshal.Copy(p, output, 0, 16);
                Marshal.FreeHGlobal(p);
            }
            catch (Exception ex)
            {
                LogSystem.LogError(ex.ToString());
            }
        }
    }

    public static void UVFxVerify_GetChooseRoleVerify(Byte[] strInfo, int nStrInfoLen,
                                                            string strIp, int nPort, string strDecID, int nDynamicKey, byte[] szRoleName, int nRoleNameLen, int nAddress, ref Byte[] output)
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            IntPtr p = Marshal.AllocHGlobal(16);
            FxVerify_GetChooseRoleVerify(fxVerifyinstance, strInfo, nStrInfoLen, strIp, nPort, strDecID, nDynamicKey, szRoleName, nRoleNameLen, nAddress, p);
            Marshal.Copy(p, output, 0, 16);
            Marshal.FreeHGlobal(p);
        }
    }

    public static void UVFxVerify_GetSelectVerify(int nDynamicKey, int nFunctionId,
                                                       string strIp, int nPort, string strDecID, int nPersistidSerial, int nSerial, int nPersistidIdent, ref Byte[] output)
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            IntPtr p = Marshal.AllocHGlobal(16);
            FxVerify_GetSelectVerify(fxVerifyinstance, nDynamicKey, nFunctionId, strIp, nPort, strDecID, nPersistidSerial, nSerial, nPersistidIdent, p);
            Marshal.Copy(p, output, 0, 16);
            Marshal.FreeHGlobal(p);
        }
    }

    public static void UVFxVerify_GetCustomVerify(string strIp, int nDynamicKey, int nPort, int nSerial,
                                                        int nArgNum, string strDecID, Byte[] pArgData, int nArgLen, ref Byte[] output)
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            IntPtr p = Marshal.AllocHGlobal(16);
            FxVerify_GetCustomVerify(fxVerifyinstance, strIp, nDynamicKey, nPort, nSerial, nArgNum, strDecID, pArgData, nArgLen, p);
            Marshal.Copy(p, output, 0, 16);
            Marshal.FreeHGlobal(p);
        }
    }

    public static bool UVFxVerify_EncodeAccount(string strAccount, ref Byte[] output, ref int nSize)
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            IntPtr p = Marshal.AllocHGlobal(strAccount.Length + 16+4);
            nSize = strAccount.Length + 16+4;
            bool ret = FxVerify_EncodeAccount(fxVerifyinstance, strAccount, p, ref nSize);
            output = new byte[nSize+4];
            Marshal.Copy(p, output, 0, nSize);
            Marshal.FreeHGlobal(p);
            return ret;
        }

        return false;
    }

    public static bool UVFxVerify_EncodePassword(string strPassword, ref Byte[] output, ref int nSize)
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            IntPtr p = Marshal.AllocHGlobal(strPassword.Length + 16);
            nSize = strPassword.Length + 16;
            bool ret = FxVerify_EncodePassword(fxVerifyinstance, strPassword, p, ref nSize);
            output = new byte[nSize+4];
            Marshal.Copy(p, output, 0, nSize);
            Marshal.FreeHGlobal(p);
            return ret;
        }

        return false;
    }

    public static void UVFxVerify_Release()
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            FxVerify_Release(fxVerifyinstance);
            fxVerifyinstance = IntPtr.Zero;
            fxVerifyType = IntPtr.Zero;
        }
    }
    public static void UVFxVerify_AntiDebuggerCheck()
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            FxVerify_AntiDebuggerCheck(fxVerifyinstance);
        }
    }
    public static void UVFxVerify_AntiDebuggerStop()
    {
        if (fxVerifyinstance != IntPtr.Zero)
        {
            FxVerify_AntiDebuggerStop(fxVerifyinstance);
        }
    }
}