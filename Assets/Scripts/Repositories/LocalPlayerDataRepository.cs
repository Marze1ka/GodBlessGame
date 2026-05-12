using UnityEngine;

public class LocalPlayerDataRepository : IPlayerDataRepository
{
    public void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("PlayerSave", json);
        PlayerPrefs.Save();
    }

    public PlayerData Load()
    {
        string json = PlayerPrefs.GetString("PlayerSave");
        return JsonUtility.FromJson<PlayerData>(json);
    }
}