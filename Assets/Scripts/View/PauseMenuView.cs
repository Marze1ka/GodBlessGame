using UnityEngine;

public class PauseMenuView : MonoBehaviour
{
    public GameObject pausePanel; // —сылка на панель в инспекторе

    public void Show()
    {
        pausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Hide()
    {
        pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}