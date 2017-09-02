using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/*******************************************************************************************
 * 类 : 骨骼动画
 *******************************************************************************************/
public class SkeletonAnimator
{
    /** 转换矩阵 */
    private float[] _globalMatrices;

    /** 骨骼数量 */
    private int _numJoints = 0;

    /** 姿态 */
    private SkeletonPose _globalPose = new SkeletonPose();

    // private Dictionary _animationStates = new Dictionary();

    /** 骨架 */
    private Skeleton _skeleton;

    /** 每个顶点受多少骨骼节点影响 */
    private int _jointsPerVertex;

    private bool _globalPropertiesDirty = false;

    public SkeletonAnimator()
    {
        _globalMatrices = new float[_numJoints * 12];
        
        // 初始化矩阵
        int i = 0;
        int j = 0;
        for (i = 0; i < _numJoints; ++i)
        {
            _globalMatrices[j++] = 1;
            _globalMatrices[j++] = 0;
            _globalMatrices[j++] = 0;
            _globalMatrices[j++] = 0;
            _globalMatrices[j++] = 0;
            _globalMatrices[j++] = 1;
            _globalMatrices[j++] = 0;
            _globalMatrices[j++] = 0;
            _globalMatrices[j++] = 0;
            _globalMatrices[j++] = 0;
            _globalMatrices[j++] = 1;
            _globalMatrices[j++] = 0;
        }
    }

    /***********************************************************************************************************
     * 属性 : 获取全局姿态
     ***********************************************************************************************************/
    public SkeletonPose globalPose
	{
        get 
        {
		    if (_globalPropertiesDirty == true)
			    UpdateGlobalProperties();
		    return _globalPose;
        }
	}

    /************************************************************************************************************
     * 功能 : 
     ************************************************************************************************************/
    private void UpdateGlobalProperties()
	{ 
		_globalPropertiesDirty = false;
			
		// get global pose
		// localToGlobalPose(_activeSkeletonState.getSkeletonPose(_skeleton), _globalPose, _skeleton);
			
		// convert pose to matrix
		int mtxOffset = 0;
		List<JointPose> globalPoses = _globalPose.jointPoses;
        float[] raw;
		float ox, oy, oz, ow;
		float xy2, xz2, xw2;
		float yz2, yw2, zw2;
		float n11, n12, n13;
		float n21, n22, n23;
		float n31, n32, n33;
		float m11, m12, m13, m14;
		float m21, m22, m23, m24;
		float m31, m32, m33, m34;

		List<SkeletonJoint> joints = _skeleton.joints;                      // 
		JointPose pose;
		Quaternion quat;
		Vector3 vec;
		float t;
			
		for (int i = 0; i < _numJoints; ++i) {
			pose = globalPoses[i];
			quat = pose.rotation;
			vec = pose.translation;
			ox = quat.x;
			oy = quat.y;
			oz = quat.z;
			ow = quat.w;
				
			xy2 = (t = 2.0f*ox)*oy;
			xz2 = t*oz;
			xw2 = t*ow;
			yz2 = (t = 2.0f*oy)*oz;
			yw2 = t*ow;
			zw2 = 2.0f*oz*ow;
				
			yz2 = 2.0f*oy*oz;
			yw2 = 2.0f*oy*ow;
			zw2 = 2.0f*oz*ow;
			ox *= ox;
			oy *= oy;
			oz *= oz;
			ow *= ow;
				
			n11 = (t = ox - oy) - oz + ow;
			n12 = xy2 - zw2;
			n13 = xz2 + yw2;
			n21 = xy2 + zw2;
			n22 = -t - oz + ow;
			n23 = yz2 - xw2;
			n31 = xz2 - yw2;
			n32 = yz2 + xw2;
			n33 = -ox - oy + oz + ow;
				
			// prepend inverse bind pose
			raw = joints[i].inverseBindPose;
			m11 = raw[0];
			m12 = raw[4];
			m13 = raw[8];
			m14 = raw[12];
			m21 = raw[1];
			m22 = raw[5];
			m23 = raw[9];
			m24 = raw[13];
			m31 = raw[2];
			m32 = raw[6];
			m33 = raw[10];
			m34 = raw[14];
			
            // 与逆矩阵相连
			_globalMatrices[mtxOffset] = n11*m11 + n12*m21 + n13*m31;
			_globalMatrices[mtxOffset + 1] = n11*m12 + n12*m22 + n13*m32;
			_globalMatrices[mtxOffset + 2] = n11*m13 + n12*m23 + n13*m33;
			_globalMatrices[mtxOffset + 3] = n11*m14 + n12*m24 + n13*m34 + vec.x;
			_globalMatrices[mtxOffset + 4] = n21*m11 + n22*m21 + n23*m31;
			_globalMatrices[mtxOffset + 5] = n21*m12 + n22*m22 + n23*m32;
			_globalMatrices[mtxOffset + 6] = n21*m13 + n22*m23 + n23*m33;
			_globalMatrices[mtxOffset + 7] = n21*m14 + n22*m24 + n23*m34 + vec.y;
			_globalMatrices[mtxOffset + 8] = n31*m11 + n32*m21 + n33*m31;
			_globalMatrices[mtxOffset + 9] = n31*m12 + n32*m22 + n33*m32;
			_globalMatrices[mtxOffset + 10] = n31*m13 + n32*m23 + n33*m33;
			_globalMatrices[mtxOffset + 11] = n31*m14 + n32*m24 + n33*m34 + vec.z;
				
			mtxOffset = mtxOffset + 12;
		}
	}

