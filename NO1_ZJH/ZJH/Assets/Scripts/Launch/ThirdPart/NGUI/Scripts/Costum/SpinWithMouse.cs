using UnityEngine;

[AddComponentMenu("NGUI/Examples/Spin With Mouse")]
public class SpinWithMouse : MonoBehaviour
{
	public Transform target;
	public float speed = 1f;

	Transform mTrans;

    //旋转回调;
    public delegate void OnDragHandle(Vector2 delta);
    public OnDragHandle onDrag;

    public delegate void OnPressHandle(bool isDown);
    public OnPressHandle onPress;

    public Vector3 rotePoint = Vector3.zero;

	void Start ()
	{
		mTrans = transform;
	}

	void OnDrag (Vector2 delta)
	{
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;

		if (target != null)
		{
            if (rotePoint !=Vector3.zero)
            {
                //围绕某个点旋转
                target.RotateAround(rotePoint, Vector3.up, -0.5f * delta.x * speed);
            }
            else
            {
                target.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * speed, 0f) * target.localRotation;
            }
            if (onDrag != null)
            {
                onDrag(delta);
            }
		}
		else
		{
            mTrans.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * speed, 0f) * mTrans.localRotation;
		}
	}

    void OnPress(bool isDown)
    {
        if (onPress != null)
        {
            onPress(isDown);
        }
    }

    void OnDestroy()
    {
        rotePoint = Vector3.zero;
        onDrag = null;
        onPress = null;
    }
}