using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
/// <summary>
/// 动态单元对象
/// 添加人：zhangrj
/// 日期：20160229
/// </summary>
public enum DynamicState
{
    NULL = 0,
    /// <summary>
    /// link父节点
    /// </summary>
    LINK_PARENT,
    /// <summary>
    /// link子对象
    /// </summary>
    LINK_CHILD,
    /// <summary>
    /// 既是节点又是对象
    /// </summary>
    LINK_PARENT_CHILD,
}
/****************************************************************************
 * 类 ：动态游戏单位
 * 注意 对象的动态移动会修改地形的障碍物数据, 动态单位的销毁需要手动处理
 ****************************************************************************/
public class DynamicUnit : GameObjectUnit
{
    /// <summary>
    /// 出生特效计时回收器
    /// </summary>
    private static string BornTimeKey = "DynamicUnit_BornTimeKey";

    /** 单位移动监听器 */
    public delegate void MoveListener(bool flag);
    public MoveListener moveListener;

    /** 下一个寻路点 */
    public delegate void PathNextPointListener(Vector3 nextPathPoint);
    public PathNextPointListener pathNextPointListener;

    /** 寻路遇到阻碍中断 */
    public delegate void PathInterruptedListener(Vector3 postion);
    public PathInterruptedListener pathInterruptedListener;

    /** 寻路到达目标点中断 */
    public delegate void PathEndListener();
    public PathEndListener pathEndListener;

    /** 寻路开始移动回调*/
    public delegate void PathStartMove();
    public PathStartMove pathStartMoveListener;

    //private bool pathInterrupted = false;

    /// <summary>
    /// 高度差可行走范围
    /// </summary>
    public float mfCanMoveHeight = 1f;

    /// <summary>
    /// 是否使用扩展高度
    /// (用于高度变化速度与移动速度不一致时)
    /// </summary>
    public bool mbExtendedHeight = false;

    /// <summary>
    /// 外部扩展高度
    /// </summary>
    public float mfExtendedHeight = -1f;

    /// <summary>
    /// 获取玩家高度
    /// </summary>
    public float UnitHeight
    {
        get
        {
            if (mbExtendedHeight && mfExtendedHeight > 0f)
            {
                return mfExtendedHeight;
            }
            else
            {
                return Position.y;
            }
        }
    }

    /** 被阻断后继续寻路 */
    public bool continuWithInterr = true;


    private bool autoComputeDynCollision = true;


    public int _startMove = 0;
    public int _endMove = 0;
    /// <summary>
    /// 插值时间
    /// </summary>
    float mfCurEaseTime = 0f;
    float mfEaseTime = 2f;


    #region 移动相关
    private Vector3 mvecTargetPos;
    public Vector3 MvecTargetPos
    {
        get { return mvecTargetPos; }
        set { mvecTargetPos = value; }
    }

    private Vector3 mvecStep;

    public float mfSpeed = 0f;

    /** 是否在移动中 */
    public bool moving = false;

    private Vector3 vecNormalStep;
#endregion

    #region 出生特效

    /// <summary>
    /// 出生特效
    /// </summary>
    protected GameObject bornEffect;

    /// <summary>
    /// 路径
    /// </summary>
    private string _bornEffectPrePath = "";

    /// <summary>
    /// 是否播放过
    /// </summary>
    private bool mbPlayed = false;

    public string bornEffectPrePath
    {
        set
        {
            _bornEffectPrePath = value;
            PlayBornEffect();
        }
        get { return _bornEffectPrePath; }

    }

    #endregion

    private Vector3 nextPostion = Vector3.zero;

    public bool isBreak = false;

    public bool dontComputeCollision = true;

    public int tick = 0;

    /** 判断单位是否在水下 */
    public bool underwater = false;

    #region 添加link   zhangrj

    /// <summary>
    /// 状态
    /// </summary>
    public DynamicState mDynState = DynamicState.NULL;

    /// <summary>
    /// link数据
    /// 子对象调用
    /// </summary>
    public List<DynamicLinkUnit> linkUnits = new List<DynamicLinkUnit>();

    //  差值旋转 
    /*public bool beLerpRotaion = false;
    public float lerpOrient = 0.0f;
    public float lerpSpeed = 0.0f;*/

    #endregion
    public DynamicUnit(int createID)
        : base(createID)
    {
        isStatic = false;   
    }

    /*******************************************************************************
     * 功能 : 销毁动态单位
     *********************************************************************************/
    override public void Destroy()
    {
        _endMove = 1;
        _startMove = 0;
        moving = false;

        TimerManager.Destroy(DelegateProxy.StringBuilder(DynamicUnit.BornTimeKey, createID));
        DestroyBornEffect();

        if (paths != null)
        {
            pathFindEnd = true;
            paths.Clear();
        }


        isMainUint = false;
        genRipple = false;
        // 去除引用监听
        this.moveListener = null;
        this.pathNextPointListener = null;
        this.pathInterruptedListener = null;
        this.pathEndListener = null;
        this.pathStartMoveListener = null;
        base.Destroy();
    }

