using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    public event EventHandler OnLobbyTaskStarted;
    public event EventHandler OnLobbyTaskCompleted;
    public event EventHandler OnLobbyError;

    public event EventHandler<OnLobbyCreatedEventArgs> OnLobbyCreated;
    public class OnLobbyCreatedEventArgs : EventArgs
    {
        public Lobby hostLobby;
    }
    public event EventHandler<OnLobbyDataChangedEventArgs> OnLobbyDataChanged;
    public class OnLobbyDataChangedEventArgs : EventArgs
    {
        public Lobby lobby;
    }

    public event EventHandler<OnListLobbiesChangedEventArgs> OnListLobbiesChanged;
    public class OnListLobbiesChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbies;
    }

    private Lobby joinedLobby;

    private List<Lobby> currentLobbies;
    private float heartbeatTimer;
    private float lobbyUpdateTimer;
    private string playerName;

    private void Awake()
    {
        Instance = this;
        playerName = "duy" + UnityEngine.Random.Range(10, 99);
        Debug.Log("Player Name: " + playerName);
        SetPlayerName(playerName);
    }

    private async void Start()
    {
        OnLobbyTaskStarted?.Invoke(this, EventArgs.Empty);
        await UnityServices.InitializeAsync();

        if (AuthenticationService.Instance.IsSignedIn)
        {
            //playerName = SceneLoader.playerName;
            OnLobbyTaskCompleted?.Invoke(this, EventArgs.Empty);
            return;
        }

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Sign In " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        OnLobbyTaskCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        HandleLobbyHeartbeat();
        HandleLobbyUpdate();
    }


    public async void CreateLobby(string lobbyName, int maxPlayer)
    {
        try
        {
            Debug.Log("Start Create Lobby");

            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer()
            };

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayer, createLobbyOptions);

            NetworkManager.Singleton.StartHost();

            OnLobbyCreated?.Invoke(this, new OnLobbyCreatedEventArgs
            {
                hostLobby = joinedLobby
            });
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public async Task<Lobby> CreateLobbyWithRelay(string lobbyName, int maxPlayers)
    {
        try
        {
            OnLobbyTaskStarted?.Invoke(this, EventArgs.Empty);

            string region = await GetNearestRegion();
            Debug.Log($"Sử dụng region: {region}");

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers, region);
            Debug.Log(allocation.GetType());

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = GetPlayer(),
                Data = new Dictionary<string, DataObject>
            {
                { "RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
            }
            };

            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

            // Cấu hình Unity Transport với Relay
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // Start host
            NetworkManager.Singleton.StartHost();

            OnLobbyCreated?.Invoke(this, new OnLobbyCreatedEventArgs
            {
                hostLobby = joinedLobby
            });

            OnLobbyTaskCompleted?.Invoke(this, EventArgs.Empty);

            return joinedLobby;
        }
        catch (Exception e)
        {
            OnLobbyError?.Invoke(this, EventArgs.Empty);
            Debug.LogError($"Error creating lobby with relay: {e}");
            return null;
        }
    }

    public async Task<List<Lobby>> ListLobbies()
    {
        try
        {
            OnLobbyTaskStarted?.Invoke(this, EventArgs.Empty);

            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter>
            {
                new QueryFilter(
                    QueryFilter.FieldOptions.IsLocked,
                    "0",
                    QueryFilter.OpOptions.EQ
                )
            }
            };

            QueryResponse queryResponse =
                await LobbyService.Instance.QueryLobbiesAsync(options);

            currentLobbies = queryResponse.Results;

            OnListLobbiesChanged?.Invoke(this, new OnListLobbiesChangedEventArgs
            {
                lobbies = currentLobbies
            });

            OnLobbyTaskCompleted?.Invoke(this, EventArgs.Empty);

            return currentLobbies;
        }
        catch (LobbyServiceException e)
        {
            OnLobbyError?.Invoke(this, EventArgs.Empty);
            Debug.LogException(e);
            return null;
        }
    }

    public async void JoinLobbyById(string id)
    {
        try
        {
            OnLobbyTaskStarted?.Invoke(this, EventArgs.Empty);


            //QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync();

            //JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            //{
            //    Player = GetPlayer()
            //};


            joinedLobby = await JoinLobbyByIdRelay(id);

            //joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, joinLobbyByIdOptions);
            //OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
            //{
            //    lobby = joinedLobby
            //});
            //NetworkManager.Singleton.StartClient();



            OnLobbyTaskCompleted?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException e)
        {
            OnLobbyError?.Invoke(this, EventArgs.Empty);
            Debug.LogException(e);
        }
    }


    public async Task<Lobby> JoinLobbyByIdRelay(string id)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = GetPlayer()
            };
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, joinLobbyByIdOptions);
            await JoinRelayFromLobby();
            OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
            {
                lobby = joinedLobby
            });
            return joinedLobby;
        }
        catch (Exception)
        {
            await ListLobbies();
            OnLobbyError?.Invoke(this, EventArgs.Empty);
            return null;
        }
    }


    private async Task JoinRelayFromLobby()
    {
        try
        {
            string joinCode = joinedLobby.Data["RelayJoinCode"].Value;
            Debug.Log($"Đang join Relay với code: {joinCode}");


            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            // Cấu hình Unity Transport
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            // Start client
            NetworkManager.Singleton.StartClient();
            Debug.Log("Đã kết nối client qua Relay");
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi join Relay: {e}");
            OnLobbyError?.Invoke(this, EventArgs.Empty);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            OnLobbyTaskStarted?.Invoke(this, EventArgs.Empty);

            if (NetworkManager.Singleton.IsHost)
            {
                await DeleteLobby();
            }
            else
            {
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            joinedLobby = null;
            NetworkManager.Singleton.Shutdown();
            OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
            {
                lobby = null
            });

            OnLobbyTaskCompleted?.Invoke(this, EventArgs.Empty);
        }
        catch (LobbyServiceException e)
        {
            OnLobbyError?.Invoke(this, EventArgs.Empty);
            Debug.LogException(e);
        }
    }

    public async Task DeleteLobby()
    {
        if (joinedLobby == null || !NetworkManager.Singleton.IsHost) return;

        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
            Debug.Log("Đã xóa lobby");
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi xóa lobby: {e}");
        }
    }


    public async void LockLobby()
    {
        if (joinedLobby != null)
        {
            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
            {
                IsLocked = true
            });
        }
    }

    private async void HandleLobbyHeartbeat()
    {
        if (joinedLobby != null && NetworkManager.Singleton.IsHost)
        {
            heartbeatTimer -= Time.deltaTime;
            if (heartbeatTimer < 0)
            {
                heartbeatTimer = 15;

                await LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private async void HandleLobbyUpdate()
    {
        if (joinedLobby == null) return;
        try
        {
            lobbyUpdateTimer -= Time.deltaTime;
            if (lobbyUpdateTimer < 0)
            {
                lobbyUpdateTimer = 5f;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
                {
                    lobby = joinedLobby
                });
            }
        }
        catch (Exception)
        {
            joinedLobby = null;
            OnLobbyDataChanged?.Invoke(this, new OnLobbyDataChangedEventArgs
            {
                lobby = joinedLobby
            });
            //Debug.LogError("Lỗi Lobby Update " + ex);
        }
    }

    private async Task<string> GetNearestRegion()
    {
        try
        {
            var regions = await RelayService.Instance.ListRegionsAsync();

            if (regions == null || regions.Count == 0)
            {
                Debug.LogWarning("Không tìm thấy regions, sử dụng region mặc định");
                return null;
            }

            string selectedRegion = regions[0].Id;
            Debug.Log($"Regions có sẵn: {string.Join(", ", regions.Select(r => r.Id))}");
            return selectedRegion;
        }
        catch (Exception e)
        {
            Debug.LogError($"Lỗi lấy regions: {e}");
            return null;
        }
    }

    public List<Lobby> GetCurrentLobbies()
    {
        return currentLobbies;
    }

    public Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public void SetPlayerName(string playerName)
    {
        this.playerName = playerName;
    }
}