    /************************************************************************************************************
     * 功能 : 
     ************************************************************************************************************/
    private void LocalToGlobalPose(SkeletonPose sourcePose, SkeletonPose targetPose, Skeleton skeleton)
	{
		List<JointPose> globalPoses = targetPose.jointPoses;
		JointPose globalJointPose;                                             // 
		List<SkeletonJoint> joints = skeleton.joints;
		int len = sourcePose.numJointPoses;
		List<JointPose> jointPoses = sourcePose.jointPoses;                    // 节点姿态列表
		int parentIndex;
		SkeletonJoint joint;
		JointPose parentPose;                                                  // 父级节点姿态
		JointPose pose;
		Quaternion or;
		Vector3 tr;
		Vector3 t;

		Quaternion q;
		
		float x1, y1, z1, w1;
		float x2, y2, z2, w2;
		float x3, y3, z3;
		
		// :s
		// if (globalPoses.Count != len)
			// globalPoses.Count = len;
		
        // 遍历骨骼节点
		for (int i = 0; i < len; ++i) {
			globalJointPose = globalPoses[i];
            if (globalJointPose == null)
                globalJointPose = new JointPose();

			joint = joints[i];
			parentIndex = joint.parentIndex;
			pose = jointPoses[i];
				
			q = globalJointPose.rotation;
			t = globalJointPose.translation;
				
			if (parentIndex < 0) {
				tr = pose.translation;
				or = pose.rotation;
				q.x = or.x;
				q.y = or.y;
				q.z = or.z;
				q.w = or.w;
				t.x = tr.x;
				t.y = tr.y;
				t.z = tr.z;
			} else {
				// append parent pose
				parentPose = globalPoses[parentIndex];
					
				// rotate point
				or = parentPose.rotation;
				tr = pose.translation;
				x2 = or.x;
				y2 = or.y;
				z2 = or.z;
				w2 = or.w;
				x3 = tr.x;
				y3 = tr.y;
				z3 = tr.z;
					
				w1 = -x2*x3 - y2*y3 - z2*z3;
				x1 = w2*x3 + y2*z3 - z2*y3;
				y1 = w2*y3 - x2*z3 + z2*x3;
				z1 = w2*z3 + x2*y3 - y2*x3;
					
				// append parent translation
				tr = parentPose.translation;
				t.x = -w1*x2 + x1*w2 - y1*z2 + z1*y2 + tr.x;
				t.y = -w1*y2 + x1*z2 + y1*w2 - z1*x2 + tr.y;
				t.z = -w1*z2 - x1*y2 + y1*x2 + z1*w2 + tr.z;
					
				// append parent orientation
				x1 = or.x;
				y1 = or.y;
				z1 = or.z;
				w1 = or.w;
				or = pose.rotation;
				x2 = or.x;
				y2 = or.y;
				z2 = or.z;
				w2 = or.w;
					
				q.w = w1*w2 - x1*x2 - y1*y2 - z1*z2;
				q.x = w1*x2 + x1*w2 + y1*z2 - z1*y2;
				q.y = w1*y2 - x1*z2 + y1*w2 + z1*x2;
				q.z = w1*z2 + x1*y2 - y1*x2 + z1*w2;
			}
		}
	}

