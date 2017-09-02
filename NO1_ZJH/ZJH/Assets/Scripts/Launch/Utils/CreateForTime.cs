using UnityEngine;
using System.Collections;

/// <summary>
/// 在半径内固定时间创建给定的个数
/// </summary>
public class CreateForTime : MonoBehaviour, IResetAnimation
{
    public float fDelayTime;

    public float fObjectCount;

    public float fRadii;
    
    /// <summary>
    /// 特效路径 用于缓存唯一id
    /// </summary>
    public string mstrEffectPath = string.Empty;

    /// <summary>
    /// 删除时间(与mbAutoCreate配合使用)
    /// mbAutoCreate为false 以mfAutoDestroyTime为删除时间
    /// mbAutoCreate为true  以对象创建次数
    /// </summary>
    public float mfAutoDestroyTime = 1f;

    /// <summary>
    /// 自动创建
    /// </summary>
    [HideInInspector]
    private bool mbAutoCreate = true;

    float fStartTime;

    //Transform thisTrans;

    int index = 0;

    private UnityEngine.Object _asset = null;

    private float mfCurLiveTime = 0f;
	// Use this for initialization
    void Start ()
    {        
        //thisTrans = this.transform;
        fStartTime = 0f;
        index = 0;
        mfCurLiveTime = mfAutoDestroyTime;
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (!mbAutoCreate)
        {//被复制出来的特效需要走的逻辑
            mfCurLiveTime -= Time.deltaTime;
            if (mfCurLiveTime <= 0)
                DelegateProxy.CollectObject(mstrEffectPath, gameObject);
            return;
        }

        if (index >= fObjectCount)
        {//主特效逻辑   如果复制出来的特效数量上限
            mfCurLiveTime -= Time.deltaTime;
            if (mfCurLiveTime <= 0)
            {
                //thisTrans.parent = null;
                //DelegateProxy.CollectObject(mstrEffectPath, gameObject);
            }
            return;
        }

        if (fStartTime <= fDelayTime)
        {// 特效复制延迟时间
            fStartTime += Time.deltaTime;
            return;
        }

        fStartTime = 0f;
        DelegateProxy.GetEffectObj(mstrEffectPath, LoadComplete);
	}

    private void LoadComplete(UnityEngine.Object oAsset, string strFileName, VarStore varStore)
    {
        //GameObject oEffect = oAsset as GameObject;
        _asset = oAsset;
        GameObject oEffect = CacheObjects.InstantiatePool(oAsset) as GameObject;
        if (oEffect == null)
            return;

        CreateForTime cft = oEffect.GetComponent<CreateForTime>();
        if (cft != null)
            cft.mbAutoCreate = false;
        float x = Random.Range(-fRadii, fRadii);
        float z = Random.Range(-fRadii, fRadii);
        oEffect.transform.position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        index++;
    }

    bool bParentIsNcDelay = false;
    /// <summary>
    /// 设置父对象是否nc
    /// </summary>
    /// <param name="bValue"></param>
    public void SetParentIsNcDelay(bool bValue)
    {
        bParentIsNcDelay = bValue;
    }

    /// <summary>
    /// 获取父对象是否是nc
    /// </summary>
    /// <returns></returns>
    public bool GetParentIsNcDelay()
    {
        return bParentIsNcDelay;
    }

    public void ResetAnimation()
    {
        mbAutoCreate = true;
        fStartTime = 0f;
        index = 0;
        mfCurLiveTime = mfAutoDestroyTime;
    }
}
