//ORG: Ghostyii & MOONLIGHTGAME
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SubtitleSystem
{
    [CustomPropertyDrawer(typeof(SubtitleFontData))]
    public class FontDataEditor : PropertyDrawer
    {
        private SerializedProperty subtitleFont;
        private SerializedProperty font;
        private SerializedProperty fontStyle;
        private SerializedProperty fontSize;
        private SerializedProperty lineSpacing;
        private SerializedProperty richText;
        private SerializedProperty alignment;
        private SerializedProperty alignByGeometry;
        private SerializedProperty horizontalWrapMode;
        private SerializedProperty verticalWrapMode;
        private SerializedProperty bestFit;

        private void Init(SerializedProperty property)
        {
            subtitleFont = property.FindPropertyRelative("subFont");
            font = property.FindPropertyRelative("font");
            fontStyle = property.FindPropertyRelative("fontStyle");
            fontSize = property.FindPropertyRelative("fontSize");
            lineSpacing = property.FindPropertyRelative("lineSpacing");
            richText = property.FindPropertyRelative("richText");
            alignment = property.FindPropertyRelative("alignment");
            alignByGeometry = property.FindPropertyRelative("alignByGeometry");
            horizontalWrapMode = property.FindPropertyRelative("horizontalWrapMode");
            verticalWrapMode = property.FindPropertyRelative("verticalWrapMode");
            bestFit = property.FindPropertyRelative("bestFit");
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);
            EditorGUI.LabelField(position, "Font Data", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            font.objectReferenceValue = EditorGUILayout.ObjectField("Font", font.objectReferenceValue, typeof(Font), true);

            //为了减少字幕系统保存所需要的数据大小，对于Font类型只保存其字体名称
            //因为YamlDotNet保存会对UnityEngine.Font进行递归保存需要占用大量空间
            subtitleFont.FindPropertyRelative("fontName").stringValue = (Font)font.objectReferenceValue == null ?
                string.Empty : ((Font)font.objectReferenceValue).name;

            fontStyle.enumValueIndex = (int)(FontStyle)EditorGUILayout.EnumPopup("Font Style", (FontStyle)fontStyle.enumValueIndex);
            fontSize.intValue = EditorGUILayout.IntField("Font Size", fontSize.intValue);
            lineSpacing.floatValue = EditorGUILayout.FloatField("Line Spacing", lineSpacing.floatValue);
            richText.boolValue = EditorGUILayout.Toggle("Rich Text", richText.boolValue);
            alignment.enumValueIndex = (int)(TextAnchor)EditorGUILayout.EnumPopup("Alignment", (TextAnchor)alignment.enumValueIndex);
            horizontalWrapMode.enumValueIndex = (int)(HorizontalWrapMode)EditorGUILayout.EnumPopup("HorizontalWrapMode", (HorizontalWrapMode)horizontalWrapMode.enumValueIndex);
            verticalWrapMode.enumValueIndex = (int)(VerticalWrapMode)EditorGUILayout.EnumPopup("VerticalWrapMode", (VerticalWrapMode)verticalWrapMode.enumValueIndex);
            bestFit.boolValue = EditorGUILayout.Toggle("Best Fit", bestFit.boolValue);
            EditorGUI.indentLevel--;
        }
    }
}
