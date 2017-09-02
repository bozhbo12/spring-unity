using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;


/****************************************************************************************************
 * Struct : Detail patch
 ****************************************************************************************************/

public struct DetailPatch	
{
    public Boolean dirty;
    public List<int> layerIndices;	
    public int[] numberOfObjects;

    public void DoDetailPatch(int patchSamples)
    {
        dirty = false;
        layerIndices = new List<int>(7);
        numberOfObjects = new int[patchSamples * patchSamples];
    }
}

/****************************************************************************************************
 * Enum : Detail render mode
 ****************************************************************************************************/

public enum DetailRenderMode
{
    kDetailBillboard = 0,	    // billboard
    kDetailMeshLit,			    // just a mesh, lit like everything else
    kDetailMeshGrass,		    // mesh (user supplied or generated grass crosses), waves in the wind
    kDetailRenderModeCount
}


/****************************************************************************************************
 * Struct : Detail prototype
 ****************************************************************************************************/

public struct KDetailPrototype
{
    public GameObject prototype;
    public Texture2D prototypeTexture;
    public int index;
    public string path;
    public float minWidth, maxWidth;		    
    public float minHeight, maxHeight;		
    public float noiseSpread;
    public float bendFactor;
    public Color healthyColor;
    public Color dryColor;
    public float lightmapFactor;
    public DetailRenderMode renderMode;
    public int usePrototypeMesh;

    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector2[] uvs;
    public Color[] colors;
    public int[] triangles;


    public void DoKDetailPrototype()
    {
        index = -1;

        path = "";
        prototype = null;
        prototypeTexture = null;

        healthyColor = new Color(210f / 255.0f, 255.0f / 255.0f, 133f / 255.0f, 1.0f);
        dryColor = new Color(205f / 255.0f, 188 / 255.0f, 26 / 255.0f, 1.0f);

        minWidth = 1.0f;
        maxWidth = 2.0f;
        minHeight = 1.0f;
        maxHeight = 2.0f;
        noiseSpread = 10.0f;
        bendFactor = 1.0f;
        lightmapFactor = 1.0f;
        renderMode = DetailRenderMode.kDetailBillboard;
        usePrototypeMesh = 0;

        vertices = null;
        normals = null;
        uvs = null;
        colors = null;
        triangles = null;
    }
};

/*********************************************************************************************************
 * Class : Detail database
 *********************************************************************************************************/
#if UNITY_EDITOR
public class DetailDataBase
{
    const int kClampedVertexCount = 50000;
    public int kResolutionPerPatch = 8;


    // ----------------------------------------------------------------------------------------------------------------------
    //  Property
    private int m_Resolution;
    private int m_ResolutionPerPatch;

    private bool m_IsPrototypesDirty;
    public DetailPatch [] m_Patches;
	public List<KDetailPrototype>	m_DetailPrototypes;
	private TerrainData			m_TerrainData;
	private int					m_PatchCount;
	private int					m_PatchSamples;
	private List<Vector3>		m_RandomRotations;
	public Texture2D		    m_AtlasTexture;

	private Color			    m_WavingGrassTint;
	private float				m_WavingGrassStrength;
	private float				m_WavingGrassAmount;
	private float				m_WavingGrassSpeed;

    private Rand			m_Random = new Rand();

	private List<Texture2D>     m_PreloadTextureAtlasData;
	private List<Rect>          m_PreloadTextureAtlasUVLayout = new List<Rect>();

    public Color WavingGrassTint = new Color(1f, 1f, 1f, 1f);
    public float WavingGrassStrength = 0.11f;
    public float WavingGrassAmount = 0.3f;
    public float WavingGrassSpeed = 5f;


    /***************************************************************************************************************
     * Function : Write data
     ***************************************************************************************************************/

