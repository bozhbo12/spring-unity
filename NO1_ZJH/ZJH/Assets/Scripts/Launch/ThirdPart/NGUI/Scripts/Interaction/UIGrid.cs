//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// All children added to the game object with this script will be repositioned to be on a grid of specified dimensions.
/// If you want the cells to automatically set their scale based on the dimensions of their content, take a look at UITable.
/// </summary>

[AddComponentMenu("NGUI/Interaction/Grid")]
public class UIGrid : UIWidgetContainer
{
    public delegate void OnReposition();

    public enum Arrangement
    {
        Horizontal,
        Vertical,
    }

    public enum Sorting
    {
        None,
        Alphabetic,
        Horizontal,
        Vertical,
        Custom,
    }

    /// <summary>
    /// Type of arrangement -- vertical or horizontal.
    /// </summary>

    public Arrangement arrangement = Arrangement.Vertical;

    /// <summary>
    /// How to sort the grid's elements.
    /// </summary>

    public Sorting sorting = Sorting.None;

    /// <summary>
    /// Final pivot point for the grid's content.
    /// </summary>

    public UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft;

    /// <summary>
    /// Maximum children per line.
    /// If the arrangement is horizontal, this denotes the number of columns.
    /// If the arrangement is vertical, this stands for the number of rows.
    /// </summary>

    public int maxPerLine = 1;

    /// <summary>
    /// The width of each of the cells.
    /// </summary>

    public float cellWidth = 200f;

    /// <summary>
    /// The height of each of the cells.
    /// </summary>

    public float cellHeight = 200f;

    /// <summary>
    /// Whether the grid will smoothly animate its children into the correct place.
    /// </summary>

    public bool animateSmoothly = false;

    /// <summary>
    /// Whether to ignore the disabled children or to treat them as being present.
    /// </summary>

    public bool hideInactive = true;

    /// <summary>
    /// Whether the parent container will be notified of the grid's changes.
    /// </summary>

    public bool keepWithinPanel = false;

    /// <summary>
    /// Callback triggered when the grid repositions its contents.
    /// </summary>

    public OnReposition onReposition;

    // Use the 'sorting' property instead
    [HideInInspector]
    [SerializeField]
    bool sorted = false;

    protected bool mReposition = false;
    protected UIPanel mPanel;
    protected bool mInitDone = false;
    /// <summary>
    /// 是否是自定义grid
    /// </summary>
    public bool mbCustomGrid = false;

    /// <summary>
    /// grid子对象显示列表
    /// </summary>
    public List<Transform> mTransSource = new List<Transform>();
    /// <summary>
    /// grid数据对象列表
    /// </summary>
    public List<object> mDataSource = new List<object>();
    public List<Transform> mFreeTrans = new List<Transform>();
    public Dictionary<object, Transform> mDataToTrans = new Dictionary<object, Transform>();
    /// <summary>
    /// grid实例化的当行对象
    /// </summary>
    public GameObject mgridItem;
    public string mstrGridItemHash;
    /// <summary>
    /// 当前选择的grid对象
    /// </summary>
    public GameObject mSelectItem;
    /// <summary>
    /// grid更新代理
    /// </summary>
    /// <param name="item">当前更新的griditem</param>
    public delegate void OnUpdateDataRow(UIGridItem item);
    /// <summary>
    /// 更新代理函数对象
    /// </summary>
    OnUpdateDataRow mfnOnChangeRow;
    /// <summary>
    /// grid面板y的偏移量
    /// </summary>
    private Vector3 mPanelPos;
    private Vector3 mPanelInitPos;
    private Vector2 mPanelInitOffest;
    private int mOffsetRows = 0;

	private const string EffectTime = "EffectTime";

	/// <summary>
	/// 打开刷新列表特效类型
	/// </summary>
	public EffectState effectState = EffectState.None;
	public enum EffectState
	{
		None,//空
		Once,//一次
		Always,//一直
	}

	/// <summary>
	/// 重复更新状态
	/// </summary>
	public enum RepeatState
	{
		None,
		Repeat,//重置
		Break,//打断
	}

    /// <summary>
    /// 移动方法
    /// </summary>
    public UITweener.Method mTweenMethod = UITweener.Method.EaseOut;

	//效果结束之前不允许再次点击
	private bool effectOver = true;

	/// <summary>
	/// 列表显示效果刷新时间
	/// </summary>
	public float EffectRefreshTime = 0.2f;

    /// <summary>
    /// 列表间隔时间
    /// </summary>
    public float mItemIntervalTime = 0.2f;

	/// <summary>
	/// 特效显示参数
	/// </summary>
	public float RefreshOffset = -50;

    public int OffestRow
    {
        get { return mOffsetRows; }
    }

