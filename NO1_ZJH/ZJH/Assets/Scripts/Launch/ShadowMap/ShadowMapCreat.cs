#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

public class ShadowMapCommand
{

    [MenuItem("GameObject/环境贴图创建/EnvironmentMap", false, 10)]
    static public void CreateShadowMap(MenuCommand menuCommand)
    {
        string shadowMapName = "ShadowMap";
        ShadowMapData sData = ScriptableObject.CreateInstance<ShadowMapData>();

        string uniquePath = AssetDatabase.GenerateUniqueAssetPath("Assets/" + shadowMapName + ".asset");
        AssetDatabase.CreateAsset(sData, uniquePath);
        LogSystem.Log(AssetDatabase.GetAssetPath(sData));

        GameObject shadowMapGO = new GameObject(shadowMapName, typeof(ShadowMapCreat));
        ShadowMapCreat shadowMapCreat = shadowMapGO.GetComponent<ShadowMapCreat>();
        shadowMapCreat.smData = sData;
        GameObjectUtility.SetParentAndAlign(shadowMapGO, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(shadowMapGO, "Create " + shadowMapGO.name);

        Selection.activeObject = shadowMapGO;
    }

}

[CustomEditor(typeof(ShadowMapCreat))]
public class ShadowMapCreatEditor : Editor
{
    void OnEnable()
    { }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ShadowMapCreat shadowMap = target as ShadowMapCreat;

        if (GUILayout.Button("生成环境映射贴图"))
        {
            shadowMap.CreatShadowMap();
        }
    }
}


public class ShadowMapCreat : MonoBehaviour
{
    public Vector3 mPos;

    public int mSize = 1024;

    public int mHeight = 1;

    public int resolutionX = 1024;

    public int resolutionY = 1024;

    public ShadowMapData smData = new ShadowMapData();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        mPos = transform.position;
        Vector3 bl = mPos;
        Vector3 tl = new Vector3(mPos.x, mPos.y,mPos.z + mSize);
        Vector3 tr = new Vector3(mPos.x + mSize, mPos.y, mPos.z + mSize);
        Vector3 br = new Vector3(mPos.x + mSize, mPos.y, mPos.z);
        DrawLine(bl, tl);
        DrawLine(tl, tr);
        DrawLine(tr, br);
        DrawLine(br, bl);
        DrawLine(bl, tl, 0, mHeight);
        DrawLine(tl, tr, 0, mHeight);
        DrawLine(tr, br, 0, mHeight);
        DrawLine(br, bl, 0, mHeight);
        DrawLine(bl, bl, 1, mHeight);
        DrawLine(tl, tl, 1, mHeight);
        DrawLine(tr, tr, 1, mHeight);
        DrawLine(br, br, 1, mHeight);
    }

    private void DrawLine(Vector3 start, Vector3 end, int hOrV = 0, int height = 0)
    {
        Gizmos.color = Color.blue;
        if(hOrV == 0)
            Gizmos.DrawLine(start + new Vector3(0, height,0), end + new Vector3(0, height, 0));
        else if (hOrV == 1)
            Gizmos.DrawLine(start, end + new Vector3(0, height, 0));
    }

    public void CreatShadowMap()
    {
        string path = "Assets/ShadowMap.png";
        if (AssetDatabase.LoadMainAssetAtPath(path) != null)
        {
            AssetDatabase.DeleteAsset(path);
        }

        Texture2D expTex = new Texture2D(resolutionX, resolutionY, TextureFormat.RGB24, false);
        expTex.wrapMode = TextureWrapMode.Clamp;

        Color c = new Color();
        for (int i = 0; i < resolutionX; i++)
        {
            for (int j = 0; j < resolutionY; j++)
            {
                Vector3 proVector3 = new Vector3(mPos.x + i * mSize / resolutionX, 9999, mPos.z + j * mSize / resolutionY);

                c = RaycastChunk(proVector3);

                expTex.SetPixel(i, j, c);
            }
        }
        expTex.Apply();
        byte[] bytes = expTex.EncodeToPNG();

        AssetDatabase.Refresh();
        QFileUtils.CreateFile(path);
        QFileUtils.WriteBytes(path, bytes);
        AssetDatabase.Refresh();

        TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Default;
        importer.linearTexture = false;
        importer.isReadable = true;
        importer.grayscaleToAlpha = false;
        importer.alphaIsTransparency = false;
        importer.mipmapEnabled = false;
        importer.maxTextureSize = 1024;
        importer.compressionQuality = 100;
        importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
        importer.filterMode = FilterMode.Bilinear;
        importer.anisoLevel = 0;
        importer.SetPlatformTextureSettings("Android", importer.maxTextureSize, TextureImporterFormat.ETC_RGB4, (int)TextureCompressionQuality.Normal, false);
        importer.SetPlatformTextureSettings("iPhone", importer.maxTextureSize, TextureImporterFormat.PVRTC_RGB4, (int)TextureCompressionQuality.Best, false);
        AssetDatabase.ImportAsset(path);
        AssetDatabase.Refresh();

        smData.m_Pos = mPos;
        smData.m_Size = mSize;
        smData.shadowMap = (Texture2D)AssetDatabase.LoadMainAssetAtPath(path);
        EditorUtility.SetDirty(smData);
    }

    public Color RaycastChunk(Vector3 pos)
    {
        RaycastHit hit;
        Vector2 lightmapCoord;
        Vector4 lightmapScaleOffset = new Vector4(1, 1, 0, 0);
        Transform hitTransform;
        MeshRenderer mRenderer;
        int lightmapIndex;
        Texture2D lightmapFar;
        Color grayColor = new Color();

        if (Physics.Raycast(pos, Vector3.down, out hit, 9999f, 1 << LayerMask.NameToLayer("Ground")))
        {
            lightmapCoord = hit.lightmapCoord;
            hitTransform = hit.transform;
            mRenderer = hitTransform.GetComponent<MeshRenderer>();
            if (mRenderer != null)
            {
                lightmapIndex = mRenderer.lightmapIndex;
                lightmapFar = LightmapSettings.lightmaps[lightmapIndex].lightmapColor;

                if (lightmapFar != null)
                {
                    grayColor = lightmapFar.GetPixelBilinear(lightmapCoord.x, lightmapCoord.y);
                }
            }
        }

        return grayColor;
    }
}




















#endif
