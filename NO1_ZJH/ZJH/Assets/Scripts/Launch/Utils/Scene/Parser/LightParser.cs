using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

public class LightParser : UnitParser
{
    private Light light;
    public bool firstRun = true;

    /** 灯光类型 */
    public LightType type;
    public Color color;
    public float intensity;
    public LightShadows shadows;
    public float shadowStrength;
    public float shadowBias;
    public float shadowSoftness;
    public float shadowSoftnessFade;
    public float range;
    public float spotAngle;
    public int cullingMask;

    public override void Destroy()
    {
        light = null;
        base.Destroy();
    }

    override public void Update(GameObject ins)
    {
		if (ins.GetComponent<Light>() == null)
			return;
        light = ins.GetComponent<Light>();
        firstRun = false;
        type = light.type;
        color = light.color;    
        intensity = light.intensity;
        shadows = light.shadows;
        shadowStrength = light.shadowStrength;
        shadowBias = light.shadowBias;
        shadowSoftness = light.shadowSoftness;
        shadowSoftnessFade = light.shadowSoftnessFade;
        range = light.range;
        spotAngle = light.spotAngle;
        cullingMask = light.cullingMask;
    }

    override public UnityEngine.Object Instantiate(UnityEngine.Object prefab)
    {
        GameObject ins = UnityEngine.Object.Instantiate(prefab) as GameObject;
        light = ins.GetComponent<Light>();
		if (ins.GetComponent<Light>() == null)
			return ins;
        if (firstRun == false)
        {
            light.type = type;
            light.color = color;
            light.intensity = intensity;
            light.shadows = shadows;
            light.shadowStrength = shadowStrength;
            light.shadowBias = shadowBias;
            light.shadowSoftness = shadowSoftness;
            light.shadowSoftnessFade = shadowSoftnessFade;
            light.range = range;    
            light.spotAngle = spotAngle;
            light.cullingMask = cullingMask;
        }

        if (Application.isPlaying == false)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            MeshFilter sm = sphere.GetComponent<MeshFilter>();
            ins.AddComponent<MeshFilter>().sharedMesh = sm.sharedMesh;
            ins.AddComponent<MeshCollider>();
            GameObject.DestroyImmediate(sphere);
        }

        // 运行状态下
        if (LightmapSettings.lightmaps.Length > 0)
        {
            light.enabled = false;
            ins.SetActive(false);
        }

        return ins;
    }

    override public UnitParser Clone()
    {
        LightParser parser = new LightParser();
        parser.firstRun = false;
        parser.type = type;
        parser.color = color;
        parser.intensity = intensity;
        parser.shadows = shadows;
        parser.shadowStrength = shadowStrength;
        parser.shadowBias = shadowBias;
        parser.shadowSoftness = shadowSoftness;
        parser.shadowSoftnessFade = shadowSoftnessFade;
        parser.range = range;
        parser.spotAngle = spotAngle;
        parser.cullingMask = cullingMask;

        return parser;
    }

    override public void Read(BinaryReader br)
    {
        if (br == null) return;
        firstRun = false;

        type = (LightType)br.ReadInt32();
        color = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
        intensity = br.ReadSingle();
        shadows = (LightShadows)br.ReadInt32();
        shadowStrength = br.ReadSingle();
        shadowBias = br.ReadSingle();
        shadowSoftness = br.ReadSingle();
        shadowSoftnessFade = br.ReadSingle();
        range = br.ReadSingle();
        spotAngle = br.ReadSingle();
        cullingMask = br.ReadInt32();
    }

    override public void Write(BinaryWriter bw)
    {
        if (bw == null) return;
        bw.Write((int)type);
        bw.Write(color.r); bw.Write(color.g); bw.Write(color.b); bw.Write(color.a);
        bw.Write(intensity);
        bw.Write((int)shadows);
        bw.Write(shadowStrength);   
        bw.Write(shadowBias);
        bw.Write(shadowSoftness);
        bw.Write(shadowSoftnessFade);
        bw.Write(range);
        bw.Write(spotAngle);
        bw.Write(cullingMask);
    }
}