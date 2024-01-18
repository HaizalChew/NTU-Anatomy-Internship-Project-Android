using System.Collections;
using System;
using UnityEngine.Video;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    [SerializeField] ControllableSlider videoSlider;
    [SerializeField] TMP_Text videoTimer;

    bool skippingVideo;

    // Start is called before the first frame update
    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoSlider.OnEndValueChanged.AddListener(SkipVideo);
    }

    private void Update()
    {
        if (videoSlider)
        {   
            if (videoPlayer.frameCount > 0 && !videoSlider.isDragged && !skippingVideo)
            {
                videoSlider.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
            }
        }

        if (videoTimer)
        {
            if (videoPlayer.frameCount > 0)
            {
                string minutes(double time) => TimeSpan.FromSeconds(time).Minutes.ToString("0");
                string seconds(double time) => TimeSpan.FromSeconds(time).Seconds.ToString("00");


                videoTimer.text = minutes(videoPlayer.time) + ":" + seconds(videoPlayer.time) + " / " + minutes(videoPlayer.length) + ":" + seconds(videoPlayer.length);
            }
        }
    }

    public void PauseVideo(bool tickOn)
    {
        if (tickOn)
        {
            videoPlayer.Pause();
        }
        else
        {
            videoPlayer.Play();
        }
        
    }

    // Fix bug where video jumps back to original before skipping
    IEnumerator WaitToSkipVideo()
    {
        skippingVideo = true;
        videoPlayer.frame = (long)(videoPlayer.frameCount * videoSlider.value);

        yield return new WaitUntil(() => videoPlayer.frame == (long)(videoPlayer.frameCount * videoSlider.value));

        skippingVideo = false;
    }

    // Skip the video to a specific time point
    public void SkipVideo()
    {
        StartCoroutine(WaitToSkipVideo());
    }
}
