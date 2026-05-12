using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public MainMenuView view;
    private MainMenuModel _model;

    private void Awake()
    {
        _model = new MainMenuModel();
        // Подтягиваем начальное значение из сервиса
        _model.Volume = GameBootstrapper.AudioService.GetVolume();
    }

    // Нажатие кнопки "Играть"
    public void OnPlayClicked()
    {
        SceneManager.LoadScene("Scene_Game");
    }

    // Нажатие кнопки "Настройки"
    public void OnSettingsClicked()
    {
        view.ShowSettings(_model.Volume);
    }

    // Нажатие кнопки "Назад" в настройках
    public void OnBackClicked()
    {
        view.ShowMainMenu();
    }

    // Изменение слайдера
    public void OnVolumeChanged(float val)
    {
        _model.Volume = val;
        GameBootstrapper.AudioService.SetVolume(val);
    }
}