using System;
using System.Collections.Generic;
using UnityEngine;

public class ParserBase
{

    public delegate void ParseCompleteListener(ParserBase parser);
    public ParseCompleteListener parseCompleteListener;

    /** 解析失败 */
    private bool _parsingFailure = false;

    /** 解析完毕 */
    private bool _parsingComplete = false;

    protected byte[] _data;

    protected int _frameLimit;

    public bool parsingFailure
	{
        get
        {
            return _parsingFailure;
        }
        set
        {
            _parsingFailure = value;
        }
	}

    /**********************************************************************************
     * 功能 : 启用解析
     **********************************************************************************/
    public void ParseAsync(byte[] data, int frameLimit = 30)
	{
		_data = data;
		StartParsing(frameLimit);
	}

    protected void StartParsing(int frameLimit)
	{
		_frameLimit = frameLimit;
		
        // 执行循环
        GameScene.mainScene.parsers.Add(this);
	}

    public void Update()
	{
        if (ProceedParsing() && !_parsingFailure)
			FinishParsing();
	}

     /**********************************************************************************
     * 功能 : 处理解析 
     * 注解 : 解析完毕后返回true
     **********************************************************************************/
    virtual public bool ProceedParsing()
    {
        return true;
    }

    public bool parsingComplete
    {
        get
        {
            return _parsingComplete;
        }
    }

    protected void FinishParsing()
	{
		_parsingComplete = true;

        // 执行回调
        if (parseCompleteListener != null)
            parseCompleteListener(this);
	}
}