    /************************************************************************************************************
     * 功能 : CPU蒙皮转换计算
     ************************************************************************************************************/
    private void MorphGeometry()
	{
        List<float> vertexData = new List<float>();                      // 原始顶点数据
        List<float> targetData = new List<float>();                      // 转换后蒙皮顶点数据
        List<int> jointIndices = new List<int>();                       // 骨骼索引记录
        List<float> jointWeights = new List<float>();                    // 骨骼权重记录

		int index = 0;
		int j = 0, k = 0;
		float vx, vy, vz;
		float nx, ny, nz;
		float tx, ty, tz;
		int len = vertexData.Count;
		float weight;
        
		float vertX, vertY, vertZ;
		float normX, normY, normZ;
		float tangX, tangY, tangZ;
		float m11, m12, m13, m14;
		float m21, m22, m23, m24;
		float m31, m32, m33, m34;
			
		while (index < len) {
            // 顶点位置
			vertX = vertexData[index];
			vertY = vertexData[index + 1];
			vertZ = vertexData[index + 2];
            // 顶点法线
			normX = vertexData[index + 3];
			normY = vertexData[index + 4];
			normZ = vertexData[index + 5];
            // 顶点正切
			tangX = vertexData[index + 6];
			tangY = vertexData[index + 7];
			tangZ = vertexData[index + 8];
			vx = 0;
			vy = 0;
			vz = 0;
			nx = 0;
			ny = 0;
			nz = 0;
			tx = 0;
			ty = 0;
			tz = 0;
			k = 0;
			while (k < _jointsPerVertex) {
				weight = jointWeights[j];
				if (weight > 0) {
					// implicit /3*12 (/3 because indices are multiplied by 3 for gpu matrix access, *12 because it's the matrix size)
					int mtxOffset = jointIndices[j++] << 2;
					m11 = _globalMatrices[mtxOffset];
					m12 = _globalMatrices[mtxOffset + 1];
					m13 = _globalMatrices[mtxOffset + 2];
					m14 = _globalMatrices[mtxOffset + 3];
					m21 = _globalMatrices[mtxOffset + 4];
					m22 = _globalMatrices[mtxOffset + 5];
					m23 = _globalMatrices[mtxOffset + 6];
					m24 = _globalMatrices[mtxOffset + 7];
					m31 = _globalMatrices[mtxOffset + 8];
					m32 = _globalMatrices[mtxOffset + 9];
					m33 = _globalMatrices[mtxOffset + 10];
					m34 = _globalMatrices[mtxOffset + 11];

                    // 矩阵转换顶点、法线、正切值
					vx += weight*(m11*vertX + m12*vertY + m13*vertZ + m14);
					vy += weight*(m21*vertX + m22*vertY + m23*vertZ + m24);
					vz += weight*(m31*vertX + m32*vertY + m33*vertZ + m34);

					nx += weight*(m11*normX + m12*normY + m13*normZ);
					ny += weight*(m21*normX + m22*normY + m23*normZ);
					nz += weight*(m31*normX + m32*normY + m33*normZ);

					tx += weight*(m11*tangX + m12*tangY + m13*tangZ);
					ty += weight*(m21*tangX + m22*tangY + m23*tangZ);
					tz += weight*(m31*tangX + m32*tangY + m33*tangZ);
					++k;
				} else {
					j += (_jointsPerVertex - k);
					k = _jointsPerVertex;
				}
			}
				
			targetData[index] = vx;
			targetData[index + 1] = vy;
			targetData[index + 2] = vz;
			targetData[index + 3] = nx;
			targetData[index + 4] = ny;
			targetData[index + 5] = nz;
			targetData[index + 6] = tx;
			targetData[index + 7] = ty;
			targetData[index + 8] = tz;
				
			index = index + 13;
		}
	}
   
}