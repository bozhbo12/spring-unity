using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**************************************************************************************
 * 功能 ： 显示单位标示在编辑器里
 **************************************************************************************/
public class UnitGizmos : MonoBehaviour
{
    public string iconFileName = "";
    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, iconFileName);
    }
}