    /// <summary>
    /// 绑定更新代理函数
    /// </summary>
    /// <param name="fn_ChangeRow">更新代里对象</param>
    public void BindCustomCallBack(OnUpdateDataRow fn_ChangeRow)
    {
        mfnOnChangeRow = fn_ChangeRow;
        mbCustomGrid = true;
    }

    /// <summary>
    /// 子对象被点击时回调给用户程序处理
    /// </summary>
    /// <param name="oItem"> 一行包含数据</param>
    /// <param name="OSubItem">点击的对象</param>
    public delegate void OnClickSubItem(UIGridItem oItem, GameObject OSubItem);
    public OnClickSubItem fnonClickSubItem;

	/// <summary>
	/// grid初使化完成回调
	/// </summary>
	public static UserDelegate OnGridInitComplete = new UserDelegate();
    /// <summary>
    /// 点击视窗列回调
    /// </summary>
    /// <param name="oItem">数据节点</param>
    /// <param name="OSubItem">点击的对象</param>
    public void OnClickItem(UIGridItem oItem, GameObject OSubItem)
    {
        if (fnonClickSubItem != null)
        {
            fnonClickSubItem(oItem, OSubItem);
        }
    }

    /// <summary>
    /// 清空整个数据或显示
    /// </summary>
    public void ClearCustomGrid()
    {
        //ClearAllData();
        ClearCustomData();
    }

    /// <summary>
    /// 清空自定义数据区
    /// </summary>
    public void ClearCustomData()
    {
        mOffsetRows = 0;
        miSelectIndex = -1;
        mSelectItem = null;
        PanelReset();
        int iIndex = mFreeTrans.Count;
        mFreeTrans.InsertRange(mFreeTrans.Count, mDataToTrans.Values);
        for (int i = iIndex; i < mFreeTrans.Count; i++)
        {
            Transform trans = mFreeTrans[i];
            if (trans != null)
            {
                trans.gameObject.SetActive(false);
            }
        }
        mDataToTrans.Clear();
        mDataSource.Clear();
		effectOver = true;
    }

    /// <summary>
    /// 清除所有数据，包括缓存的节点组件
    /// </summary>
    public void ClearAllData()
    {
        ClearCustomData();
        for (int i = 0; i < mFreeTrans.Count; i++)
        {
            Transform trans = mFreeTrans[i];
            if (trans != null)
            {
                CacheObjects.DestroyClone(trans.gameObject);
                trans = null;
            }
        }
        mFreeTrans.Clear();
    }

    /// <summary>
    /// 删除用户数据
    /// 修改者：zhangrj
    /// 修改日期20141218
    /// 源码为注释部分
    /// </summary>
    /// <param name="oData"></param>
    public void DeleteCustomData(object oData)
    {
        if (oData == null)
            return;

        for (int i = 0; i < mDataSource.Count; i++)
        {
            object oSrcData = mDataSource[i];
            if (oSrcData == oData)
            {
                mDataSource.RemoveAt(i);
                if (mDataToTrans.ContainsKey(oData))
                {
                    Transform trans = mDataToTrans[oData];
                    mDataToTrans.Remove(oData);
                    if (trans != null)
                    {
                        trans.gameObject.SetActive(false);
                        mFreeTrans.Add(trans);
                        UpdateCustomView();
                    }
                }
                break;
            }
        }
    }
    /// <summary>
    /// 更新单个结点
    /// </summary>
    /// <param name="odata"></param>
    public void UpdateCustomData(object odata)
    {
        if (odata == null)
            return;

        for (int i = 0; i < mDataSource.Count; i++)
        {
            object oSrcData = mDataSource[i];
            if (oSrcData == odata)
            {
                mDataSource[i] = odata;
                if (mDataToTrans.ContainsKey(odata))
                {
                    Transform trans = mDataToTrans[odata];
                    if (trans != null)
                    {
                        UIGridItem griditem = trans.gameObject.GetComponent<UIGridItem>();
                        if (griditem != null)
                        {
                            griditem.setIndex(i);
                            griditem.onClick = OnClick;
                            griditem.oData = odata;
                            mfnOnChangeRow(griditem);
                        }
                    }
                }
                break;
            }
        }
    }
  
