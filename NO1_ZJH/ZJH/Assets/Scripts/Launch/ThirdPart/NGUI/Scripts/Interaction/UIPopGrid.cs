using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///<summary>
///作用：UIGrid扩充可以弹出和收起的Grid(目前仅限Vertical)
///作者：shijinhao
///编写日期：2016-05-30
///</summary>

[AddComponentMenu("NGUI/Interaction/PopGrid")]
public class UIPopGrid : UIWidgetContainer
{
    /// <summary>
    /// Final pivot point for the grid's content.
    /// </summary>

    public UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft;

    /// <summary>
    /// Maximum children per line.
    /// If the arrangement is horizontal, this denotes the number of columns.
    /// If the arrangement is vertical, this stands for the number of rows.
    /// </summary>

    public const int maxPerLine = 1;

    /// <summary>
    /// The width of each of the pops. //弹出收起控制单元
    /// </summary>

    public float popHeight = 100f;

    /// <summary>
    /// The height of each of the cells.
    /// </summary>

    public float cellHeight = 100f;

    /// <summary>
    /// pop单元选中后的附加信息高度
    /// </summary>
    public float gapHeight = 50f;
    /// <summary>
    /// grid子对象显示列表
    /// </summary>

    protected UIPanel mPanel;
    protected bool mInitDone = false;
    /// <summary>
    /// grid实例化的Pop单元对象
    /// </summary>
    public GameObject mpopItem;
    /// <summary>
    /// grid实例化的显示单元对象
    /// </summary>
    public GameObject mcellItem;
    /// <summary>
    /// grid面板y的偏移量
    /// </summary>
    private Vector3 mPanelPos;
    private Vector3 mPanelInitPos;
    private Vector2 mPanelInitOffest;

    public List<Transform> mTransSource = new List<Transform>();
    /// <summary>
    /// grid数据对象列表
    /// </summary>
    private List<bool> m_popState = new List<bool>();
    public DictionaryEx<PopUnit, List<CellUnit>> mDataDic = new DictionaryEx<PopUnit, List<CellUnit>>();
    public List<object> mDataSource = new List<object>();
    public List<Transform> mFreeTrans = new List<Transform>();
    public Dictionary<object, Transform> mDataToTrans = new Dictionary<object, Transform>();
    /// <summary>
    /// 当前选择的grid对象
    /// </summary>
    public GameObject mSelectItem;

    public enum ItemType
    {
        POP,
        CELL,
    }
    public class DisplayUnit
    {
        public ItemType type;
        public int index;
        public object data;
    }
    public class PopUnit:DisplayUnit
    {
    
