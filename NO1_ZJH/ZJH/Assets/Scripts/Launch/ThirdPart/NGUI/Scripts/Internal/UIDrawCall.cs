//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

//#define SHOW_HIDDEN_OBJECTS

using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// This is an internally-created script used by the UI system. You shouldn't be attaching it manually.
/// </summary>

[ExecuteInEditMode]
[AddComponentMenu("NGUI/Internal/Draw Call")]
public class UIDrawCall : MonoBehaviour
{
	static BetterList<UIDrawCall> mActiveList = new BetterList<UIDrawCall>();
	static BetterList<UIDrawCall> mInactiveList = new BetterList<UIDrawCall>();

	[System.Obsolete("Use UIDrawCall.activeList")]
	static public BetterList<UIDrawCall> list { get { return mActiveList; } }

	/// <summary>
	/// List of active draw calls.
	/// </summary>

	static public BetterList<UIDrawCall> activeList { get { return mActiveList; } }

	/// <summary>
	/// List of inactive draw calls. Only used at run-time in order to avoid object creation/destruction.
	/// </summary>

	static public BetterList<UIDrawCall> inactiveList { get { return mInactiveList; } }

	public enum Clipping : int
	{
		None = 0,
		AlphaClip = 2,				// Adjust the alpha, compatible with all devices
		SoftClip = 3,				// Alpha-based clipping with a softened edge
		ConstrainButDontClip = 4,	// No actual clipping, but does have an area
	}

	[HideInInspector][System.NonSerialized] public int depthStart = int.MaxValue;
	[HideInInspector][System.NonSerialized] public int depthEnd = int.MinValue;
	[HideInInspector][System.NonSerialized] public UIPanel manager;
	[HideInInspector][System.NonSerialized] public UIPanel panel;
	[HideInInspector][System.NonSerialized] public bool alwaysOnScreen = false;
	[HideInInspector][System.NonSerialized] public List<Vector3> verts ;
	[HideInInspector][System.NonSerialized] public List<Vector2> uvs ;
	[HideInInspector][System.NonSerialized] public List<Color32> cols;
    public void AllocateMemory()
    {
        if( verts == null )
        {
            verts = CreateVertsMemory();
        }
        if( uvs == null )
        {
            uvs = CreateUvsMemory();
        }
        if( cols == null)
        {
            cols = CreateColorsMemory();
        }
    }
    public void CollectMemory()
    {
        if(verts != null)
        {
            CollectVertsMemory(verts);
            verts = null;
        }
        if(uvs != null)
        {
            CollectUvsMemory(uvs);
            uvs = null;
        }
        if( cols != null )
        {
            CollectColorsMemory(cols);
            cols = null;
        }
    }
    /// <summary>
    /// 清场景清空空间
    /// </summary>
    public static void ClearMemeory()
    {
        mCacheVerts.Clear();
        mCacheVertsTimes.Clear();
        mCacheColors.Clear();
        mCacheColorsTimes.Clear();
        mCacheUvs.Clear();
        mCacheUvsTimes.Clear();
    }
    private static long mlLastUpdateTime = -1;
    private static long mlUpdateTickTime = 10000000;
    /// <summary>
    /// 更新缓冲区，优化存储量
    /// </summary>
    /// <param name="lCurrentTicks"></param>
    public static void UpdateMemory(long lCurrentTicks)
    {
        ///1秒钟定时更新
        if( lCurrentTicks - mlLastUpdateTime > mlUpdateTickTime )
        {
            UpdateVertsMemory(lCurrentTicks);
            UpdateColorsMemory(lCurrentTicks);
            UpdateUvsMemory(lCurrentTicks);
            mlLastUpdateTime = lCurrentTicks;
        }
    }
   