    /// <summary>
    /// 只更新数据，不会添加数据
    /// </summary>
    /// <param name="list"></param>
	public void UpdateCustomDataList(List<object> list,RepeatState state = RepeatState.None)
    {
		UpdateCustomDataListAll(list,state);
    }
    /// <summary>
    /// 更新客户整体数据，可以添加，或者删除
    /// </summary>
    /// <param name="list"></param>
	public void UpdateCustomDataListAll(List<object> list,RepeatState state = RepeatState.None)
    {
		if(state == RepeatState.Repeat)
		{
			effectOver = true;
		}
		else if(state == RepeatState.Break)
		{
			effectOver = true;
			if (effectState == EffectState.Once)
				effectState = EffectState.None;
			TimerManager.Destroy (EffectTime);
		}	
        int iListCount = list.Count;
        int iDataCount = mDataSource.Count;
        int iMax = Mathf.Max(iListCount, iDataCount);
        bool remove = false;
        for (int i = 0; i < iMax; i++)
        {
            if (i < iListCount)
            {
                ///有数据添加
                if (i >= iDataCount)
                {
                    ///历史数据不足装下，新加节点
                    AddCustomData(list[i]);
                }
                else
                {
                    if (mDataSource[i] == list[i])
                        continue;

                   object oData = mDataSource[i];
                   if (oData == null)
                       continue;

                   if (mDataToTrans.ContainsKey(oData))
                   {
                       Transform trans = mDataToTrans[oData];
                       mDataToTrans.Remove(oData);
                       if (trans != null)
                       {
                           trans.gameObject.SetActive(false);
                           mFreeTrans.Add(trans);
                       }
                   }
                   ///重设数据
                   mDataSource[i] = list[i];
                }
            }
            else
            {
                ///数据减少了，去除多余的数据
                if (i < iDataCount)
                {
                    remove = true;
                    //object oData = mDataSource[i];
                    //if (mDataToTrans.ContainsKey(oData))
                    //{
                    //    Transform trans = mDataToTrans[oData];
                    //    mDataToTrans.Remove(oData);
                    //    if (trans != null)
                    //    {
                    //        trans.gameObject.SetActive(false);
                    //        mFreeTrans.Add(trans);
                    //    }
                    //}
                    //mDataSource.RemoveAt(i);
                    //i--;
                }
            }
        }
        if (remove)
        {
            int len = iDataCount - iListCount;
            for (int i = 0; i < len; ++i)
            {
                object oData = mDataSource[iListCount];
                if (oData == null)
                    continue;

                if (mDataToTrans.ContainsKey(oData))
                {
                    Transform trans = mDataToTrans[oData];
                    mDataToTrans.Remove(oData);
                    if (trans != null)
                    {
                        trans.gameObject.SetActive(false);
                        mFreeTrans.Add(trans);
                    }
                }
                mDataSource.RemoveAt(iListCount);
            }
        }
		UpdateCustomView();
    }
    /// <summary>
    /// 清空数据重新更新
    /// </summary>
    /// <param name="list">数据列表</param>
	public void AddCustomDataList(List<object> list)
    {
        ClearCustomData();
		UpdateCustomDataListAll(list);
    }
    //添加一行数据
    public void AddCustomData(object oData)
    {
        mDataSource.Add(oData);
    }
    
    ///检查，第一个与最后一个是否在panel.y+height之内
    void OnCustomDrag()
    {
        int iMaxRows = 0;
        int iTotalRows = 0;
        int iOffset = 0;
        if (arrangement == Arrangement.Vertical)
        {
            ///不满足一页，不能做动态拖动处理
            iMaxRows = (int)(mPanel.height / cellHeight)+2;
            iTotalRows = Mathf.CeilToInt((float)mDataSource.Count / (float)maxPerLine);
            if (mDataSource.Count < maxPerLine * iMaxRows)
                return;

            iOffset = (int)((mPanel.transform.localPosition.y - mPanelPos.y) / cellHeight);
            if (iOffset > iTotalRows - iMaxRows)
            {
                iOffset = iTotalRows - iMaxRows;
            }
            if (iOffset < 0)
            {
                iOffset = 0;
            }
        }
        else if (arrangement == Arrangement.Horizontal)
        {
            iMaxRows = Mathf.FloorToInt(mPanel.width / cellWidth)+2;
            iTotalRows = Mathf.CeilToInt((float)mDataSource.Count / (float)maxPerLine);

            if (mDataSource.Count < maxPerLine * iMaxRows)
                return;

            iOffset = (int)((mPanelPos.x - mPanel.transform.localPosition.x) / cellWidth);
            if (iOffset > iTotalRows - iMaxRows)
            {
                iOffset = iTotalRows - iMaxRows;
            }
            if (iOffset < 0)
            {
                iOffset = 0;
            }
        }

        if (mOffsetRows == iOffset)
        {
            return;
        }

        mOffsetRows = iOffset;
        UpdateCustomView(false);
    }

