using Steamworks;
using Steamworks.Data;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    public static LobbyUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI lobbyName;
    [SerializeField] private Button leaveLobbyBtn;
    [SerializeField] private Button startGameBtn;

    [SerializeField] private List<PlayerInfoSingleUI> playerInfos;
    [SerializeField] private GameObject playerInfoSingleUI;
    [SerializeField] private GameObject playerContainer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Hide();
        playerInfoSingleUI.SetActive(false);

        leaveLobbyBtn.onClick.AddListener(() =>
        {
            SteamLobbyManager.Instance.LeaveLobby();
        });

        startGameBtn.onClick.AddListener(() =>
        {
            // Start game bằng NGO
            if (NetworkManager.Singleton.IsHost)
            {
                //NetworkManager.Singleton.StartHost();
            }
        });

        SteamLobbyManager.Instance.OnLobbyJoined += OnLobbyUpdated;
        SteamLobbyManager.Instance.OnLobbyCreated += OnLobbyUpdated;
        SteamLobbyManager.Instance.OnLobbyUpdated += OnLobbyUpdated;
        SteamLobbyManager.Instance.OnLobbyLeft += Hide;
    }

    private void OnDestroy()
    {
        leaveLobbyBtn.onClick.RemoveAllListeners();
        startGameBtn.onClick.RemoveAllListeners();
        SteamLobbyManager.Instance.OnLobbyJoined -= OnLobbyUpdated;
        SteamLobbyManager.Instance.OnLobbyCreated -= OnLobbyUpdated;
        SteamLobbyManager.Instance.OnLobbyUpdated -= OnLobbyUpdated;
        SteamLobbyManager.Instance.OnLobbyLeft -= Hide;
    }

    private void OnLobbyUpdated(Lobby lobby)
    {
        UpdateLobby(lobby);
    }

    public void UpdateLobby(Lobby lobby)
    {
        if (!lobby.Id.IsValid)
        {
            Hide();
            return;
        }

        foreach (Transform child in playerContainer.transform)
        {
            if (child == playerInfoSingleUI.transform) continue; // giữ lại template
            Destroy(child.gameObject);
        }

        playerInfos.Clear();

        // ====== LOBBY NAME ======
        string name = lobby.GetData("name");
        lobbyName.text = string.IsNullOrEmpty(name) ? "Lobby" : name;

        // ====== PLAYER LIST ======
        var members = lobby.Members;

        foreach (var member in members)
        {
            GameObject obj = Instantiate(playerInfoSingleUI, playerContainer.transform);
            obj.SetActive(true);

            PlayerInfoSingleUI ui = obj.GetComponent<PlayerInfoSingleUI>();
            ui.UpdatePlayerInfo(member.Id);

            playerInfos.Add(ui);
        }

        Show(lobby);
    }

    public void Show(Lobby lobby)
    {
        gameObject.SetActive(true);

        // chỉ host mới được start
        bool isHost = lobby.Owner.Id == SteamClient.SteamId;

        startGameBtn.gameObject.SetActive(isHost);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}