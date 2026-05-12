using UnityEngine;
using System.Collections.Generic;

public class SaveLoadInteractor
{
    private readonly IPlayerDataRepository _playerRepo;
    private readonly IEnemyRepository _enemyRepo;

    public SaveLoadInteractor(IPlayerDataRepository playerRepo, IEnemyRepository enemyRepo)
    {
        _playerRepo = playerRepo;
        _enemyRepo = enemyRepo;
    }

    public void SaveGame()
    {
        // 1. СОХРАНЯЕМ ИГРОКА (Теперь через PlayerController)
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            PlayerController pc = playerObj.GetComponent<PlayerController>();
            if (pc != null)
            {
                PlayerData pData = new PlayerData
                {
                    HP = pc.GetModel().Health, // Берем ХП из модели через контроллер
                    Position = playerObj.transform.position
                };
                _playerRepo.Save(pData);
            }
        }

        // 2. СОХРАНЯЕМ ВРАГОВ (Остается как было, у них остался скрипт Health)
        EnemySaveData eSaveData = new EnemySaveData();
        Health[] allUnits = Object.FindObjectsByType<Health>(FindObjectsSortMode.None);

        foreach (var unit in allUnits)
        {
            if (!unit.isPlayer)
            {
                eSaveData.allEnemies.Add(new EnemyData
                {
                    enemyID = unit.gameObject.name,
                    currentHP = unit.currentHealth,
                    position = unit.transform.position
                });
            }
        }
        _enemyRepo.Save(eSaveData);
        Debug.Log("Интерактор: Все данные успешно сохранены.");
    }

    public void LoadGame()
    {
        // 1. ЗАГРУЖАЕМ ИГРОКА
        PlayerData pData = _playerRepo.Load();
        if (pData != null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                PlayerController pc = playerObj.GetComponent<PlayerController>();
                CharacterController cc = playerObj.GetComponent<CharacterController>();

                if (cc != null) cc.enabled = false; // Выключаем для телепортации

                playerObj.transform.position = pData.Position;

                if (pc != null) pc.SetHealthFromSave(pData.HP); // Загружаем ХП в модель

                if (cc != null) cc.enabled = true; // Включаем обратно
            }
        }

        // 2. ЗАГРУЖАЕМ ВРАГОВ
        EnemySaveData eData = _enemyRepo.Load();
        if (eData != null)
        {
            foreach (var enemyInfo in eData.allEnemies)
            {
                GameObject enemyObj = GameObject.Find(enemyInfo.enemyID);
                if (enemyObj != null)
                {
                    UnityEngine.AI.NavMeshAgent agent = enemyObj.GetComponent<UnityEngine.AI.NavMeshAgent>();
                    if (agent != null) agent.enabled = false;

                    enemyObj.transform.position = enemyInfo.position;
                    Health h = enemyObj.GetComponent<Health>();
                    if (h != null)
                    {
                        h.currentHealth = enemyInfo.currentHP;
                        if (h.healthSlider != null) h.healthSlider.value = enemyInfo.currentHP;
                    }

                    if (agent != null) agent.enabled = true;
                }
            }
        }
    }
}