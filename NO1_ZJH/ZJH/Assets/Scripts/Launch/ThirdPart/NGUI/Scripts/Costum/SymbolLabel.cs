using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

/// <summary>
/// 支持动态字体都Label组件
/// </summary>
public class SymbolLabel : MonoBehaviour
{
    /// <summary>
    /// 表情转移字符定义
    /// </summary>
    private static List<string> m_Symbols = new List<string>(100);
    private static void InitSymbols()
    {
        if (m_Symbols.Count > 0)
            return;

        m_Symbols.Clear();

        m_Symbols.Add("{00}"); m_Symbols.Add("{01}"); m_Symbols.Add("{02}"); m_Symbols.Add("{03}"); m_Symbols.Add("{04}"); m_Symbols.Add("{05}"); m_Symbols.Add("{06}"); m_Symbols.Add("{07}"); m_Symbols.Add("{08}"); m_Symbols.Add("{09}");
        m_Symbols.Add("{10}"); m_Symbols.Add("{11}"); m_Symbols.Add("{12}"); m_Symbols.Add("{13}"); m_Symbols.Add("{14}"); m_Symbols.Add("{15}"); m_Symbols.Add("{16}"); m_Symbols.Add("{17}"); m_Symbols.Add("{18}"); m_Symbols.Add("{19}");
        m_Symbols.Add("{20}"); m_Symbols.Add("{21}"); m_Symbols.Add("{22}"); m_Symbols.Add("{23}"); m_Symbols.Add("{24}"); m_Symbols.Add("{25}"); m_Symbols.Add("{26}"); m_Symbols.Add("{27}"); m_Symbols.Add("{28}"); m_Symbols.Add("{29}");
        m_Symbols.Add("{30}"); m_Symbols.Add("{31}"); m_Symbols.Add("{32}"); m_Symbols.Add("{33}"); m_Symbols.Add("{34}"); m_Symbols.Add("{35}"); m_Symbols.Add("{36}"); m_Symbols.Add("{37}"); m_Symbols.Add("{38}"); m_Symbols.Add("{39}");
        m_Symbols.Add("{40}"); m_Symbols.Add("{41}"); m_Symbols.Add("{42}"); m_Symbols.Add("{43}"); m_Symbols.Add("{44}"); m_Symbols.Add("{45}"); m_Symbols.Add("{46}"); m_Symbols.Add("{47}"); m_Symbols.Add("{48}"); m_Symbols.Add("{49}");
        m_Symbols.Add("{50}"); m_Symbols.Add("{51}"); m_Symbols.Add("{52}"); m_Symbols.Add("{53}"); m_Symbols.Add("{54}"); m_Symbols.Add("{55}"); m_Symbols.Add("{56}"); m_Symbols.Add("{57}"); m_Symbols.Add("{58}"); m_Symbols.Add("{59}");
        m_Symbols.Add("{60}"); m_Symbols.Add("{61}"); m_Symbols.Add("{62}"); m_Symbols.Add("{63}"); m_Symbols.Add("{64}"); m_Symbols.Add("{65}"); m_Symbols.Add("{66}"); m_Symbols.Add("{67}"); 
        m_Symbols.Add("{97}"); m_Symbols.Add("{98}"); m_Symbols.Add("{99}");
    } 

    private string m_Text;
    private string m_realText;
    public string realText { get { return m_realText; } }

    private Vector2 m_textSize;
    public Vector2 textSize { get { return m_textSize; } }

    public UIFont uifont;
    //public Font font;
    public int fontSize = 26;
    public int symbolSize = 40;
    public int spacingY = 0;
    public int width = 100;
    public int depth = 0;
    public int maxLine = 0;
    public int textHeight = 24;
    public UILabel.Overflow overflowMethod = UILabel.Overflow.ResizeHeight;
    public NGUIText.Alignment alignment = NGUIText.Alignment.Left;
    public UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft;

    private UILabel m_TextLabel;
    private UILabel m_SymbolLabel;

