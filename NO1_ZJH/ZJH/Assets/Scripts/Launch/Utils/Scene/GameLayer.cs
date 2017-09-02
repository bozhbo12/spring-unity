using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;

/***********************************************************************************************
 * 类 ：游戏层级管理
 ***********************************************************************************************/
public class GameLayer
{
    static public string Layer_Str_Ground = "Ground";                                   // 地面
    static public string Layer_Str_Builder = "Builder";                                 // 建筑物
    static public string Layer_Str_Builder_NO_Occlusion = "Builder_NO_Occlusion";       // 建筑物
    static public string Layer_Str_Plant = "Plant";                                     // 植物
    static public string Layer_Str_Water = "Water";                                     // 水面
    static public string Layer_Str_Occlusion_Water = "OcclusionWater";                  // 不可行走水面
    static public string Layer_Str_Player = "Player";                                   // 玩家   
    static public string Layer_Str_NPC = "NPC";                                         // NPC        
    static public string Layer_Str_Occlusion_Object = "Occlusion_Object";               // 遮挡装饰物
    static public string Layer_Str_NO_Occlusion_Object = "NO_Occlusion_Object";         // 非遮挡装饰物
    static public string Layer_Str_Light = "Light";                                     // 灯光
    static public string Layer_Str_Camera = "Camera";                                   // 摄像机
    static public string Layer_Str_Effect = "Effect";                                   // 特效
    static public string Layer_Str_AmbienceSphere = "AmbienceSphere";                   // 环境球
        
    static public int Layer_Ground = LayerMask.NameToLayer(Layer_Str_Ground);        
    static public int Layer_Builder = LayerMask.NameToLayer(Layer_Str_Builder);         
    static public int Layer_Builder_NO_Occlusion = LayerMask.NameToLayer(Layer_Str_Builder_NO_Occlusion);
    static public int Layer_Plant = LayerMask.NameToLayer(Layer_Str_Plant);
    static public int Layer_Water = LayerMask.NameToLayer(Layer_Str_Water);
    static public int Layer_Occlusion_Water = LayerMask.NameToLayer(Layer_Str_Occlusion_Water);     
    static public int Layer_Player = LayerMask.NameToLayer(Layer_Str_Player);
    static public int Layer_NPC = LayerMask.NameToLayer(Layer_Str_NPC); 
    static public int Layer_Occlusion_Object = LayerMask.NameToLayer(Layer_Str_Occlusion_Object);
    static public int Layer_NO_Occlusion_Object = LayerMask.NameToLayer(Layer_Str_NO_Occlusion_Object);
    static public int Layer_Light = LayerMask.NameToLayer(Layer_Str_Light);
    static public int Layer_Camera = LayerMask.NameToLayer(Layer_Str_Camera);
    static public int Layer_Effect = LayerMask.NameToLayer(Layer_Str_Effect);
    static public int Layer_AmbienceSphere = LayerMask.NameToLayer(Layer_Str_AmbienceSphere);


    static public int Mask_Ground = GetMask(Layer_Ground);
    static public int Mask_Builder = GetMask(Layer_Builder);
    static public int Mask_Builder_NO_Occlusion = GetMask(Layer_Builder_NO_Occlusion);
    static public int Mask_Plant = GetMask(Layer_Plant);
    static public int Mask_Water = GetMask(Layer_Water);
    static public int Mask_Occlusion_Water = GetMask(Layer_Occlusion_Water);
    static public int Mask_Player = GetMask(Layer_Player);
    static public int Mask_NPC = GetMask(Layer_NPC);
    static public int Mask_Occlusion_Object = GetMask(Layer_Occlusion_Object);
    static public int Mask_NO_Occlusion_Object = GetMask(Layer_NO_Occlusion_Object);
    static public int Mask_Light = GetMask(Layer_Light);
    static public int Mask_Camera = GetMask(Layer_Camera);
    static public int Mask_Effect = GetMask(Layer_Effect);
    static public int Mask_AmbienceSphere = GetMask(Layer_AmbienceSphere);

    /****************************************************************************************
     * 功能 : 获取层级掩码
     *****************************************************************************************/
    static public int GetMask(int lay)
    {
        return 1 << lay;
    }

    /****************************************************************************************
     * 功能 : 获取组合后的层级
     *****************************************************************************************/
    static public int GetCombineMask(String[] layers)
    {
        int lay;
        int i = 0;
        string layName = "";
        int mask = 0;
        for (i = 0; i < layers.Length; i++)
        {
            layName = layers[i];
            lay = LayerMask.NameToLayer(layName);
            // Debug.Log("lay" + lay);
            mask = mask | 1 << lay;
        }
        return mask;
    }


}