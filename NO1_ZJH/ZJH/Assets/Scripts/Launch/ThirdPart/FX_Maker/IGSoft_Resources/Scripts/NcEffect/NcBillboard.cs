// ----------------------------------------------------------------------------------
//
// FXMaker
// Created by ismoon - 2012 - ismoonto@gmail.com
//
// ----------------------------------------------------------------------------------


using UnityEngine;
using System.Collections;

public class NcBillboard : NcEffectBehaviour
{
    public Camera mainCamera = null;

    public Transform SpeCameraTransform = null;

    private Transform mainCameraTransform = null;

    private Transform thisTransform = null;

    // Attribute ------------------------------------------------------------------------
    public bool m_bCameraLookAt = true;
    public bool m_bFixedObjectUp = false;
    public bool m_bFixedStand = false;
    public enum AXIS_TYPE { AXIS_FORWARD, AXIS_BACK, AXIS_RIGHT, AXIS_LEFT, AXIS_UP, AXIS_DOWN };
    public AXIS_TYPE m_FrontAxis;
    public enum ROTATION { NONE, RND, ROTATE }
    public ROTATION m_RatationMode;
    public enum AXIS { X = 0, Y, Z };
    public AXIS m_RatationAxis = AXIS.Z;
    public float m_fRotationValue = 180;

    protected float m_fRndValue;
    protected float m_fTotalRotationValue;
    protected Quaternion m_qOiginal;

    public bool m_bUseUICamera = false;

    

    // Property -------------------------------------------------------------------------
#if UNITY_EDITOR
    public override string CheckProperty()
    {
        if (1 < gameObject.GetComponents(GetType()).Length)
            return "SCRIPT_WARRING_DUPLICATE";

        return string.Empty;	// no error
    }
#endif

    // Loop Function --------------------------------------------------------------------
    void Awake()
    {
        mainCamera = Camera.main;
        mainCameraTransform = mainCamera == null ? null : mainCamera.transform;
        thisTransform = transform;
        m_bCameraLookAt = true;
        //m_bFixedObjectUp = true;
    }

#if EDITOR_EFFECT
    protected override void OnEnable()
    {
        base.OnEnable();
        ChangeCamera();
        UpdateBillboard();
    }
#else
    void OnEnable()
    {
        ChangeCamera();
        UpdateBillboard();
    }

#endif



    public void ChangeCamera()
    {
        if (SpeCameraTransform != null)
        {
            Camera sCamera = SpeCameraTransform.GetComponent<Camera>();
            if (sCamera != null)
            {
                mainCamera = sCamera;
            }
        }
        else
        {
            if (m_bUseUICamera)
            {
                GameObject oUICamera = GameObject.FindWithTag("UICamera");
                if (oUICamera != null)
                {
                    Camera cCamera = oUICamera.GetComponent<Camera>();
                    if (cCamera != null)
                    {
                        mainCamera = cCamera;
                    }
                }
            }
            else
            {
                mainCamera = Camera.main;
            }
        }
        
        mainCameraTransform = mainCamera == null ? null : mainCamera.transform;
    }


    public void UpdateBillboard()
    {
        m_fRndValue = Random.Range(0, 360.0f);
        if (enabled)
            Update();
    }

    void Start()
    {
        m_qOiginal = transform.rotation;
    }

    void Update()
    {
        if (mainCamera == null || mainCameraTransform == null)
            return;

        Vector3 vecUp;

        // 카메라 업벡터를 무시하고 오젝의 업벡터를 유지한다
        if (m_bFixedObjectUp)
            //  			vecUp		= m_qOiginal * Vector3.up;
            vecUp = transform.up;
        else vecUp = mainCameraTransform.rotation * Vector3.up;

        if (m_bCameraLookAt)
            thisTransform.LookAt(mainCameraTransform, vecUp);
        else thisTransform.LookAt(thisTransform.position + mainCameraTransform.rotation * Vector3.back, vecUp);

        switch (m_FrontAxis)
        {
            case AXIS_TYPE.AXIS_FORWARD: break;
            case AXIS_TYPE.AXIS_BACK: thisTransform.Rotate(thisTransform.up, 180, Space.World); break;
            case AXIS_TYPE.AXIS_RIGHT: thisTransform.Rotate(thisTransform.up, 270, Space.World); break;
            case AXIS_TYPE.AXIS_LEFT: thisTransform.Rotate(thisTransform.up, 90, Space.World); break;
            case AXIS_TYPE.AXIS_UP: thisTransform.Rotate(thisTransform.right, 90, Space.World); break;
            case AXIS_TYPE.AXIS_DOWN: thisTransform.Rotate(thisTransform.right, 270, Space.World); break;
        }

        if (m_bFixedStand)
            thisTransform.rotation = Quaternion.Euler(new Vector3(0, thisTransform.rotation.eulerAngles.y, thisTransform.rotation.eulerAngles.z));

        if (m_RatationMode == ROTATION.RND)
            thisTransform.localRotation *= Quaternion.Euler((m_RatationAxis == AXIS.X ? m_fRndValue : 0), (m_RatationAxis == AXIS.Y ? m_fRndValue : 0), (m_RatationAxis == AXIS.Z ? m_fRndValue : 0));
        if (m_RatationMode == ROTATION.ROTATE)
        {
            float fRotValue = m_fTotalRotationValue + GetEngineDeltaTime() * m_fRotationValue;
            thisTransform.Rotate((m_RatationAxis == AXIS.X ? fRotValue : 0), (m_RatationAxis == AXIS.Y ? fRotValue : 0), (m_RatationAxis == AXIS.Z ? fRotValue : 0), Space.Self);
            m_fTotalRotationValue = fRotValue;
        }


        //		thisTransform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back,
        //			Camera.main.transform.rotation * Vector3.up);
    }
    // Control Function -----------------------------------------------------------------
    // Event Function -------------------------------------------------------------------
#region 姦렴
    //add by chenfei 20150824
    
    void OnDestroy()
    {
        mainCamera = null;
        m_bUseUICamera = false;
        mainCameraTransform = null;

        thisTransform = null;
    }
#endregion;
}


