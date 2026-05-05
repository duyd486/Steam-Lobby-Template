using Steamworks;
using UnityEngine;

public class SteamLobbyManager : MonoBehaviour
{
    private void Start()
    {
        try
        {
            SteamClient.Init(480);
            Debug.Log("Steam Initialized!");
            Debug.Log("Name: " + SteamClient.Name);
            Debug.Log("SteamID: " + SteamClient.SteamId);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Steam fail: " + e.Message);
        }
    }
    void Update()
    {
        SteamClient.RunCallbacks();
    }
    void OnApplicationQuit()
    {
        SteamClient.Shutdown();
    }
}
