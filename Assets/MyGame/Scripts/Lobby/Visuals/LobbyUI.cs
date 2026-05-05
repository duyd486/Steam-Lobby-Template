using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
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

    private Lobby lobby;

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
            LobbyManager.Instance.LeaveLobby();
        });

        startGameBtn.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LockLobby();
            //SceneLoader.LoadSceneByNetwork(SceneLoader.Scene.Game);
        });

        LobbyManager.Instance.OnLobbyDataChanged += LobbyManager_OnLobbyDataChanged;
        LobbyManager.Instance.OnLobbyCreated += LobbyManager_OnLobbyCreated;
    }

    private void LobbyManager_OnLobbyCreated(object sender, LobbyManager.OnLobbyCreatedEventArgs e)
    {
        UpdateLobby(e.hostLobby);
    }

    private void LobbyManager_OnLobbyDataChanged(object sender, LobbyManager.OnLobbyDataChangedEventArgs e)
    {
        UpdateLobby(e.lobby);
    }

    public void UpdateLobby(Lobby lobby)
    {
        if (lobby == null)
        {
            Hide();
            return;
        }

        if (playerInfos.Count < lobby.MaxPlayers)
        {
            for (int i = playerInfos.Count; i < lobby.MaxPlayers; i++)
            {
                PlayerInfoSingleUI playerInfo = Instantiate(playerInfoSingleUI, playerContainer.transform).GetComponent<PlayerInfoSingleUI>();
                playerInfos.Add(playerInfo);
                playerInfo.Show();
            }
        }

        this.lobby = lobby;
        lobbyName.text = lobby.Name;

        int playerIndex = 0;
        List<Player> players = lobby.Players;

        foreach (PlayerInfoSingleUI playerInfo in playerInfos)
        {
            if (playerIndex < players.Count)
            {
                playerInfo.UpdatePlayerInfo(players[playerIndex].Data["PlayerName"].Value);
                playerIndex++;
            }
            else
            {
                playerInfo.UpdatePlayerInfo();
            }
        }

        Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (NetworkManager.Singleton.IsHost)
        {
            startGameBtn.gameObject.SetActive(true);
        }
        else
        {
            startGameBtn.gameObject.SetActive(false);
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
