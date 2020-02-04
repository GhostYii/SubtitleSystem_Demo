//ORG: Ghostyii & MOONLIGHTGAME
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

namespace SubtitleSystem
{
    //SubtitleAsset播放器
    [DisallowMultipleComponent]
    public class SubtitleAssetPlayer : MonoBehaviour
    {
        public bool isPlaying = false;
        public bool isPause = false;

        public bool IsStop { get { return !isPlaying && !isPause; } }

        //当前字幕进度 range[0,1]
        public float currentProgress = 0;
        //当前字幕内容
        public string currentSubtileContent = string.Empty;

        public SubtitleAsset subtitleAsset;
        public Text text;

        public UnityEvent onPlay = new UnityEvent();
        public UnityEvent onPause = new UnityEvent();
        public UnityEvent onStop = new UnityEvent();

        private Subtitle currentSubtitle = null;

        public void Play()
        {
            if (text == null)
                text = SubtitleManager.Instance.CreateText(Vector3.zero, 14, Color.white);
            isPause = false;

            onPlay.Invoke();

            if (currentSubtitle == null)
                StartCoroutine(PlayCoroutine());
            else
                currentSubtitle.Play();

        }

        public void Pause()
        {
            if (isPause)
                return;

            onPause.Invoke();

            if (currentSubtitle != null)
            {
                isPause = true;
                currentSubtitle.Pause();
            }
        }

        public void Stop()
        {
            onStop.Invoke();

            //由于PlayCouroutine中的yield return语句会创建新的协程
            //故此处使用StopAllCouroutine而不应该使用StopCouroutine(PlayCouroutine())
            StopAllCoroutines();

            if (currentSubtitle != null)
            {
                currentSubtitle.Stop();
                //置空使得Play逻辑正确
                currentSubtitle = null;
            }

            text.text = string.Empty;
            currentSubtileContent = string.Empty;
            currentProgress = 0f;
            isPlaying = false;
            isPause = false;
        }

        private IEnumerator PlayCoroutine()
        {
            isPlaying = true;

            for (int i = 0; i < subtitleAsset.subtitles.Count; i++)
            {
                SubtitleInfo s = subtitleAsset.subtitles[i];
                if (isPause)
                    yield return new WaitUntil(() => !isPause);

                //currentProgress = 0f;
                currentSubtileContent = s.Content;

                #region Set Format        
                SubtitleFormat fmt = subtitleAsset.GetFormatByMsg(s);

                text.font = SubtitleUtility.GetFontByFontName(fmt.FontData.Font.FontName);
                text.fontSize = fmt.FontData.FontSize;
                text.fontStyle = fmt.FontData.FontStyle;
                text.lineSpacing = fmt.FontData.LineSpacing;
                text.supportRichText = fmt.FontData.RichText;
                text.alignment = fmt.FontData.Alignment;
                text.alignByGeometry = fmt.FontData.AlignByGeometry;
                text.horizontalOverflow = fmt.FontData.HorizontalWrapMode;
                text.verticalOverflow = fmt.FontData.VerticalWrapMode;
                text.resizeTextForBestFit = fmt.FontData.BestFit;

                text.color = fmt.GetUnityColor();
                text.raycastTarget = fmt.RaycastTarget;
                #endregion

                #region Set Subtitle                
                if (s.FadeInDuration > 0)
                    currentSubtitle = new Subtitle(string.Empty, s.GetVector2(), fmt.FontData.FontSize, fmt.GetUnityColor(), fmt.FontData.Font.FontName, s.Duration + s.FadeInDuration + s.FadeOutDuration);
                else
                    currentSubtitle = new Subtitle(s.Content, s.GetVector2(), fmt.FontData.FontSize, fmt.GetUnityColor(), fmt.FontData.Font.FontName, s.Duration + s.FadeInDuration + s.FadeOutDuration);

                currentSubtitle.Text = text;
                currentSubtitle.destoryTextOnComplete = text == null;

                currentSubtitle.SetDirect(s.IsVertical ? SubtitleDirect.Vertical : SubtitleDirect.Horizontal);
                if (s.FadeInDuration > 0 || s.FadeOutDuration > 0)
                {
                    currentSubtitle.OnPlay = (t, d) =>
                    {
                        if (s.FadeInDuration > 0)
                        {
                            t.color = new Color(t.color.r, t.color.g, t.color.b, 0);
                            t.text = s.Content;
                        }
                        t.DOColor(new Color(t.color.r, t.color.g, t.color.b, 1), s.FadeInDuration)
                        .OnComplete(() =>
                        {
                            float tmp = 0;
                            DOTween.To(() => tmp, (x) => tmp = x, 0, s.Duration)
                            .OnComplete(() =>
                            {
                                t.DOColor(new Color(t.color.r, t.color.g, t.color.b, 0), s.FadeOutDuration)
                                .OnComplete(() => t.color = new Color(t.color.r, t.color.g, t.color.b, 1));
                            });
                        });
                    };
                }

                currentSubtitle.OnUpdate = (t, c) => { if (!currentSubtitle.IsPause) currentProgress = c; };
                #endregion

                if (!isPause)
                    currentSubtitle.Play();
                else
                    currentSubtitle.Pause();

                yield return currentSubtitle.WaitForCompletion();
            }
            text.text = string.Empty;
            currentSubtileContent = string.Empty;
            currentProgress = 0f;
            isPlaying = false;
            isPause = false;
            currentSubtitle = null;
        }
    }
}
