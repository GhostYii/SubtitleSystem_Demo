//ORG: Ghostyii & MOONLIGHTGAME
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SubtitleSystem
{
    [CustomEditor(typeof(SubtitleAsset))]
    public class SubtitleAssetEditor : Editor
    {
        private SubtitleAsset asset = null;

        private void OnEnable()
        {
            if (asset == null)
                asset = target as SubtitleAsset;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Brief attribute", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Info", EditorStyles.miniBoldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(string.Format("Version: {0}", asset.info.Version));
            EditorGUILayout.LabelField("Information:");
            EditorGUILayout.LabelField(asset.info.Information, EditorStyles.textArea);
            EditorGUI.indentLevel--;

            EditorGUILayout.LabelField("Statistics", EditorStyles.miniBoldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(string.Format("Format count: {0}", asset.formats.Count));
            EditorGUILayout.LabelField(string.Format("Subtitle count: {0}", asset.subtitles.Count));
            EditorGUI.indentLevel--;

            EditorGUI.indentLevel--;


            if (GUILayout.Button("Load from file"))
            {
                string filePath = EditorUtility.OpenFilePanelWithFilters("Open Subtitle File", Application.dataPath, new string[] { "Subtitle File", "sa" });

                if (string.IsNullOrEmpty(filePath))
                    return;

                asset.LoadFromFile(filePath);
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Save to (SA)file"))
            {
                string saveFilename = EditorUtility.SaveFilePanelInProject("Save file", target.name, "sa", "subtitle asset file");

                if (string.IsNullOrEmpty(saveFilename))
                    return;

                asset.SaveToFile(saveFilename);
            }

            if (GUILayout.Button("Open subtitle asset editor"))
            {
                CreateInstance<SubtitleAssetEditorWindow>().Show(asset);
            }
        }
    }
}