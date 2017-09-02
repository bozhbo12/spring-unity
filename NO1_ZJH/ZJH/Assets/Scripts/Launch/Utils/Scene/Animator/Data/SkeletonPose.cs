using System;
using System.Collections.Generic;
using System.Text;

/***********************************************************************************************
 * 类 : 骨骼姿态
 ***********************************************************************************************/
public class SkeletonPose
{
    /** 节点姿态列表 */
    public List<JointPose> jointPoses;

    public SkeletonPose()
    {
        jointPoses = new List<JointPose>();
    }

    public int numJointPoses
	{
        get
        {
            return jointPoses.Count;
        }
	}
}