using UnityEngine;
using System.Collections;


/// <summary>
/// 动态link对象数据
/// 20150229
/// zhangrj
/// </summary>
public class DynamicLinkUnit
{
    /// <summary>
    /// 相对角度
    /// </summary>
    private float localorient;

    /// <summary>
    /// 相对坐标
    /// </summary>
    private Vector3 localPosition;

    private Vector3 dir;

    /// <summary>
    /// link动态对象
    /// </summary>
    public DynamicUnit mDynamic;


    public Vector3 GetPosition(float y)
    {
        Quaternion rotation = Quaternion.Euler(0, localorient / Mathf.Deg2Rad + y, 0);
        return rotation * dir;
    }

    public void SetPositionAndOrient(Vector3 position, float orient)
    {
        this.localorient = orient;
        this.localPosition = position;

        dir = position - Vector3.zero;
    }



    /*
     * 后面三个会预先记录Dynamic的数据
     * 在设置link的时候强行修改
     * UnLink在修改回来
     */

    private  bool isMainUint=false;

    private bool genRipple = false;

    private bool needSampleHeight = false;

    public DynamicLinkUnit(DynamicUnit unit)
    {
        this.mDynamic = unit;

        this.isMainUint = unit.isMainUint;
        this.genRipple = unit.genRipple;
        this.needSampleHeight = unit.needSampleHeight;

        mDynamic.isMainUint = false;
        mDynamic.genRipple = false;
        mDynamic.needSampleHeight = false;
    }

    public void Init()
    {
        if (mDynamic == null) return;
        if (mDynamic.destroyed) return;
        mDynamic.isMainUint = false;
        mDynamic.genRipple = false;
        mDynamic.needSampleHeight = false;
    }

    public void Update(Vector3 position, Quaternion rotation)
    {
        if (mDynamic == null) return;
        if (mDynamic.destroyed) return;

        Vector3 angle = rotation.eulerAngles;
        mDynamic.Position = position + GetPosition(angle.y);
        mDynamic.Rotation = Quaternion.Euler(0, angle.y + localorient / Mathf.Deg2Rad, 0);
    }

    public void Remove()
    {
        if (mDynamic == null) return;
        if (mDynamic.destroyed) return;
        mDynamic.isMainUint = this.isMainUint;
        mDynamic.genRipple = this.genRipple;
        mDynamic.needSampleHeight = this.needSampleHeight;
        if (mDynamic.mDynState == DynamicState.LINK_PARENT_CHILD)
        {
            mDynamic.mDynState = DynamicState.LINK_PARENT;
        }
        else
        {
            mDynamic.mDynState = DynamicState.NULL;
        }
    }
}