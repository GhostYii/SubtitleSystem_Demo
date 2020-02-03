using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SubtitleSystem;
using UnityEngine.Video;

public class VideoWithSubtitle : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public SubtitleAssetPlayer subtitleAssetPlayer;

    private void Start()
    {
        subtitleAssetPlayer.onPlay.AddListener(() => videoPlayer.Play());
        subtitleAssetPlayer.onPause.AddListener(() => videoPlayer.Pause());
        subtitleAssetPlayer.onStop.AddListener(() => videoPlayer.Stop());
    }
}
