/// 作者 zhangrj
/// 日期 20140923
/// 实现目标  U3D页签编辑类
/// 跟新 20141017

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/*********************************
 *   常用组件单选按钮界面编辑器       
 * ******************************/

[CustomEditor(typeof(SingleSelectionButton), true)]
public class SingleSelectionButtonEditor : Editor
{
    private SerializedObject single;
    private SerializedProperty atlas;
    private SerializedProperty sprites;
    private SerializedProperty vector2s;
    private SerializedProperty sizes;
    private SingleSelectionButton btn;
    private List<string> names = new List<string>();

    private List<SerializedProperty> normals = new List<SerializedProperty>();
    private List<SerializedProperty> hovers = new List<SerializedProperty>();

    private string mstrAtlasName = string.Empty;
    private UIAtlas muiAtlas = null;

    void SelectNormalSprite(string spriteName)
    {
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("normalSprite");
        sp.stringValue = spriteName;
        serializedObject.ApplyModifiedProperties();
        NGUISettings.selectedSprite = spriteName;
    }

    //void SelectHoverSprite(string spriteName)
    //{
    //    serializedObject.Update();
    //    SerializedProperty sp = serializedObject.FindProperty("checkSprite");
    //    sp.stringValue = spriteName;
    //    serializedObject.ApplyModifiedProperties();
    //    NGUISettings.selectedSprite = spriteName;
    //}
    void OnEnable()
    {
        single = new SerializedObject(target);
        btn = target as SingleSelectionButton;
        int len = btn.transform.childCount;
        names = new List<string>();
        for (int i = 0; i < len; i++)
        {
            names.Add(btn.transform.GetChild(i).name);
        }
    }

