using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    private void Start()
    {
        LobbyManager.Instance.OnLobbyTaskStarted += LobbyManager_OnLobbyTaskStarted;
        LobbyManager.Instance.OnLobbyTaskCompleted += LobbyManager_OnLobbyTaskCompleted;
        LobbyManager.Instance.OnLobbyError += LobbyManager_OnLobbyError;

        Hide();
    }

    private void LobbyManager_OnLobbyError(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void LobbyManager_OnLobbyTaskCompleted(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void LobbyManager_OnLobbyTaskStarted(object sender, System.EventArgs e)
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