    private static int miFreeTickTime = 200000000;
    //private static int miMaxCacheCount = 15;
    /// <summary>
    /// 顶点部分
    /// </summary>
    private static List<List<Vector3>> mCacheVerts = new List<List<Vector3>>();
    private static List<long> mCacheVertsTimes = new List<long>();
    private static int mireuseTimes = 0;
    private static int minewTimes = 0;
    private static List<Vector3> CreateVertsMemory()
    {
        if(mCacheVerts.Count > 0)
        {
            List<Vector3> verts = mCacheVerts[0];
            mCacheVerts.RemoveAt(0);
            mCacheVertsTimes.RemoveAt(0);
            mireuseTimes++;
            
            return verts;
        }
        minewTimes++;
        return new List<Vector3>(1024);
    }
    private static void CollectVertsMemory(List<Vector3> verts )
    {
        if (verts == null)
            return;

//         if (mCacheVerts.Count > miMaxCacheCount)
//         {
//             mCacheVertsTimes.RemoveAt(0);
//             mCacheVerts.RemoveAt(0);
//         }
        mCacheVertsTimes.Add(DateTime.Now.Ticks);
        mCacheVerts.Add(verts);
    }
    /// <summary>
    /// 更新顶点数据
    /// </summary>
    /// <param name="lCurrentTicks"></param>
    private static void UpdateVertsMemory(long lCurrentTicks)
    {
        for( int i = 0; i < mCacheVertsTimes.Count;i++)
        {
            if(lCurrentTicks - mCacheVertsTimes[i]> miFreeTickTime)
            {
                mCacheVertsTimes.RemoveAt(i);
                mCacheVerts.RemoveAt(i);
                i--;
                continue;
            }
        }
    }

    /// <summary>
    /// uv部分
    /// </summary>
    private static List<List<Color32>> mCacheColors = new List<List<Color32>>();
    private static List<long> mCacheColorsTimes = new List<long>();
    private static List<Color32> CreateColorsMemory()
    {
        if (mCacheColors.Count > 0)
        {
            List<Color32> Uvs = mCacheColors[0];
            mCacheColorsTimes.RemoveAt(0);
            mCacheColors.RemoveAt(0);
            return Uvs;
        }

        return new List<Color32>(1024);
    }
    private static void CollectColorsMemory(List<Color32> Colors)
    {
        if (Colors == null)
            return;

//         if (mCacheColors.Count > miMaxCacheCount)
//         {
//             mCacheColorsTimes.RemoveAt(0);
//             mCacheColors.RemoveAt(0);
//         }
        mCacheColorsTimes.Add(DateTime.Now.Ticks);
        mCacheColors.Add(Colors);
    }
    /// <summary>
    /// 更新顶点数据
    /// </summary>
    /// <param name="lCurrentTicks"></param>
    private static void UpdateColorsMemory(long lCurrentTicks)
    {
        for (int i = 0; i < mCacheColorsTimes.Count; i++)
        {
            if (lCurrentTicks - mCacheColorsTimes[i] > miFreeTickTime)
            {
                mCacheColorsTimes.RemoveAt(i);
                mCacheColors.RemoveAt(i);
                i--;
                continue;
            }
        }
    }
    /// <summary>
    /// uv部分
    /// </summary>
    private static List<List<Vector2>> mCacheUvs = new List<List<Vector2>>();
    private static List<long> mCacheUvsTimes = new List<long>();
    private static List<Vector2> CreateUvsMemory()
    {
        if (mCacheUvs.Count > 0)
        {
            List<Vector2> Uvs = mCacheUvs[0];
            mCacheUvsTimes.RemoveAt(0);
            mCacheUvs.RemoveAt(0);
            return Uvs;
        }

        return new List<Vector2>(1024);
    }
    private static void CollectUvsMemory(List<Vector2> Uvs)
    {
        if (Uvs == null)
            return;

//         if (mCacheUvs.Count > miMaxCacheCount)
//         {
//             mCacheUvsTimes.RemoveAt(0);
//             mCacheUvs.RemoveAt(0);
//         }
        mCacheUvsTimes.Add(DateTime.Now.Ticks);
        mCacheUvs.Add(Uvs);
    }
    /// <summary>
    /// 更新顶点数据
    /// </summary>
    /// <param name="lCurrentTicks"></param>
    private static void UpdateUvsMemory(long lCurrentTicks)
    {
        for (int i = 0; i < mCacheUvsTimes.Count; i++)
        {
            if (lCurrentTicks - mCacheUvsTimes[i] > miFreeTickTime)
            {
                mCacheUvsTimes.RemoveAt(i);
                mCacheUvs.RemoveAt(i);
                i--;
                continue;
            }
        }
    }
	Material		mMaterial;		// Material used by this screen
	Texture			mTexture;		// Main texture used by the material
	Shader			mShader;		// Shader used by the dynamically created material
	Clipping		mClipping;		// Clipping mode
	Vector4			mClipRange;		// Clipping, if used
	Vector2			mClipSoft;		// Clipping softness

