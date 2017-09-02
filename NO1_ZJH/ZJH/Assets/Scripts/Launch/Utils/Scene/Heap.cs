using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using UnityEngine;

/************************************************************************************************
 * 功能 ：二叉树堆栈
 ************************************************************************************************/
public class Heap
{
    public List<HValue> _Values = new List<HValue>();

    /** 功能 ： 添加的长度 */
    public int PushCount;

    public float randomCode = 0f;

    /*************************************************************************
     * 功能 ： 创建新的堆栈
     ****************************************************************************/
    public Heap()
    {
        this.Reset();
        randomCode = UnityEngine.Random.Range(0f, 10000f);
    }

    /**************************************************************************
     * 功能 : 堆栈长度
     ******************************************************************************/
    public int Count
    {
        get
        {
            if (_Values == null)
                return 0;
            return _Values.Count;
        }
    }

    /************************************************************************************
     * 功能 : 检测堆栈是否为空
     **************************************************************************************/
    public bool IsEmpty
    {
        get
        {
            return _Values == null || _Values.Count == 0;
        }
    }

    public void Clear()
    {
        this.Reset();
    }

    /************************************************************************************
     * 功能 : 查找格子在二叉树中的线性索引 
     * @ instance 
     **************************************************************************************/
    public int Find(HValue instance)    
    {
        if (_Values != null && instance != null)
        {
            for (int i = 0; i < _Values.Count; i++)
            {
                if (instance.X == _Values[i].X &&
                    instance.Y == _Values[i].Y)
                    return i;
            }
        }
        return -1;
    }

    /** 重置堆栈 */
    private void Reset()
    {
        _Values.Clear();
        this.PushCount = 0;
    }

    /************************************************************************************
     * 功能 : 追加
     **************************************************************************************/
    public void Push(HValue value)
    {
        _Values.Add(value);
        int pos = _Values.Count - 1;
        Upper(pos);                         // 如果插入的值不是最大的，将重新对堆栈进行排序
        this.PushCount++;
    }

    /**********************************************************************************************
     * 功能 : 
     ************************************************************************************************/
    public HValue Pop()
    {
        if (_Values.Count >= 1)
        {
            HValue ret = _Values[0];
            _Values[0] = _Values[_Values.Count - 1];
            _Values.RemoveAt(_Values.Count - 1);
            Lower(0);
            return ret;
        }
        else
            return new HValue();
    }

    /*************************************************************************************************
     * 功能 : 获取堆栈中第一个值
     **************************************************************************************************/
    public HValue Peek()
    {
        if (_Values.Count >= 1)
            return _Values[0];
        else
            return new HValue();
    }

    /****************************************************************************************************
     * 功能 ： 对插入值进行比较升序排序
     ****************************************************************************************************/
    public void Upper(int pos)
    {
        int current = pos;
        int father = (pos - 1) / 2;

        while (true)
        {
            father = (current - 1) / 2;

            if (current != 0 && HValue.Compare(_Values[current], _Values[father]))
            {
                HValue temp = _Values[current];
                _Values[current] = _Values[father];
                _Values[father] = temp;

                current = father;
                // this.AllDiffer();
            }
            else
            {
                break;
            }
        }
    }

    /****************************************************************************************************
     * 功能 ： 降序排列
     ******************************************************************************************************/
    public void Lower(int pos)
    {
        if (_Values.Count == 0)
            return;

        int current = pos;
        int childL = pos * 2 + 1;
        int childR = pos * 2 + 2;

        while (true)
        {
            int choose = current;                       // 父节点
            childL = current * 2 + 1;                   // 当前节点的左右节点
            childR = current * 2 + 2;
            if (childL < _Values.Count && HValue.Compare(_Values[childL], _Values[choose]))
                choose = childL;
            if (childR < _Values.Count && HValue.Compare(_Values[childR], _Values[choose]))
                choose = childR;

            if (choose == current)
            {
                break;
            }
            else
            {
                HValue temp = _Values[current];
                _Values[current] = _Values[choose];
                _Values[choose] = temp;

                current = choose;
            }
        }
    }

    /** 功能：验证队列的正确性 */
    public bool Validate()
    {
        bool ret = true;
        for (int i = 0; i < _Values.Count; i++)
        {
            if (i * 2 + 1 < _Values.Count && HValue.Compare(_Values[2 * i + 1], _Values[i]))
                ret = false;
            if (i * 2 + 2 < _Values.Count && HValue.Compare(_Values[2 * i + 2], _Values[i]))
                ret = false;
            if (ret == false || i * 2 >= _Values.Count)
                break;
        }
        return ret;
    }

    /** 功能 ：检测是否有重复的值 */
    public bool AllDiffer()
    {
        for (int i = 0; i < _Values.Count; i++)
        {
            for (int j = i + 1; j < _Values.Count; j++)
            {
                if (_Values[i].X == _Values[j].X && _Values[i].Y == _Values[j].Y)
                {
                    return false;
                    /**
                    if (_Values[i] == _Values[j])
                    {
                        Debug.WriteLine("Same object.");
                    }
                    **/
                }
            }
        }
        return true;
    }
}