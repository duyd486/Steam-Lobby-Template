using UnityEngine;

public class GlobalUI : MonoBehaviour
{
    [SerializeField] private NotificationEventChannelSO notificationEventChannel;

    public static GlobalUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        notificationEventChannel.OnEventRaised += NotificationEventChannel_OnEventRaised;
    }

    private void OnDestroy()
    {
        notificationEventChannel.OnEventRaised -= NotificationEventChannel_OnEventRaised;
    }

    private void NotificationEventChannel_OnEventRaised(NotificationData data)
    {
        Debug.Log($"GlobalUI received notification: {data.Message}");
    }
}