	Transform		mTrans;			// Cached transform
	Mesh			mMesh;			// First generated mesh
	MeshFilter		mFilter;        // Mesh filter for this draw call
                                    // Mesh renderer for this screen
    private bool mbHasRender = false;
    MeshRenderer mRenderOrign = null;
    MeshRenderer mRenderer
    {
        set
        {
            mRenderOrign = value;
            mbHasRender = mRenderOrign != null;
        }
        get
        {
            return mRenderOrign;
        }
    } 
	Material		mDynamicMat;	// Instantiated material
	//List<int>		mIndices ;		// Cached indices
    int miLastIndices = 0;
	bool mRebuildMat = true;
	bool mReset = true;
	int mRenderQueue = 3000;
	Clipping mLastClip = Clipping.None;
	int mTriangles = 0;

	/// <summary>
	/// Whether the draw call has changed recently.
	/// </summary>

	[System.NonSerialized]
	public bool isDirty = false;

	/// <summary>
	/// Render queue used by the draw call.
	/// </summary>

	public int renderQueue
	{
		get
		{
			return mRenderQueue;
		}
		set
		{
			if (mRenderQueue != value)
			{
				mRenderQueue = value;

				if (mDynamicMat != null)
				{
					mDynamicMat.renderQueue = value;
#if UNITY_EDITOR
					if (mbHasRender) mRenderer.enabled = isActive;
#endif
				}
			}
		}
	}

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
	/// <summary>
	/// Renderer's sorting order, to be used with Unity's 2D system.
	/// </summary>

	public int sortingOrder
	{
		get { return (mbHasRender) ? mRenderer.sortingOrder : 0; }
		set { if (mbHasRender && mRenderer.sortingOrder != value) mRenderer.sortingOrder = value; }
	}
#endif

	/// <summary>
	/// Final render queue used to draw the draw call's geometry.
	/// </summary>

	public int finalRenderQueue
	{
		get
		{
			return (mDynamicMat != null) ? mDynamicMat.renderQueue : mRenderQueue;
		}
	}

#if UNITY_EDITOR

	/// <summary>
	/// Whether the draw call is currently active.
	/// </summary>

	public bool isActive
	{
		get
		{
			return mActive;
		}
		set
		{
			if (mActive != value)
			{
				mActive = value;

				if (mbHasRender)
				{
					mRenderer.enabled = value;
					NGUITools.SetDirty(gameObject);
				}
			}
		}
	}
	bool mActive = true;
#endif

	/// <summary>
	/// Transform is cached for speed and efficiency.
	/// </summary>

	public Transform cachedTransform { get { if (mTrans == null) mTrans = transform; return mTrans; } }

    /// <summary>
    /// Material used by this screen.
    /// </summary>
    public string mstrMaterialShaderName= string.Empty;
	public Material baseMaterial
	{
		get
		{
			return mMaterial;
		}
		set
		{
			if (mMaterial != value)
			{
				mMaterial = value;
                if( mMaterial != null )
                {
                    if( mMaterial.shader != null )
                    {
                        mstrMaterialShaderName = mMaterial.shader.name;
                    }
                    else
                    {
                        mstrMaterialShaderName = string.Empty;
                    }
                }
                else
                {
                    mstrMaterialShaderName = string.Empty;
                }
				mRebuildMat = true;
			}
		}
	}

