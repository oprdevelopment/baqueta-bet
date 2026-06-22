using Assets.Bet.UI;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class Sair : MonoBehaviour
{
    void Awake()
    {
        var root = FindAnyObjectByType<UIDocument>().rootVisualElement;
        root.Display(false);
        UnityEngine.Cursor.visible = false;
        GetComponent<VideoPlayer>().loopPointReached += e =>
        {
            root.Q<Button>("Again").clicked += () => UnityEngine.SceneManagement.SceneManager.LoadScene(1);
            root.Q<Button>("Sair").clicked += () => Application.Quit();
            root.Display(true);
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        };
    }
}
