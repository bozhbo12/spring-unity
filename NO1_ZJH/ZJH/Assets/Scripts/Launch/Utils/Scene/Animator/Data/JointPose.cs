using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*****************************************************************************************
 * 类 : 骨骼节点姿态
 *****************************************************************************************/
public class JointPose
{
    /** 骨骼节点旋转 */
    public Quaternion rotation;

    /** 骨骼节点平移 */
    public Vector3 translation;

    public JointPose()
    {
    	
    }

    /*************************************************************************************
     * 功能 : 将节点变换转为矩阵
     *************************************************************************************/
    public Matrix4x4 ToMatrix3D(ref Matrix4x4 target)
	{
        if (target == null)
            target = new Matrix4x4();

		//orientation.toMatrix3D(target);
		//target.appendTranslation(translation.x, translation.y, translation.z);
		return target;
	}

    /**************************************************************************************
     * 功能 : 克隆骨骼节点数据
     **************************************************************************************/
    public void CopyFrom(JointPose pose)
    {
        Quaternion or = pose.rotation;
        Vector3 tr = pose.translation;
        rotation.x = or.x;
        rotation.y = or.y;
        rotation.z = or.z;
        rotation.w = or.w;

		translation.x = tr.x;
		translation.y = tr.y;
		translation.z = tr.z;
    }
}