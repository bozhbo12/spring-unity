//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Generated geometry class. All widgets have one.
/// This class separates the geometry creation into several steps, making it possible to perform
/// actions selectively depending on what has changed. For example, the widget doesn't need to be
/// rebuilt unless something actually changes, so its geometry can be cached. Likewise, the widget's
/// transformed coordinates only change if the widget's transform moves relative to the panel,
/// so that can be cached as well. In the end, using this class means using more memory, but at
/// the same time it allows for significant performance gains, especially when using widgets that
/// spit out a lot of vertices, such as UILabels.
/// </summary>

public class UIGeometry
{
	/// <summary>
	/// Widget's vertices (before they get transformed).
	/// </summary>

	public List<Vector3> verts ;

	/// <summary>
	/// Widget's texture coordinates for the geometry's vertices.
	/// </summary>

	public List<Vector2> uvs ;

	/// <summary>
	/// Array of colors for the geometry's vertices.
	/// </summary>

	public List<Color32> cols;

	// Relative-to-panel vertices
	List<Vector3> mRtpVerts ;

	/// <summary>
	/// Whether the geometry contains usable vertices.
	/// </summary>

	public bool hasVertices { get { return (verts.Count > 0); } }

	/// <summary>
	/// Whether the geometry has usable transformed vertex data.
	/// </summary>

	public bool hasTransformed { get { return (mRtpVerts != null) && (mRtpVerts.Count > 0) && (mRtpVerts.Count == verts.Count); } }
    public UIGeometry(int iLength)
    {
        verts = new List<Vector3>(iLength);
        uvs = new List<Vector2>(iLength);
        cols = new List<Color32>(iLength);
        mRtpVerts = new List<Vector3>(iLength);
    }
    /// <summary>
    /// 适应战斗期间频繁人头，战斗飘雪的变化需求
    /// </summary>
    private static int miFreeTimes = 200000000;
    private static int miMaxStyleLabelCache = 20;
    private static int miMaxLabelCache = 30;
    private static int miMaxSpriteCache = 30;
    private static List<UIGeometry> mLabelGeometrys = new List<UIGeometry>();
    private static List<long> mLabelTimes = new List<long>();
    private static List<UIGeometry> mStyleLabelGeometrys = new List<UIGeometry>();
    private static List<long> mStyleLabelTimes = new List<long>();
    private static List<UIGeometry> mSpriteGeometrys = new List<UIGeometry>();
    private static List<long> mSpriteTimes = new List<long>();

    public static void ClearGeometry()
    {
        mLabelTimes.Clear();
        mLabelGeometrys.Clear();
        mStyleLabelTimes.Clear();
        mStyleLabelGeometrys.Clear();
        mSpriteTimes.Clear();
        mSpriteGeometrys.Clear();
    }
    private static long mlLastUpdateTicks = -1;
    private static long mlUpdateTickTime = 10000000;
    /// <summary>
    /// 更新显示模型缓存区
    /// </summary>
    /// <param name="lCurrentTicks"></param>
    public static void UpdateGeometry(long lCurrentTicks)
    {
        ///1秒钟更新一次
        if( lCurrentTicks- mlLastUpdateTicks > mlUpdateTickTime)
        {
            UpdateLableGeometry(lCurrentTicks);
            UpdateStyleLableGeometry(lCurrentTicks);
            UpdateSpriteGeometry(lCurrentTicks);
            mlLastUpdateTicks = lCurrentTicks;
        }     
    }

    public static UIGeometry CreateLabelGeometry()
    {
        if( mLabelGeometrys.Count > 0 )
        {
            UIGeometry gemetry = mLabelGeometrys[0];
            mLabelGeometrys.RemoveAt(0);
            mLabelTimes.RemoveAt(0);

            return gemetry;
        }
     
        return new UIGeometry(0);
    }

    private static void UpdateLableGeometry(long lCurrentTicks)
    {
        for( int i = 0; i < mLabelTimes.Count;i++)
        {
            if(lCurrentTicks - mLabelTimes[i] > miFreeTimes)
            {
                mLabelTimes.RemoveAt(i);
                mLabelGeometrys.RemoveAt(i);
                i--;
                continue;
            }
        }
    }
    public static void CollectLabelGeometry(UIGeometry geometry )
    {
        if (geometry == null)
            return;

        geometry.Clear();
        if( mLabelGeometrys.Count > miMaxLabelCache)
        {
            mLabelTimes.RemoveAt(0);
            mLabelGeometrys.RemoveAt(0);
        }
        ///保留最后使用对象
        mLabelTimes.Add(DateTime.Now.Ticks);
        mLabelGeometrys.Add(geometry);
    }
    public static UIGeometry CreateStyleLabelGeometry()
    {
        if (mStyleLabelGeometrys.Count > 0)
        {
            UIGeometry gemetry = mStyleLabelGeometrys[0];
            mStyleLabelGeometrys.RemoveAt(0);
            mStyleLabelTimes.RemoveAt(0);

            return gemetry;
        }

        return new UIGeometry(0);
    }