    /// <summary>
    /// 获取或者生成空闲的显示单元
    /// </summary>
    /// <returns></returns>
    private Transform GetFreeTrans()
    {
        if (mFreeTrans.Count > 0)
        {
            Transform trans = mFreeTrans[0];
             mFreeTrans.RemoveAt(0);
             return trans;
        }
        ///实例化一个对象
        GameObject item = Instantiate(mgridItem) as GameObject;
        if (item != null)
        {
            item.transform.parent = transform;
            item.transform.localPosition = Vector3.zero;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector3.one;
            return item.transform;
        }

        return null;
    }

    ///更新Grid排列
	public void UpdateCustomView(bool effectPlayer = true)
    {
		if(!effectOver)
			return;
		if(effectState != EffectState.None)
			effectOver = false;
		else
			effectOver = true;

        int x = 0;
        int y = 0;
        int iPageMax = 0;
        if (arrangement == Arrangement.Vertical)
        {
            iPageMax = (int)(mPanel.height / cellHeight) + 2;
        }
        else if (arrangement == Arrangement.Horizontal)
        {
            iPageMax = Mathf.FloorToInt(mPanel.width / cellWidth) + 2;
        }
       	int iStartIndex = mOffsetRows * maxPerLine;
       	int iEndIndex = iStartIndex + iPageMax * maxPerLine;
        ///1,去除不显示的数据关联
        ///2,添加待显示的关联信息
		QEffectList.Clear();
		for (int i = 0; i < mDataSource.Count; i++)
        {
            object oData = mDataSource[i];
            if ( i < iStartIndex || i > iEndIndex )
            {
                if( mDataToTrans.ContainsKey(oData))
                {
                    Transform trans = mDataToTrans[oData];
                    if (trans != null)
                    {
                        trans.gameObject.SetActive(false);
                        mFreeTrans.Add(trans);
                    }
                   
                    mDataToTrans.Remove(oData);
                }
                if (SelectItem != null)
                {
                    if (miSelectIndex == i)
                    {
                        SelectItem(miSelectIndex, false, false);
                    }
                }
                //continue;
            }
            else
            {
                int iIndex = i - iStartIndex;
                Transform trans = null;
                if( mDataToTrans.ContainsKey(oData))
                {
                    trans = mDataToTrans[oData];
                }
                else 
                {
                    trans = GetFreeTrans();
                    ///早点出错，早点检查
                    if (trans == null)
                        return;

                    mDataToTrans.Add(oData, trans);
                }

                trans.name = GetItemString(i);

                float depth = trans.localPosition.z;
                Vector3 pos = Vector3.zero;
                if (arrangement == Arrangement.Vertical)
                {
                    pos.x = cellWidth * x;
                    pos.y = -cellHeight * y;
                    pos.z = depth;
                }
                else
                {
                    pos.x = cellWidth * y;
                    pos.y = -cellHeight * x;
                    pos.z = depth;
                }
               
                trans.localPosition = pos;
                if (!trans.gameObject.activeSelf)
                {
                    trans.gameObject.SetActive(true);
                }

				if (effectState != EffectState.None && effectPlayer)
				{
					trans.gameObject.SetActive (false);
					QEffectList.Enqueue(trans.gameObject);
				}
				else
					trans.gameObject.SetActive (true);
				
                UIGridItem griditem = trans.gameObject.GetComponent<UIGridItem>();
                if (griditem != null)
                {
                    griditem.setIndex(i);
                    griditem.onClick = OnClick;
                    griditem.oData = oData;
                    if (mfnOnChangeRow != null)
                        mfnOnChangeRow(griditem);
                }

                if (SelectItem != null)
                {
                    if (miSelectIndex == i)
                    {
                        SelectItem(miSelectIndex, true, false);
                    }
                }
            }

            if (++x >= maxPerLine && maxPerLine > 0)
            {
                x = 0;
                ++y;
            }
        }
        if (effectState != EffectState.None && effectPlayer)
        {
            EffectUpdateView();
        }
        else
        {
            VarStore varStore = VarStore.CreateVarStore();
            varStore += this;
            OnGridInitComplete.ExecuteCalls(varStore);
            varStore.Destroy();
        }
    }

    private static string[] strItems = new string[]
    {
       "Item0","Item1","Item2","Item3","Item4","Item5","Item6","Item7","Item8","Item9",
       "Item10","Item11","Item12","Item13","Item14","Item15","Item16","Item17","Item18","Item19",
       "Item20","Item21","Item22","Item23","Item24","Item25","Item26","Item27","Item28","Item29",
       "Item30","Item31","Item32","Item33","Item34","Item35","Item36","Item37","Item38","Item39",
       "Item40","Item41","Item42","Item43","Item44","Item45","Item46","Item47","Item48","Item49",
       "Item50","Item51","Item52","Item53","Item54","Item55","Item56","Item57","Item58","Item59",
       "Item60","Item61","Item62","Item63","Item64","Item65","Item66","Item67","Item68","Item69",
       "Item70","Item71","Item72","Item73","Item74","Item75","Item76","Item77","Item78","Item79",
       "Item80","Item81","Item82","Item83","Item84","Item85","Item86","Item87","Item88","Item89",
       "Item90","Item91","Item92","Item93","Item94","Item95","Item96","Item97","Item98","Item99",
    };
    static string GetItemString(int iIndex)
    {
        if (iIndex < 0 || iIndex > 99)
        {
            return DelegateProxy.StringBuilder("Item", iIndex);
        }

        return strItems[iIndex];
    } 
   