    private MatchCollection m_matchs;
    private MatchCollection m_spaceMatchs;
    private List<Match> m_realMatchs;
    /// <summary>
    /// 绑定加载接口相关
    /// </summary>
    /// <param name="strFileName"></param>
    /// <param name="callback"></param>
	public delegate void CallLoadAsset(string strFileName, AssetCallback callback, VarStore varStore = null, bool bAsync = false);
    public static CallLoadAsset monLoadAsset = null;
    public static void SetLoadAssetCall(CallLoadAsset call)
    {
        monLoadAsset = call;
    }
    /// <summary>
    /// 启动后加载表情图集字体
    /// </summary>
    void LoadUiFont()
    {
        if(monLoadAsset != null )
        {
            string strFileName = "Prefabs/UIAtlas/"+"/SymbolFont";
            monLoadAsset(strFileName, OnUIFontLoaded,null);
        }
    }
    /// <summary>
    /// 加载表情图集字体回调
    /// </summary>
    /// <param name="oAsset"></param>
    /// <param name="strFileName"></param>
    /// <param name="args"></param>
    void OnUIFontLoaded(UnityEngine.Object oAsset, string strFileName, VarStore varStore = null)
    {
        GameObject oPrefab = oAsset as GameObject;
        if (oPrefab == null)
        {
            LogSystem.LogWarning("cant find SymbolFont");
            return;
        }

        uifont = oPrefab.GetComponent<UIFont>();
        m_SymbolLabel.bitmapFont = uifont;  
    }
    void Awake()
    {
        m_realMatchs = new List<Match>();
        InitSymbols();
		Transform tran = this.transform.Find ("textLabel");
		if (tran != null)
		{
			m_TextLabel = tran.GetComponent<UILabel>();
		}


		if (m_TextLabel == null)
		{
			m_TextLabel = NGUITools.AddChild<UILabel> (gameObject);
			m_TextLabel.name = "textLabel";
			m_TextLabel.FontName = "NGUIFont";
		} 
        if (Config.SnailUIFont != null)
        {
            m_TextLabel.trueTypeFont = Config.SnailUIFont.dynamicFont;
        }

        
        m_TextLabel.spacingY = spacingY;
        m_TextLabel.fontSize = fontSize;
        m_TextLabel.overflowMethod = overflowMethod;
        m_TextLabel.alignment = alignment;
        m_TextLabel.pivot = pivot;
        m_TextLabel.width = width;
        m_TextLabel.depth = depth;
        m_TextLabel.transform.localPosition = Vector3.zero;
        m_TextLabel.SetSymbolOffset(SymbolOffset);
        if (overflowMethod == UILabel.Overflow.ClampContent)
        {
            m_TextLabel.height = textHeight;
            m_TextLabel.maxLineCount = maxLine;
        }

		Transform trans  = this.transform.Find ("symbolLabel");

		if(trans != null)
		{
			m_SymbolLabel = trans.GetComponent<UILabel> ();
		}
		if(m_SymbolLabel == null)
		{
			m_SymbolLabel = NGUITools.AddChild<UILabel>(gameObject);
			m_SymbolLabel.name = "symbolLabel";
			m_SymbolLabel.FontName = "SymbolFont";
		}

        m_SymbolLabel.bitmapFont = uifont;
        m_SymbolLabel.fontSize = symbolSize;
        m_SymbolLabel.overflowMethod = overflowMethod;
        m_SymbolLabel.alignment = alignment;
        m_SymbolLabel.pivot = pivot;
        m_SymbolLabel.depth = depth + 1;
        Vector3 vTemp = Vector3.zero;
        vTemp.x = 0;
        vTemp.y =  -3;
        vTemp.z =  0;
        m_SymbolLabel.transform.localPosition = vTemp;
        m_SymbolLabel.SetSymbolOffset(SymbolOffset);
        m_SymbolLabel.width = m_SymbolLabel.height = 10;
    }
    void Start()
    {
        LoadUiFont();
    }
    void OnDestroy()
    {
        if (uifont != null)
        {
            CacheObjects.PopCache(uifont.gameObject);
        }
    }
    /// <summary>
    /// 设置Label宽度
    /// </summary>
    /// <param name="width"></param>
    public void SetWidth(int width)
    {
        this.width = width;
        m_TextLabel.width = width;
        m_SymbolLabel.width = width;
    }

    public int height
    {
        get
        {
            return m_TextLabel.height;
        }
    }

    public UILabel labelText
    {
        get
        {
            return m_TextLabel;
        }
    }

    public UILabel labelSymbol
    {
        get
        {
            return m_SymbolLabel;
        }
    }
    const string pat = " ";
    const string pattern = "\\{\\w\\w\\}";
    StringBuilder sString = new StringBuilder();
    public string text
    {
        get { return m_Text; }

        set
        {
            if (m_TextLabel == null || m_SymbolLabel == null || m_realMatchs == null) return;
            if (string.IsNullOrEmpty(value))
            {
                m_Text = string.Empty;
                m_TextLabel.text = null;
                m_SymbolLabel.text = null;
                m_realMatchs.Clear();
                return;
            }
            m_realMatchs.Clear();

            m_Text = value;

            string mProcessedText = m_TextLabel.processedText;
            m_TextLabel.UpdateNGUIText();

            NGUIText.fontSize = fontSize;
            NGUIText.maxLines = maxLine;
            NGUIText.rectWidth = width;
            NGUIText.rectHeight = 100000;

            if (overflowMethod == UILabel.Overflow.ResizeHeight) mProcessedText = m_Text;
            else NGUIText.WrapSymbolText(m_Text, out mProcessedText, symbolSize);

            m_textSize = NGUIText.CalculatePrintedSize(value);


            //string t = value;
            //const string pattern = "\\{\\d\\d*\\}";

            if (sString.Length > 0)
            {
                sString.Remove(0, sString.Length);
            }
            m_realText = NGUIText.StripSymbols(mProcessedText);
            if( m_realText.IndexOf("{") >= 0 )
            {
                m_matchs = Regex.Matches(m_realText, pattern);
                if (m_realText.IndexOf(pat) >= 0)
                {
                    m_spaceMatchs = Regex.Matches(m_realText, pat);
                }

                if (m_matchs != null && m_matchs.Count > 0)
                {
                    int iCount = m_matchs.Count;
                    Match item;
                    for (int i = 0; i < iCount; i++)
                    {
                        item = m_matchs[i];

                        if (m_Symbols.IndexOf(item.Value) > -1)
                        {
                            m_realMatchs.Add(item);
                            sString.Append(item.Value);
                        }
                    }
                }
            }

            m_TextLabel.text = value;
            m_SymbolLabel.text = sString.ToString();

            m_SymbolLabel.width = m_TextLabel.width;
            m_SymbolLabel.height = m_TextLabel.height;

            m_SymbolLabel.MarkAsChanged();
        }
    }

