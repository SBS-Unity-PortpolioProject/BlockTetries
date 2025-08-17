using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public Button retryButton;
    public Button quitButton;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        retryButton.onClick.AddListener(OnRetry);
        quitButton.onClick.AddListener(OnQuit);
    }

    private void OnRetry()
    {
        SceneManager.LoadScene("Block");
    }

    private void OnQuit()
    {
        SceneManager.LoadScene("Main Menu");
    }

}
