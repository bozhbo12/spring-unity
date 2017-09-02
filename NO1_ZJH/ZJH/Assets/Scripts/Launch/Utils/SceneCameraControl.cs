using UnityEngine;
using System;

public class SceneCameraControl : MonoBehaviour
{
    public Transform end;
    private float distance;


    public float maxView = 26, minView = 20;
    public float range = 10;
    public bool inverseFlag = false;

    public static Action<float> scrollMouseDelegation;
    public static Transform characterTransform;
    /// <summary>
    /// 缩入比率
    /// </summary>
    public float rate = 1f;
    public static void init(Action<float> _scrollMouseDelegation, Transform _mRoleTransform)
    {
        scrollMouseDelegation = _scrollMouseDelegation;
        characterTransform = _mRoleTransform;
    }

    // Use this for initialization
    void Start()
    {
        if (characterTransform == null)
        {
            return;
        }
        distance = Vector3.Distance(end.position, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (characterTransform == null)
            return;
        float currentDistance = Vector3.Distance(characterTransform.position, transform.position);
        if (currentDistance > distance)
            return;


        //float factor = Vector3.Dot((characterTransform.transform.position - transform.position), (end.position - transform.position)) / Vector3.Dot(vecoterOrigin, vecoterOrigin);
        float factor = 1 - currentDistance / distance;
        //print("distance " + cameraController.distance + "factor " + factor);

        if (factor < 0 || factor > 1)
            return;

        scrollMouseDelegation(factor * range);
    }
}