    /// <summary>
    /// 动态单位被实便化
    /// </summary>
    protected override void OnInitialize()
    {
        // 调用创建实例监听
        if (createInsListener != null)
        {
            try
            {
                createInsListener.Invoke(this);
            }
            catch (Exception e)
            {
                LogSystem.LogError("监听创建单位函数中出错!" + e.ToString());
            }
        }
    }

    /// <summary>
    /// 动态单元激活
    /// </summary>
    protected override void OnActiveUnit(bool bValue)
    {
        if (activeListener != null)
        {
            try
            {
                activeListener(bValue);
            }
            catch (Exception e)
            {
                LogSystem.LogError("监听创建单位函数中出错!" + e.ToString());
            }
        }
    }

    #region 出生特效

    /// <summary>
    /// 播放出生特效
    /// </summary>
    private void PlayBornEffect()
    {
        if (mbPlayed)
            return;

        if (string.IsNullOrEmpty(_bornEffectPrePath))
            return;

        mbPlayed = true;
        GameObjectUnit.ThridPardLoad(_bornEffectPrePath, OnBornEffectLoadComplete, null, false);
    }

    /// <summary>
    /// 出生特效加载完成
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="strFileName"></param>
    /// <param name="varStore"></param>
    private void OnBornEffectLoadComplete(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        if (destroyed)
        {
            CacheObjects.PopCache(oAsset);
            return;
        }

        bornEffect = CacheObjects.InstantiatePool(oAsset) as GameObject;
        bornEffect.transform.position = this.Position;
        //特效时间获取不到，固定用3秒代替
        TimerManager.AddTimer(DelegateProxy.StringBuilder(DynamicUnit.BornTimeKey, createID), 3f, BornEffectPlayEnd);
    }

    /// <summary>
    /// 特效播放结束
    /// </summary>
    private void BornEffectPlayEnd()
    {
        if (destroyed)
            return;

        DestroyBornEffect();
    }

    /// <summary>
    /// 删除出生特效
    /// </summary>
    private void DestroyBornEffect()
    {
        if (bornEffect != null)
            CacheObjects.DestoryPoolObject(bornEffect);

        bornEffect = null;
        bornEffectPrePath = "";
    }

    #endregion

    /*******************************************************************************
     * 功能 : 作为主单位进行计算,例如玩家自己
     *********************************************************************************/
    public void AsMainUint()
    {
        scene.mainUnit = this;
        isMainUint = true;
        this.isCollider = false;            // 自身玩家不计算碰撞
        this.dontComputeCollision = false;

        if (isMainUint == true && GameScene.isPlaying == true)
            genRipple = true;
        // LogSystem.LogError("设置主对象");
    }

    /**************************************************************************************
     * 功能 : 立即设置碰撞
     **************************************************************************************/
    public void SetCollision(bool value)
    {
        if (value == false)
        {
            scene.mapPath.SetDynamicCollision(Position, collisionSize, true);
            hasCollision = false;
            isCollider = false;
        }
        else
        {
            scene.mapPath.SetDynamicCollision(Position, collisionSize, false);
            hasCollision = true;
            isCollider = true;
        }
    }

    /**************************************************************************************
     * 功能 : 立即设置碰撞
     **************************************************************************************/
    public void SetCustomCollision(bool value)
    {
        if (this.grids != null)
        {
            if (value == false)
            {
                 if (scene!= null && scene.mapPath != null)
                     scene.mapPath.SetDynamicCollision(Position, this.grids, true);
                isCollider = false;
            }
            else
            {
                 if (scene != null && scene.mapPath != null)
                     scene.mapPath.SetDynamicCollision(Position, this.grids, false);
                isCollider = true;
            }
        }
    }

