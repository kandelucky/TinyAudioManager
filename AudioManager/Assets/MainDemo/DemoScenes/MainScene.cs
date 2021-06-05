using UnityEngine.SceneManagement;
using UnityEngine;

public class MainScene : MonoBehaviour
{
    //SCENE MANAGMENT
    private void Awake()
    {
        GameObject audios = GameObject.FindGameObjectWithTag("music");
        if (audios) Destroy(audios);
    }
    public void MoveToScene(string sceneN)
    {
        SceneManager.LoadScene(sceneN);
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
