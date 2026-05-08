using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerCamera : NetworkBehaviour
{
    [SerializeField] private Transform cameraContainer;

    private CinemachineCamera cinemachineCamera;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();

        if (cinemachineCamera != null)
        {
            //cinemachineCamera = FindFirstObjectByType<CinemachineCamera>();
            cinemachineCamera.Follow = transform;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        Camera.main.transform.SetParent(null);
    }
}
