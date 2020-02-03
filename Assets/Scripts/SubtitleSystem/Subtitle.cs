//ORG: Ghostyii & MOONLIGHTGAME
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SubtitleSystem
{
    public class Subtitle
    {
        private SubtitleMsg msg;
        private Tween tween = null;
        private bool isComplete = false;
        private bool isPause = false;
        private Text text = null;
        private float currentProgress = 0f;
        private UnityEvent onComplete = new UnityEvent();
        private Action<Text, float> onPlay = null;

        //Range: [0,1]
        public float CurrentProgress { get => currentProgress; set => currentProgress = value; }
        public UnityEvent OnComplete { get => onComplete; set => onComplete = value; }
        public Action<Text, float> OnPlay { get => onPlay; set => onPlay = value; }
        public Text Text { get => text; set => text = value; }        
        public float Duration { get => msg.duration; }
        public bool IsCompleted { get => isComplete; }
        public bool IsPause { get => isPause; }

        public Subtitle()
        {
            msg = new SubtitleMsg()
            {
                content = "",
                duration = float.MaxValue,
                fontName = "Arial",
                color = Color.black,
                fontSize = 14,
                position = Vector3.zero,
                horizontalFit = ContentSizeFitter.FitMode.PreferredSize,
                verticalFit = ContentSizeFitter.FitMode.PreferredSize
            };
            //text = SubtitleManager.Instance.CreateText(msg.position, msg.fontSize, msg.color);
        }
        public Subtitle(string content, Vector3 position, int fontSize, Color color, string fontName, float duration)
        {
            msg = new SubtitleMsg()
            {
                content = content,
                color = color,
                fontSize = fontSize,
                position = position,
                duration = duration,
                fontName = fontName,
                horizontalFit = ContentSizeFitter.FitMode.PreferredSize,
                verticalFit = ContentSizeFitter.FitMode.PreferredSize
            };
            //text = SubtitleManager.Instance.CreateText(msg.position, msg.fontSize, msg.color, msg.fontName);
            //text.text = msg.content;
        }

        //播放，如果执行此函数时该字幕处于暂停状态则会继续播放而无视参数
        public void Play()
        {
            if (tween != null)
                return;

            if (isPause)
            {
                tween.TogglePause();
                isPause = false;
                return;
            }

            isPause = false;
            currentProgress = 0;
            isComplete = false;
            if (text == null)
            {
                text = SubtitleManager.Instance.CreateText(msg.position, msg.fontSize, msg.color, msg.fontName);
                text.rectTransform.localPosition = msg.position;
                text.fontSize = msg.fontSize;
                text.color = msg.color;
                if (!string.IsNullOrEmpty(msg.fontName))
                    text.font = SubtitleUtility.GetFontByFontName(msg.fontName);
                else
                    text.font = SubtitleUtility.GetFontByFontName("Arial");
                var fitter = text.GetComponent<ContentSizeFitter>();
                fitter.horizontalFit = msg.horizontalFit;
                fitter.verticalFit = msg.verticalFit;
                text.text = msg.content;
            }

            if (msg.duration > 0)
            {
                onComplete.AddListener(() =>
                {
                    if (!text.IsDestroyed())
                        text.gameObject.AddComponent<EventBuilder>().Destroy();
                });
            }

            tween = DOTween.To(() => currentProgress, x => currentProgress = x, 1f, msg.duration)
                .SetEase(Ease.Linear)
                .OnPlay(() => { if (onPlay != null) onPlay(text, currentProgress); })
                .OnComplete(() => { isComplete = true; onComplete.Invoke(); });
        }
        public void Play(string content, float duration)
        {
            if (tween != null)
                return;

            if (isPause)
            {
                tween.TogglePause();
                isPause = false;
                return;
            }

            msg.content = content;
            msg.duration = duration;

            Play();
        }
        public void Play(string content, Vector3 position, int fontSize, Color color, string fontName, float duration)
        {
            if (tween != null)
                return;

            if (isPause)
            {
                tween.TogglePause();
                isPause = false;
                return;
            }

            msg.content = content;
            msg.position = position;
            msg.fontSize = fontSize;
            msg.color = color;
            msg.fontName = fontName;
            msg.duration = duration;

            Play();
        }
        
        //暂停
        public void Pause()
        {
            if (tween == null)
                return;

            isPause = true;
            tween = tween.Pause();
        }

        //切换暂停状态，即暂停->播放，播放->暂停
        public void TogglePause()
        {
            if (tween == null)
                return;

            isPause = !isPause;
            tween.TogglePause();
        }

        //停止播放
        public void Stop()
        {
            if (tween == null)
                return;
            else
            {
                tween.Kill(true);
                currentProgress = 1f;
                tween = null;
                isComplete = true;
                isPause = false;
                text.gameObject.AddComponent<EventBuilder>().Destroy();
            }
        }

        //立即终止此字幕并重新开始播放
        public void Restart()
        {
            Stop();
            Play();
        }

        //设置字幕显示方向
        public Subtitle SetDirect(SubtitleDirect dir)
        {
            msg.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            msg.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            switch (dir)
            {
                case SubtitleDirect.Horizontal:
                    break;
                case SubtitleDirect.Vertical:
                    msg.horizontalFit = ContentSizeFitter.FitMode.MinSize;
                    break;
                default:
                    break;
            }

            return this;
        }
    }
}
