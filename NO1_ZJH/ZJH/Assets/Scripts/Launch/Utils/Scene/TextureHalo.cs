using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;

/***********************************************************************************************
 * 类 : 纹理太阳
 ***********************************************************************************************/

[ExecuteInEditMode]
public class TextureHalo : MonoBehaviour
{
    public Color color;
    public Texture texture;
    public float size = 1f;

    public Vector3[] vertices;
    public Color[] colors;
    public Vector2[] uvs;

    private Material material;

    private int tick = 0;

    void Start()
    {
		return;
        const int kHaloVertices = 21;

        vertices = new Vector3[kHaloVertices];
        colors = new Color[kHaloVertices];
        uvs = new Vector2[kHaloVertices];

        material = new Material(Shader.Find("Snail/Grid"));

        TextureHaloDraw.halos.Add(this);
    }

    void Update()
    {
		return;
        Camera camera = Camera.current;
        if (camera == null)
            return;

        if (camera.GetComponent<TextureHaloDraw>() == null)
            camera.gameObject.AddComponent<TextureHaloDraw>();

        Matrix4x4 mat = camera.worldToCameraMatrix;

        Vector3 v = mat.MultiplyPoint(transform.position);
        float s = size;

        Color c;
        if (v.z <= -s * 2.0f)
        {
            c = color;
        }
        else
        {
            int fac = Mathf.RoundToInt((-v.z * 255.0f / s) - 255.0f);
            c = color * fac;
        }

        float z2 = v.z + s * 0.333f;

        int vbPtr = 0;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x - s, v.y, v.z); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(0.0f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x - s, v.y - s, v.z); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(0, 0); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y - s, v.z); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, 0); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x + s, v.y - s, v.z); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(1, 0); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x + s, v.y, v.z); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(1, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x + s, v.y + s, v.z); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(1, 1); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y + s, v.z); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, 1); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x - s, v.y + s, v.z); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(0, 1); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x - s, v.y, v.z); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(0.0f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;
        vertices[vbPtr] = new Vector3(v.x, v.y, z2); colors[vbPtr] = c; uvs[vbPtr] = new Vector2(.5f, .5f); ++vbPtr;

        /**
         * 
        GL.PushMatrix(); 
        // GL.LoadPixelMatrix();
        GL.Begin(GL.TRIANGLE_STRIP);

        for (int i = 0; i < vertices.Length; i++)
        {
            GL.TexCoord2(uvs[i].x, uvs[i].y);
            GL.Vertex3(vertices[i].x, vertices[i].y, vertices[i].z);
            GL.Color(colors[i]);
        }
        GL.End();
        GL.PopMatrix();
        **/
       
        tick++;
    }

}

public class TextureHaloDraw : MonoBehaviour
{
    static public List<TextureHalo> halos = new List<TextureHalo>();

    void OnPostRender()
    {
        for (int i = 0; i < halos.Count; i++)
        {
            Vector3[] vertices = halos[i].vertices;
            Vector2[] uvs = halos[i].uvs;
            Color[] colors = halos[i].colors ;

            GL.PushMatrix();
            // GL.LoadPixelMatrix();
            GL.Begin(GL.TRIANGLE_STRIP);

            for (int j = 0; j < vertices.Length; j++)
            {
                GL.TexCoord2(uvs[j].x, uvs[j].y);
                GL.Vertex3(vertices[j].x, vertices[j].y, vertices[j].z);
                GL.Color(colors[j]);
            }
            GL.End();
            GL.PopMatrix();
        }
    }
}