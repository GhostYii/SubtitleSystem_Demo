//ORG: Ghostyii & MOONLIGHTGAME
using DG.Tweening;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SubtitleSystem
{
    [DisallowMultipleComponent]
    public class SubtitleManager : Singleton<SubtitleManager>
    {
        public bool dontDestory = true;
        public GameObject canvasPrefab;

        private GameObject canvasRoot;
        public GameObject CanvasRoot { get => canvasRoot; }

        private void Awake()
        {
            if (dontDestory)
                DontDestroyOnLoad(gameObject);
            CreateSubtitleCanvas();
        }

        public void CreateSubtitleCanvas()
        {
            //此处不判canvas是否为空
            if (canvasRoot == null)
            {
                canvasRoot = Instantiate<GameObject>(canvasPrefab);
                var cs = canvasRoot.GetComponent<CanvasScaler>();
                cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                cs.referenceResolution = Screen.safeArea.size;                
            }
            DontDestroyOnLoad(canvasRoot);
        }

        public Text CreateText(Vector3 position, int fontSize, Color color, string fontName = "Arial")
        {
            GameObject textObj = new GameObject("SubtitleText", typeof(Text), typeof(ContentSizeFitter));
            textObj.transform.parent = canvasRoot.transform;
            textObj.transform.localPosition = position;

            Text text = textObj.GetComponent<Text>();
            if (!string.IsNullOrEmpty(fontName))
                text.font = SubtitleUtility.GetFontByFontName(fontName);
            else
                text.font = SubtitleUtility.GetFontByFontName("Arial");
            text.fontSize = fontSize;
            text.color = color;
            var sizeFitter = text.gameObject.GetComponent<ContentSizeFitter>();
            sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return text;
        }

        //显示字幕，duration<=0时该字幕会永久显示
        public Subtitle Show(string content, float duration = 0, bool playOnCreate = true)
        {
            return Show(content, new Vector3(0, -190f, 0), 14, Color.black, string.Empty, duration, playOnCreate);
        }
        public Subtitle Show(string content, string fontName, float duration, bool playOnCreate = true)
        {
            return Show(content, new Vector3(0, -190f, 0), 14, Color.black, fontName, duration, playOnCreate);
        }
        public Subtitle Show(string content, Vector3 position, float duration, bool playOnCreate = true)
        {
            return Show(content, position, 14, Color.black, duration, playOnCreate);
        }
        public Subtitle Show(string content, Color color, float duration, bool playOnCreate = true)
        {
            return Show(content, new Vector3(0, -190f, 0), 14, color, duration, playOnCreate);
        }
        public Subtitle Show(string content, Vector3 position, int fontSize, float duration, bool playOnCreate = true)
        {
            return Show(content, position, fontSize, Color.black, duration, playOnCreate);
        }
        public Subtitle Show(string content, Vector3 position, int fontSize, Color color, float duration, bool playOnCreate = true)
        {
            return Show(content, position, fontSize, color, string.Empty, duration, playOnCreate);
        }
        public Subtitle Show(string content, Vector3 position, int fontSize, Color color, string fontName, float duration, bool playOnCreate = true)
        {
            Subtitle sub = new Subtitle(content, position, fontSize, color, fontName, duration);
            sub.SetDirect(SubtitleDirect.Horizontal);
            //sub.OnComplete.AddListener(() => { if (!sub.Text.IsDestroyed()) Destroy(sub.Text.gameObject); });
            if (playOnCreate)
                sub.Play(content, position, fontSize, color, fontName, duration);

            return sub;
        }
        public Subtitle Show(SubtitleInfo info, bool playOnCreate = true)
        {
            Subtitle sub = new Subtitle();
            sub.Text.gameObject.transform.localPosition = info.GetVector2();

            sub.Text.text = info.Content;
            sub.OnComplete.AddListener(() => Destroy(sub.Text.gameObject));
            sub.Play(info.Content, info.GetVector2(), 14, Color.black, "Arial", info.Duration);

            return sub;
        }
        public Subtitle Show(SubtitleInfo info, SubtitleFormat format, bool playOnCreate = true)
        {
            Subtitle sub = new Subtitle();
            sub.Text.gameObject.transform.localPosition = info.GetVector2();

            sub.Text.font = format.FontData.GetUnityFont();
            sub.Text.fontStyle = format.FontData.FontStyle;
            sub.Text.fontSize = format.FontData.FontSize;
            sub.Text.lineSpacing = format.FontData.LineSpacing;
            sub.Text.supportRichText = format.FontData.RichText;
            sub.Text.alignment = format.FontData.Alignment;
            sub.Text.alignByGeometry = format.FontData.AlignByGeometry;
            sub.Text.horizontalOverflow = format.FontData.HorizontalWrapMode;
            sub.Text.verticalOverflow = format.FontData.VerticalWrapMode;
            sub.Text.resizeTextForBestFit = format.FontData.BestFit;
            sub.Text.color = format.GetUnityColor();
            //text.material = format.GetMaterial();
            sub.Text.raycastTarget = format.RaycastTarget;


            sub.Text.text = info.Content;
            sub.OnComplete.AddListener(() => { if (!sub.Text.IsDestroyed()) Destroy(sub.Text.gameObject); });
            sub.Play(info.Content, info.GetVector2(), format.FontData.FontSize, format.GetUnityColor(), format.FontData.Font.FontName, info.Duration);

            return sub;
        }

        //显示竖排字幕，duration<=0时该字幕会永久显示
        public Subtitle ShowVertical(string content, float duration = 0, bool playOnCreate = true)
        {
            return ShowVertical(content, new Vector3(0, -190f, 0), 14, Color.black, duration, playOnCreate);
        }
        public Subtitle ShowVertical(string content, string fontName, float duration, bool playOnCreate = true)
        {
            return ShowVertical(content, new Vector3(0, -190f, 0), 14, Color.black, fontName, duration, playOnCreate);
        }
        public Subtitle ShowVertical(string content, Vector3 position, float duration, bool playOnCreate = true)
        {
            return ShowVertical(content, position, 14, Color.black, duration, playOnCreate);
        }
        public Subtitle ShowVertical(string content, Color color, float duration, bool playOnCreate = true)
        {
            return ShowVertical(content, new Vector3(0, -190f, 0), 14, color, duration, playOnCreate);
        }
        public Subtitle ShowVertical(string content, Vector3 position, int fontSize, float duration, bool playOnCreate = true)
        {
            return ShowVertical(content, position, fontSize, Color.black, duration, playOnCreate);
        }
        public Subtitle ShowVertical(string content, Vector3 position, int fontSize, Color color, float duration, bool playOnCreate = true)
        {
            return ShowVertical(content, position, fontSize, color, string.Empty, duration, playOnCreate);
        }
        public Subtitle ShowVertical(string content, Vector3 position, int fontSize, Color color, string fontName, float duration, bool playOnCreate = true)
        {
            Subtitle sub = new Subtitle(content, position, fontSize, color, fontName, duration);
            sub.SetDirect(SubtitleDirect.Vertical);
            //sub.OnComplete.AddListener(() => { if (!sub.Text.IsDestroyed()) Destroy(sub.Text.gameObject); });
            if (playOnCreate)
                sub.Play(content, duration);

            return sub;
        }

        //显示淡入淡出字幕
        public Subtitle ShowWithFade(string content, Vector3 position, int fontSize, Color color, float duration, float fadeInDuration, float fadeOutDuration, string fontName = "Arial", bool playOnCreate = true)
        {
            string rawContent = content;
            content = fadeInDuration > 0 ? string.Empty : content;
            Subtitle sub = new Subtitle(content, position, fontSize, color, fontName, duration + fadeInDuration + fadeOutDuration);

            sub.OnPlay = (t, d) =>
            {
                if (fadeInDuration > 0)
                {
                    t.color = new Color(t.color.r, t.color.g, t.color.b, 0);
                    t.text = rawContent;
                }
                t.DOColor(new Color(t.color.r, t.color.g, t.color.b, 1), fadeInDuration)
                .OnComplete(() =>
                {
                    float tmp = 0;
                    DOTween.To(() => tmp, (x) => tmp = x, 0, duration)
                    .OnComplete(() =>
                    {
                        t.DOColor(new Color(t.color.r, t.color.g, t.color.b, 0), fadeOutDuration)
                        .OnComplete(() => t.color = new Color(t.color.r, t.color.g, t.color.b, 1));
                    });
                });
            };

            sub.OnComplete.AddListener(() => { if (!sub.Text.IsDestroyed()) Destroy(sub.Text.gameObject); });
            if (playOnCreate)
                sub.Play(content, position, fontSize, color, fontName, duration + fadeInDuration + fadeOutDuration);

            return sub;
        }

        //显示带震动效果的字幕
        public Subtitle ShowWithShakePosition(string content, Vector3 position, int fontSize, Color color, float duration, float intensity, bool fadeOut = false, string fontName = "Arial", bool playOnCreate = true)
        {
            Subtitle sub = new Subtitle(content, position, fontSize, color, fontName, duration);
            sub.OnPlay = (t, f) =>
            {                
                t.transform.DOShakePosition(duration, intensity, 10, 90, false, fadeOut)
                .OnComplete(() => { if (!t.IsDestroyed()) Destroy(t.gameObject); });
            };
            if (playOnCreate)
                sub.Play(content, position, fontSize, color, fontName, duration);

            return sub;
        }
        public Subtitle ShowWithShakeRotation(string content, Vector3 position, int fontSize, Color color, float duration, float intensity, bool fadeOut = false, string fontName = "Arial", bool playOnCreate = true)
        {
            Subtitle sub = new Subtitle(content, position, fontSize, color, fontName, duration);
            sub.OnPlay = (t, f) =>
            {
                t.transform.DOShakeRotation(duration, intensity, 10, 90, fadeOut)
                .OnComplete(() => { if (!t.IsDestroyed()) Destroy(t.gameObject); });
            };
            if (playOnCreate)
                sub.Play(content, position, fontSize, color, fontName, duration);

            return sub;
        }
        public Subtitle ShowWithShakeScale(string content, Vector3 position, int fontSize, Color color, float duration, float intensity, bool fadeOut = false, string fontName = "Arial", bool playOnCreate = true)
        {
            Subtitle sub = new Subtitle(content, position, fontSize, color, fontName, duration);
            sub.OnPlay = (t, f) =>
            {
                t.transform.DOShakeScale(duration, intensity, 10, 90, fadeOut)
                .OnComplete(() => { if (!t.IsDestroyed()) Destroy(t.gameObject); });
            };
            if (playOnCreate)
                sub.Play(content, position, fontSize, color, fontName, duration);

            return sub;
        }

        //显示打字机效果字幕
        public Subtitle ShowWithTypewriter(string content, Vector3 position, int fontSize, Color color, float duration, float interval, string fontName = "Arial", bool playOnCreate = true)
        {
            float durationSum = duration + interval * content.Length;
            Subtitle sub = new Subtitle(string.Empty, position, fontSize, color, fontName, durationSum);
            sub.OnPlay = (t, f) =>
            {
                t.text = string.Empty;
                t.DOText(content, interval * content.Length).SetEase(Ease.Linear);
            };
            sub.OnComplete.AddListener(() =>
            {
                if (!sub.Text.IsDestroyed())
                    Destroy(sub.Text.gameObject);
            });
            if (playOnCreate)
                sub.Play(string.Empty, position, fontSize, color, fontName, durationSum);

            return sub;
        }

        //显示自定义效果字幕
        //onPlay(Text text, float duration)
        //onUpdate(Text text, float currentProgress)
        public Subtitle ShowWithCustom(string content, Vector3 position, int fontSize, Color color, float duration,Action<Text, float> onPlay, Action<Text, float> onUpdate, Action onComplete, string fontName = "Arial", bool playOnCreate = true)
        {
            Subtitle sub = new Subtitle(content, position, fontSize, color, fontName, duration);

            sub.OnPlay = onPlay;
            sub.OnUpdate = onUpdate;
            if (onComplete != null)
                sub.OnComplete.AddListener(() => onComplete());
            sub.OnComplete.AddListener(() => { if (!sub.Text.IsDestroyed()) Destroy(sub.Text.gameObject); });
            if (playOnCreate)
                sub.Play(content, position, fontSize, color, fontName, duration);
            return sub;
        }

        //淡出Text文字
        private void TextFadeOut(Text text, float duration)
        {
            text.CrossFadeAlpha(0f, duration, false);
            SubtitleUtility.WaitSecondsForSomething(() =>
            {
                if (!text.IsDestroyed())
                    Destroy(text.gameObject);
            }, duration);

        }

        //打字机效果，使用协程实现，不可暂停
        public IEnumerator TypeWriterCoroutine(Text text, string endValue, float interval)
        {
            int count = 1;
            while (count <= endValue.Length)
            {
                string value = endValue.Substring(0, count);
                text.text = value;
                yield return new WaitForSeconds(interval);
                count++;
            }
        }
    }

    [CustomEditor(typeof(SubtitleManager))]
    public class SubtitleManagerEditor : Editor
    {
        private SubtitleManager script = null;

        private void OnEnable()
        {
            if (script == null)
                script = target as SubtitleManager;
        }

        public override void OnInspectorGUI()
        {
            script.dontDestory = EditorGUILayout.Toggle("Dont Destory", script.dontDestory);
            script.canvasPrefab = (GameObject)EditorGUILayout.ObjectField("Canvas Prefab", script.canvasPrefab, typeof(GameObject), false);
        }
    }
}