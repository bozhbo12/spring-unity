using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.IO;
/******************************************************************************************************
 * 功能 : 寻路
 *******************************************************************************************************/
public class MapPath
{
    //private string Version = "2.0";
    public int width;
    public int height;
    private float gridSize;

    public int halfWidth;
    public int halfHeight;

    /** 尺寸格子数据由不同二进制位存储 */
    public byte[,] grids;

    /** 动态单位最大的碰撞尺寸 */
    public int maxDynamicCollisizeSize = 2;

    public int gridTypeMask = 5;

    private int fullMask = 0;

    /**************************************************************************************************************
     * 注解 : 格子数量是2的倍数
     ***************************************************************************************************************/
    public MapPath(float gridSize, int width = 0, int height = 0)
    {
        int i = 0;
        int j = 0;

        for (i = 0; i < maxDynamicCollisizeSize; i++)
        {
            fullMask |= (1 << i);
        }
        this.width = width;
        this.height = height;
        this.halfWidth = width / 2;
        this.halfHeight = height / 2;
        this.gridSize = gridSize;

        grids = new byte[width, height];
        Array.Clear(grids, 0, grids.Length);
        // 初始化格子数据
        //for (i = 0; i < width; i++)
        //{
        //    for (j = 0; j < height; j++)
        //    {
        //        grids[i, j] = 0;
        //    }
        //}

        // 初始化路径格子数据
        if (path == null)
        {
            path = new int[this.width, this.height];
            for (i = 0; i < width; i++)
            {
                for (j = 0; j < height; j++)
                {
                    path[i, j] = -1;
                }
            }
        }

        // 
        for (i = 0; i < cacheCount; i++)
        {
            HValueCache.Add(new HValue());
        }
    }

    private int[,] path = null;                                     // 路径缓存
    private int cacheIndex = 0;
    private int cacheCount = 10000;
    private List<HValue> HValueCache = new List<HValue>(10000);
    private HValue getHValue(int x, int y)
    {
        if (path[x, y] == -1)
        {
            if (cacheIndex == cacheCount)
            {
                for (int i = 0; i < 1000; i++)
                {
                    HValue hv = new HValue();
                    hv.Reset();
                    HValueCache.Add(hv);
                }
                cacheCount += 1000;
            }
            path[x, y] = cacheIndex;
            cacheIndex++;
        }
        return HValueCache[path[x, y]];
    }

    /*************************************************************************************************************
     * 功能 : 每次寻路前初始化寻路数据, 寻路结束后释放初始化数据节约内存
     *************************************************************************************************************/
    public void Init()
    {
        /**
        if (path == null)
        {
            int i = 0;
            int j = 0;
            path = new HValue[this.width, this.height];

            for (i = 0; i < width; i++)
            {
                for (j = 0; j < height; j++)
                {
                    path[i, j] = new HValue();
                }
            }
        }
        **/
    }

    /***************************************************************************************************************
     * 功能 : 寻路结束后释放 路径数组
     ***************************************************************************************************************/
    public void Reset()
    {

    }

