using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

/*********************************************************************************************************
 * 功能 : 环境采样球
 **********************************************************************************************************/
public class AmbienceSphereParser : UnitParser
{
    override public UnityEngine.Object Instantiate(UnityEngine.Object prefab)
    {
        GameObject ins = UnityEngine.Object.Instantiate(prefab) as GameObject;

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        MeshFilter sm = sphere.GetComponent<MeshFilter>();
        ins.AddComponent<MeshFilter>().sharedMesh = sm.sharedMesh;
        ins.AddComponent<MeshCollider>();

        Material mat = new Material(Shader.Find("Snail/Diffuse"));

        ins.AddComponent<MeshRenderer>().sharedMaterial = mat;
        ins.GetComponent<MeshRenderer>().castShadows = false;
        ins.GetComponent<MeshRenderer>().receiveShadows = false;

        unit.localScale = new Vector3(5f, 5f, 5f);

        ins.AddComponent<AmbienceSampler>();

        // Debug.Log("解析特效");
        GameObject.DestroyImmediate(sphere);

        return ins;
    }

    override public UnitParser Clone()
    {
        AmbienceSphereParser parser = new AmbienceSphereParser();

        return parser;
    }
}