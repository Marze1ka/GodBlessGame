using UnityEngine;
using System.Collections.Generic;

public class SaveLoadInteractor
{
    private readonly IPlayerDataRepository _playerRepo;
    private readonly IEnemyRepository _enemyRepo; // Этого поля не хватало (Ошибка CS0103)

    // Конструктор теперь принимает ДВА аргумента (Исправляет ошибку CS1729)
    public SaveLoadInteractor(IPlayerDataRepository playerRepo, IEnemyRepository enemyRepo)
    {
        _playerRepo = playerRepo;
        _enemyRepo = enemyRepo;
    }

    public void SaveGame()
    {
        // 1. СОХРАНЯЕМ ИГРОКА
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            Health pHealth = playerObj.GetComponent<Health>();
            PlayerData pData = new PlayerData
            {
                HP = pHealth.currentHealth,
                Position = playerObj.transform.position
            };
            _playerRepo.Save(pData);
        }

        // 2. СОХРАНЯЕМ ВРАГОВ
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
        Debug.Log("Интерактор: Все данные сохранены.");
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
                CharacterController cc = playerObj.GetComponent<CharacterController>();
                if (cc != null) cc.enabled = false;

                playerObj.transform.position = pData.Position;
                Health h = playerObj.GetComponent<Health>();
                h.currentHealth = pData.HP;
                if (h.healthSlider != null) h.healthSlider.value = pData.HP;

                if (cc != null) cc.enabled = true;
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
                    h.currentHealth = enemyInfo.currentHP;
                    if (h.healthSlider != null) h.healthSlider.value = enemyInfo.currentHP;

                    if (agent != null) agent.enabled = true;
                }
            }
        }
    }
}