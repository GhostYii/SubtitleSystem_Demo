//本文件来源：https://blog.csdn.net/qq_33337811/article/details/77099001
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace SubtitleSystem
{
    [CustomEditor(typeof(DefaultAsset))]
    public class SaAssetEditor : Editor
    {
        private Editor editor;
        private static Type[] customAssetTypes;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            customAssetTypes = GetCustomAssetTypes();
        }

        public override void OnInspectorGUI()
        {
            if (editor != null)
            {
                GUI.enabled = true;
                editor.OnInspectorGUI();
            }
        }

        private void OnEnable()
        {
            var assetPath = AssetDatabase.GetAssetPath(target);
            var extension = Path.GetExtension(assetPath);

            if (string.IsNullOrEmpty(extension))
                return;

            var customAssetEditorType = GetCustomAssetEditorType(extension);

            if (customAssetEditorType.FullName.Equals("UnityEditor.DefaultAsset"))
                return;

            editor = CreateEditor(target, customAssetEditorType);
        }

        private static Type[] GetCustomAssetTypes()
        {
            var assemblyPaths = Directory.GetFiles("Library/ScriptAssemblies", "*.dll");
            var types = new List<Type>();
            var customAssetTypes = new List<Type>();

            foreach (var assembly in assemblyPaths.Select(assemblyPath => Assembly.LoadFile(assemblyPath)))
            {
                types.AddRange(assembly.GetTypes());
            }

            foreach (var type in types)
            {
                var customAttributes = type.GetCustomAttributes(typeof(CustomAssetAttribute), false) as CustomAssetAttribute[];

                if (0 < customAttributes.Length)
                    customAssetTypes.Add(type);
            }

            return customAssetTypes.ToArray();
        }

        private Type GetCustomAssetEditorType(string extension)
        {
            foreach (var type in customAssetTypes)
            {
                var customAttributes = type.GetCustomAttributes(typeof(CustomAssetAttribute), false) as CustomAssetAttribute[];

                foreach (var customAttribute in customAttributes)
                {
                    if (customAttribute.extension.Equals(extension))
                        return type;
                }
            }
            return typeof(DefaultAsset);
        }

        public override bool HasPreviewGUI()
        {
            return false;
        }
    }

    [CustomAsset(".sa")]
    public class SaInspector : Editor
    {
        private SubtitleAsset asset = null;

        private void OnEnable()
        {
            if (asset == null)
            {
                asset = CreateInstance<SubtitleAsset>();
                asset.LoadFromFile(AssetDatabase.GetAssetPath(target), false);
            }
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

            if (GUILayout.Button("Create subtitle asset by this file"))
            {
                SubtitleAsset sa = CreateInstance<SubtitleAsset>();
                sa.LoadFromFile(AssetDatabase.GetAssetPath(target), false);
                string savePath = EditorUtility.SaveFilePanelInProject("Create Subtitle", target.name, "asset", string.Empty);
                if (!string.IsNullOrEmpty(savePath))
                {
                    AssetDatabase.CreateAsset(sa, savePath);
                    AssetDatabase.Refresh();
                }
            }
        }
    }

    [CustomAsset(".subs")]
    public class SubsInspector : Editor
    {
        private List<SubtitleInfo> msgs = null;

        private void OnEnable()
        {
            if (msgs == null)
            {
                msgs = new List<SubtitleInfo>();
                string[] lines = File.ReadAllLines(AssetDatabase.GetAssetPath(target));
                foreach (var line in lines)
                {
                    SubtitleInfo msg = new SubtitleInfo();
                    msg.InitByFormat(line);
                    msgs.Add(msg);
                }
            }                
        }

        public override void OnInspectorGUI()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var msg in msgs)            
                sb.AppendLine(msg.Content);

            EditorGUILayout.TextArea(sb.ToString());
        }
    }

    public class CustomAssetAttribute : Attribute
    {
        public string extension;

        public CustomAssetAttribute(string ex)
        {
            extension = ex;
        }
    }

}
