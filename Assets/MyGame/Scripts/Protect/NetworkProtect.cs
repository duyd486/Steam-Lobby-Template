using Unity.Netcode;
using UnityEngine;

public class NetworkProtect : MonoBehaviour
{
    void Awake()
    {
        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.gameObject != gameObject)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
