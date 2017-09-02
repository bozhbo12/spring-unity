using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class VarData
{
    private Dictionary<string, System.Object> varMap = new Dictionary<string, System.Object>();

    public void SetColor(String propertyName, Color color)
    {
        if (varMap.ContainsKey(propertyName) == false)
            varMap.Add(propertyName, color);
        else
            varMap[propertyName] = color;
    }

    public Color GetColor(String propertyName)
    { 
        if (varMap.ContainsKey(propertyName) == true)
            return (Color)varMap[propertyName];
        return Color.black;
    }

    public void SetVector(String propertyName, Vector4 vector)
    {
        if (varMap.ContainsKey(propertyName) == false)
            varMap.Add(propertyName, vector);
        else
            varMap[propertyName] = vector;
    }

    public Color GetVector(String propertyName)
    {
        if (varMap.ContainsKey(propertyName) == true)
            return (Vector4)varMap[propertyName];
        return Vector4.zero;
    }

    public void SetFloat(String propertyName, float value)
    {
        if (varMap.ContainsKey(propertyName) == false)
            varMap.Add(propertyName, value);
        else
            varMap[propertyName] = value;
    }

    public float GetFloat(String propertyName)
    {
        if (varMap.ContainsKey(propertyName) == true)
            return Convert.ToSingle(varMap[propertyName]);
        return 0f;
    }

    public void SetInt(String propertyName, int value)
    {
        if (varMap.ContainsKey(propertyName) == false)
            varMap.Add(propertyName, value);
        else
            varMap[propertyName] = value;
    }

    public int GetInt(String propertyName)
    {
        if (varMap.ContainsKey(propertyName) == true)
            return Convert.ToInt32(varMap[propertyName]);
        return 0;
    }
}