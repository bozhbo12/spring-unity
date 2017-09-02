/*
 * 刷新管理器
 * @处理定时任务
 * @统一刷新回调机制
 * @处理倒计时(防后台)
 */

using UnityEngine;
using System.Collections.Generic;

public class TimerManager : MonoBehaviour
{

#if TIMER

    public static bool bWriteLog = false;
    public class TimerStruct : CacheObject
    {
        public string strTimerKey;
        public float fTime;
        public override string ToString()
        {
            return DelegateProxy.StringBuilder(strTimerKey, ":", fTime);
        }
    }

    public List<TimerStruct> mTimerStruckList = new List<TimerStruct>(8);

#endif

    /// <summary>
    /// 定时器字典
    /// </summary>
    private static DictionaryEx<string, Timer> m_TimerList = new DictionaryEx<string, Timer>();

    public static DictionaryEx<string, Timer> TimerList
    {
        get
        {
            return m_TimerList;
        }
    }

    /// <summary>
    ///   增加队列
    /// </summary>
    private static DictionaryEx<string, Timer> m_AddTimerList = new DictionaryEx<string, Timer>();

    /// <summary>
    ///   销毁队列
    /// </summary>
    private static List<string> m_DestroyTimerList = new List<string>();

    public delegate void TimerManagerHandler();

    public delegate void TimerManagerHandlerArgs(params object[] args);
   
    /// <summary>
    /// 倒计时回调
    /// </summary>
    /// <param name="count">倒计时剩余时间</param>
    /// <param name="args">倒计时参数</param>
    public delegate void TimerManagerCountHandlerArgs(int count, params object[] args);


    // Use this for initialization
    void Awake()
    {
    }
    public static void ClearTimer()
    {
        m_DestroyTimerList.Clear();
        m_AddTimerList.Clear();
        m_TimerList.Clear();
    }

    /// <summary>
    /// 清除指定计时器
    /// </summary>
    /// <param name="Key"></param>
    public static void ClearOneTimer(string key)
    {
        List<string> keyList = new List<string>();
        for (int i = 0; i < m_AddTimerList.Count; i++)
        {
            if (i < m_AddTimerList.Count && m_AddTimerList.mList[i] != null)
            {
                if (m_AddTimerList.mList[i].StartsWith(key))
                {
                    keyList.Add(m_AddTimerList.mList[i]);
                }
            }
        }
        for (int i = 0; i < m_TimerList.Count; i++)
        {
            if (i < m_TimerList.Count && m_TimerList.mList[i] != null)
            {
                if (m_TimerList.mList[i].StartsWith(key))
                {
                    keyList.Add(m_TimerList.mList[i]);
                }
            }
        }
        for (int i = 0; i < keyList.Count; i++)
        {
            if (i < keyList.Count && keyList[i] != null)
            {
                m_AddTimerList.Remove(keyList[i]);
                m_TimerList.Remove(keyList[i]);
            }
        }
        keyList.Clear();
    }

