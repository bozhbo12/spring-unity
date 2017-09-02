// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NcCurveAnimation : NcEffectAniBehaviour
{
    // Class ------------------------------------------------------------------------
    class NcComparerCurve : IComparer<NcInfoCurve>
    {
        static protected float m_fEqualRange = 0.03f;
        static protected float m_fHDiv = 5.0f;

        public int Compare(NcInfoCurve a, NcInfoCurve b)
        {
            float val = a.m_AniCurve.Evaluate(m_fEqualRange / m_fHDiv) - b.m_AniCurve.Evaluate(m_fEqualRange / m_fHDiv);
            // Equal
            if (Mathf.Abs(val) < m_fEqualRange)
            {
                val = b.m_AniCurve.Evaluate(1 - m_fEqualRange / m_fHDiv) - a.m_AniCurve.Evaluate(1 - m_fEqualRange / m_fHDiv);
                if (Mathf.Abs(val) < m_fEqualRange)
                    return 0;
            }
            return (int)(val * 1000);
        }

        static public int GetSortGroup(NcInfoCurve info)
        {
            float val = info.m_AniCurve.Evaluate(m_fEqualRange / m_fHDiv);
            if (val < -m_fEqualRange) return 1;		// low
            if (m_fEqualRange < val) return 3;		// high
            return 2;								// middle
        }
    }

    [System.Serializable]
    public class NcInfoCurve
    {
        protected const float m_fOverDraw = 0.2f;
        // edit
        public bool m_bEnabled = true;
        public string m_CurveName = string.Empty;
        public AnimationCurve m_AniCurve = new AnimationCurve();
        public enum APPLY_TYPE { NONE, POSITION, ROTATION, SCALE, COLOR, TEXTUREUV, RANDOMANGLE };
        public static string[] m_TypeName = { "None", "Position", "Rotation", "Scale", "Color", "TextureUV", "RandomAngle" };
        public APPLY_TYPE m_ApplyType = APPLY_TYPE.POSITION;
        public bool[] m_bApplyOption = new bool[4] { false, true, false, false };	// w, true=World, false=local
        public bool m_bRecursively = false;			// olny color
        public float m_fValueScale = 1.0f;
        public Vector4 m_ToColor = Color.white;
       
        // internal
        public int m_nTag = 0;
        public int m_nSortGroup;
        public Vector4 m_OriginalValue;
        public Vector4 m_BeforeValue;
        public Vector4[] m_ChildOriginalColorValues;
        public Vector4[] m_ChildBeforeColorValues;

        //		public		enum MODE_TYPE { BY, TO, FROMTO };
        // 		public		static string[]	m_ToModeName		= { "By", "To", "FromTo" };
        // 		public		MODE_TYPE		m_ModeType			= MODE_TYPE.TO;
        // 		public		Vector4			m_ByValue			= new Vector4(1, 1, 1, 1);
        // 		public		Vector4			m_ToValue			= new Vector4(1, 1, 1, 1);

        public bool IsEnabled()
        {
            return m_bEnabled;
        }

        public void SetEnabled(bool bEnable)
        {
            m_bEnabled = bEnable;
        }

        public string GetCurveName()
        {
            return m_CurveName;
        }

        public NcInfoCurve GetClone()
        {
            NcInfoCurve newInfo = new NcInfoCurve();
            newInfo.m_AniCurve = new AnimationCurve(m_AniCurve.keys);
            newInfo.m_AniCurve.postWrapMode = m_AniCurve.postWrapMode;
            newInfo.m_AniCurve.preWrapMode = m_AniCurve.preWrapMode;

            newInfo.m_bEnabled = m_bEnabled;
            newInfo.m_CurveName = m_CurveName;
            newInfo.m_ApplyType = m_ApplyType;
            System.Array.Copy(m_bApplyOption, newInfo.m_bApplyOption, m_bApplyOption.Length);
            newInfo.m_fValueScale = m_fValueScale;
            newInfo.m_bRecursively = m_bRecursively;
            newInfo.m_ToColor = m_ToColor;
            // 			newInfo.m_ModeType		= m_ModeType;
            // 			newInfo.m_ByValue		= m_ByValue;
            // 			newInfo.m_ToValue		= m_ToValue;

            newInfo.m_nTag = m_nTag;
            newInfo.m_nSortGroup = m_nSortGroup;

            return newInfo;
        }

        public void CopyTo(NcInfoCurve target)
        {
            target.m_AniCurve = new AnimationCurve(m_AniCurve.keys);
            target.m_AniCurve.postWrapMode = m_AniCurve.postWrapMode;
            target.m_AniCurve.preWrapMode = m_AniCurve.preWrapMode;

            target.m_bEnabled = m_bEnabled;
            target.m_ApplyType = m_ApplyType;
            System.Array.Copy(m_bApplyOption, target.m_bApplyOption, m_bApplyOption.Length);
            target.m_fValueScale = m_fValueScale;
            target.m_bRecursively = m_bRecursively;
            target.m_ToColor = m_ToColor;
            // 			target.m_ModeType		= m_ModeType;
            // 			target.m_ByValue		= m_ByValue;
            // 			target.m_ToValue		= m_ToValue;

            target.m_nTag = m_nTag;
            target.m_nSortGroup = m_nSortGroup;
        }

        public int GetValueCount()
        {
            switch (m_ApplyType)
            {
                case NcInfoCurve.APPLY_TYPE.POSITION: return 4;
                case NcInfoCurve.APPLY_TYPE.ROTATION: return 4;
                case NcInfoCurve.APPLY_TYPE.SCALE: return 3;
                case NcInfoCurve.APPLY_TYPE.COLOR: return 4;
                case NcInfoCurve.APPLY_TYPE.TEXTUREUV: return 4;
                case NcInfoCurve.APPLY_TYPE.RANDOMANGLE: return 3;
                case NcInfoCurve.APPLY_TYPE.NONE:
                default: return 0;
            }
        }

        public string GetValueName(int nIndex)
        {
            string[] valueNames;

            switch (m_ApplyType)
            {
                case NcInfoCurve.APPLY_TYPE.POSITION:
                case NcInfoCurve.APPLY_TYPE.ROTATION: valueNames = new string[4] { "X", "Y", "Z", "World" }; break;
                case NcInfoCurve.APPLY_TYPE.SCALE: valueNames = new string[4] { "X", "Y", "Z", string.Empty }; break;
                case NcInfoCurve.APPLY_TYPE.COLOR: valueNames = new string[4] { "R", "G", "B", "A" }; break;
                case NcInfoCurve.APPLY_TYPE.TEXTUREUV: valueNames = new string[4] { "X", "Y", "TX", "TY" }; break;
                case NcInfoCurve.APPLY_TYPE.RANDOMANGLE: valueNames = new string[4] { "X", "Y", "Z", string.Empty }; break;
                case NcInfoCurve.APPLY_TYPE.NONE:
                default: valueNames = new string[4] { string.Empty, string.Empty, string.Empty, string.Empty }; break;
            }
            return valueNames[nIndex];
        }

        public void SetDefaultValueScale()
        {
            switch (m_ApplyType)
            {
                case NcInfoCurve.APPLY_TYPE.POSITION: m_fValueScale = 1; break;
                case NcInfoCurve.APPLY_TYPE.ROTATION: m_fValueScale = 360; break;
                case NcInfoCurve.APPLY_TYPE.SCALE: m_fValueScale = 1; break;
                case NcInfoCurve.APPLY_TYPE.COLOR: break;
                case NcInfoCurve.APPLY_TYPE.TEXTUREUV: m_fValueScale = 10; break;
                case NcInfoCurve.APPLY_TYPE.NONE: break;
            }
        }

        public Rect GetFixedDrawRange()
        {
            return new Rect(-m_fOverDraw, -1 - m_fOverDraw, 1 + m_fOverDraw * 2, 2 + m_fOverDraw * 2);
        }

        public Rect GetVariableDrawRange()
        {
            Rect range = new Rect();
            for (int n = 0; n < m_AniCurve.keys.Length; n++)
            {
                // 				range.xMin = Mathf.Min(range.xMin, m_AniCurve[n].time);
                // 				range.xMax = Mathf.Max(range.yMax, m_AniCurve[n].time);
                range.yMin = Mathf.Min(range.yMin, m_AniCurve[n].value);
                range.yMax = Mathf.Max(range.yMax, m_AniCurve[n].value);
            }
            int unit = 20;
            for (int n = 0; n < unit; n++)
            {
                float value = m_AniCurve.Evaluate(n / (float)unit);
                range.yMin = Mathf.Min(range.yMin, value);
                range.yMax = Mathf.Max(range.yMax, value);
            }
            range.xMin = 0;
            range.xMax = 1;
            range.xMin -= range.width * m_fOverDraw;
            range.xMax += range.width * m_fOverDraw;
            range.yMin -= range.height * m_fOverDraw;
            range.yMax += range.height * m_fOverDraw;

            return range;
        }

        public Rect GetEditRange()
        {
            return new Rect(0, -1, 1, 2);
        }

        public void NormalizeCurveTime()
        {
            int n = 0;
            while (n < m_AniCurve.keys.Length)
            {
                Keyframe key = m_AniCurve[n];
                float fMax = Mathf.Max(0, key.time);
                float fVal = Mathf.Min(1, Mathf.Max(fMax, key.time));
                if (fVal != key.time)
                {
                    Keyframe newKey = new Keyframe(fVal, key.value, key.inTangent, key.outTangent);
                    m_AniCurve.RemoveKey(n);
                    n = 0;
                    m_AniCurve.AddKey(newKey);
                    continue;
                }
                n++;
            }
        }
    }
    [SerializeField]

    // Attribute ------------------------------------------------------------------------
    public List<NcInfoCurve> m_CurveInfoList;
    public float m_fDelayTime = 0;
    private bool m_fDelayTimeComplete = false;
    public float m_fDurationTime = 0.6f;
    public bool m_bAutoDestruct = true;

    public int m_iRoationX_Start = 0;
    public int m_iRoationX_End = 0;
    public int m_iRoationY_Start = 0;
    public int m_iRoationY_End = 0;
    public int m_iRoationZ_Start = 0;
    public int m_iRoationZ_End = 0;

    protected float m_fStartTime;
    protected float m_fElapsedRate = 0;
    protected Transform m_Transform;
    private Vector3 m_TransformScale;
    private Vector3 m_TransformLocalPosition;
    private Quaternion m_TransformLocalRotation;
    protected string m_ColorName;
    protected Material m_MainMaterial;
    protected Color m_MainMaterialOriginColor;
    protected string[] m_ChildColorNames;
    protected Renderer[] m_ChildRenderers;
    protected NcUvAnimation m_NcUvAnimation;
    private bool isStarted = false;
    private Renderer thisRenderer = null;

    // Property -------------------------------------------------------------------------
#if UNITY_EDITOR
    public override string CheckProperty()
    {
        if (1 < gameObject.GetComponents(GetType()).Length)
            return "SCRIPT_WARRING_DUPLICATE";

        if (m_CurveInfoList == null || m_CurveInfoList.Count < 1)
            return "SCRIPT_EMPTY_CURVE";

        foreach (NcInfoCurve curveInfo in m_CurveInfoList)
        {
            if (curveInfo.m_bEnabled == false)
                continue;
            if (curveInfo.m_ApplyType == NcInfoCurve.APPLY_TYPE.TEXTUREUV)
                if (GetComponent("NcUvAnimation") == null)
                    return "SCRIPT_NEED_NCUVANIMATION";
            if (curveInfo.m_ApplyType == NcInfoCurve.APPLY_TYPE.COLOR)
                if (curveInfo.m_bRecursively == false && (thisRenderer == null || Ng_GetMaterialColorName(thisRenderer.sharedMaterial) == null))
                    return "SCRIPT_EMPTY_COLOR";
        }

        return string.Empty;	// no error
    }

#endif
    public override int GetAnimationState()
    {
        if (enabled == false || IsActive(gameObject) == false)
            return -1;
        // 		Debug.Log(m_fStartTime);
        // 		Debug.Log(IsEndAnimation());
        if ((0 < m_fDurationTime && (m_fStartTime == 0 || IsEndAnimation() == false)))
            return 1;
        return 0;
    }

    // 	public void UpdateAnimation(float fElapsedRate, bool bResetStartTime)
    // 	{
    // 		UpdateAnimation(fElapsedRate);
    // 		if (bResetStartTime)
    // 			m_fStartTime = GetEngineTime();
    // 	}

    public override void ResetAnimation()
    {
        if (isStarted)
        {
            if (m_MainMaterial != null)
            {
                m_MainMaterial.SetColor(m_ColorName, m_MainMaterialOriginColor);
            }
            if (m_Transform != null)
            {
                if (ContainsCurve(NcInfoCurve.APPLY_TYPE.SCALE))
                {
                    m_Transform.localScale = m_TransformScale;
                }
                m_Transform.localPosition = m_TransformLocalPosition;
                m_Transform.localRotation = m_TransformLocalRotation;
            }
            m_fDelayTimeComplete = false;
            m_fStartTime = GetEngineTime();
        }
        if (gameObject != null)
        {
            //如果像父类有延迟，由父类控制
            if (transform.GetComponentInParent<NcDelayActive>() == null)
            {
                gameObject.SetActive(true);
            }
        }
        if (isStarted)
        {
            if (m_NcUvAnimation != null)
            {
                m_NcUvAnimation.ResetAnimation();
            }
            base.ResetAnimation();
            InitAnimation();
            if (0 < m_fDelayTime)
            {
                if (thisRenderer)
                {
                    thisRenderer.enabled = false;
                }
            }
        }
    }

    public float GetRepeatedRate()
    {
        return m_fElapsedRate;
    }

    // ------------------------------------------------------------------------
    void Awake()
    {
        thisRenderer = GetComponent<Renderer>();
        // 		Debug.Log("NcCurveAnimation.Awake " + transform.parent.name);
    }

    void Start()
    {
        //		Debug.Log("NcCurveAnimation.Start " + transform.parent.name);

        isStarted = true;
        m_fStartTime = GetEngineTime();
        InitAnimation();

        if (0 < m_fDelayTime)
        {
            if (thisRenderer)
                thisRenderer.enabled = false;
            return;
        }
        else
        {
            InitAnimationTimer();
            UpdateAnimation(0);
        }
    }

    void LateUpdate()
    {
        // 		Debug.Log("NcCurveAnimation.Update " + transform.parent.name);
        if (m_fStartTime == 0)
            return;

        if (!m_fDelayTimeComplete)
        {
            if (GetEngineTime() < m_fStartTime + m_fDelayTime)
                return;
            m_fDelayTimeComplete = true;
            InitAnimationTimer();
            if (thisRenderer)
                thisRenderer.enabled = true;
        }

        float fElapsedTime = m_Timer.GetTime();
        float fElapsedRate = fElapsedTime;

        if (0 != m_fDurationTime)
            fElapsedRate = fElapsedTime / m_fDurationTime;
        UpdateAnimation(fElapsedRate);
    }

    private bool ContainsCurve(NcInfoCurve.APPLY_TYPE type)
    {
        int numCurves = m_CurveInfoList.Count;
        for (int i = 0; i < numCurves; ++i)
        {
            NcInfoCurve curveInfo = m_CurveInfoList[i];

            if (curveInfo.m_bEnabled == false)
                continue;

            if (curveInfo.m_ApplyType == type)
            {
                return true;
            }
        }
        return false;
    }

    void InitAnimation()
    {
        m_fElapsedRate = 0;
        m_Transform = transform;
        m_TransformScale = m_Transform.localScale;
        m_TransformLocalPosition = m_Transform.localPosition;
        m_TransformLocalRotation = m_Transform.localRotation;

        int numCurves = m_CurveInfoList.Count;
        for (int i = 0; i < numCurves; ++i)
        {
            NcInfoCurve curveInfo = m_CurveInfoList[i];

            if (curveInfo.m_bEnabled == false)
                continue;

            switch (curveInfo.m_ApplyType)
            {
                case NcInfoCurve.APPLY_TYPE.NONE: continue;
                case NcInfoCurve.APPLY_TYPE.POSITION:
                    {
                        curveInfo.m_OriginalValue = Vector4.zero;
                        curveInfo.m_BeforeValue = curveInfo.m_OriginalValue;
                        break;
                    }
                case NcInfoCurve.APPLY_TYPE.ROTATION:
                    {
                        curveInfo.m_OriginalValue = Vector4.zero;
                        curveInfo.m_BeforeValue = curveInfo.m_OriginalValue;
                        break;
                    }
                case NcInfoCurve.APPLY_TYPE.SCALE:
                    {
                        curveInfo.m_OriginalValue = m_Transform.localScale;
                        curveInfo.m_BeforeValue = curveInfo.m_OriginalValue;
                        break;
                    }
                case NcInfoCurve.APPLY_TYPE.COLOR:
                    {
                        if (curveInfo.m_bRecursively)
                        {
                            // Recursively
                            if (m_ChildRenderers == null)
                            {
                                m_ChildRenderers = transform.GetComponentsInChildren<Renderer>(true);
                                if (m_ChildColorNames == null || m_ChildColorNames.Length != m_ChildRenderers.Length)
                                {
                                    m_ChildColorNames = new string[m_ChildRenderers.Length];
                                }
                                if (curveInfo.m_ChildOriginalColorValues == null || curveInfo.m_ChildOriginalColorValues.Length != m_ChildRenderers.Length)
                                {
                                    curveInfo.m_ChildOriginalColorValues = new Vector4[m_ChildRenderers.Length];
                                }
                                if (curveInfo.m_ChildBeforeColorValues == null || curveInfo.m_ChildBeforeColorValues.Length != m_ChildRenderers.Length)
                                {
                                    curveInfo.m_ChildBeforeColorValues =  new Vector4[m_ChildRenderers.Length];
                                }

                                m_ChildColorNames = new string[m_ChildRenderers.Length];
                                curveInfo.m_ChildOriginalColorValues = new Vector4[m_ChildRenderers.Length];
                                curveInfo.m_ChildBeforeColorValues = new Vector4[m_ChildRenderers.Length];
                                for (int n = 0; n < m_ChildRenderers.Length; n++)
                                {
                                    Renderer ren = m_ChildRenderers[n];
                                    m_ChildColorNames[n] = Ng_GetMaterialColorName(ren.material);

                                    if (m_ChildColorNames[n] != null)
                                        curveInfo.m_ChildOriginalColorValues[n] = ren.material.GetColor(m_ChildColorNames[n]);
                                    curveInfo.m_ChildBeforeColorValues[n] = Vector4.zero;
                                }
                            }
                            else
                            {
                                for (int n = 0; n < m_ChildRenderers.Length; n++)
                                {
                                    Renderer ren = m_ChildRenderers[n];
                                    m_ChildColorNames[n] = Ng_GetMaterialColorName(ren.material);

                                    if (m_ChildColorNames[n] != null)
                                        curveInfo.m_ChildOriginalColorValues[n] = ren.material.GetColor(m_ChildColorNames[n]);
                                    curveInfo.m_ChildBeforeColorValues[n] = Vector4.zero;
                                }
                            }
                        }
                        else
                        {
                            // this Only
                            if (thisRenderer != null)
                            {
                                if (string.IsNullOrEmpty(m_ColorName))
                                    m_ColorName = Ng_GetMaterialColorName(thisRenderer.sharedMaterial);

                                if (m_ColorName != null)
                                    curveInfo.m_OriginalValue = thisRenderer.sharedMaterial.GetColor(m_ColorName);

                                curveInfo.m_BeforeValue = Vector4.zero;
                            }
                        }
                        break;
                    }
                case NcInfoCurve.APPLY_TYPE.TEXTUREUV:
                    {
                        if (m_NcUvAnimation == null)
                            m_NcUvAnimation = GetComponent<NcUvAnimation>();
                        if (m_NcUvAnimation)
                            curveInfo.m_OriginalValue = new Vector4(m_NcUvAnimation.m_fScrollSpeedX, m_NcUvAnimation.m_fScrollSpeedY, 0, 0);
                        curveInfo.m_BeforeValue = curveInfo.m_OriginalValue;
                        break;
                    }
                case NcInfoCurve.APPLY_TYPE.RANDOMANGLE:
                    {
                        Vector3 vRotation = Vector3.zero;
                        if (curveInfo.m_bApplyOption[0])
                        {
                            vRotation.x = Random.Range(m_iRoationX_Start, m_iRoationX_End);
                        }
                        if (curveInfo.m_bApplyOption[1])
                        {
                            vRotation.y = Random.Range(m_iRoationY_Start, m_iRoationY_End);
                        }
                        if (curveInfo.m_bApplyOption[2])
                        {
                            vRotation.z = Random.Range(m_iRoationZ_Start, m_iRoationZ_End);
                        }
                        transform.localEulerAngles = vRotation;
                        break;
                    }
            }
        }
    }

    void UpdateAnimation(float fElapsedRate)
    {
        m_fElapsedRate = fElapsedRate;

        int numCurces = m_CurveInfoList.Count;
        for (int i = 0; i < numCurces; ++i)
        {
            NcInfoCurve curveInfo = m_CurveInfoList[i];

            if (curveInfo.m_bEnabled == false)
                continue;

            float fValue = curveInfo.m_AniCurve.Evaluate(m_fElapsedRate);

            if (curveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.COLOR)
                fValue *= curveInfo.m_fValueScale;

            switch (curveInfo.m_ApplyType)
            {
                case NcInfoCurve.APPLY_TYPE.NONE: continue;
                case NcInfoCurve.APPLY_TYPE.POSITION:
                    {
                        if (curveInfo.m_bApplyOption[3])
                        {
                            Vector3 vPos = Vector3.zero;
                            vPos.x = GetNextValue(curveInfo, 0, fValue);
                            vPos.y = GetNextValue(curveInfo, 1, fValue);
                            vPos.z = GetNextValue(curveInfo, 2, fValue);
                            m_Transform.position += vPos;
                        }
                        else
                        {
                            Vector3 vPos = Vector3.zero;
                            vPos.x = GetNextValue(curveInfo, 0, fValue);
                            vPos.y = GetNextValue(curveInfo, 1, fValue);
                            vPos.z = GetNextValue(curveInfo, 2, fValue);
                            m_Transform.localPosition += vPos;
                        }
                        break;
                    }
                case NcInfoCurve.APPLY_TYPE.ROTATION:
                    {
                        if (curveInfo.m_bApplyOption[3])
                            m_Transform.rotation *= Quaternion.Euler(GetNextValue(curveInfo, 0, fValue), GetNextValue(curveInfo, 1, fValue), GetNextValue(curveInfo, 2, fValue));
                        else m_Transform.localRotation *= Quaternion.Euler(GetNextValue(curveInfo, 0, fValue), GetNextValue(curveInfo, 1, fValue), GetNextValue(curveInfo, 2, fValue));
                        break;
                    }
                case NcInfoCurve.APPLY_TYPE.SCALE:
                    {
                        Vector3 vScale = Vector3.zero;
                        vScale.x = GetNextScale(curveInfo, 0, fValue);
                        vScale.y = GetNextScale(curveInfo, 1, fValue);
                        vScale.z = GetNextScale(curveInfo, 2, fValue);
                        m_Transform.localScale += vScale;
                        break;
                    }
                case NcInfoCurve.APPLY_TYPE.COLOR:
                    {
                        if (curveInfo.m_bRecursively)
                        {
                            // Recursively
                            if (m_ChildColorNames == null || m_ChildColorNames.Length < 0)
                                break;
                            for (int arrayIndex = 0; arrayIndex < m_ChildColorNames.Length; arrayIndex++)
                                if (m_ChildColorNames[arrayIndex] != null && m_ChildRenderers[arrayIndex] != null)
                                    SetChildMaterialColor(curveInfo, fValue, arrayIndex);
                        }
                        else
                        {
                            if (thisRenderer != null && m_ColorName != null)
                            {
                                if (m_MainMaterial == null)
                                {
                                    m_MainMaterial = thisRenderer.material;
                                    m_MainMaterialOriginColor = m_MainMaterial.GetColor(m_ColorName);
                                }

                                // this Only
                                Color color = curveInfo.m_ToColor - curveInfo.m_OriginalValue;
                                Color currentColor = m_MainMaterial.GetColor(m_ColorName);

                                for (int n = 0; n < 4; n++)
                                    currentColor[n] += GetNextValue(curveInfo, n, color[n] * fValue);
                                m_MainMaterial.SetColor(m_ColorName, currentColor);
                            }
                        }
                        break;
                    }
                case NcInfoCurve.APPLY_TYPE.TEXTUREUV:
                    {
                        if (m_NcUvAnimation)
                        {
                            m_NcUvAnimation.m_fOffsetX += GetNextValue(curveInfo, 0, fValue);
                            m_NcUvAnimation.m_fOffsetY += GetNextValue(curveInfo, 1, fValue);
                            m_NcUvAnimation.m_fTilingX += GetNextValue(curveInfo, 2, fValue);
                            m_NcUvAnimation.m_fTilingY += GetNextValue(curveInfo, 3, fValue);
                        }
                        break;
                    }
                case NcInfoCurve.APPLY_TYPE.RANDOMANGLE:
                    {
                        break;
                    }
            }
        }

        if (0 != m_fDurationTime)
        {
            if (1 < m_fElapsedRate)
            {
                if (IsEndAnimation() == false)
                    OnEndAnimation();
                // AutoDestruct
                if (m_bAutoDestruct)
                {
                    gameObject.SetActive(false);
                    //DestroyObject(gameObject);
                }
            }
        }
    }

    void SetChildMaterialColor(NcInfoCurve curveInfo, float fValue, int arrayIndex)
    {
        //m_bRecursively
        Color color = curveInfo.m_ToColor - curveInfo.m_ChildOriginalColorValues[arrayIndex];
        Color currentColor = m_ChildRenderers[arrayIndex].material.GetColor(m_ChildColorNames[arrayIndex]);
        for (int n = 0; n < 4; n++)
            currentColor[n] += GetChildNextColorValue(curveInfo, n, color[n] * fValue, arrayIndex);
        m_ChildRenderers[arrayIndex].material.SetColor(m_ChildColorNames[arrayIndex], currentColor);
    }

    float GetChildNextColorValue(NcInfoCurve curveInfo, int nIndex, float fValue, int arrayIndex)
    {
        if (curveInfo.m_bApplyOption[nIndex])
        {
            float incValue = fValue - curveInfo.m_ChildBeforeColorValues[arrayIndex][nIndex];
            curveInfo.m_ChildBeforeColorValues[arrayIndex][nIndex] = fValue;
            return incValue;
        }
        return 0;
    }

    float GetNextValue(NcInfoCurve curveInfo, int nIndex, float fValue)
    {
        if (curveInfo.m_bApplyOption[nIndex])
        {
            float incValue = fValue - curveInfo.m_BeforeValue[nIndex];
            curveInfo.m_BeforeValue[nIndex] = fValue;
            return incValue;
        }
        return 0;
    }

    float GetNextScale(NcInfoCurve curveInfo, int nIndex, float fValue)
    {
        if (curveInfo.m_bApplyOption[nIndex])
        {
            float absValue = curveInfo.m_OriginalValue[nIndex] * (1.0f + fValue);
            float incValue = absValue - curveInfo.m_BeforeValue[nIndex];
            curveInfo.m_BeforeValue[nIndex] = absValue;
            return incValue;
        }
        return 0;
    }

    // Property -------------------------------------------------------------------------
    public float GetElapsedRate()
    {
        return m_fElapsedRate;
    }

    public void CopyTo(NcCurveAnimation target, bool bCurveOnly)
    {
        target.m_CurveInfoList = new List<NcInfoCurve>();

        foreach (NcInfoCurve curveInfo in m_CurveInfoList)
            target.m_CurveInfoList.Add(curveInfo.GetClone());
        if (bCurveOnly == false)
        {
            target.m_fDelayTime = m_fDelayTime;
            target.m_fDurationTime = m_fDurationTime;
            // 			target.m_bAutoDestruct	= m_bAutoDestruct;
        }
    }

    public void AppendTo(NcCurveAnimation target, bool bCurveOnly)
    {
        if (target.m_CurveInfoList == null)
            target.m_CurveInfoList = new List<NcInfoCurve>();

        foreach (NcInfoCurve curveInfo in m_CurveInfoList)
            target.m_CurveInfoList.Add(curveInfo.GetClone());
        if (bCurveOnly == false)
        {
            target.m_fDelayTime = m_fDelayTime;
            target.m_fDurationTime = m_fDurationTime;
            // 			target.m_bAutoDestruct	= m_bAutoDestruct;
        }
    }

    public NcInfoCurve GetCurveInfo(int nIndex)
    {
        if (m_CurveInfoList == null || nIndex < 0 || m_CurveInfoList.Count <= nIndex)
            return null;
        return m_CurveInfoList[nIndex] as NcInfoCurve;
    }

    public NcInfoCurve GetCurveInfo(string curveName)
    {
        if (m_CurveInfoList == null)
            return null;
        foreach (NcInfoCurve curveInfo in m_CurveInfoList)
            if (curveInfo.m_CurveName == curveName)
                return curveInfo;
        return null;
    }

    public NcInfoCurve SetCurveInfo(int nIndex, NcInfoCurve newInfo)
    {
        if (m_CurveInfoList == null || nIndex < 0 || m_CurveInfoList.Count <= nIndex)
            return null;
        NcInfoCurve oldCurveInfo = m_CurveInfoList[nIndex] as NcInfoCurve;
        m_CurveInfoList[nIndex] = newInfo;
        return oldCurveInfo;
    }

    public int AddCurveInfo()
    {
        NcInfoCurve curveInfo = new NcInfoCurve();
        curveInfo.m_AniCurve = AnimationCurve.EaseInOut(0, 0, 1, 0);
        curveInfo.m_AniCurve.AddKey(0.5f, 0.5f);

        if (m_CurveInfoList == null)
            m_CurveInfoList = new List<NcInfoCurve>();
        m_CurveInfoList.Add(curveInfo);
        return m_CurveInfoList.Count - 1;
    }

    public int AddCurveInfo(NcInfoCurve addCurveInfo)
    {
        if (m_CurveInfoList == null)
            m_CurveInfoList = new List<NcInfoCurve>();
        m_CurveInfoList.Add(addCurveInfo.GetClone());
        return m_CurveInfoList.Count - 1;
    }

    public void DeleteCurveInfo(int nIndex)
    {
        if (m_CurveInfoList == null || nIndex < 0 || m_CurveInfoList.Count <= nIndex)
            return;
        m_CurveInfoList.Remove(m_CurveInfoList[nIndex]);
    }

    public void ClearAllCurveInfo()
    {
        if (m_CurveInfoList == null)
            return;
        m_CurveInfoList.Clear();
    }

    public int GetCurveInfoCount()
    {
        if (m_CurveInfoList == null)
            return 0;
        return m_CurveInfoList.Count;
    }

    public void SortCurveInfo()
    {
        if (m_CurveInfoList == null)
            return;
        m_CurveInfoList.Sort(new NcComparerCurve());

        foreach (NcInfoCurve info in m_CurveInfoList)
            info.m_nSortGroup = NcComparerCurve.GetSortGroup(info);
    }

    public bool CheckInvalidOption()
    {
        bool bDup = false;

        for (int n = 0; n < m_CurveInfoList.Count; n++)
            if (CheckInvalidOption(n))
                bDup = true;
        return bDup;
    }

    public bool CheckInvalidOption(int nSrcIndex)
    {
        NcInfoCurve srcCurveInfo = GetCurveInfo(nSrcIndex);

        if (srcCurveInfo == null)
            return false;
        if (srcCurveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.COLOR && srcCurveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.SCALE && srcCurveInfo.m_ApplyType != NcInfoCurve.APPLY_TYPE.TEXTUREUV)
            return false;

        // 可记 吝汗八荤
        bool bDup = false;
        // 		int		nChkCount = srcCurveInfo.GetValueCount();
        // 
        // 		for (int nIndex = 0;  nIndex < m_CurveInfoList.Count; nIndex++)
        // 		{
        // 			if (nIndex == nSrcIndex)
        // 				continue;
        // 			if (srcCurveInfo.m_ApplyType == m_CurveInfoList[nIndex].m_ApplyType)
        // 			{
        // 				for (int n = 0; n < nChkCount; n++)
        // 					if (srcCurveInfo.m_bApplyOption[n] && m_CurveInfoList[nIndex].m_bApplyOption[n])
        // 					{
        // 						Debug.Log(GetHelpString("DUPLICATE", string.Format("CurveIndex [{0}], TypeValue[{1}]", nIndex.ToString(), m_CurveInfoList[nIndex].GetValueName(n))));
        // 						bDup = true;
        // 					}
        // 			}
        // 		}
        return bDup;
    }

    // Event ----------------------------------------------------------------------------
    public override void OnUpdateEffectSpeed(float fSpeedRate, bool bRuntime)
    {
        m_fDelayTime /= fSpeedRate;
        m_fDurationTime /= fSpeedRate;
    }

    // utility fonction ----------------------------------------------------------------
    public static string Ng_GetMaterialColorName(Material mat)
    {
        string[] propertyNames = { "_Color", "_TintColor", "_EmisColor" };

        if (mat != null)
            foreach (string name in propertyNames)
                if (mat.HasProperty(name))
                    return name;
        return null;
    }

}