    /*******************************************************************************************
     * 功能 ：服务端创建单位实例
     * 注 ：显示实例的显隐由LOD距离计算
     *******************************************************************************************/
    static public DynamicUnit Create(GameScene scene, Vector3 pos, int createID, string prePath, float radius, float dynamiCullingDistance = -1f, bool bNeedSampleHeight = true)
    {
        DynamicUnit unit = new DynamicUnit(createID);
        unit.scene = scene;
        // 读取游戏对象的位置信息
        unit.Position = pos;
        unit.prePath = prePath;
        unit.radius = radius;
        unit.needSampleHeight = bNeedSampleHeight;
        if (dynamiCullingDistance > 0f)
            unit.near = dynamiCullingDistance;
        else
            unit.near = scene.terrainConfig.dynamiCullingDistance;
        unit.far = unit.near + 2f;

        return unit;
    }
    /**************************************************************************************************
   * 功能 : 设置动态Link对象
   * 添加人：zhangrj
   * 日期：20160229
   ***************************************************************************************************/
    public void SetDynamicLink(Vector3 position, float orient, DynamicUnit unit)
    {
        if (unit == null) return;

        if (destroyed || unit.destroyed || scene.mapPath == null)
            return;

        if (mDynState == DynamicState.LINK_CHILD)
            mDynState = DynamicState.LINK_PARENT_CHILD;
        else if (mDynState == DynamicState.NULL)
        {
            mDynState = DynamicState.LINK_PARENT;
        }


        if (linkUnits == null)
        {
            linkUnits = new List<DynamicLinkUnit>();
        }

        int len = linkUnits.Count;

        DynamicLinkUnit linkUnit = null;

        for (int i = 0; i < len; ++i)
        {
            if (linkUnits[i] != null && linkUnits[i].mDynamic != null)
            {
                if (linkUnits[i].mDynamic == unit)
                {
                    linkUnit = linkUnits[i];
                    break;
                }
            }
        }

        if (linkUnit == null)
        {
            linkUnit = new DynamicLinkUnit(unit);
            linkUnits.Add(linkUnit);
        }
        else
        {
            linkUnit.Init();
        }

        if (unit.mDynState == DynamicState.LINK_PARENT)
        {
            unit.mDynState = DynamicState.LINK_PARENT_CHILD;
        }
        else
        {
            unit.mDynState = DynamicState.LINK_CHILD;
        }


        //linkUnit.localorient = orient;
        //linkUnit.localPosition = position;

        Vector3 angle = this.Rotation.eulerAngles;
        linkUnit.SetPositionAndOrient(position, orient);
        Vector3 target = this.Position + linkUnit.GetPosition(angle.y);
        float o = angle.y * Mathf.Deg2Rad + orient;

        unit.OnLinkLocation(target, o);
    }

    /**************************************************************************************************
     * 功能 : 移除动态Link对象
     * 添加人：zhangrj
     * 日期：20160229
     ***************************************************************************************************/
    public void RemoveLinkDynamic(DynamicUnit unit)
    {
        int len = linkUnits.Count;
        for (int i = 0; i < len; ++i)
        {
            if (linkUnits[i] != null && linkUnits[i].mDynamic != null)
            {
                if (linkUnits[i].mDynamic == unit)
                {
                    linkUnits[i].Remove();
                    linkUnits.RemoveAt(i);
                    break;
                }
            }
        }
        len = linkUnits.Count;
        if (len == 0)
        {
            mDynState = DynamicState.NULL;
        }
    }

    /**************************************************************************************************
    * 功能 : 移除所以动态Link对象
    * 添加人：zhangrj
    * 日期：20160229
    ***************************************************************************************************/
    public void RemoveAllLinkDynamic()
    {
        mDynState = DynamicState.NULL;
        int len = linkUnits.Count;
        for (int i = len - 1; i >= 0; i--)
        {
            if (linkUnits[i] != null)
            {
                linkUnits[i].Remove();
            }
            linkUnits.RemoveAt(i);
        }
    }

    /**************************************************************************************************
     * 功能 : 设置动态单位剔除距离
     * 无用功能
     ***************************************************************************************************/
    public void SetCullingDistance(float dynamiCullingDistance)
    {
        if (dynamiCullingDistance > 0f)
            this.near = dynamiCullingDistance;
        else
            this.near = scene.terrainConfig.dynamiCullingDistance;
        this.far = this.near + 2f;  
    }

    /**************************************************************************************************
     * 功能 : 动态单位更新 
     ***************************************************************************************************/
    override public void Update()
    {
        // 动态单位更新渲染实例位置
        if (Ins != null)
        {
            //移动;
            if ((Mathf.Abs(InsPosition.x - Position.x) > 0.01f) || (Mathf.Abs(InsPosition.z - Position.z) > 0.01f) || (Mathf.Abs(InsPosition.y - Position.y) > 0.01f))
            {
//                 if (needSampleHeight == true)
//                 {
//                     smHeight = scene.SampleHeight(position);
//                     position.y = smHeight;
//                     // position.y = Mathf.Lerp(position.y, scene.SampleHeight(position), 0.2f);
//                 }
                // 计算当前单位的屏幕坐标
                if (needScreenPoint == true || mouseEnable == true)
                {
                    if (scene.mainCamera)
                    {
                        scenePoint.x = Position.x;
                        scenePoint.z = Position.z;
                        scenePoint.y = Position.y + scenePointBias;

                        screenPoint = scene.mainCamera.WorldToScreenPoint(scenePoint);
                    }
                }

                if (genRipple == true)
                {
                    if (tick % 4 == 0)
                    {
                        if (scene.Underwater(this.Position))
                        {
                            ripplePos.x = Position.x;
                            ripplePos.z = Position.z;
                            ripplePos.y = scene.waterHeight;
                            // Debug.Log("水面高度-> " + scene.waterHeight);
                            // Debug.Log("我已经在水下了");
                            Ripple.CreateRippleGameObject(ripplePos);

                            this.underwater = true;
                        }
                        else
                        {
                            this.underwater = false;
                        }
                    }
                }
            }

            if (needSampleHeight)
            {
                SetPostionY(scene.SampleHeight(Position));
            }
            else
            {
                if (mbExtendedHeight && mfExtendedHeight > 0)
                {
                    SetPostionY(mfExtendedHeight);
                }
            }

            InsPosition = Position;
            if (_rotationDirty == true)
            {
                if (Quaternion.Angle(Rotation, targetRotation) < 3f)
                {
                    Rotation = targetRotation;
                    _rotationDirty = false;
                }
                InsRotation = Rotation;
            }
        }

        // 生成涟漪效果
        if (genRipple == true)
        {
            if (tick % genRippleDelayTick == 0)
            {
                if (scene.Underwater(Position))
                {
                    ripplePos.x = Position.x;
                    ripplePos.z = Position.z;
                    ripplePos.y = scene.waterHeight;
                    // Debug.Log("水面高度-> " + scene.waterHeight);
                    // Debug.Log("我已经在水下了");
                    Ripple.CreateRippleGameObject(ripplePos);

                    this.underwater = true;
                }
                else
                {
                    this.underwater = false;
                }
            }
        }

        if (bornEffect != null)
            bornEffect.transform.position = Position;

        // Debug.Log("当前格子数据-> " + scene.getGridValue(position));

        //if (pathInterrupted == true)
        //{
        //    curDelayTick--;
        //    if (curDelayTick < 1)
        //        FindPathMove(pathFindTarget, this.mfSpeed, this.continuWithInterr, this.delayTick, true);
        //}

        if (_startMove > 0)
            _startMove--;
        if (_endMove > 0)
            _endMove--;

        if (move_type == 0)
            UpdateMove();
        else if (move_type == 1)
            UpdateForce();

        // /Link对象更新
        ///添加人：zhangrj
        ///日期：20160229
        if (mDynState == DynamicState.LINK_PARENT || mDynState == DynamicState.LINK_PARENT_CHILD)
        {
            for (int k = 0; k < linkUnits.Count; ++k)
            {
                if (linkUnits[k] != null)
                {
                    linkUnits[k].Update(this.Position, this.Rotation);
                }
            }
        }
        tick++;
    }