    #region 刷新器管理
    // Update is called once per frame
    void Update()
    {
        if (m_DestroyTimerList.Count > 0)
        {
            ///>从销毁队列中销毁指定内容
            for (int i = 0; i < m_DestroyTimerList.Count; i++)
            {
                m_TimerList.Remove(m_DestroyTimerList[i]);
            }

            //清空
            m_DestroyTimerList.Clear();
        }

        if (m_AddTimerList.Count > 0)
        {
            Timer value = null;
            ///>从增加队列中增加定时器
            for (int i = 0, imax = m_AddTimerList.mList.Count; i < imax; i++)
            {
                if (!m_AddTimerList.GetTryValue(i, out value) || value == null)
                    continue;

                if (string.IsNullOrEmpty(value.m_Name))
                    continue;

                if (m_TimerList.ContainsKey(value.m_Name))
                {
                    m_TimerList[value.m_Name] = value;
                }
                else
                {
                    m_TimerList.Add(value.m_Name, value);
                }
            }
            //清空
            m_AddTimerList.Clear();
        }

#if TIMER

        if (mTimerStruckList.Count > 0)
        {
            for (int i = 0; i < mTimerStruckList.Count; i++)
            {
                CacheObjects.DespawnCache(mTimerStruckList[i]);
            }
            mTimerStruckList.Clear();
        }

#endif

        if (m_TimerList.Count > 0)
        {

#if TIMER
            TimerStruct tStruct;
#endif
            Timer value;
            //响应定时器
            for (int i = 0, imax = m_TimerList.mList.Count; i < imax; i++)
            {
                if (!m_TimerList.GetTryValue(i, out value) || value == null)
                    continue;

#if TIMER
                tStruct = CacheObjects.SpawnCache<TimerStruct>();
                tStruct.strTimerKey = m_TimerList.mList[i];
                float tempTime = Time.realtimeSinceStartup;
#endif
                value.Run();
#if TIMER
                tStruct.fTime = (Time.realtimeSinceStartup - tempTime) * 1000;
                mTimerStruckList.Add(tStruct);
#endif
            }
        }
#if TIMER
        if (mTimerStruckList.Count > 0)
        {
            mTimerStruckList.Sort((TimerStruct ts1, TimerStruct ts2) =>{
                return ts1.fTime.CompareTo(ts2.fTime);
            });
            if (bWriteLog)
            {
                for (int i = 0; i < mTimerStruckList.Count; i++)
                {
                    LogSystem.Log("时间:", mTimerStruckList[i]);
                }
            }
        }
#endif
    }

#if TIMER

    GUIStyle fontStyle = new GUIStyle();  
    void OnGUI()
    {
        if (mTimerStruckList.Count == null)
            return;

        fontStyle.fontSize = 30;       //字体大小  
        fontStyle.normal.textColor = new Color(1, 0, 0);
        for(int i = 0; i<mTimerStruckList.Count; i++)
        {
            GUI.Label(new Rect(0, 10+(i*30), 400, 30), mTimerStruckList[i].ToString(), fontStyle);
        }
        
    }

#endif

    public void DebugPrintTimerRunTime()
    {
        for (int i = 0, imax = m_TimerList.mList.Count; i < imax; i++)
        {
            Timer value = m_TimerList[m_TimerList.mList[i]];
            if (value == null)
                return;

            float tempTime = Time.realtimeSinceStartup;
            value.Run();
            LogSystem.Log("usetime:", m_TimerList.mList[i],"  " ,(Time.realtimeSinceStartup - tempTime));
        }
    }


    /// -----------------------------------------------------------------------------
    /// <summary>
    /// 增加刷新器
    /// </summary>
    /// <param name=string.Empty></param>
    /// <returns></returns>
    /// -----------------------------------------------------------------------------
    public static bool AddTimer(string key, float duration, TimerManagerHandler handler)
    {
        if (m_DestroyTimerList.Contains(key))
            m_DestroyTimerList.Remove(key);

        return Internal_AddTimer(key, TIMER_MODE.NORMAL, duration, handler);
    }
    public static bool AddTimer(string key, float duration, TimerManagerHandlerArgs handler, params object[] args)
    {
        if (m_DestroyTimerList.Contains(key))
            m_DestroyTimerList.Remove(key);
        return Internal_AddTimer(key, TIMER_MODE.NORMAL, duration, handler, args);
    }


