using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SteamLobbyManager : MonoBehaviour
{
    public static SteamLobbyManager Instance;

    public Lobby? CurrentLobby;

    // Events (giống Unity Lobby pattern)
    public Action<Lobby> OnLobbyCreated;
    public Action<Lobby> OnLobbyJoined;
    public Action OnLobbyLeft;
    public Action<List<Lobby>> OnLobbyListUpdated;

    private void Awake()
    {
        try
        {
            SteamClient.Init(480);
            Debug.Log("Steam OK: " + SteamClient.Name);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Steam fail: " + e.Message);
        }

        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SteamMatchmaking.OnLobbyEntered += HandleLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += HandleMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += HandleMemberLeft;

        _ = ListLobbies();
    }

    private void Update()
    {
        SteamClient.RunCallbacks();
    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyEntered -= HandleLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= HandleMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= HandleMemberLeft;
    }

    // ================= CREATE =================
    public async Task CreateLobby(int maxMembers = 4)
    {
        var lobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);

        if (!lobby.HasValue)
        {
            Debug.LogError("Create lobby failed");
            return;
        }

        CurrentLobby = lobby;

        lobby.Value.SetPublic();

        // QUAN TRỌNG: lưu host
        lobby.Value.SetData("HostAddress", SteamClient.SteamId.ToString());

        Debug.Log("Lobby created: " + lobby.Value.Id);

        OnLobbyCreated?.Invoke(lobby.Value);
    }

    // ================= LIST =================
    public async Task ListLobbies()
    {
        Debug.Log("Requesting lobby list...");

        var lobbies = await SteamMatchmaking.LobbyList
            .WithMaxResults(20)
            .RequestAsync();

        var list = new List<Lobby>(lobbies);

        Debug.Log("Found " + list.Count + " lobbies");

        OnLobbyListUpdated?.Invoke(list);
    }

    // ================= JOIN =================
    public async Task JoinLobby(ulong lobbyId)
    {
        await SteamMatchmaking.JoinLobbyAsync(lobbyId);
    }

    private void HandleLobbyEntered(Lobby lobby)
    {
        CurrentLobby = lobby;

        Debug.Log("Joined lobby: " + lobby.Id);

        OnLobbyJoined?.Invoke(lobby);
    }

    // ================= LEAVE =================
    public void LeaveLobby()
    {
        if (CurrentLobby.HasValue)
        {
            CurrentLobby.Value.Leave();
            CurrentLobby = null;

            Debug.Log("Left lobby");

            OnLobbyLeft?.Invoke();
        }
    }

    // ================= MEMBER EVENTS =================
    private void HandleMemberJoined(Lobby lobby, Friend friend)
    {
        Debug.Log($"Player joined: {friend.Name}");
    }

    private void HandleMemberLeft(Lobby lobby, Friend friend)
    {
        Debug.Log($"Player left: {friend.Name}");
    }

    // ================= GET HOST =================
    public ulong GetHostSteamId()
    {
        if (!CurrentLobby.HasValue) return 0;

        string hostId = CurrentLobby.Value.GetData("HostAddress");

        return ulong.Parse(hostId);
    }
}