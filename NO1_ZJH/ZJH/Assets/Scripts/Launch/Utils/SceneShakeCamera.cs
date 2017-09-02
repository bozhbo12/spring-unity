using UnityEngine;
/// <summary>
/// 震屏(随机5 8 秒一次)
/// </summary>
public class SceneShakeCamera : MonoBehaviour
{

    public delegate void SceneShakeCameraDelegate(float shakeStrength = 0.2f, float rate = 14, float shakeTime = 0.4f);
    /// <summary>
    /// 声音播放
    /// </summary>
    public static SceneShakeCameraDelegate ShakeCamera;
    /// <summary>
    /// 震动幅度
    /// </summary>
    public float shakeStrength = 0.2f;

    /// <summary>
    /// 震动频率
    /// </summary>
    public float rate = 14f;

    /// <summary>
    /// 震动时长
    /// </summary>
    private float shakeTime = 0.4f;

    public int miMin = 5;
    public int miMax = 8;

    private float miCurNum = 0f;
    // Use this for initialization
    void Start()
    {
        if (ShakeCamera == null)
            enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (miCurNum <= 0f)
        {
            miCurNum = Random.Range(miMin, miMax);
            if (ShakeCamera != null)
                ShakeCamera(shakeStrength, rate, shakeTime);
        }
        else
        {
            miCurNum -= Time.deltaTime;
        }
    }
}
