using UnityEngine;
using UnityEngine.Video;

public class Sair : MonoBehaviour
{
    void Awake()
    {
        Cursor.visible = false;
        GetComponent<VideoPlayer>().loopPointReached += e => Application.Quit();
    }
}
