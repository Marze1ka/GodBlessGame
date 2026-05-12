using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    public GameObject mainButtonsGroup;
    public GameObject settingsPanel;
    public Slider volumeSlider;

    public void ShowMainMenu()
    {
        mainButtonsGroup.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ShowSettings(float currentVolume)
    {
        mainButtonsGroup.SetActive(false);
        settingsPanel.SetActive(true);
        volumeSlider.value = currentVolume;
    }
}