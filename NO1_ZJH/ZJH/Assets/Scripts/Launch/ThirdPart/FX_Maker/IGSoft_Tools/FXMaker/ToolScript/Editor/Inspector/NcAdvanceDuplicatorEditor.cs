// ----------------------------------------------------------------------------------
//
// FXMaker Extension
// Created by ¿ÓÕ˙∆Ê
//
// ----------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

[CustomEditor(typeof(NcAdvanceDuplicator))]

public class NcAdvanceDuplicatorEditor : FXMakerEditor
{
    // Attribute ------------------------------------------------------------------------
    protected NcAdvanceDuplicator m_Sel;

    // Property -------------------------------------------------------------------------
    // Event Function -------------------------------------------------------------------
    void OnEnable()
    {
        m_Sel = target as NcAdvanceDuplicator;
        m_UndoManager = new FXMakerUndoManager(m_Sel, "NcSyncDuplicator");
    }

    void OnDisable()
    {
    }

    public override void OnInspectorGUI()
    {
        AddScriptNameField(m_Sel);
        m_UndoManager.CheckUndo();
        // --------------------------------------------------------------
        bool bClickButton = false;
        EditorGUI.BeginChangeCheck();
        {
            //			DrawDefaultInspector();
            m_Sel.m_fUserTag = EditorGUILayout.FloatField(GetCommonContent("m_fUserTag"), m_Sel.m_fUserTag);
            m_Sel.m_bSim = EditorGUILayout.Toggle("Is Simutaniously", m_Sel.m_bSim);
            if (!m_Sel.m_bSim)
                m_Sel.m_fDuplicateTime = EditorGUILayout.FloatField(GetHelpContent("m_fDuplicateTime"), m_Sel.m_fDuplicateTime);
            m_Sel.m_nDuplicateCount = EditorGUILayout.IntField(GetHelpContent("m_nDuplicateCount"), m_Sel.m_nDuplicateCount);
            m_Sel.m_fDuplicateLifeTime = EditorGUILayout.FloatField(GetHelpContent("m_fDuplicateLifeTime"), m_Sel.m_fDuplicateLifeTime);

            EditorGUILayout.Space();
            m_Sel.positionMask = EditorGUILayout.Popup("Position Option", m_Sel.positionMask, new string[] { "Fixed Mode", "Accumulative Mode", "Precise Mode" });
            switch (m_Sel.positionMask)
            {
                case 0:
                    m_Sel.m_AddStartPos = EditorGUILayout.Vector3Field("Specified Position", m_Sel.m_AddStartPos, null);
                    m_Sel.m_TargetPos = null;
                    break;
                case 1:
                    m_Sel.m_AddStartPos = EditorGUILayout.Vector3Field("Accumulated Distance", m_Sel.m_AddStartPos, null);                    
                    m_Sel.m_TargetPos = null;
                    break;
                case 2:
                    if (m_Sel.m_TargetPos == null || m_Sel.m_TargetPos.Length != m_Sel.m_nDuplicateCount)
                        m_Sel.m_TargetPos = new Vector3[m_Sel.m_nDuplicateCount];

                    for (int i = 0; i < m_Sel.m_nDuplicateCount; i++)
                    {
                        EditorGUILayout.LabelField("Instance ", string.Empty + i);
                        m_Sel.m_TargetPos[i] = EditorGUILayout.Vector3Field("Position ", m_Sel.m_TargetPos[i], null);
                    }
                    break;
            }
            EditorGUILayout.Space();

            m_Sel.scaleMask = EditorGUILayout.Popup("Scale Option", m_Sel.scaleMask, new string[] { "Normal Mode", "Accumulative Mode", "Precise Mode" });
            switch (m_Sel.scaleMask)
            {
                case 1:
                    m_Sel.m_AddStartScale = EditorGUILayout.Vector3Field("Accumulated Scale", m_Sel.m_AddStartScale, null);
                    m_Sel.m_TargetScale = null;
                    break;
                case 2:
                    if (m_Sel.m_TargetScale == null || m_Sel.m_TargetScale.Length != m_Sel.m_nDuplicateCount)
                        m_Sel.m_TargetScale = new Vector3[m_Sel.m_nDuplicateCount];

                    for (int i = 0; i < m_Sel.m_nDuplicateCount; i++)
                    {
                        EditorGUILayout.LabelField("Instance ", string.Empty + i);
                        m_Sel.m_TargetScale[i] = EditorGUILayout.Vector3Field("Scale ", m_Sel.m_TargetScale[i], null);                       
                    }
                    break;
            }
            EditorGUILayout.Space();

            m_Sel.m_RandomRange = EditorGUILayout.Vector3Field("Position Random Range", m_Sel.m_RandomRange, null);
            m_Sel.m_AccumStartRot = EditorGUILayout.Vector3Field("Rotation Accumulation", m_Sel.m_AccumStartRot, null);

            SetMinValue(ref m_Sel.m_nDuplicateCount, 0);
            SetMinValue(ref m_Sel.m_fDuplicateLifeTime, 0);

            // err check
            if (GetFXMakerMain())
                if (m_Sel.gameObject == GetFXMakerMain().GetOriginalEffectObject())
                {
                    m_Sel.enabled = false;
                    // 					NgLayout.GUIColorBackup(Color.red);
                    // 					GUILayout.TextArea(GetHsScriptMessage("SCRIPT_ERROR_ROOT", string.Empty), GUILayout.MaxHeight(80));
                    // 					NgLayout.GUIColorRestore();
                }
        }
        m_UndoManager.CheckDirty();
        // --------------------------------------------------------------
        if ((EditorGUI.EndChangeCheck() || bClickButton) && GetFXMakerMain())
            GetFXMakerMain().CreateCurrentInstanceEffect(true);
        // ---------------------------------------------------------------------
        if (GUI.tooltip != string.Empty)
            m_LastTooltip = GUI.tooltip;
        HelpBox(m_LastTooltip);
    }

    // ----------------------------------------------------------------------------------
    // ----------------------------------------------------------------------------------
    protected GUIContent GetHelpContent(string tooltip)
    {
        string caption = tooltip;
        string text = FXMakerTooltip.GetHsEditor_NcDuplicator(tooltip);
        return GetHelpContent(caption, text);
    }

    protected override void HelpBox(string caption)
    {
        string str = caption;
        if (caption == string.Empty || caption == "Script")
            str = FXMakerTooltip.GetHsEditor_NcDuplicator(string.Empty);
        base.HelpBox(str);
    }
}
