using System;
using System.Collections.Generic;
using System.Text;

/***********************************************************************************************************
 * 类 : 骨骼节点
 ************************************************************************************************************/
public class SkeletonJoint
{
    public int parentIndex = -1;

    /** 转换逆矩阵 */
    public float[] inverseBindPose;

    public SkeletonJoint()
    {
        
    }
}