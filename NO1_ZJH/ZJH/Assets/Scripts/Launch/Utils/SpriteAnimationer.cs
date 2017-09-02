
/// <summary>
/// UISpriteAnimation帧动画设置类;
/// chenjunyi;
/// </summary>
public static class SpriteAnimationer
{
    /// <summary>
    /// 初始化有限次播放帧动画;
    /// </summary>
    /// <param name="loopTime">循环次数</param>
    public static void InitLimitLoopSrpiteAnimation(UISpriteAnimation animation, int loopTime = 1)
    {
        if (animation == null)
        {
            LogSystem.LogWarning("animation is null");
            return;
        }
        animation.loop = false;
        animation.gameObject.SetActive(false);
        animation.Reset(loopTime);
        //播放动画完成重置，并且设置成非活动状态;
        animation.m_callBack = () =>
        {
            animation.Replay();
            animation.gameObject.SetActive(false);
        };
    }

    /// <summary>
    /// 播放帧动画;
    /// </summary>
    /// <param name="animation"></param>
    public static void PlayAnimation(UISpriteAnimation animation)
    {
        if (animation == null)
        {
            LogSystem.LogWarning("animation is null");
            return;
        }
        if (animation.loop)
        {
            //LogSystem.Log("[PlayNoneLoopAnimation] current animation is loop mode,effect name:", animation.gameObject.name);
            return;
        }
        animation.gameObject.SetActive(true);
    }
}