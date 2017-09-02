/// 作者 zhangrj
/// 日期 20140923
/// 实现目标  U3D页签公用类
/// 跟新 20141017
///<summary>
///修改描述：增加标签页等级打开功能
///作者：高伟伟
///修改日期：20161116
///</summary>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate bool TouchEvent(int index, GameObject go);
public delegate bool CanTabActive(int index);

/*********************************
 *      常用组件单选按钮       
 *      使用时调用init方法
 *      绑定回调touchEvent（int）
 * ******************************/
[AddComponentMenu("NGUI/Custom/SingleSelectionButton")]
public class SingleSelectionButton : MonoBehaviour
{
    public enum LayoutDirect
    {
        Horizontal,
        Vertical,
        Custom,
    }
    /// <summary>
    /// 点击事件回调，记录上次点击是否与本次一样,二选一不能同时监听,先回调在改变CurrentIndex
    /// </summary>
    public TouchEvent touchEvent;

    /// <summary>
    /// 点击事件回调，不记录上次点击是否与本次一样,二选一不能同时监听,先回调在改变CurrentIndex
    /// 每次点击都会有回调
    /// </summary>
    public TouchEvent repeatTouchEvent;

    public CanTabActive canTabActive;
    /// <summary>
    /// 单选按钮背景
    /// </summary>
    UIAtlas mAtlas = null;
    /// <summary>
    /// 图集名
    /// </summary>
    [HideInInspector]
    [SerializeField]
    string mAtlasName = string.Empty;

    string mstrLoadAtlasName = string.Empty;

