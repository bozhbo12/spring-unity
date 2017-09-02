using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***********************************************************************************************
 * 类 : 环境采样器
 ***********************************************************************************************/
[ExecuteInEditMode]
public class AmbienceSampler : MonoBehaviour
{
    /** 水面采样器模式 */
    public enum WaterMode
    {
        Simple = 0,
        Reflective = 1,
        Refractive = 2,
    };

    public WaterMode m_WaterMode = WaterMode.Refractive;
    public bool m_DisablePixelLights = true;
    public int m_TextureSize = 256;
    public float m_ClipPlaneOffset = 0.07f;

    public LayerMask m_ReflectLayers = -1;
    public LayerMask m_RefractLayers = -1;

    private Dictionary<Camera, Camera> m_ReflectionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table
    private Dictionary<Camera, Camera> m_RefractionCameras = new Dictionary<Camera, Camera>(); // Camera -> Camera table

    private RenderTexture m_ReflectionTexture = null;
    private RenderTexture m_RefractionTexture = null;
    private WaterMode m_HardwareWaterSupport = WaterMode.Refractive;
    private int m_OldReflectionTextureSize = 0;
    private int m_OldRefractionTextureSize = 0;

    private static bool s_InsideWater = false;

    private Texture m_MirrorReflectionTexture = null;
    private Texture m_MirrorRefractionTexture = null;

    private string ambienceMapPath = "";

    private int tick = 0;

    void Start()
    {

    }

    public Texture reflectionTexture
    {
        get
        {
            if (m_MirrorReflectionTexture != null)
                return m_MirrorReflectionTexture;
            return m_ReflectionTexture;
        }
    }

    public Texture refractionTexture
    {
        get
        {
            if (m_MirrorRefractionTexture != null)
                return m_MirrorRefractionTexture;
            return m_RefractionTexture;
        }
    }

    public RenderTexture reflectionRenderTexture
    {
        get
        {
            return m_ReflectionTexture;
        }
    }

    public RenderTexture refractionRenderTexture
    {
        get
        {
            return m_RefractionTexture;
        }
    }