    public byte[] Write()
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);

        // Write BaseData
        bw.Write(m_Resolution);
        bw.Write(m_ResolutionPerPatch);

        // Write DoKDetailPrototype
        bw.Write(m_DetailPrototypes.Count);
        for (int i = 0; i < m_DetailPrototypes.Count; i++)
        {
            KDetailPrototype detailPrototype = m_DetailPrototypes[i];
            bw.Write(detailPrototype.index);
            bw.Write(detailPrototype.path);

            bw.Write(detailPrototype.healthyColor.r);
            bw.Write(detailPrototype.healthyColor.g);
            bw.Write(detailPrototype.healthyColor.b);
            bw.Write(detailPrototype.healthyColor.a);

            bw.Write(detailPrototype.dryColor.r);
            bw.Write(detailPrototype.dryColor.g);
            bw.Write(detailPrototype.dryColor.b);
            bw.Write(detailPrototype.dryColor.a);

            bw.Write(detailPrototype.minWidth);
            bw.Write(detailPrototype.maxWidth);
            bw.Write(detailPrototype.minHeight);
            bw.Write(detailPrototype.maxHeight);
            bw.Write(detailPrototype.noiseSpread);
            bw.Write(detailPrototype.bendFactor);
            bw.Write(detailPrototype.lightmapFactor);
            bw.Write((int)detailPrototype.renderMode);
            bw.Write(detailPrototype.usePrototypeMesh);
        }

        // Write numberOfObjects
        bw.Write(m_Patches.Length);
        for (int i = 0; i < m_Patches.Length; i++)
        {
            bw.Write(m_Patches[i].layerIndices.Count);
            for (int k = 0; k < m_Patches[i].layerIndices.Count; k++)
            {
                bw.Write(m_Patches[i].layerIndices[k]);   
            }
            bw.Write(m_Patches[i].numberOfObjects.Length);
            for (int j = 0; j < m_Patches[i].numberOfObjects.Length; j++)
            {
                bw.Write(m_Patches[i].numberOfObjects[j]);
            }
        }

        return stream.ToArray();
    }

    /***************************************************************************************************************
     * Function : Read data
     ***************************************************************************************************************/

    public void Read(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        // Read BaseData
        m_Resolution = br.ReadInt32();
        m_ResolutionPerPatch = br.ReadInt32();

        SetDetailResolution(m_Resolution, m_ResolutionPerPatch);

        // --------------------------------------------------------------------------------------------
        // Read DoKDetailPrototype
        int detailPrototypesCount = br.ReadInt32();
        List<KDetailPrototype> detailPrototypes = new List<KDetailPrototype>();
        for (int i = 0; i < detailPrototypesCount; i++)
        {
            KDetailPrototype detailPrototype = new KDetailPrototype();
            detailPrototype.DoKDetailPrototype();

            detailPrototype.index = br.ReadInt32();

            // detailPrototype.index = i;

            detailPrototype.path = br.ReadString();

            // detailPrototype.path = "Textures/Terrain/Detail_" + (detailPrototype.index + 1);

            detailPrototype.prototypeTexture = Resources.Load(detailPrototype.path, typeof(Texture2D)) as Texture2D;

            detailPrototype.healthyColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
            detailPrototype.dryColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

            detailPrototype.minWidth = br.ReadSingle();
            detailPrototype.maxWidth = br.ReadSingle();
            detailPrototype.minHeight = br.ReadSingle();
            detailPrototype.maxHeight = br.ReadSingle();
            detailPrototype.noiseSpread = br.ReadSingle();
            detailPrototype.bendFactor = br.ReadSingle();
            detailPrototype.lightmapFactor = br.ReadSingle();
            detailPrototype.renderMode = (DetailRenderMode)br.ReadInt32();
            detailPrototype.usePrototypeMesh = br.ReadInt32();

            detailPrototypes.Add(detailPrototype);
        }

        SetDetailPrototypes(detailPrototypes);

        // -----------------------------------------------------------------------------------------------------------
        // Write numberOfObjects
        int patchesCount = br.ReadInt32();
        for (int i = 0; i < patchesCount; i++)
        {
            int layCount = br.ReadInt32();
            for (int j = 0; j < layCount; j++)
            {
                int lay = br.ReadInt32();
                AddLayerIndex(lay, m_Patches[i]);
            }   
                
            int len = br.ReadInt32();
            for (int j = 0; j < len; j++)
            {
                m_Patches[i].numberOfObjects[j] = br.ReadInt32();
            }
        }
    }
    
    // ----------------------------------------------------------------------------------------------------------------------
    //  Function

    public int GetPatchCount () { return m_PatchCount; }

    private DetailPatch GetPatch (int x, int y) 
    { 
        return m_Patches[y * m_PatchCount + x]; 
    }

    private void ResetDirtyDetails()
    {
        for (int i = 0; i < m_Patches.Length; i++)
            (m_Patches[i]).dirty = false;
    }

    /*****************************************************************************************************************
     * Function : Dither table
     *****************************************************************************************************************/

    private float[] kDitherTable = {
	    1, 49, 13, 61, 4, 52, 16, 64,
	    33, 17, 45, 29, 36, 20, 48, 32,
	    9, 57, 5, 53, 12, 60, 8, 56,
	    41, 25, 37, 21, 44, 28, 40, 24,
	    3, 51, 15, 63, 2, 50, 14, 62,
	    35, 19, 47, 31, 34, 18, 46, 30,
	    11, 59, 7, 55, 10, 58, 6, 54,
	    43, 27, 39, 23, 42, 26, 38, 22,
    };

    /*****************************************************************************************************************
     * Function : Detail database
     *****************************************************************************************************************/

    public void DetailDatabase (TerrainData terrainData)
    {
	    m_TerrainData = terrainData;
	    m_WavingGrassTint = new Color (0.7f, 0.6f, 0.5f, 0.0f);
	    m_WavingGrassStrength = .5F;
	    m_WavingGrassAmount = .5F;
	    m_WavingGrassSpeed = .5F;
	    m_PatchCount = 0;
	    m_PatchSamples = kResolutionPerPatch;
	    m_IsPrototypesDirty = true;
	    m_AtlasTexture = null;
    }

    public Texture2D GetAtlasTexture()
    {
        return m_AtlasTexture;
    }

    public bool IsPatchEmpty (int x, int y) 
    {
        DetailPatch detailPatch = GetPatch(x, y);
        if (detailPatch.numberOfObjects == null || detailPatch.numberOfObjects.Length < 1)
            return true;
        return false;
    }


    public bool IsPatchDirty(int x, int y)
    {
	    return GetPatch(x,y).dirty;
    }


    private void SetDirty ()
    {
	    
    }

    public int GetWidth ()  { return GetResolution (); }
    public int GetHeight() { return GetResolution(); }
    public int GetResolution() { return m_PatchSamples * m_PatchCount; }
    public int GetResolutionPerPatch() { return m_PatchSamples; }


    int GetIndex (int x, int y) 
	{
		int res = m_PatchSamples;
		int nbIndex = y * res + x;
		return nbIndex;
	}

    // Fixes bounds of the patch of billboards. Since each quad making a billboard
    // has its vertices collapsed in one point, the bounding box does not take
    // the height nor the width of the billboard into account.
    void ExpandDetailBillboardBounds(Mesh mesh, float detailMaxHalfWidth, float detailMaxHeight)
    {
	    // The origin of the billboard is in the middle of the bottom edge.
	    Bounds aabb = mesh.bounds;

	    // The billboard always faces the camera, so when looking from the top
	    // it's the height of the billboard that extends in the XZ plane.
	    float maxHalfWidth = Mathf.Max(detailMaxHalfWidth, detailMaxHeight);

	    aabb.extents += new Vector3(maxHalfWidth, 0.5f * detailMaxHeight, maxHalfWidth);
	    aabb.center += new Vector3(0, 0.5f * detailMaxHeight, 0);
	    mesh.bounds = aabb;
    }

    /*****************************************************************************************************************
     * Function : Set detail resolution
     *****************************************************************************************************************/

    public void SetDetailResolution (int resolution, int resolutionPerPatch)
    {
        m_Resolution = resolution;
        m_ResolutionPerPatch = resolutionPerPatch;
	    m_PatchCount = Mathf.Clamp(resolution / resolutionPerPatch, 0, 10000);
	    m_PatchSamples = Mathf.Clamp(resolutionPerPatch, 8, 1000);
	
	    m_Patches = null;
        m_Patches = new DetailPatch[m_PatchCount * m_PatchCount];
        for (int i = 0; i < m_Patches.Length; i++)
        {
            m_Patches[i].DoDetailPatch(m_PatchSamples);
        }
        SetDirty();
    }

    void SetDetailPrototypesDirty ()
    {
	    m_IsPrototypesDirty = true;
    }


    /*****************************************************************************************************************
     * Function : Build mesh
     *****************************************************************************************************************/

    public Mesh BuildMesh (int patchX, int patchY, Vector3 size, int lightmapIndex, DetailRenderMode renderMode, float density)
    {
	    int totalTriangleCount, totalVertexCount;
        
	    DetailPatch patch = GetPatch (patchX, patchY);
	    ComputeVertexAndTriangleCount(patch, renderMode, density, out totalVertexCount, out totalTriangleCount);
	    if (totalTriangleCount == 0 || totalVertexCount == 0)
		    return null;
	    else
	    {
		    Mesh mesh = new Mesh();
		    GenerateMesh (mesh, patchX, patchY, size, lightmapIndex, renderMode, density, totalVertexCount, totalTriangleCount);
		    return mesh;
	    }
    }

    /*****************************************************************************************************************
     * Function : Compute vertex and triangle count
     *****************************************************************************************************************/

    void ComputeVertexAndTriangleCount(DetailPatch patch, DetailRenderMode renderMode, float density, out int vertexCount, out int triangleCount)
    {
	    triangleCount = 0;
	    vertexCount = 0;
	    int res = m_PatchSamples;
        
	    for (int i=0; i<patch.layerIndices.Count; i++)
	    {
		    KDetailPrototype prototype = m_DetailPrototypes[patch.layerIndices[i]];
		    if (prototype.renderMode != renderMode)
			    continue;
			
		    if (prototype.vertices == null)
			    continue;

		    int count = 0;
		    for (int y=0;y<res;y++)
		    {
			    for (int x=0;x<res;x++)
			    {
				    // int nbIndex = y * res + x;
                    int origCount = GetLayerNumbertOfObjects(patch.numberOfObjects[y * res + x], i);
				    if (origCount < 1)
					    continue;
				    int newCount = (int)(origCount * density + (kDitherTable[(x&7)*8+(y&7)] - 0.5f) / 64.0f);
				    count += newCount;
			    }
		    }
		
		    // Clamp the number of genrated details to not generate more than kClampedVertex vertices
		    int maxCount = (kClampedVertexCount - vertexCount) / prototype.vertices.Length;
		    count = Mathf.Min(maxCount, count);
		
		    triangleCount += prototype.triangles.Length * count;
		    vertexCount += prototype.vertices.Length * count;
	    }
    }

    /*****************************************************************************************************************
     * Function : Generate mesh
     *****************************************************************************************************************/

    void GenerateMesh (Mesh mesh, int patchX, int patchY, Vector3 size, int lightmapIndex, DetailRenderMode renderMode, float density, int totalVertexCount, int totalTriangleCount)
    {
	    DetailPatch patch = GetPatch (patchX, patchY);
	    Vector3[] vertices = new Vector3[totalVertexCount];
        Vector2[] uvs = new Vector2[totalVertexCount];
        Vector2[] uvs2 = new Vector2[totalVertexCount];

	    int tangentCount = 0;
	    if (renderMode == DetailRenderMode.kDetailBillboard)
		    tangentCount = totalVertexCount;
	    Vector4[] tangents = new Vector4[tangentCount];
	    Color[] colors = new Color[totalVertexCount];
	    int normalCount = totalVertexCount;
	    Vector3[] normals = new Vector3[normalCount];
	    int[] triangles = new int[totalTriangleCount];

	    int triangleCount = 0;
	    int vertexCount = 0;
	    float randomResolutionSize = 1.0f / GetResolution();
	    int res = m_PatchSamples;

        TerrainData terrainData = GameScene.mainScene.terrainData;
        int terWidth = GameScene.mainScene.terrainConfig.sceneWidth;
        int terHeight = GameScene.mainScene.terrainConfig.sceneHeight;

	    float detailMaxHalfWidth = 0.0f;
	    float detailMaxHeight = 0.0f;

        // ------------------------------------------------------------------------------------------------------------
        //  iterate layers
        for (int i = 0; i < patch.layerIndices.Count; i++)
	    {
		    KDetailPrototype prototype = m_DetailPrototypes[patch.layerIndices[i]];
		
		    if (prototype.renderMode != renderMode)
			    continue;
		    
		    float noiseSpread = prototype.noiseSpread;
		    Color dry = prototype.dryColor; 
		    Color healthy = prototype.healthyColor;
		
		    float halfGrassWidth = prototype.minWidth * 0.5f;
		    float halfGrassWidthDelta = (prototype.maxWidth - prototype.minWidth) * .5f;
		    float grassHeight = prototype.minHeight;
		    float grassHeightDelta = prototype.maxHeight - prototype.minHeight;
		    int prototypeTrisSize = prototype.triangles.Length;
		    int prototypeVerticesSize = prototype.vertices.Length;
		
		    if (prototypeVerticesSize == 0)
			    continue;

		    Vector3[] prototypeVertices = prototype.vertices.Length > 0 ? prototype.vertices : null;
            Vector3[] prototypeNormals = (prototype.normals != null && prototype.normals.Length > 0) ? prototype.normals : null;
		    Vector2[] prototypeUvs = prototype.uvs.Length > 0 ? prototype.uvs : null;
		    Color[] prototypeColors = prototype.colors.Length > 0 ? prototype.colors : null;
		    int[] prototypeTris = prototype.triangles.Length > 0 ? prototype.triangles : null;
             

		    for (int y=0;y<res;y++)
		    {
			    for (int x=0;x<res;x++)
			    {
				    // int nbIndex = y * res + x + i * res * res;
                   
                    // continue when data none
                    int origCount = GetLayerNumbertOfObjects(patch.numberOfObjects[y * res + x], i);
				    if (origCount == 0)
					    continue;
				
				    float nx = (float)patchX / m_PatchCount + (float)x / (res * m_PatchCount);
				    float ny = (float)patchY / m_PatchCount + (float)y / (res * m_PatchCount);
                    m_Random.SetSeed((UInt32)(y * res + x + (patchX * m_PatchCount + patchY) * 1013));
				
				    // Clamp the number of genrated details to not generate more than kClampedVertex vertices
				    int maxCount = (kClampedVertexCount - vertexCount) / prototypeVerticesSize;
				    origCount = Mathf.Min(maxCount, origCount);

				    int newCount = (int)(origCount * density + (kDitherTable[(x&7)*8+(y&7)] - 0.5f) / 64.0f);
				    for (int k=0;k<newCount;k++)
				    {
					    // Generate position & rotation
					
					    float normalizedX = nx + m_Random.GetFloat() * randomResolutionSize;
					    float normalizedZ = ny + m_Random.GetFloat() * randomResolutionSize;
					
					    // normalizedX = nx + 0.5F * randomSize;
					    // normalzedZ = ny + 0.5F * randomSize;
					    Vector3 pos = new Vector3();


                        pos.y = terrainData.GetHeight((int)(normalizedX * (float)terWidth), (int)(normalizedZ * (float)terHeight));
         
                        pos.x = normalizedX * size.x + i;
                        pos.z = normalizedZ * size.z + i;
					
					    float noise = PerlinNoise.NoiseNormalized(pos.x * noiseSpread, pos.z * noiseSpread);
                        Color healthyDryColor = Color.Lerp(dry, healthy, noise) * 1.4f;

					    // set second UVs to point to the fragment of the terrain lightmap underneath the detail mesh
					    CopyUVFromTerrain (normalizedX, normalizedZ, uvs2, vertexCount, prototypeVerticesSize);
					
					    if (renderMode == DetailRenderMode.kDetailBillboard)
					    {
						    if (prototypeVerticesSize != 4) return; 
						    if (prototypeTrisSize != 6) return;

						    float grassX = halfGrassWidth + halfGrassWidthDelta * noise;
						    float grassY = grassHeight + grassHeightDelta * noise;  
                            float grassZ = 0f;

                            float rot = noise * 123456789.9f;
                            grassX = Mathf.Cos(rot) * grassX;
                            grassZ = Mathf.Sin(rot) * grassX;

						    detailMaxHalfWidth = Mathf.Max(detailMaxHalfWidth, grassX);
						    detailMaxHeight = Mathf.Max(detailMaxHeight, grassY);
						    
						    Vector3[] billboardSize = 
						    { 
							    new Vector3 (-grassX, 0, -grassZ),
							    new Vector3 (-grassX, grassY, -grassZ),
							    new Vector3 (grassX, grassY, grassZ),
							    new Vector3 (grassX, 0, grassZ) 
						    };

                            Vector3[] rPos = 
                            {
                                new Vector3(pos.x - grassX, pos.y, pos.z - grassZ),
                                new Vector3(pos.x - grassX, pos.y + grassY, pos.z - grassZ),
                                new Vector3(pos.x + grassX, pos.y + grassY, pos.z + grassZ),
                                new Vector3(pos.x + grassX, pos.y, pos.z + grassZ)
                            };

                            CopyVertex(rPos, vertices, vertexCount, prototypeVerticesSize);
						    CopyUV (prototypeUvs, uvs, vertexCount, prototypeVerticesSize);
                            CopyColor (prototypeColors, colors, healthyDryColor, vertexCount, prototypeVerticesSize);
						    // used for offsetting vertices in the vertex shader
						    CopyTangents (billboardSize, tangents, vertexCount, prototypeVerticesSize);


                            CopyNormalFromTerrain(terrainData, normalizedX, normalizedZ, normals, vertexCount, prototypeVerticesSize);
						
						    for (int t=0;t<prototypeTrisSize;t++)
							    triangles[t+triangleCount] = prototypeTris[t] + vertexCount;
							
					        // increase tick
						    triangleCount += prototypeTrisSize;
						    vertexCount += prototypeVerticesSize;
					    }
					    else
					    {
                            
					    }
				    }
			    }
		    }
	    }

	    if (triangleCount != totalTriangleCount) return;
	    if (vertexCount != totalVertexCount) return;
	
	    // Assign the mesh
	    mesh.Clear(true);

        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.uv2 = uvs2;
        
	    if (renderMode == DetailRenderMode.kDetailBillboard)
		   mesh.tangents = tangents;

        mesh.triangles = triangles;
	    mesh.RecalculateBounds ();
	
	    if (renderMode == DetailRenderMode.kDetailBillboard)
		    ExpandDetailBillboardBounds(mesh, detailMaxHalfWidth, detailMaxHeight);

	    
    }
	
    
    /*********************************************************************************************************************
     * Function : Add layer index
     *********************************************************************************************************************/

    int AddLayerIndex (int detailIndex, DetailPatch patch)
    {
	    for (int i=0; i < patch.layerIndices.Count; i++)
	    {
		    if (patch.layerIndices[i] == detailIndex)
			    return i;
	    }
	    patch.layerIndices.Add(detailIndex);


        return patch.layerIndices.Count - 1;
    }

    /*********************************************************************************************************************
     * Function : Remove local layer index
     *********************************************************************************************************************/

    void RemoveLocalLayerIndex (int detailIndex, DetailPatch patch)
    {
	    if (detailIndex >= 0 || detailIndex < patch.layerIndices.Count)
        {
            int count = m_PatchSamples * m_PatchSamples;
            for (int i = 0; i < count; i++)
            {
                patch.numberOfObjects[i] = SetLayerNumberOfObjects(patch.numberOfObjects[i], detailIndex, 0);   
            }
            patch.layerIndices.Remove(detailIndex);
        }
    }

    int SetLayerNumberOfObjects(int value, int detailIndex, int num)
    {
        // 1 1 1 1
        if (num > 15)
            num = 15;
        // reserve 1 bit for sign
        int pos = detailIndex * 4;
        int sg1 = (value >> (pos + 4)) << (pos + 4);
        int sg2 = num << pos;
        int sg3 = ((value << (31 - pos)) & 0xfffffff) >> (31 - pos);
        return sg1 | sg2 | sg3;
    }

    int GetLayerNumbertOfObjects(int value, int detailIndex)
    {
        int pos = detailIndex * 4;
        return (value >> pos) & 15;
    }

    int RemoveLayerNumberOfObjects(int value, int detailIndex)
    {
        int pos = detailIndex * 4;

        int sg1 = (value >> (pos + 4)) << (pos + 4);
        int sg2 = sg1 >> 4;
        int sg3 = ((value << (31 - pos)) & 0xfffffff) >> (31 - pos);

        return sg2 | sg3;
    }
    
    /*********************************************************************************************************************
     * Function : Get supported layers
     *********************************************************************************************************************/

    public List<int> GetSupportedLayers (int xBase, int yBase, int totalWidth, int totalHeight)
    {
	    if( m_PatchCount <= 0 )
	    {
	        LogSystem.LogError ("Terrain has zero detail resolution");
		    return null;
	    }
        List<int> supportedLayers =  new List<int>();
 	    int prototypeCount = m_DetailPrototypes.Count;
	    List<int> enabledLayers = new List<int>(prototypeCount);
	    
        
	    int minPatchX = Mathf.Clamp(xBase / m_PatchSamples, 0, m_PatchCount - 1);
	    int minPatchY = Mathf.Clamp(yBase / m_PatchSamples, 0, m_PatchCount - 1);
	    int maxPatchX = Mathf.Clamp((xBase+totalWidth) / m_PatchSamples, 0, m_PatchCount - 1);
	    int maxPatchY = Mathf.Clamp((yBase+totalHeight) / m_PatchSamples, 0, m_PatchCount - 1);
	
	    for (int patchY=minPatchY;patchY<=maxPatchY;patchY++)
	    {
		    for (int patchX=minPatchX;patchX<=maxPatchX;patchX++)
		    {
			    int minX = Mathf.Clamp (xBase - patchX * m_PatchSamples, 0, m_PatchSamples - 1);
			    int minY = Mathf.Clamp (yBase - patchY * m_PatchSamples, 0, m_PatchSamples - 1);
	
			    int maxX = Mathf.Clamp (xBase + totalWidth - patchX * m_PatchSamples, 0, m_PatchSamples);
			    int maxY = Mathf.Clamp (yBase + totalHeight - patchY * m_PatchSamples, 0, m_PatchSamples);
	
			    int width = maxX - minX;
			    int height = maxY - minY;
			    if (width == 0 || height == 0)
				    continue;
	
			    DetailPatch patch = GetPatch(patchX, patchY);
			    for (int l=0;l<patch.layerIndices.Count;l++)
			    {
				    int layer = patch.layerIndices[l];
				    enabledLayers[layer] = 1;
			    }
		    }
	    }
	
	    int enabledCount = 0;
	    for (int i=0;i<prototypeCount;i++)
	    {
		    if (enabledLayers[i] > 0)
		    {
                supportedLayers.Add(i);
			    enabledCount++;
		    }
	    }
	
	    return supportedLayers;
    }

    /**************************************************************************************************************
     * Function : Get layer data
     **************************************************************************************************************/

    public int[,] GetLayer(int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex)
    {
	    if( m_PatchCount <= 0 )
	    {
		    LogSystem.LogError ("Terrain has zero detail resolution");
		    return null;
	    }
	    
        int[,] buffer = new int[totalWidth, totalHeight];

        // get patch range
	    int minPatchX = Mathf.Clamp(xBase / m_PatchSamples, 0, m_PatchCount - 1);
	    int minPatchY = Mathf.Clamp(yBase / m_PatchSamples, 0, m_PatchCount - 1);
	    int maxPatchX = Mathf.Clamp((xBase+totalWidth) / m_PatchSamples, 0, m_PatchCount - 1);
	    int maxPatchY = Mathf.Clamp((yBase+totalHeight) / m_PatchSamples, 0, m_PatchCount - 1);
	
	    for (int patchY=minPatchY;patchY<=maxPatchY;patchY++)
	    {
		    for (int patchX=minPatchX;patchX<=maxPatchX;patchX++)
		    {
                // data range in patch
			    int minX = Mathf.Clamp(xBase - patchX * m_PatchSamples, 0, m_PatchSamples - 1);
			    int minY = Mathf.Clamp(yBase - patchY * m_PatchSamples, 0, m_PatchSamples - 1);
	
			    int maxX = Mathf.Clamp(xBase + totalWidth - patchX * m_PatchSamples, 0, m_PatchSamples);
			    int maxY = Mathf.Clamp(yBase + totalHeight - patchY * m_PatchSamples, 0, m_PatchSamples);
	
			    int width = maxX - minX;
			    int height = maxY - minY;
			    if (width == 0 || height == 0)
				    continue;
	            
			    int xOffset = minX + patchX * m_PatchSamples - xBase;
			    int yOffset = minY + patchY * m_PatchSamples - yBase;
			    
			    DetailPatch patch = GetPatch(patchX, patchY);
			    
			    int[] numberOfObjects = patch.numberOfObjects;
			    for (int l=0;l<patch.layerIndices.Count;l++)
			    {
				    int layer = patch.layerIndices[l];
				    if (layer != detailIndex)
					    continue;
					
				    for (int y=0;y<height;y++)
				    {
					    for (int x=0;x<width;x++)
					    {
                            int nbOfObjects = GetLayerNumbertOfObjects(numberOfObjects[GetIndex(minX + x, minY + y)], l);
					
                            buffer[x + xOffset, y + yOffset] = nbOfObjects;
					    }
				    }
			    }
		    }
	    }

        return buffer;
    }
    
    /*********************************************************************************************************************
     * Function : Set layer
     *********************************************************************************************************************/

    public void SetLayer (int xBase, int yBase, int totalWidth, int totalHeight, int detailIndex, int[,] buffer)
    {
	    if (detailIndex >= m_DetailPrototypes.Count)
	    {
		    LogSystem.LogError ("Detail index out of bounds in DetailDatabase.SetLayers");
		    return;
	    }

	    if (m_PatchCount <= 0)
	    {
		    LogSystem.LogError ("Terrain has zero detail resolution");
		    return;
	    }

	    int minPatchX = Mathf.Clamp(xBase / m_PatchSamples, 0, m_PatchCount - 1);
	    int minPatchY = Mathf.Clamp(yBase / m_PatchSamples, 0, m_PatchCount - 1);
	    int maxPatchX = Mathf.Clamp((xBase+totalWidth) / m_PatchSamples, 0, m_PatchCount - 1);
	    int maxPatchY = Mathf.Clamp((yBase+totalHeight) / m_PatchSamples, 0, m_PatchCount - 1);
	
	    for (int patchY = minPatchY; patchY <= maxPatchY; patchY++)
	    {
		    for (int patchX=minPatchX;patchX<=maxPatchX;patchX++)
		    {
			    int minX = Mathf.Clamp(xBase - patchX * m_PatchSamples, 0, m_PatchSamples - 1);
			    int minY = Mathf.Clamp(yBase - patchY * m_PatchSamples, 0, m_PatchSamples - 1);
	
			    int maxX = Mathf.Clamp(xBase + totalWidth - patchX * m_PatchSamples, 0, m_PatchSamples);
			    int maxY = Mathf.Clamp(yBase + totalHeight - patchY * m_PatchSamples, 0, m_PatchSamples);
	
			    int width = maxX - minX;
			    int height = maxY - minY;
			    if (width == 0 || height == 0)
				    continue;
	
			    int xOffset = minX + patchX * m_PatchSamples - xBase;
			    int yOffset = minY + patchY * m_PatchSamples - yBase;
			
			    DetailPatch patch = GetPatch(patchX, patchY);
			
			    int localLayerIndex = AddLayerIndex(detailIndex, patch);
			    int[] numberOfObjects = patch.numberOfObjects;

			    for (int y=0;y<height;y++)
			    {
				    for (int x=0;x<width;x++)
				    {
					    
                        ushort nb = (ushort)Mathf.Clamp(buffer[x + xOffset, y + yOffset], 0, 255);
					    int nbIndex = GetIndex(minX + x, minY + y);
                        int numOfObjects = patch.numberOfObjects[nbIndex];
                        int num = GetLayerNumbertOfObjects(numOfObjects, localLayerIndex);
                        if (nb != num)
					    {
                            patch.numberOfObjects[nbIndex] = SetLayerNumberOfObjects(numOfObjects, localLayerIndex, nb);
               
						    patch.dirty = true;
					    }
				    }
			    }
			
			    // Detect if this patch has zero details on this layer
			    // In that case delete the layer completely to save space
			    int hasSomething = 0;
			    int oneLayerSampleCount = m_PatchSamples * m_PatchSamples;
			    for (int i=0;i<oneLayerSampleCount;i++)
                    hasSomething += GetLayerNumbertOfObjects(numberOfObjects[i], localLayerIndex);
			
			    if (hasSomething == 0)
				    RemoveLocalLayerIndex((ushort)localLayerIndex, patch);
		    }	
	    }
	    SetDirty ();
	
	    // update renderer
	    ResetDirtyDetails();
    }
    
    /**********************************************************************************************************
     * Function : Cleanup prototype
     **********************************************************************************************************/

    void CleanupPrototype (KDetailPrototype proto, string error)
    {
	    proto.vertices = null;
        proto.uvs = null;
        proto.colors = null;
        proto.triangles = null;
    }

    void OnDestroy ()
    {
	    /// DestroySingleObject(m_AtlasTexture);
    }

    // For thread loading we need to know the textures. Going through the meshes then textures is not thread safe.
    void SetupPreloadTextureAtlasData ()
    {
	    m_PreloadTextureAtlasData = new List<Texture2D>(m_DetailPrototypes.Count);

        Texture2D[] sourceTextures = new Texture2D[m_DetailPrototypes.Count];
	
	    RefreshPrototypesStep1(ref sourceTextures);

	    for (int i=0; i < m_DetailPrototypes.Count; i++)
	    {
            m_PreloadTextureAtlasData.Add(sourceTextures[i]);
		    if (sourceTextures[i] == null)
		    {
                LogSystem.LogWarning("Missing detail texture in Terrain, degraded loading performance");
			    m_PreloadTextureAtlasData.Clear();
			    break;
		    }
	    }
	
	    SetDetailPrototypesDirty();
    }

    void GenerateTextureAtlasThreaded ()
    {
	    if (m_PreloadTextureAtlasData != null)
	    {
            Texture2D[] sourceTextures = new Texture2D[m_PreloadTextureAtlasData.Count];

            int i;
            for (i = 0; i < m_PreloadTextureAtlasData.Count; i++)
            {
                Texture2D tex = m_PreloadTextureAtlasData[i];
                if (tex == null)
                    break;
                sourceTextures[i] = tex;
            }

            if (i == m_PreloadTextureAtlasData.Count)
            {
                m_AtlasTexture = new Texture2D(1024, 1024, TextureFormat.BGRA32, false, true);
                m_PreloadTextureAtlasUVLayout = (m_AtlasTexture.PackTextures(m_PreloadTextureAtlasData.ToArray(), 0, 1024)).ToList();
            }
	    }
    }

    /*****************************************************************************************************************
     * Function : Refresh prototypes step1
     *****************************************************************************************************************/

    void RefreshPrototypesStep1(ref Texture2D[] sourceTextures)
    {
	    for (int i=0;i<m_DetailPrototypes.Count;i++)
	    {
		    KDetailPrototype proto = m_DetailPrototypes[i];
		    sourceTextures[i] = null;
		
		    GameObject prototype = proto.prototype;
		    if (proto.usePrototypeMesh == 1 && prototype != null)
		    {
			    Renderer renderer = prototype.GetComponent<Renderer>();
			    Material sharedMaterial = renderer.sharedMaterial;
			    MeshFilter filter = prototype.GetComponent<MeshFilter>();	
			    Mesh mesh = filter.sharedMesh;

                proto.vertices = mesh.vertices;
                proto.uvs = mesh.uv;
                proto.colors = mesh.colors;
                proto.normals = mesh.normals;
                proto.triangles = mesh.triangles;
			
			    if (sharedMaterial)
				    sourceTextures[i] = sharedMaterial.mainTexture as Texture2D;
		    }
		    // We don't have a mesh, but we have a texture: it's grass quads
		    else if( proto.usePrototypeMesh == 0 && proto.prototypeTexture)
		    {
			    float halfWidth = 0.5F;
			    float height = 1.0F;
			    // color modifier at the top of the grass.
			    // billboard top vertex color = topColor * perlinNoise * 2
			    // Was 1.5f before we doublemultiplied
			    Color topColor = new Color(1f, 1f, 1f, 1f);
			    Color bottomColor = new Color(0.5f, 0.5f, 0.5f, 0f);

			    Vector3[] vertices = { 
				    new Vector3 (-halfWidth, 0, 0),
				    new Vector3 (-halfWidth, height, 0),
				    new Vector3 (halfWidth, height, 0),
				    new Vector3 (halfWidth, 0, 0),
			    };

			    Color[] colors = { bottomColor, topColor, topColor, bottomColor};
			    Vector2[] uvs = { new Vector2 (0, 0), new Vector2 (0, 1), new Vector2 (1, 1), new Vector2 (1, 0) };
			    int[] triangles = { 0, 1, 2, 2, 3, 0,  };

			    const int actualVertexCount = 4;
			    const int actualIndexCount = 6;

			    // skip normals creation, since they will be taken from the terrain in GenerateMesh()

                AssignVertices(out proto.vertices, vertices, actualVertexCount);
                AssignColors(out proto.colors, colors, actualVertexCount);
                AssignUVs(out proto.uvs, uvs, actualVertexCount);
                AssignTriangles(out proto.triangles, triangles, actualIndexCount);

                sourceTextures[i] = proto.prototypeTexture;
                m_DetailPrototypes[i] = proto;
		    }
	    }
    }

    private void AssignVertices(out Vector3[] dest,Vector3[] src, int count)
    { 
        dest = new Vector3[src.Length];
        for (int i = 0; i < count; i++)
        {
            dest[i] = src[i];
        }
    }

    private void AssignColors(out Color[] dest, Color[] src, int count)
    {
        dest = new Color[src.Length];
        for (int i = 0; i < count; i++)
        {
            dest[i] = src[i];
        }
    }

    private void AssignUVs(out Vector2[] dest, Vector2[] src, int count)
    {
        dest = new Vector2[src.Length];
        for (int i = 0; i < count; i++)
        {
            dest[i] = src[i];
        }
    }

    private void AssignTriangles(out int[] dest, int[] src, int count)
    {
        dest = new int[src.Length];
        for (int i = 0; i < count; i++)
        {
            dest[i] = src[i];
        }
    }

    /*****************************************************************************************************************
     * Function : Refresh prototypes
     *****************************************************************************************************************/

    public void RefreshPrototypes ()
    {
	    // Normal non-threaded creation mode
	    if (m_AtlasTexture == null)
	    {
            Texture2D[] sourceTextures = new Texture2D[m_DetailPrototypes.Count];
            RefreshPrototypesStep1(ref sourceTextures);

		    // Not created yet 
		    if (m_AtlasTexture == null)
			    m_AtlasTexture = new Texture2D(1024, 1024, TextureFormat.ARGB32, true, true);
		
		    // TODO: Make 4096 a property & clamp to GFX card, detail settings
		    List<Rect> rects = new List<Rect>(m_DetailPrototypes.Count);

            rects = m_AtlasTexture.PackTextures(sourceTextures.ToArray(), 0, 1024).ToList();
		    
            // scale and offset
		    for (int i=0;i<m_DetailPrototypes.Count;i++)
		    {
			    KDetailPrototype proto = m_DetailPrototypes[i];
			    Rect r = rects[i];
			    float w = r.width;
			    float h = r.height;
                if (proto.uvs != null)
                {
                    for (int v = 0; v < proto.uvs.Length; v++)
                    {
                        proto.uvs[v].x = proto.uvs[v].x * w + r.x;
                        proto.uvs[v].y = proto.uvs[v].y * h + r.y;
                    }
                }
		    }
	    }
	    // Generated in loading thread - Just upload
	    else
		{
            SetupPreloadTextureAtlasData();
			GenerateTextureAtlasThreaded();

            for (int i = 0; i < m_DetailPrototypes.Count; i++)
            {
                KDetailPrototype proto = m_DetailPrototypes[i];
                Rect r = m_PreloadTextureAtlasUVLayout[i];
                float w = r.width;
                float h = r.height;
                if (proto.uvs != null)
                {
                    for (int v = 0; v < proto.uvs.Length; v++)
                    {
                        proto.uvs[v].x = proto.uvs[v].x * w + r.x;
                        proto.uvs[v].y = proto.uvs[v].y * h + r.y;
                    }
                }
            }
	    }
	
	    m_IsPrototypesDirty = false;
    }

    /*****************************************************************************************************************
     * Funtion : Set detail prototypes
     *****************************************************************************************************************/
  
    public void SetDetailPrototypes (List<KDetailPrototype> detailPrototypes)
    {
	    m_DetailPrototypes = detailPrototypes;
	    RefreshPrototypes ();
	    SetDirty ();
    }
    
    /*****************************************************************************************************************
     * Funtion : Remvoe detail prototype
     *****************************************************************************************************************/

    public void RemoveDetailPrototype (int index)
    {
	    if( index < 0 || index >= m_DetailPrototypes.Count )
	    {
            LogSystem.LogWarning("invalid detail prototype index");
		    return;
	    }
	
	    // erase detail prototype
	    m_DetailPrototypes.RemoveAt(index);

        for (int i = 0; i < m_DetailPrototypes.Count; i++)
        {
            KDetailPrototype detailPrototype = m_DetailPrototypes[i];
            detailPrototype.index = i;
        }
	    // update detail patches
	    for( int i = 0; i < m_Patches.Length; ++i )
	    {
		    DetailPatch patch = m_Patches[i];
		    int localIndex = -1;
		    for( int j = 0; j < patch.layerIndices.Count; ++j )
		    {
			    if( patch.layerIndices[j] == index )
				    localIndex = j;
			    else if( patch.layerIndices[j] > index )
				    --patch.layerIndices[j];
		    }
		    if( localIndex == -1 )
			    continue;
		
		    // if ( patch.numberOfObjects.Length != patch.layerIndices.Count * m_PatchSamples * m_PatchSamples ) return;
		
		    patch.layerIndices.RemoveAt(localIndex);

            int cout = m_PatchSamples * m_PatchSamples;
            for (int k = 0; k < cout; k++)
            {
                patch.numberOfObjects[k] = RemoveLayerNumberOfObjects(patch.numberOfObjects[k], localIndex);
            }
	    }
	
	    RefreshPrototypes ();
	    SetDirty();
	}
    
    /*****************************************************************************************************************
     * Funtion :
     *****************************************************************************************************************/

    static void CopyVertex (Vector3[] src, Vector3[] dst, Matrix4x4 transform, int offset, int count)
    {
	    for (int i=0;i<count;i++)
		    dst[i+offset] = transform.MultiplyPoint(src[i]);
    }

    static void CopyVertex (Vector3[] pos, Vector3[] dst, int offset, int count)
    {
	    for (int i=0;i<count;i++)
            dst[i + offset] = pos[i];
    }

    /*****************************************************************************************************************
     * Funtion : Copy UV
     *****************************************************************************************************************/

    static void CopyUV (Vector2[] src, Vector2[] dst, int offset, int count)
    {
	    for (int i=0;i<count;i++)
		    dst[i+offset] = src[i];
    }
    
    static void CopyColor (Color[] src, Color[] dst, Color scale, int offset, int count)
    {
	    for (int i=0;i<count;i++)
		    dst[i+offset] = src[i] * scale;
    }

    /*****************************************************************************************************************
     * Funtion :
     *****************************************************************************************************************/

    static void CopyNormal (Vector3[]  src, Vector3[] dst, Quaternion rot, int offset, int count)
    {
	    for (int i=0;i<count;i++)
		    dst[i+offset] = rot * src[i];
    }
    
    
    /*****************************************************************************************************************
     * Funtion : 
     *****************************************************************************************************************/

    static void CopyNormalFromTerrain (TerrainData terrainData, float normalizedX, float normalizedZ, Vector3[] dst, int offset, int count)
    {
	    Vector3 terrainNormal = terrainData.GetInterpolatedNormal(normalizedX, normalizedZ);
	
	    for (int i = 0; i < count; i++)
		    dst[i+offset] = terrainNormal;
    }

    
    
    static void CopyTangents (Vector3[] src, Vector4[] dst, int offset, int count)
    {
	    for (int i = 0; i < count; i++)
	    {
            dst[i + offset].x = src[i].x;
            dst[i + offset].y = src[i].y;
            dst[i + offset].z = src[i].z;
	    }
    }
    
    /*****************************************************************************************************************
     * Funtion : 
     *****************************************************************************************************************/

    static void CopyUVFromTerrain (float normalizedX, float normalizedZ, Vector2[] dst, int offset, int count)
    {
	    Vector2 lightmapUV = new Vector2(normalizedX, normalizedZ);
	    for (int i = 0; i < count; i++)
		    dst[i+offset] = lightmapUV;
    }

    Color MultiplyDouble (Color inC0, Color inC1)
    {
	    return new Color (
		    Mathf.Min (((int)inC0.r * (int)inC1.r) / 128, 255), 
		    Mathf.Min (((int)inC0.g * (int)inC1.g) / 128, 255), 
		    Mathf.Min (((int)inC0.b * (int)inC1.b) / 128, 255), 
		    Mathf.Min (((int)inC0.a * (int)inC1.a) / 128, 255)
	    );
    }

    /*****************************************************************************************************************
     * Function : 
     *****************************************************************************************************************/

    void UpdateDetailPrototypesIfDirty ()
    {
	    if (m_IsPrototypesDirty)
		    RefreshPrototypes();
    }


}
#endif
