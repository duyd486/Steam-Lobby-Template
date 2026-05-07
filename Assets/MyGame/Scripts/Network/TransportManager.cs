using System.Net;
using System.Net.Sockets;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
[RequireComponent(typeof(UnityTransport))]
public class TransportManager : MonoBehaviour
{
    public static TransportManager Instance { get; private set; }

    [Header("Config")]
    [SerializeField] private NotificationEventChannelSO notificationEventChannelSO;
    [SerializeField] private ushort defaultPort = 7777;

    public bool IsOnline =>
        networkManager != null &&
        networkManager.IsListening;

    private NetworkManager networkManager;
    private UnityTransport unityTransport;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        networkManager = GetComponent<NetworkManager>();

        unityTransport = GetComponent<UnityTransport>();
    }

    // ================= HOST =================

    public bool StartHost()
    {
        if (networkManager.IsListening)
        {
            notificationEventChannelSO.Raise(new NotificationData
            {
                Message = "Network already running",
                IsError = true
            });
            return false;
        }

        unityTransport.SetConnectionData(
            "0.0.0.0",
            defaultPort
        );

        bool success = networkManager.StartHost();

        notificationEventChannelSO.Raise(new NotificationData
        {
            Message = success ? "Host started" : "Failed to start host",
            IsError = !success
        });

        return success;
    }

    // ================= CLIENT =================

    public bool StartClient(string ipAddress)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                notificationEventChannelSO.Raise(new NotificationData
                {
                    Message = "FFailed to start client! Maybe you are in lobby of another game!",
                    IsError = true
                });
                return false;
            }

            if (networkManager.IsListening)
            {
                notificationEventChannelSO.Raise(new NotificationData
                {
                    Message = "Network already running",
                    IsError = true
                });
                return false;
            }

            unityTransport.SetConnectionData(
                ipAddress,
                defaultPort
            );

            bool success = networkManager.StartClient();

            notificationEventChannelSO.Raise(new NotificationData
            {
                Message = "Client started at: " + ipAddress,
                IsError = false
            });

            return success;
        }
        catch
        {
            notificationEventChannelSO.Raise(new NotificationData
            {
                Message = "FFailed to start client! Maybe you are in lobby of another game!",
                IsError = true
            });
            return false;
        }
    }

    // ================= SHUTDOWN =================

    public void Shutdown()
    {
        if (!networkManager.IsListening)
            return;

        networkManager.Shutdown();

        Debug.Log("Network shutdown");
    }

    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }

        return "127.0.0.1";
    }
}