        private bool select;
        public PopUnit(ItemType type, int index, object data)
        {
            this.type = type;
            this.index = index;
            this.data = data;
        }
        public bool Select
        {
            get
            {
                return select;
            }
            set
            {
                select = value;
            }
        }
        public override bool Equals(object obj)
        {
            PopUnit other = obj as PopUnit;
            if (other == null)
                return false;
            return this.index == other.index;
        }
    }
    public class CellUnit:DisplayUnit
    {
        public bool active=false;
        public CellUnit(ItemType type, int index, object data)
        {
            this.type = type;
            this.index = index;
            this.data = data;
        }
        public override bool Equals(object obj)
        {
            CellUnit other = obj as CellUnit;
            if (other == null)
                return false;
            return this.index == other.index;
        }
        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }
    }
    /// <summary>
    /// 清空自定义数据区
    /// </summary>
    public void ClearCustomData()
    {
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
    }
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
    /// 绑定更新代理函数
    /// </summary>
    /// <param name="fn_ChangeRow">更新代里对象</param>
    public void BindCustomCallBack(OnUpdateDataRow fn_ChangeRow)
    {
        mfnOnChangeRow = fn_ChangeRow;
    }

    protected virtual void Init()
    {
        mInitDone = true;
        mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
        mPanelInitPos = mPanel.transform.localPosition;
        mPanelInitOffest = mPanel.clipOffset;
    }

    protected virtual void Start()
    {
        if (!mInitDone) Init();
        mPanelPos = mPanel.transform.localPosition;
        if (mpopItem != null)
        {
            mpopItem.SetActive(false);
        }
        if (mcellItem != null)
        {
            mcellItem.SetActive(false);
        }
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
    private Transform GetFreeTrans(ItemType type)
    {
        ///实例化一个对象
        GameObject obj = null;
        if (type == ItemType.POP)
            obj = Object.Instantiate(mpopItem) as GameObject;
        else
            obj = Object.Instantiate(mcellItem) as GameObject;
        if (obj != null)
        {
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            return obj.transform;
        }

        return null;
    }
    private void UpdataDisplay()
    {
        int listIndex = 0;
        int popIndex = 0;
        int itemIndex = 0;
        float addHeight = 0;
        for (int i = 0; i < mDataSource.Count; i++)
        {
            DisplayUnit unit = mDataSource[i] as DisplayUnit;
            Transform trans = null;
            if (mDataToTrans.ContainsKey(unit))
            {
                trans = mDataToTrans[unit];
            }
            else
            {

                trans = GetFreeTrans(unit.type);
                if (trans == null)
                    return;
                mDataToTrans.Add(unit, trans);
            }
            Vector3 pos = Vector3.zero;
            if (unit.type == ItemType.POP)
            {
                pos.y = -popHeight * popIndex - cellHeight * itemIndex - addHeight;
                trans.name = DelegateProxy.StringBuilder("Pop", ++popIndex);
                PopUnit popUnit = unit as PopUnit;
                if(popUnit.Select)
                {
                    addHeight = gapHeight;
                }
            }
            else if (unit.type == ItemType.CELL)
            {
                CellUnit cellUnit = unit as CellUnit;
                if (cellUnit.Active)
                {
                    pos.y = -popHeight * popIndex - cellHeight * itemIndex - addHeight;
                    trans.name = DelegateProxy.StringBuilder("Item", ++itemIndex);
                }
                else
                {
                    if (trans.gameObject.activeSelf)
                    {
                        trans.gameObject.SetActive(false);
                    }
                    continue;
                }   

            }
           
            trans.localPosition = pos;
            if (!trans.gameObject.activeSelf)
            {
                trans.gameObject.SetActive(true);
            }
            UIGridItem griditem = trans.gameObject.GetComponent<UIGridItem>();
            if (griditem != null)
            {
                griditem.setIndex(i);
                griditem.onClick = OnClick;
                griditem.oData = unit;
                mfnOnChangeRow(griditem);
            }

            listIndex++;
        }
    }
    /// <summary>
    /// 获取指向序号的用户数据单元
    /// </summary>
    /// <param name="iIndex">第几个</param>
    /// <returns>数据单元</returns>
    public object GetCustomDataItem(int iIndex)
    {
        if (iIndex < 0 || iIndex > mDataSource.Count - 1)
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
        if (iIndex < 0 || iIndex > mDataSource.Count)
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
        if (iIndex < 0 || iIndex > mDataSource.Count)
            return null;
        UIGridItem item = null;
        object oData = mDataSource[iIndex];
        //获取指向序号的UIGridITem
        if (mDataToTrans.ContainsKey(oData))
        {
            Transform trans = mDataToTrans[oData];
            if (trans != null)
            {
                item = trans.GetComponent<UIGridItem>();
            }
            return item;
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

        if (mSelectItem != null && miSelectIndex >= 0)
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
    public void AddCustomDataDic(DictionaryEx<PopUnit,List<CellUnit>> dic)
    {
        mDataDic.Clear();
        m_popState.Clear();
        //for (int i = 0; i < dic.mList.Count; i++)
        //{
        //    mDataDic.Add(dic.mList[i], dic[dic.mList[i]]);
        //}
        mDataDic = new DictionaryEx<PopUnit, List<CellUnit>>(dic);

        //mDataDic = dic;
        for (int i = 0; i < mDataDic.mList.Count; i++)
        {
            m_popState.Add(false);
        }
        ReSerialize(-1);//默认全部闭合
    }
    //重新序列化
    public void ReSerialize(int index)
    {
        //m_typeDic.Clear();
        mDataSource.Clear();
        for (int i = 0; i < m_popState.Count; i++)
        {
            if (i == index)
                m_popState[index] = !m_popState[index];
            else
                m_popState[i] = false;
        }

        foreach (KeyValuePair<PopUnit, List<CellUnit>> pair in mDataDic)
        {
            PopUnit unit_key = pair.Key as PopUnit;
            if (unit_key.index == index)
            {
                unit_key.Select = true;
                mDataSource.Add(unit_key);
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    (pair.Value[i] as CellUnit).Active = m_popState[index];
                    mDataSource.Add(pair.Value[i]);//(new DisplayUnit(ItemType.C, ItemType.RECIPE.ToString() + pair.Key.ToString() + pair.Value[i].ToString(), m_popState[type]));
                }
            }
            else
            {
                unit_key.Select = false;
                mDataSource.Add(unit_key);
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    (pair.Value[i] as CellUnit).Active = false;
                    mDataSource.Add(pair.Value[i]);
                }
            }
        }
        PanelReset();
        UpdataDisplay();
    }
    /// <summary>
    /// 弹钮是否闭合,true是打开
    /// </summary>
    /// <returns></returns>
    public bool IsPopCloseByIdx(int index)
    {
        return m_popState[index];
    }
    /// <summary>
    /// 预制件重置
    /// </summary>
    private void PanelReset()
    {
        UIScrollView view = NGUITools.FindInParents<UIScrollView>(gameObject);
        if (view == null)
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
}
