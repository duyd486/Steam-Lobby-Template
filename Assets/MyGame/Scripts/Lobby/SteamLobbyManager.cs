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
    public bool IsInitialized { get; private set; }

    // Events (giống Unity Lobby pattern)
    public Action OnSteamInitDone;
    public Action<Lobby> OnLobbyCreated;
    public Action<Lobby> OnLobbyJoined;
    public Action<Lobby> OnLobbyUpdated;
    public Action OnLobbyLeft;
    public Action<List<Lobby>> OnLobbyListUpdated;

    public Action OnLobbyTaskStarted;
    public Action OnLobbyTaskCompleted;
    public Action OnLobbyError;

    private void Awake()
    {
        try
        {
            SteamClient.Init(480);
            Debug.Log("Steam OK: " + SteamClient.Name);
            IsInitialized = true;
            OnSteamInitDone?.Invoke();
        }
        catch (System.Exception e)
        {
            IsInitialized = false;
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
        SteamMatchmaking.OnLobbyDataChanged += HandleLobbyDataChanged;

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

    private void HandleLobbyDataChanged(Lobby lobby)
    {
        Debug.Log("Lobby data changed: " + lobby.Id);

        if (!CurrentLobby.HasValue || lobby.Id != CurrentLobby.Value.Id) return;

        CurrentLobby = lobby;

        OnLobbyUpdated?.Invoke(CurrentLobby.Value);
    }

    // ================= CREATE =================
    public async Task CreateLobby(int maxMembers = 4)
    {
        OnLobbyTaskStarted?.Invoke();

        var lobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);

        if (!lobby.HasValue)
        {
            Debug.LogError("Create lobby failed");
            return;
        }

        CurrentLobby = lobby;

        lobby.Value.SetPublic();
        lobby.Value.SetData("name", SteamClient.Name + "'s Room");

        // QUAN TRỌNG: lưu host
        lobby.Value.SetData("HostAddress", SteamClient.SteamId.ToString());

        Debug.Log("Lobby created: " + lobby.Value.Id);

        OnLobbyCreated?.Invoke(lobby.Value);

        OnLobbyTaskCompleted?.Invoke();
    }

    // ================= LIST =================
    public async Task ListLobbies()
    {
        OnLobbyTaskStarted?.Invoke();

        Debug.Log("Requesting lobby list...");

        var lobbies = await SteamMatchmaking.LobbyList
            .WithMaxResults(20)
            .RequestAsync();

        var list = new List<Lobby>(lobbies);

        Debug.Log("Found " + list.Count + " lobbies");

        OnLobbyListUpdated?.Invoke(list);

        OnLobbyTaskCompleted?.Invoke();
    }

    // ================= JOIN =================
    public async Task JoinLobby(ulong lobbyId)
    {
        OnLobbyTaskStarted?.Invoke();

        await SteamMatchmaking.JoinLobbyAsync(lobbyId);

        OnLobbyTaskCompleted?.Invoke();
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
        OnLobbyTaskStarted?.Invoke();

        if (CurrentLobby.HasValue)
        {
            CurrentLobby.Value.Leave();
            CurrentLobby = null;

            Debug.Log("Left lobby");

            OnLobbyLeft?.Invoke();
        }

        OnLobbyTaskCompleted?.Invoke();
    }

    // ================= MEMBER EVENTS =================
    private void HandleMemberJoined(Lobby lobby, Friend friend)
    {
        Debug.Log($"Member joined: {friend.Name} ({friend.Id})");

        if (!CurrentLobby.HasValue || lobby.Id != CurrentLobby.Value.Id) return;

        CurrentLobby = lobby;

        OnLobbyUpdated?.Invoke(CurrentLobby.Value);
    }

    private void HandleMemberLeft(Lobby lobby, Friend friend)
    {
        Debug.Log($"Member left: {friend.Name} ({friend.Id})");

        if (!CurrentLobby.HasValue || lobby.Id != CurrentLobby.Value.Id) return;

        CurrentLobby = lobby;

        OnLobbyUpdated?.Invoke(CurrentLobby.Value);
    }

    // ================= GET HOST =================
    public ulong GetHostSteamId()
    {
        if (!CurrentLobby.HasValue) return 0;

        string hostId = CurrentLobby.Value.GetData("HostAddress");

        return ulong.Parse(hostId);
    }


    public string GetPlayerName()
    {
        return SteamClient.Name;
    }
}