using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/********************************************************************************************
 * 类 : 蒙皮动画解析器
 ********************************************************************************************/
public class SkinnedMeshParser : ParserBase
{
    /** 解析后最终蒙皮 */
    public Mesh mesh;
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
    private string rootName = "";

    private MeshRenderer skinnedMeshRenderer;
    private Material mat;

    public SkinnedMeshParser()
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
            br = new BinaryReader( new MemoryStream(_data) );

            // int vertexCount = br.ReadInt32();
            /**
            _vertices = new Vector3[vertexCount];
            _normals = new Vector3[vertexCount];
            _uvs = new Vector2[vertexCount];
            **/
            _startedParsing = false;

            if (br.ReadString() != "skeletonModel")
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
                case "vertexs" :
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
                case "weights":
                    ParseWeights();
                    break;
                case "bindPoses":
                    ParseBindPoses();
                    break;
                case "bones":
                    ParseBones();
                    break;
                case "gos":
                    rootName = br.ReadString();
                    CreateGameObjects();
                    break;
                case "material":
                    ParseMainMaterial();
                    break;
                case "end":
                    endStream = true;
                    break;
            }

            return false;
        }

        buffIndex++;
        if (buffIndex == 1)
        {
            CreateVertexBuffer(); return false;
        }
        if (buffIndex == 2)
        {
            CreateUVBuffer(); return false;
        }
        if (buffIndex == 3)
        {
            CreateTriBuffer(); return false;
        }
        /**
        if (buffIndex == 4)
        {
            CreateWeightsBuffer(); return false;
        }
        if (buffIndex == 5)
        {
            CreateBones(); return false;
        }
        if (buffIndex == 6)
        {
            CreateBindposes(); return false;
        }
        **/
        // 
        // skinnedMeshRenderer.sharedMesh = mesh;
        mesh.RecalculateNormals();

        skinnedMeshRenderer.material = mat;
        MeshFilter mf = gos[rootName].AddComponent<MeshFilter>();
        mf.sharedMesh = mesh;
        // 

        return true;
    }

    private int buffIndex = 0;

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
    }

    /**********************************************************************************
     * 功能 : 解析权值
     **********************************************************************************/
    public void ParseWeights()
    {
        int len = br.ReadInt32();
        _weights = new BoneWeight[len];
        int i = 0;
        for (i = 0; i < len; i++)
        {
            BoneWeight weight = new BoneWeight();
            weight.weight0 = br.ReadSingle();
            weight.weight1 = br.ReadSingle();
            weight.weight2 = br.ReadSingle();
            weight.weight3 = br.ReadSingle();

            weight.boneIndex0 = br.ReadInt32();
            weight.boneIndex1 = br.ReadInt32();
            weight.boneIndex2 = br.ReadInt32();
            weight.boneIndex3 = br.ReadInt32();

            _weights[i] = weight;
        }
    }

    /**********************************************************************************
     * 功能 : 解析姿态
     **********************************************************************************/
    public void ParseBindPoses()
    {
        int len = br.ReadInt32();
        _bindPoses = new Matrix4x4[len];
        int i = 0;
        for (i = 0; i < len; i++)
        {
            Matrix4x4 pose = new Matrix4x4();
            pose[0] = br.ReadSingle();
            pose[1] = br.ReadSingle();
            pose[2] = br.ReadSingle();
            pose[3] = br.ReadSingle();

            pose[4] = br.ReadSingle();
            pose[5] = br.ReadSingle();
            pose[6] = br.ReadSingle();
            pose[7] = br.ReadSingle();

            pose[8] = br.ReadSingle();
            pose[9] = br.ReadSingle();
            pose[10] = br.ReadSingle();
            pose[11] = br.ReadSingle();

            pose[12] = br.ReadSingle();
            pose[13] = br.ReadSingle();
            pose[14] = br.ReadSingle();
            pose[15] = br.ReadSingle();

            _bindPoses[i] = pose;
        }
    }

    /**********************************************************************************
     * 功能 : 解析骨架
     **********************************************************************************/
    private GameObject CreateGameObjects()
    {
        string name = br.ReadString();
        GameObject ins = new GameObject();
        if (go == null)
            go = ins;

        ins.name = name;

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

        bool attachPoint = br.ReadBoolean();
        if (attachPoint == true)
        {
            //AttachmentPoint ap = ins.AddComponent<AttachmentPoint>();
            //ap.slot = (AttachmentPoint.Slot)br.ReadInt32();
        }

        gos.Add(name, ins);
        int childCount = br.ReadInt32();
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = CreateGameObjects();
            child.transform.parent = ins.transform;
        }
        return ins;
    }

    /**********************************************************************************
     * 功能 : 解析骨架
     **********************************************************************************/
    public void ParseBones()
    {
        int len = br.ReadInt32();
        int i = 0;
        _bones = new Transform[len];
        for (i = 0; i < len; i++)
        {
            string name = br.ReadString();
            if (gos.ContainsKey(name) == false)
                LogSystem.Log("miss " + name);
            _bones[i] = gos[name].transform;
            Transform tf = _bones[i];
            Vector3 localPosition = tf.localPosition;
            Quaternion localRotation = tf.localRotation;

            localPosition.x = br.ReadSingle();
            localPosition.y = br.ReadSingle();
            localPosition.z = br.ReadSingle();

            localRotation.x = br.ReadSingle();
            localRotation.y = br.ReadSingle();
            localRotation.z = br.ReadSingle();
            localRotation.w = br.ReadSingle();

            gos[name].transform.localPosition = localPosition;
            gos[name].transform.localRotation = localRotation;
        }
    }

    static private Shader skmShader = Shader.Find("Snail/Bumped Specular Point Light");

    /**********************************************************************************
     * 功能 : 解析主材质
     **********************************************************************************/
    public void ParseMainMaterial()
    {
        string texpath = br.ReadString();
        Texture2D tex = AssetLibrary.Load(texpath, AssetType.Texture2D, LoadType.Type_Resources).texture2D;
        mat = new Material(skmShader);
        mat.mainTexture = tex;
    }

    /**********************************************************************************
     * 功能 : 创建buffer
     **********************************************************************************/
    public void CreateVertexBuffer()
    {
        GameObject root = gos[rootName];
        if (root != null)
            skinnedMeshRenderer = root.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        mesh.vertices = _vertices;
    }
    public void CreateUVBuffer()
    {
        mesh.uv = _uvs;
    }
    public void CreateTriBuffer()
    {
        mesh.triangles = _triangles;
    }
    public void CreateWeightsBuffer()
    {
        mesh.boneWeights = _weights;
    }
    public void CreateBindposes()
    {
        mesh.bindposes = _bindPoses;
    }
    public void CreateBones()
    {
        // skinnedMeshRenderer.bones = _bones;
    }
}