    /// <summary>
    /// 修改顶点坐标 适配表情位置
    /// 1 — 2
    /// |  / |
    /// 0 — 3
    /// </summary>
    private void SymbolOffset()
    {
        List<Vector3> textVerts = m_TextLabel.geometry.verts;
        List<Vector3> symbolVerts = m_SymbolLabel.geometry.verts;
        Vector3 vTemp = Vector3.zero;
        vTemp.x = 0;
        vTemp.y =  0;
        Vector3 spacing = vTemp;
        if (textVerts.Count > 0 && symbolVerts.Count > 0)
        {
            Match item;
            float tw, sw, x = 0;
            int end, start;

            for (int i = 0; i < m_realMatchs.Count; i++)
            {
                item = m_realMatchs[i];

                //获取表情转移字符顶点开始、结束索引
                start = GetIndex(item.Index) * 4;
                end = start + (item.Length - 1) * 4 + 3;

                //表情的顶点索引
                int p = i * 4;

                if ((p + 3) >= symbolVerts.Count) break;

                //表情宽度
                sw = Mathf.Abs(symbolVerts[p].x - symbolVerts[p + 3].x);

                if (end >= textVerts.Count) break;

                //如果不换行，计算文本表情转移符都宽带 否则换行不需要计算 添加1个单位距离 跟在后面
                if (textVerts[start].y == textVerts[end].y)
                {
                    //文本表情转义符宽度
                    tw = Mathf.Abs(textVerts[start].x - textVerts[end].x);

                    //计算居中坐标
                    x = (tw - sw) / 2;
                }
                else x = 1;

                //居中显示表情
                spacing.x = x;

                //计算偏移
                Vector2 po = m_TextLabel.pivotOffset;
                float fx = Mathf.Lerp(0f, -NGUIText.rectWidth, po.x);
                float fy = Mathf.Lerp(NGUIText.rectHeight, 0f, po.y) + Mathf.Lerp((m_TextLabel.printedSize.y - NGUIText.rectHeight), 0f, po.y);
                fx = Mathf.Round(fx);
                fy = Mathf.Round(fy);

                //计算出位移向量   
                Vector3 v = textVerts[start] - symbolVerts[p];
                //第一个顶点
                Vector3 vTempP = textVerts[start] + spacing;
                vTempP.x -= fx;          
                vTempP.y -= fy;
                symbolVerts[p ] = vTempP;

                //第二个顶点
                vTempP = symbolVerts[p + 1] + v + spacing;
                vTempP.x -= fx;
                vTempP.y -= fy;
                symbolVerts[p + 1] = vTempP;

                //第三个顶点
                vTempP = symbolVerts[p + 2] + v + spacing;
                vTempP.x -= fx;
                vTempP.y -= fy;
                symbolVerts[p + 2] = vTempP;

                //第四个顶点
                vTempP = symbolVerts[p + 3] + v + spacing;
                vTempP.x -= fx;
                vTempP.y -= fy;
                symbolVerts[p+3] = vTempP;

                for (int j = 0; j < item.Length; j++)
                {
                    //本来是希望将顶点坐标抹除、但是由于会出现坐标不对都情况、所以放弃了该方法，将顶点都颜色清除掉。
                    //textVerts.buffer[start++] = Vector3.zero;
                    //textVerts.buffer[start++] = Vector3.zero;
                    //textVerts.buffer[start++] = Vector3.zero;
                    //textVerts.buffer[start++] = Vector3.zero;

                    if (m_TextLabel.geometry.cols.Count >= (start + 4))
                    {
                        m_TextLabel.geometry.cols[start++] = Color.clear;
                        m_TextLabel.geometry.cols[start++] = Color.clear;
                        m_TextLabel.geometry.cols[start++] = Color.clear;
                        m_TextLabel.geometry.cols[start++] = Color.clear;
                    }
                }
            }

        }
    }

    /// <summary>
    /// 获取表情转移字符'{'顶点索引，并且需要排除空格符的部分，因为空格符UILabel是不会生成顶点的 所以需要减去空格符都数量，才能正确获得表情索引
    /// </summary>
    /// <returns></returns>
    private int GetIndex(int itemIndex)
    {
        if (m_spaceMatchs == null)
            return itemIndex ;

        Match item;
        int iMaxCount = m_spaceMatchs.Count;
        int count = 0;
        for (int i = 0; i < iMaxCount; i++)
        {
            item = m_spaceMatchs[i];
            if (item.Index < itemIndex)
            {
                count++;
            }
        }

        return itemIndex - count;
    }

}