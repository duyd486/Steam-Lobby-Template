using UnityEngine;

public class GlobalUI : MonoBehaviour
{
    public static GlobalUI Instance { get; private set; }

    [SerializeField] private NotificationEventChannelSO notificationEventChannel;
    [SerializeField] private GameObject notificationContainer;
    [SerializeField] private NotificationSingleUI notificationSingleUI;

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
        NotificationSingleUI notificationSingleUITmp = Instantiate(notificationSingleUI, notificationContainer.transform);
        notificationSingleUITmp.SetNotification(data.Message, data.IsError);
    }
}
