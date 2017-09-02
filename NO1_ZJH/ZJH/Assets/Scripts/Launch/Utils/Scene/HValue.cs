using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using UnityEngine;

/***************************************************************************************************
 * 功能 ：为A* 算法准备
 ***************************************************************************************************/
public class HValue
{
    /** 0 没有用于计算路径, 1 已经在路径点中计算, 2 路径点有更优的计算,替代路径  */
	public short Mark;

    public float Key;                                 // 总开销
    public float Cost;                                // 寻路开销

	public short X;                                   // 格子坐标
	public short Y;

	public short PreX;                                // -1 为未引用任何
	public short PreY;

    /**************************************************************************************
     * 功能 ： 设置格子的数据
     **************************************************************************************/
	public void Set(short x, short y, float cost, float key)
    {
        this.X = x;
        this.Y = y;
        this.Cost = cost;
        this.Key = key;
    }

    /************************************************************************************
     * 功能 ：重置格子数据
     *************************************************************************************/
    public void Reset()
    {
        this.Mark = -1;
        // Previous = null;
        Cost = 10000000;
        Key = 10000000;
        X = 0;
        Y = 0;
        PreX = -1;
        PreY = -1;
    }

    /********************************************************************************
     * 功能 ： 比较两个格子的寻路开销
     *************************************************************************************/
    public static bool Compare(HValue left, HValue right)
    {
        if (Math.Abs(right.Key - left.Key) < 0.1f)
        {
            if (Math.Abs(right.Cost - left.Cost) < 0.1f)
                return false;
            else
                return left.Cost > right.Cost;
        }
        else
            return left.Key < right.Key;
    }
}