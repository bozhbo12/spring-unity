using UnityEngine;
using System.Collections;


public class ShadowMapData : ScriptableObject
{
    public Vector3 m_Pos;

    public int m_Size = 0;

    public float m_AddLight = 0;

    public Texture2D shadowMap;

}