	/// <summary>
	/// Dynamically created material used by the draw call to actually draw the geometry.
	/// </summary>

	public Material dynamicMaterial { get { return mDynamicMat; } }

	/// <summary>
	/// Texture used by the material.
	/// </summary>

	public Texture mainTexture
	{
		get
		{
			return mTexture;
		}
		set
		{
			mTexture = value;
			if (mDynamicMat != null) mDynamicMat.mainTexture = value;
		}
	}

    /// <summary>
    /// Shader used by the material.
    /// </summary>
    private string mstrShaderName = string.Empty;
	public Shader shader
	{
		get
		{
			return mShader;
		}
		set
		{
			if (mShader != value)
			{
				mShader = value;
                if (mShader != null)
                    mstrShaderName = mShader.name;
                else
                {
                    mstrShaderName = string.Empty;
                }
                mRebuildMat = true;
			}
		}
	}

	/// <summary>
	/// The number of triangles in this draw call.
	/// </summary>

	public int triangles { get { return (mMesh != null) ? mTriangles : 0; } }

	/// <summary>
	/// Whether the draw call is currently using a clipped shader.
	/// </summary>

	public bool isClipped { get { return mClipping != Clipping.None; } }

	/// <summary>
	/// Clipping used by the draw call
	/// </summary>

	public Clipping clipping { get { return mClipping; } set { if (mClipping != value) { mClipping = value; mReset = true; } } }

	/// <summary>
	/// Clip range set by the panel -- used with a shader that has the "_ClipRange" property.
	/// </summary>

	public Vector4 clipRange { get { return mClipRange; } set { mClipRange = value; } }

	/// <summary>
	/// Clipping softness factor, if soft clipping is used.
	/// </summary>

	public Vector2 clipSoftness { get { return mClipSoft; } set { mClipSoft = value; } }
    /// <summary>
    /// Create an appropriate material for the draw call.
    /// </summary>
    void Awake()
    {
        AllocateMemory();
        CreateRvCache();
        miClipSharpness = Shader.PropertyToID("_ClipSharpness");
    }
    public void CreateRvCache()
    {
        if( rvCache1 == null )
        {
            rvCache1 = new List<int>(128);
        }
        if (rvCache2 == null)
        {
            rvCache2 = new List<int>(256);
        }
        if (rvCache3 == null)
        {
            rvCache3 = new List<int>(512);
        }
        if (rvCache4 == null)
        {
            rvCache4 = new List<int>(768);
        }
        if (rvCache5 == null)
        {
            rvCache5 = new List<int>(1024);
        }
        if (rvCache6 == null)
        {
            rvCache6 = new List<int>(2048);
        }
        if (rvCache7 == null)
        {
            rvCache7 = new List<int>(4096);
        }
    }

    void CreateMaterial ()
	{
		const string alpha = " (AlphaClip)";
		const string soft = " (SoftClip)";
		string shaderName = (!string.IsNullOrEmpty(mstrShaderName)) ? mstrShaderName :
			(!string.IsNullOrEmpty(mstrMaterialShaderName) ? mstrMaterialShaderName : "Unlit/Transparent Colored");

		// Figure out the normal shader's name
		shaderName = shaderName.Replace("GUI/Text Shader", "Unlit/Text");
		shaderName = shaderName.Replace(alpha, string.Empty);
		shaderName = shaderName.Replace(soft, string.Empty);

		// Try to find the new shader
		Shader shader;

		if (mClipping == Clipping.SoftClip)
		{
			shader = Shader.Find(shaderName + soft);
		}
		else if (mClipping == Clipping.AlphaClip)
		{
			shader = Shader.Find(shaderName + alpha);
		}
		else // No clipping
		{
			shader = Shader.Find(shaderName);
		}


		if (mMaterial != null)
		{
            mDynamicMat = new Material(mMaterial);
            mDynamicMat.CopyPropertiesFromMaterial(mMaterial);

		}
		else
        {
            mDynamicMat = new Material(shader);
        }

		// If there is a valid shader, assign it to the custom material
		if (shader != null)
		{
			mDynamicMat.shader = shader;
		}
		else if (mClipping != Clipping.None)
		{
            Debug.LogWarning(shaderName + " doesn't have a clipped shader version for " + mClipping);
			mClipping = Clipping.None;
		}
	}

