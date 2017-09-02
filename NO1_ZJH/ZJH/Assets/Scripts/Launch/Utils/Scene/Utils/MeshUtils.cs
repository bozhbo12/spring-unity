using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshUtils
{
    static public Vector4 CalculaMesUV2Rect(Mesh mesh)
    {
        float maxX = 0f;
        float minX = 0f;
        float maxY = 0f;
        float minY = 0f;
        for (int i = 0; i < mesh.uv2.Length; i++)
        {
            Vector2 uv = mesh.uv2[i];
            if (uv.x > maxX)
                maxX = uv.x;
            if (uv.x < minX)
                minX = uv.x;
            if (uv.y > maxY)
                maxY = uv.y;
            if (uv.y < minY)
                minY = uv.y;
        }
        return new Vector4(minX, minY, (maxX - minX), (maxY - minY));
    }

}