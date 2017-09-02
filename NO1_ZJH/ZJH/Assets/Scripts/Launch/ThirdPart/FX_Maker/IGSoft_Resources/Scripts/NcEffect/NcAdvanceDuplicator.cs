// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ¿ÓÕ˙∆Ê
//
// ----------------------------------------------------------------------------------


using UnityEngine;

using System.Collections;
using SystemHelper.Duplicator;
using UnityDevelopment.Calculator;

public class NcAdvanceDuplicator : NcEffectBehaviour
{
    // Attribute ------------------------------------------------------------------------
    public float m_fDuplicateTime = 0.1f;
    public bool m_bSim = false;
    public int m_nDuplicateCount = 3;
    public float m_fDuplicateLifeTime = 0;
    public Vector3 m_AddStartPos = Vector3.zero;
    public Vector3 m_AccumStartRot = Vector3.zero;
    public Vector3 m_RandomRange = Vector3.zero;
    public Vector3[] m_TargetPos, m_TargetScale;

    protected int m_nCreateCount = 1;
    protected float m_fStartTime = 0;

    // addtional attributes
    public int positionMask;
    public int scaleMask;
    public Vector3 m_AddStartScale;

    private DuplicatorBase<GameObject> duplicator;
    private TimedPulser pulser;
    private GameObject clone;

    void Awake()
    {
        pulser = new TimedPulser(m_fDuplicateTime);
        duplicator = DuplicatorBase<GameObject>.create(m_bSim ? DuplicationStrategyType.InstantGeneration : DuplicationStrategyType.SmoothGeneration, m_nDuplicateCount - 1, generateInstance);
    }

    void Start()
    {
        clone = Instantiate(gameObject) as GameObject;
        clone.transform.parent = transform.parent;
        clone.transform.localScale = transform.localScale;
        clone.transform.localRotation = transform.localRotation;
        clone.transform.localPosition = transform.localPosition;
        clone.name = gameObject.name + " Clone ";
        clone.SetActive(false);
    }

    private GameObject generateInstance()
    {
        GameObject instance = Instantiate(clone) as GameObject;
        instance.transform.parent = transform.parent;
        instance.transform.localScale = clone.transform.localScale;
        instance.transform.localRotation = clone.transform.localRotation;
        instance.transform.localPosition = clone.transform.localPosition;
        Destroy(instance.GetComponent<NcAdvanceDuplicator>());

        Vector3 newPos = instance.transform.localPosition;
        // position operation
        switch (positionMask)
        {
            case 0:
                newPos = newPos + m_AddStartPos;
                break;
            case 1:
                newPos = newPos + m_AddStartPos * (m_nCreateCount);
                break;
            case 2:
                newPos = m_TargetPos[m_nCreateCount];
                break;
        }

        // random position       
        instance.transform.localPosition = new Vector3(Random.Range(-m_RandomRange.x, m_RandomRange.x) + newPos.x, Random.Range(-m_RandomRange.y, m_RandomRange.y) + newPos.y, Random.Range(-m_RandomRange.z, m_RandomRange.z) + newPos.z);

        // rotation operation
        instance.transform.localRotation *= Quaternion.Euler(m_AccumStartRot.x * m_nCreateCount, m_AccumStartRot.y * m_nCreateCount, m_AccumStartRot.z * m_nCreateCount);
        instance.name += " " + m_nCreateCount;

        // scale operation
        switch (scaleMask)
        {
            case 0:
                break;
            case 1:
                instance.transform.localScale = m_AddStartScale * (m_nCreateCount + 1);
                break;
            case 2:
                instance.transform.localScale = m_TargetScale[m_nCreateCount];
                break;
        }
        m_nCreateCount++;
        instance.SetActive(false);
        instance.SetActive(true);
        return instance;
    }

    void Update()
    {

        if (m_bSim || pulser.pulse())
        {
            duplicator.operation();

        }
    }
    #region  Õ∑≈
    //add by chenfei 20150824
    void OnDestroy()
    {

        if (null != duplicator)
        {
            duplicator.clear();
        }

        clone = null;
    }
    #endregion;

}


