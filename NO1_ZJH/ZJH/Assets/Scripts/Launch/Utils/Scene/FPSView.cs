using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class FPSView : MonoBehaviour
{
    /** 当前场景的帧频 */
    private float updateInterval = 0.5f;
    private float lastInterval = Time.realtimeSinceStartup;
    private int frames = 0;
    public float fps = 1f;
    private float oldfps = 1f;
    public float ms = 1f;
    public float time = 0f;
    private int tick = 0;
    private int viewRect = 1;

    // Use this for initialization
    void Start()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = 30;
#endif
    }

    void OnGUI()
    {
        GUI.TextField(new Rect(10f, 10f, 300f, 35f), "fps->" + fps);
    }


    // Update is called once per frame
    void Update()
    {
        frames++;
        var timeNow = Time.time;
        if (timeNow > lastInterval + updateInterval)
        {
            fps = frames / (timeNow - lastInterval);
            ms = 1000.0f / Mathf.Max(fps, 0.00001f);
            frames = 0;
            lastInterval = timeNow;
        }
    }
}
