using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 安全辅助接口
/// </summary>
public static class SafeHelper
{
    /// <summary>
    /// 测试委托方法指向对象是否有效
    /// </summary>
    /// <param name="oFuncTarget"></param>
    /// <returns></returns>
    public static bool IsTargetValid(System.Object oFuncTarget)
    {
        ///空的时候应该为静态委托，无对象
        if (oFuncTarget == null)
            return false;

        ///如果原先有对象，后被销毁，对象值会变为"null"
        if (oFuncTarget.Equals( null ) )
            return false;

        return true;
    }
    /// <summary>
    /// 测试测试对象是否为空
    /// </summary>
    /// <param name="oFuncTarget"></param>
    /// <returns></returns>
    public static bool IsObjectNull(System.Object oObject)
    {
        ///正常空对象测试
        if (oObject == null)
            return true;

        ///对象未销毁，但丢失引用，变为"null"
        if (oObject.Equals(null))
            return true;

        return false;
    }
    /// <summary>
    /// 测试测试对象是否为空
    /// </summary>
    /// <param name="oFuncTarget"></param>
    /// <returns></returns>
    public static bool IsObjectNotNull(System.Object oObject)
    {
        ///正常空对象测试
        if (oObject == null)
            return false;

        ///对象未销毁，但丢失引用，变为"null"
        if (oObject.Equals(null))
            return false;

        return true;
    }
}