    /*************************************************************************************
     * 功能 : 寻路移动到目标点
     *************************************************************************************/

    private Vector3 pathFindTarget = Vector3.zero;
    private List<Vector3> paths = null;
    public int delayTick = 0;
    public int curDelayTick = 0;
    private bool pathFindEnd = true;
    private string pfWrongTip = "注意：【失败原因1】：寻路移动失败! 请相关策划查询目标点配置是否正确  【失败原因2】有人站在你的格子上与你人物有重叠或者一群怪物围着你,寻路目标点->";
    public bool FindPathMove(Vector3 pTarget, float speed, bool continuWithInterr, int delayTick = 0, bool sysInvoke = false)
    {
        if (sysInvoke == false)
            Stop();

        // 重置目标点
        MvecTargetPos = Position;

        float h = GameScene.mainScene.SampleHeight(pTarget);
        if (h < 10f)
        {
            Stop();
            if (GameScene.isEditor == true)
                LogSystem.LogWarning(pfWrongTip + pTarget + "; 失败场景ID:" + GameScene.mainScene.sceneID);
            return false;
        }

        bool b = GameScene.mainScene.IsValidForWalk(pTarget, this.collisionSize);
        if (b == false)
        {
            //float dx = pTarget.x - position.x;
            //float dz = pTarget.z - position.z;
            //if (Mathf.Sqrt(dx * dx + dz * dz) < 4f)
            //    pathInterrupted = false;

            Stop();
            if (GameScene.isEditor == true)
                LogSystem.LogWarning(pfWrongTip + pTarget + "; 失败场景ID:" + GameScene.mainScene.sceneID);
            return false;
        }
        pathFindEnd = false;
        this.delayTick = delayTick;
        this.curDelayTick = delayTick;
        this.continuWithInterr = continuWithInterr;
        this.mfSpeed = speed;
        pathFindTarget = pTarget;
        if (DelegateProxy.bIsWayInitOk())
        {
            DelegateProxy.RequestPaths(this.Position, pTarget, OnPathFindEnd);
        }
        else
        {
            scene.mapPath.RequestPaths(this.Position, pTarget, this.collisionSize, OnPathFindEnd);
        }
        return true;
    }

    // 获取当前寻路路径的距离
    public float GetPathFindDis()
    {
        float distance = 0.0f;
        if (paths != null && paths.Count>0)
        {
            distance += MathUtils.Distance2D(Position, paths[0]);
            for (int x = 0; x < paths.Count-1;x++)
            {
                distance += MathUtils.Distance2D(paths[x], paths[x+1]); 
            }
        }
        return distance;
    }

    public void OnPathFindEnd(List<Vector3> paths)
    {
        this.paths = paths;
        if (paths.Count < 2)
        {
            paths.Clear();
            paths = null;
            Stop();
            return;
        }

        if (pathStartMoveListener != null)
            pathStartMoveListener.Invoke();

        //第0个为起点
        paths.RemoveAt(0);
        //pathInterrupted = false;
        //解决：寻路跳跃人物陷入地下问题
        Vector3 vTarget = paths[0];
        if (scene != null)
        {
            vTarget.y = scene.SampleHeight(vTarget);
        }
        // 移动到最近一个目标
        Move(vTarget, mfSpeed);

        // 避免移动监听调用停止,路径被清空
        if (paths.Count > 1)
            paths.RemoveAt(0);
    }

