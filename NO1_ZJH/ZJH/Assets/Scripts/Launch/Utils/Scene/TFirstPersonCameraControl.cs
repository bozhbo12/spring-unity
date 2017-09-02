using UnityEngine;
using System.Collections;

/*****************************************************************************************************
 * 类 : 测试场景第一人称摄像机
 ******************************************************************************************************/
public class TFirstPersonCameraControl : MonoBehaviour
{
    //[HideInInspector]
    public GameObjectUnit target;       // 观测对象
    public float xSpeed;                // x平移速度
    public float ySpeed;                // y平移速度
    public float yMinLimit;             // 
    public float yMaxLimit;             // 

    public float scrollSpeed;           // 
    public float zoomMin;               // 
    public float zoomMax;               // 

    private float distance;
    private float distanceLerp;
    private Vector3 position;
    private bool isActivated;
    private float x;    
    private float y;
    private bool setupCamera;

    void Start()
    {

    }

    public void SetPose(Vector3 eulerAngles)
    {
        Vector3 angles = eulerAngles;
        x = angles.y;
        y = angles.x;

        CalDistance();

        RotateCamera();
    }

    void LateUpdate()
    {
        if (target == null)
        {
            if (GameScene.mainScene != null)
                target = GameScene.mainScene.mainUnit;

            if (target != null)
            {
                Vector3 angles = transform.eulerAngles;
                x = angles.y;
                y = angles.x;

                CalDistance();

                RotateCamera(true);
            }
        }

        if (target != null)
        {
            ScrollMouse();
            RotateCamera();
        }
    }

    void RotateCamera(bool forcibly = false)
    {
        if (Input.GetMouseButtonDown(1))
        {
            isActivated = true;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isActivated = false;
        }

        if (target != null && isActivated || forcibly)
        {
            y -= Input.GetAxis("Mouse Y") * ySpeed;

            x += Input.GetAxis("Mouse X") * xSpeed;

            y = ClampAngle(y, yMinLimit, yMaxLimit);


            Quaternion rotation = Quaternion.Euler(y, x, 0);

            Vector3 calPos = new Vector3(0, 0, -distanceLerp);

            position = rotation * calPos + target.Position;

            transform.rotation = rotation;

            transform.position = position;
        }
        else
        {
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            Vector3 calPos = new Vector3(0, 0, -distanceLerp);

            position = rotation * calPos + target.Position;

            transform.rotation = rotation;

            transform.position = position;
        }
    }

    void CalDistance()
    {
        distance = zoomMax;
        distanceLerp = distance;
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 calPos = new Vector3(0, 0, -distanceLerp);
        position = rotation * calPos + target.Position;
        transform.rotation = rotation;
        transform.position = position;
    }

    void ScrollMouse()
    {
        distanceLerp = Mathf.Lerp(distanceLerp, distance, Time.deltaTime * 5);
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            distance = Vector3.Distance(transform.position, target.Position);

            distance = ScrollLimit(distance - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, zoomMin, zoomMax);
        }
    }

    float ScrollLimit(float dist, float min, float max)
    {
        if (dist < min)
            dist = min;

        if (dist > max)
            dist = max;

        return dist;
    }

    
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

}
