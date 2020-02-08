//ORG: Ghostyii & MOONLIGHTGAME
using System;
using UnityEngine;
using FitMode = UnityEngine.UI.ContentSizeFitter.FitMode;

namespace SubtitleSystem
{
    //字幕显示方向
    public enum SubtitleDirect
    {
        Horizontal,
        Vertical
    }

    [Serializable]
    public struct SubtitleFormat
    {
        [SerializeField]
        private string code;
        [SerializeField]
        private SubtitleFontData fontData;
        [SerializeField]
        private SubtitleColor subColor;
        [SerializeField]
        private Color color;
        //[SerializeField]
        //private Material material;
        [SerializeField]
        private bool raycastTarget;

        public string Code { get => code; set => code = value; }
        public SubtitleFontData FontData { get => fontData; set => fontData = value; }
        public SubtitleColor Color
        {
            get => subColor;
            set
            {
                subColor = value;
                color = new Color(value.R, value.G, value.B, value.A);
            }
        }
        //public Material Material { get => material; set => material = value; }
        public bool RaycastTarget { get => raycastTarget; set => raycastTarget = value; }

        #region 备用        
        public Color GetUnityColor()
        {
            return new Color(subColor.R, subColor.G, subColor.B, subColor.A);
        }

        public void SetColorFormUnityColor(Color color)
        {
            subColor.R = color.r;
            subColor.G = color.g;
            subColor.B = color.b;
            subColor.A = color.a;
        }

        public void Fill()
        {
            color = GetUnityColor();
            //Debug.Log(color == null);
            fontData.Fill();
            //material = fontData.GetUnityFont().material;
        }
        #endregion

        //public Material GetMaterial()
        //{ return material; }
    }

    [Serializable]
    public struct SubtitleInfo
    {
        //public SubtitleFormat format;
        [SerializeField]
        private int formatIndex;
        [SerializeField]
        private string formatCode;
        [SerializeField]
        private Vector2 position;
        [SerializeField]
        private Point point;
        [SerializeField]
        private string content;
        [SerializeField]
        private float duration;
        [SerializeField]
        private bool isVertical;
        [SerializeField]
        private float fadeInDuration;
        [SerializeField]
        private float fadeOutDuration;

        public int FormatIndex { get => formatIndex; set => formatIndex = value; }
        public string FormatCode { get => formatCode; set => formatCode = value; }
        public Point Position
        {
            get => point;
            set
            {
                point = value;
                position.x = value.X;
                position.y = value.Y;
            }
        }
        public string Content { get => content; set => content = value; }
        public float Duration { get => duration; set => duration = value; }
        public bool IsVertical { get => isVertical; set => isVertical = value; }
        public float FadeInDuration { get => fadeInDuration; set => fadeInDuration = value; }
        public float FadeOutDuration { get => fadeOutDuration; set => fadeOutDuration = value; }


        #region 备用
        public Vector2 GetVector2()
        {
            return new Vector2(point.X, point.Y);
        }

        public void SetPointFromVector2(Vector2 v)
        {
            point.X = v.x;
            point.Y = v.y;
        }

        public void Fill()
        {
            position = GetVector2();
        }
        #endregion

        //用于单独保存字幕信息
        public override string ToString()
        {
            return string.Format("\"{0}\"|\"{1}\"|\"{2}\"|\"{3}\"|\"{4}\"|\"{5}\"|\"{6}\"|\"{7}\"", content.Replace("\n", "\\n"), formatIndex, formatCode, duration, fadeInDuration, fadeOutDuration, isVertical, position.ToString());
        }

        //通过ToString生成的字符串初始化
        public void InitByFormat(string format)
        {
            if (string.IsNullOrEmpty(format))
                return;

            string[] parts = format.Split('|');
            if (parts.Length != 8)
                return;

            content = parts[0].Substring(1, parts[0].Length - 2).Replace("\\n", "\n");
            int tmpInt = 0;
            formatIndex = int.TryParse(parts[1].Substring(1, parts[1].Length - 2), out tmpInt) ? tmpInt : 0;
            formatCode = parts[2].Substring(1, parts[2].Length - 2);
            float tmpFloat = 2f;
            duration = float.TryParse(parts[3].Substring(1, parts[3].Length - 2), out tmpFloat) ? tmpFloat : 2f;
            float tmpFadeIn = 0, tmpFadeOut = 0;
            fadeInDuration = float.TryParse(parts[4].Substring(1, parts[4].Length - 2), out tmpFadeIn) ? tmpFadeIn : 0;
            fadeOutDuration = float.TryParse(parts[5].Substring(1, parts[5].Length - 2), out tmpFadeOut) ? tmpFadeOut : 0;

            bool vtc = bool.TryParse(parts[6].Substring(1, parts[6].Length - 2), out vtc) ? vtc : false;

            string[] vectors = parts[7].Substring(1, parts[7].Length - 2).Split(',');
            Position = new Point()
            {
                X = float.Parse(vectors[0].Replace("(", string.Empty)),
                Y = float.Parse(vectors[1].Replace(")", string.Empty))
            };

        }

