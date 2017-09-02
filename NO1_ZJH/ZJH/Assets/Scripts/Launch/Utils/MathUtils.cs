using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/***********************************************************************************
 * 功能 ： 数学方法
 ***********************************************************************************/
public class MathUtils
{
    static public bool FloatEquals(float a, float b)
    {
        if (Math.Abs(a - b) <= 1e-6)
            return true;
        return false;
    }

    static public bool FloatEquals(float a, float b, float offest)
    {
        if (Math.Abs(a - b) <= offest)
            return true;
        return false;
    }

    static public bool FloatZero(float a)
    {
        if (Math.Abs(a - 0f) <= 1e-6)
            return true;
        return false;
    }

    public static float Hermite(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
    }

    public static Vector3 Hermite(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Hermite(start.x, end.x, value), Hermite(start.y, end.y, value), Hermite(start.z, end.z, value));
    }


    public static float Sinerp(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, Mathf.Sin(value * Mathf.PI * 0.5f));
    }

    public static Vector3 Sinerp(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Sinerp(start.x, end.x, value), Sinerp(start.y, end.y, value), Sinerp(start.z, end.z, value));
    }

    public static float Coserp(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, 1.0f - Mathf.Cos(value * Mathf.PI * 0.5f));
    }
    public static Vector3 Coserp(Vector3 start, Vector3 end, float value)
    {
        return new Vector3(Coserp(start.x, end.x, value), Coserp(start.y, end.y, value), Coserp(start.z, end.z, value));
    }

    public static float Berp(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    public static float SmoothStep(float x, float min, float max)
    {
        x = Mathf.Clamp(x, min, max);
        float v1 = (x - min) / (max - min);
        float v2 = (x - min) / (max - min);
        return -2 * v1 * v1 * v1 + 3 * v2 * v2;
    }

    public static float Lerp(float start, float end, float value)
    {
        return ((1.0f - value) * start) + (value * end);
    }

    /***************************************************************************
     * 功能 ： 2D距离平方
     ***************************************************************************/
    static public float Distance2D(float x1, float y1, float x2, float y2)
    {
        float x = x2 - x1;
        float y = y2 - y1;
        return Mathf.Sqrt(x * x + y * y);
    }
    /***************************************************************************
     * 功能 ： 2D距离平方
     ***************************************************************************/
    static public float Distance2D2(float x1, float y1, float x2, float y2)
    {
        float x = x2 - x1;
        float y = y2 - y1;
        return x * x + y * y;
    }

    /***************************************************************************
     * 功能 ： 2D距离平方
     ***************************************************************************/
    static public float Distance2D2(Vector3 vPos1, Vector3 vPos2)
    {
        float x = vPos1.x - vPos2.x;
        float z = vPos1.z - vPos2.z;
        return x * x + z * z;
    }

    /// <summary>
    /// 功能 ： 2D平方距离
    /// </summary>
    static public float SqrMagnitude2D(float x1, float y1, float x2, float y2)
    {
        float x = x2 - x1;
        float y = y2 - y1;
        return x * x + y * y;
    }

    /// <summary>
    /// 功能 ： 2D平方距离
    /// </summary>
    static public float SqrMagnitude2D(Vector3 v1, Vector3 v2)
    {
        float x = v2.x - v1.x;
        float y = v2.z - v1.z;
        return x * x + y * y;
    }

    static public float Distance2D(Vector3 v1, Vector3 v2)
    {
        v1.y = v2.y;
        return Vector3.Distance(v1, v2);
    }

    /** 两个向量的距离小于等于 目标距离 */
    static public bool Distance2DLessEqual(float x1, float y1, float x2, float y2, float distance)
    {
        float x = x2 - x1;
        float y = y2 - y1;
        float sqrDis1 = x * x + y * y;
        float sqrDis2 = distance * distance;
        if (sqrDis1 <= sqrDis2)
            return true;
        return false;
    }

    /** 两个向量的距离小于等于 目标距离 */
    static public bool Distance2DLessEqual(Vector3 pos1, Vector3 pos2, float distance)
    {
        float x = pos2.x - pos1.x;
        float z = pos2.z - pos1.z;
        float sqrDis1 = x * x + z * z;
        float sqrDis2 = distance * distance;
        if (sqrDis1 <= sqrDis2)
            return true;
        return false;
    }

    /** 两个向量的距离大于等于 目标距离 */
    static public bool Distance2DMoreEqual(Vector3 pos1, Vector3 pos2, float distance)
    {
        float x = pos2.x - pos1.x;
        float z = pos2.z - pos1.z;
        float sqrDis1 = x * x + z * z;
        float sqrDis2 = distance * distance;
        if (sqrDis1 >= sqrDis2)
            return true;
        return false;
    }

    /***************************************************************************
    * 功能 ： 3D距离平方
    ***************************************************************************/
    static public float Distance3D2(Vector3 vPos1, Vector3 vPos2)
    {
        float x = vPos1.x - vPos2.x;
        float y = vPos1.y - vPos2.y;
        float z = vPos1.z - vPos2.z;
        return x * x + y * y + z * z;
    }

    /// <summary>
    /// 两个向量的距离小于等于 目标距离
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="z3"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    /// <param name="z3"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    static public bool Distacne3DLessEqual(float x1, float y1, float z1, float x2, float y2, float z2, float distance)
    {
        float x = x2 - x1;
        float y = y2 - y1;
        float z = z2 - z1;
        float sqrDis1 = x * x + y * y + z * z;
        float sqrDis2 = distance * distance;
        if (sqrDis1 <= sqrDis2)
            return true;
        return false;
    }

    /// <summary>
    /// 两个向量的距离小于等于 目标距离
    /// </summary>
    /// <param name="vPos1"></param>
    /// <param name="vPos2"></param>
    /// <param name="distance"></param>
    /// <returns></returns>
    static public bool Distacne3DLessEqual(Vector3 vPos1, Vector3 vPos2, float distance)
    {
        float sqrDis1 = (vPos2 - vPos1).sqrMagnitude;
        float sqrDis2 = distance * distance;
        if (sqrDis1 <= sqrDis2)
            return true;
        return false;
    }

    /** 计算正方形与圆形之间的碰撞 */
    static public bool Cube2CirIntersect(float gridX, float gridZ, float halfSideLen, Vector3 cirPosition, float radius)
    {
        float distance = Distance2D(gridX, gridZ, cirPosition.x, cirPosition.z);
        if (distance < radius + halfSideLen)
            return true;

        return false;
    }


    //弧度转角度
    public static float Rad2Deg(float rad)
    {
        return rad / Mathf.Deg2Rad;
    }
    //角度转弧度
    public static float Rad3Deg(float angle)
    {
        return angle * Mathf.Deg2Rad;
    }

    public static float Deg(float deg)
    {
        if (deg < 0)
        {
            deg *= -1;
            return 360 - deg % 360;
        }
        return deg % 360;
    }
    /// <summary>
    /// 找到a有b没有
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static List<string> ArrayDiff(string[] a, string[] b)
    {
        List<string> result = new List<string>();
        for (int i = 0; i < a.Length; i++)
        {
            for (int j = 0; j < b.Length; j++)
            {
                if (!b.Contains(a[i]))
                {
                    result.Add(a[i]);
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 4个byte拼接一个int型
    /// </summary>
    /// <param name="byteA"></param>
    /// <param name="byteB"></param>
    /// <param name="byteC"></param>
    /// <param name="byteD"></param>
    /// <returns></returns>
    public static uint SpellInt(sbyte byteA, sbyte byteB, sbyte byteC, sbyte byteD)
    {
        uint a = 0;
        a = (0xff000000 & (((uint)byteA) << 24)
            | 0x00ff0000 & (((uint)byteB) << 16)
            | 0x0000ff00 & (((uint)byteC) << 8)
            | 0x000000ff & (((uint)byteD) << 0));
        return a;  
    }

    /// <summary>
    /// 按8位分割int 
    /// </summary>
    /// <param name="uCode">待分割值</param>
    /// <param name="sCodeA">高位的高8位</param>
    /// <param name="sCodeB">高位的低8位</param>
    /// <param name="sCodeC">低位的高8位</param>
    /// <param name="sCodeD">低位的低8位</param>
    public static void SplitByte(uint uCode, ref sbyte sCodeA, ref sbyte sCodeB, ref sbyte sCodeC, ref sbyte sCodeD)
    {
        ushort uHighCodeA = (ushort)((uCode >> 24) & 0xff);
        ushort uHighCodeB = (ushort)((uCode >> 16) & 0xff);
        ushort uLowCodeA = (ushort)((uCode >> 8) & 0xff);
        ushort uLowCodeB = (ushort)((uCode ) & 0xff);

        sCodeA = (sbyte)uHighCodeA;
        sCodeB = (sbyte)uHighCodeB;
        sCodeC = (sbyte)uLowCodeA;
        sCodeD = (sbyte)uLowCodeB;
    }
    /// <summary>
    /// 按16位分割int
    /// </summary>
    /// <param name="uCode">待分割值</param>
    /// <param name="sCodeA">高16位</param>
    /// <param name="sCodeC">低16位</param>
    public static void SplitShort(uint uCode, ref short sHighCode, ref short sLowCode)
    {
        ushort uHighCode = (ushort)((uCode >> 16) & 0xffff);
        ushort uLowCode = (ushort)((uCode) & 0xffff);

        sHighCode = (short)uHighCode;
        sLowCode = (short)uLowCode;
    }

    /// <summary>
    /// 距离获取抛物线高度
    /// 已知　最大高度 最远位置
    /// y= -4h/s^2 * x^2 + 4h/s * x
    /// </summary>
    /// <param name="fJumpHeight">抛物线最高点</param>
    /// <param name="fJumpDistance">抛物线长度</param>
    /// <param name="distance">某点</param>
    /// <returns></returns>
    public static float GetParabolaHeight(float fJumpHeight, float fJumpDistance, float distance)
    {
        float fp1 = (-4 * fJumpHeight / (fJumpDistance * fJumpDistance)) * (distance * distance);
        float fp2 = 4 * fJumpHeight * distance / fJumpDistance;
        return fp1 + fp2;
    }

    static string[] strNumString = new string[]
    {
        "0","1","2","3","4","5","6","7","8","9",
        "10","11","12","13","14","15","16","17","18","19",
        "20","21","22","23","24","25","26","27","28","29",
        "30","31","32","33","34","35","36","37","38","39",
        "40","41","42","43","44","45","46","47","48","49",
        "50","51","52","53","54","55","56","57","58","59",
        "60","61","62","63","64","65","66","67","68","69",
        "70","71","72","73","74","75","76","77","78","79",
        "80","81","82","83","84","85","86","87","88","89",
        "90","91","92","93","94","95","96","97","98","99",
    };

    public static string GetIntString(int iNum, string strFormat = "#0")
    {
        if (iNum >= 0 && iNum < 100)
        {
            return strNumString[iNum];
        }
        return iNum.ToString(strFormat);
    }
    static string[] strTimeString = new string[]
    {
        "00","01","02","03","04","05","06","07","08","09",
        "10","11","12","13","14","15","16","17","18","19",
        "20","21","22","23","24","25","26","27","28","29",
        "30","31","32","33","34","35","36","37","38","39",
        "40","41","42","43","44","45","46","47","48","49",
        "50","51","52","53","54","55","56","57","58","59",
        "60","61","62","63","64","65","66","67","68","69",
        "70","71","72","73","74","75","76","77","78","79",
        "80","81","82","83","84","85","86","87","88","89",
        "90","91","92","93","94","95","96","97","98","99",
    };

    public static string GetIntTime(int iNum, string strFormat = "00")
    {
        if (iNum >= 0 && iNum < 100)
        {
            return strTimeString[iNum];
        }
        return iNum.ToString(strFormat);
    }
}