	/// <summary>
	/// Rebuild the draw call's material.
	/// </summary>

	Material RebuildMaterial ()
	{
        // Create a new material
        CreateMaterial();

		mDynamicMat.renderQueue = mRenderQueue;
		mLastClip = mClipping;

		// Assign the main texture
		if (mTexture != null)
            mDynamicMat.mainTexture = mTexture;

		// Update the renderer
		if (mbHasRender)
            mRenderer.sharedMaterials = new Material[] { mDynamicMat };

		return mDynamicMat;
	}

	/// <summary>
	/// Update the renderer's materials.
	/// </summary>

	void UpdateMaterials ()
	{
		// If clipping should be used, we need to find a replacement shader
		if (mRebuildMat || mDynamicMat == null || mClipping != mLastClip)
		{
			RebuildMaterial();
			mRebuildMat = false;
		}
		else if (mRenderer.sharedMaterial != mDynamicMat)
		{
#if UNITY_EDITOR
            Debug.LogWarning("Hmm... This point got hit!");
#endif
			mRenderer.sharedMaterials = new Material[] { mDynamicMat };
		}
	}

	/// <summary>
	/// Set the draw call's geometry.
	/// </summary>

	public void UpdateGeometry ()
	{
		int count = verts.Count;

		// Safety check to ensure we get valid values
		if (count > 0 && (count == uvs.Count && count == cols.Count) && (count % 4) == 0)
		{
			// Cache all components
			if (mFilter == null) mFilter = gameObject.GetComponent<MeshFilter>();
			if (mFilter == null) mFilter = gameObject.AddComponent<MeshFilter>();

			if (verts.Count < 65000)
			{
				// Populate the index buffer
				int indexCount = (count >> 1) * 3;
				bool setIndices = (miLastIndices != indexCount);

				// Create the mesh
				if (mMesh == null)
				{
					mMesh = new Mesh();
					mMesh.hideFlags = HideFlags.DontSave;
					mMesh.name = (mMaterial != null) ? mMaterial.name : "Mesh";
#if !UNITY_3_5
					mMesh.MarkDynamic();
#endif
					setIndices = true;
				}
#if !UNITY_FLASH
				// If the buffer length doesn't match, we need to trim all buffers
				bool trim = (uvs.Count != verts.Count) || (cols.Count != verts.Count);

				// Non-automatic render queues rely on Z position, so it's a good idea to trim everything
				if (!trim && panel.renderQueue != UIPanel.RenderQueue.Automatic)
					trim = (mMesh == null || mMesh.vertexCount != verts.Count);

				// If the number of vertices in the buffer is less than half of the full buffer, trim it
				if (!trim && (verts.Count << 1) < verts.Count) trim = true;

				mTriangles = (verts.Count >> 1);

				if (trim || verts.Count > 65000)
				{
					if (trim || mMesh.vertexCount != verts.Count)
					{
						mMesh.Clear();
						setIndices = true;
					}

                    mMesh.SetVertices(verts);
					mMesh.SetUVs(0,uvs);
                    mMesh.SetColors(cols);

//                     if (norms != null)
//                     {
//                         mMesh.SetNormals(norms);
//                     }
//                     if (tans != null)
//                     {
//                         mMesh.SetTangents(tans); 
//                     }
				}
				else
				{
					if (mMesh.vertexCount != verts.Count)
					{
						mMesh.Clear();
						setIndices = true;
					}

                    mMesh.SetVertices(verts);
                    mMesh.SetUVs(0, uvs);
                    mMesh.SetColors(cols);

				}
#else
				mTriangles = (verts.size >> 1);

				if (mMesh.vertexCount != verts.size)
				{
					mMesh.Clear();
					setIndices = true;
				}

				mMesh.vertices = verts.ToArray();
				mMesh.uv = uvs.ToArray();
				mMesh.colors32 = cols.ToArray();

				if (norms != null) mMesh.normals = norms.ToArray();
				if (tans != null) mMesh.tangents = tans.ToArray();
#endif
				if (setIndices)
				{
                    List<int> indices = GenerateCachedIndexBuffer(count, indexCount);
                    mMesh.SetTriangles(indices, 0);

                   // mMesh.triangles = mIndices;
				}

#if !UNITY_FLASH
				if (trim || !alwaysOnScreen)
#endif
					mMesh.RecalculateBounds();

				mFilter.mesh = mMesh;
			}
			else
			{
				mTriangles = 0;
				if (mFilter.mesh != null) mFilter.mesh.Clear();
                Debug.LogWarning("Too many vertices on one panel: " + verts.Count);
			}

			if (!mbHasRender) mRenderer = gameObject.GetComponent<MeshRenderer>();

			if (!mbHasRender)
			{
				mRenderer = gameObject.AddComponent<MeshRenderer>();
#if UNITY_EDITOR
				mRenderer.enabled = isActive;
#endif
			}
			UpdateMaterials();
		}
		else
		{
			if (mFilter.mesh != null) mFilter.mesh.Clear();
            Debug.LogWarning("UIWidgets must fill the buffer with 4 vertices per quad. Found " + count);
		}

		verts.Clear();
		uvs.Clear();
		cols.Clear();
	}

