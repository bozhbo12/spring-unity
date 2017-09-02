using UnityEngine;

public delegate void FinishedCallback(GameObject go);

/// <summary>
/// ÑÓ³ÙÉ¾³ý
/// </summary>
public class DestroyForTime : MonoBehaviour
{
    //public delegate void OnDestroyEffect(GameObject o, bool forceDistroy = true);
    //public static OnDestroyEffect monDestroyEffect;
    //public OnDestroyEffect finishedCallback;
    /*public static void SetDestroyEffect(OnDestroyEffect onDestroyEffect, bool forceDistroy = true)
    {
        monDestroyEffect = onDestroyEffect;
    }*/
    public float time = 0.1f;
    public bool isDestory;
    private float fStartTime = 0.0f;
    private GameObject ui_Effect;

    void Start()
    {
        fStartTime = Time.time;
    }

    public void setUiEffect(GameObject go)
    {
        ui_Effect = go;
    }

    void OnEnable()
    {
        fStartTime = Time.time;
    }

    void Update()
    {
        if (Time.time - fStartTime > time)
        {
            if (ui_Effect != null && CacheObjects.ChechPrefabInPool(ui_Effect))
            {
                CacheObjects.DestoryPoolObject(gameObject);
            }
            else
            {
                CacheObjects.DestoryPoolObject(gameObject);
            }
        }
    }
}
