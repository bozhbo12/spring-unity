using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/************************************************************************************************************
 * Class : Detail patch render
 ************************************************************************************************************/
#if UNITY_EDITOR
class DetailPatchRender
{
    public GameObject ins;
    public Mesh  mesh;
	public bool inited;
    public bool dirty;
	public int x;
	public int y;
}

/************************************************************************************************************
 * Class : Detail renderer
 ************************************************************************************************************/

public class DetailRenderer
{
    private int GetLightmapIndex() { return m_LightmapIndex; }
    private void SetLightmapIndex(int value) { m_LightmapIndex = value; }

    private Vector3 m_TerrainSize;
    private int m_LightmapIndex;

    private float m_Time = 0f;
    private Vector3 m_Position;
    private TerrainData m_Database;
    private int kDetailRenderModeCount = 2;
    private List<Material> m_Materials = new List<Material>();

    private List<Dictionary<int, DetailPatchRender>> m_Patches = new List<Dictionary<int, DetailPatchRender>>();

    public DetailRenderer(TerrainData terrain, Vector3 position, int lightmapIndex)
    { 
	    m_Database = terrain;
	    m_Position = position;
	    m_LightmapIndex = lightmapIndex;

        m_TerrainSize = new Vector3(GameScene.mainScene.terrainConfig.sceneWidth, 0, GameScene.mainScene.terrainConfig.sceneHeight);
		
	    string[] shaders = {
		    "Snail/Details/WavingDoublePass",
            "Snail/Details/Vertexlit"
	    };

	    bool shaderNotFound = false;
	    for (int i = 0; i < kDetailRenderModeCount; i++)
	    {
            m_Patches.Add(new Dictionary<int,DetailPatchRender>());

		    Shader shader = Shader.Find(shaders[i]);
		    if (shader == null)
		    {
			    shaderNotFound = true;
			    shader = Shader.Find("Legacy Shaders/Diffuse");
		    }

            m_Materials.Add(new Material(shader));
	    }

	    if (shaderNotFound)
	    {
            LogSystem.LogWarning("Unable to find shaders used for the terrain engine. ");
	    }
        if (grassRoot == null)
            grassRoot = new GameObject("GrassRoot");
    }

    private GameObject grassRoot;

    DetailPatchRender GrabCachedPatch (int x, int y, int lightmapIndex, DetailRenderMode mode, float density)
    {

        Dictionary<int, DetailPatchRender> patches = m_Patches[(int)mode];
	    int index = x + y*m_Database.detailDatabase.GetPatchCount();
        if (patches.ContainsKey(index) == false)
            patches[index] = new DetailPatchRender();
	    DetailPatchRender render = patches[index];
	    if(render.inited == false)
	    {
            GameObject ins = new GameObject();
            ins.name = "grass_" + x + "_" + y;

            ins.transform.parent = grassRoot.transform;

            ins.transform.position = new Vector3(-GameScene.mainScene.terrainConfig.sceneHeight * 0.5f, 0f, -GameScene.mainScene.terrainConfig.sceneHeight * 0.5f);

		    render.x = x;   
		    render.y = y;
		    render.inited = true;
            render.ins = ins;
            render.ins.AddComponent<MeshFilter>();
            render.ins.AddComponent<MeshRenderer>();
            render.dirty = true;
           
	    }

        if (render.dirty == true)
        {
            m_Materials[0].mainTexture = m_Database.detailDatabase.GetAtlasTexture();
            render.mesh = m_Database.detailDatabase.BuildMesh(x, y, m_TerrainSize, lightmapIndex, mode, density);
            render.ins.GetComponent<MeshFilter>().sharedMesh = render.mesh;
            render.ins.GetComponent<MeshRenderer>().sharedMaterial = m_Materials[0];
            render.ins.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            render.ins.GetComponent<MeshRenderer>().receiveShadows = false;
            render.dirty = false;
        }
        
	    return render;
    }