	const int maxIndexBufferCache = 20;

#if UNITY_FLASH
	List<int[]> mCache = Allocator.CreateList<int[]>(maxIndexBufferCache);
#else
/*	static List<List<int>> mCache = Allocator.CreateList<List<int>>(maxIndexBufferCache);*/
#endif

	/// <summary>
	/// Generates a new index buffer for the specified number of vertices (or reuses an existing one).
	/// </summary>
    private static List<int> rvCache1 = null;
    private static List<int> rvCache2 = null;
    private static List<int> rvCache3 = null;
    private static List<int> rvCache4 = null;
    private static List<int> rvCache5 = null;
    private static List<int> rvCache6 = null;
    private static List<int> rvCache7 = null;
    List<int> GenerateCachedIndexBuffer(int vertexCount, int indexCount)
    {
        if (indexCount < 128)
        {
            return GenerateDestCachedIndexBuffer(rvCache1, vertexCount, indexCount);
        }
        else if (indexCount < 256)
        {
            return GenerateDestCachedIndexBuffer(rvCache2, vertexCount, indexCount);
        }
        else if(indexCount < 512)
        {
            return GenerateDestCachedIndexBuffer(rvCache3, vertexCount, indexCount);
        }
        else if (indexCount < 768)
        {
            return GenerateDestCachedIndexBuffer(rvCache4, vertexCount, indexCount);
        }
        else if (indexCount < 1024)
        {
            return GenerateDestCachedIndexBuffer(rvCache5, vertexCount, indexCount);
        }
        else if (indexCount < 2048)
        {
            return GenerateDestCachedIndexBuffer(rvCache6, vertexCount, indexCount);
        }
        else 
        {
            return GenerateDestCachedIndexBuffer(rvCache7, vertexCount, indexCount);
        }
    }
    