    /// <summary>
    /// 图集
    /// </summary>
    public UIAtlas atlas
    {
        get
        {
            return mAtlas;
        }
        set
        {
            if (mAtlas != value)
            {
                if (!string.IsNullOrEmpty(mstrLoadAtlasName))
                {
                    if (atlas != null)
                    {
                        CacheObjects.PopCache(atlas.gameObject);
                    }
                    mstrLoadAtlasName = string.Empty;
                }

                mAtlas = value;

#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    string strAtlasPath = UnityEditor.AssetDatabase.GetAssetPath(mAtlas);
                    if (strAtlasPath.Contains("/Local/"))
                    {
                        string strFix1 = "/Local/";
                        int iIndex = strAtlasPath.LastIndexOf(strFix1);
                        if (iIndex > 0)
                        {
                            strAtlasPath = strAtlasPath.Substring(iIndex + 1);
                            int iIndex2 = strAtlasPath.IndexOf(".");
                            if (iIndex2 > 0)
                            {
                                strAtlasPath = strAtlasPath.Substring(0, iIndex2);
                            }
                        }
                    }
                    else
                    {
                        string strFix = "/UIAtlas/" +"/";
                        int iIndex = strAtlasPath.LastIndexOf(strFix);
                        if (iIndex > 0)
                        {
                            strAtlasPath = strAtlasPath.Substring(iIndex + strFix.Length);
                            int iIndex2 = strAtlasPath.IndexOf(".");
                            if (iIndex2 > 0)
                            {
                                strAtlasPath = strAtlasPath.Substring(0, iIndex2);
                            }
                        }
                    }
                    mAtlasName = strAtlasPath;
                }
#endif
            }
        }
    }
    [SerializeField]
    public int Height = 100;
    [SerializeField]
    public LayoutDirect Direct = LayoutDirect.Custom;
    [SerializeField]
    public Color LabNormalColor = Color.white;   //Label一般颜色
    [SerializeField]
    public Color LabPressColor = Color.white;    //Label选中颜色

    [SerializeField]
    public Color OutlineNorColor = Color.white;   //Label一般描边颜色
    [SerializeField]
    public Color OutlinePreColor = Color.white;   //Label选中描边颜色

    [SerializeField]
    public int LabNormalSize = 0; //Label一般字体大小
    [SerializeField]
    public int LabPressSize = 0;  //Label选中字体大小

    [SerializeField]
    public int LabNormalX = 0; //Label一般X偏移
    [SerializeField]
    public int LabPressX = 0; //Label选中X偏移

    [SerializeField]
    public int SprNormalX = 0; //Sprite一般X偏移
    [SerializeField]
    public int SprPressX = 0; //Sprite选中X偏移

    //----add---
    [SerializeField]
    public Vector2 SpriteNormalSize = Vector2.zero;
    [SerializeField]
    public Vector2 SpritePressSize = Vector2.zero;

    [SerializeField]
    public Vector2 RedNorCoord = Vector2.zero;
    [SerializeField]
    public Vector2 RedPreCoord = Vector2.zero;

    [SerializeField]
    public int RedNorSize = 0;
    [SerializeField]
    public int RedPreSize = 0;

    [SerializeField]
    public int NorPosFrom = 0; //一般坐标开始
    [SerializeField]
    public int NorPosTo = 0; //一般坐标目标

    [SerializeField]
    public int PrePosFrom = 0; //一般坐标开始
    [SerializeField]
    public int PrePosTo = 0; //一般坐标目标

    [SerializeField]
    public float AnimaInterval = 0; //间隔时间

    public Sprite[] spritesName;   //背景色

   
    public int currentIndex;
    public int perviousIndex;
    CustomList<GameObject> children;
    //CustomList<GameObject> childrenTween;

    CustomList<UISprite>[] activeSprite;
    CustomList<UILabel>[] activeLabel;
    /// <summary>
    /// 缓存坐标
    /// </summary>
    Vector3 temp = Vector3.zero;
    /// <summary>
    /// 播放动画缓存
    /// </summary>
    bool cachePlayTween = false;
    public TAB[] tabs;

    public delegate void CallLoadAsset(string strFileName, AssetCallback callback, VarStore varStore = null, bool bAsync = false);
    public static CallLoadAsset monLoadAsset = null;
    public static void SetLoadAssetCall(CallLoadAsset call)
    {
        monLoadAsset = call;
    }

    void Awake()
    {
        if (!string.IsNullOrEmpty(mAtlasName) && mAtlas == null)
        {
            if (Application.isPlaying)
            {
                if (mAtlasName.StartsWith("Local/"))
                {
                    if (monLoadAsset != null)
                    {
                        monLoadAsset(mAtlasName, OnFileLoaded,null);
                    }
                    else
                    {
                        ///编辑器模式
                        Object oAsset = Resources.Load(mAtlasName);
                        CacheObjects.PushCache(mAtlasName, oAsset);
                        OnFileLoaded(oAsset, mAtlasName);
                    }
                }
                else
                {
                    string strFileName = "Prefabs/UIAtlas/"  + mAtlasName;
                    if (monLoadAsset != null)
                    {
                        monLoadAsset(strFileName, OnFileLoaded,null);
                    }
                    else
                    {
                        ///编辑器模式
                        Object oAsset = Resources.Load(strFileName);
                        CacheObjects.PushCache(mAtlasName, oAsset);
                        OnFileLoaded(oAsset, strFileName);
                    }
                }
            }
#if UNITY_EDITOR
            else
            {
                string strFileName = "Assets/Resources/Prefabs/UIAtlas/" + "/" + mAtlasName + ".prefab";
                Object oAsset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(strFileName) as Object;
                OnFileLoaded(oAsset, strFileName);
            }
#endif
        }
    }

    /// <summary>
    /// 网格加载结束
    /// </summary>
    /// <param name="args"></param>
    private void OnFileLoaded(UnityEngine.Object oAsset2, string strFileName, VarStore varStore=null)
    {
        GameObject oAsset = oAsset2 as GameObject;
        if (oAsset == null) return;

        UIAtlas uiAtlas = oAsset.GetComponent<UIAtlas>();
        if (uiAtlas != null)
        {
            atlas = uiAtlas;
        }
    }

    /// <summary>
    /// 是否播放
    /// </summary>
    /// <returns></returns>
    private bool IsPaly()
    {
        return (cachePlayTween && (NorPosFrom != NorPosTo) && (PrePosFrom != PrePosTo));
    }

    public void Init(bool isFristCall = false, bool playTween = true, string[] contents = null)
    {
        if (isFristCall)
        {
            currentIndex = -1;
        }
        else
        {
            currentIndex = 0;
        }
        cachePlayTween = playTween;
        perviousIndex = -1;
        int len = transform.childCount;
        if (len <= 0) throw new System.Exception("transform don't have child");
        children = new CustomList<GameObject>();
        if (IsPaly())
        {
            activeSprite = new CustomList<UISprite>[len];
            activeLabel = new CustomList<UILabel>[len];
        }
        
        tabs = new TAB[len];
        for (int i = 0; i < len; i++)
        {
            children.Add(transform.GetChild(i).gameObject);
        }
        children.Sort();
        for (int j = 0; j < len; j++)
        {
            UISprite[] _sprites = children[j].GetComponentsInChildren<UISprite>(true);
            UILabel[] lbl = children[j].GetComponentsInChildren<UILabel>(true);

            if (IsPaly())
            {
                activeSprite[j] = new CustomList<UISprite>(_sprites);
                activeLabel[j] = new CustomList<UILabel>(lbl);
            }

            if (lbl != null && lbl.Length > 0)
            {
                tabs[j].lbl = lbl[0];
                if (contents != null && !string.IsNullOrEmpty(contents[j]))
                {
                    tabs[j].lbl.text = contents[j];
                }
            }
            int sL = _sprites.Length;
            tabs[j].auxSprite = new List<UISprite>();
            if (sL == 1)
            {
                tabs[j].back = _sprites[0];
            }
            else
            {
                for (int k = 0; k < sL; k++)
                {
                   
                    if (_sprites[k].name.StartsWith("back"))
                    {
                        tabs[j].back = _sprites[k];
//                         _sprites[k].atlas = mAtlas;
                    }
                    else if (_sprites[k].name.StartsWith("red"))
                    {
                        tabs[j].tabLogo = _sprites[k];
                    }
                    else
                        tabs[j].auxSprite.Add(_sprites[k]);
                }
            }

          
            UIEventListener.Get(children[j]).onClick = onTouch;
        }
        //初始化等级限制,重新定位tab
        int posY = -Height;
        if (children != null && Direct == LayoutDirect.Vertical)
        {
            for (int index = 0; index < children.Length; index++)
            {
                if (canTabActive != null)
                {
                    GameObject obj = children[index];
                    if (!canTabActive(index))
                    {
                        obj.SetActive(false);
                    }
                    else
                    {
                        posY += Height;
                    }
                    Vector3 pos = obj.transform.localPosition;
                    pos.y = -posY;
                    obj.transform.localPosition = pos;
                }
                else
                {
                    GameObject obj = children[index];
                    Vector3 pos = obj.transform.localPosition;
                    pos.y = -index * Height;
                    obj.transform.localPosition = pos;
                }
            }
        }

        if (IsPaly())
            ResetPosAndDepth();

        Normal();
        if (!isFristCall)
            Active();


        if (isFristCall && children != null && children.Length > 0)
        {
            onTouch(children[0]);
        }
    }

    /// <summary>
    /// 重置
    /// </summary>
    private void ResetPosAndDepth()
    {
        if (!IsPaly())
            return;

        int curIndex = 0;
        if (currentIndex >= 0 && children.Length > curIndex)
            curIndex = currentIndex;

        for (int i = 0; i < children.Length; i++)
        {
            temp = children[i].transform.localPosition;

            if (i == curIndex)
                temp.x = PrePosFrom;
            else
                temp.x = NorPosFrom;

            children[i].transform.localPosition = temp;
        }

        SetUIWidgetDepth(cachePlayTween, true);

    }

    /// <summary>
    /// 播放动画基本在OnShow里面调用
    /// </summary>
    public void OnPlayUIAnimation(bool Immediately = true,float delay = 0.2f)
    {
        if (Immediately)
        {
            PlayUIAnmation(AnimaInterval);
        }
        else
        {
            string key = DelegateProxy.StringBuilder(mTimerKey, gameObject.GetInstanceID());
            TimerManager.AddTimer(key, delay, DelayPalyUIAnimation, key);
        }
        
    }

    /// <summary>
    /// 延时播放动画
    /// </summary>
    /// <param name="args"></param>
    private void DelayPalyUIAnimation(params object[] args)
    {
        if (args != null && args.Length == 1)
        {
            string key = args[0] as string;
            
            if (string.IsNullOrEmpty(key))
                return;

            TimerManager.Destroy(key);

            PlayUIAnmation(AnimaInterval);
        }
    }

    private void SetUIWidgetDepth(bool playTween, bool before)
    {
        if (playTween == false)
            return;
        
        int temp = 0;
        for (int i = 0; i < activeSprite.Length; i++)
        {
            for (int j = 0; j < activeSprite[i].Length; j++)
            {
                temp = activeSprite[i][j].depth;
                activeSprite[i][j].depth = before ? temp - 100: temp + 100;
            }

            for (int j = 0; j < activeLabel[i].Length; j++)
            {
                temp = activeLabel[i][j].depth;
                activeLabel[i][j].depth = before ? temp - 100 : temp + 100;
            }
        }
    }

   

    private string mTimerKey = "SingleSelectionButton";
    private void PlayUIAnmation(float time = 0.1f)
    {
        if (!IsPaly())
            return;

        int curIndex = 0;
        if (currentIndex >= 0 && children.Length > curIndex)
            curIndex = currentIndex;

        if (children != null && children.Length > 0)
        {
            for (int i = 0; i < children.Length; i++)
            {

                temp = children[i].transform.localPosition;
                if (i == curIndex)
                    temp.x = PrePosTo;
                else
                    temp.x = NorPosTo;

                TweenPosition posTween = TweenPosition.Begin(children[i], i * time, temp);
                string key = DelegateProxy.StringBuilder(mTimerKey, children[i].GetInstanceID());
                TimerManager.AddTimer(key, i * time, OnFinishTween, i, key);
                //posTween.method = UITweener.Method.EaseInOut;
                posTween.PlayForward();
            }
        }
    }

   
    void OnFinishTween(params object[] args)
    {
        if (args != null && args.Length == 2)
        {
            int childIndex = (int)args[0] ;
            string key = args[1] as string;

            if (childIndex < 0 || childIndex >= children.Length)
                return;

            if (string.IsNullOrEmpty(key))
                return;

            TimerManager.Destroy(key);

            int temp = 0;
            for (int j = 0; j < activeSprite[childIndex].Length; j++)
            {
                temp = activeSprite[childIndex][j].depth;
                activeSprite[childIndex][j].depth = temp + 100;
            }

            for (int j = 0; j < activeLabel[childIndex].Length; j++)
            {
                temp = activeLabel[childIndex][j].depth;
                activeLabel[childIndex][j].depth = temp + 100;
            }
        }
       
    }

   
    /// <summary>
    /// 获取当前激活页签号
    /// </summary>
    /// <returns></returns>
    public int GetCurrentIdx()
    {
        return currentIndex;
    }
    
    /// <summary>
    /// 设置当前选中
    /// </summary>
    /// <param name="value"></param>
    public void SetCurrentIndex(int value)
    {
        int index = 0;
        index = Mathf.Min(value, children.Length - 1);
        index = Mathf.Max(index, 0);

        if (children != null && children.Length > index)
        {
            onTouch(children[index]);
        }
    }

    /// <summary>
    /// 选中按扭
    /// </summary>
    /// <param name="go"></param>
    public void SelectBtn(GameObject go)
    {
        onTouch(go);
    }

    /// <summary>
    /// 点击页签响应事件
    /// </summary>
    /// <param name="go"></param>
    void onTouch(GameObject go)
    {
        if (children == null || children.Length < 1)
            return;
        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            if (children[i].name == go.name)
            {
                if (repeatTouchEvent != null && repeatTouchEvent(i, children[i]))
                {
                    if (i != currentIndex)
                    {
                        Active(i);
                        //添加播放声音入口
                        DelegateProxy.PlayUIAudio((int)UIPlaySound.SoundMode.FunctionSwitch);
                    }

                    return;
                }
                    

                if (i == currentIndex)
                    return;
                if (touchEvent != null && touchEvent(i, children[i]))
                {
                    Active(i);
                    //添加播放声音入口
                    DelegateProxy.PlayUIAudio((int)UIPlaySound.SoundMode.FunctionSwitch);
                }
                break;
            }
        }
    }

    void Normal()
    {
        int len = children.Length;
        for (int i = 0; i < len; i++)
        {
            //sprites[i].spriteName = normalSprite;
            tabs[i].Normal(spritesName[i].normal, LabNormalColor, LabNormalSize, LabNormalX, OutlineNorColor,SprNormalX, SpriteNormalSize,RedNorCoord,RedNorSize);
        }
    }

    void Active()
    {
        //sprites[0].spriteName = pressSprite;
        //BackScale(scale,true);
        tabs[0].Active(spritesName[0].hover, LabPressColor, LabPressSize, LabPressX, OutlinePreColor, SprPressX,SpritePressSize,RedPreCoord,RedPreSize);
    }

    public void Active(int index)
    {
        if (currentIndex == index)
            return;
        perviousIndex = currentIndex;
        currentIndex = index;
        if (currentIndex >= 0 && currentIndex < children.Length && children != null)
            tabs[currentIndex].Active(spritesName[currentIndex].hover, LabPressColor, LabPressSize, LabPressX, OutlinePreColor, SprPressX, SpritePressSize, RedPreCoord, RedPreSize);

        if (perviousIndex >= 0 && perviousIndex < children.Length && children != null)
            tabs[perviousIndex].Normal(spritesName[perviousIndex].normal, LabNormalColor, LabNormalSize, LabNormalX , OutlineNorColor, SprNormalX, SpriteNormalSize, RedNorCoord, RedNorSize);
    }

    public GameObject GetTabGOByIndex(int index)
    {
        GameObject go = null;
        if(children != null && index < children.Length)
        {
            go = children[index];
        }
        return go;
    }

    void OnDestroy()
    {
        if (touchEvent != null)
            touchEvent = null;

        if (repeatTouchEvent != null)
            repeatTouchEvent = null;
    }

    /// <summary>
    /// 页签单元
    /// </summary>
    public struct TAB
    {
        public UISprite back;
        public List<UISprite> auxSprite;
        public UILabel lbl;
        public UISprite tabLogo;
        

        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="spriteName"></param>
        /// <param name="pressColor"></param>
        /// <param name="lblClr"></param>
        /// <param name="result"></param>
        public void Active(string spriteName, Color lblClr, int pressSize, int pressXOff ,Color outlineColor , int spriteXOff ,Vector2 spriteSize, Vector2 redCoord, int redSize)
        {
            if (back != null)
            {
                back.spriteName = spriteName;

                if (spriteSize.x != 0 && spriteSize.y != 0)
                    back.SetDimensions((int)spriteSize.x, (int)spriteSize.y);

                if (spriteXOff != 0)
                {
                    Vector3 tmp = back.transform.localPosition;
                    tmp.x = spriteXOff;
                    back.transform.localPosition = tmp;
                }
            }


            if (tabLogo != null)
            {
                if (redCoord != Vector2.zero)
                {
                    Vector3 tmp = tabLogo.transform.localPosition;
                    tmp.x = redCoord.x;
                    tmp.y = redCoord.y;
                    tabLogo.transform.localPosition = tmp;
                }

                if (redSize != 0)
                {
                    tabLogo.SetDimensions(redSize, redSize);
                }
            }

            if (lbl != null)
            {
                if (outlineColor != Color.white)
                {
                    lbl.effectStyle = UILabel.Effect.Outline;
                    lbl.effectColor = outlineColor;
                }
                else
                {
                    lbl.effectStyle = UILabel.Effect.None;
                }

                if (lbl)
                {
                    lbl.color = lblClr;
                    if (pressSize != 0)
                        lbl.fontSize = pressSize;

                    Vector3 normalPos = lbl.transform.localPosition;
                    normalPos.x = pressXOff;
                    lbl.transform.localPosition = normalPos;
                }
            }
        }

        /// <summary>
        /// 正常
        /// </summary>
        /// <param name="spriteName"></param>
        /// <param name="lblClr"></param>
        /// <param name="result"></param>
        public void Normal(string spriteName, Color lblClr, int normalSize, int normalXOff, Color outlineColor ,int spriteXOff,Vector2 spriteSize ,Vector2 redCoord,int redSize)
        {
            if (back != null)
            {
                back.spriteName = spriteName;

                if (spriteSize.x != 0 && spriteSize.y != 0)
                    back.SetDimensions((int)spriteSize.x, (int)spriteSize.y);

                if (spriteXOff != 0)
                {
                    Vector3 tmp = back.transform.localPosition;
                    tmp.x = spriteXOff;
                    back.transform.localPosition = tmp;
                }

            }

            if (tabLogo != null)
            {
                if (redCoord != Vector2.zero)
                {
                    Vector3 tmp = tabLogo.transform.localPosition;
                    tmp.x = redCoord.x;
                    tmp.y = redCoord.y;
                    tabLogo.transform.localPosition = tmp;
                }

                if (redSize != 0)
                {
                    tabLogo.SetDimensions(redSize, redSize);
                }  
            }

            
            if (auxSprite != null)
            {
                int len = auxSprite.Count;
                for (int i = 0; i < len; i++)
                {
                    auxSprite[i].color = Color.white;
                }
            }
            if (lbl)
            {
                lbl.color = lblClr;
                if (normalSize != 0)
                    lbl.fontSize = normalSize;

                if (outlineColor != Color.white)
                {
                    lbl.effectStyle = UILabel.Effect.Outline;
                    lbl.effectColor = outlineColor;
                }
                else
                {
                    lbl.effectStyle = UILabel.Effect.None;
                }

                Vector3 normalPos = lbl.transform.localPosition;
                normalPos.x = normalXOff;
                lbl.transform.localPosition = normalPos;
            }

        }
    }

    /// <summary>
    /// sprite状态值
    /// </summary>
    [System.Serializable]
    public class Sprite
    {
        public string normal;
        public string hover;
    }
}
