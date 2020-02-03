//ORG: Ghostyii & MOONLIGHTGAME
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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

        private Tween currentTween = null;

        private void Start()
        {
            if (text == null)
                text = SubtitleManager.Instance.CreateText(Vector3.zero, 14, Color.white);
                //text = SubtitleManager.Instance.CanvasRoot.GetComponentInChildren<Text>();
        }

        public void Play()
        {
            if (text == null)
                text = SubtitleManager.Instance.CreateText(Vector3.zero, 14, Color.white);
                //text = SubtitleManager.Instance.CanvasRoot.GetComponentInChildren<Text>();
            isPause = false;

            onPlay.Invoke();
            if (currentTween == null)            
                StartCoroutine(PlayCoroutine());            
            else
                currentTween.Play();
        }

        public void Pause()
        {
            if (isPause)
                return;

            onPause.Invoke();
            if (currentTween != null)
            {
                isPause = true;
                currentTween.Pause();
            }
        }

        public void Stop()
        {
            onStop.Invoke();

            //由于PlayCouroutine中的yield return语句会创建新的协程
            //故此处使用StopAllCouroutine而不应该使用StopCouroutine(PlayCouroutine())
            StopAllCoroutines();

            if (currentTween != null)
            {
                currentTween.Kill();
                //置空使得Play逻辑正确
                currentTween = null;
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
            foreach (SubtitleInfo s in subtitleAsset.subtitles)
            {
                if (isPause)
                    yield return new WaitUntil(() => !isPause);

                currentProgress = 0f;
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
                text.rectTransform.localPosition = s.GetVector2();
                text.text = s.Content;
                #endregion

                if (currentTween != null && isPause)
                {
                    currentTween.Pause();
                    yield return new WaitUntil(() => !isPause);
                }

                currentTween = DOTween.To(() => currentProgress, (x) => currentProgress = x, 1f, s.Duration).SetEase(Ease.Linear);
                yield return currentTween.WaitForCompletion();
            }
            text.text = string.Empty;
            currentSubtileContent = string.Empty;
            currentProgress = 0f;
            isPlaying = false;
            isPause = false;
            currentTween = null;
        }
    }
}