    List<int> GenerateDestCachedIndexBuffer(List<int> rvCache,int vertexCount, int indexCount)
    {
        int iRvCacheCount = rvCache.Count;
        if (iRvCacheCount < indexCount)
        {
            ///不足，补充顶点
            int iVertexStart = iRvCacheCount * 4 / 6;
            for (int i = iVertexStart; i < vertexCount; i += 4)
            {
                rvCache.Add(i);
                rvCache.Add(i + 1);
                rvCache.Add(i + 2);

                rvCache.Add(i + 2);
                rvCache.Add(i + 3);
                rvCache.Add(i);
            }
        }
        else if (iRvCacheCount > indexCount)
        {
            ///多余删除顶点
            rvCache.RemoveRange(indexCount, iRvCacheCount - indexCount);
        }

        return rvCache;
    }
	/// <summary>
	/// This function is called when it's clear that the object will be rendered.
	/// We want to set the shader used by the material, creating a copy of the material in the process.
	/// We also want to update the material's properties before it's actually used.
	/// </summary>

	void OnWillRenderObject ()
	{
		if (mReset)
		{
			mReset = false;
			UpdateMaterials();
		}

		if (mDynamicMat != null && isClipped && mClipping != Clipping.ConstrainButDontClip)
		{
            Vector2 vTemp = Vector2.zero;
            Vector2 vTemp2 = Vector2.zero;
            
            vTemp2.x = 1f / mClipRange.z;
            vTemp2.y = 1f / mClipRange.w;
     
            vTemp.x = -mClipRange.x * vTemp2.x;
            vTemp.y = -mClipRange.y * vTemp2.y;
         
            mDynamicMat.mainTextureOffset = vTemp;
            mDynamicMat.mainTextureScale = vTemp2;

            vTemp.x = 1000.0f;
            vTemp.y = 1000.0f;
			if (mClipSoft.x > 0f) vTemp.x = mClipRange.z / mClipSoft.x;
			if (mClipSoft.y > 0f) vTemp.y = mClipRange.w / mClipSoft.y;
			mDynamicMat.SetVector(miClipSharpness, vTemp);
		}
	}
    int miClipSharpness = -1;

    void OnEnable ()
    {
        AllocateMemory();
        mRebuildMat = true;
    }

	/// <summary>
	/// Clear all references.
	/// </summary>

	void OnDisable ()
	{
		depthStart = int.MaxValue;
		depthEnd = int.MinValue;
		panel = null;
		manager = null;
		mMaterial = null;
        mstrMaterialShaderName = string.Empty;
        mTexture = null;
        mDynamicMat = null;
        CollectMemory();
    }
   
	/// <summary>
	/// Cleanup.
	/// </summary>

	void OnDestroy ()
	{
		NGUITools.DestroyImmediateNGUI(mMesh);
        //add by chenfei
         mTrans =null;			// Cached transform
         mMesh=null;			// First generated mesh
         mFilter=null;		// Mesh filter for this draw call
         mRenderer=null;        // Mesh renderer for this screen
         mDynamicMat =null;	// Instantiated material
        CollectMemory();
	}

	/// <summary>
	/// Return an existing draw call.
	/// </summary>

	static public UIDrawCall Create (UIPanel panel, Material mat, Texture tex, Shader shader)
	{
#if UNITY_EDITOR
        string name = string.Empty;
		if (tex != null) name = tex.name;
		else if (shader != null) name = shader.name;
		else if (mat != null) name = mat.name;
		return Create(name, panel, mat, tex, shader);
#else
		return Create(null, panel, mat, tex, shader);
#endif
	}

	/// <summary>
	/// Create a new draw call, reusing an old one if possible.
	/// </summary>

