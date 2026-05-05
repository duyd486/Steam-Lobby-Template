using TMPro;
using Unity.Services.Lobbies.Models;
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
        joinBtn.onClick.AddListener(() =>
        {
            LobbyManager.Instance.JoinLobbyById(lobby.Id);
        });
    }

    public void UpdateLobby(Lobby lobby)
    {
        this.lobby = lobby;
        nameText.text = lobby.Name;
        playerCountText.text = (lobby.MaxPlayers - lobby.AvailableSlots).ToString() + "/" + lobby.MaxPlayers;
    }
}
