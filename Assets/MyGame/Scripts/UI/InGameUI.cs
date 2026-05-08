using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [SerializeField] private Button leaveBtn;

    private void Awake()
    {
        leaveBtn.onClick.AddListener(OnLeaveBtnClicked);
    }

    private void OnLeaveBtnClicked()
    {
        // Leave Steam Lobby
        if (SteamLobbyManager.Instance.CurrentLobby.HasValue)
        {
            SteamLobbyManager.Instance.LeaveLobby();
        }

        // Stop NGO
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Back menu
        SceneManager.LoadScene("SteamLobby");
    }
}
