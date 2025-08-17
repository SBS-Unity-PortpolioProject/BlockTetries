using UnityEngine;
using UnityEngine.UI;

public class QuitButtonHandler : MonoBehaviour
{
    public Button quitButton;  // Quit ��ư

    void Start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
        else
        {
            Debug.LogWarning("Quit ��ư�� ������� �ʾҽ��ϴ�.");
        }
    }

    void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}