    /*****************************************************************************************************************
     * 功能 : 读取预计算好的格子数据
     ******************************************************************************************************************/
    public void Read(BinaryReader br)
    {
        if (br != null)
        {
            width = br.ReadInt32();
            height = br.ReadInt32();
            halfWidth = width / 2;
            halfHeight = height / 2;
            grids = new byte[width, height];

            // 初始化格子数据
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int iValue = br.ReadInt32();
                    grids[i, j] = (byte)iValue;
                }
            }
        }
    }

    /******************************************************************************************************************
     * 功能 : 修正自定义格子
     ********************************************************************************************************************/
    public int[,] CheckCustomGrids(int[,] customGrids)
    {
        // 计算中心位置的格子
        int ind = 0;
        int i = 0;
        int j = 0;
        int len = customGrids.GetLength(0);
        List<int> newGrids = new List<int>();
        for (ind = 0; ind < len; ind++)
        {
            i = customGrids[ind, 0];
            j = customGrids[ind, 1];

            if (grids[i, j] != 3)
            {
                newGrids.Add(i);
                newGrids.Add(j);
            }
        }
        customGrids = new int[newGrids.Count / 2, 2];
        int dataInd = 0;
        for (ind = 0; ind < newGrids.Count; ind++)
        {
            i = newGrids[ind];
            j = newGrids[ind + 1];
            ind = ind + 1;
            customGrids[dataInd, 0] = i;
            customGrids[dataInd, 1] = j;
            dataInd++;
        }
        return customGrids;
    }

    /******************************************************************************************************************
     * 功能 : 设置自定义格子碰撞
     ********************************************************************************************************************/
    public void SetDynamicCollision(Vector3 worldPostion, int[,] customGrids, bool isRemove = false, int type = 0)
    {
        // 计算中心位置的格子
        int ind = 0;
        int i = 0;
        int j = 0;
        int len = customGrids.GetLength(0);
        for (ind = 0; ind < len; ind++)
        {
            i = customGrids[ind, 0];
            j = customGrids[ind, 1];

            // 修改除格子的阻塞数据
            if (isRemove == false)
            {
                if (grids[i, j] == 1 || grids[i, j] == 2)
                    continue;

                grids[i, j] = 1;              // 基础位已被占

                // 使任何尺寸的单位无法穿越
                for (int k = 1; k < maxDynamicCollisizeSize; k++)
                    grids[i, j] |= (byte)(1 << k);

                // 设置动态或静态格子标记
                //grids[i, j] |= (1 << gridTypeMask);
            }
            // 删除阻塞
            else
            {
                if ((grids[i, j] & 1) > 0)
                    grids[i, j] = 0;
            }
        }
        /**
        for (ind = 0; ind < customGrids.GetLength(0); ind++)
        {
            i = customGrids[ind, 0];
            j = customGrids[ind, 1];
            PretestWalkBlocker(i, j);
        }**/
    }

    /******************************************************************************************************************
     * 功能 : 插入动态单位的阻塞
     ********************************************************************************************************************/
    public void SetDynamicCollision(Vector3 worldPostion, int size = 1, bool isRemove = false, int type = 1)
    {
        try
        {
            int px = Mathf.FloorToInt(worldPostion.x / gridSize) + halfWidth;
            int py = Mathf.FloorToInt(worldPostion.z / gridSize) + halfHeight;

            int xPos = 0, yPos = 0;
            // 以当前位置为中心点, 设置周围的阻塞状况
            for (int i = -size; i <= size; i++)
            {
                for (int j = -size; j <= size; j++)
                {
                    xPos = px + i;
                    yPos = py + j;
                    if (xPos < 0 || yPos < 0 || xPos >= width || yPos >= height)
                        continue;

                    // 不改变静态格子数据
                    if ((grids[xPos, yPos] >> gridTypeMask) < 1 && grids[xPos, yPos] > 0)
                        continue;

                    // 修改除格子的阻塞数据
                    if (isRemove == false)
                    {
                        grids[xPos, yPos] = 1;              // 基础位已被占

                        for (int k = 1; k < maxDynamicCollisizeSize; k++)           // 有待优化,将grids[xPos, yPos]直接附上满值
                            grids[xPos, yPos] |= (byte)(1 << k);

                        // 设置动态或静态格子标记
                        grids[xPos, yPos] |= (byte)(type << gridTypeMask);
                    }
                    // 删除阻塞
                    else
                    {
                        if ((grids[xPos, yPos]) > 0)
                            grids[xPos, yPos] = 0;
                    }
                }
            }
        }
        catch (Exception e)
        {
            LogSystem.LogError(e.ToString());
        }

        // 当次格子被占用后 检测周边的被阻塞情况, 其他体积的单位是否能通行
        /**
        for (int i = -size - maxDynamicCollisizeSize - 1; i <= size + maxDynamicCollisizeSize + 1; i++)
        {
            for (int j = -size - maxDynamicCollisizeSize - 1; j <= size + maxDynamicCollisizeSize + 1; j++)
            {
                if ((grids[px + i, py + j] >> gridTypeMask) < 1 && grids[px + i, py + j] > 0)
                    continue;

                PretestWalkBlocker(px + i, py + j, type);
            }
        }**/
    }

    /********************************************************************************************************
     * 功能 ： 预处理格子阻塞情况
     ***********************************************************************************************************/
    public void PretestWalkBlocker(int x, int y, int type = 0)
    {
        int grid = 0;
        bool blocker = false;
        int collisionSize;
        //if (grids[x, y] == null)
        //    return;

        // 获取当前格子的阻塞状态
        blocker = ((grids[x, y] & 1) > 0);

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
                if ((m >= 0 && m < width) && (n >= 0 && n < height))
                {
                    grid = grids[m, n];
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
                if ((m >= 0 && m < width) && (n >= 0 && n < height))
                {
                    grid = grids[m, n];
                    if ((grid & 1) > 0)
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
                if ((m >= 0 && m < width) && (n >= 0 && n < height))
                {
                    grid = grids[m, n];
                    if ((grid & 1) > 0)
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
                if ((m >= 0 && m < width) && (n >= 0 && n < height))
                {
                    grid = grids[m, n];
                    if ((grid & 1) > 0)
                    {
                        blocker = true;
                        break;
                    }
                }
            }
            if (blocker)
                break;

            // 如果各个方向都没有阻塞，则返回false
            if ((grids[x, y] & (1 << collisionSize)) != 0)
                grids[x, y] -= (byte)(1 << collisionSize);
        }

        // 如果有阻塞, 从当前有阻塞的那段开始起记录阻塞状态
        if (blocker == true)
        {
            for (int k = collisionSize; k < maxDynamicCollisizeSize; k++)
                grids[x, y] |= (byte)(1 << k);

            grids[x, y] |= (byte)(type << gridTypeMask);
        }
        else
        {
            if ((grids[x, y] & fullMask) == 0)
                grids[x, y] = 0;
        }
    }

    /****************************************************************************************************
     * 功能 : 验证目标点是否可行走
     *****************************************************************************************************/
    public bool IsValidForWalk(int x, int y, int collisionSize)
    {
        try
        {
            if (x > width || y > height)
            {
                return false;
            }

            int val = grids[x, y] & (1 << collisionSize);

            // Debug.Log("实际位置 x->" + (x - halfWidth) * gridSize + " y-> " + (y - halfHeight) * gridSize);
            if (val < 1)
                return true;
            else
                return false;
        }
        catch (Exception e)
        {
            LogSystem.Log(e);
            return false;
        }
    }

    public bool IsValidForWalk(Vector3 postion, int collisionSize)
    {
        int x = Mathf.FloorToInt(postion.x / gridSize);
        int y = Mathf.FloorToInt(postion.z / gridSize);

        int gx = x + this.halfWidth;
        int gy = y + this.halfHeight;
        int val = grids[gx, gy] & (1 << collisionSize);
        return (val < 1);
    }

    public bool IsValidForWalk(float worldGridX, float worldGridZ, int collisionSize)
    {
        int x = Mathf.FloorToInt(worldGridX);
        int y = Mathf.FloorToInt(worldGridZ);

        if (Math.Abs(x) > this.halfWidth)
            return false;
        if (Math.Abs(y) > this.halfHeight)
            return false;

        int gx = x + this.halfWidth;
        int gy = y + this.halfHeight;
        int val = grids[gx, gy] & (1 << collisionSize);
        return (val < 1);
    }


    /****************************************************************************************************
     * 功能 ：预处理行走阻塞状态
     *****************************************************************************************************/
    public void PrepareForPathSearch()
    {
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
                PretestWalkBlocker(i, j);
    }

    public void Clear()
    {
        pathFindEnd = true;
        pathFindEndBack = null;
        path = null;
        grids = null;

        // 初始化两条路径
        for (int i = 0; i < 2; i++)
        {
            if (heap[i] != null)
            {
                heap[i].Clear();
                heap[i] = null;
            }
        }

        distanceRecord.Clear();
        distanceRecord = null;

        HValueCache.Clear();

        tryPath.Clear();
        tryPath = null;

        pointsList.Clear();
        pointsList = null;
    }

    /*************************************************************************************************************
     * 寻路属性相关
     **************************************************************************************************************/

    private HValue nearestTarget;                           // 最近的目标

    private Heap[] heap = new Heap[2];                     // 0 去的路径， 1 来的路径


    /*****************************************************************************************
     * 功能 ：获取坐标方向
     *******************************************************************************************/
    private int Sign(double value)
    {
        if (Math.Abs(value) < 0.001f) return 0;
        if (value > 0) return 1;
        if (value < 0) return -1;
        return 0;
    }

    /******************************************************************************************************
     * 功能 : 计算两点的距离
     ******************************************************************************************************/
    private float Distance(int x, int y, int tx, int ty)
    {
        return (float)Math.Sqrt((x - tx) * (x - tx) + (y - ty) * (y - ty));
    }

    private float HeapHFunction(int nx, int ny, int gx, int gy)
    {
        float h_diagonal, h, h_straight;
        h_diagonal = Math.Min(Math.Abs(nx - gx), Math.Abs(ny - gy));
        h_straight = Math.Abs(nx - gx) + Math.Abs(ny - gy);
        h = 1.414f * h_diagonal + 1f * (h_straight - 2f * h_diagonal);

        return h;
    }

    private int computeTick = 0;

    /** 寻路接口 */
    public bool pathFindResult = true;

    private HValue curPathValue;

    private int[] heapX = new int[2];
    private int[] heapY = new int[2];
    private int markBase = 10;

    private int[] dx = new int[8];
    private int[] dy = new int[8];

    private List<HValue> pathHValues = new List<HValue>();

    public bool pathFindEnd = true;

    public Vector3 pfStartPoint;
    public Vector3 pfEndPoint;
    public int pfCollisionSize = 0;

    public List<Vector3> paths = new List<Vector3>();
    /** 实例销毁监听器 */
    public delegate void PathFindEndBack(List<Vector3> paths);
    public PathFindEndBack pathFindEndBack;

    public void RequestPaths(Vector3 startPoint, Vector3 endPoint, int collisionSize, out List<Vector3> paths)
    {
        RequestPaths(startPoint, endPoint, collisionSize, null, 2000000);

        paths = this.paths;
    }

    /*************************************************************************************************************
     * 功能 : 请求寻路路径
     **************************************************************************************************************/
    public void RequestPaths(Vector3 startPoint, Vector3 endPoint, int collisionSize, PathFindEndBack pathFindEndBack = null, int maxComputeCount = 8000)
    {
        try
        {
#if UNITY_EDITOR
            LogSystem.Log("RequestPaths: 发起寻路!");
#endif

            pfStartPoint = startPoint;
            pfEndPoint = endPoint;
            pfCollisionSize = collisionSize;

            pathFindEnd = false;
            pathFindResult = true;
            this.pathFindEndBack = pathFindEndBack;

            int i = 0;
            int j = 0;

            paths.Clear();

            computeTick = 0;

            heapX = new int[2];
            heapY = new int[2];

            // 起点格子坐标
            heapX[0] = Mathf.FloorToInt(startPoint.x / gridSize) + halfWidth;
            heapY[0] = Mathf.FloorToInt(startPoint.z / gridSize) + halfHeight;

            // 终点格子坐标
            heapX[1] = Mathf.FloorToInt(endPoint.x / gridSize) + halfWidth;
            heapY[1] = Mathf.FloorToInt(endPoint.z / gridSize) + halfHeight;


            // 判断终点和起点是否重叠
            if (heapX[0] == heapX[1] && heapY[0] == heapY[1])
            {
                paths.Add(endPoint);
                pathFindEnd = true;

                if (pathFindEndBack != null)
                    pathFindEndBack(paths);
                return;
            }

            // 初始化两条路径
            for (i = 0; i < 2; i++)
            {
                if (heap[i] == null)
                    heap[i] = new Heap();
                heap[i].Clear();
            }

            // 重置
            for (i = 0; i < HValueCache.Count; i++)
            {
                HValueCache[i].Reset();    
            }

            for (i = 0; i < width; i++)
            {
                for (j = 0; j < height; j++)
                {
                    path[i, j] = -1;
                }
            }
            cacheIndex = 0;

            // 测试起点和终点是否被封闭
            for (int heapIndex = 0; heapIndex < 2; heapIndex++)
            {
                int sx = heapX[heapIndex];
                int sy = heapY[heapIndex];

                int tx = heapX[1 - heapIndex];
                int ty = heapY[1 - heapIndex];

                for (i = -1; i <= 1; i++)
                {
                    for (j = -1; j <= 1; j++)
                    {
                        int x = sx + i;
                        int y = sy + j;

                        Vector3 p1 = new Vector3((heapX[0] - halfWidth) * gridSize, 0.0f, (heapY[0] - halfHeight) * gridSize);
                        Vector3 p2 = new Vector3((heapX[1] - halfWidth) * gridSize, 0.0f, (heapY[1] - halfHeight) * gridSize);
                        Vector3 from = (heapIndex == 0) ? p1 : p2;
                        Vector3 target = new Vector3((x - halfWidth) * gridSize, 0.0f, (y - halfHeight) * gridSize);

                        from.y = target.y = 0;                                                  // 忽略Y值的

                        float distance = Vector3.Distance(from, target);

                        // 测试能移动的距离
                        float canMove = TryWalkDistance(from, target, collisionSize);

                        // 如果能到达的位置 小于指定距离， 开始记录
                        if (Math.Abs(canMove - distance) < 0.01f)
                        {
                            HValue phv = getHValue(x, y);
                            phv.Set((short)x, (short)y, distance, 0);                        // 设置格子及目标距离
                            phv.Mark = (short)(1 + heapIndex * markBase);               // 
                            phv.PreX = -1;                                     // 上一步寻路为0
                            phv.PreY = -1;
                            heap[heapIndex].Push(phv);                         // 存储来回两条线路的路径点
                        }
                    }
                }
            }

            nearestTarget = heap[0].Peek();

            // 创建8方向
            int dC = 8;
            dx = new int[dC];
            dy = new int[dC];
            for (i = 0; i < dC; i++)
            {
                float angle = i * 2.0f * (float)Math.PI / (float)dC;
                dx[i] = Sign(Math.Cos(angle));             // X方向 + - 1
                dy[i] = Sign(Math.Sin(angle));             // Y方向 + - 1
            }

            // 初始化路径
            pathHValues.Clear();


            RequestPathsImmed(pfStartPoint, pfEndPoint, pfCollisionSize, maxComputeCount);

        }
        catch (Exception e)
        {
            paths = new List<Vector3>();
            if (GameScene.isEditor == true)
                LogSystem.Log("寻路失败: 起始点 " + startPoint + " 目标点 : " + endPoint + " 错误消息 : " + e.Message);
        }
    }

    public void Update()
    {
        if (pathFindEnd == false)
            RequestPathsImmed(pfStartPoint, pfEndPoint, pfCollisionSize);
    }


    private void RequestPathsImmed(Vector3 startPoint, Vector3 endPoint, int collisionSize, int maxComputeCount = 8000)
    {
        int i = 0;
        int j = 0;

        // 如果来回两条路径的起始点存在
        while (heap[0].IsEmpty == false || heap[1].IsEmpty == false)
        {
            int heapIndex = 0;

            // 比较来回线路的路径长度选择 最终计算那条线路
            if (heap[0].Count < heap[1].Count)
                heapIndex = 0;
            else
                heapIndex = 1;

            if (heap[0].Count == 0) heapIndex = 1;
            if (heap[1].Count == 0) heapIndex = 0;
            {
                // 查找开销最小的节点
                HValue current = heap[heapIndex].Pop();
                current.Mark = (short)(heapIndex * markBase + 2);

                int arrivedX, arrivedY;                 // 目标点

                // 8方向寻路
                for (int d = 0; d < 8; d++)
                {
                    // 计算此方向的开销
                    float unitCost = (float)(dx[d] * dx[d] + dy[d] * dy[d]);
                    unitCost = (float)Math.Sqrt(unitCost);

                    arrivedX = current.X + dx[d];
                    arrivedY = current.Y + dy[d];

                    // 检测是否能到达位置
                    bool canArrived = true;
                    canArrived = this.IsValidForWalk(current.X, current.Y, collisionSize);

                    // 直角方向阻塞判断
                    if (Math.Abs(unitCost - 1) < 0.01f)
                        canArrived = this.IsValidForWalk(arrivedX, arrivedY, collisionSize);
                    // 斜角方向阻塞判断
                    else
                    {
                        for (int m = 0; m <= 1; m++)
                        {
                            for (int n = 0; n <= 1; n++)
                            {
                                canArrived = canArrived && this.IsValidForWalk(current.X + m * dx[d], current.Y + n * dy[d], collisionSize);
                            }
                        }
                    }

                    // 对可到达点进行标记
                    if (canArrived)
                    {
                        curPathValue = getHValue(arrivedX, arrivedY);

                        // mark = 1
                        if (curPathValue.Mark == -1)
                        {
                            float cost = current.Cost + unitCost;
                            float dis = HeapHFunction(arrivedX, arrivedY, heapX[1 - heapIndex], heapY[1 - heapIndex]);
                            curPathValue.Set((short)arrivedX, (short)arrivedY, cost, cost + dis);
                            curPathValue.PreX = current.X;
                            curPathValue.PreY = current.Y;
                            curPathValue.Mark = (short)(heapIndex * markBase + 1);                     // 标记为路径中
                            heap[heapIndex].Push(curPathValue);                              // 追加目标格子
                        }

                        if (curPathValue.Mark == heapIndex * markBase + 1)
                        {
                            float cost = current.Cost + unitCost;                       // 距离++
                            if (cost + 0.1f < curPathValue.Cost)            // 如果查找的目标点 距离小于以及查询的距离则进行重新记录
                            {
                                float dis = Distance(current.X, current.Y, arrivedX, arrivedY);
                                int pos = heap[heapIndex].Find(curPathValue);

                                curPathValue.Set((short)arrivedX, (short)arrivedY, cost, cost + dis);       // 记录格子的路径长度值
                                curPathValue.PreX = current.X;                              // 记录当前的路径点为上一个路径点
                                curPathValue.PreY = current.Y;
                                heap[heapIndex].Upper(pos);                                               // 升序
                            }
                        }

                        // if (curPathValue.Mark == heapIndex * markBase + 2)

                        computeTick++;

                        // 超出计算量下帧继续计算
                        if (computeTick > maxComputeCount && d == 7)
                        {
                            computeTick = 0;
                            return;
                        }

                        // 终点闭塞情况下
                        if (heapIndex == 0)
                        {
                            float dis1 = Distance(nearestTarget.X, nearestTarget.Y, heapX[1], heapY[1]);
                            float dis2 = Distance(arrivedX, arrivedY, heapX[1], heapY[1]);

                            // 通过距离判断离目标点最近点
                            if (dis2 < dis1)
                                nearestTarget = curPathValue;

                            float nearestKey = nearestTarget.Cost + 8 * dis1;
                            float currentKey = current.Cost + dis2;
                            
                            if (currentKey > nearestKey && heap[0].PushCount > 4096 && heap[1].IsEmpty == true)
                            {
                                current = nearestTarget;
                                pathHValues.Add(current);
                                // 追加到路径中去
                                while (true)
                                {
                                    if (current.PreX == -1 || current.PreY == -1)
                                        break;
                                    pathHValues.Add(getHValue(current.PreX, current.PreY));
                                    current = getHValue(current.PreX, current.PreY);
                                }

                                for (i = 0; i < pathHValues.Count / 2; i++)
                                {
                                    HValue temp = pathHValues[i];
                                    pathHValues[i] = pathHValues[pathHValues.Count - i - 1];
                                    pathHValues[pathHValues.Count - i - 1] = temp;
                                }

                                Vector3 endPoint2 = new Vector3((nearestTarget.X - halfWidth) * gridSize, 0.0f, (nearestTarget.Y - halfHeight) * gridSize);

                                // 创建路径
                                paths = BuildPath(pathHValues, startPoint, endPoint2, collisionSize);

                                pathFindEnd = true;

                                if (pathFindEndBack != null)
                                    pathFindEndBack(paths);
                                return;
                            }
                        }
                        // 来回路径重叠寻路结束
                        if (curPathValue.Mark >= 0 && curPathValue.Mark / 10 != heapIndex)
                        {
                            HValue[] ends = new HValue[2];
                            ends[heapIndex] = current;
                            ends[1 - heapIndex] = curPathValue;

                            paths.Clear();
                            current = ends[0];

                            pathHValues.Add(current);
                            // 追加到路径中去
                            while (true)
                            {
                                if (current.PreX == -1 || current.PreY == -1)
                                    break;
                                pathHValues.Add(getHValue(current.PreX, current.PreY));
                                current = getHValue(current.PreX, current.PreY);
                            }

                            for (i = 0; i < pathHValues.Count / 2; i++)
                            {
                                HValue temp = pathHValues[i];
                                pathHValues[i] = pathHValues[pathHValues.Count - i - 1];
                                pathHValues[pathHValues.Count - i - 1] = temp;
                            }

                            current = ends[1];
                            pathHValues.Add(current);
                            // 追加到路径中去
                            while (true)
                            {
                                if (current.PreX == -1 || current.PreY == -1)
                                    break;
                                pathHValues.Add(getHValue(current.PreX, current.PreY));
                                current = getHValue(current.PreX, current.PreY);
                            }

                            paths = BuildPath(pathHValues, startPoint, endPoint, collisionSize);
                          
                            pathFindEnd = true;

                            if (pathFindEndBack != null)
                                pathFindEndBack(paths);
                            return;
                        }
                    }
                }
            }
        }
    }

    // private List<GameObject> list = new List<GameObject>();

    /**************************************************************************************************************
     * 功能 ： 创建路径
     ****************************************************************************************************************/
    private List<Vector3> BuildPath(List<HValue> paths, Vector3 start, Vector3 end, int collisionSize)
    {
        // 初始化返回值
        List<Vector3> ret = new List<Vector3>();

        /**
        for (int i = 0; i < paths.Count; i++)
        {
            Vector3 pos = Vector3.zero;
            pos.x = (paths[i].X - halfWidth) * gridSize;
            pos.z = (paths[i].Y - halfHeight) * gridSize;
            pos.y = GameScene.mainScene.SampleHeight(pos);
            ret.Add(pos);
        }   

        return ret;
        **/

        // step 1:
        // 收集同一个方向上的点
        List<HValue> newPaths = new List<HValue>();
        for (int i = 0; i < paths.Count; i++)
        {
            bool add = true;
            if (0 < i && i + 1 < paths.Count)
            {
                int d1 = GetDirection(paths[i - 1], paths[i]);
                int d2 = GetDirection(paths[i], paths[i + 1]);
                if (d1 == d2)
                    add = false;
                else
                    add = true;

            }
            if (add)
                newPaths.Add(paths[i]);
        }
        paths = newPaths;
        newPaths = new List<HValue>();
        newPaths.Clear();

        // step 2: 
        // 创建真实世界中的路径
        List<Vector3> realPaths = new List<Vector3>();              // 真实世界空间中的坐标点

        realPaths.Add(start);

        int iPathCount = paths.Count;
        for ( int i= 0;i < iPathCount;i++)
        {
            HValue h = paths[i];
            Vector3 vTemp = Vector3.zero;
            vTemp.x = (h.X - halfWidth) * gridSize;
            vTemp.z = (h.Y - halfHeight) * gridSize;
            realPaths.Add(vTemp);
        }

        int count = realPaths.Count;
        if (count > 2)
        {
            if (Vector3.Distance(realPaths[0], realPaths[1]) < 0.1f)
                realPaths.RemoveAt(1);
        }
        //if (count == 0 || Vector3.Distance(start, realPaths[0]) > 0.1f)
        //realPaths.Insert(0, start);
        if (Vector3.Distance(end, realPaths[realPaths.Count - 1]) < 0.1f)
            realPaths.Add(end);

        // step 3:
        // 省略可跳过的点
        ret.Clear();
        for (int current = 0; current < realPaths.Count; )
        {
            ret.Add(realPaths[current]);
            int jumpTo = -1;
            for (int j = realPaths.Count - 1; j > current; j--)
            {
                Vector3 s = realPaths[current];
                Vector3 t = realPaths[j];
                s.y = t.y = 0;
                float actualMoveDis = this.TryWalkDistance(s, t, collisionSize);
                float fx = s.x - t.x;
                float fz = s.z - t.z;
                float fdist = fx * fx + fz * fz;
                if (Math.Abs(fdist - actualMoveDis* actualMoveDis) < 0.01f)
                {
                    jumpTo = j;
                    break;
                }
            }
            if (jumpTo == -1)
            {
                jumpTo = current + 1;
            }

            current = jumpTo;
            if (current == realPaths.Count - 1)
            {
                ret.Add(realPaths[current]);
                break;
            }
        }
        paths = newPaths;

        /**
        for (int i = 0; i < ret.Count; i++)
        {
            Vector3 pos = Vector3.zero;
            pos.x = ret[i].x;
            pos.z = ret[i].z;
            pos.y = GameScene.mainScene.SampleHeight(pos);

            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "path_" + i;

            go.transform.position = pos;
        }
        **/
        return ret;
    }

    /*********************************************************************************************************
     * 功能 ： 获取方向
     ************************************************************************************************************/
    private int GetDirection(HValue p, HValue c)
    {
        int x = c.X - p.X;
        int y = c.Y - p.Y;
        return (x + 1) * 3 + y;
    }

    /** 获取左边 */
    private float Left(float x)
    {
        float ret = Mathf.Floor(x);
        if (Math.Abs(x - ret) < 0.01f)
            return (x - 1);
        else
            return ret;
    }

    /** 获取右边值 */
    private float Right(float x)
    {
        return Mathf.FloorToInt(x + 1);
    }

    /** 获取键值 */
    private int GetKey(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.z);
        return x * baseValue + y;
    }

    /** 获取2D向量 */
    private Vector2 ToValue(int key)
    {
        Vector2 vTemp = Vector2.zero;
        vTemp.x = key / baseValue;
        vTemp.y = key % baseValue;
        return vTemp;
    }

    private Dictionary<int, float> distanceRecord = new Dictionary<int, float>();
    private List<Vector2> tryPath = new List<Vector2>();
    private List<int> pointsList = new List<int>();
    private const int baseValue = 2048;

    /**************************************************************************************************************
     * 功能 : 测试移动距离
     * 注解 : 
     ***************************************************************************************************************/
    public float TryWalkDistance(Vector3 start, Vector3 target, int collisionSize)
    {
        start.y = target.y = 0;                                                 // 关闭高度检测


        start = start / gridSize;                                               // 起始格子
        start.x += halfWidth;
        start.z += halfHeight;
        target = target / gridSize;                                             // 目标格子
        target.x += halfWidth;
        target.z += halfHeight;

        // 如果起始点为阻塞点将直接返回0
        if (this.IsValidForWalk((int)start.x, (int)start.z, 0) == false)
            return 0.0f;


        float gridDis = Vector3.Distance(start, target);             // 格子距离

        // 计算目标朝向及标准化目标朝向
        Vector3 direction = target - start;
        if (direction == Vector3.zero)
            return 0.0f;
        direction.Normalize();

        float moveDis = 0;
        float currentDis = 0;                                               // 格子距离
        Vector3 position = start;

        // 记录删除
        distanceRecord.Clear();
        pointsList.Clear();
        tryPath.Clear();

        // 记录从起点到结束点的路径点 --------------------------------------------------------------------------
        while (currentDis <= gridDis)
        {
            // 查询下一个点
            float xDis = 10000000.0f;
            float zDis = 10000000.0f;

            float testStep = 0.5f;

            // 计算x轴移动距离
            if (direction.x != 0)
            {
                if (direction.x > 0)
                {
                    float p = Right(position.x);
                    xDis = (p - position.x) / direction.x;
                    if (xDis <= 0)
                        xDis = (p + testStep - position.x) / direction.x;
                }
                else
                {
                    float p = Left(position.x);
                    xDis = (position.x - p) / -direction.x;
                    if (xDis <= 0)
                        xDis = (position.x + testStep - p) / -direction.x;
                }
            }

            // 计算y轴移动距离
            if (direction.z != 0)
            {
                if (direction.z > 0)
                {
                    float p = Right(position.z);
                    zDis = (p - position.z) / direction.z;
                    if (zDis <= 0)
                        zDis = (p + testStep - position.z) / direction.z;
                }
                else
                {
                    float p = Left(position.z);
                    zDis = (position.z - p) / -direction.z;
                    if (zDis <= 0)
                        zDis = (position.z + testStep - p) / -direction.z;
                }
            }

            if (xDis <= 0f || zDis <= 0f)
                throw new Exception("步伐应该大于0.");

            float choose = 0.0f;
            if (xDis < zDis)
                choose = xDis;
            else
                choose = zDis;

            moveDis = currentDis + choose * 0.5f;
            if (moveDis > gridDis)
                moveDis = gridDis;
            position = start + moveDis * direction;

            // 路径记录
            int key = GetKey(position);
            if (distanceRecord.ContainsKey(key) == false)
            {
                distanceRecord.Add(key, currentDis);
                pointsList.Add(key);    
                Vector2 vTemp = Vector2.zero;
                vTemp.x = position.x;
                vTemp.y = position.z;
                tryPath.Add(vTemp);
            }

            moveDis = currentDis + choose;
            if (moveDis > gridDis)
                moveDis = gridDis;
            position = start + moveDis * direction;
            key = GetKey(position);
            if (distanceRecord.ContainsKey(key) == false)
            {
                distanceRecord.Add(key, currentDis);
                pointsList.Add(key);
                Vector2 vTemp = Vector2.zero;
                vTemp.x = position.x;
                vTemp.y = position.z;
                tryPath.Add(vTemp);
            }

            // 记录当前行走的距离
            currentDis = moveDis;

            if (currentDis > (gridDis - 0.1f))
            {
                currentDis = gridDis;
                break;
            }
        }

        // 记录从起点到结束点的路径点 end ----------------------------------------------------------------------------

        int lastKey = -1;
        Vector2 lastPoint = Vector2.zero;

        // 如果其中有路径点不能行走,则直接返回
        for (int i = 0; i < pointsList.Count; i++)
        {
            int key = pointsList[i];
            Vector2 point = ToValue(key);

            if (lastKey != -1 && Math.Abs(lastPoint.x - point.x) + Math.Abs(lastPoint.y - point.y) == 2)
            {
                if (IsValidForWalk((int)lastPoint.x, (int)point.y, collisionSize) == false || IsValidForWalk((int)point.x, (int)lastPoint.y, collisionSize) == false)
                {
                    currentDis = distanceRecord[key];
                    break;
                }
            }

            // 如果寻路点发生阻塞则返回上一个距离
            if (IsValidForWalk((int)point.x, (int)point.y, collisionSize) == false)
            {
                currentDis = distanceRecord[key];
                break;
            }

            lastPoint = point;
            lastKey = key;
        }


        distanceRecord.Clear();
        tryPath.Clear();
        pointsList.Clear();

        return currentDis * gridSize;
    }
}