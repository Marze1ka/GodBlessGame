using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private const string SaveExistsKey = "RuntimeSave_Exists";
    private const string SceneKey = "RuntimeSave_Scene";
    private const string HpKey = "RuntimeSave_HP";
    private const string PosXKey = "RuntimeSave_PosX";
    private const string PosYKey = "RuntimeSave_PosY";
    private const string PosZKey = "RuntimeSave_PosZ";

    public PauseMenuView view;
    private PauseModel _model;
    private bool _hasRuntimeSave;
    private PlayerData _runtimeSave;

    private void Awake()
    {
        GameBootstrapper.EnsureInitialized();
        _model = new PauseModel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        _model.IsPaused = !_model.IsPaused;

        if (_model.IsPaused)
        {
            Time.timeScale = 0f;
            view.Show();
        }
        else
        {
            Time.timeScale = 1f;
            view.Hide();
        }
    }

    public void OnSaveClicked()
    {
        GameBootstrapper.EnsureInitialized();
        GameBootstrapper.SaveInteractor.SaveGame();

        PlayerController player = Object.FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("PauseMenuController: cannot save, PlayerController was not found.");
            return;
        }

        player.EnsureInitialized();
        _runtimeSave = new PlayerData
        {
            HP = player.GetModel().Health,
            Position = player.transform.position,
            SceneName = SceneManager.GetActiveScene().name
        };

        _hasRuntimeSave = true;
        SaveRuntimeSlotToPrefs(_runtimeSave);

        Debug.Log($"SAVE OK: scene={_runtimeSave.SceneName}, hp={_runtimeSave.HP}, pos={_runtimeSave.Position}");
    }

    public void OnLoadClicked()
    {
        Time.timeScale = 1f;
        _model.IsPaused = false;
        view.Hide();
        GameBootstrapper.EnsureInitialized();

        PlayerData data = _hasRuntimeSave ? _runtimeSave : LoadRuntimeSlotFromPrefs();
        if (data == null)
        {
            Debug.LogWarning("LOAD FAILED: save was not found.");
            return;
        }

        if (data.SceneName != SceneManager.GetActiveScene().name)
        {
            GameBootstrapper.SaveInteractor.LoadGame();
            return;
        }

        RestorePlayerNow(data);
        Debug.Log($"LOAD OK: scene={data.SceneName}, hp={data.HP}, pos={data.Position}");
    }

    public void OnExitClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene_MainMenu");
    }

    private void RestorePlayerNow(PlayerData data)
    {
        PlayerController player = Object.FindAnyObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("PauseMenuController: cannot load, PlayerController was not found.");
            return;
        }

        player.EnsureInitialized();

        CharacterController characterController = player.GetComponent<CharacterController>();
        if (characterController != null) characterController.enabled = false;

        player.transform.SetPositionAndRotation(data.Position, player.transform.rotation);
        player.SetHealthFromSave(data.HP);

        if (characterController != null) characterController.enabled = true;
        Physics.SyncTransforms();
    }

    private static void SaveRuntimeSlotToPrefs(PlayerData data)
    {
        PlayerPrefs.SetInt(SaveExistsKey, 1);
        PlayerPrefs.SetString(SceneKey, data.SceneName);
        PlayerPrefs.SetFloat(HpKey, data.HP);
        PlayerPrefs.SetFloat(PosXKey, data.Position.x);
        PlayerPrefs.SetFloat(PosYKey, data.Position.y);
        PlayerPrefs.SetFloat(PosZKey, data.Position.z);
        PlayerPrefs.Save();
    }

    private static PlayerData LoadRuntimeSlotFromPrefs()
    {
        if (PlayerPrefs.GetInt(SaveExistsKey, 0) == 0)
        {
            return null;
        }

        return new PlayerData
        {
            SceneName = PlayerPrefs.GetString(SceneKey),
            HP = PlayerPrefs.GetFloat(HpKey),
            Position = new Vector3(
                PlayerPrefs.GetFloat(PosXKey),
                PlayerPrefs.GetFloat(PosYKey),
                PlayerPrefs.GetFloat(PosZKey))
        };
    }
}

