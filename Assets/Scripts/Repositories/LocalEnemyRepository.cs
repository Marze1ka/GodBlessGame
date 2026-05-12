using UnityEngine;

public class LocalEnemyRepository : IEnemyRepository
{
    private const string SaveKey = "EnemiesSave";

    public void Save(EnemySaveData data)
    {
        // Превращаем список врагов в одну длинную строку (JSON)
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log("Репозиторий: Данные всех врагов записаны в память.");
    }

    public EnemySaveData Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return null;

        string json = PlayerPrefs.GetString(SaveKey);
        // Превращаем строку обратно в список объектов
        return JsonUtility.FromJson<EnemySaveData>(json);
    }
}