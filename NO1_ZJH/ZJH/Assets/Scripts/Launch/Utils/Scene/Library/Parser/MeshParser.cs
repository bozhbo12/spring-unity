using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/********************************************************************************************
 * 类 : 模型解析器
 ********************************************************************************************/
public class MeshParser : ParserBase
{
    public GameObject go;
    private Dictionary<string, GameObject> gos = new Dictionary<string, GameObject>();
    private bool _startedParsing = true;

    private Vector3[] _vertices;
    private Vector2[] _uvs;
    private Vector3[] _normals;
    private int[] _triangles;
    private BoneWeight[] _weights;
    private Matrix4x4[] _bindPoses;
    private Transform[] _bones;
    private BinaryReader br;
//    private string rootName = "";

    private Material mat;

    public MeshParser()
    {

    }

    private bool endStream = false;

    /**********************************************************************************
     * 功能 : 处理解析 
     **********************************************************************************/
    override public bool ProceedParsing()
    {
        if (_startedParsing == true)
        {
            br = new BinaryReader(new MemoryStream(_data));

            // int vertexCount = br.ReadInt32();
            /**
            _vertices = new Vector3[vertexCount];
            _normals = new Vector3[vertexCount];
            _uvs = new Vector2[vertexCount];
            **/
            _startedParsing = false;
            if (br.ReadString() != "model")
            {
                go = new GameObject("EmptyModel");
                endStream = true;
                return true;
            }

        }

        // 解析模型数据
        while (endStream == false)
        {
            string type = br.ReadString();
            switch (type)
            {
                case "gameobject":
                    CreateGameObjects();
                    break;
                case "mesh":
                    ParseMesh();
                    break;
                case "vertexs":
                    ParseVertex();
                    break;
                case "uvs":
                    ParseUV();
                    break;
                case "nomals":
                    ParseVertexNormal();
                    break;
                case "triangles":
                    ParseFace();
                    break;
                case "material":
                    ParseMainMaterial();
                    break;
                case "end" :
                    endStream = true;
                    break;
            }

            return false;
        }


        return true;
    }


    private GameObject curGo = null;

    /**********************************************************************************
     * 功能 : 解析骨架
     **********************************************************************************/
    private GameObject CreateGameObjects()
    {
        string name = br.ReadString();
        string id = br.ReadString();
        GameObject ins = new GameObject();
        curGo = ins;
        if (go == null)
            go = ins;

        ins.name = name;

        bool hasParent = br.ReadBoolean();
        if (hasParent)
        { 
            GameObject parent = gos[br.ReadString()];
            ins.transform.parent = parent.transform;
        }

        Vector3 pos = new Vector3();
        pos.x = br.ReadSingle();
        pos.y = br.ReadSingle();
        pos.z = br.ReadSingle();

        Quaternion rot = new Quaternion();
        rot.x = br.ReadSingle();
        rot.y = br.ReadSingle();
        rot.z = br.ReadSingle();
        rot.w = br.ReadSingle();

        Vector3 scale = new Vector3();
        scale.x = br.ReadSingle();
        scale.y = br.ReadSingle();
        scale.z = br.ReadSingle();

        ins.transform.localPosition = pos;
        ins.transform.localRotation = rot;
        ins.transform.localScale = scale;

        gos.Add(id, ins);

        return ins;
    }

    private MeshFilter meshFilter;
    private Mesh mesh;

    /**********************************************************************************
     * 功能 : 解析模型
     **********************************************************************************/
    public void ParseMesh()
    {
        meshFilter = curGo.AddComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.sharedMesh = mesh;
    }

    /**********************************************************************************
     * 功能 : 解析顶点
     **********************************************************************************/
    public void ParseVertex()
    {
        int len = br.ReadInt32();
        _vertices = new Vector3[len];
        int i = 0;
        for (i = 0; i < len; i++)
        {
            Vector3 vert = new Vector3();
            vert.x = br.ReadSingle();
            vert.y = br.ReadSingle();
            vert.z = br.ReadSingle();
            _vertices[i] = vert;
        }

        mesh.vertices = _vertices;
    }

    /**********************************************************************************
     * 功能 : 解析UV
     **********************************************************************************/
    public void ParseUV()
    {
        int len = br.ReadInt32();
        _uvs = new Vector2[len];
        int i = 0;
        for (i = 0; i < len; i++)
        {
            Vector2 uv = new Vector2();
            uv.x = br.ReadSingle();
            uv.y = br.ReadSingle();
            _uvs[i] = uv;
        }

        mesh.uv = _uvs;
    }

    /**********************************************************************************
     * 功能 : 解析顶点法线
     **********************************************************************************/
    public void ParseVertexNormal()
    {
       br.ReadInt32();

    }

    /**********************************************************************************
     * 功能 : 解析三角面
     **********************************************************************************/
    public void ParseFace()
    { 
        int len = br.ReadInt32();
        _triangles = new int[len];
        int i = 0; 
        for (i = 0; i < len; i++)
        {
            _triangles[i] = br.ReadInt32();
        }
        mesh.triangles = _triangles;
    }

    private MeshRenderer mr;

//    static private Shader skmShader = Shader.Find("Snail/Bumped Specular Point Light");

    /**********************************************************************************
     * 功能 : 解析主材质
     **********************************************************************************/
    public void ParseMainMaterial()
    {
        mr = curGo.AddComponent<MeshRenderer>();

        string mtName = br.ReadString();

        string shaderName = br.ReadString();

        Material mt = new Material(Shader.Find(shaderName));
        mt.name = mtName;

        // 主色
        string colorProName = br.ReadString();
        if (colorProName == "_TintColor")
        {
            mt.SetColor(colorProName, new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
        }
        else if (colorProName == "_Color")
        {
            mt.SetColor(colorProName, new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
        }

        string texpath = br.ReadString();
        Texture2D tex = AssetLibrary.Load(texpath, AssetType.Texture2D, LoadType.Type_Resources).texture2D;
        mt.mainTexture = tex;

        mr.sharedMaterial = mt;
    }

  
}