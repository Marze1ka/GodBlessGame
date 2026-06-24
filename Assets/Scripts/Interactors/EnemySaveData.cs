using System;
using System.Collections.Generic;

[Serializable]
public class EnemySaveData
{
    public string sceneName;
    public int killCount;
    public bool bossPresent;
    public List<EnemyData> allEnemies = new List<EnemyData>();
}
