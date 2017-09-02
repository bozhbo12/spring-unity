/// changang1
/// 日期 20150422
/// 实现目标  场景声音播放
using UnityEngine;

public class SceneSoundPlay : MonoBehaviour
{
    public delegate bool PlayObjectAudioDelegate(GameObject oSource, string strAudioId, bool play2D);
    /// <summary>
    /// 声音播放
    /// </summary>
    public static PlayObjectAudioDelegate PlayObjectAudio;
    /// <summary>
    /// 声音ID
    /// </summary>
    public string mstrSoundID;
    //public float mfDelay = 0f;
    /// <summary>
    /// 是否开始计算延迟
    /// </summary>
    bool beCountDelay = false;
    void Awake()
    {
        StartSound();
        //if (mfDelay <= 0)
        //{
        //    beCountDelay = false;
            
        //}
        //else
        //{
        //    beCountDelay = true;
        //}
    }

    void OnEnable()
    {// 重新被激活   重新播放音效
        AudioSource source = gameObject.GetComponent<AudioSource>();
        if (source == null)
            StartSound();
        else
        {
            if (!source.enabled)
                source.enabled = true;
        }
    }

    void StartSound()
    {
        if (PlayObjectAudio != null)
            PlayObjectAudio(gameObject, mstrSoundID,false);
    }

    //void Update()
    //{
    //    if (!beCountDelay)
    //        return;
    //    // 延迟播放
    //    if (mfDelay > 0)
    //    {
    //        mfDelay -= Time.deltaTime;
    //        return;
    //    }
    //    StartSound();
    //    //Destroy(this);
    //}
}
