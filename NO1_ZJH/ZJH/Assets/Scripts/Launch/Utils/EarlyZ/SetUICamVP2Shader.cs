using UnityEngine;

/// <summary>
/// 该脚本置于UICamera对象上，将UICamera的VP矩阵传给Shader。
/// 注：这里假设UICamera在游戏运行时VP参数不会变化，因此只在Start中设置。
/// </summary>
[RequireComponent(typeof(Camera))]
public class SetUICamVP2Shader : MonoBehaviour
{

    private float size = 0.0f;
    private float Size
    {
        set
        {
            if ((size - value) > 0.01f || (size - value) < -0.01f)
            {
                size = value;
                SetCamera();
            }
        }
    }

    void Start()
    {
        Size = GetComponent<Camera>().orthographicSize;
    }

    void SetCamera()
    {
        Matrix4x4 m = new Matrix4x4();
        Matrix4x4 m1 = new Matrix4x4();
        //float t = camera.orthographicSize;
        float r = size * GetComponent<Camera>().aspect;
        float n = GetComponent<Camera>().nearClipPlane;
        float f = GetComponent<Camera>().farClipPlane;

        float a = 1.0f / r;
        float b = 1.0f / size;
        float c_gl = -2.0f / (f - n);  // OpenGL
        float c_d3x = -2.0f / (f - n);  // D3DX 
        float d_gl = -(f + n) / (f - n); // OpenGL
        float d_d3d = -n / (f - n); // D3DX

        m[0, 0] = a; m[0, 1] = 0; m[0, 2] = 0; m[0, 3] = 0;
        m[1, 0] = 0; m[1, 1] = b; m[1, 2] = 0; m[1, 3] = 0;
        m[2, 0] = 0; m[2, 1] = 0; m[2, 2] = c_gl; m[2, 3] = d_gl;
        m[3, 0] = 0; m[3, 1] = 0; m[3, 2] = 0; m[3, 3] = 1;

        m1[0, 0] = a; m1[0, 1] = 0; m1[0, 2] = 0; m1[0, 3] = 0;
        m1[1, 0] = 0; m1[1, 1] = b; m1[1, 2] = 0; m1[1, 3] = 0;
        m1[2, 0] = 0; m1[2, 1] = 0; m1[2, 2] = c_d3x; m1[2, 3] = d_d3d;
        m1[3, 0] = 0; m1[3, 1] = 0; m1[3, 2] = 0; m1[3, 3] = 1;

        Matrix4x4 materix;

#if (UNITY_EDITOR && !UNITY_STANDALONE_OSX) || UNITY_STANDALONE_WIN
        materix = m1 * GetComponent<Camera>().worldToCameraMatrix;
#else
        materix = GetComponent<Camera>().projectionMatrix * GetComponent<Camera>().worldToCameraMatrix;
        //materix = m * camera.worldToCameraMatrix;
#endif
        Shader.SetGlobalMatrix("UI_CAM_MATRIX_VP", materix);
    }

    void LateUpdate()
    {
        Size = GetComponent<Camera>().orthographicSize;
    }
}