        //仅指定内容以初始化
        public void InitByText(string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return;

            content = txt;
            formatIndex = 0;
            FormatCode = string.Empty;
            duration = 2f;
            Position = new Point() { X = 0, Y = 0 };
        }
    }

    [Serializable]
    public struct SubtitleAssetInfo
    {
        [SerializeField]
        private string version;
        [SerializeField]
        private string information;

        public string Version { get => version; set => version = value; }
        public string Information { get => information; set => information = value; }
    }

    [Serializable]
    public class SubtitleFontData
    {
        [SerializeField]
        private SubtitleFont subFont;
        [SerializeField]
        private Font font;
        [SerializeField]
        private FontStyle fontStyle = FontStyle.Normal;
        [SerializeField]
        private int fontSize = 14;
        [SerializeField]
        private float lineSpacing = 1f;
        [SerializeField]
        private bool richText = true;
        [SerializeField]
        private TextAnchor alignment = TextAnchor.UpperLeft;
        [SerializeField]
        private bool alignByGeometry;
        [SerializeField]
        private HorizontalWrapMode horizontalWrapMode = HorizontalWrapMode.Wrap;
        [SerializeField]
        private VerticalWrapMode verticalWrapMode = VerticalWrapMode.Truncate;
        [SerializeField]
        private bool bestFit = false;

        public SubtitleFont Font
        {
            get => subFont;
            set
            {
                subFont = value;
                font = GetUnityFont();
            }
        }
        public FontStyle FontStyle { get => fontStyle; set => fontStyle = value; }
        public int FontSize { get => fontSize; set => fontSize = value; }
        public float LineSpacing { get => lineSpacing; set => lineSpacing = value; }
        public bool RichText { get => richText; set => richText = value; }
        public TextAnchor Alignment { get => alignment; set => alignment = value; }
        public bool AlignByGeometry { get => alignByGeometry; set => alignByGeometry = value; }
        public HorizontalWrapMode HorizontalWrapMode { get => horizontalWrapMode; set => horizontalWrapMode = value; }
        public VerticalWrapMode VerticalWrapMode { get => verticalWrapMode; set => verticalWrapMode = value; }
        public bool BestFit { get => bestFit; set => bestFit = value; }

        #region 备用        
        public Font GetUnityFont()
        {
            if (!string.IsNullOrEmpty(subFont.FontName))
                return SubtitleUtility.GetFontByFontName(subFont.FontName);
            else
                return new Font();
        }

        public void SetFontFormUnityFont(Font font)
        {
            subFont.FontName = font.name;
        }

        public void Fill()
        {
            font = GetUnityFont();
        }
        #endregion

        //public static FontData GetFontDataByUnityFontData(UnityEngine.UI.FontData fontData)
        //{
        //    FontData fd = new FontData()
        //    {
        //        subFont = new Font() { FontName = fontData.font.name },
        //        font = fontData.font,
        //        fontStyle = fontData.fontStyle,
        //        fontSize = fontData.fontSize,
        //        lineSpacing = fontData.lineSpacing,
        //        richText = fontData.richText,
        //        alignment = fontData.alignment,
        //        alignByGeometry = fontData.alignByGeometry,
        //        horizontalWrapMode = fontData.horizontalOverflow,
        //        verticalWrapMode = fontData.verticalOverflow,
        //        bestFit = fontData.bestFit
        //    };
        //    return fd;
        //}

        //public FontData defaultFontData { get => GetFontDataByUnityFontData(UnityEngine.UI.FontData.defaultFontData); }
    }

    #region 用于字幕系统减少存储空间以及以防YamlDotNet序列化失败
    [Serializable]
    public struct SubtitleColor
    {
        [SerializeField]
        private float r;
        [SerializeField]
        private float g;
        [SerializeField]
        private float b;
        [SerializeField]
        private float a;

        public float R { get => r; set => r = value; }
        public float G { get => g; set => g = value; }
        public float B { get => b; set => b = value; }
        public float A { get => a; set => a = value; }
    }

    [Serializable]
    public struct SubtitleFont
    {
        [SerializeField]
        private string fontName;

        public string FontName { get => fontName; set => fontName = value; }
    }

    [Serializable]
    public struct Point
    {
        [SerializeField]
        private float x;
        [SerializeField]
        private float y;

        public float X { get => x; set => x = value; }
        public float Y { get => y; set => y = value; }
    }
    #endregion

    [Serializable]
    public struct SubtitleMsg
    {
        public string content;
        public Vector3 position;
        public int fontSize;
        public Color color;
        public string fontName;
        public float duration;
        public FitMode horizontalFit;
        public FitMode verticalFit;
    }

    ////调用显示字幕的方法委托
    //public delegate Text SubtitleShowFunc();
}