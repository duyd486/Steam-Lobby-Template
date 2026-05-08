using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<GameObject> activePlayers;

    //private void Awake()
    //{
    //    NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadCompleted;
    //}

    //private void OnDestroy()
    //{
    //    NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnLoadCompleted;
    //}

    //private void OnLoadCompleted(
    //    string sceneName,
    //    LoadSceneMode mode,
    //    List<ulong> clientsCompleted,
    //    List<ulong> clientsTimedOut)
    //{
    //    if (!NetworkManager.Singleton.IsServer)
    //        return;

    //    foreach (ulong clientId in clientsCompleted)
    //    {
    //        SpawnPlayerForClient(clientId);
    //    }
    //}

    private void Update()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnPlayers();
        }
    }

    private void SpawnPlayers()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Debug.Log("Spawning player for clientId: " + clientId);
            SpawnPlayerForClient(clientId);
        }
    }

    private void SpawnPlayerForClient(ulong clientId)
    {
        GameObject playerInstance = Instantiate(playerPrefab, GetRandomSpawnPosition(), Quaternion.identity);
        activePlayers.Add(playerInstance);
        NetworkObject networkObject = playerInstance.GetComponent<NetworkObject>();
        networkObject.SpawnAsPlayerObject(clientId);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(1f, 6f);
        return new Vector3(x, 3f, z);
    }
}
