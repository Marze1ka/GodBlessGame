using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public PauseMenuView view; // Ссылка на Вид
    private PauseModel _model; // Наша модель данных

    private void Awake()
    {
        _model = new PauseModel(); // Создаем модель
    }

    private void Update()
    {
        // Слушаем нажатие Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        _model.IsPaused = !_model.IsPaused;

        if (_model.IsPaused)
        {
            Time.timeScale = 0f;
            view.Show();
        }
        else
        {
            Time.timeScale = 1f;
            view.Hide();
        }
    }

    // Методы для кнопок (вызываем Интерактор)
    public void OnSaveClicked()
    {
        GameBootstrapper.SaveInteractor.SaveGame();
    }

    public void OnLoadClicked()
    {
        GameBootstrapper.SaveInteractor.LoadGame();
        TogglePause(); // Закрываем меню после загрузки
    }

    public void OnExitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene_MainMenu");
    }
}