using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

/*********************************************************************************************************
 * 功能 : NPC
 **********************************************************************************************************/
public class NPCParser : UnitParser
{
    override public UnityEngine.Object Instantiate(UnityEngine.Object prefab)
    {
        GameObject ins = UnityEngine.Object.Instantiate(prefab) as GameObject;

        return ins;
    }

    /******************************************************************************
     * 功能 : 单位是否支持特殊存储, 部分策划配置对象,如NPC需要XML存储
     *******************************************************************************/
    override public bool SpecialStorage()
    {
        return true;
    }

    override public UnitParser Clone()
    {
        NPCParser parser = new NPCParser();

        return parser;
    }
}