    // This is called when it's known that the object will be rendered by some
    // camera. We render reflections / refractions and do other updates here.
    // Because the script executes in edit mode, reflections for the scene view
    // camera will just work!
    public void OnWillRenderObject()
    {
		return;
        tick++;
        GameObjectUnit unit = GameScene.mainScene.FindUnit(gameObject.name);

        ambienceMapPath = "Scenes/" + unit.scene.sceneID + "/ambienceMap/";
        m_MirrorReflectionTexture = AssetLibrary.Load(ambienceMapPath + unit.createID + "Reflection", AssetType.Texture2D).texture2D;
        m_MirrorRefractionTexture = AssetLibrary.Load(ambienceMapPath + unit.createID + "Refraction", AssetType.Texture2D).texture2D;
        
        // 减少采样次数
        if (tick % 2 == 0)
            return;

        if (m_MirrorReflectionTexture != null || m_MirrorRefractionTexture != null)
            return;

        if (!enabled || !GetComponent<Renderer>() || !GetComponent<Renderer>().sharedMaterial || !GetComponent<Renderer>().enabled)
            return;

        Camera cam = Camera.current;
        if (!cam)
            return;

        //transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y - 1f, cam.transform.position.z);  

        // Safeguard from recursive water reflections.		
        if (s_InsideWater)
            return;
        s_InsideWater = true;

        WaterMode mode = WaterMode.Refractive;

        Camera reflectionCamera, refractionCamera;

        // 创建反射和折射采样摄像机
        CreateWaterObjects(cam, out reflectionCamera, out refractionCamera);

        // find out the reflection plane: position and normal in world space
        Vector3 pos = transform.position;
        Vector3 normal = transform.up;

        // 选择有光或者无光模式采样
        int oldPixelLightCount = QualitySettings.pixelLightCount;
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = 0;

        UpdateCameraModes(cam, reflectionCamera);
        UpdateCameraModes(cam, refractionCamera);

        // 渲染反射纹理
        if (mode >= WaterMode.Reflective)
        {
            // Reflect camera around reflection plane
            float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            Matrix4x4 reflection = Matrix4x4.zero;
            CalculateReflectionMatrix(ref reflection, reflectionPlane);
            Vector3 oldpos = cam.transform.position;
            Vector3 newpos = reflection.MultiplyPoint(oldpos);
            reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

            // Setup oblique projection matrix so that near plane is our reflection
            // plane. This way we clip everything below/above it for free.
            Vector4 clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
            Matrix4x4 projection = cam.projectionMatrix;
            CalculateObliqueMatrix(ref projection, clipPlane);
            reflectionCamera.projectionMatrix = projection;

            reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value;               // 不渲染水面层级
            reflectionCamera.targetTexture = m_ReflectionTexture as RenderTexture;
            GL.SetRevertBackfacing(true);
            reflectionCamera.transform.position = newpos;
            Vector3 euler = cam.transform.eulerAngles;
            reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
            reflectionCamera.Render();
            reflectionCamera.transform.position = oldpos;
            GL.SetRevertBackfacing(false);
        }


        // 渲染折射纹理
        if (mode >= WaterMode.Refractive)
        {
            refractionCamera.worldToCameraMatrix = cam.worldToCameraMatrix;

            // Setup oblique projection matrix so that near plane is our reflection
            // plane. This way we clip everything below/above it for free.
            Vector4 clipPlane = CameraSpacePlane(refractionCamera, pos, normal, -1.0f);
            Matrix4x4 projection = cam.projectionMatrix;
            CalculateObliqueMatrix(ref projection, clipPlane);
            refractionCamera.projectionMatrix = projection;

            refractionCamera.cullingMask = ~(1 << 4) & m_RefractLayers.value; // never render water layer
            refractionCamera.targetTexture = m_RefractionTexture as RenderTexture;
            refractionCamera.transform.position = cam.transform.position;
            refractionCamera.transform.rotation = cam.transform.rotation;
            refractionCamera.Render();
            // renderer.sharedMaterial.SetTexture("_RefractionTex", m_RefractionTexture);
        }

        // 如果渲染器不为空，将采样球显示反射图
        if (GetComponent<Renderer>() != null)
            GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", m_ReflectionTexture);

        // 影响任意物体的最大像素光源数
        if (m_DisablePixelLights)
            QualitySettings.pixelLightCount = oldPixelLightCount;

        s_InsideWater = false;
    }


    /***********************************************************************************************
     * 功能 : 消耗创建的对象
     ************************************************************************************************/
    void OnDisable()
    {
        if (m_ReflectionTexture)
        {
            DestroyImmediate(m_ReflectionTexture);
            m_ReflectionTexture = null;
        }
        if (m_RefractionTexture)
        {
            DestroyImmediate(m_RefractionTexture);
            m_RefractionTexture = null;
        }
        foreach (KeyValuePair<Camera, Camera> kvp in m_ReflectionCameras)
            DestroyImmediate((kvp.Value).gameObject);
        m_ReflectionCameras.Clear();
        foreach (KeyValuePair<Camera, Camera> kvp in m_RefractionCameras)
            DestroyImmediate((kvp.Value).gameObject);
        m_RefractionCameras.Clear();
    }



    private void UpdateCameraModes(Camera src, Camera dest)
    {
        if (dest == null)
            return;
        // set water camera to clear the same way as current camera
        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;
        if (src.clearFlags == CameraClearFlags.Skybox)
        {
            Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
            Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
            if (!sky || !sky.material)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }
        // update other values to match current camera.
        // even if we are supplying custom camera&projection matrices,
        // some of values are used elsewhere (e.g. skybox uses far plane)
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }

