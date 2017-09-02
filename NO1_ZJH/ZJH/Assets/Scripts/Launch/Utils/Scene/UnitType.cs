using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*******************************************************************************
 * 功能 ： 单位类型
 *******************************************************************************/
public class UnitType
{   
    static public int UnitType_General = 0;                    // 一般显示单位
    static public int UnitType_Light = 1;                      // 灯光
    static public int UnitType_Camera = 2;                     // 摄像机
    static public int UnitType_Effect = 3;                     // 特效
    static public int UnitType_Sound = 4;                      // 声源
    static public int UnitType_AmbienceSphere = 5;             // 环境球
    static public int UnitType_Npc = 6;                        // Npc

    static public UnitParser GenUnitParser(int type)
    {
        if (type == UnitType_General)
            return new UnitParser();
        else if (type == UnitType_Light)
            return new LightParser();
        else if (type == UnitType_Effect)
            return new EffectParser();
        else if (type == UnitType_AmbienceSphere)
            return new AmbienceSphereParser();
        else if (type == UnitType_Npc)
            return new NPCParser();
        return null;
    }

    static public string GetTypeName(int type)
    {
        if (type == 1)
            return "灯光";
        if (type == 2)
            return "摄像机";
        if (type == 3)
            return "特效";
        if (type == 4)
            return "声源";
        if (type == 5)
            return "环境球";
        if (type == 6)
            return "Npc";
        return "普通类型";
    }

    static public int GetTypeByComponent(GameObject gameObject)
    {
        if (gameObject.GetComponent<Light>() != null)
            return UnitType_Light;
        if (gameObject.GetComponent<Camera>() != null)
            return UnitType_Camera;
        if (gameObject.GetComponent<AmbienceSampler>() != null)
            return UnitType_AmbienceSphere;
        if (gameObject.GetComponent<Npc>() != null)
            return UnitType_Npc;
        return -1;
    }

    static public int GetType(int lay)
    {
        if (lay == GameLayer.Layer_Light)
            return UnitType_Light;
        else if (lay == GameLayer.Layer_Camera)
            return UnitType_Camera;
        else if (lay == GameLayer.Layer_Effect)
            return UnitType_Effect;
        else if (lay == GameLayer.Layer_AmbienceSphere)
            return UnitType_AmbienceSphere;
        else if (lay == GameLayer.Layer_NPC)
            return UnitType_Npc;
        return UnitType_General;
    }
}