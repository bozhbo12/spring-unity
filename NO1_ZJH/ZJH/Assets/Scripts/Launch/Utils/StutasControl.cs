using UnityEngine;
using System.Collections;

public class StutasControl : MonoBehaviour
{
    GameObject go;

    void Awake()
    {
        go = gameObject;
    }

    // Use this for initialization
    void Start()
    {
        if (go == null)
        {
            go = gameObject;
        }

        go.SetActive(false);
    }

    void OnEnable()
    {
        if (go == null)
        {
            go = gameObject;
        }

        go.SetActive(false);
    }
}
