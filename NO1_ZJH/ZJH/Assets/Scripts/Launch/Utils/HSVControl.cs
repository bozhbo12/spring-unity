using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSVControl : MonoBehaviour
{

    public float speed;

    private Material mat;
    private Color startColor;
    private Vector4 startColorHSV;
    public string colName = "_TintColor";
    private Color curColor;

	// Use this for initialization
	void Start ()
	{
	    mat = this.gameObject.GetComponent<Renderer>().material;
	    startColor = mat.GetColor(colName);
	    startColorHSV = ConvertRGBToHSV(startColor);

	}
	
	// Update is called once per frame
	void Update () {
	    if (mat != null)
	    {
	        startColorHSV[0] += Time.deltaTime*speed;

            if (startColorHSV[0] >= 1)
	        {
                startColorHSV = ConvertRGBToHSV(startColor);
            }
            curColor = Color.HSVToRGB(startColorHSV.x, startColorHSV.y, startColorHSV.z);
            mat.SetColor(colName, curColor);
        }
	}

    public static Vector4 ConvertRGBToHSV(Color color)
    {
        float h;
        float s;
        float v;
        Color.RGBToHSV(color, out h, out s, out v);
        return new Vector4(h,s,v,color.a);
    }
}
