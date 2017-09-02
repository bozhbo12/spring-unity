// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------

using UnityEngine;

public class NcDelayActive : MonoBehaviour
{
    public float GetParentDelayTime(bool bCheckStarted) { return 0; }

	// Attribute ------------------------------------------------------------------------
	public		float		m_fDelayTime;

    private NcTimerTool timer = new NcTimerTool();
    private bool usedflag = false;

    private Transform mTrans;
    void Awake()
    {
        mTrans = transform;
        for (int i = 0, length = mTrans.childCount; i < length; i++)
        {
            Transform childTrans = mTrans.GetChild(i);
            IResetAnimation Reset = childTrans.GetComponent<IResetAnimation>();
            if (Reset != null)
                Reset.SetParentIsNcDelay(true);
        }
            
    }

    void Update()
    {
        if (timer.GetTime() > m_fDelayTime && !usedflag)
        {
            Renderer[] renderers = GetComponents<Renderer>();
            for (int i = 0, length = renderers.Length; i < length; i++)
                if (renderers[i] != this)
                    renderers[i].enabled = true;

            for (int i = 0, length = transform.childCount; i < length; i++)
            {
                Transform trans = transform.GetChild(i);
                trans.gameObject.SetActive(true);
                IResetAnimation Reset = trans.GetComponent<IResetAnimation>();
                if (Reset != null)
                    Reset.ResetAnimation();
            }
            usedflag = true;
        }
    }

    void OnEnable()
    {
        timer.Start();

        Renderer[] renderers = GetComponents<Renderer>();
        for (int i = 0, length = renderers.Length; i < length; i++)
                renderers[i].enabled = false;

        for (int i = 0, length = transform.childCount; i < length; i++)
            transform.GetChild(i).gameObject.SetActive(false);

        usedflag = false;
    }

    public void ResetAnimation()
    {
        OnEnable();
    }
}
