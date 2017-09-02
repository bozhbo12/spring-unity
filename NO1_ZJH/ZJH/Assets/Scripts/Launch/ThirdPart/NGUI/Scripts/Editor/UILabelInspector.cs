//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

#if !UNITY_3_5 && !UNITY_FLASH
#define DYNAMIC_FONT
#endif

using UnityEngine;
using UnityEditor;

/// <summary>
/// Inspector class used to edit UILabels.
/// </summary>

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UILabel))]
#else
[CustomEditor(typeof(UILabel), true)]
#endif
public class UILabelInspector : UIWidgetInspector
{
    public enum FontType
    {
        NGUI,
        Unity,
    }

    UILabel mLabel;
    FontType mFontType;

    protected override void OnEnable()
    {
        if (target == null)
            return;

        base.OnEnable();
        //SerializedProperty bit = serializedObject.FindProperty("mFont");
        SerializedProperty sp = serializedObject.FindProperty("mstrFontName");
       // SerializedProperty sp = serializedObject.FindProperty("mTrueTypeFont");
        mFontType = (sp != null && !string.IsNullOrEmpty(sp.stringValue)) ? FontType.NGUI : FontType.Unity;
    }

    string mstrFontName = string.Empty;
    UIFont muiFont = null;
    UIFont GetFont(string strFontPath)
    {
        string strFilePath = string.Empty;
        if ( strFontPath.Contains("NGUIFont") )
        {
            strFilePath = "Assets/Resources/Fonts/" +  "/" + strFontPath + ".prefab";
        }
        else
        {
            strFilePath = "Assets/Resources/Prefabs/UIAtlas/" + "/" + strFontPath + ".prefab";
        }
       
        GameObject oFont = AssetDatabase.LoadMainAssetAtPath(strFilePath) as GameObject;
        if (oFont != null)
        {
            UIFont uiFont = oFont.GetComponent<UIFont>();
            //NGUISettings.atlas = uiAtlas;
            return uiFont;
        }
        return null;
    }
    void SaveFont(UIFont uifont)
    {
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("mstrFontName");
        
        if (sp.stringValue != uifont.name)
        {
            sp.stringValue = uifont.name;
            serializedObject.ApplyModifiedProperties();
			serializedObject.Update ();
			UILabel lbl = mWidget as UILabel;
			if (lbl != null) 
			{
				lbl.bitmapFont = muiFont;
				EditorUtility.SetDirty (lbl);
			}
        }
    }

    void OnNGUIFont(Object obj)
    {

        if (obj is UIFont)
        {
            muiFont = obj as UIFont;
            SaveFont(muiFont);
        }

        //serializedObject.Update();
        //SerializedProperty sp = serializedObject.FindProperty("mFont");
        //sp.objectReferenceValue = obj;
        //serializedObject.ApplyModifiedProperties();
        //NGUISettings.ambigiousFont = obj;

    }

    void OnUnityFont(Object obj)
    {
        serializedObject.Update();
        SerializedProperty sp = serializedObject.FindProperty("mTrueTypeFont");
        sp.objectReferenceValue = obj;
        serializedObject.ApplyModifiedProperties();
        NGUISettings.ambigiousFont = obj;
    }

    /// <summary>
    /// Draw the label's properties.
    /// </summary>

