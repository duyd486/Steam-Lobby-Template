using Unity.Netcode;
using UnityEngine;

public class SteamLobbyProtect : MonoBehaviour
{
    private void Awake()
    {
        if (SteamLobbyManager.Instance != null && SteamLobbyManager.Instance.gameObject != gameObject)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
