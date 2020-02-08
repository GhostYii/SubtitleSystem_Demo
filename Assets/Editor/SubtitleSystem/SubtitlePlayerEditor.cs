//ORG: Ghostyii & MOONLIGHTGAME
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using SubtitleSystem;

[CustomEditor(typeof(SubtitleAssetPlayer))]
public class SubtitlePlayerEditor : Editor
{
    private string prefsKey = "eventsFoldout";
    private SubtitleAssetPlayer script = null;

    private void OnEnable()
    {
        if (script == null)
            script = target as SubtitleAssetPlayer;        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("subtitleAsset"), new GUIContent("Subtitle Asset"));
        using (new EditorGUI.DisabledScope(script.subtitleAsset == null))
        {
            if (GUILayout.Button("Edit", EditorStyles.miniButtonRight))
            {
                CreateInstance<SubtitleAssetEditorWindow>().Show(script.subtitleAsset);
            }
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("text"), new GUIContent("Text"));

        EditorGUILayout.BeginHorizontal();
        bool buttonGroupDisabled = serializedObject.FindProperty("subtitleAsset").objectReferenceValue == null || !Application.isPlaying;
        using (new EditorGUI.DisabledGroupScope(buttonGroupDisabled))
        {
            if (GUILayout.Button("Play", EditorStyles.miniButtonLeft))
            {
                script.Play();
            }
            if (GUILayout.Button("Pause", EditorStyles.miniButtonMid))
            {
                script.Pause();
            }
            if (GUILayout.Button("Stop", EditorStyles.miniButtonRight))
            {
                script.Stop();
            }
        }
        EditorGUILayout.EndHorizontal();


        if (!Application.isPlaying)
            EditorGUILayout.HelpBox("Play control is only available in run mode", MessageType.Info);
        else
            EditorGUI.ProgressBar(GUILayoutUtility.GetRect(80, 20), script.currentProgress, string.IsNullOrEmpty(script.currentSubtileContent) ? "no subtitle playing" : script.currentSubtileContent);

        EditorPrefs.SetBool(prefsKey, EditorGUILayout.Foldout(EditorPrefs.GetBool(prefsKey, false), "Events", true));
        if (EditorPrefs.GetBool(prefsKey, false))
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onPlay"), new GUIContent("OnPlay"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onPause"), new GUIContent("OnPause"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onStop"), new GUIContent("OnStop"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onComplete"), new GUIContent("OnComplete"));
        }


        serializedObject.ApplyModifiedProperties();
        //Repaint();
    }

}