    public override void OnInspectorGUI()
    {
        //single.Update();
        GUILayout.BeginHorizontal();
        if (NGUIEditorTools.DrawPrefixButton("Atlas"))
            ComponentSelector.Show<UIAtlas>(OnSelectAtlas);

        SerializedProperty sp = serializedObject.FindProperty("mAtlasName");
        if (muiAtlas == null)
        {
            if (mstrAtlasName != sp.stringValue)
            {
                muiAtlas = GetAtlas(sp.stringValue);
                if (muiAtlas != null)
                {
                    mstrAtlasName = sp.stringValue;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        } 
        UIAtlas atlas = EditorGUILayout.ObjectField(muiAtlas, typeof(UIAtlas), false, GUILayout.MinWidth(20f)) as UIAtlas;
        if (atlas != muiAtlas)
        {
            muiAtlas = atlas;
            SaveAtlas(muiAtlas);
        }

        //SerializedProperty atlas = serializedObject.FindProperty("mAtlasName");
        //atlas = NGUIEditorTools.DrawProperty(string.Empty, serializedObject, "mAtlas", GUILayout.MinWidth(20f));

        if (GUILayout.Button("Edit", GUILayout.Width(40f)))
        {
            if (muiAtlas != null)
            {
                NGUISettings.atlas = muiAtlas;
                NGUIEditorTools.Select(muiAtlas.gameObject);
            }
        }
        GUILayout.EndHorizontal();

        sprites = serializedObject.FindProperty("spritesName");
        if (sprites.arraySize != names.Count)
        {
            serializedObject.Update();
            sprites.arraySize = names.Count;
            normals.Clear();
            hovers.Clear();
            for (int i = 0; i < sprites.arraySize; i++)
            {
                SerializedProperty sprite = sprites.GetArrayElementAtIndex(i);
                normals.Add(sprite.FindPropertyRelative("normal"));
                hovers.Add(sprite.FindPropertyRelative("hover"));
            }
            serializedObject.ApplyModifiedProperties();
        }
        else
        {
            if (sprites.arraySize != normals.Count || sprites.arraySize != hovers.Count)
            {
                normals.Clear();
                hovers.Clear();
                for (int i = 0; i < sprites.arraySize; i++)
                {
                    SerializedProperty sprite = sprites.GetArrayElementAtIndex(i);
                    normals.Add(sprite.FindPropertyRelative("normal"));
                    hovers.Add(sprite.FindPropertyRelative("hover"));
                }
            }
        }

        if (normals.Count > 0)
        {
            for (int i = 0; i < normals.Count; i++)
            {
                GUILayout.Label(names[i] + ":");
                NGUIEditorTools.DrawSpriteField2("Normal:", serializedObject, sp, normals[i], true);
                NGUIEditorTools.DrawSpriteField2("Hover:", serializedObject, sp, hovers[i], true);
            }
        }

      
        btn.LabNormalColor = EditorGUILayout.ColorField("LabelNormalColor", btn.LabNormalColor, GUILayout.Width(200f));
        btn.LabPressColor = EditorGUILayout.ColorField("LabelActiveColor", btn.LabPressColor, GUILayout.Width(200f));

        btn.OutlineNorColor = EditorGUILayout.ColorField("OutlineNorColor", btn.OutlineNorColor, GUILayout.Width(200f));
        btn.OutlinePreColor = EditorGUILayout.ColorField("OutlinePreColor", btn.OutlinePreColor, GUILayout.Width(200f));

        //为了解决在Inspector有时不可编辑的问题
        EditorGUILayout.PropertyField(single.FindProperty("Direct"));
        EditorGUILayout.PropertyField(single.FindProperty("Height"));
        EditorGUILayout.PropertyField(single.FindProperty("SpriteNormalSize"));
        EditorGUILayout.PropertyField(single.FindProperty("SpritePressSize"));
        EditorGUILayout.PropertyField(single.FindProperty("RedNorCoord"));
        EditorGUILayout.PropertyField(single.FindProperty("RedPreCoord"));
        EditorGUILayout.PropertyField(single.FindProperty("LabNormalSize"));
        EditorGUILayout.PropertyField(single.FindProperty("LabPressSize"));
        EditorGUILayout.PropertyField(single.FindProperty("LabNormalSize"));
        EditorGUILayout.PropertyField(single.FindProperty("LabPressSize"));
        EditorGUILayout.PropertyField(single.FindProperty("LabNormalX"));
        EditorGUILayout.PropertyField(single.FindProperty("LabPressX"));
        EditorGUILayout.PropertyField(single.FindProperty("SprNormalX"));
        EditorGUILayout.PropertyField(single.FindProperty("SprPressX"));
        EditorGUILayout.PropertyField(single.FindProperty("RedNorSize"));
        EditorGUILayout.PropertyField(single.FindProperty("RedPreSize"));
        EditorGUILayout.PropertyField(single.FindProperty("NorPosFrom"));
        EditorGUILayout.PropertyField(single.FindProperty("NorPosTo"));
        EditorGUILayout.PropertyField(single.FindProperty("PrePosFrom"));
        EditorGUILayout.PropertyField(single.FindProperty("PrePosTo"));
        EditorGUILayout.PropertyField(single.FindProperty("AnimaInterval"));

        single.ApplyModifiedProperties();
    }

    void OnSelectAtlas(Object obj)
    {
        if (obj is UIAtlas)
        {
            muiAtlas = obj as UIAtlas;
            SaveAtlas(muiAtlas);
        }
    }

    private void SaveAtlas(UIAtlas atlas)
    {
        string strAtlasPath = AssetDatabase.GetAssetPath(atlas);
		string strPath = "/Prefabs/UIAtlas/";
        if (!strAtlasPath.Contains(strPath))
        {
            if (strAtlasPath.Contains("/Local/"))
            {
                serializedObject.Update();
                SerializedProperty sp2 = serializedObject.FindProperty("mAtlasName");
                string strFix2 = "/Local/";
                int iIndex2 = strAtlasPath.LastIndexOf(strFix2);
                if (iIndex2 > 0)
                {
                    strAtlasPath = strAtlasPath.Substring(iIndex2 + 1);
                    int iIndex3 = strAtlasPath.IndexOf(".");
                    if (iIndex3 > 0)
                    {
                        strAtlasPath = strAtlasPath.Substring(0, iIndex3);
                    }
                }
                if (sp2.stringValue != strAtlasPath)
                {
                    sp2.stringValue = strAtlasPath;
                    serializedObject.ApplyModifiedProperties();
                }
            }
            else
            {
                LogSystem.LogWarning("invalid Path" + strAtlasPath);
            }

            return;
        }
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("mAtlasName");
        string strFix = "/UIAtlas/";
        int iIndex = strAtlasPath.LastIndexOf(strFix);
        if (iIndex > 0)
        {
            strAtlasPath = strAtlasPath.Substring(iIndex + strFix.Length);
            int iIndex2 = strAtlasPath.IndexOf(".");
            if (iIndex2 > 0)
            {
                strAtlasPath = strAtlasPath.Substring(0, iIndex2);
            }
        }
        if (sp.stringValue != strAtlasPath)
        {
            sp.stringValue = strAtlasPath;
            serializedObject.ApplyModifiedProperties();
        }
    }

    private UIAtlas GetAtlas(string strAtlasPath)
    {
        if (strAtlasPath.StartsWith("Local/"))
        {
            string strFilePath = "Assets/Resources/" + strAtlasPath + ".prefab";
            GameObject oAtlas = AssetDatabase.LoadMainAssetAtPath(strFilePath) as GameObject;
            if (oAtlas != null)
            {
                UIAtlas uiAtlas = oAtlas.GetComponent<UIAtlas>();
                //NGUISettings.atlas = uiAtlas;
                return uiAtlas;
            }
            else
            {
                LogSystem.LogWarning("No Atlas Asset" + strAtlasPath);
            }
        }
        else
        {
            string strFilePath = "Assets/Resources/Prefabs/UIAtlas/" +  "/" + strAtlasPath + ".prefab";
            GameObject oAtlas = AssetDatabase.LoadMainAssetAtPath(strFilePath) as GameObject;
            if (oAtlas != null)
            {
                UIAtlas uiAtlas = oAtlas.GetComponent<UIAtlas>();
                return uiAtlas;
            }
            else
            {
                LogSystem.LogWarning("No Atlas Asset" + strFilePath);
            }
        }

        return null;
    }
    
}
