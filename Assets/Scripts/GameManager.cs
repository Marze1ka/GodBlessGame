using UnityEngine;
using UnityEngine.SceneManagement; // Для перезагрузки

public class GameManager : MonoBehaviour
{
    public GameObject gameOverUI; // Ссылка на панель смерти

    // Метод для показа окна (вызовем из Health.cs)
    public void ShowGameOverScreen()
    {
        gameOverUI.SetActive(true); // Включаем панель

        // Разблокируем курсор мыши, чтобы можно было нажать кнопку
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Останавливаем время в игре (мобы перестанут бегать)
        Time.timeScale = 0f;
    }

    // Метод для кнопки рестарта
    public void RestartGame()
    {
        Time.timeScale = 1f; // Возвращаем время в норму
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}