public interface IPlayerDataRepository
{
    void Save(PlayerData data);
    PlayerData Load();
}