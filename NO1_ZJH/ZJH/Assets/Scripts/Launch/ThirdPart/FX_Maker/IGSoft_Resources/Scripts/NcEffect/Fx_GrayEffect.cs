using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 特效自定义
/// 控制特效shader中的_Amount || _SliceAmount
/// </summary>
public class Fx_GrayEffect : MonoBehaviour, IResetAnimation
{
    public List<EffectEvent> effects;

    public float fDelayTime;

    public GrayEffect targetCompent;

    float fSetAmount;

    bool init = false;

    float time;

    int index;

    float value;

    float speed;

    float fStartTime;

    void Start()
    {
        if (effects == null)
            init = false;
        else
            init = true;
        time = 0;
        index = 0;
        fStartTime = 0;

        targetCompent = GetComponent<GrayEffect>();

        if (targetCompent == null)
            init = false;
        
        if (effects.Count > 0 && init)
        {
            if (targetCompent != null)
                fSetAmount = targetCompent.grayScaleAmount;
            value = fSetAmount;
            float f = effects[0].value;
            speed = (f - value) / effects[0].time;
        }
        else
            init = false;

        if (!init)
            enabled = false;
    }

    void Update()
    {
        if (fStartTime <= fDelayTime)
        {
            fStartTime += Time.deltaTime;
            return;
        }       
        if (!init) return;
        if (index >= effects.Count)
        {
            enabled = false;
            return;
        }
        time += Time.deltaTime;
        if (effects[index] == null)
        {
            index++;
        }
        else 
        {
            value += speed * Time.deltaTime;
            if (time >= effects[index].time)
            {
                value = effects[index].value;
                index++;
                if (index < effects.Count)
                {
                    float f = effects[index].value;
                    speed = (f - value) / (effects[index].time - effects[index - 1].time);
                }
            }

            if (targetCompent != null)
                targetCompent.grayScaleAmount = value;
        }
    }

    bool bParentIsNcDelay = false;
    /// <summary>
    /// 设置父对象是否nc
    /// </summary>
    /// <param name="bValue"></param>
    public void SetParentIsNcDelay(bool bValue)
    {
        bParentIsNcDelay = bValue;
    }

    /// <summary>
    /// 获取父对象是否是nc
    /// </summary>
    /// <returns></returns>
    public bool GetParentIsNcDelay()
    {
        return bParentIsNcDelay;
    }

    /// <summary>
    /// 重置值
    /// </summary>
    public void ResetAnimation()
    {
        if (!init)
            return;

        if (targetCompent != null)
            targetCompent.grayScaleAmount = fSetAmount;

        enabled = true;
        Start();
    }

    void OnDestroy()
    {
        if (this != null && targetCompent != null)
            targetCompent.grayScaleAmount = fSetAmount;
    }
}
