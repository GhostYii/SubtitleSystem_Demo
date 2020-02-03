using DG.Tweening;
using SubtitleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SubtitleSequenceTester : MonoBehaviour
{
    public Text timerText;

    private float startTime = 0;
    private SubtitleSequence sequence = new SubtitleSequence();
    private Subtitle tip;


    private void Start()
    {
        tip = SubtitleManager.Instance.Show("按下空格键以开始显示字幕", Vector3.zero, 30, Color.white, 0);

        sequence.Append(SubtitleManager.Instance.Show("字幕测试，所有字幕显示时间2s", Vector3.zero, 30, Color.white, "Arial", 2f, false));
        sequence.Append(SubtitleManager.Instance.ShowVertical("竖排字幕预览", Vector3.zero, 30, Color.white, 2f, false));
        sequence.Append(SubtitleManager.Instance.ShowWithFade("淡入淡出字幕，淡入淡出各2s", Vector3.zero, 30, Color.white, 0, 2f, 2f, "Arial", false));
        sequence.Append(SubtitleManager.Instance.ShowWithShakePosition("位置震动字幕", Vector3.zero, 30, Color.white, 2f, 30f, true, "Arial", false));
        sequence.Append(SubtitleManager.Instance.ShowWithShakeRotation("旋转震动字幕", Vector3.zero, 30, Color.white, 2f, 90f, true, "Arial", false));
        sequence.Append(SubtitleManager.Instance.ShowWithShakeScale("缩放震动字幕", Vector3.zero, 30, Color.white, 2f, 5f, true, "Arial", false));
        sequence.Append(SubtitleManager.Instance.ShowWithTypewriter("打字机效果预览字幕", Vector3.zero, 30, Color.white, 2f, 0.1f, "Arial", false));
        sequence.Append(SubtitleManager.Instance.ShowWithCustom("自定义效果字幕，字体颜色渐变为红色", Vector3.zero, 30, Color.white, 2f, (t, d) => t.DOColor(Color.red, 3f), null, "Arial", false));
        sequence.Append(new Subtitle("常驻字幕，永不消失", Vector3.zero, 30, Color.white, "Arial", 0));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            tip.Stop();
            startTime = Time.realtimeSinceStartup;
            sequence.Start();
        }

        if (startTime > 0)
            timerText.text = (Time.realtimeSinceStartup - startTime).ToString("F2");
    }

}
