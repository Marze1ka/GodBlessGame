using UnityEngine;

public class AudioService : IAudioService
{
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume; // Глобальная громкость Unity
        PlayerPrefs.SetFloat("MusicVolume", volume); // Сохраняем настройку
    }

    public float GetVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 1.0f);
    }
}