using UnityEngine;

using System.Collections;

[ExecuteInEditMode] 
public class Mirror : MonoBehaviour
{
    public bool m_DisablePixelLights = false;
    //public int m_TextureSize = 256;
    public float m_ClipPlaneOffset = 0.07f;
	public bool m_IsFlatMirror = true;
   
    public LayerMask m_ReflectLayers = -1;
       
    private Hashtable m_ReflectionCameras = new Hashtable(); 
   
    private RenderTexture m_ReflectionTexture = null;
    //private int m_OldReflectionTextureSize = 0;

    private Renderer mRenderer;

    private static bool s_InsideRendering = false;

    public Camera CurrentCamera;

    public void OnWillRenderObject()
    {
        if (mRenderer == null)
        {
            mRenderer = GetComponent<Renderer>();
        }
        if( !enabled || !mRenderer || !mRenderer.sharedMaterial || !mRenderer.enabled )
            return;
           
        Camera cam = CurrentCamera;
        if( !cam )
            return;
   
        if( s_InsideRendering )
            return;
        s_InsideRendering = true;
       
        Camera reflectionCamera;
        CreateMirrorObjects( cam, out reflectionCamera );
       
        Vector3 pos = transform.position;
		Vector3 normal;
        normal = transform.up;
        //normal.Normalize();
        int oldPixelLightCount = QualitySettings.pixelLightCount;
        if( m_DisablePixelLights )
            QualitySettings.pixelLightCount = 0;
       
        UpdateCameraModes( cam, reflectionCamera );

        float d = -Vector3.Dot (normal, pos) - m_ClipPlaneOffset;
        Vector4 reflectionPlane = new Vector4 (normal.x, normal.y, normal.z, d);
   
        Matrix4x4 reflection = Matrix4x4.zero;
        CalculateReflectionMatrix (ref reflection, reflectionPlane);
        Vector3 oldpos = cam.transform.position;
        Vector3 newpos = reflection.MultiplyPoint( oldpos );
        reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;
   

        Vector4 clipPlane = CameraSpacePlane( reflectionCamera, pos, normal, 1.0f );
        reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);
        //reflectionCamera.cullingMatrix = cam.projectionMatrix * cam.worldToCameraMatrix;


        reflectionCamera.cullingMask = ~(1<<4) & m_ReflectLayers.value; 
        reflectionCamera.targetTexture = m_ReflectionTexture;
        bool oldCulling = GL.invertCulling;
        GL.invertCulling = !oldCulling;
        reflectionCamera.transform.position = newpos;
        Vector3 euler = cam.transform.eulerAngles;
        reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
        reflectionCamera.Render();
        reflectionCamera.transform.position = oldpos;
        GL.invertCulling = oldCulling;
        Material[] materials = mRenderer.sharedMaterials;
        foreach( Material mat in materials ) {
            if( mat.HasProperty("_Ref") )
                mat.SetTexture( "_Ref", m_ReflectionTexture );
        }
        if( m_DisablePixelLights )
            QualitySettings.pixelLightCount = oldPixelLightCount;
       
        s_InsideRendering = false;
    }
   
    void OnDisable()
    {
        if( m_ReflectionTexture ) {
            DestroyImmediate( m_ReflectionTexture );
            m_ReflectionTexture = null;
        }
        foreach( DictionaryEntry kvp in m_ReflectionCameras )
            DestroyImmediate( ((Camera)kvp.Value).gameObject );
        m_ReflectionCameras.Clear();
    }
   
   
    private void UpdateCameraModes( Camera src, Camera dest )
    {
        if( dest == null )
            return;

        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;       
        if( src.clearFlags == CameraClearFlags.Skybox )
        {
            Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
            Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
            if( !sky || !sky.material )
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }

        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
		dest.renderingPath = src.renderingPath;
    }
   

    private void CreateMirrorObjects( Camera currentCamera, out Camera reflectionCamera )
    {
        reflectionCamera = null;

        if( !m_ReflectionTexture)
        {
            if( m_ReflectionTexture )
                DestroyImmediate( m_ReflectionTexture );
            m_ReflectionTexture = new RenderTexture( 512, 512, 16 );
            m_ReflectionTexture.name = "__MirrorReflection" + GetInstanceID();
            m_ReflectionTexture.isPowerOfTwo = true;
            m_ReflectionTexture.hideFlags = HideFlags.DontSave;
        }
       

        reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;
        if( !reflectionCamera ) 
        {
            GameObject go = new GameObject( "Mirror Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox) );
            reflectionCamera = go.GetComponent<Camera>();
            if (reflectionCamera == null)
                reflectionCamera = go.AddComponent<Camera>();
            reflectionCamera.enabled = false;
            reflectionCamera.transform.position = transform.position;
            reflectionCamera.transform.rotation = transform.rotation;
            reflectionCamera.gameObject.AddComponent<FlareLayer>();
            go.hideFlags = HideFlags.HideAndDontSave;
            m_ReflectionCameras[currentCamera] = reflectionCamera;
        }       
    }
   
    private Vector4 CameraSpacePlane (Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cpos = m.MultiplyPoint( offsetPos );
        Vector3 cnormal = m.MultiplyVector( normal ).normalized * sideSign;
        return new Vector4( cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos,cnormal) );
    }


    private static void CalculateReflectionMatrix (ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = (1F - 2F*plane[0]*plane[0]);
        reflectionMat.m01 = (   - 2F*plane[0]*plane[1]);
        reflectionMat.m02 = (   - 2F*plane[0]*plane[2]);
        reflectionMat.m03 = (   - 2F*plane[3]*plane[0]);

        reflectionMat.m10 = (   - 2F*plane[1]*plane[0]);
        reflectionMat.m11 = (1F - 2F*plane[1]*plane[1]);
        reflectionMat.m12 = (   - 2F*plane[1]*plane[2]);
        reflectionMat.m13 = (   - 2F*plane[3]*plane[1]);
   
        reflectionMat.m20 = (   - 2F*plane[2]*plane[0]);
        reflectionMat.m21 = (   - 2F*plane[2]*plane[1]);
        reflectionMat.m22 = (1F - 2F*plane[2]*plane[2]);
        reflectionMat.m23 = (   - 2F*plane[3]*plane[2]);

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
}
