using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoAcabar : MonoBehaviour
{
    VideoPlayer video;
    void Awake()
    {
        video = GetComponent<VideoPlayer>();
        video.loopPointReached += c => SceneManager.LoadScene(1);
        Cursor.visible = false;
    }
}