    /// -----------------------------------------------------------------------------
    /// <summary>
    /// 增加持续刷新器
    /// </summary>
    /// <param name=string.Empty></param>
    /// <returns></returns>
    /// -----------------------------------------------------------------------------
    public static bool AddTimerRepeat(string key, float duration, TimerManagerHandler handler)
    {
        if (m_DestroyTimerList.Contains(key))
            m_DestroyTimerList.Remove(key);
        return Internal_AddTimer(key, TIMER_MODE.REPEAT, duration, handler);
    }
    public static bool AddTimerRepeat(string key, float duration, TimerManagerHandlerArgs handler, params object[] args)
    {
        if (m_DestroyTimerList.Contains(key))
            m_DestroyTimerList.Remove(key);
        return Internal_AddTimer(key, TIMER_MODE.REPEAT, duration, handler, args);
    }
    /// <summary>
    /// 刷新倒计时
    /// </summary>
    /// <param name="key">标记字符</param>
    /// <param name="duration">倒计时时间</param>
    /// <param name="handler">计时回调</param>
    /// <param name="args">计时参数</param>
    /// <returns></returns>
    public static bool AddTimerCount(string key, float duration, TimerManagerCountHandlerArgs handler, params object[] args)
    {
        if (m_DestroyTimerList.Contains(key))
            m_DestroyTimerList.Remove(key);
        return Internal_AddTimer(key, TIMER_MODE.COUNTTIME, duration, handler, args);
    }

    /// <summary>
    /// 添加延时触发心跳
    /// </summary>
    /// <param name="key">键值</param>
    /// <param name="duration">延时时间</param>
    /// <param name="handler">回调句柄</param>
    /// <returns>添加成/败</returns>
    public static bool AddDelayTimer(string key, float duration, TimerManagerHandler handler)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        if (duration < 0.0f)
        {
            if (handler != null)
            {
                handler();
            }
            return true;
        }

        if (m_DestroyTimerList.Contains(key))
            m_DestroyTimerList.Remove(key);

        Timer timer = new Timer(key, TIMER_MODE.DELAYTIME, Time.realtimeSinceStartup, duration, handler);

        if (m_TimerList.ContainsKey(key))
        {
            m_TimerList[key] = timer;
        }
        else
        {
            m_TimerList.Add(key, timer);
        }

