using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    private const string HardSceneName = "Scene_Game";
    private const string PeacefulSceneName = "Scene_Game_Peaceful";

    public MainMenuView view;
    private MainMenuModel _model;

    private void Awake()
    {
        _model = new MainMenuModel();

        if (GameBootstrapper.AudioService != null)
        {
            _model.Volume = GameBootstrapper.AudioService.GetVolume();
        }
    }

    public void OnPlayClicked()
    {
        SceneManager.LoadScene(HardSceneName);
    }

    public void OnPlayPeacefulClicked()
    {
        SceneManager.LoadScene(PeacefulSceneName);
    }

    public void OnSettingsClicked()
    {
        view.ShowSettings(_model.Volume);
    }

    public void OnBackClicked()
    {
        view.ShowMainMenu();
    }

    public void OnVolumeChanged(float val)
    {
        _model.Volume = val;
        GameBootstrapper.AudioService?.SetVolume(val);
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}
