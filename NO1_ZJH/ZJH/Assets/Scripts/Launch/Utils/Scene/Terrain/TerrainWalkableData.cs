using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class TerrainWalkableData
{
    /// <summary>
    /// 最大行走层数
    /// </summary>
    public const int MAX_FLOOR_NUM = 15;

    /// <summary>
    /// 无效高度值
    /// </summary>
    public const int HEIGHT_NULL = -100000;    

    /// <summary>
    /// 层标记
    /// </summary>
    public const int FLOOR_MARKER_EXISTS = 0x1;
    public const int FLOOR_MARKER_MOVE = 0x2;
    public const int FLOOR_MARKER_STAND = 0x4;
    public const int FLOOR_MARKER_WALL = 0x8;

    /// <summary>
    /// 行走层标记
    /// 注：为了方便取第一层数据给寻路使用
    /// </summary>
    private List<byte[,]> mWalkMarkersList;

    /// <summary>
    ///  层高度
    /// </summary>
    private List<float[,]> mFloorHeightsList;

    /// <summary>
    /// 无障碍空间高度
    /// </summary>
    private List<float[,]> mSpaceHeightsList;      

    private int mWidth = 0;
    private int mHeight = 0;
    private int mFloorCount = 0;

    private bool isPlaying = true;
    private int mHalfWidth = 0;
    private int mHalfHeight = 0;
    private float mGridSize = 0f;

    public int HalfWidth
    {
        get
        {
            return mHalfWidth;
        }
    }

    public int HalfHeight
    {
        get
        {
            return mHalfHeight;
        }
    }

    /**************************************************************************************************************
     * 注解 : 格子数量是2的倍数
     ***************************************************************************************************************/
    public TerrainWalkableData(float gridSize, int floorCount = 1, int width = 0, int height = 0)
    {
        this.mFloorCount = floorCount;
        this.mWidth = width;
        this.mHeight = height;

//         this.mGridSize = gridSize;
//         this.isPlaying = Application.isPlaying;
        this.mHalfWidth = width / 2;
        this.mHalfHeight = height / 2;
    }

    public List<byte[,]> GetWalkMarkersList()
    {
        return mWalkMarkersList;
    }

    public List<float[,]> GetFloorHeightsList()
    {
        return mFloorHeightsList;
    }

    public List<float[,]> GetSpaceHeightsList()
    {
        return mSpaceHeightsList;
    }


    public int GetWidth()
    {
         return mWidth;
    }

    public int GetHeight()
    {
        return mHeight;
    }


    /// <summary>
    /// 获得总的行走层数量
    /// </summary>
    public int GetFloorCount()
    {
        return mFloorCount;
    }

    /// <summary>
    /// 获得行走标记
    /// </summary>
    public int GetWalkMarker(int row, int col, int floor)
    {
        if (row < 0 || row >= mWidth || col < 0 || col >= mHeight)
        {
            return 0;
        }
        if (floor < 0 || mWalkMarkersList == null || mWalkMarkersList.Count <= floor)
        {
            return 0;
        }

        return mWalkMarkersList[floor][row, col];
    }

    /// <summary>
    ///  获得行走层高度
    /// </summary>
    public float GetFloorHeight(int row, int col, int floor)
    {
        if (row < 0 || row >= mWidth || col < 0 || col >= mHeight)
        {
            return 0;
        }
        if (floor < 0 || mFloorHeightsList == null || mFloorHeightsList.Count <= floor)
        {
            return 0;
        }
        return mFloorHeightsList[floor][row, col];
    }

    /// <summary>
    /// 获得行走层的无障碍高度
    /// </summary>
    public float GetFloorSpace(int row, int col, int floor)
    {
        if (row < 0 || row >= mWidth || col < 0 || col >= mHeight)
        {
            return 0;
        }
        if (floor < 0 || mSpaceHeightsList == null || mSpaceHeightsList.Count <= floor)
        {
            return 0;
        }

        return mSpaceHeightsList[floor][row, col];
    }

    /// <summary>
    ///  创建高度数据
    /// </summary>
    public bool BuildFloorHeight(int floor, float[,] floorHeight)
    {
        if (floorHeight == null || floorHeight.GetLength(0) != mWidth || floorHeight.GetLength(1) != mHeight)
        {
            return false;
        }
        if (mFloorHeightsList == null)
        {
            mFloorHeightsList = new List<float[,]>();
        }
        if (mFloorHeightsList.Count > floor)
        {
            mFloorHeightsList[floor] = floorHeight;
        }
        else
        {
            mFloorHeightsList.Add(floorHeight);
        }
        return true;
    }

    /// <summary>
    ///  创建高度数据
    /// </summary>
    public bool BuildSpaceHeight(int floor, float[,] spaceHeight)
    {
        if (spaceHeight == null || spaceHeight.GetLength(0) != mWidth || spaceHeight.GetLength(1) != mHeight)
        {
            return false;
        }
        if (mSpaceHeightsList == null)
        {
            mSpaceHeightsList = new List<float[,]>();
        }
        if (mSpaceHeightsList.Count > floor)
        {
            mSpaceHeightsList[floor] = spaceHeight;
        }
        else
        {
            mSpaceHeightsList.Add(spaceHeight);
        }
        return true;
    }

    /// <summary>
    ///  创建标记数据
    /// </summary>
    public bool BuildWalkMarker(int floor, byte[,] walkMarkers)
    {
        if (walkMarkers == null || walkMarkers.GetLength(0) != mWidth || walkMarkers.GetLength(1) != mHeight)
        {
            return false;
        }
        if (mWalkMarkersList == null)
        {
            mWalkMarkersList = new List<byte[,]>();
        }
        if (mWalkMarkersList.Count > floor)
        {
            mWalkMarkersList[floor] = walkMarkers;
        }
        else
        {
            mWalkMarkersList.Add(walkMarkers);
        }
        return true;
    }

    /// <summary>
    /// 读取行走标记数据
    /// </summary>
    public bool ReadWalkMarker(BinaryReader br)
    {
        if (br == null)
        {
            return false;
        }
        if (mWalkMarkersList == null)
        {
            mWalkMarkersList = new List<byte[,]>();
        }

        int value = 0;
        for (int k = 0; k < mFloorCount; ++k)
        {
            if (mWalkMarkersList.Count >= k)
            {
                mWalkMarkersList.Add(new byte[mWidth, mHeight]);
            }
            else
            {
                mWalkMarkersList[k] = new byte[mWidth, mHeight];
            }
            for (int i = 0; i < mWidth; ++i)
            {
                for (int j = 0; j < mHeight; ++j)
                {
                    // 读取原始格子数据
                    value = br.ReadInt32();

                    mWalkMarkersList[k][i, j] = (byte)value;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 读取层高
    /// </summary>
    public bool ReadFloorHeight(BinaryReader br)
    {
        if (br == null)
        {
            return false;
        }

        if (mFloorHeightsList == null)
        {
            mFloorHeightsList = new List<float[,]>();
        }
        float value = 0;
        for (int k = 0; k < mFloorCount; ++k)
        {
            if (mFloorHeightsList.Count >= k)
            {
                mFloorHeightsList.Add(new float[mWidth, mHeight]);
            }
            else
            {
                mFloorHeightsList[k] = new float[mWidth, mHeight];
            }

            for (int i = 0; i < mWidth; ++i)
            {
                for (int j = 0; j < mHeight; ++j)
                {
                    // 读取原始格子高度数据
                    value = br.ReadSingle();
                    mFloorHeightsList[k][i, j] = value;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 读取无障碍高度
    /// </summary>
    public bool ReadSpaceHeight(BinaryReader br)
    {
        if (br == null)
        {
            return false;
        }

        if (mSpaceHeightsList == null)
        {
            mSpaceHeightsList = new List<float[,]>();
        }
        float value = 0;
        for (int k = 0; k < mFloorCount; ++k)
        {
            if (mSpaceHeightsList.Count >= k)
            {
                mSpaceHeightsList.Add(new float[mWidth, mHeight]);
            }
            else
            {
                mSpaceHeightsList[k] = new float[mWidth, mHeight];
            }

            for (int i = 0; i < mWidth; ++i)
            {
                for (int j = 0; j < mHeight; ++j)
                {
                    // 读取原始格子高度数据
                    value = br.ReadSingle();
                    mSpaceHeightsList[k][i, j] = value;
                }
            }
        }
        return true;
    }

    public bool WriterWalkMarker(BinaryWriter bw)
    {
        if (bw == null)
        {
            return false;
        }
        for (int k = 0; k < mFloorCount; ++k)
        {
            for (int i = 0; i < mWidth; ++i)
            {
                for (int j = 0; j < mHeight; ++j)
                {
                    bw.Write(mWalkMarkersList[k][i, j]);
                }
            }
        }
        return true;
    }

    public bool WriterFloorHeight(BinaryWriter  bw)
    {
        if (bw == null || mFloorHeightsList == null)
        {
            return false;
        }

        for (int k = 0; k < mFloorCount; ++k)
        {
            for (int i = 0; i < mWidth; ++i)
            {
                for (int j = 0; j < mHeight; ++j)
                {
                    bw.Write(mFloorHeightsList[k][i, j]);
                }
            }
        }
        return true;
    }

    public bool WriterSpaceHeight(BinaryWriter  bw)
    {
        if (bw == null || mSpaceHeightsList == null)
        {
            return false;
        }
        for (int k = 0; k < mFloorCount; ++k)
        {
            for (int i = 0; i < mWidth; ++i)
            {
                for (int j = 0; j < mHeight; ++j)
                {
                    bw.Write(mSpaceHeightsList[k][i, j]);
                }
            }
        }
        return true;
    }

    public void GenarateTerrainWalkableData(TerrainConfig terrainConfig, int startRx, int startRy, int endRx, int endRy, GameScene scene)
    {
        if (terrainConfig == null || scene== null) return;

        Dictionary<int, byte[,]> walkMarkerDic = new Dictionary<int,byte[,]>();
        Dictionary<int, float[,]> floorHeightDic = new Dictionary<int, float[,]>();
        Dictionary<int, float[,]> spaceHeightDic = new Dictionary<int, float[,]>();

        for (int i = startRx; i <= endRx; i++)
        {
            for (int j = startRy; j <= endRy; j++)
            {
                for (int m = 0; m < terrainConfig.tileCountPerSide; m++)
                {
                    for (int n = 0; n < terrainConfig.tileCountPerSide; n++)
                    {
                        uint ukey = GameScene.GetKey(i , j, (m - 2), (n - 2));
                        Tile tile = scene.FindTile(ukey);
                        if (tile != null && tile.mTileWalkableData != null)
                        {
                            int tileGridStartX = ((i - startRx) * terrainConfig.tileCountPerSide + m) * terrainConfig.tileSize;
                            int tileGridStartY = ((j - startRy) * terrainConfig.tileCountPerSide + n) * terrainConfig.tileSize;
                            GenarateWalkMarkerByTile(tileGridStartX, tileGridStartY, terrainConfig.tileSize, tile.mTileWalkableData, walkMarkerDic);
                            GenarateFloorHeightByTile(tileGridStartX, tileGridStartY, terrainConfig.tileSize, tile.mTileWalkableData, floorHeightDic);
                            GenarateSpaceHeightByTile(tileGridStartX, tileGridStartY, terrainConfig.tileSize, tile.mTileWalkableData, spaceHeightDic);
                        }
                    }
                }
            }
        }
        if (walkMarkerDic.Count != floorHeightDic.Count && walkMarkerDic.Count != spaceHeightDic.Count)
        {
            LogSystem.LogWarning("TerrainWalkableData.GenarateTerrainWalkableData 生存地形数据错误 ");
            return;
        }
        List<int> floorKey = new List<int>();
        foreach (int key in spaceHeightDic.Keys)
        {
            if (!floorKey.Contains(key))
            {
                floorKey.Add(key);
            }
        }
        floorKey.Sort();
        mFloorCount = floorKey.Count;
        for (int i = 0; i < mFloorCount; ++i)
        {
            BuildFloorHeight(i, floorHeightDic[floorKey[i]]);
            BuildSpaceHeight(i, spaceHeightDic[floorKey[i]]);
            BuildWalkMarker(i, walkMarkerDic[floorKey[i]]);
        }
        floorKey.Clear();
    }

    public void GenarateWalkMarkerByTile(int tileRx,  int tileRy, int gridR, TileWalkableData tileWalkData, Dictionary<int, byte[,]> walkMarkerDic)
    {
        if (tileWalkData == null || walkMarkerDic == null)
        {
            return;
        }
        Dictionary<int, int[,]> mTileWalkData = tileWalkData.GetWalkMarkerDic();
        foreach(KeyValuePair<int, int[,]> data in mTileWalkData)
        {
            if (data.Value == null)
            {
                continue;
            }
            byte[,] arrWalkMarker = null;
            if (!walkMarkerDic.TryGetValue(data.Key, out arrWalkMarker) || arrWalkMarker == null)
            {
                arrWalkMarker = new byte[mWidth, mHeight];
            }

            for (int x = 0; x < gridR; ++x)
            {
                for (int y = 0; y < gridR; ++y)
                {
                    arrWalkMarker[tileRx + x, tileRy + y] = (byte)data.Value[x, y];
                }
            }
            walkMarkerDic[data.Key] = arrWalkMarker;
        }
    }

    public void GenarateFloorHeightByTile(int tileRx, int tileRy, int gridR, TileWalkableData tileWalkData, Dictionary<int, float[,]> floorHeightDic)
    {
        if (tileWalkData == null && floorHeightDic == null)
        {
            return;
        }
        Dictionary<int, float[,]> floorHeightData = tileWalkData.GetFloorHeightDic();
        foreach (KeyValuePair<int, float[,]> data in floorHeightData)
        {
            if (data.Value == null)
            {
                continue;
            }
            float[,] arrFloorHeight = null;
            if (!floorHeightDic.TryGetValue(data.Key, out arrFloorHeight) || arrFloorHeight == null)
            {
                arrFloorHeight = new float[mWidth, mHeight];
                for (int i = 0; i < mWidth; ++i)
                {
                    for (int j = 0; j < mHeight; ++j)
                    {
                        arrFloorHeight[i, j] = 0;
                    }
                }
            }

            for (int x = 0; x < gridR; ++x)
            {
                for (int y = 0; y < gridR; ++y)
                {
                    arrFloorHeight[tileRx + x, tileRy + y] = data.Value[x, y];
                }
            }
            floorHeightDic[data.Key] = arrFloorHeight;
        }
    }

    public void GenarateSpaceHeightByTile(int tileRx, int tileRy, int gridR, TileWalkableData tileWalkData, Dictionary<int, float[,]> spaceHeightDic)
    {
        if (tileWalkData == null)
        {
            return;
        }
        Dictionary<int, float[,]> spaceHeightData = tileWalkData.GetFloorSpaceDic();
        foreach (KeyValuePair<int, float[,]> data in spaceHeightData)
        {
            if (data.Value == null)
            {
                continue;
            }
            float[,] arrSpaceHeight = null;
            if (!spaceHeightDic.TryGetValue(data.Key, out arrSpaceHeight) || arrSpaceHeight == null)
            {
                arrSpaceHeight = new float[mWidth, mHeight];
            }

            for (int x = 0; x < gridR; ++x)
            {
                for (int y = 0; y < gridR; ++y)
                {
                    arrSpaceHeight[tileRx + x, tileRy + y] = data.Value[x, y];
                }
            }
            spaceHeightDic[data.Key] = arrSpaceHeight;
        }
    }

    /****************************************************************************************************
    * 功能 ：预处理行走阻塞状态
    *****************************************************************************************************/
    public void PrepareForPathSearch()
    {
        for (int key = 0; key < mWalkMarkersList.Count; ++key)
        {
            for (int i = 0; i < mWidth; i++)
            {
                for (int j = 0; j < mHeight; j++)
                {
                    PretestWalkBlocker(key, i, j);
                }
            }
        }
    }


    /********************************************************************************************************
     * 功能 ： 预处理格子阻塞情况  注意：代码逻辑有问题，为了保持版本兼容沿用以前方式
     ***********************************************************************************************************/
    /** 动态单位最大的碰撞尺寸 */
    private int maxDynamicCollisizeSize = 2;
    private int gridTypeMask = 5;
    private int fullMask = 2;
    public void PretestWalkBlocker(int key, int x, int y, int type = 0)
    {
        if (mWalkMarkersList == null || mWalkMarkersList.Count <= key ||
            x < 0 || x >= mWidth || y < 0 || y >= mHeight)
        {
            LogSystem.LogWarning("TerrainWalkableData.PretestWalkBlocker 预处理格子阻塞数据失败");
            return;
        }

        int grid = 0;
        bool blocker = false;
        int collisionSize;
        if (mWalkMarkersList[key][x, y] == 0)
            return;

        // 获取当前格子的阻塞状态
        blocker = ((mWalkMarkersList[key][x, y] & 1) > 0);

        int m = 0;
        int n = 0;

        // 检测此区域能否占当前尺寸的单位
        // 遍历各尺寸的碰撞体积，测试此格子的阻塞状态，是否四通八达
        // ----------
        // |        |       检测上下左右阻塞状态
        // |   +    |       
        // |        |       如果当前级别的物体遇到了阻塞，更大的物体将也不能在当前格子中通过
        // ----------       
        for (collisionSize = 1; collisionSize <= maxDynamicCollisizeSize; collisionSize++)
        {
            if (blocker)
                break;

            // 左边的阻塞情况
            for (int i = -collisionSize; i <= collisionSize; i++)
            {
                m = x - collisionSize;
                n = y + i;
                if ((m >= 0 && m < mWidth) && (n >= 0 && n < mHeight))
                {
                    grid = mWalkMarkersList[key][m, n];
                    if ((grid & 1) > 0)
                    {
                        blocker = true;
                        break;
                    }
                }
            }
            if (blocker)
                break;

            // 右边的阻塞情况
            for (int i = -collisionSize; i <= collisionSize; i++)
            {
                m = x + collisionSize;
                n = y + i;
                if ((m >= 0 && m < mWidth) && (n >= 0 && n < mHeight))
                {
                    grid = mWalkMarkersList[key][m, n];
                    if (((grid & 1) > 0))
                    {
                        blocker = true;
                        break;
                    }
                }
            }
            if (blocker)
                break;

            // 上边的阻塞情况
            for (int i = -collisionSize; i <= collisionSize; i++)
            {
                m = x + i;
                n = y + collisionSize;
                if ((m >= 0 && m < mWidth) && (n >= 0 && n < mHeight))
                {
                    grid = mWalkMarkersList[key][m, n];
                    if (((grid & 1) > 0))
                    {
                        blocker = true;
                        break;
                    }
                }
            }
            if (blocker)
                break;

            // 下边的阻塞情况
            for (int i = -collisionSize; i <= collisionSize; i++)
            {
                m = x + i;
                n = y - collisionSize;
                if ((m >= 0 && m < mWidth) && (n >= 0 && n < mHeight))
                {
                    grid = mWalkMarkersList[key][m, n];
                    if (((grid & 1) > 0))
                    {
                        blocker = true;
                        break;
                    }
                }
            }
            if (blocker)
                break;

            // 如果各个方向都没有阻塞，则返回false
            if ((mWalkMarkersList[key][x, y] & (1 << collisionSize)) != 0)
                mWalkMarkersList[key][x, y] -= (byte)(1 << collisionSize);
        }

        // 如果有阻塞, 从当前有阻塞的那段开始起记录阻塞状态
        if (blocker == true)
        {
            for (int k = collisionSize; k < maxDynamicCollisizeSize; k++)
                mWalkMarkersList[key][x, y] |= (byte)(1 << k);

            mWalkMarkersList[key][x, y] |= (byte)(type << gridTypeMask);
        }
        else
        {
            if ((mWalkMarkersList[key][x, y] & fullMask) == 0)
                mWalkMarkersList[key][x, y] = 0;
        }
    }

    public void DrawGridData(TerrainConfig terrainConfig)
    {
        int sumX = mWidth / terrainConfig.tileSize;    // 需要能整除
        int sumY = mHeight / terrainConfig.tileSize;   // 需要能整除
        int index = 0;
        int width = mWidth / sumX;      // 需要能整除
        int height = mHeight / sumY;    // 需要能整除
        for (int k = 0; k < mFloorCount; ++k)
        {
            for (int i = 0; i < sumX; ++i)
            {
                for (int j = 0; j < sumY; ++j)
                {
                    DrawOneFloorGridData(terrainConfig, k, index, i * width, j * height, width, height);
                    index++;
                }
                
            }
        }
    }

    /// <summary>
    /// 显示格子数据
    /// </summary>
    Dictionary<int, GameObject> mGirdObjDic = new Dictionary<int, GameObject>();
    public void DrawOneFloorGridData(TerrainConfig terrainConfig, int key, int index, int x, int z, int width, int height)
    {
        float gridSize = terrainConfig.gridSize;
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
                float vx = i * gridSize;
                float vy = mFloorHeightsList[key][i + x, j + z] + 0.2f;
                float vz = j * gridSize;

                Vector3 pos1 = new Vector3(vx, vy, vz);
                vertices[ind] = pos1;
                Vector3 uv1 = new Vector2(0, 0);
                uvs[ind] = uv1;
                colors[ind] = GetGridColorByPos(key, i + x, j + z);
                ind++;

                Vector3 pos2 = new Vector3(vx + gridSize, vy, vz);
                vertices[ind] = pos2;
                Vector3 uv2 = new Vector2(1, 0);
                uvs[ind] = uv2;
                colors[ind] = GetGridColorByPos(key, i + x, j + z);
                ind++;

                Vector3 pos3 = new Vector3(vx, vy, vz + gridSize);
                vertices[ind] = pos3;
                Vector3 uv3 = new Vector2(0, 1);
                uvs[ind] = uv3;
                colors[ind] = GetGridColorByPos(key, i + x, j + z);
                ind++;

                Vector3 pos4 = new Vector3(vx + gridSize, vy, vz + gridSize);
                vertices[ind] = pos4;
                Vector3 uv4 = new Vector2(1, 1);
                uvs[ind] = uv4;
                colors[ind] = GetGridColorByPos(key, i + x, j + z);
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
        if (!mGirdObjDic.TryGetValue(index, out gridObj) || gridObj == null)
        {
            gridObj = new GameObject();
            gridObj.AddComponent<MeshFilter>();
            gridObj.AddComponent<MeshRenderer>();
            Shader gridShader = Shader.Find("Snail/Grid1");
            Material mat = new Material(gridShader);
            gridObj.GetComponent<Renderer>().material = mat;
            mGirdObjDic[index] = gridObj;
        }
        gridObj.name = "GridData_" + index + "_FloorKey_" + key;
        gridObj.transform.position = new Vector3(x - mWidth / 2, 0, z - mHeight / 2);

        Mesh gridMesh = new Mesh();
        gridMesh.vertices = vertices;
        gridMesh.uv = uvs;
        gridMesh.triangles = triangles;
        gridMesh.colors = colors;
        gridObj.GetComponent<MeshFilter>().mesh = gridMesh;
    }

    private int DefFloor = 0; // 默认第0层（绿色标识层及地表层）
    private Color GetGridColorByPos(int key, int row, int col)
    {
        if (mWalkMarkersList[key][row, col] > 0)
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