    /*************************************************************************************
     * 功能 ： 移动单位到指定目标
     * @target 移动目标
     * @speed 移动速度
     **************************************************************************************/
    public void Move(Vector3 target, float speed , bool bRotation = false)
    {
        //Debug.Log("Move::" + target + " " + target.y);
        if (destroyed == true)
            return;

        if (Vector3.SqrMagnitude(target - Position) < 0.01f)
        {
            if (paths != null && paths.Count > 0)
            {
                MoveImmediately(paths[0], speed);
            }
            //LogSystem.LogWarning("Distance too close " + target + " " + Position);
            return;
        }

        move_type = 0;
        this._startMove = 1;
        this._endMove = 0;
        this.moving = true;

        _rootDirty = false;

        mfSpeed = speed;
        //MvecTargetPos = target;
        //if (needSampleHeight)
        //{
        //    MvecTargetPos.y = scene.SampleHeight(MvecTargetPos);
        //}
        if (needSampleHeight)
        {
            target.y = scene.SampleHeight(target);
        }
        MvecTargetPos = target;

        vecNormalStep = (MvecTargetPos - this.Position).normalized;
        mvecStep = vecNormalStep * Time.deltaTime * mfSpeed;
        if (moveListener != null)
            moveListener.Invoke(true);

        // 计算目标旋转 飞行爬升阶段 不处理旋转
        if (bRotation)
            return;

        //距离大于0.2f
        if (!MathUtils.Distance2DLessEqual(target, Position, 0.2f))
        {
            _rotationDirty = true;
            //Debug.Log("位置:" + target + " " + Position);
            float euler = -180.0f / Mathf.PI * Mathf.Atan2(target.z - Position.z, target.x - Position.x) + 90f;
            lookAtEuler.y = euler;
            targetRotation = Quaternion.Euler(lookAtEuler);
        }
        else
        {
            _rotationDirty = false;
        }
    }


    /** 立即移动,降低寻路连续移动中的时间差 */
    public void MoveImmediately(Vector3 pTarget, float speed)
    {
        paths.RemoveAt(0);
        Move(pTarget, speed);
        UpdateMove();
    }

    public void UpdateMove()
    {
        this.isBreak = false;
        if (moving == true)
        {
            // 判断需要旋转再做插值
            if (_rotationDirty)
                Rotation = Quaternion.Lerp(Rotation, targetRotation, 0.35f);

            //if (mbExtendedHeight)
            //{
            //    //高度发生变化，需要重新计算单位向量
            //    vecNormalStep = (this.mvecTargetPos - this.position).normalized;
            //}
            mvecStep = vecNormalStep * Time.deltaTime * mfSpeed;
            bool bMoveToEnd = false;
            //跳跃
            //修复在跳跃中人物会缓慢向前移动的bug
            //原因，是3维向量计算问题，距离都累加在高度上
            if (mbExtendedHeight)
            {
                bMoveToEnd = MathUtils.Distance2DLessEqual(Position, MvecTargetPos, Time.deltaTime * mfSpeed);
            }
            else
            {
                bMoveToEnd = Vector3.SqrMagnitude(MvecTargetPos - Position) <= mvecStep.sqrMagnitude;
            }

            // 到达目的地单位停止移动
            if (bMoveToEnd)
            {
                if (paths != null && paths.Count > 0)
                {
                    // Debug.Log("next move point!");
                    if (pathNextPointListener != null)
                        pathNextPointListener(paths[0]);

                    // 移动到最近一个目标
                    MoveImmediately(paths[0], mfSpeed);
                }
                else
                {
                    Position = MvecTargetPos;

                    if (pathFindEnd == false)
                    {
                        if (pathEndListener != null)
                            pathEndListener.Invoke();
                    }
                   
                    Stop();
                }
            }
            else
            {
                if (dontComputeCollision == false && outObstacles == false)
                {
#if UNITY_EDITOR
                    if (!isMainUint)
                    {
                        LogSystem.LogWarning("非主玩家不计算碰撞");
                    }
#endif
                    //飞与跳都不走逻辑
                    nextPostion = Position + mvecStep;

                    // 移动过程中被打断
                    if (scene.IsValidForWalk(nextPostion, this.collisionSize) == false)
                    {
                        // 寻路被中断
                        if (pathFindEnd == false)
                        {
                            // Debug.Log("移动过程中被障碍物阻挡!");
							/**
                            if (scene.IsValidForWalk(nextPostion, 0) == false)
                            {
                                if (pathInterruptedListener != null)
                                    pathInterruptedListener(position);

                                pathInterrupted = true;
                                if (continuWithInterr == true && delayTick == 0)
                                {
                                    FindPathMove(pathFindTarget, this.speed, this.continuWithInterr, 0, true);
                                    return;
                                }
                            }
                            else
                            **/
                            {
                                Position += mvecStep;
                                return;
                            }
                        }
                        isBreak = true;
                        Stop();
                    }
                    else
                        Position += mvecStep;
                }
                else
                {
                    Position += mvecStep;
                }
            }
        }
        else 
        {
            if (_rotationDirty && _rootDirty)
            {
                targetTime -= Time.deltaTime;
    //            LogSystem.LogError("========开始旋转=====" + Rotation.eulerAngles.y + " , , " + targetRotation.eulerAngles.y + " ,,  InsRotation.eulerAngles.y = " + InsRotation.eulerAngles.y);

                if (targetTime > 0)
                {
                    //RotateTowards 
                    Rotation = Quaternion.RotateTowards(Rotation, targetRotation, Mathf.Abs(tempspeed));
                    changeTime += Time.deltaTime * 3f;
                }
                else 
                {
                    _rootDirty = false;
                    Rotation = targetRotation;
                }
            }
        }
    }

