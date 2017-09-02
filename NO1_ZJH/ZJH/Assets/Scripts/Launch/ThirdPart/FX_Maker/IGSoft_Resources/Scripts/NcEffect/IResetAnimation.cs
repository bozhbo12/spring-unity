/// <summary>
/// 特效重新初使化接口
/// </summary>
public interface IResetAnimation
{
    /// <summary>
    /// 重置动画 
    /// 父节点使用ncDelay延迟,子节点也做了延迟时,统一调用重置接口会发生ncDelay延迟失效问题
    /// 原因是在统一调用接口中会重置子节点，这时子节点已经开始做计时，导致ncdelay失效
    /// 统一调用先是ncDelay做延迟，当nc完成延迟后再调用子节点延迟
    /// </summary>
    void ResetAnimation();

    /// <summary>
    /// 设置父对象是否nc
    /// </summary>
    /// <param name="bValue"></param>
    void SetParentIsNcDelay(bool bValue);

    /// <summary>
    /// 获取父对象是否是nc
    /// </summary>
    /// <returns></returns>
    bool GetParentIsNcDelay();
}