	/// <summary>
	/// 等待播放效果的组件
	/// </summary>
	private Queue<GameObject> QEffectList = new Queue<GameObject> ();
	/// <summary>
	/// 增加播放效果计时器
	/// </summary>
	/// <param name="Effect">If set to <c>true</c> effect.</param>
	private void EffectUpdateView(bool Effect = false)
	{
		UIScrollView view = NGUITools.FindInParents<UIScrollView>(gameObject);
		if (view != null)
			view.enabled = false;
		EffectUpdateViewOver ();
        TimerManager.AddTimerRepeat(EffectTime, mItemIntervalTime, EffectUpdateViewOver);
	}

	/// <summary>
	/// 开始执行效果
	/// </summary>
	private void EffectUpdateViewOver()
	{
		Vector3 pos = Vector3.zero;
        for (int i= 0;i< maxPerLine;i++)
        {
            if (QEffectList.Count <= 0)
            {
				TimerManager.Destroy(EffectTime);
                return;
            }

            GameObject obj = QEffectList.Dequeue();

            Vector3 fromPos = Vector3.zero;
			if (arrangement == Arrangement.Vertical)
			{
				pos.x = 0;
				pos.y = RefreshOffset;
				pos.z = 0;
				fromPos = obj.transform.localPosition + pos;
			} 
			else
			{
				pos.x = RefreshOffset;
				pos.y = 0;
				pos.z = 0;
				fromPos = obj.transform.localPosition + pos;
			}

            Vector3 toPos = obj.transform.localPosition;
            obj.transform.localPosition = fromPos;
            obj.gameObject.SetActive(true);
			TweenPosition tp = TweenPosition.Begin(obj, EffectRefreshTime, fromPos, toPos);
            tp.method = mTweenMethod;
			if(QEffectList.Count <= 0)
			{
				EventDelegate.Add(tp.onFinished,OnFinished, true);
			}

        }
	}
	/// <summary>
	/// 特效播放结束
	/// </summary>
	private void OnFinished()
	{
		if (effectState == EffectState.Once)
			effectState = EffectState.None;
		effectOver = true;
		UIScrollView view = NGUITools.FindInParents<UIScrollView>(gameObject);
		if (view != null)
			view.enabled = true;

        VarStore varStore = VarStore.CreateVarStore();
        varStore += this;
        OnGridInitComplete.ExecuteCalls(varStore);
        varStore.Destroy();
	}
    public bool repositionNow { set { if (value) { mReposition = true; enabled = true; } } }

	/// <summary>
	/// 延迟goto
	/// </summary>
	public void DelayGoTo(params object[] args)
	{
		if(args.Length == 0)
			return;
		int index  = (int)args[0];
		if (index < 0 || index >= mDataSource.Count)
			return;

		int actualIndex = index / maxPerLine;

		if (arrangement == Arrangement.Vertical)
		{
			float h = cellHeight * actualIndex;
			Vector3 vTemp = Vector3.zero;
			vTemp.x = mPanelInitPos.x;
			vTemp.y = mPanelInitPos.y + h;
			vTemp.z = mPanelInitPos.z;
			mPanel.transform.localPosition = vTemp;
			vTemp.x = mPanelInitOffest.x;
			vTemp.y = mPanelInitOffest.y - h;
			vTemp.z = 0;
			mPanel.clipOffset = vTemp;
		}
		else if (arrangement == Arrangement.Horizontal)
		{
			float w = cellWidth * actualIndex;
			Vector3 vTemp = Vector3.zero;
			vTemp.x = mPanelInitPos.x - w;
			vTemp.y = mPanelInitPos.y;
			vTemp.z = mPanelInitPos.z;
			mPanel.transform.localPosition = vTemp;

			vTemp.x = mPanelInitOffest.x + w;
			vTemp.y = mPanelInitOffest.y;
			vTemp.z = 0;
			mPanel.clipOffset = vTemp;
		}

		OnCustomDrag();
		UIScrollView view = NGUITools.FindInParents<UIScrollView>(gameObject);
		view.DisableSpring();
		view.UpdateScrollbars(true);
		view.RestrictWithinBounds(false, view.canMoveHorizontally, view.canMoveVertically);
	}