    /*****************************************************************************************
     * 功能 : 加速移动
     *****************************************************************************************/
    private int move_type = -1;

    public int MoveType
    {
        get
        {
            return move_type;
        }
    }

    public void EaseMove(Vector3 dest, float time, int size)
    {
        if (destroyed == true)
            return;
        move_type = 1;

        // 标记状态
        //this._startMove = 1;
        //this._endMove = 0;
        //this.moving = true;

        MvecTargetPos = dest;

        this.mfEaseTime = time;
        //this.inertia = inertia;
        scene.mapPath.SetDynamicCollision(Position, size, true);
    }

    /******************************************************************************************
     * 功能 : 更新受力
     ******************************************************************************************/
    public void UpdateForce()
    {
        mfCurEaseTime += Time.deltaTime;
        float dist = Vector3.SqrMagnitude(Position - MvecTargetPos);
        if (dist > 0.03f * 0.03f)
        {
            Position = Vector3.Lerp(Position, MvecTargetPos, mfCurEaseTime / mfEaseTime);
        }
        else
        {
            Position = MvecTargetPos;
            move_type = -1;
            mfEaseTime = 0;
            mfCurEaseTime = 0;
        }
    }

    private Quaternion targetRotation;

    private float targetTime = 0;

    private float changeTime = 0;

    private float tempspeed = 0;
    
    private Vector3 lookAtEuler = new Vector3(0f, 0f, 0f);

    public void SetTargetRotation(Quaternion q)
    {
        targetRotation = q;
        _rotationDirty = true;
    }

    public void SetTargetRotation(float euler, float speed)
    {
        if (Ins == null)
            return;


        _rootDirty = true;

        changeTime = 0.2f;

        if (speed > 0)
        {
            targetTime = Mathf.Abs((euler+Mathf.PI - InsRotation.eulerAngles.y / Mathf.Rad2Deg)/ speed);
        }
        else 
        {
            targetTime = Mathf.Abs((-euler-Mathf.PI - InsRotation.eulerAngles.y / Mathf.Rad2Deg) / speed);
        }

        tempspeed = speed;
     //   LogSystem.LogError("  ====   开始转身=======targetTime = " + targetTime + " , tempspeed = " + tempspeed);
   //     changeTimeAcceleration * t  = 0.8f / targetTime * t;

        targetRotation = Quaternion.Euler(0, euler * Mathf.Rad2Deg, 0);

        _rotationDirty = true;

        if (move_type == -1) 
        {
            move_type = 0;
        }

    }

    //public void SetTargetRotation(Quaternion target, float time = 0.5f)
    //{
    //        _rootDirty = true;
    //        index = 0;
    //        targetTime = time;
    //        changeTime = 0.2f;
    //        targetRotation = target;// Quaternion.Euler(0, euler * Mathf.Rad2Deg, 0);
    //        _rotationDirty = true;
    //}


    /****************************************************************************************
     * 功能 : 停止移动
     ******************************************************************************************/
    public void Stop()
    {
        _endMove = 1;
        _startMove = 0;
        moving = false;
        MvecTargetPos = Position;
        //if (ins != null)
        //{
        //    ins.transform.position = position;
        //    ins.transform.rotation = this.targetRotation;
        //}
        if (isMainUint)
        {
            ClearFindPath();
        }
        if (moveListener != null)
            moveListener.Invoke(false);
    }

    /// <summary>
    /// 清除自动寻路数据
    /// </summary>
    public void ClearFindPath()
    {
        pathFindEnd = true;
        if (paths != null && paths.Count>0)
        {
            paths.Clear();
        }
    }

    /***********************************************************************************************************
     * 功能 ： 检测单位是否开始移动
     ************************************************************************************************************/
    public bool startMove
    {
        get
        {
            return (_startMove > 0);
        }
    }

    /***********************************************************************************************************
     * 功能 ： 检测单位是否结束移动
     ************************************************************************************************************/
    public bool endMove
    {
        get
        {
            return (_endMove > 0);
        }
    }

