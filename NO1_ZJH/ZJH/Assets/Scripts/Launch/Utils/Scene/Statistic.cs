using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*******************************************************************************
 * 功能 ： 单位类型
 *******************************************************************************/
public class Statistic
{
    static private List<Material> matList = new List<Material>();
    
    /** 统计材质 */
    static public void Push(UnityEngine.Object asset, AssetType type)
    {
        if (type == AssetType.Material)
        {
            Material mt = asset as Material;
            if (matList.Contains(mt) == false)
                matList.Add(mt);
        }
    }

    /** 资源使用情况 */
    static public int Useage(AssetType type)
    {
        if (type == AssetType.Material)
        {
            return matList.Count;
        }
        return 0;
    }

    /** 获取材质 */
    static public List<Material> materials
    {
        get {
            return matList;
        }
    }

    /** 清理 */
    static public void Clear()
    {
        matList = new List<Material>();
    }
}