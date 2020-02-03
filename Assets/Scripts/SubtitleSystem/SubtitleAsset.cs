//ORG: Ghostyii & MOONLIGHTGAME
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Utility;

namespace SubtitleSystem
{
    [CreateAssetMenu(fileName = "new subtitle", menuName = "Subtitle Asset")]
    public class SubtitleAsset : ScriptableObject
    {
        public SubtitleAssetInfo info = new SubtitleAssetInfo() { Version = "1.0" };
        public List<SubtitleFormat> formats = new List<SubtitleFormat>();
        public List<SubtitleInfo> subtitles = new List<SubtitleInfo>();

        [SerializeField]
        private Vector2 defaultPosition = Vector2.zero;

        [SerializeField]
        private Point position;
        public Point DefaultPosition { get => position; set => position = value; }

        public void LoadFromFile(string filename, bool showDialog = true)
        {
            if (!Path.GetExtension(filename).Equals(".sa"))
                return;

            SubtitleAsset sam = CreateInstance<SubtitleAsset>();
            sam = YamlHelper.FromYamlFile<SubtitleAsset>(filename);
            info = sam.info;
            formats = sam.formats;
            foreach (var item in formats)
            {
                item.Fill();                
            }

            subtitles = sam.subtitles;
            foreach (var item in subtitles)
            {
                item.Fill();
            }
            defaultPosition = new Vector2(sam.position.X, sam.position.Y);

#if UNITY_EDITOR
            if (showDialog)
                EditorUtility.DisplayDialog("Tip", "Load success!", "OK");
#endif
        }

        public void SaveToFile(string filename)
        {

            string yaml = YamlHelper.ToYaml(this);
            File.WriteAllText(filename, yaml);
            EditorUtility.DisplayDialog("Tip", "Save completed.", "OK");
            AssetDatabase.Refresh();

        }

        public Vector2 GetdefaultPosition()
        { return defaultPosition; }
        public void SetdefaultPosition(Vector2 v)
        { defaultPosition = v; }

        public SubtitleFormat GetFormatByMsg(SubtitleInfo msg)
        {
            if (msg.FormatIndex >= 0)
                return formats[msg.FormatIndex];
            else
                return formats.Find(x => x.Code.Equals(msg.FormatCode));
        }
    }
}