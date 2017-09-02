using System;
using System.Collections.Generic;
using System.Text;

/**************************************************************************************************
 * 类 : 骨架
 ***************************************************************************************************/
public class Skeleton
{
    /** 骨骼节点 */
    public List<SkeletonJoint> joints;

    public Skeleton()
    {
        joints = new List<SkeletonJoint>();
    }

    /** 骨骼节点数量 */
    public int numJoints()
    {
        return joints.Count;
    }

    /***************************************************************************************************
     * 功能 : 释放
     ****************************************************************************************************/
    public void Dispose()
	{
        joints.Clear();
	}
}