    /// <summary>
    /// 定位
    /// </summary>
    /// <param name="index"></param>
    public void GoToPosition(int index,float delayTime = 0)
    {
		TimerManager.AddDelayTimer ("GoToPosition",delayTime,DelayGoTo,index);
    }

    ///取选中序号
    public int GetSelectedIndex()
    {
        return miSelectIndex;
    }
    /// <summary>
    /// 设置选中
    /// </summary>
    /// <param name="iSelectIndex"></param>
    public void SetSelect(int iSelectIndex)
    {
        if (iSelectIndex < 0 || iSelectIndex >= mDataSource.Count)
            return;

        object oData = mDataSource[iSelectIndex];

        SetSelect(oData);
    }

    /// <summary>
    /// 设置选中
    /// </summary>
    public void SetSelect(object oData)
    {
        if (oData == null) return;

        for (int i = 0; i < mDataSource.Count; i++)
        {
            if (mDataSource[i] != null && oData == mDataSource[i])
            {
                if (mSelectItem != null)
                {
                    if (SelectItem != null)
                        SelectItem(miSelectIndex, false, true);
                }
                if (mDataToTrans.ContainsKey(oData))
                {
                    Transform tran = mDataToTrans[oData];
                    if (tran != null)
                    {
                        mSelectItem = tran.gameObject;
                    }
                }
                miSelectIndex = i;
                if (SelectItem != null)
                    SelectItem(miSelectIndex, true, true);
                break;
            }
        }
    }
    ///选中对象数据单元
    public object GetSelected()
    {
        if (miSelectIndex < 0 || miSelectIndex >= mDataSource.Count)
            return null;
        return mDataSource[miSelectIndex];
    }

    /// <summary>
    /// 获取对象
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public GameObject GetSelected(int index)
    {
        if (index < 0 || index >= mDataSource.Count)
            return null;
        object oData = mDataSource[index];
        if (oData == null) return null;
        if (mDataToTrans.ContainsKey(oData))
        {
            Transform tran = mDataToTrans[oData];
            if (tran != null)
            {
                return tran.gameObject;
            }
        }
        return null;
    }

    public Transform GetSelectedItem()
    {
        if (mSelectItem == null)
            return null;

        return mSelectItem.transform;
    }
    /// <summary>
    /// 获得UIGrid 的当前选中UIGridItem
    /// </summary>
    /// <returns></returns>
    public UIGridItem GetSelectedGridItem()
    {
        if (miSelectIndex < 0 || miSelectIndex >= mDataSource.Count)
            return null;

        UIGridItem item = null;
        if( mSelectItem != null )
        {
            item = mSelectItem.GetComponent<UIGridItem>();
        }
     
        return item;
    }
    /// <summary>
    /// 获取指向序号的用户数据单元
    /// </summary>
    /// <param name="iIndex">第几个</param>
    /// <returns>数据单元</returns>
    public object GetCustomDataItem(int iIndex)
    {
        if (iIndex < 0 || iIndex >= mDataSource.Count || mDataSource.Count == 0)
            return null;

        object oData = mDataSource[iIndex];
        return oData;
    }
    /// <summary>
    /// 获取指向序号的用户显示单元
    /// </summary>
    /// <param name="iIndex">第几个</param>
    /// <returns>显示单元</returns>
    public Transform GetCustomTransItem(int iIndex)
    {
        if (iIndex < 0 || iIndex > mDataSource.Count || mDataSource.Count == 0)
            return null;

        object oData = mDataSource[iIndex];
        //获取指向序号的UIGridITem
        if (mDataToTrans.ContainsKey(oData))
        {
            return mDataToTrans[oData];
        }

        return null;
    }
    /// <summary>
    /// 获取指向序号的用户显示控件单元
    /// </summary>
    /// <param name="iIndex">第几个</param>
    /// <returns>控件单元</returns>
    public UIGridItem GetCustomGridItem(int iIndex)
    {
        if( iIndex< 0 || iIndex> mDataSource.Count || mDataSource.Count == 0)
            return null;
        UIGridItem item = null;
        object oData = mDataSource[iIndex];
        //获取指向序号的UIGridITem
        if (mDataToTrans.ContainsKey(oData))
        {
            Transform trans = mDataToTrans[oData];
            if( trans != null )
            {
                item = trans.GetComponent<UIGridItem>();
            }
            return item;
        }

        return item;
    }