        return true;
    }
    /// <summary>
    /// 添加延时触发心跳
    /// </summary>
    /// <param name="key">键值</param>
    /// <param name="duration">延时时间</param>
    /// <param name="handler">回调句柄</param>
    /// <returns>添加成/败</returns>
    public static bool AddDelayTimer(string key, float duration, TimerManagerHandlerArgs handler, params object[] args)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        if (duration < 0.0f)
        {
            if (handler != null)
            {
                handler();
            }
            return true;
        }

        if (m_DestroyTimerList.Contains(key))
            m_DestroyTimerList.Remove(key);

        Timer timer = new Timer(key, TIMER_MODE.DELAYTIME, Time.realtimeSinceStartup, duration, handler, args);

        if (m_TimerList.ContainsKey(key))
        {
            m_TimerList[key] = timer;
        }
        else
        {
            m_TimerList.Add(key, timer);
        }

        return true;
    }
    /// <summary>
    /// 销毁带有前缀的所有刷新器
    /// </summary>
    /// <param name="prefix"></param>
    public static void ClearTimerWithPrefix(string prefix)
    {
        if (m_TimerList != null && m_TimerList.Count > 0)
        {
            foreach (string timerKey in m_TimerList.Keys)
            {
                if (timerKey.StartsWith(prefix))
                {
                    Destroy(timerKey);
                }
            }
        }
    }

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// 销毁指定定时器
    /// </summary>
    /// <param name="key">标识符</param>
    /// <returns></returns>
    /// -----------------------------------------------------------------------------
    public static bool Destroy(string key)
    {
        if (!m_TimerList.ContainsKey(key))
        {
            if (m_AddTimerList.ContainsKey(key))
            {
                m_AddTimerList.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        if (!m_DestroyTimerList.Contains(key))
        {
            m_DestroyTimerList.Add(key);
        }

        return true;
    }

    /// <summary>
    /// 是否含用定时器
    /// </summary>
    /// <returns><c>true</c> if this instance is have timer the specified key; otherwise, <c>false</c>.</returns>
    /// <param name="key">Key.</param>
    public static bool IsHaveTimer(string key)
    {
        if(m_TimerList.ContainsKey(key) || m_AddTimerList.ContainsKey(key))
        {
            return !m_DestroyTimerList.Contains(key);
        }
        return false;
    }

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// 增加定时器
    /// </summary>
    /// <param name=string.Empty></param>
    /// <returns></returns>
    /// -----------------------------------------------------------------------------
    private static bool Internal_AddTimer(string key, TIMER_MODE mode, float duration, TimerManagerHandler handler)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        if (duration < 0.0f)
            return false;

        Timer timer = new Timer(key, mode, Time.realtimeSinceStartup, duration, handler);

        if (m_AddTimerList.ContainsKey(key))
        {
            m_AddTimerList[key] = timer;
        }
        else
        {
            m_AddTimerList.Add(key, timer);
        }

        return true;
    }

    private static bool Internal_AddTimer(string key, TIMER_MODE mode, float duration, TimerManagerHandlerArgs handler, params object[] args)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        if (duration < 0.0f)
            return false;

        Timer timer = new Timer(key, mode, Time.realtimeSinceStartup, duration, handler, args);

        if (m_AddTimerList.ContainsKey(key))
        {
            m_AddTimerList[key] = timer;
        }
        else
        {
            m_AddTimerList.Add(key, timer);
        }

        return true;
    }

    private static bool Internal_AddTimer(string key, TIMER_MODE mode, float duration, TimerManagerCountHandlerArgs handler, params object[] args)
    {
        if (string.IsNullOrEmpty(key))
            return false;

        if (duration < 0.0f)
            return false;

        Timer timer = new Timer(key, mode, Time.realtimeSinceStartup, duration, handler, args);

        if (m_AddTimerList.ContainsKey(key))
        {
            m_AddTimerList[key] = timer;
        }
        else
        {
            m_AddTimerList.Add(key, timer);
        }

        return true;
    }
    /// <summary>
    /// 获取指定前缀计时器
    /// </summary>
    /// <param name="prefix"></param>
    public static Timer GetTimerWithPrefix(string prefix)
    {
        Timer timer = null;
        if (m_TimerList != null && m_TimerList.Count > 0)
        {
            foreach (string timerKey in m_TimerList.Keys)
            {
                if (timerKey.StartsWith(prefix))
                {
                  if(m_TimerList.TryGetValue(timerKey,out timer))
                    {
                        return timer;
                    }
                }
            }
        }
        return timer;
    }
    public static bool IsRunning(string key)
    {
        return m_TimerList.ContainsKey(key);
    }

    /// -----------------------------------------------------------------------------
    /// <summary>
    ///  定时器模式
    /// </summary>
    /// -----------------------------------------------------------------------------
    public enum TIMER_MODE
    {
        NORMAL,
        REPEAT,
        COUNTTIME,
        DELAYTIME,
    }

    public class Timer
    {
        /// <summary>
        ///   名称
        /// </summary>
        public string m_Name;

        /// <summary>
        ///   模式
        /// </summary>
        private TIMER_MODE m_Mode;

        /// <summary>
        ///   开始时间
        /// </summary>
        private float m_StartTime;

        /// <summary>
        ///   时长
        /// </summary>
        private float m_duration;

        /// <summary>
        ///   时间点
        /// </summary>
        private float m_time = 0;

        /// <summary>
        ///   定时器委托
        /// </summary>
        private TimerManagerHandler m_TimerEvent;

        private TimerManagerHandlerArgs m_TimerArgsEvent;

        private TimerManagerCountHandlerArgs m_TimerCountArgsEvent;

        private object[] m_Args = null;

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// 开始时间
        /// </summary>
        /// <param name=string.Empty></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------
        public float StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// 剩余时间
        /// </summary>
        /// <param name=string.Empty></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------
        public float TimeLeft
        {
            get
            {
                return Mathf.Max(0.0f, m_duration - (Time.realtimeSinceStartup - m_StartTime));
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name=string.Empty></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------
        public Timer(string name, TIMER_MODE mode, float startTime, float duration, TimerManagerHandler handler)
        {
            m_Name = name;
            m_Mode = mode;
            m_StartTime = startTime;
            m_duration = duration;
            m_TimerEvent = handler;
            //m_Manger = manager;
        }

        public Timer(string name, TIMER_MODE mode, float startTime, float duration, TimerManagerHandlerArgs handler, params object[] args)
        {
            m_Name = name;
            m_Mode = mode;
            m_StartTime = startTime;
            m_duration = duration;
            m_TimerArgsEvent = handler;

            m_Args = args;
        }

        public Timer(string name, TIMER_MODE mode, float startTime, float duration, TimerManagerCountHandlerArgs handler, params object[] args)
        {
            m_Name = name;
            m_Mode = mode;
            m_StartTime = startTime;
            m_duration = duration;
            m_TimerCountArgsEvent = handler;

            m_Args = args;
        }
        public void ExecuterArgHandler()
        {
            if (this.m_TimerArgsEvent != null && AsyncTrigger.IsTargetValid(this.m_TimerArgsEvent.Target))
            {
                try
                {
                    this.m_TimerArgsEvent(m_Args);
                }
                catch (System.Exception ex)
                {
                    TimerManager.Destroy(this.m_Name);
                    LogSystem.LogError("Time event catch 1", ex.ToString());
                }
            }
        }
        /// -----------------------------------------------------------------------------
        /// <summary>
        /// 运行事件
        /// </summary>
        /// <param name=string.Empty></param>
        /// <returns></returns>
        /// -----------------------------------------------------------------------------
        public void Run()
        {
            if (m_Mode == TIMER_MODE.DELAYTIME)
            {
                if (Time.realtimeSinceStartup - m_StartTime > m_duration)
                {
                    if (this.m_TimerEvent != null && AsyncTrigger.IsTargetValid(this.m_TimerEvent.Target))
                    {
                        try
                        {
                            this.m_TimerEvent();
                        }
                        catch (System.Exception ex)
                        {
                            TimerManager.Destroy(this.m_Name);
                            LogSystem.LogError("Time event catch 1", ex.ToString());
                        }
                    }
                    else if (this.m_TimerArgsEvent != null && AsyncTrigger.IsTargetValid(this.m_TimerArgsEvent.Target))
                    {
                        try
                        {
                            this.m_TimerArgsEvent(m_Args);
                        }
                        catch (System.Exception ex)
                        {
                            TimerManager.Destroy(this.m_Name);
                            LogSystem.LogError("Time event catch 1", ex.ToString());
                        }
                    }
                    TimerManager.Destroy(this.m_Name);
                }
                return;
            }
            else if (m_Mode == TIMER_MODE.COUNTTIME)
            {
                float lastTime = Time.realtimeSinceStartup - m_time;
                if (lastTime > 1.0f)
                {
                    m_time = Time.realtimeSinceStartup;
                    try
                    {
                        if (this.m_TimerCountArgsEvent != null && AsyncTrigger.IsTargetValid(this.m_TimerCountArgsEvent.Target))
                        {
                            this.m_TimerCountArgsEvent(Mathf.CeilToInt(this.TimeLeft), m_Args);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        TimerManager.Destroy(this.m_Name);
                        LogSystem.LogError("Time event catch2 ", ex.ToString());
                    }
                 
                    if (this.TimeLeft <= 0f)
                    {
                        TimerManager.Destroy(this.m_Name);
                    }
                }
                return;
            }

            if (this.TimeLeft > 0.0f)
                return;

            try
            {
                if (this.m_TimerEvent != null && AsyncTrigger.IsTargetValid(this.m_TimerEvent.Target))
                {
                    this.m_TimerEvent();
                }

                if (this.m_TimerArgsEvent != null && AsyncTrigger.IsTargetValid(this.m_TimerArgsEvent.Target))
                {
                    this.m_TimerArgsEvent(m_Args);
                }
            }
            catch (System.Exception ex)
            {
                TimerManager.Destroy(this.m_Name);
                LogSystem.LogError("Time event catch3", ex.ToString());
            }

            if (m_Mode == TIMER_MODE.NORMAL)
            {
                TimerManager.Destroy(this.m_Name);
            }
            else
            {
                m_StartTime = Time.realtimeSinceStartup;
            }
        }
    }
    #endregion

}
