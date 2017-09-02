/// <summary>
/// 第一人称摄像机控制
/// </summary>  

using UnityEngine;

public class EditorCameraControl : MonoBehaviour
{
    //[HideInInspector]
    public Transform target;           // 观测对象

    public float xSpeed = 3.5f;              // x平移速度
    public float ySpeed = 3.5f;              // y平移速度
    public float yMinLimit = 25;             // 
    public float yMaxLimit = 100;            // 

    public float scrollSpeed = 0.3f;            // 
    public float zoomMin = 0;                   // 
    public float zoomMax = 200;                 // 

    public float distance;
    private float distanceLerp;
    private Vector3 position;
    private bool isActivated;
    public float x;
    public float y;

    public GameScene mainScene = new GameScene();
    public TerrainConfig terrainConfig;

    void Start()
    {

    }

    /*****************************************************************************
     * 功能 : 更新摄像机
     *****************************************************************************/

    public void Update()
    {
        MoveCamera();
        ScrollMouse();
        RotateCamera();
    }

    private Vector2 oldMousePos;
    private float mousetAxisX = 0f;
    private float mousetAxisY = 0f;

    /*****************************************************************************
     * 功能 : 移动摄像机
     *****************************************************************************/

    private float speedAdd = 1.0f;

    private bool moving = false;

    public void MoveCamera()
    {
        Event e = Event.current;
        if (e.isKey && e.control == false && e.shift == false && e.type == EventType.keyDown)
        {
            //x = 0.0f;
            //z = 0.0f;

            float degree = 0f;

            if (e.keyCode == KeyCode.A)
                degree = 360f;
            if (e.keyCode == KeyCode.D)
                degree = 180f;
            if (e.keyCode == KeyCode.W)
                degree = 270f;
            if (e.keyCode == KeyCode.S)
                degree = 90f;

            if (e.keyCode == KeyCode.A && e.keyCode == KeyCode.W)
                degree = 225f;

            speedAdd += 0.05f;
            float speed = scrollSpeed * speedAdd;

            // Debug.Log("> " + speed);

            if (Mathf.Abs(degree) > 0.01f)
            {
                if (e.keyCode == KeyCode.A || e.keyCode == KeyCode.D)
                {
                    Quaternion q = Quaternion.Euler(0, -degree, 0);
                    if (GameScene.mainScene != null && GameScene.mainScene.mainCamera != null)
                    {
                        Vector3 dir = (q * (-GameScene.mainScene.mainCamera.transform.right)).normalized;
                        this.transform.position += dir * speed;
                    }
                }
                else if (e.keyCode == KeyCode.W)
                {
                    this.transform.position += transform.forward * speed;
                }
                else if (e.keyCode == KeyCode.S)
                {
                    this.transform.position -= transform.forward * speed;
                }

                moving = true;
            }
        }
        if (e.type == EventType.keyUp)
        {
            speedAdd = 1.0f;

            moving = false;
        }
    }

    /*****************************************************************************
     * 功能 : 旋转摄像机
     *****************************************************************************/

    public void RotateCamera(bool forcibly = false)
    {
        Event e = Event.current;
        if (e.control == true || e.shift == true)
            return;

        if (moving == false)
        {
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                oldMousePos = e.mousePosition;
                isActivated = true;
            }

            if (e.type == EventType.MouseUp || e.button != 1)
            {
                isActivated = false;
            }
        }   
        else
        {
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                oldMousePos = e.mousePosition;
                isActivated = true;
            }

            if (e.type == EventType.MouseUp)
            {
                isActivated = false;
            }
        }

        if (isActivated || forcibly )
        {
            mousetAxisX = (e.mousePosition.x - oldMousePos.x) / 100;
            mousetAxisY = -(e.mousePosition.y - oldMousePos.y) / 100;

            oldMousePos = e.mousePosition;

            y -= mousetAxisY * ySpeed;
            x += mousetAxisX * xSpeed;

            Quaternion rotation = Quaternion.Euler(y, x, 0);
            transform.rotation = rotation;
        }
       
    }


    /*****************************************************************************
     * 功能 : 滚动鼠标
     *****************************************************************************/

    void ScrollMouse()
    {
        Event e = Event.current;
        float speed = scrollSpeed;
        if (e.shift == true)
            speed = scrollSpeed * 5;
        if (e.type == EventType.ScrollWheel)
        {
            float axis = -e.delta.y;
            if (axis != 0)
            {
                this.transform.position += transform.forward * speed * axis;
            }
        }
    }

    //Scroll Limit Method
    float ScrollLimit(float dist, float min, float max)
    {
        if (dist < min)
            dist = min;

        if (dist > max)
            dist = max;

        return dist;
    }

    //Clamp Angle Method
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

}