    protected override bool ShouldDrawProperties()
    {
        mLabel = mWidget as UILabel;

        GUILayout.BeginHorizontal();

#if DYNAMIC_FONT
        mFontType = (FontType)EditorGUILayout.EnumPopup(mFontType, "DropDown", GUILayout.Width(74f));
        if (NGUIEditorTools.DrawPrefixButton("Font", GUILayout.Width(64f)))
#else
		mFontType = FontType.NGUI;
		if (NGUIEditorTools.DrawPrefixButton("Font", GUILayout.Width(74f)))
#endif
        {
            if (mFontType == FontType.NGUI)
            {
                ComponentSelector.Show<UIFont>(OnNGUIFont);
            }
            else
            {
                ComponentSelector.Show<Font>(OnUnityFont, new string[] { ".ttf", ".otf" });
            }
        }

        bool isValid = false;

        SerializedProperty ttf = null;

        //SerializedProperty fnt = null;
        //SerializedProperty ttf = null;


        if (mFontType == FontType.NGUI)
        {

            //string strFontName = string.Empty;
            if (muiFont != null)
            {
                //strFontName = muiFont.name;
                isValid = true;
            }
            else
            {
                SerializedProperty sp = serializedObject.FindProperty("mstrFontName");
                if (sp != null && mstrFontName != sp.stringValue)
                {
                    muiFont = GetFont(sp.stringValue);
                    if (muiFont != null)
                    {
                        mstrFontName = sp.stringValue;
                        //strFontName = muiFont.name;
                        serializedObject.ApplyModifiedProperties();
                    }
                    isValid = true;
                }
            }

            UIFont uifont = EditorGUILayout.ObjectField( muiFont, typeof(UIFont), false, GUILayout.MinWidth(20f)) as UIFont;
            if (uifont != muiFont)
            {
                muiFont = uifont;   
                SaveFont(muiFont);
                NGUISettings.ambigiousFont = muiFont;
                isValid = true;
                //NGUISettings.ambigiousFont = fnt.objectReferenceValue;

            }


           
            //fnt = NGUIEditorTools.DrawProperty(string.Empty, serializedObject, "mFont", GUILayout.MinWidth(40f));

            //if (fnt.objectReferenceValue != null)
            //{
            //    NGUISettings.ambigiousFont = fnt.objectReferenceValue;
            //    isValid = true;
            //}


        }
        else
        {
            ttf = NGUIEditorTools.DrawProperty(string.Empty, serializedObject, "mTrueTypeFont", GUILayout.MinWidth(40f));

            if (ttf.objectReferenceValue != null)
            {
                NGUISettings.ambigiousFont = ttf.objectReferenceValue;
                isValid = true;
            }
        }

        GUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(!isValid);
        {

			UIFont uiFont = muiFont;
			Font dynFont = null;
			if( uiFont != null )
			{
				GUILayout.BeginHorizontal();
				SerializedProperty prop = NGUIEditorTools.DrawProperty("Font Size", serializedObject, "mFontSize", GUILayout.Width(142f));

				EditorGUI.BeginDisabledGroup(true);
				if (!serializedObject.isEditingMultipleObjects)
					GUILayout.Label(" Default: " + mLabel.defaultFontSize);
				EditorGUI.EndDisabledGroup();

				NGUISettings.fontSize = prop.intValue;
				GUILayout.EndHorizontal();
			}

            //UIFont uiFont = (fnt != null) ? fnt.objectReferenceValue as UIFont : null;
            //Font dynFont = (ttf != null) ? ttf.objectReferenceValue as Font : null;
            //if (uiFont != null && uiFont.isDynamic)
            //{
            //    dynFont = uiFont.dynamicFont;
            //    uiFont = null;
            //}

            //if (dynFont != null)
            //{
            //    GUILayout.BeginHorizontal();
            //    {
            //        EditorGUI.BeginDisabledGroup((ttf != null) ? ttf.hasMultipleDifferentValues : fnt.hasMultipleDifferentValues);

            //        SerializedProperty prop = NGUIEditorTools.DrawProperty("Font Size", serializedObject, "mFontSize", GUILayout.Width(142f));
            //        NGUISettings.fontSize = prop.intValue;

            //        prop = NGUIEditorTools.DrawProperty(string.Empty, serializedObject, "mFontStyle", GUILayout.MinWidth(40f));
            //        NGUISettings.fontStyle = (FontStyle)prop.intValue;

            //        GUILayout.Space(18f);
            //        EditorGUI.EndDisabledGroup();
            //    }
            //    GUILayout.EndHorizontal();

            //    NGUIEditorTools.DrawProperty("Material", serializedObject, "mMaterial");
            //}
            //else if (uiFont != null )
            //{
            //    GUILayout.BeginHorizontal();
            //    SerializedProperty prop = NGUIEditorTools.DrawProperty("Font Size", serializedObject, "mFontSize", GUILayout.Width(142f));

            //    EditorGUI.BeginDisabledGroup(true);
            //    if (!serializedObject.isEditingMultipleObjects)
            //    GUILayout.Label(" Default: " + mLabel.defaultFontSize);
            //    EditorGUI.EndDisabledGroup();

            //    NGUISettings.fontSize = prop.intValue;
            //    GUILayout.EndHorizontal();
            //}


            bool ww = GUI.skin.textField.wordWrap;
            GUI.skin.textField.wordWrap = true;
            SerializedProperty sp = serializedObject.FindProperty("mText");
#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			GUI.changed = false;
			string text = EditorGUILayout.TextArea(sp.stringValue, GUI.skin.textArea, GUILayout.Height(100f));
			if (GUI.changed) sp.stringValue = text;
#else
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2
			GUILayout.Space(-16f);
#endif
            if (sp.hasMultipleDifferentValues)
            {
                NGUIEditorTools.DrawProperty(string.Empty, sp, GUILayout.Height(128f));
            }
            else
            {
                GUIStyle style = new GUIStyle(EditorStyles.textField);
                style.wordWrap = true;

                float height = style.CalcHeight(new GUIContent(sp.stringValue), ResolutionConstrain.Instance.width - 100f);
                bool offset = true;

                if (height > 90f)
                {
                    offset = false;
                    height = style.CalcHeight(new GUIContent(sp.stringValue), ResolutionConstrain.Instance.width - 20f);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical(GUILayout.Width(76f));
                    GUILayout.Space(3f);
                    GUILayout.Label("Text");
                    GUILayout.EndVertical();
                    GUILayout.BeginVertical();
                }
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(height));

                // YangDan 
                //----------------- 修改编号转义文本 -----------------
                //GUI.changed = false;
                //string text = EditorGUI.TextArea(rect, sp.stringValue, style);
                //if (GUI.changed) sp.stringValue = text;
                EditorGUI.LabelField(rect, sp.stringValue, style);
                //-----------------------------------------------------

                if (offset)
                {
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();
                }
            }
#endif
            GUI.skin.textField.wordWrap = ww;

            //---------------   YangDan 添加一条属性(用于转义文本)  ---------------------
            SerializedProperty original = serializedObject.FindProperty("mOriginalText");
            GUIStyle s = new GUIStyle(EditorStyles.textField);
            s.wordWrap = true;
            float h = s.CalcHeight(new GUIContent(original.stringValue), ResolutionConstrain.Instance.width - 100f);
            if (h > 90f)
            {
                h = s.CalcHeight(new GUIContent(original.stringValue), ResolutionConstrain.Instance.width - 20f);
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical(GUILayout.Width(76f));
                GUILayout.Space(3f);
                GUILayout.Label("UIString");
                GUILayout.EndVertical();
                GUILayout.BeginVertical();
            }
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(h));
            GUI.changed = false;
            string text = EditorGUI.TextArea(r, original.stringValue, s);
            if (GUI.changed) 
                original.stringValue = text;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
            NGUIEditorTools.DrawProperty("IntConvert", serializedObject, "mIntConvert", GUILayout.Width(100f));
            GUILayout.EndVertical();
            //-----------------------------------------------------------------------