    /*********************************************************************************************************
     * 功能 : 将单位定位到指定位置
     *********************************************************************************************************/
    public void OnLocation(Vector3 target, float orient)
    {
        if (destroyed == true || scene.mapPath == null)
            return;

        // 清理占用格子, 注意update前已经删除单位所占格子,所以下句if判断存在歧义
        // if (this.hasCollision == true)
        scene.mapPath.SetDynamicCollision(Position, collisionSize, true);

        MvecTargetPos = target;
        Quaternion q = Quaternion.Euler(0, orient / Mathf.Deg2Rad, 0);
        this.Rotation = q;
        this.Position = MvecTargetPos;
        if (Ins != null)
            InsRotation = this.Rotation;
    }
    /*********************************************************************************************************
     * 功能 : 定位Link对象
     * 添加人：zhangrj
     * 日期：20160229
     *********************************************************************************************************/
    public void OnLinkLocation(Vector3 target, float orient)
    {
        if (destroyed == true || scene.mapPath == null)
            return;

        MvecTargetPos = target;
        Quaternion q = Quaternion.Euler(0, orient / Mathf.Deg2Rad, 0);
        this.Rotation = q;
        this.Position = MvecTargetPos;
    }
    /***********************************************************************
     * 功能 : 尝试移动到指定位置或附近
     ************************************************************************/
    public bool TryLocationAt(Vector3 postion, out Vector3 target)
    {
        target = Vector3.zero;
        return false;
    }

    private Vector3 newTarget = Vector3.zero;
    /*********************************************************************
     * 功能 ： 以指定速度向目标移动
     * 如果当前目标被阻塞，将返回一个可以到达的目标
     * 如果下一个目标就出现阻塞将返回可行走目标
     * 注 ：如果当前位置已经被占，则玩家避开阻塞位置
     *********************************************************************/
    private Vector3 from;
    private Vector3 lastTarget;
    private Vector3 dir;
    private Vector3 tryStep;
    private Vector3 nextTarget;

    private bool outObstacles = false;


    public Vector3 TryMove(Vector3 target, out bool isBlocked, bool doneAvoid = true)
    {
        // 自动判定是否计算动态碰撞数据
        if (autoComputeDynCollision == true)
            this.dontComputeCollision = false;
        if (dontComputeCollision == true)
        {
            isBlocked = false;
            return target;
        }

        outObstacles = false;

        //avoid = false;

        from = Position;

        // 如果尝试移动到当前位置则返回
        float distanceSqr = Vector3.SqrMagnitude(from - target);
        if (distanceSqr < 0.0001f)
        {
            isBlocked = false;
            return target;
        }
        lastTarget = from;                                          // 最终的目标
        dir = (target - from).normalized;
        tryStep = dir * radius;                                        // 下一目标的位移

        nextTarget = from + tryStep;                                   // 下一个目标

        #region 已阻塞
        
        // 如果当前目标已经阻塞,检测下个目标是否空置,如果空置则跳过去
        if (scene.IsValidForWalk(Position, this.collisionSize) == false)
        {
            if (distanceSqr < 25f)
                distanceSqr = 25f;

            //target = from + dir * distanceSqr;

            // 移动到前方离目标点可停靠的地方
            do
            {
                // 如果前方遇到场景静态障碍物则返回
                if (scene.getGridType(nextTarget) < 1 && scene.IsValidForWalk(nextTarget, 0) == false)
                    break;

                if (scene.IsValidForWalk(nextTarget, this.collisionSize) == true)
                {
                    outObstacles = true;
                    lastTarget = nextTarget;
                    break;
                }
                float nextDistanceSqr = Vector3.SqrMagnitude(from - nextTarget);
                if (nextDistanceSqr >= distanceSqr)
                {
                    lastTarget = Position;
                    break;
                }
                lastTarget = nextTarget;
                nextTarget = nextTarget + tryStep;
            }
            while (true);

            isBlocked = true;
            return lastTarget;
        }

        #endregion

        #region 前方有阻塞
        // 如果第一步就遇到障碍点,尝试绕开障碍点
        if (doneAvoid == true)
        {
            if (scene.IsValidForWalk(from + dir * 0.3f, this.collisionSize) == false)
            {
                isBlocked = true;

                target.y = Position.y;

                // 计算目标与当前方向夹角
                float angle = -180.0f / Mathf.PI * Mathf.Atan2(target.z - Position.z, target.x - Position.x) + 90;
                int dx = Sign(Mathf.Sin(angle * Mathf.Deg2Rad));
                int dz = Sign(Mathf.Cos(angle * Mathf.Deg2Rad));

                newTarget.x = Position.x;
                newTarget.z = Position.z + dz * 1f;

                if (scene.IsValidForWalk(newTarget, this.collisionSize))
                {
                    //avoid = true;
                    return newTarget;
                }
                newTarget.x = Position.x + dx * 1f;
                newTarget.z = Position.z;

                newTarget.y = this.Position.y;

                if (scene.IsValidForWalk(newTarget, this.collisionSize))
                {
                   // avoid = true;
                    return newTarget;
                }
            }
        }

        #endregion

        do
        {
            if (scene.IsValidForWalk(nextTarget, this.collisionSize) == false)
            {
                // Debug.Log("遇到障碍物,格子数据->" + scene.getGridValue(nextTarget) + " 碰撞类型-> " + scene.getGridType(nextTarget) + " 碰撞尺寸-> " + this.collisionSize );
                isBlocked = true;
                return lastTarget;
            }
            float nextDistanceSqr = Vector3.SqrMagnitude(from - nextTarget);
            if (nextDistanceSqr >= distanceSqr)
            {
                isBlocked = false;
                return nextTarget;
            }
            lastTarget = nextTarget;
            nextTarget = nextTarget + tryStep;
        }
        while (true);
    }

