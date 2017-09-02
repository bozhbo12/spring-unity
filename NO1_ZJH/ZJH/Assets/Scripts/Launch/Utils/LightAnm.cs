using UnityEngine;

public class LightAnm : MonoBehaviour
{
    public Material ma;
    //初始化
    void Start()
    {
        ma = new Material(Shader.Find("Custom/CameraFade")); //创建一个材质
    }

    //这允许你使用基于shader的过滤器来处理最后的图片，
    //进入的图片是source渲染纹理，结果是destination渲染纹理。
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //拷贝源纹理到目的渲染纹理。这主要是用于实现图像效果。
        //Blit设置dest到激活的渲染纹理，在材质上设置source作为
        //_MainTex属性，并且绘制一个全屏方块。
        Graphics.Blit(source, destination, ma);
    }

    float from;
    float to;
    float sp;
    bool upOrDown;
    bool going = false;

    public void startLight(float from, float to, float time)
    {
        this.from = from;
        this.to = to;
        this.sp = (to - from) / time;

        upOrDown = to > from;

        going = true;

    }

    void Update()
    {
        if (!going)
            return;

        from += sp * Time.deltaTime;

        if (upOrDown)
        {
            if (from - to >= 0.01)
            {
                going = false;
                ma.SetFloat("_Float1", 0);
                return;
            }
        }
        else
        {
            if (to - from >= 0.01)
            {
                going = false;
                ma.SetFloat("_Float1", 0);
                return;
            }
        }
        ma.SetFloat("_Float1", from);
    }

}
