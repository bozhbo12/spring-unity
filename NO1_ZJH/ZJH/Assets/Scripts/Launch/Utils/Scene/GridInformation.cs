using System;
using System.Collections.Generic;
using System.Text;

public enum EGridType
{
    IsTree = 0,
    IsWalkBlocker = 1,
    IsFlyBlocker = 2,
    IsBuildBlocker = 3,
    IsDynamicWalkBlocker = 4,
    IsBlight = 5,
    IsNoWater = 6,
    IsCliff = 7,
}

/**************************************************************************************************
 * 功能 ： 格子信息
 ****************************************************************************************************/

//Flags table:
//0x01: 0 (unused)
//0x02: 1=no walk, 0=walk ok
//0x04: 1=no fly, 0=fly ok
//0x08: 1=no build, 0=build ok
//0x10: 0 (unused)
//0x20: 1=blight, 0=normal
//0x40: 1=no water, 0=water
//0x80: 1=unknown, 0=normal
public class GridInformation
{
    /** 功能 : 最大碰撞尺寸 */
    private int MaxDynamicCollisizeSize = 0;    

    /** 功能 : 各尺寸的碰撞状态 */
    private Dictionary<int, bool> _WalkBlocker = null;

    private int TerrainBlocker = -1;

    public bool IsTree
    {
        get
        {
            return (this.Data & (1 << (int)EGridType.IsTree)) != 0;
        }
    }

    public GridInformation()
    {
        this.Data = 0;
    }

    public int Data
    {
        get;
        set;
    }

    /*****************************************************************************************
     * 功能 : 获取当前格子数据的碰撞状态
     ******************************************************************************************/
    public bool IsWalkBlocker(int collisionSize)
    {
        return _WalkBlocker[collisionSize];
    }

    /******************************************************************************************
     * 功能 ：设置行走阻塞
     * @ collisionSize 碰撞尺寸
     * @ blocker 阻塞状态
     *******************************************************************************************/
    public void SetWalkBlocker(int collisionSize, bool blocker)
    {
        _WalkBlocker[collisionSize] = blocker;
    }

    /**************************************************************************************
     * 功能 ：更新行走阻塞
     ******************************************************************************************/
    public void UpdateWalkBlocker()
    {
        if (_WalkBlocker == null)
        {
            _WalkBlocker = new Dictionary<int, bool>();
            for (int i = 0; i <= MaxDynamicCollisizeSize; i++)
                _WalkBlocker.Add(i, false);
        }

        for (int i = 0; i <= 0; i++)
        {
            bool blocker = false;           // 默认没有阻塞情况发生

            if ((this.Data & (1 << (int)EGridType.IsWalkBlocker)) != 0)
                blocker = true;
            if ((this.Data & (1 << (int)EGridType.IsDynamicWalkBlocker)) != 0)
                blocker = true;
            if ((this.Data & (1 << (int)EGridType.IsTree)) != 0)
                blocker = true;

            _WalkBlocker[i] = blocker;      // 更新阻塞状态
        }
    }


}