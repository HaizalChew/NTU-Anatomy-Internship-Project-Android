using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoPlayer : MonoBehaviour
{
    public Camera mainCamera;
    private UnityEngine.Video.VideoPlayer videoPlayer;
    // Start is called before the first frame update
    public void Awake()
    {
        videoPlayer = mainCamera.GetComponent<UnityEngine.Video.VideoPlayer>();
    }
    public void StartVideo()
    {
        videoPlayer.Play();
    }

    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
