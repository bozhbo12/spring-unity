using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

/***********************************************************************************************
 * 功能 ： 水面数据
 ***********************************************************************************************/
public class WaterData
{
    /** 环境采样器ID */
    public int ambienceSamplerID;

    public float height = 0f;                                        // 水面高度

    public Vector4 waveSpeed = new Vector4(2f, 2f, -2f, -2f);
    public float waveScale = 0.02f;                                  // 水面缩放
    public Color horizonColor;                                       // 映射天边颜色

    public string depthMapPath = "";
    public string colorControlPath = "";
    public string waterBumpMapPath = "";

    public float waterVisibleDepth = 0f;                            // 水面能见深度

    public float waterDiffValue = 0f;

    public float reflDistort = 0.44f;                               // 水面反射值
    public float refrDistort = 0.2f;                                // 水面折射强度

    public Texture2D colorControlMap;                               // 水面颜色控制纹理
    public Texture2D bumpMap;                                       // 水面法线
    public Texture2D depthMap;
    public float alpha = 1f;

    public void Read(BinaryReader br)
    {
        long pos;
        this.ambienceSamplerID = br.ReadInt32();

        // 水面高度
        this.height = br.ReadSingle();

        // 写入水波速度
        this.waveSpeed.x = br.ReadSingle();
        this.waveSpeed.y = br.ReadSingle();
        this.waveSpeed.z = br.ReadSingle();
        this.waveSpeed.w = br.ReadSingle();

        // 写入水波缩放
        this.waveScale = br.ReadSingle();

        // 写入天际颜色
        this.horizonColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

        this.waterVisibleDepth = br.ReadSingle();

        this.waterDiffValue = br.ReadSingle();

        pos = br.BaseStream.Position;
        if (br.ReadInt32() == 10015)
        {
            this.alpha = br.ReadSingle();
        }
        else
        {
            br.BaseStream.Position = pos;
        }

        this.depthMapPath = br.ReadString();

        // 写入控制色纹理
        this.colorControlPath = br.ReadString();
        this.waterBumpMapPath = br.ReadString();

        this.colorControlMap = AssetLibrary.Load(colorControlPath, AssetType.Texture2D, LoadType.Type_Resources).texture2D;
        this.bumpMap = AssetLibrary.Load(waterBumpMapPath, AssetType.Texture2D, LoadType.Type_Resources).texture2D;
        this.depthMap = AssetLibrary.Load(this.depthMapPath, AssetType.Texture2D, LoadType.Type_Auto).texture2D;
    }

    /******************************************************************************
     * 功能 ： 释放水面数据
     ******************************************************************************/
    public void Release()
    {
        this.colorControlMap = null;
        this.bumpMap = null;
        this.depthMap = null;
    }

}