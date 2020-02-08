//ORG: Ghostyii & MOONLIGHTGAME
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace SubtitleSystem
{
    public class SubtitleAssetEditorWindow : EditorWindow
    {
        public SubtitleAsset target;

        public bool canAssetChanged = false;

        private bool needInitFormatType = true;
        private bool needInitColor = true;
        private bool formatsFoldout = false;
        private bool subtitlesFoldout = false;
        private Vector2 scrollPosition;
        private SerializedObject serializedObject;

        private List<SubtitleInfoFlag> editorFlags = new List<SubtitleInfoFlag>();

        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);
            titleContent = new GUIContent("Subtitle Asset Editor");
            needInitFormatType = true;            
        }

        private void OnGUI()
        {
            serializedObject.Update();
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            using (new EditorGUI.DisabledScope(!canAssetChanged))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("target"), new GUIContent("Asset"));
                if (canAssetChanged)
                    serializedObject.ApplyModifiedProperties();
            }

            if (target == null)
            {
                EditorGUILayout.HelpBox("Not Subtitle Asset File Selected.", MessageType.Info, true);
                if (GUILayout.Button("Create new subtitle asset"))
                {
                    string newSaPath = EditorUtility.SaveFilePanelInProject("Save", "new subtitle", "asset", string.Empty);
                    if (!string.IsNullOrEmpty(newSaPath))
                    {
                        var subtitleAsset = ScriptableObject.CreateInstance<SubtitleAsset>();
                        AssetDatabase.CreateAsset(subtitleAsset, newSaPath);
                        target = subtitleAsset;
                        AssetDatabase.SaveAssets();
                    }
                }
                EditorGUILayout.EndScrollView();
                return;
            }
            var assetObj = new SerializedObject(serializedObject.FindProperty("target").objectReferenceValue);

            #region Info Area            
            GUILayout.Label("Info", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });

            target.info.Version = EditorGUILayout.TextField(new GUIContent("Version"), target.info.Version);
            GUILayout.Label("Information");
            target.info.Information = GUILayout.TextArea(target.info.Information, GUILayout.Height(60));
            EditorGUILayout.Space();
            #endregion

            #region Format Area         
            Rect rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.grey;
            Handles.DrawLine(new Vector2(rect.x - position.width, rect.y), new Vector2(rect.x + position.width, rect.y));
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Format", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });

            EditorGUILayout.BeginHorizontal();
            target.SetdefaultPosition(EditorGUILayout.Vector2Field("Default Position", target.GetdefaultPosition()));
            if (GUILayout.Button("Set all position to default position", EditorStyles.miniButton))
            {
                for (int i = 0; i < target.subtitles.Count; i++)
                {
                    var item = target.subtitles[i];
                    item.Position = target.DefaultPosition;
                    target.subtitles[i] = item;
                }
                assetObj.ApplyModifiedProperties();
            }
            EditorGUILayout.EndHorizontal();
            target.DefaultPosition = new Point() { X = target.GetdefaultPosition().x, Y = target.GetdefaultPosition().y };

            //EditorGUILayout.BeginHorizontal();            
            //EditorGUILayout.ObjectField(assetObj.FindProperty("defaultFont"), new GUIContent("Default Font"));
            //if (GUILayout.Button("Set all font to default font", EditorStyles.miniButton))
            //{
            //    for (int i = 0; i < target.formats.Count; i++)
            //    {
            //        var item = target.formats[i];
            //        item.FontData.SetFontFormUnityFont(UnityEngine.Font.CreateDynamicFontFromOSFont("Arial", 14));
            //        target.formats[i] = item;
            //    }
            //}
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.PropertyField(assetObj.FindProperty("formats"), new GUIContent("Formats"), true);

            SerializedProperty fmtSp = assetObj.FindProperty("formats");

            //预处理数据使得初始化枚举显示正确，此遍历只需执行一遍
            if (needInitColor)
            {
                for (int i = 0; i < fmtSp.arraySize; i++)
                {
                    var item = fmtSp.GetArrayElementAtIndex(i);
                    var color = item.FindPropertyRelative("color");
                    color.colorValue = new Color(item.FindPropertyRelative("subColor").FindPropertyRelative("r").floatValue, item.FindPropertyRelative("subColor").FindPropertyRelative("g").floatValue, item.FindPropertyRelative("subColor").FindPropertyRelative("b").floatValue, item.FindPropertyRelative("subColor").FindPropertyRelative("a").floatValue);
                }
                needInitColor = !needInitColor;
            }

            formatsFoldout = EditorGUILayout.Foldout(formatsFoldout, "Formats", true);
            //if (EditorGUILayout.PropertyField(fmtSp))
            if (formatsFoldout)
            {
                EditorGUI.indentLevel++;
                if (editorFlags.Count != fmtSp.arraySize)
                {
                    for (int i = editorFlags.Count; i < fmtSp.arraySize; i++)
                    {
                        SubtitleInfoFlag smf = new SubtitleInfoFlag()
                        {
                            fmtFoldout = false,
                            sliderMaxValue = 10
                        };
                        editorFlags.Add(smf);
                    }
                }

                for (int i = 0; i < fmtSp.arraySize; i++)
                {
                    var item = fmtSp.GetArrayElementAtIndex(i);
                    var code = item.FindPropertyRelative("code");

                    EditorGUILayout.BeginHorizontal();
                    editorFlags[i].fmtFoldout = EditorGUILayout.Foldout(editorFlags[i].fmtFoldout, string.IsNullOrEmpty(code.stringValue) ? string.Format("Format {0} [Index:{0}]", i) : string.Format("{0} [Index:{1}]", code.stringValue, i));
                    if (GUILayout.Button(new GUIContent("+", ""), GUILayout.Width(25), GUILayout.Height(15)))
                    {
                        target.formats.Insert(i, new SubtitleFormat()
                        {
                            Color = new SubtitleColor() { A = 1 },
                            FontData = new SubtitleFontData() { FontSize = 14, LineSpacing = 1, Font = new SubtitleFont() { FontName = "Arial" } }
                        });
                        target.formats[i].Fill();
                        assetObj.ApplyModifiedProperties();
                    }
                    if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(15)))
                    {
                        target.formats.RemoveAt(i);
                        assetObj.ApplyModifiedProperties();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;
                    if (editorFlags[i].fmtFoldout)
                    {
                        code.stringValue = EditorGUILayout.TextField("Code", code.stringValue);
                        var fontData = item.FindPropertyRelative("fontData");
                        EditorGUILayout.PropertyField(fontData, new GUIContent("Font Data"), true);
                        var color = item.FindPropertyRelative("color");
                        color.colorValue = EditorGUILayout.ColorField("Color", color.colorValue);
                        var subColor = item.FindPropertyRelative("subColor");

                        //在选定Color后需要设置subColor以供序列化
                        subColor.FindPropertyRelative("r").floatValue = color.colorValue.r;
                        subColor.FindPropertyRelative("g").floatValue = color.colorValue.g;
                        subColor.FindPropertyRelative("b").floatValue = color.colorValue.b;
                        subColor.FindPropertyRelative("a").floatValue = color.colorValue.a;

                        //var material = item.FindPropertyRelative("material");
                        //material.objectReferenceValue = EditorGUILayout.ObjectField("Material", (Material)material.objectReferenceValue, typeof(Material), true);
                        var raycast = item.FindPropertyRelative("raycastTarget");
                        raycast.boolValue = EditorGUILayout.Toggle("Raycast Target", raycast.boolValue);
                    }

                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;

                float buttonWidth = position.width / 3;
                if (GUILayout.Button("Add format", new GUIStyle(EditorStyles.miniButton) { margin = new RectOffset((int)(position.width / 2 - buttonWidth / 2), 0, 0, 0) }, GUILayout.Width(buttonWidth)))
                {
                    target.formats.Add(new SubtitleFormat()
                    {
                        Color = new SubtitleColor() { A = 1 },
                        FontData = new SubtitleFontData() { FontSize = 14, LineSpacing = 1, Font = new SubtitleFont() { FontName = "Arial" } }
                    });
                    assetObj.ApplyModifiedProperties();
                }

            }
            EditorGUILayout.Space();
            #endregion

            #region Subtitle Area
            rect = EditorGUILayout.BeginHorizontal();
            Handles.DrawLine(new Vector2(rect.x - position.width, rect.y), new Vector2(rect.x + position.width, rect.y));
            EditorGUILayout.EndHorizontal();
            GUILayout.Label("Subtitle", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });

            SerializedProperty subSp = assetObj.FindProperty("subtitles");

            subtitlesFoldout = EditorGUILayout.Foldout(subtitlesFoldout, "Subtitles", true);
            //if (EditorGUILayout.PropertyField(subSp))
            if (subtitlesFoldout)
            {
                EditorGUI.indentLevel++;
                if (editorFlags.Count != subSp.arraySize)
                {
                    for (int i = editorFlags.Count; i < subSp.arraySize; i++)
                    {
                        SubtitleInfoFlag smf = new SubtitleInfoFlag()
                        {
                            formatID = 0,
                            subFoldout = false,
                            sliderMaxValue = 10,
                            formatType = FormatType.Index
                        };
                        editorFlags.Add(smf);
                    }
                }

                //预处理数据使得初始化枚举显示正确，此遍历只需执行一遍
                if (needInitFormatType)
                {
                    for (int i = 0; i < subSp.arraySize; i++)
                    {
                        editorFlags[i].formatType = string.IsNullOrEmpty(target.subtitles[i].FormatCode) ? FormatType.Index : FormatType.Code;
                        editorFlags[i].formatID = target.subtitles[i].FormatIndex;
                        editorFlags[i].formatCode = target.subtitles[i].FormatCode;
                    }
                    needInitFormatType = !needInitFormatType;
                }

                for (int i = 0; i < subSp.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    var item = subSp.GetArrayElementAtIndex(i);
                    var positionProp = item.FindPropertyRelative("position");
                    var contentProp = item.FindPropertyRelative("content");

                    var itemLines = contentProp.stringValue.Split('\n');
                    string itemTitle = itemLines[0].Length > 35 ? itemLines[0].Substring(0, 35) + "..." : itemLines[0];
                    if (itemLines.Length > 1 && itemLines[0].Length < 35)
                        itemTitle += "...";

                    editorFlags[i].subFoldout = EditorGUILayout.Foldout(editorFlags[i].subFoldout, string.IsNullOrEmpty(contentProp.stringValue) ? string.Format("Subtitle {0}", i) : itemTitle);
                    if (GUILayout.Button(new GUIContent("+", "Add item in this position"), GUILayout.Width(25), GUILayout.Height(15)))
                    {
                        target.subtitles.Insert(i, new SubtitleInfo());
                        //EditorGUIUtility.hotControl = EditorGUIUtility.GetControlID(new GUIContent(string.Format("Subtitle {0}", i)), FocusType.Passive);
                        assetObj.ApplyModifiedProperties();
                    }
                    if (GUILayout.Button("-", GUILayout.Width(25), GUILayout.Height(15)))
                    {
                        target.subtitles.RemoveAt(i);
                        assetObj.ApplyModifiedProperties();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUI.indentLevel++;
                    if (editorFlags[i].subFoldout)
                    {
                        if (target.subtitles.Count <= i)
                            continue;

                        SubtitleInfo tmpMsg = target.subtitles[i];

                        EditorGUILayout.BeginHorizontal();
                        editorFlags[i].formatType = (FormatType)EditorGUILayout.EnumPopup("Format Type", editorFlags[i].formatType);

                        switch (editorFlags[i].formatType)
                        {
                            case FormatType.Index:
                                editorFlags[i].formatID = EditorGUILayout.IntField(new GUIContent("Format Index"), editorFlags[i].formatID);
                                tmpMsg = target.subtitles[i];
                                tmpMsg.FormatIndex = editorFlags[i].formatID;
                                tmpMsg.FormatCode = string.Empty;
                                target.subtitles[i] = tmpMsg;
                                break;
                            case FormatType.Code:
                                editorFlags[i].formatCode = EditorGUILayout.TextField(new GUIContent("Format Code"), editorFlags[i].formatCode);
                                tmpMsg = target.subtitles[i];
                                tmpMsg.FormatCode = editorFlags[i].formatCode;
                                tmpMsg.FormatIndex = -1;
                                target.subtitles[i] = tmpMsg;
                                break;
                            default:
                                break;
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        positionProp.vector2Value = EditorGUILayout.Vector2Field("Position", positionProp.vector2Value);
                        if (GUILayout.Button("Use default Position", EditorStyles.miniButton, GUILayout.Width(125)))
                        {
                            positionProp.vector2Value = target.GetdefaultPosition();
                            assetObj.ApplyModifiedProperties();
                        }

                        var pointProp = item.FindPropertyRelative("point");
                        pointProp.FindPropertyRelative("x").floatValue = positionProp.vector2Value.x;
                        pointProp.FindPropertyRelative("y").floatValue = positionProp.vector2Value.y;

                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.LabelField("Content");
                        contentProp.stringValue = EditorGUILayout.TextArea(contentProp.stringValue, GUILayout.Height(60));
                        EditorGUILayout.BeginHorizontal();
                        var durationProp = item.FindPropertyRelative("duration");
                        durationProp.floatValue = EditorGUILayout.Slider("Duration", durationProp.floatValue, 0, editorFlags[i].sliderMaxValue);
                        editorFlags[i].sliderMaxValue = Mathf.Abs(EditorGUILayout.FloatField("Max Value", editorFlags[i].sliderMaxValue, EditorStyles.miniTextField));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        var fadeIn = item.FindPropertyRelative("fadeInDuration");
                        fadeIn.floatValue = EditorGUILayout.FloatField("Fade In Duration", fadeIn.floatValue);
                        var fadeOut = item.FindPropertyRelative("fadeOutDuration");
                        fadeOut.floatValue = EditorGUILayout.FloatField("Fade Out Duration", fadeOut.floatValue);
                        var isVertical = item.FindPropertyRelative("isVertical");
                        isVertical.boolValue = EditorGUILayout.Toggle("Vertical Subtitle", isVertical.boolValue);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Load subtitles from file", EditorStyles.miniButtonLeft))
                {
                    int choose = EditorUtility.DisplayDialogComplex("Tip", "Append or Overwrite?", "Append", "Overwrite", "Cancel");
                    if (choose == 0)
                    {
                        string filePath = EditorUtility.OpenFilePanelWithFilters("Select subtitle file (append)", Application.dataPath, new string[] { "Subtitle files", "txt,subs" });

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            string extension = System.IO.Path.GetExtension(filePath).ToLower();
                            string[] lines = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);

                            foreach (var line in lines)
                            {
                                SubtitleInfo tmp = new SubtitleInfo();
                                if (extension.Equals(".subs"))
                                    tmp.InitByFormat(line);
                                else if (extension.Equals(".txt"))
                                    tmp.InitByText(line);
                                target.subtitles.Add(tmp);
                            }

                            needInitFormatType = true;
                            assetObj.ApplyModifiedProperties();
                        }

                    }
                    else if (choose == 1)
                    {
                        string filePath = EditorUtility.OpenFilePanelWithFilters("Select subtitle file (overwrite)", Application.dataPath, new string[] { "Subtitle files", "txt,subs" });

                        if (!string.IsNullOrEmpty(filePath))
                        {
                            string extension = System.IO.Path.GetExtension(filePath).ToLower();
                            string[] lines = System.IO.File.ReadAllLines(filePath, Encoding.UTF8);
                            target.subtitles.Clear();

                            foreach (var line in lines)
                            {
                                SubtitleInfo tmp = new SubtitleInfo();
                                if (extension.Equals(".subs"))
                                    tmp.InitByFormat(line);
                                else if (extension.Equals(".txt"))
                                    tmp.InitByText(line);
                                target.subtitles.Add(tmp);
                            }
                            needInitFormatType = true;
                            assetObj.ApplyModifiedProperties();
                        }
                    }
                }

                if (GUILayout.Button("Add subtitle", subSp.arraySize == 0 ? EditorStyles.miniButtonRight : EditorStyles.miniButtonMid))
                {
                    target.subtitles.Add(new SubtitleInfo());
                    assetObj.ApplyModifiedProperties();
                }

                if (subSp.arraySize > 0)
                {
                    if (GUILayout.Button("Clear Subtitles", EditorStyles.miniButtonMid))
                    {
                        if (EditorUtility.DisplayDialog("Warning", "Are you sure clear all subtitles?", "Yes", "No"))
                        {
                            target.subtitles.Clear();
                            assetObj.ApplyModifiedProperties();
                        }
                    }

                    if (GUILayout.Button("Save subtitles to file", EditorStyles.miniButtonRight))
                    {
                        if (target.subtitles.Count > 0)
                        {
                            string path = EditorUtility.SaveFilePanelInProject("Save", "subtitles", "subs", "Save subtitles to text file");
                            //string extension = System.IO.Path.GetExtension(path);
                            if (!string.IsNullOrEmpty(path))
                            {
                                StringBuilder sb = new StringBuilder();
                                foreach (var item in target.subtitles)
                                    sb.AppendLine(item.ToString());

                                try
                                {
                                    System.IO.File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
                                    if (EditorUtility.DisplayDialog("Success", "Save success.\nDo you want to open it?", "Yes", "No"))
                                        EditorUtility.OpenWithDefaultApp(path);
                                }
                                catch (System.IO.IOException ioe)
                                {
                                    EditorUtility.DisplayDialog("Error", "Save failed.\n" + ioe.Message, "OK");
                                }

                                AssetDatabase.Refresh();
                            }
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();

            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.Space();
            #endregion

            #region Button Area                        
            rect = EditorGUILayout.BeginHorizontal();            
            Handles.DrawLine(new Vector2(rect.x - position.width, rect.y), new Vector2(rect.x + position.width, rect.y));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load asset from file"))
            {
                string filePath = EditorUtility.OpenFilePanelWithFilters("Open Subtitle File", Application.dataPath, new string[] { "Subtitle File", "sa" });

                if (string.IsNullOrEmpty(filePath))
                    return;

                target.LoadFromFile(filePath);
            }
            if (GUILayout.Button("Save to (SA)file"))
            {
                string saveFilename = EditorUtility.SaveFilePanelInProject("Save file", target.name, "sa", "subtitle asset file");

                if (string.IsNullOrEmpty(saveFilename))
                    return;

                target.SaveToFile(saveFilename);
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            assetObj.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
        }

        public void Show(SubtitleAsset asset)
        {
            if (asset != null)
            {
                target = asset;
                editorFlags = new List<SubtitleInfoFlag>(target.subtitles.Count);
            }
            Show();
        }

        [MenuItem("Window/Subtitle Asset Editor")]
        private static void OpenWindow()
        {
            var window = CreateInstance<SubtitleAssetEditorWindow>();
            window.canAssetChanged = true;
            window.Show();
        }

        [System.Serializable]
        private class SubtitleInfoFlag
        {
            public bool fmtFoldout;
            public bool subFoldout;
            public int formatID;
            public string formatCode;
            public float sliderMaxValue;
            public FormatType formatType;
        }

        private enum FormatType
        {
            Index = 0,
            Code = 1
        }
    }
}
