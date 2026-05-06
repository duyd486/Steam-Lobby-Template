using Steamworks.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySingleUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private TextMeshProUGUI playerCountText;

    private Button joinBtn;
    private Lobby lobby;

    private void Awake()
    {
        joinBtn = GetComponent<Button>();
    }

    private void Start()
    {
        // ===== JOIN BUTTON =====
        joinBtn.onClick.RemoveAllListeners();
        joinBtn.onClick.AddListener(async () =>
        {
            await SteamLobbyManager.Instance.JoinLobby(lobby.Id);
        });
    }

    private void OnDestroy()
    {
        joinBtn.onClick.RemoveAllListeners();
    }

    public void UpdateLobby(Lobby lobby)
    {
        this.lobby = lobby;

        // ===== NAME =====
        string name = lobby.GetData("name");
        if (string.IsNullOrEmpty(name))
        {
            name = "Lobby";
        }
        nameText.text = name;

        // ===== MODE =====
        string mode = lobby.GetData("mode");
        gameModeText.text = string.IsNullOrEmpty(mode) ? "Default" : mode;

        // ===== PLAYER COUNT =====
        playerCountText.text = $"{lobby.MemberCount}/{lobby.MaxMembers}";
    }
}