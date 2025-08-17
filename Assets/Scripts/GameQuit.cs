using UnityEngine;
using UnityEngine.UI;

public class QuitButtonHandler : MonoBehaviour
{
    public Button quitButton;  // Quit 버튼

    void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
        else
        {
            Debug.LogWarning("Quit 버튼이 연결되지 않았습니다.");
        }
    }

    void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}