    /**************************************************************************************************************
     * 功能 : 消耗创建的对象
     **************************************************************************************************************/
    private void CreateWaterObjects(Camera currentCamera, out Camera reflectionCamera, out Camera refractionCamera)
    {
        WaterMode mode = GetWaterMode();

        reflectionCamera = null;
        refractionCamera = null;

        if (mode >= WaterMode.Reflective)
        {
            // 创建反射纹理
            if (!m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize)
            {
                if (m_ReflectionTexture)
                    DestroyImmediate(m_ReflectionTexture);
                m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
                m_ReflectionTexture.name = "__WaterReflection" + GetInstanceID();
                (m_ReflectionTexture  as RenderTexture).isPowerOfTwo  = true;
                m_ReflectionTexture.hideFlags = HideFlags.DontSave;
                //m_ReflectionTexture.depth = 0;
                m_ReflectionTexture.format = RenderTextureFormat.ARGB32;
                m_OldReflectionTextureSize = m_TextureSize;
            }

            // 创建反射摄像机
            m_ReflectionCameras.TryGetValue(currentCamera, out reflectionCamera);
            if (!reflectionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
            {
                GameObject go = new GameObject("Water Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
                reflectionCamera = go.GetComponent<Camera>();
                reflectionCamera.enabled = false;
                reflectionCamera.transform.position = transform.position;
                reflectionCamera.transform.rotation = transform.rotation;
                reflectionCamera.gameObject.AddComponent<FlareLayer>();
                go.hideFlags = HideFlags.HideAndDontSave;
                m_ReflectionCameras[currentCamera] = reflectionCamera;
            }
        }

        if (mode >= WaterMode.Refractive)
        {
            // 创建折射纹理
            if (!m_RefractionTexture || m_OldRefractionTextureSize != m_TextureSize)
            {
                if (m_RefractionTexture)
                    DestroyImmediate(m_RefractionTexture);
                m_RefractionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16);
                m_RefractionTexture.name = "__WaterRefraction" + GetInstanceID();
                (m_ReflectionTexture as RenderTexture).isPowerOfTwo = true;
                m_RefractionTexture.hideFlags = HideFlags.DontSave;
                //m_RefractionTexture.depth = 0;
                m_RefractionTexture.format = RenderTextureFormat.ARGB32;
                m_OldRefractionTextureSize = m_TextureSize;
            }

            // 创建折射纹理
            m_RefractionCameras.TryGetValue(currentCamera, out refractionCamera);
            if (!refractionCamera) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
            {
                GameObject go = new GameObject("Water Refr Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox));
                refractionCamera = go.GetComponent<Camera>();
                refractionCamera.enabled = false;
                refractionCamera.transform.position = transform.position;
                refractionCamera.transform.rotation = transform.rotation;
                refractionCamera.gameObject.AddComponent<FlareLayer>();
                go.hideFlags = HideFlags.HideAndDontSave;
                m_RefractionCameras[currentCamera] = refractionCamera;
            }
        }
    }

    private WaterMode GetWaterMode()
    {
        if (m_HardwareWaterSupport < m_WaterMode)
            return m_HardwareWaterSupport;
        else
            return m_WaterMode;
    }


    // Extended sign: returns -1, 0 or 1 based on sign of a
    private static float sgn(float a)
    {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
    }

    // Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint(offsetPos);
        Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    // Adjusts the given projection matrix so that near plane is the given clipPlane
    // clipPlane is given in camera space. See article in Game Programming Gems 5 and
    // http://aras-p.info/texts/obliqueortho.html
    private static void CalculateObliqueMatrix(ref Matrix4x4 projection, Vector4 clipPlane)
    {
        Vector4 q = projection.inverse * new Vector4(
            sgn(clipPlane.x),
            sgn(clipPlane.y),
            1.0f,
            1.0f
        );
        Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
        // third row = clip plane - fourth row
        projection[2] = c.x - projection[3];
        projection[6] = c.y - projection[7];
        projection[10] = c.z - projection[11];
        projection[14] = c.w - projection[15];
    }

    // Calculates reflection matrix around the given plane
    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
        reflectionMat.m01 = (-2F * plane[0] * plane[1]);
        reflectionMat.m02 = (-2F * plane[0] * plane[2]);
        reflectionMat.m03 = (-2F * plane[3] * plane[0]);

        reflectionMat.m10 = (-2F * plane[1] * plane[0]);
        reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
        reflectionMat.m12 = (-2F * plane[1] * plane[2]);
        reflectionMat.m13 = (-2F * plane[3] * plane[1]);

        reflectionMat.m20 = (-2F * plane[2] * plane[0]);
        reflectionMat.m21 = (-2F * plane[2] * plane[1]);
        reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
        reflectionMat.m23 = (-2F * plane[3] * plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
}