	static UIDrawCall Create (string name, UIPanel pan, Material mat, Texture tex, Shader shader)
	{
		UIDrawCall dc = Create(name);
		dc.gameObject.layer = pan.cachedGameObject.layer;
		dc.clipping = pan.clipping;
		dc.baseMaterial = mat;
		dc.mainTexture = tex;
		dc.shader = shader;
		dc.renderQueue = pan.startingRenderQueue;
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
		dc.sortingOrder = pan.sortingOrder;
#endif
		dc.manager = pan;
		return dc;
	}
    public static UIDrawCall Create(UIPanel pan, Material mat, Texture tex, Shader shader, UIDrawCall dc)
    {
        if (dc == null )
        {
            return null;
        }

        if ( mInactiveList.Contains(dc) )
        {
            mInactiveList.Remove(dc);
            mActiveList.Add(dc);
 
            NGUITools.SetActive(dc.gameObject, true);

            dc.gameObject.layer = pan.cachedGameObject.layer;
            dc.clipping = pan.clipping;
            dc.baseMaterial = mat;
            dc.mainTexture = tex;
            dc.shader = shader;
            dc.renderQueue = pan.startingRenderQueue;
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_1 && !UNITY_4_2
            dc.sortingOrder = pan.sortingOrder;
#endif
            dc.manager = pan;
            return dc;
        }
        return null;
    }
	/// <summary>
	/// Create a new draw call, reusing an old one if possible.
	/// </summary>

	static UIDrawCall Create (string name)
	{
#if SHOW_HIDDEN_OBJECTS && UNITY_EDITOR
		name = (name != null) ? "_UIDrawCall [" + name + "]" : "DrawCall";
#endif
		if (mInactiveList.size > 0)
		{
			UIDrawCall dc = mInactiveList.Pop();
			mActiveList.Add(dc);
			if (name != null) dc.name = name;
			NGUITools.SetActive(dc.gameObject, true);
			return dc;
		}

#if UNITY_EDITOR
		// If we're in the editor, create the game object with hide flags set right away
		GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(name,
 #if SHOW_HIDDEN_OBJECTS
			HideFlags.DontSave | HideFlags.NotEditable, typeof(UIDrawCall));
 #else
			HideFlags.HideAndDontSave, typeof(UIDrawCall));
 #endif
		UIDrawCall newDC = go.GetComponent<UIDrawCall>();
#else
		GameObject go = new GameObject(name);
		DontDestroyOnLoad(go);
		UIDrawCall newDC = go.AddComponent<UIDrawCall>();
#endif
		// Create the draw call
		mActiveList.Add(newDC);
		return newDC;
	}

	/// <summary>
	/// Clear all draw calls.
	/// </summary>

	static public void ClearAll ()
	{
		bool playing = Application.isPlaying;

		for (int i = mActiveList.size; i > 0; )
		{
			UIDrawCall dc = mActiveList[--i];

			if (dc)
			{
				if (playing) NGUITools.SetActive(dc.gameObject, false);
				else NGUITools.DestroyImmediateNGUI(dc.gameObject);
			}
		}
		mActiveList.Clear();
	}

	/// <summary>
	/// Immediately destroy all draw calls.
	/// </summary>

	static public void ReleaseAll ()
	{
		ClearAll();
		ReleaseInactive();
	}

	/// <summary>
	/// Immediately destroy all inactive draw calls (draw calls that have been recycled and are waiting to be re-used).
	/// </summary>

	static public void ReleaseInactive()
	{
		for (int i = mInactiveList.size; i > 0; )
		{
			UIDrawCall dc = mInactiveList[--i];
			if (dc) NGUITools.DestroyImmediateNGUI(dc.gameObject);
		}
		mInactiveList.Clear();
	}

	/// <summary>
	/// Count all draw calls managed by the specified panel.
	/// </summary>

	static public int Count (UIPanel panel)
	{
		int count = 0;
		for (int i = 0; i < mActiveList.size; ++i)
			if (mActiveList[i].manager == panel) ++count;
		return count;
	}

	/// <summary>
	/// DestroyTimer the specified draw call.
	/// </summary>

	static public void DestroyDrawCall (UIDrawCall dc)
	{
		if (dc)
		{
			if (!Application.isPlaying)
			{
                mActiveList.Remove(dc);
                NGUITools.DestroyImmediateNGUI(dc.gameObject);
            }
			else
            {
                if (mActiveList.Remove(dc))
                {
                    NGUITools.SetActive(dc.gameObject, false);
                    mInactiveList.Add(dc);
                }
			}
		}
	}
}