            SerializedProperty ov = NGUIEditorTools.DrawPaddedProperty("Overflow", serializedObject, "mOverflow");
            NGUISettings.overflowStyle = (UILabel.Overflow)ov.intValue;

            NGUIEditorTools.DrawPaddedProperty("Alignment", serializedObject, "mAlignment");

            if (dynFont != null)
                NGUIEditorTools.DrawPaddedProperty("Keep crisp", serializedObject, "keepCrispWhenShrunk");

            EditorGUI.BeginDisabledGroup(mLabel.bitmapFont != null && mLabel.bitmapFont.packedFontShader);
            GUILayout.BeginHorizontal();
            SerializedProperty gr = NGUIEditorTools.DrawProperty("Gradient", serializedObject, "mApplyGradient",
#if UNITY_3_5
				GUILayout.Width(93f));
#else
 GUILayout.Width(95f));
#endif
            EditorGUI.BeginDisabledGroup(!gr.hasMultipleDifferentValues && !gr.boolValue);
            {
                NGUIEditorTools.SetLabelWidth(30f);
                NGUIEditorTools.DrawProperty("Top", serializedObject, "mGradientTop", GUILayout.MinWidth(40f));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                NGUIEditorTools.SetLabelWidth(50f);
#if UNITY_3_5
				GUILayout.Space(81f);
#else
                GUILayout.Space(79f);
#endif
                NGUIEditorTools.DrawProperty("Bottom", serializedObject, "mGradientBottom", GUILayout.MinWidth(40f));
                NGUIEditorTools.SetLabelWidth(80f);
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Effect", GUILayout.Width(76f));
            sp = NGUIEditorTools.DrawProperty(string.Empty, serializedObject, "mEffectStyle", GUILayout.MinWidth(16f));

            EditorGUI.BeginDisabledGroup(!sp.hasMultipleDifferentValues && !sp.boolValue);
            {
                NGUIEditorTools.DrawProperty(string.Empty, serializedObject, "mEffectColor", GUILayout.MinWidth(10f));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(" ", GUILayout.Width(56f));
                    NGUIEditorTools.SetLabelWidth(20f);
                    NGUIEditorTools.DrawProperty("X", serializedObject, "mEffectDistance.x", GUILayout.MinWidth(40f));
                    NGUIEditorTools.DrawProperty("Y", serializedObject, "mEffectDistance.y", GUILayout.MinWidth(40f));
                    GUILayout.Space(18f);
                    NGUIEditorTools.SetLabelWidth(80f);
                }
            }
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Spacing", GUILayout.Width(56f));
            NGUIEditorTools.SetLabelWidth(20f);
            NGUIEditorTools.DrawProperty("X", serializedObject, "mSpacingX", GUILayout.MinWidth(40f));
            NGUIEditorTools.DrawProperty("Y", serializedObject, "mSpacingY", GUILayout.MinWidth(40f));
            GUILayout.Space(18f);
            NGUIEditorTools.SetLabelWidth(80f);
            GUILayout.EndHorizontal();

            NGUIEditorTools.DrawProperty("Max Lines", serializedObject, "mMaxLineCount", GUILayout.Width(110f));

            GUILayout.BeginHorizontal();
            sp = NGUIEditorTools.DrawProperty("BBCode", serializedObject, "mEncoding", GUILayout.Width(100f));
            EditorGUI.BeginDisabledGroup(!sp.boolValue || mLabel.bitmapFont == null || !mLabel.bitmapFont.hasSymbols);
            NGUIEditorTools.SetLabelWidth(60f);
            NGUIEditorTools.DrawPaddedProperty("Symbols", serializedObject, "mSymbols");
            NGUIEditorTools.SetLabelWidth(80f);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
        }
        EditorGUI.EndDisabledGroup();
        return isValid;
    }
}
