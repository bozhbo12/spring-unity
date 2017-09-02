using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillSelectFrame : MonoBehaviour {
    List<Transform> frameList;
    Transform mTrans;

    void Awake(){
        mTrans = transform;

        frameList = new List<Transform>();
        Transform tempSprite = mTrans.Find("TopLeft");
        frameList.Add(tempSprite);
        tempSprite = mTrans.Find("BottomLeft");
        frameList.Add(tempSprite);
        tempSprite = mTrans.Find("TopRight");
        frameList.Add(tempSprite);
        tempSprite = mTrans.Find("BottomRight");
        frameList.Add(tempSprite);
    }

    /// <summary>
    /// 显示技能选中框
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="spriteDepth"></param>
    /// <param name="scale"></param>
    /// <param name="positionInterval"></param>
    /// <param name="duation"></param>
    public void ShowSkillSelectFrame(Transform parent,int spriteDepth,float scale,float positionInterval = 6,float duation = 0.5f)
    {
        mTrans.gameObject.SetActive(true);
        mTrans.parent = parent;
        mTrans.localScale = Vector3.one;
        mTrans.localPosition = Vector3.zero;

        for (int i = 0, count = frameList.Count; i < count;i++ )
        {
            UISprite sprite = frameList[i].GetComponent<UISprite>();
            if (sprite) {
                sprite.depth = spriteDepth;
            }

            TweenPosition tp = frameList[i].GetComponent<TweenPosition>();
            if (tp)
            {
                Vector3 from = tp.from;
                Vector3 to = tp.to;

                from.x = Mathf.Sign(from.x) * (scale / 2 + positionInterval);
                from.y = Mathf.Sign(from.y) * (scale / 2 + positionInterval);
                tp.from = from;

                to.x = Mathf.Sign(to.x) * scale / 2;
                to.y = Mathf.Sign(to.y) * scale / 2;
                tp.to = to;

                tp.duration = duation;
            }
        }
    }
}
