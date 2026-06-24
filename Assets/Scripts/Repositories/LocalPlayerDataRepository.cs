using UnityEngine;

public class LocalPlayerDataRepository : IPlayerDataRepository
{
    private const string JsonKey = "PlayerSave";
    private const string HpKey = "PlayerSave_HP";
    private const string PosXKey = "PlayerSave_PosX";
    private const string PosYKey = "PlayerSave_PosY";
    private const string PosZKey = "PlayerSave_PosZ";
    private const string SceneKey = "PlayerSave_Scene";

    public void Save(PlayerData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(JsonKey, json);
        PlayerPrefs.SetFloat(HpKey, data.HP);
        PlayerPrefs.SetFloat(PosXKey, data.Position.x);
        PlayerPrefs.SetFloat(PosYKey, data.Position.y);
        PlayerPrefs.SetFloat(PosZKey, data.Position.z);
        PlayerPrefs.SetString(SceneKey, data.SceneName ?? string.Empty);
        PlayerPrefs.Save();
    }

    public PlayerData Load()
    {
        if (PlayerPrefs.HasKey(HpKey) && PlayerPrefs.HasKey(PosXKey) && PlayerPrefs.HasKey(PosYKey) &&
            PlayerPrefs.HasKey(PosZKey) && PlayerPrefs.HasKey(SceneKey))
        {
            return new PlayerData
            {
                HP = PlayerPrefs.GetFloat(HpKey),
                Position = new Vector3(
                    PlayerPrefs.GetFloat(PosXKey),
                    PlayerPrefs.GetFloat(PosYKey),
                    PlayerPrefs.GetFloat(PosZKey)),
                SceneName = PlayerPrefs.GetString(SceneKey)
            };
        }

        if (!PlayerPrefs.HasKey(JsonKey))
        {
            return null;
        }

        string json = PlayerPrefs.GetString(JsonKey);
        return JsonUtility.FromJson<PlayerData>(json);
    }
}
