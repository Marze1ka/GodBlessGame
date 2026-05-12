using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Ссылка на панель с кнопкой рестарт

    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // РАЗБЛОКИРУЕМ МЫШКУ (иначе ты не сможешь нажать кнопку)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Останавливаем время, чтобы мир замер
            Time.timeScale = 0f;
        }
    }

    // Этот метод привяжи к кнопке "Начать заново" в инспекторе
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}