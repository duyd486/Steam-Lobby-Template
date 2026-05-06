using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    private void Start()
    {
        SteamLobbyManager.Instance.OnLobbyTaskStarted += LobbyManager_OnLobbyTaskStarted;
        SteamLobbyManager.Instance.OnLobbyTaskCompleted += LobbyManager_OnLobbyTaskCompleted;
        SteamLobbyManager.Instance.OnLobbyError += LobbyManager_OnLobbyError;

        Hide();
    }

    private void OnDestroy()
    {
        SteamLobbyManager.Instance.OnLobbyTaskStarted -= LobbyManager_OnLobbyTaskStarted;
        SteamLobbyManager.Instance.OnLobbyTaskCompleted -= LobbyManager_OnLobbyTaskCompleted;
        SteamLobbyManager.Instance.OnLobbyError -= LobbyManager_OnLobbyError;
    }

    private void LobbyManager_OnLobbyError()
    {
        Hide();
    }

    private void LobbyManager_OnLobbyTaskCompleted()
    {
        Hide();
    }

    private void LobbyManager_OnLobbyTaskStarted()
    {
        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