    /// <summary>
    /// 根据数据查询UIGridItem,可外部实现逻辑比较
    /// </summary>
    /// <param name="oData"></param>
    /// <returns></returns>
    public UIGridItem GetCustomGridItem(object oData)
    {
        if (oData == null) return null;

        UIGridItem item = null;

        for (int i = 0; i < mDataSource.Count; i++)
        {
            if (mDataSource[i] != null && mDataSource[i].Equals(oData))
            {
                if (mDataToTrans.ContainsKey(mDataSource[i]))
                {
                    Transform tran = mDataToTrans[mDataSource[i]];
                    if (tran != null)
                    {
                        item = tran.GetComponent<UIGridItem>();
                        break;
                    }
                }
            }
        }

        return item;
    }


    /// <summary>
    /// 选中
    /// </summary>
    /// <param name="iSelect">选中索引</param>
    /// <param name="active">状态</param>
    /// <param name="select">是否主动选中</param>
    public delegate void OnSelectEvent(int iSelect, bool active, bool select);
    public OnSelectEvent SelectItem;
    public int miSelectIndex = -1;
    /// <summary>
    /// 点击事件
    /// </summary>
    /// <param name="item"></param>
    void OnClick(UIGridItem item)
    {
        if (item == null) return;

        if (mSelectItem != null && miSelectIndex>=0)
        {
            if (SelectItem != null)
                SelectItem(miSelectIndex, false, true);
        }

        miSelectIndex = item.GetIndex();
        mSelectItem = item.gameObject;
        if (SelectItem != null && miSelectIndex >= 0)
        {
            if (SelectItem != null)
                SelectItem(miSelectIndex, true, true);
        }
        
        //UpdateCustomView();
    }
    void OnSubmit(UIGridItem item)
    {
    }
    void OnDoubleClick(UIGridItem item)
    {
    }
    void OnHover(UIGridItem item, bool isOver)
    {
    }
    void OnPress(UIGridItem item, bool isPressed)
    {
    }
    void OnSelect(UIGridItem item, bool selected)
    {
    }
    void OnScroll(float delta)
    {
    }
    void OnDrag(UIGridItem item, Vector2 delta)
    {
    }
    void OnDrop(UIGridItem item, GameObject go)
    {
    }
    void OnKey(UIGridItem item, KeyCode key)
    {
    }

    protected virtual void Init()
    {
        mInitDone = true;
        mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
        mPanelInitPos = mPanel.transform.localPosition;
        mPanelInitOffest = mPanel.clipOffset;
        if (maxPerLine < 1) maxPerLine = 1;
    }

    protected virtual void Start()
    {
        if (!mInitDone) Init();
        mPanelPos = mPanel.transform.localPosition;
        if (mgridItem != null)
        {
            mgridItem.SetActive(false);
        }
        bool smooth = animateSmoothly;
        animateSmoothly = false;
        Reposition();
        
        animateSmoothly = smooth;
        enabled = mbCustomGrid ? true : false;
    }

    /// <summary>
    /// 开启自定义模式
    /// </summary>
    public void StartCustom()
    {
		ClearAllData();
//		mInitDone = false;
        if (!mInitDone) Init();
		//做一层清理保护

        bool smooth = animateSmoothly;
        animateSmoothly = false;
        PanelReset();
        Reposition();

        mPanelPos = mPanel.transform.localPosition;

        animateSmoothly = smooth;
        enabled = mbCustomGrid ? true : false;
		effectOver = true;
    }

    /// <summary>
    /// 预制件重置
    /// </summary>
    private void PanelReset()
    {
        if (gameObject == null)
            return;

        UIScrollView view = NGUITools.FindInParents<UIScrollView>(gameObject);
        if (SafeHelper.IsObjectNull(view))
            return;
        //禁用SpringPanel
        view.DisableSpring();

        if (mPanel != null)
        {
            mPanel.transform.localPosition = mPanelInitPos;
            mPanel.clipOffset = mPanelInitOffest;
        }
        view.DisableSpring();
        //刷新滚动条
        view.UpdateScrollbars(true);
        
    }

    protected virtual void Update()
    {
        if (mbCustomGrid)
        {
            OnCustomDrag();
        }
        else
        {
            if (mReposition)
                Reposition();
        }

        enabled = mbCustomGrid ? true : false;
    }

    static public int SortByName(Transform a, Transform b) { return string.Compare(a.name, b.name); }
    static public int SortHorizontal(Transform a, Transform b) { return a.localPosition.x.CompareTo(b.localPosition.x); }
    static public int SortVertical(Transform a, Transform b) { return b.localPosition.y.CompareTo(a.localPosition.y); }

    /// <summary>
    /// Want your own custom sorting logic? Override this function.
    /// </summary>

    protected virtual void Sort(BetterList<Transform> list) { list.Sort(SortByName); }

    /// <summary>
    /// Recalculate the position of all elements within the grid, sorting them alphabetically if necessary.
    /// </summary>
    BetterList<Transform> list = new BetterList<Transform>();

