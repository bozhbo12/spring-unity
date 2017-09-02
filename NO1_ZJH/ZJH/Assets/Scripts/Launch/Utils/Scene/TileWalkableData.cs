using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TileWalkableData
{
    private const int RAY_NUM = 5;

    private Dictionary<int, float[,]> mFloorHeightDic = new Dictionary<int, float[,]>();       // 层高
    private Dictionary<int, float[,]> mFloorSpaceDic = new Dictionary<int, float[,]>();        // 无障碍高度
    private Dictionary<int, int[,]> mWalkMarkerDic = new Dictionary<int, int[,]>();          // 行走标记
    private int mFloorNum = 0;
    private int[] mFloorKey = null;

    private Dictionary<int, List<RaycastHit>> mRaycastHitDic = new Dictionary<int, List<RaycastHit>>();
    private Dictionary<int, RaycastHit> mOnePosRaycastHit = new Dictionary<int, RaycastHit>();

    private TerrainConfig mTerrainConfig = null;
    private int mMask = 0;
    private int mOcclusionMask = 0;
    private Vector3 mTilePos;
    private string mTileKey;

    public TileWalkableData(string key, Vector3 tilePos, TerrainConfig terrainConfig)
    {
        mTileKey = key;
        mTerrainConfig = terrainConfig;
        mTilePos = tilePos;
    }

    /*************************************************************************************************
     * 功能 ： 采样切片高度，取缔碰撞体计算行走高度
     * 注解 : 采样高度只能选取最高度为最终高度, 避免角色与物件穿插
     *        阻塞状态计算 :
     *        1、根据高度差计算初始格子阻塞状态
     *        2、根据碰撞对象层级判断
     * @mask 检测层级
     * @occlusionMask 阻挡部分标记
     **************************************************************************************************/
    public void ComputeHeights(int mask, int occlusionMask, GameObjectUnit selectObject = null)
    {
        mMask = mask;
        mOcclusionMask = occlusionMask;

        mFloorHeightDic.Clear();
        mFloorSpaceDic.Clear();
        mWalkMarkerDic.Clear();

        Vector3[] origins = new Vector3[RAY_NUM];
        origins[0] = new Vector3(0f, 500, 0f);
        origins[1] = new Vector3(1f, 500, 0f);
        origins[2] = new Vector3(0f, 500, 1f);
        origins[3] = new Vector3(1f, 500, 1f);
        origins[4] = new Vector3(0.5f, 500, 0.5f);

        Vector3 origin = new Vector3(-0.5f, 500, -0.5f);

        Ray ray = new Ray();
        ray.direction = Vector3.down;

        List<RaycastHit> hitList = null;
        int layer = -1;

        //        bool flagObj = false;
        int width = mTerrainConfig.tileSize;
        int height = mTerrainConfig.tileSize;
        float m = (float)mTerrainConfig.tileSize / (float)mTerrainConfig.tileSize;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                mRaycastHitDic.Clear();

                /// 格子的4个角加中心点发射5条射线
                for (int k = 0; k < origins.Length; k++)
                {
                    origin.x = i * m + mTilePos.x + origins[k].x - mTerrainConfig.tileSize * 0.5f;
                    origin.z = j * m + mTilePos.z + origins[k].z - mTerrainConfig.tileSize * 0.5f;
                    ray.origin = origin;

                    RaycastHit[] arrHit = Physics.RaycastAll(ray, 2000, mask);
                    if (arrHit == null || arrHit.Length == 0)
                    {
                        continue;
                    }
                    mOnePosRaycastHit.Clear();
                    int length = arrHit.Length;
                    for (int n = 0; n < length; ++n)
                    {
                        if (arrHit[n].transform == null)
                        {
                            continue;
                        }
                        layer = arrHit[n].transform.gameObject.layer;
                        if (!mOnePosRaycastHit.ContainsKey(layer))
                        {
                            mOnePosRaycastHit[layer] = arrHit[n];
                        }
                        else if (mOnePosRaycastHit[layer].point.y < arrHit[n].point.y)
                        {
                            mOnePosRaycastHit[layer] = arrHit[n];
                        }
                    }

                    //                         if (selectObject != null && selectObject.ins != null)
                    //                         {
                    //                             if (arrHit[n].transform.gameObject.name == selectObject.ins.name)
                    //                             {
                    //                                 flagObj = true;
                    //                             }
                    //                          }
                    foreach (KeyValuePair<int, RaycastHit> data in mOnePosRaycastHit)
                    {
                        hitList = null;
                        if (!mRaycastHitDic.TryGetValue(data.Key, out hitList) || hitList == null)
                        {
                            hitList = new List<RaycastHit>();
                        }
                        if (!hitList.Contains(data.Value))
                        {
                            hitList.Add(data.Value);
                        }
                        mRaycastHitDic[data.Key] = hitList;
                    }
                }

                foreach (KeyValuePair<int, List<RaycastHit>> hitDic in mRaycastHitDic)
                {
                    SetTerrainDataByFloor(hitDic.Key, i, j, hitDic.Value);
                }

                // 计算指定对象所占的格子
                //                 if (flagObj == true && grids[i, j] == 1)
                //                 {
                //                     float gwx = i * m + mTilePos.x + origins[4].x - mTerrainConfig.tileSize * 0.5f;
                //                     float gwy = j * m + mTilePos.z + origins[4].z - mTerrainConfig.tileSize * 0.5f;
                // 
                //                     int gridX = Mathf.FloorToInt(gwx / (mTerrainConfig.gridSize));
                //                     int gridY = Mathf.FloorToInt(gwy / (mTerrainConfig.gridSize));
                // 
                //                     selectObject.AppendGrid(gridX, gridY);
                //                 }
            }
        }
    }

    /// <summary>
    ///  设置指定格指定层的地形数据
    /// </summary>
    private void SetTerrainDataByFloor(int layer, int row, int col, List<RaycastHit> hitList)
    {
        //         if (arrHit == null )
        //         {
        //             return;
        //         }

        /// 设置层高
        float[,] arrFloorHeight = null;
        if (!mFloorHeightDic.TryGetValue(layer, out arrFloorHeight) || arrFloorHeight == null)
        {
            arrFloorHeight = new float[mTerrainConfig.tileSize, mTerrainConfig.tileSize];
        }
        arrFloorHeight[row, col] = GetFloorHeight(hitList);
        mFloorHeightDic[layer] = arrFloorHeight;

        /// 设置行走层的无障碍高度
        float[,] arrFloorSpace = null;
        if (!mFloorSpaceDic.TryGetValue(layer, out arrFloorSpace) || arrFloorSpace == null)
        {
            arrFloorSpace = new float[mTerrainConfig.tileSize, mTerrainConfig.tileSize];
        }
        arrFloorSpace[row, col] = GetFloorSpace(hitList);
        mFloorSpaceDic[layer] = arrFloorSpace;

        /// 设置行走标记
        int[,] arrWalkMarker = null;
        if (!mWalkMarkerDic.TryGetValue(layer, out arrWalkMarker) || arrWalkMarker == null)
        {
            arrWalkMarker = new int[mTerrainConfig.tileSize, mTerrainConfig.tileSize];
        }
        arrWalkMarker[row, col] = GetWalkMarker(hitList);
        mWalkMarkerDic[layer] = arrWalkMarker;
    }

    /// <summary>
    /// 获取层高度
    /// 注：arrHit：同时发送5条射线取最大值高度
    /// </summary>
    private float GetFloorHeight(List<RaycastHit> hitList)
    {
        float maxHeight = -10000;
        if (hitList == null)
        {
            return maxHeight;
        }
  
        int lenght = hitList.Count;
        for (int i = 0; i < lenght; ++i)
        {
            if (maxHeight < hitList[i].point.y)
            {
                maxHeight = hitList[i].point.y;
            }
        }
        return maxHeight;
    }

    /// <summary>
    /// 获得行走层的无障碍高度
    /// 注：arrHit：同时发送5条射线取最小值高度
    /// </summary>
    private float GetFloorSpace(List<RaycastHit> hitList)
    {
        float defSpace = 1e8f;
        if (hitList == null)
        {
            return defSpace;
        }
        float minSpace = defSpace;
        Ray ray = new Ray();
        ray.direction = Vector3.up;
        
        int lenght = hitList.Count;
        for (int i = 0; i < lenght; ++i)
        {
            ray.origin = hitList[i].point;
            RaycastHit hit;
            Physics.Raycast(ray, out hit, defSpace, mMask);
            if (hit.transform != null)
            {
                if (minSpace > hit.distance)
                {
                    minSpace = hit.distance;
                }
            }
        }
        return minSpace;
    }

    /// <summary>
    /// 获得行走标记
    ///  注：arrHit：同时发送5条射线，(通过高度差 、 阻塞物体 或者 可到达高度来计算是否阻挡)
    ///  return 0非阻挡，1阻挡
    /// </summary>
    private int GetWalkMarker(List<RaycastHit> hitList)
    {
        int block = 1;
        if (hitList == null)
        {
            return block;
        }
        block = 0;
        float maxH = -10000;
        float minH = 10000;
        int lenght = hitList.Count;
        for (int i = 0; i < lenght; ++i)
        {
            if (hitList[i].transform != null)
            {
               if (block == 0 && GetBit(hitList[i].transform.gameObject.layer, mOcclusionMask) >= 1)
               {
                    block = 1;
               }
               if (maxH < hitList[i].point.y)
               {
                    maxH = hitList[i].point.y;
               }
               if (minH > hitList[i].point.y)
               {
                    minH = hitList[i].point.y;
               }
            }
        }
        if (block == 0)
        {
            if ((maxH - minH) > mTerrainConfig.blockHeight)
            {
                block = 1;
            }
            else if (maxH > mTerrainConfig.maxReachTerrainHeight)
            {
                block = 1;
            }
        }
        return block;
    }

    private int GetBit(int layer, int occlusionMask)
    {
        return (1 << layer) & occlusionMask;
    }

    public Dictionary<int, float[,]> GetFloorHeightDic()
    {
        return mFloorHeightDic;
    }

    public Dictionary<int, float[,]> GetFloorSpaceDic()
    {
        return mFloorSpaceDic;
    }

    public Dictionary<int, int[,]> GetWalkMarkerDic()
    {
        return mWalkMarkerDic;
    }

    public bool WriterFloorInfo(BinaryWriter bw)
    {
        if (bw == null || mWalkMarkerDic == null)
        {
            return false;
        }
        mFloorNum = mWalkMarkerDic.Keys.Count;
        mFloorKey = new int[mFloorNum];
        bw.Write(mFloorNum);
        int index = 0;
        foreach (int key in mWalkMarkerDic.Keys)
        {
            bw.Write(key);
            mFloorKey[index] = key;
            index++;
        }
        return true;
    }

    public bool WriterWalkMarker(BinaryWriter bw)
    {
        if (bw == null || mFloorKey == null)
        {
            return false;
        }
        int key = 0;
        for (int k = 0; k < mFloorNum; ++k)
        {
            key = mFloorKey[k];
            for (int i = 0; i < mTerrainConfig.tileSize; ++i)
            {
                for (int j = 0; j < mTerrainConfig.tileSize; ++j)
                {
                    bw.Write(mWalkMarkerDic[key][i, j]);
                }
            }
        }
        return true;
    }

    public bool WriterFloorHeight(BinaryWriter bw)
    {
        if (bw == null || mFloorKey == null)
        {
            return false;
        }

        int key = 0;
        for (int k = 0; k < mFloorNum; ++k)
        {
            key = mFloorKey[k];
            for (int i = 0; i < mTerrainConfig.tileSize; ++i)
            {
                for (int j = 0; j < mTerrainConfig.tileSize; ++j)
                {
                    bw.Write(mFloorHeightDic[key][i, j]);
                }
            }
        }
        return true;
    }

    public bool WriterSpaceHeight(BinaryWriter bw)
    {
        if (bw == null || mFloorKey == null)
        {
            return false;
        }

        int key = 0;
        for (int k = 0; k < mFloorNum; ++k)
        {
            key = mFloorKey[k];
            for (int i = 0; i < mTerrainConfig.tileSize; ++i)
            {
                for (int j = 0; j < mTerrainConfig.tileSize; ++j)
                {
                    bw.Write(mFloorSpaceDic[key][i, j]);
                }
            }
        }
        return true;
    }

    public bool ReadFloorInfo(BinaryReader br)
    {
        if (br == null)
        {
            return false;
        }
        mFloorNum = br.ReadInt32();
        mFloorKey = new int[mFloorNum];
        for (int i = 0; i < mFloorNum; ++i)
        {
            mFloorKey[i] = br.ReadInt32();
        }
        return true;
    }

    public bool ReadWalkMarker(BinaryReader br)
    {
        if (br == null || mFloorKey == null)
        {
            return false;
        }
        mWalkMarkerDic.Clear();
        int key = 0;
        for (int k = 0; k < mFloorNum; ++k)
        {
            key = mFloorKey[k];
            int[,] arrWalkMarker = new int[mTerrainConfig.tileSize, mTerrainConfig.tileSize];
            for (int i = 0; i < mTerrainConfig.tileSize; ++i)
            {
                for (int j = 0; j < mTerrainConfig.tileSize; ++j)
                {
                    arrWalkMarker[i, j] = br.ReadInt32();
                }
            }
            mWalkMarkerDic[key] = arrWalkMarker;
        }
        return true;
    }

    public bool ReadFloorHeight(BinaryReader br)
    {
        if (br == null || mFloorKey == null)
        {
            return false;
        }
        mFloorHeightDic.Clear();
        int key = 0;
        for (int k = 0; k < mFloorNum; ++k)
        {
            key = mFloorKey[k];
            float[,] arrFloorHeight = new float[mTerrainConfig.tileSize, mTerrainConfig.tileSize];
            for (int i = 0; i < mTerrainConfig.tileSize; ++i)
            {
                for (int j = 0; j < mTerrainConfig.tileSize; ++j)
                {
                    arrFloorHeight[i, j] = br.ReadSingle();
                }
            }
            mFloorHeightDic[key] = arrFloorHeight;
        }
        return true;

    }

    public bool ReadSpaceHeight(BinaryReader br)
    {
        if (br == null || mFloorKey == null)
        {
            return false;
        }
        mFloorSpaceDic.Clear();
        int key = 0;
        for (int k = 0; k < mFloorNum; ++k)
        {
            key = mFloorKey[k];
            float[,] arrFloorSpace = new float[mTerrainConfig.tileSize, mTerrainConfig.tileSize];
            for (int i = 0; i < mTerrainConfig.tileSize; ++i)
            {
                for (int j = 0; j < mTerrainConfig.tileSize; ++j)
                {
                    arrFloorSpace[i, j] = br.ReadSingle();
                }
            }
            mFloorSpaceDic[key] = arrFloorSpace;
        }
        return true;
    }

    public void DrawGridData()
    {
        if (mWalkMarkerDic.Count <= 0 || mWalkMarkerDic.Count != mFloorHeightDic.Count || mWalkMarkerDic.Count != mFloorSpaceDic.Count)
        {
            LogSystem.LogWarning("TileWalkableData.DrawGridData 生成地形切片数据失败");
            return;
        }
        foreach(int key in mWalkMarkerDic.Keys)
        {
            DrawOneFloorGridData(key);
        }

    }

    Dictionary<int, GameObject> mGirdObjDic = new Dictionary<int, GameObject>();
    private void DrawOneFloorGridData(int key)
    {
        float tileSize = mTerrainConfig.tileSize;
        float gridSize = mTerrainConfig.tileSize / (float)mTerrainConfig.tileSize;
        int width = mTerrainConfig.tileSize;
        int height = mTerrainConfig.tileSize;
        int count = width * height;

        Vector3[] vertices = new Vector3[count * 4];
        Vector2[] uvs = new Vector2[count * 4];
        Color[] colors = new Color[count * 4];
        int[] triangles = new int[count * 6];

        int ind = 0;

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                float vx = i * gridSize - tileSize * 0.5f;
                float vy = mFloorHeightDic[key][i, j] + 0.5f;
                float vz = j * gridSize - tileSize * 0.5f;

                Vector3 pos1 = new Vector3(vx, vy, vz);
                vertices[ind] = pos1;
                Vector3 uv1 = new Vector2(0, 0);
                uvs[ind] = uv1;
                colors[ind] = GetGridColorByPos(key, i, j);
                ind++;

                Vector3 pos2 = new Vector3(vx + gridSize, vy, vz);
                vertices[ind] = pos2;
                Vector3 uv2 = new Vector2(1, 0);
                uvs[ind] = uv2;
                colors[ind] = GetGridColorByPos(key, i, j);
                ind++;

                Vector3 pos3 = new Vector3(vx, vy, vz + gridSize);
                vertices[ind] = pos3;
                Vector3 uv3 = new Vector2(0, 1);
                uvs[ind] = uv3;
                colors[ind] = GetGridColorByPos(key, i, j);
                ind++;

                Vector3 pos4 = new Vector3(vx + gridSize, vy, vz + gridSize);
                vertices[ind] = pos4;
                Vector3 uv4 = new Vector2(1, 1);
                uvs[ind] = uv4;
                colors[ind] = GetGridColorByPos(key, i, j);
                ind++;
            }
        }
        
        for (int i = 0; i < count; ++i)
        {
            triangles[i * 6] = i * 4;
            triangles[i * 6 + 1] = i * 4 + 2;
            triangles[i * 6 + 2] = i * 4 + 3;

            triangles[i * 6 + 3] = i * 4;
            triangles[i * 6 + 4] = i * 4 + 3;
            triangles[i * 6 + 5] = i * 4 + 1;
        }

        GameObject gridObj = null;
        if (!mGirdObjDic.TryGetValue(key, out gridObj) || gridObj == null)
        {
            gridObj = new GameObject();
            gridObj.name = "GridData_" + mTileKey + "_FloorKey_" + key;
            gridObj.transform.position = new Vector3(mTilePos.x, 0f, mTilePos.z);
            gridObj.AddComponent<MeshFilter>();
            gridObj.AddComponent<MeshRenderer>();
            Shader gridShader = Shader.Find("Snail/Grid1");
            Material mat = new Material(gridShader);
            gridObj.GetComponent<Renderer>().material = mat;
            mGirdObjDic[key] = gridObj;
        }

        Mesh gridMesh = new Mesh();
        gridMesh.vertices = vertices;
        gridMesh.uv = uvs;
        gridMesh.triangles = triangles;
        gridMesh.colors = colors;
        gridObj.GetComponent<MeshFilter>().mesh = gridMesh;
    }

    private int DefFloor = 8; // 默认第0层（绿色标识层及地表层）
    private Color GetGridColorByPos(int key, int row, int col)
    {
        if (mWalkMarkerDic[key][row, col] > 0)
        {
            return Color.red;
        }
        else if (key == DefFloor)
        {
            return Color.green;
        }
        else
        {
            return Color.blue;
        }
    }
}