    private static void UpdateStyleLableGeometry(long lCurrentTicks)
    {
        for (int i = 0; i < mStyleLabelTimes.Count; i++)
        {
            if (lCurrentTicks - mStyleLabelTimes[i] > miFreeTimes)
            {
                mStyleLabelTimes.RemoveAt(i);
                mStyleLabelGeometrys.RemoveAt(i);
                i--;
                continue;
            }
        }
    }
    public static void CollectStyleLabelGeometry(UIGeometry geometry)
    {
        if (geometry == null)
            return;

        geometry.Clear();

        if (mStyleLabelGeometrys.Count > miMaxStyleLabelCache)
        {
            mStyleLabelTimes.RemoveAt(0);
            mStyleLabelGeometrys.RemoveAt(0);
        }
        ///保留最后使用对象
        mStyleLabelTimes.Add(DateTime.Now.Ticks);
        mStyleLabelGeometrys.Add(geometry);
    }
    public static UIGeometry CreateSpriteGeometry()
    {
        if (mSpriteGeometrys.Count > 0)
        {
            UIGeometry gemetry = mSpriteGeometrys[0];
            mSpriteTimes.RemoveAt(0);
            mSpriteGeometrys.RemoveAt(0);
            return gemetry;
        }

        return new UIGeometry(4);
    }

    private static void UpdateSpriteGeometry(long lCurrentTicks)
    {
        for (int i = 0; i < mSpriteTimes.Count; i++)
        {
            if (lCurrentTicks - mSpriteTimes[i] > miFreeTimes)
            {
                mSpriteTimes.RemoveAt(i);
                mSpriteGeometrys.RemoveAt(i);
                i--;
                continue;
            }
        }
    }
    public static void CollectSpriteGeometry(UIGeometry geometry)
    {
        if (geometry == null)
            return;

        geometry.Clear();

        if (mSpriteGeometrys.Count > miMaxSpriteCache)
        {
            mSpriteTimes.RemoveAt(0);
            mSpriteGeometrys.RemoveAt(0);
        }

        ///保留最后使用对象
        mSpriteGeometrys.Add(geometry);
        mSpriteTimes.Add(DateTime.Now.Ticks);
    }
    /// <summary>
    /// Step 1: Prepare to fill the buffers -- make them clean and valid.
    /// </summary>

    public void Clear ()
	{
		verts.Clear();
		uvs.Clear();
		cols.Clear();
		mRtpVerts.Clear();
	}

	/// <summary>
	/// Step 2: Transform the vertices by the provided matrix.
	/// </summary>

	public void ApplyTransform (Matrix4x4 widgetToPanel)
	{
		if (verts.Count > 0)
		{
			mRtpVerts.Clear();
            for (int i = 0, imax = verts.Count; i < imax; ++i)
            {
                mRtpVerts.Add(widgetToPanel.MultiplyPoint3x4(verts[i]));
            }

			// Calculate the widget's normal and tangent
// 			mRtpNormal = widgetToPanel.MultiplyVector(Vector3.back).normalized;
// 			Vector3 tangent = widgetToPanel.MultiplyVector(Vector3.right).normalized;
   
//             mRtpTan.x = tangent.x;
//             mRtpTan.y = tangent.y;
//             mRtpTan.z = tangent.z;
//             mRtpTan.w = -1f;
            //mRtpTan = vecTemp4;
		}
		else mRtpVerts.Clear();
	}
    public int GetRtpVertsCount()
    {
        if (mRtpVerts != null )
        {
            return mRtpVerts.Count;
        }
        return 0;
    }
	/// <summary>
	/// Step 3: Fill the specified buffer using the transformed values.
	/// </summary>

	public void WriteToBuffers (List<Vector3> v, List<Vector2> u, List<Color32> c/*, List<Vector3> n, List<Vector4> t*/)
	{
		if (mRtpVerts != null && mRtpVerts.Count > 0)
		{
                v.AddRange(mRtpVerts);
                u.AddRange(uvs);
                c.AddRange(cols);
                //                 for (int i = 0; i < mRtpVerts.size; ++i)
                //  				{
                //  					v.Add(mRtpVerts.buffer[i]);
                //  					u.Add(uvs.buffer[i]);
                //  					c.Add(cols.buffer[i]);
                //  				}
//             }
//             else
//             {
//                 v.AddRange(mRtpVerts);
//                 u.AddRange(uvs);
//                 c.AddRange(cols);
//                 //                 n.AddRange(mRtpNormal);
//                 //                 t.AddRange(mRtpTan);
// 
//                 for (int i = 0; i < mRtpVerts.Count; ++i)
//                 {
//                     n.Add(mRtpNormal);
//                     t.Add(mRtpTan);
//                     // 					u.Add(uvs.buffer[i]);
//                     // 					c.Add(cols.buffer[i]);
//                     // 					n.Add(mRtpNormal);
//                     // 					t.Add(mRtpTan);
//                     // 				}
//                 }
//             }
		}
	}
}