    [ContextMenu("Execute")]
    public virtual void Reposition()
    {
        if (Application.isPlaying && !mInitDone && NGUITools.GetActive(this))
        {
            mReposition = true;
            return;
        }

        if (!mInitDone) Init();

        mReposition = false;
        Transform myTrans = transform;

        int x = 0;
        int y = 0;
        int maxX = 0;
        int maxY = 0;

        if (sorting != Sorting.None || sorted)
        {
            list.Clear();
            for (int i = 0; i < myTrans.childCount; ++i)
            {
                Transform t = myTrans.GetChild(i);
                if (t && (!hideInactive || NGUITools.GetActive(t.gameObject))) list.Add(t);
            }

            if (sorting == Sorting.Alphabetic) list.Sort(SortByName);
            else if (sorting == Sorting.Horizontal) list.Sort(SortHorizontal);
            else if (sorting == Sorting.Vertical) list.Sort(SortVertical);
            else Sort(list);

            for (int i = 0, imax = list.size; i < imax; ++i)
            {
                Transform t = list[i];

                if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;

                float depth = t.localPosition.z;
                Vector3 pos = Vector3.zero;
                if (arrangement == Arrangement.Vertical)
                {
                    pos.x = cellWidth * x;
                    pos.y = -cellHeight * y;
                    pos.z = depth;
                }
                else
                {
                    pos.x = cellWidth * y;
                    pos.y = -cellHeight * x - cellHeight * y;
                    pos.z = depth;
                }

                if (animateSmoothly && Application.isPlaying)
                {
                    SpringPosition.Begin(t.gameObject, pos, 15f).updateScrollView = true;
                }
                else t.localPosition = pos;

                maxX = Mathf.Max(maxX, x);
                maxY = Mathf.Max(maxY, y);

                if (++x >= maxPerLine && maxPerLine > 0)
                {
                    x = 0;
                    ++y;
                }
            }
        }
        else
        {
            for (int i = 0; i < myTrans.childCount; ++i)
            {
                Transform t = myTrans.GetChild(i);

                if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;

                float depth = t.localPosition.z;
                Vector3 pos = Vector3.zero;
                if (arrangement == Arrangement.Vertical)
                {
                    pos.x = cellWidth * x;
                    pos.y = -cellHeight * y;
                    pos.z = depth;
                }
                else
                {
                    pos.x = cellWidth * y;
                    pos.y = -cellHeight * x;
                    pos.z = depth;
                }
                t.localPosition = pos;

                if (++x >= maxPerLine && maxPerLine > 0)
                {
                    x = 0;
                    ++y;
                }
            }
        }

        // Apply the origin offset
        if (pivot != UIWidget.Pivot.TopLeft)
        {
            Vector2 po = NGUIMath.GetPivotOffset(pivot);

            float fx, fy;

            if (arrangement == Arrangement.Horizontal)
            {
                fx = Mathf.Lerp(0f, maxX * cellWidth, po.x);
                fy = Mathf.Lerp(-maxY * cellHeight, 0f, po.y);
            }
            else
            {
                fx = Mathf.Lerp(0f, maxY * cellWidth, po.x);
                fy = Mathf.Lerp(-maxX * cellHeight, 0f, po.y);
            }

            for (int i = 0; i < myTrans.childCount; ++i)
            {
                Transform t = myTrans.GetChild(i);

                if (!NGUITools.GetActive(t.gameObject) && hideInactive) continue;

                SpringPosition sp = t.GetComponent<SpringPosition>();

                if (sp != null)
                {
                    sp.target.x -= fx;
                    sp.target.y -= fy;
                }
                else
                {
                    Vector3 pos = t.localPosition;
                    pos.x -= fx;
                    pos.y -= fy;
                    t.localPosition = pos;
                }
            }
        }

        if (keepWithinPanel && mPanel != null)
            mPanel.ConstrainTargetToBounds(myTrans, true);

        if (onReposition != null)
            onReposition();
    }

    //
    #region 释放
    //add by chenfei 20150824
    void OnDestroy()
    {
        if( mDataToTrans.Count > 0 )
        {
            mFreeTrans.AddRange(mDataToTrans.Values);
        }
        for( int i = 0; i< mFreeTrans.Count;i++ )
        {
            Transform trans = mFreeTrans[i];
            if( trans != null )
            {
                CacheObjects.DestroyClone(trans.gameObject);
            }
        }
        mFreeTrans.Clear();
        mDataToTrans.Clear();

        if (null != mDataSource)
        {
            mDataSource.Clear();
        }

        mgridItem = null;

        mSelectItem = null;

        mfnOnChangeRow = null;

        fnonClickSubItem = null;

        SelectItem = null;
    }
    #endregion;

}
