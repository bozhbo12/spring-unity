using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

public class EffectParser : UnitParser
{
    override public UnityEngine.Object Instantiate(UnityEngine.Object prefab)
    {
        GameObject ins = UnityEngine.Object.Instantiate(prefab) as GameObject;

        /**
        if (Application.isPlaying == false)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            MeshFilter sm = sphere.GetComponent<MeshFilter>();
            ins.AddComponent<MeshFilter>().sharedMesh = sm.sharedMesh;
            ins.AddComponent<MeshCollider>();
            ins.AddComponent<UnitGizmos>().iconFileName = "effect.png";

            // Debug.Log("解析特效");
            GameObject.DestroyImmediate(sphere);
        }
        **/
        return ins;
    }

    override public UnitParser Clone()
    {
        EffectParser parser = new EffectParser();

        return parser;
    }
}