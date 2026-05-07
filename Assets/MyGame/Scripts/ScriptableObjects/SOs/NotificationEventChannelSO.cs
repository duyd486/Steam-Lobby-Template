using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Notification Event")]
public class NotificationEventChannelSO : ScriptableObject
{
    public Action<NotificationData> OnEventRaised;

    public void Raise(NotificationData data)
    {
        OnEventRaised?.Invoke(data);
    }
}

[Serializable]
public class NotificationData
{
    public string Message;
    public bool IsError;
}