    public void BuildMesh(Vector3 center, float viewDistance)
    {
        DetailDataBase detail = m_Database.detailDatabase;
        int patchCount = detail.GetPatchCount();
        if (patchCount == 0)
            return;

        center.x += m_TerrainSize.x * 0.5f;
        center.z += m_TerrainSize.z * 0.5f;

        int centerX = Mathf.RoundToInt(center.x * patchCount / m_TerrainSize.x);
        int centerY = Mathf.RoundToInt(center.z * patchCount / m_TerrainSize.z);

        int halfWidth = (int)(Mathf.Ceil(patchCount * (viewDistance / m_TerrainSize.x)) + 1);
        int halfHeight = (int)(Mathf.Ceil(patchCount * (viewDistance / m_TerrainSize.z)) + 1);

        int minx = centerX - halfWidth;
        if (minx < 0) minx = 0;
        if (minx > patchCount - 1) minx = patchCount - 1;

        int miny = centerY - halfHeight;
        if (miny < 0) miny = 0;
        if (miny > patchCount - 1) miny = patchCount - 1;

        int maxx = centerX + halfWidth;
        if (maxx < 0) maxx = 0;
        if (maxx > patchCount - 1) maxx = patchCount - 1;

        int maxy = centerY + halfHeight;
        if (maxy < 0) maxy = 0;
        if (maxy > patchCount - 1) maxy = patchCount - 1;


        for (int y = miny; y <= maxy; y++)
        {
            for (int x = minx; x <= maxx; x++)
            {
                if (detail.IsPatchEmpty(x, y))
                    continue;

                for (int i = 0; i < kDetailRenderModeCount; i++)
                {

                    Dictionary<int, DetailPatchRender> patches = m_Patches[i];
                    int index = x + y * m_Database.detailDatabase.GetPatchCount();
                    DetailPatchRender render = patches[index];
                    if (render != null)
                        render.dirty = true;

                    GrabCachedPatch(x, y, m_LightmapIndex, (DetailRenderMode)i, 1f);
                }
            }
        }
    }

    /*********************************************************************************************************************
     * Function : Detail render
     *********************************************************************************************************************/

    public void Render(float viewDistance, float detailDensity)
    {
        DetailDataBase detail = m_Database.detailDatabase;

        float sqrViewDistance = viewDistance * viewDistance;

        Transform camT = Camera.main.transform;

        Vector3 position = camT.position - m_Position;


        // get camera transform up
        Vector3 up = camT.TransformDirection(new Vector3(0.0f, 1.0f, 0.0f));
        up.z = 0;
        up = camT.TransformDirection(up);
        up = up.normalized;


        Vector3 right = Vector3.Cross(camT.TransformDirection(new Vector3(0.0f, 0.0f, -1.0f)), up);
        right = right.normalized;

        m_Time += 0.002f * detail.WavingGrassStrength;

        Shader.SetGlobalFloat("_MaxDistanceSqr", sqrViewDistance);

        Vector4 prop = new Vector4();
        prop[0] = position.x;
        prop[1] = position.y;
        prop[2] = position.z;
        prop[3] = 1.0f / sqrViewDistance;
        Shader.SetGlobalVector("_CameraPosition", prop);

        prop[0] = m_Time;
        prop[1] = detail.WavingGrassSpeed * 0.4f;
        prop[2] = detail.WavingGrassAmount * 6f;
        prop[3] = sqrViewDistance;
        Shader.SetGlobalVector("_WaveAndDistance", prop);

        prop[0] = right.x;
        prop[1] = right.y;
        prop[2] = right.z;
        prop[3] = 0.0f;
        Shader.SetGlobalVector("_CameraRight", prop);

        prop[0] = up.x;
        prop[1] = up.y;
        prop[2] = up.z;
        prop[3] = 0.0f;
        Shader.SetGlobalVector("_CameraUp", prop);

        int patchCount = detail.GetPatchCount();
        if (patchCount == 0)
            return;

        int minx = 0;
        int miny = 0;

        int maxx = patchCount - 1;
        int maxy = patchCount - 1;

        for (int y = miny; y <= maxy; y++)
        {
            for (int x = minx; x <= maxx; x++)
            {
                if (detail.IsPatchEmpty(x, y))
                    continue;

                for (int i = 0; i < kDetailRenderModeCount; i++)
                {
                    GrabCachedPatch(x, y, m_LightmapIndex, (DetailRenderMode)i, 1f);
                }
            }
        }

        return;

        
	
    }
}
#endif
