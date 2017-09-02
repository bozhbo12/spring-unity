using UnityEngine;

/// <summary>
/// 随机点 随机时间 震屏（落石）
/// </summary>
public class SceneSpecialEffects : MonoBehaviour
{
    /// <summary>
    /// 落石效果
    /// </summary>
    public GameObject mGoEffect;
    /// <summary>
    /// 偏移位置
    /// </summary>
    public float mfMinX;
    public float mfMaxX;
    public float mfMinY;
    public float mfMaxY;
    public float mfMinZ;
    public float mfMaxZ;

    /// <summary>
    /// 时间区间
    /// </summary>
    public float mfMaxTime;
    public float mfMinTime;

    /// <summary>
    /// 存在时间
    /// </summary>
    public float mfLiveTime = 3f;

    Vector3 mvecOffset = Vector3.zero;
    float mfBornTime;
    // Use this for initialization
    void Start()
    {
        if (mGoEffect == null)
        {
            Destroy(this);
            LogSystem.LogWarning("SceneSpecialEffects::not find Gameobject");
        }
        mGoEffect.SetActive(false);
        mfBornTime = Randow(mfMinTime, mfMaxTime);
    }

    // Update is called once per frame
    void Update()
    {
        mfBornTime -= Time.deltaTime;
        if (mfBornTime < 0)
        {
            mfBornTime = Randow(mfMinTime, mfMaxTime);
            mvecOffset.x = Randow(mfMinX, mfMaxX);
            mvecOffset.y = Randow(mfMinY, mfMaxY);
            mvecOffset.z = Randow(mfMinZ, mfMaxZ);
            PlayObject();
        }
    }

    /// <summary>
    /// 生成实例
    /// </summary>
    void PlayObject()
    {
        if (mGoEffect == null)
            return;
        GameObject o = GameObject.Instantiate(mGoEffect, Vector3.zero, Quaternion.identity) as GameObject;
        if (o == null)
            return;
        o.transform.parent = transform;
        o.transform.localRotation = mGoEffect.transform.localRotation;
        o.transform.localScale = mGoEffect.transform.localScale;
        o.transform.localPosition = mGoEffect.transform.localPosition + mvecOffset;
        o.SetActive(true);
        DestroyForTime dTime = o.AddComponent<DestroyForTime>();
        dTime.time = mfLiveTime;
    }

    /// <summary>
    /// 随机值
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    float Randow(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}
