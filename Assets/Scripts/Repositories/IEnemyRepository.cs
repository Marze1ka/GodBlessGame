public interface IEnemyRepository
{
    void Save(EnemySaveData data);
    EnemySaveData Load();
}