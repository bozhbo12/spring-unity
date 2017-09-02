using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CullingData", menuName = "创建剔除因子数据(CullingData)")]
public class CullingData : ScriptableObject
{
    public List<int> cullingKey = new List<int>(); 

    public List<float> cullingValue = new List<float>();
}