    /// <summary>
    /// 获取飞行目标点
    /// </summary>
    /// <param name="target"></param>
    /// <param name="isBlocked"></param>
    /// <returns></returns>
    public Vector3 TryFly(Vector3 target, out bool isBlocked)
    {
        outObstacles = false;
        from = Position;
        // 如果尝试移动到当前位置则返回
        float fSqrLen = (from - target).sqrMagnitude;
        if (fSqrLen < 0.0001f)
        {
            isBlocked = false;
            return target;
        }

        lastTarget = from;                                              // 最终的目标
        dir = (target - from).normalized;
        tryStep = dir * radius;                                         // 下一目标的位移
        nextTarget = from + tryStep;                                    // 下一个目标
        do
        {
            if (scene.IsValidForFly(Position, nextTarget, mfCanMoveHeight, this.collisionSize) == false)
            {
                // Debug.Log("遇到障碍物,格子数据->" + scene.getGridValue(nextTarget) + " 碰撞类型-> " + scene.getGridType(nextTarget) + " 碰撞尺寸-> " + this.collisionSize );
                isBlocked = true;
                return lastTarget;
            }

            float nextSqrLen = (from - nextTarget).sqrMagnitude;
            if (nextSqrLen >= fSqrLen)
            {
                isBlocked = false;
                return nextTarget;
            }
            lastTarget = nextTarget;
            nextTarget = nextTarget + tryStep;
        }
        while (true);
    }

    /// <summary>
    /// 获取跳跃目标点
    /// </summary>
    /// <param name="target"></param>
    /// <param name="isBlocked"></param>
    /// <returns></returns>
    public Vector3 TryJump(Vector3 target, float fJumpHeight, float fJumpDistance)
    {
        from = Position;
        // 如果尝试移动到当前位置则返回
        float fSqrLen = (from - target).sqrMagnitude;
        if (fSqrLen < 0.0001f)
        {
            return target;
        }

        lastTarget = from;
        dir = (target - from).normalized;
        tryStep = dir * radius;
        nextTarget = from + tryStep;
        bool bCanStand = false;

        do
        {
            float fTryJumpDistance = Vector3.Distance(from, nextTarget);
            if (scene.IsValidForJump(Position, nextTarget, mfCanMoveHeight, this.collisionSize,
                fJumpHeight, fJumpDistance, fTryJumpDistance, ref bCanStand) == false)
            {
                //阻档时，返回后可通行点
                //lastTarget.y = scene.SampleHeight(lastTarget);
                return lastTarget;
            }
            if (bCanStand)
            {
                lastTarget = nextTarget;
            }
            float nextSqrLen = (from - nextTarget).sqrMagnitude;
            if (nextSqrLen >= fSqrLen)
            {
                //lastTarget.y = scene.SampleHeight(lastTarget);
                return lastTarget;
            }
            //尝试点
            nextTarget = nextTarget + tryStep;
        }
        while (true);
    }

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

    /** 尝试移动的目标位置 */
    public Vector3 tryMoveTarget = Vector3.zero;
    /** 越到碰撞的格子类型 */
    public int gridType = 0;

    /**************************************************************************************************
     * 功能 : 尝试移动
     **************************************************************************************************/
    public bool TryMove(Vector3 target)
    {
        // 自动判定是否计算动态碰撞数据
        if (autoComputeDynCollision == true)
            this.dontComputeCollision = true;

       // avoid = false;

        from = Position;

        lastTarget = from;                                          // 最终的目标
        dir = (target - from).normalized;
        tryStep = dir * radius;                                        // 下一目标的位移

        float distance = Vector3.Distance(from, target);

        if (distance < 0.01f)
        {
            tryMoveTarget = lastTarget;
            return false;
        }

        nextTarget = from + tryStep;                                   // 下一个目标

        do
        {
            if (scene.IsValidForWalk(nextTarget, this.collisionSize, out gridType) == false)
            {
                tryMoveTarget = lastTarget;
                return true;
            }
            float nextDistance = Vector3.Distance(from, nextTarget);
            if (nextDistance >= distance)
            {
                // Debug.Log("请求位置-> " + target + " 最终位置-> " + target);
                tryMoveTarget = target;
                return false;
            }
            lastTarget = nextTarget;
            nextTarget = nextTarget + tryStep;
        }
        while (true);
    }

}
