//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
#if UNITY_3_5
[CustomEditor(typeof(UIButton))]
#else
[CustomEditor(typeof(UIButton), true)]
#endif
public class UIButtonEditor : UIButtonColorEditor
{
	enum Highlight
	{
		DoNothing,
		Press,
	}

	protected override void DrawProperties ()
	{
		SerializedProperty sp = serializedObject.FindProperty("dragHighlight");
		Highlight ht = sp.boolValue ? Highlight.Press : Highlight.DoNothing;
		GUILayout.BeginHorizontal();
		bool highlight = (Highlight)EditorGUILayout.EnumPopup("Drag Over", ht) == Highlight.Press;
		GUILayout.Space(18f);
		GUILayout.EndHorizontal();
		if (sp.boolValue != highlight) sp.boolValue = highlight;

        DrawBtnLabel();
		DrawTransition();
		DrawColors();


		UIButton btn = target as UIButton;

		if (btn.tweenTarget != null)
		{
			UISprite sprite = btn.tweenTarget.GetComponent<UISprite>();

			if (sprite != null)
			{
				if (NGUIEditorTools.DrawHeader("Sprites"))
				{
					NGUIEditorTools.BeginContents();
					EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);
					{
						SerializedObject obj = new SerializedObject(sprite);
						obj.Update();
                        SerializedProperty atlas = obj.FindProperty("mstrAtlasName");
						NGUIEditorTools.DrawSpriteField2("Normal", obj, atlas, obj.FindProperty("mSpriteName"));
						obj.ApplyModifiedProperties();

						NGUIEditorTools.DrawSpriteField2("Hover", serializedObject, atlas, serializedObject.FindProperty("hoverSprite"), true);
						NGUIEditorTools.DrawSpriteField2("Pressed", serializedObject, atlas, serializedObject.FindProperty("pressedSprite"), true);
						NGUIEditorTools.DrawSpriteField2("Disabled", serializedObject, atlas, serializedObject.FindProperty("disabledSprite"), true);
					}
					EditorGUI.EndDisabledGroup();

					NGUIEditorTools.DrawProperty("Pixel Snap", serializedObject, "pixelSnap");
					NGUIEditorTools.EndContents();
				}
			}
		}

		UIButton button = target as UIButton;
		NGUIEditorTools.DrawEvents("On Click", button, button.onClick);
	}

    /// <summary>
    /// 绘制BtnLabel
    /// </summary>
    protected void DrawBtnLabel()
    {
        NGUIEditorTools.DrawProperty("Btn Label", serializedObject, "mBtnLabel");
    }
}
